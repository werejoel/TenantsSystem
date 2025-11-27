using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TenantsManagementApp.Data;
using TenantsManagementApp.Models;
using TenantsManagementApp.Models.FlutterWave;

namespace TenantsManagementApp.Services;

public class FlutterwaveService
{
    private readonly HttpClient _httpClient;
    private readonly string _secretKey;
    private readonly string _webhookSecret;
    private readonly ApplicationDbContext _dbContext;

    public FlutterwaveService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ApplicationDbContext dbContext)
    {
        _httpClient = httpClientFactory.CreateClient();
        _secretKey = configuration["Flutterwave:SecretKey"];
        _webhookSecret = configuration["Flutterwave:WebhookSecret"];
        _dbContext = dbContext;
        
        Console.WriteLine($"[FlutterwaveService] Initializing with SecretKey: {(_secretKey?.Substring(0, Math.Min(10, _secretKey?.Length ?? 0)) ?? "NULL")}...");
        
        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _secretKey);
    }
    public async Task<ChargeResponse> InitiatePaymentAsync(InitiatePaymentRequest request)
    {
        var tenant = await _dbContext.Tenants
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Id == request.TenantId && t.IsActive);
        if (tenant == null) throw new Exception("Tenant not found or inactive");

        var house = await _dbContext.Houses.FindAsync(request.HouseId);
        if (house == null) throw new Exception("House not found");

        // Validate charges
        var charges = await _dbContext.Charges
            .Where(c => request.ChargeIds.Contains(c.Id) && c.TenantId == request.TenantId && c.Status != "Paid")
            .ToListAsync();
        if (!charges.Any()) throw new Exception("No valid charges selected");

        // determine network string expected by Flutterwave
        string network = "MTN";
        if (!string.IsNullOrEmpty(request.Provider))
        {
            var p = request.Provider.Trim().ToLowerInvariant();
            if (p == "airtel" || p == "airtelug") network = "AIRTEL";
            else if (p == "mtn" || p == "mtnug") network = "MTN";
            else network = p.ToUpperInvariant();
        }

        var txRef = Guid.NewGuid().ToString();
        var payload = new
        {
            phone_number = tenant.User.PhoneNumber,
            network = network,
            amount = request.AmountPaid,
            currency = "UGX",
            email = tenant.User.Email,
            tx_ref = txRef,
            redirect_url = request.RedirectUrl,
            meta = new
            {
                tenant_id = request.TenantId,
                house_id = request.HouseId,
                purpose = request.Purpose,
                provider = request.Provider,
                period_start = request.PeriodStart?.ToString("o"),
                period_end = request.PeriodEnd?.ToString("o"),
                charge_ids = request.ChargeIds
            }
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync(
                "https://api.flutterwave.com/v3/charges?type=mobile_money_uganda", content);

            var result = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[FlutterwaveService] HTTP Status Code: {response.StatusCode}");
            Console.WriteLine($"[FlutterwaveService] API Response Body: {result}");

            var chargeResponse = JsonSerializer.Deserialize<ChargeResponse>(
                result, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (chargeResponse?.Status == "success")
            {
                // NOTE: Payment record should be created by PaymentService, NOT here
                // Only create PaymentTransaction for audit trail
                
                // Save PaymentTransaction record for audit trail (with provisional data)
                var txn = new PaymentTransaction
                {
                    TxRef = chargeResponse.Data.TxRef,
                    FlwRef = chargeResponse.Data.FlwRef ?? "pending",
                    Amount = request.AmountPaid,
                    Currency = "UGX",
                    Status = chargeResponse.Status,
                    Provider = request.Provider ?? "MTN",
                    TenantId = request.TenantId,
                    PaymentId = null, // Will be set later after Payment is created
                    CustomerName = tenant.User?.FullName,
                    CustomerEmail = tenant.User?.Email,
                    CustomerPhone = tenant.User?.PhoneNumber,
                    PaymentDate = DateTime.UtcNow,
                    Verified = false,
                    RawResponse = JsonSerializer.Serialize(chargeResponse)
                };
                _dbContext.PaymentTransactions.Add(txn);
                await _dbContext.SaveChangesAsync();
                
                Console.WriteLine($"[FlutterwaveService] PaymentTransaction created: {txn.Id}");
            }
            else
            {
                Console.WriteLine($"[FlutterwaveService] API returned non-success status: {chargeResponse?.Status}, Message: {chargeResponse?.Message}");
            }

            return chargeResponse;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[FlutterwaveService] Exception in InitiatePaymentAsync: {ex}");
            throw;
        }
    }

    public async Task<VerificationResponse> VerifyTransactionAsync(string transactionId)
    {
        var response = await _httpClient.GetAsync(
            $"https://api.flutterwave.com/v3/transactions/{transactionId}/verify");

        if (!response.IsSuccessStatusCode) return null;

        var result = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<VerificationResponse>(
            result, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    public async Task<RefundResponse> InitiateRefundAsync(string transactionId, decimal? amount = null)
    {
        var payload = new { amount = amount?.ToString() };
        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(
            $"https://api.flutterwave.com/v3/transactions/{transactionId}/refund", content);

        var result = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<RefundResponse>(
            result, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    public bool ValidateWebhookSignature(string payload, string signature)
    {
        using var hmac = new System.Security.Cryptography.HMACSHA256(Encoding.UTF8.GetBytes(_webhookSecret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        var computedSignature = Convert.ToBase64String(hash);
        return computedSignature == signature;
    }
}
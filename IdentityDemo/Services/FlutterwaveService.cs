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

        var payload = new
        {
            phone_number = tenant.User.PhoneNumber,
            network = "MTN",
            amount = request.AmountPaid,
            currency = "UGX",
            email = tenant.User.Email,
            tx_ref = Guid.NewGuid().ToString(),
            redirect_url = request.RedirectUrl,
            meta = new
            {
                tenant_id = request.TenantId,
                house_id = request.HouseId,
                purpose = request.Purpose,
                period_start = request.PeriodStart?.ToString("o"),
                period_end = request.PeriodEnd?.ToString("o"),
                charge_ids = request.ChargeIds
            }
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(
            "https://api.flutterwave.com/v3/charges?type=mobile_money_uganda", content);

        var result = await response.Content.ReadAsStringAsync();
        var chargeResponse = JsonSerializer.Deserialize<ChargeResponse>(
            result, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (chargeResponse?.Status == "success")
        {
            // Save payment record
            var payment = new Payment
            {
                TenantId = request.TenantId,
                HouseId = request.HouseId,
                AmountPaid = request.AmountPaid,
                PaymentDate = request.PaymentDate,
                PaymentMethod = "MTN Mobile Money",
                TransactionReference = chargeResponse.Data.TxRef,
                PeriodStart = request.PeriodStart,
                PeriodEnd = request.PeriodEnd,
                Notes = request.Notes
            };
            _dbContext.Payments.Add(payment);
            await _dbContext.SaveChangesAsync();

            // Link payment to charges
            foreach (var chargeId in request.ChargeIds)
            {
                var charge = charges.FirstOrDefault(c => c.Id == chargeId);
                if (charge != null)
                {
                    var paymentCharge = new PaymentCharge
                    {
                        PaymentId = payment.Id,
                        ChargeId = chargeId,
                        AmountPaid = Math.Min(charge.OutstandingAmount, request.AmountPaid)
                    };
                    _dbContext.PaymentCharges.Add(paymentCharge);
                    request.AmountPaid -= paymentCharge.AmountPaid; // Reduce remaining amount
                    if (charge.OutstandingAmount <= 0) charge.Status = "Paid";
                }
            }
            await _dbContext.SaveChangesAsync();
        }

        return chargeResponse;
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
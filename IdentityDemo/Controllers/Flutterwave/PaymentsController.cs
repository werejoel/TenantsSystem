using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TenantsManagementApp.Data;
using TenantsManagementApp.Models;
using TenantsManagementApp.Models.FlutterWave;
using TenantsManagementApp.Services;

namespace TenantsManagementApp.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Require authentication
public class FlutterwavePaymentsController : Controller
{
    private readonly FlutterwaveService _flutterwaveService;
    private readonly ApplicationDbContext _dbContext;
    private readonly INotificationService _notificationService;
    private readonly ILogger<FlutterwavePaymentsController> _logger;

    public FlutterwavePaymentsController(
        FlutterwaveService flutterwaveService,
        ApplicationDbContext dbContext,
        INotificationService notificationService,
        ILogger<FlutterwavePaymentsController> logger)
    {
        _flutterwaveService = flutterwaveService;
        _dbContext = dbContext;
        _notificationService = notificationService;
        _logger = logger;
    }

    // POST: Initiate Payment
    [HttpPost("initiate")]
    [Authorize(Policy = "AddUserPolicy")]
    public async Task<IActionResult> InitiatePayment([FromBody] InitiatePaymentRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            _logger.LogWarning("Invalid input for payment initiation: {Errors}", string.Join(", ", errors));
            return BadRequest(new { error = "Invalid input.", details = errors });
        }

        if (request.AmountPaid < 100)
        {
            _logger.LogWarning("Payment amount {Amount} is less than minimum 100 UGX", request.AmountPaid);
            return BadRequest(new { error = "Amount must be at least 100 UGX." });
        }

        if (request.PaymentDate < DateTime.UtcNow.AddDays(-1))
        {
            _logger.LogWarning("Payment date {PaymentDate} is in the past", request.PaymentDate);
            return BadRequest(new { error = "Payment date cannot be in the past." });
        }

        var tenant = await _dbContext.Tenants
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Id == request.TenantId && t.IsActive);
        if (tenant == null || !tenant.User.PhoneNumber.StartsWith("256"))
        {
            _logger.LogWarning("Tenant {TenantId} not found or invalid phone number", request.TenantId);
            return BadRequest(new { error = "Invalid tenant or phone number (must start with 256)." });
        }

        var house = await _dbContext.Houses.FindAsync(request.HouseId);
        if (house == null)
        {
            _logger.LogWarning("House {HouseId} not found", request.HouseId);
            return BadRequest(new { error = "House not found." });
        }

        var charges = await _dbContext.Charges
            .Where(c => request.ChargeIds.Contains(c.Id) && c.TenantId == request.TenantId && c.Status != "Paid")
            .ToListAsync();
        if (!charges.Any())
        {
            _logger.LogWarning("No valid charges selected for payment by tenant {TenantId}", request.TenantId);
            return BadRequest(new { error = "No valid charges selected." });
        }

        try
        {
            var response = await _flutterwaveService.InitiatePaymentAsync(request);
            if (response?.Status != "success" || response.Meta?.Authorization?.Mode != "redirect")
            {
                _logger.LogError("Failed to initiate payment: {Message}", response?.Message ?? "Unknown error");
                return BadRequest(new { error = response?.Message ?? "Failed to initiate payment" });
            }

            _logger.LogInformation("Payment initiated for tenant {TenantId}, tx_ref: {TxRef}", request.TenantId, response.Data.TxRef);
            return Ok(new
            {
                redirectUrl = response.Meta.Authorization.Redirect,
                txRef = response.Data.TxRef,
                flwRef = response.Data.FlwRef
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Payment initiation failed for tenant {TenantId}", request.TenantId);
            return StatusCode(500, new { error = $"Payment initiation failed: {ex.Message}" });
        }
    }

    // POST: Webhook Handler
    [HttpPost("webhook")]
    [AllowAnonymous] 
    public async Task<IActionResult> HandleWebhook()
    {
        // Validate signature
        var signature = Request.Headers["verif-hash"].ToString();
        using var reader = new StreamReader(Request.Body);
        var payload = await reader.ReadToEndAsync();
        if (!string.IsNullOrEmpty(signature) && !_flutterwaveService.ValidateWebhookSignature(payload, signature))
        {
            _logger.LogWarning("Invalid webhook signature received");
            return Unauthorized(new { error = "Invalid webhook signature" });
        }

        var webhookData = JsonSerializer.Deserialize<WebhookData>(
            payload, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (webhookData?.Event != "charge.completed")
        {
            _logger.LogInformation("Ignoring webhook event: {Event}", webhookData?.Event);
            return Ok();
        }

        var payment = await _dbContext.Payments
            .Include(p => p.PaymentCharges)
            .ThenInclude(pc => pc.Charge)
            .FirstOrDefaultAsync(p => p.TransactionReference == webhookData.Data.TxRef);
        if (payment == null)
        {
            _logger.LogWarning("Payment not found for tx_ref: {TxRef}", webhookData.Data.TxRef);
            return BadRequest(new { error = "Payment not found" });
        }

        var verification = await _flutterwaveService.VerifyTransactionAsync(webhookData.Data.Id.ToString());
        if (verification?.Status == "success" && verification.Data.Status == "successful" &&
            verification.Data.Currency == "UGX" && verification.Data.Amount >= payment.AmountPaid)
        {
            payment.TransactionReference = verification.Data.FlwRef;

            // Update charge statuses
            foreach (var pc in payment.PaymentCharges)
            {
                if (pc.Charge.OutstandingAmount <= 0)
                {
                    pc.Charge.Status = "Paid";
                }
            }
            await _dbContext.SaveChangesAsync();

            // Notify tenant
            try
            {
                var tenant = await _dbContext.Tenants
                    .Include(t => t.User)
                    .FirstOrDefaultAsync(t => t.Id == payment.TenantId);
                if (tenant != null)
                {
                    await _notificationService.SendNotificationAsync(
                        tenant.UserId,
                        $"Payment of {payment.AmountPaid:C} completed successfully.",
                        "Payment Confirmation"
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send notification for payment {PaymentId}", payment.Id);
            }

            _logger.LogInformation("Payment completed for tx_ref: {TxRef}", webhookData.Data.TxRef);
            return Ok(new { status = "success" });
        }

        _logger.LogWarning("Transaction verification failed for tx_ref: {TxRef}", webhookData.Data.TxRef);
        return BadRequest(new { error = "Transaction verification failed" });
    }

    // GET: Verify Transaction
    [HttpGet("verify/{transactionId}")]
    [Authorize(Policy = "ViewUsersPolicy")] 
    public async Task<IActionResult> VerifyTransaction(string transactionId)
    {
        var payment = await _dbContext.Payments
            .Include(p => p.PaymentCharges)
            .ThenInclude(pc => pc.Charge)
            .FirstOrDefaultAsync(p => p.TransactionReference == transactionId);
        if (payment == null)
        {
            _logger.LogWarning("Payment not found for transactionId: {TransactionId}", transactionId);
            return BadRequest(new { error = "Payment not found" });
        }

        var verification = await _flutterwaveService.VerifyTransactionAsync(transactionId);
        if (verification?.Status != "success" || verification.Data.Status != "successful")
        {
            _logger.LogWarning("Transaction verification failed for transactionId: {TransactionId}", transactionId);
            return BadRequest(new { error = "Transaction verification failed" });
        }

        if (verification.Data.Amount >= payment.AmountPaid && verification.Data.Currency == "UGX")
        {
            payment.TransactionReference = verification.Data.FlwRef;

            // Update charge statuses
            foreach (var pc in payment.PaymentCharges)
            {
                if (pc.Charge.OutstandingAmount <= 0)
                {
                    pc.Charge.Status = "Paid";
                }
            }
            await _dbContext.SaveChangesAsync();

            // Notify tenant
            try
            {
                var tenant = await _dbContext.Tenants
                    .Include(t => t.User)
                    .FirstOrDefaultAsync(t => t.Id == payment.TenantId);
                if (tenant != null)
                {
                    await _notificationService.SendNotificationAsync(
                        tenant.UserId,
                        $"Payment of {payment.AmountPaid:C} verified successfully.",
                        "Payment Verification"
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send notification for payment {PaymentId}", payment.Id);
            }

            _logger.LogInformation("Payment verified for transactionId: {TransactionId}", transactionId);
            return Ok(verification);
        }

        _logger.LogWarning("Transaction verification failed for transactionId: {TransactionId}", transactionId);
        return BadRequest(new { error = "Transaction verification failed" });
    }

    // POST: Initiate Refund
    [HttpPost("refund/{transactionId}")]
    [Authorize(Policy = "ManageUsersPolicy")]
    public async Task<IActionResult> InitiateRefund(string transactionId, [FromBody] RefundRequest request)
    {
        var payment = await _dbContext.Payments
            .Include(p => p.PaymentCharges)
            .ThenInclude(pc => pc.Charge)
            .FirstOrDefaultAsync(p => p.TransactionReference == transactionId);
        if (payment == null)
        {
            _logger.LogWarning("Payment not found or not completed for transactionId: {TransactionId}", transactionId);
            return BadRequest(new { error = "Payment not found or not completed" });
        }

        if (request.Amount.HasValue && request.Amount.Value > payment.AmountPaid)
        {
            _logger.LogWarning("Refund amount {Amount} exceeds payment amount {PaymentAmount}", request.Amount, payment.AmountPaid);
            return BadRequest(new { error = "Refund amount cannot exceed original payment amount" });
        }

        try
        {
            var refund = await _flutterwaveService.InitiateRefundAsync(transactionId, request.Amount);
            if (refund?.Status != "success")
            {
                _logger.LogError("Refund initiation failed for transactionId: {TransactionId}, message: {Message}", transactionId, refund?.Message);
                return BadRequest(new { error = refund?.Message ?? "Refund initiation failed" });
            }

            foreach (var pc in payment.PaymentCharges)
            {
                pc.Charge.Status = "Pending"; // Revert charge status
            }
            await _dbContext.SaveChangesAsync();

            // Notify tenant
            try
            {
                var tenant = await _dbContext.Tenants
                    .Include(t => t.User)
                    .FirstOrDefaultAsync(t => t.Id == payment.TenantId);
                if (tenant != null)
                {
                    await _notificationService.SendNotificationAsync(
                        tenant.UserId,
                        $"Refund of {refund.Data.AmountRefunded:C} for payment processed successfully.",
                        "Refund Confirmation"
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send refund notification for payment {PaymentId}", payment.Id);
            }

            _logger.LogInformation("Refund processed for transactionId: {TransactionId}", transactionId);
            return Ok(refund);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Refund initiation failed for transactionId: {TransactionId}", transactionId);
            return StatusCode(500, new { error = $"Refund initiation failed: {ex.Message}" });
        }

    }

    [HttpGet]
    [Route("/Payments/Callback")]
    public IActionResult Callback()
    {
        
        return View();
    }

}


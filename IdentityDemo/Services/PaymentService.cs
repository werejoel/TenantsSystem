using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenantsManagementApp.Data;
using TenantsManagementApp.DTOS;
using TenantsManagementApp.Models;
using TenantsManagementApp.Models.FlutterWave;

namespace TenantsManagementApp.Services;

public class PaymentService : IPaymentService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly FlutterwaveService _flutterwaveService;

    public PaymentService(ApplicationDbContext dbContext, FlutterwaveService flutterwaveService)
    {
        _dbContext = dbContext;
        _flutterwaveService = flutterwaveService;
    }

    public async Task<Payment> CreatePaymentAsync(InitiatePaymentRequest request)
    {
        try
        {
            Console.WriteLine($"[PaymentService] Creating payment for tenant {request.TenantId}, house {request.HouseId}, amount {request.AmountPaid}");
            
            var tenant = await _dbContext.Tenants
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == request.TenantId && t.IsActive);
            if (tenant == null) throw new Exception("Tenant not found or inactive");

            var house = await _dbContext.Houses.FindAsync(request.HouseId);
            if (house == null) throw new Exception("House not found");

            var charges = await _dbContext.Charges
                .Where(c => request.ChargeIds.Contains(c.Id) && c.TenantId == request.TenantId && c.Status != "Paid")
                .ToListAsync();
            if (!charges.Any()) throw new Exception("No valid charges selected");

            Console.WriteLine($"[PaymentService] Found {charges.Count} charges");

            var payment = new Payment
            {
                TenantId = request.TenantId,
                HouseId = request.HouseId,
                AmountPaid = request.AmountPaid,
                PaymentDate = request.PaymentDate,
                PaymentMethod = "MTN Mobile Money",
                PeriodStart = request.PeriodStart,
                PeriodEnd = request.PeriodEnd,
                Notes = request.Notes,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _dbContext.Payments.Add(payment);
            await _dbContext.SaveChangesAsync();
            
            Console.WriteLine($"[PaymentService] Payment record created: {payment.Id}");

            var remainingAmount = request.AmountPaid;
            foreach (var chargeId in request.ChargeIds)
            {
                var charge = charges.FirstOrDefault(c => c.Id == chargeId);
                if (charge != null)
                {
                    var amountToPay = Math.Min(charge.OutstandingAmount, remainingAmount);
                    var paymentCharge = new PaymentCharge
                    {
                        PaymentId = payment.Id,
                        ChargeId = chargeId,
                        AmountPaid = amountToPay,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    _dbContext.PaymentCharges.Add(paymentCharge);
                    remainingAmount -= amountToPay;
                    charge.UpdatedAt = DateTime.Now;
                    if (charge.OutstandingAmount <= amountToPay) 
                    {
                        charge.Status = "Paid";
                    }
                }
            }
            await _dbContext.SaveChangesAsync();
            
            Console.WriteLine($"[PaymentService] PaymentCharges created and charges updated");

            // Initiate Flutterwave payment
            Console.WriteLine($"[PaymentService] Calling FlutterwaveService.InitiatePaymentAsync");
            var response = await _flutterwaveService.InitiatePaymentAsync(request);
            
            Console.WriteLine($"[PaymentService] Flutterwave response status: {response?.Status}");
            
            if (response?.Status == "success")
            {
                payment.TransactionReference = response.Data.TxRef;
                payment.UpdatedAt = DateTime.Now;
                _dbContext.Payments.Update(payment);
                await _dbContext.SaveChangesAsync();
                Console.WriteLine($"[PaymentService] Payment transaction reference updated: {response.Data.TxRef}");
            }
            else
            {
                throw new Exception($"Flutterwave API returned non-success status: {response?.Status} - {response?.Message}");
            }

            return payment;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[PaymentService] Exception: {ex.Message}");
            Console.WriteLine($"[PaymentService] Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    public async Task<PaymentSummaryDto> GetPaymentSummaryAsync(DateTime fromDate, DateTime toDate)
    {
        return await _dbContext.GetPaymentSummaryAsync(fromDate, toDate);
    }

    public async Task<PaymentSummaryDto> GetTenantPaymentSummaryAsync(int tenantId, DateTime fromDate, DateTime toDate)
    {
        return await _dbContext.GetTenantPaymentSummaryAsync(tenantId, fromDate, toDate);
    }

    public async Task<TenantBalanceDto> GetTenantBalanceAsync(int tenantId)
    {
        return await _dbContext.GetTenantBalanceAsync(tenantId);
    }
}
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

        var payment = new Payment
        {
            TenantId = request.TenantId,
            HouseId = request.HouseId,
            AmountPaid = request.AmountPaid,
            PaymentDate = request.PaymentDate,
            PaymentMethod = "MTN Mobile Money",
            PeriodStart = request.PeriodStart,
            PeriodEnd = request.PeriodEnd,
            Notes = request.Notes
        };

        _dbContext.Payments.Add(payment);
        await _dbContext.SaveChangesAsync();

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
                request.AmountPaid -= paymentCharge.AmountPaid;
                if (charge.OutstandingAmount <= 0) charge.Status = "Paid";
            }
        }
        await _dbContext.SaveChangesAsync();

        // Initiate Flutterwave payment
        var response = await _flutterwaveService.InitiatePaymentAsync(request);
        if (response.Status == "success")
        {
            payment.TransactionReference = response.Data.TxRef;
            await _dbContext.SaveChangesAsync();
        }
        else
        {
            throw new Exception("Failed to initiate Flutterwave payment");
        }

        return payment;
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
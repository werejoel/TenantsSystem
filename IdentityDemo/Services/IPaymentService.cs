using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TenantsManagementApp.DTOS;
using TenantsManagementApp.Models;
using TenantsManagementApp.Models.FlutterWave;

namespace TenantsManagementApp.Services;

public interface IPaymentService
{
    Task<Payment> CreatePaymentAsync(InitiatePaymentRequest request);
    Task<PaymentSummaryDto> GetPaymentSummaryAsync(DateTime fromDate, DateTime toDate);
    Task<PaymentSummaryDto> GetTenantPaymentSummaryAsync(int tenantId, DateTime fromDate, DateTime toDate);
    Task<TenantBalanceDto> GetTenantBalanceAsync(int tenantId);
}
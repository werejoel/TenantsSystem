using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TenantsManagementApp.DTOS;
using TenantsManagementApp.Models;

namespace TenantsManagementApp.Services
{
    public interface ITenantService
    {
        Task<List<Tenant>> GetAllTenantsAsync();
        Task<Tenant?> GetTenantByIdAsync(int id);
        Task<Tenant?> GetTenantByUserIdAsync(Guid userId);
        Task<Tenant> CreateTenantAsync(Tenant tenant);
        Task<Tenant?> UpdateTenantAsync(Tenant tenant);
        Task<bool> DeleteTenantAsync(int id);
        Task<bool> AssignToHouseAsync(int tenantId, int houseId);
        Task<TenantBalanceDto?> GetTenantBalanceAsync(int tenantId);
        Task<PaymentSummaryDto?> GetPaymentSummaryAsync(int tenantId, DateTime fromDate, DateTime toDate);
    }
}
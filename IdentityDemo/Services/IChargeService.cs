using TenantsManagementApp.Models;

namespace TenantsManagementApp.Services
{
    public interface IChargeService
    {
        Task<List<Charge>> GetChargesByTenantAsync(int tenantId);
        Task<Charge> CreateChargeAsync(Charge charge);
        Task<List<Charge>> GetOverdueChargesAsync();
        Task<int> CreateMonthlyRentChargesAsync(DateTime forMonth);
        Task<bool> MarkChargeAsPaidAsync(int chargeId);
    }
}

using TenantsManagementApp.Models;

namespace TenantsManagementApp.Services
{
    public interface IMaintenanceService
    {
        Task<List<MaintenanceRequest>> GetMaintenanceRequestsAsync();
        Task<MaintenanceRequest> CreateMaintenanceRequestAsync(MaintenanceRequest request);
        Task<MaintenanceRequest?> UpdateMaintenanceRequestAsync(MaintenanceRequest request);
        Task<List<MaintenanceRequest>> GetRequestsByTenantAsync(int tenantId);
        Task<List<MaintenanceRequest>> GetPendingRequestsAsync();
    }
}

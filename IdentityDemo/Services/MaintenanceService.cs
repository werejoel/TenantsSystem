using Microsoft.EntityFrameworkCore;
using TenantsManagementApp.Data;
using TenantsManagementApp.Models;

namespace TenantsManagementApp.Services
{
    public class MaintenanceService : IMaintenanceService
    {
        private readonly ApplicationDbContext _context;

        public MaintenanceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<MaintenanceRequest>> GetMaintenanceRequestsAsync()
        {
            return await _context.MaintenanceRequests
                .Include(mr => mr.Tenant)
                .ThenInclude(t => t.User)
                .Include(mr => mr.House)
                .OrderByDescending(mr => mr.RequestedAt)
                .ToListAsync();
        }

        public async Task<MaintenanceRequest> CreateMaintenanceRequestAsync(MaintenanceRequest request)
        {
            _context.MaintenanceRequests.Add(request);
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task<MaintenanceRequest?> UpdateMaintenanceRequestAsync(MaintenanceRequest request)
        {
            var existingRequest = await _context.MaintenanceRequests.FindAsync(request.Id);
            if (existingRequest == null) return null;

            existingRequest.Status = request.Status;
            existingRequest.ManagerNotes = request.ManagerNotes;
            existingRequest.Priority = request.Priority;

            if (request.Status == "Completed")
            {
                existingRequest.CompletedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return existingRequest;
        }

        public async Task<List<MaintenanceRequest>> GetRequestsByTenantAsync(int tenantId)
        {
            return await _context.MaintenanceRequests
                .Include(mr => mr.House)
                .Where(mr => mr.TenantId == tenantId)
                .OrderByDescending(mr => mr.RequestedAt)
                .ToListAsync();
        }

        public async Task<List<MaintenanceRequest>> GetPendingRequestsAsync()
        {
            return await _context.MaintenanceRequests
                .Include(mr => mr.Tenant)
                .ThenInclude(t => t.User)
                .Include(mr => mr.House)
                .Where(mr => mr.Status == "Pending" || mr.Status == "In Progress")
                .OrderByDescending(mr => mr.RequestedAt)
                .ToListAsync();
        }
    }
}
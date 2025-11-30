using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TenantsManagementApp.Services;
using TenantsManagementApp.ViewModels.TenantsDashboard;

namespace TenantsManagementApp.Controllers
{
    [Authorize(Roles = "Tenant,Landlord,Admin")]
    public class TenantController : Controller
    {
        private readonly ITenantService _tenantService;
        private readonly IMaintenanceService _maintenanceService;
        private readonly INotificationService _notificationService;

        public TenantController(ITenantService tenantService, IMaintenanceService maintenanceService, INotificationService notificationService)
        {
            _tenantService = tenantService;
            _maintenanceService = maintenanceService;
            _notificationService = notificationService;
        }

        [HttpGet]
        [Route("/Tenant/GetProfile")]
        public async Task<IActionResult> GetProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var tenant = await _tenantService.GetTenantByUserIdAsync(userId);
            if (tenant == null)
                return NotFound();

            var vm = new TenantProfileViewModel
            {
                TenantId = tenant.Id.ToString(),
                FullName = tenant.User?.FullName ?? string.Empty,
                Email = tenant.User?.Email ?? string.Empty,
                UnitNumber = tenant.House?.Name ?? string.Empty,
                PropertyName = tenant.House?.Location ?? string.Empty,
                LeaseStartDate = tenant.LeaseStartDate ?? DateTime.MinValue,
                LeaseEndDate = tenant.LeaseEndDate ?? DateTime.MinValue
            };

            return Json(vm);
        }

        [HttpGet]
        [Route("/Tenant/GetDashboardStats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var tenant = await _tenantService.GetTenantByUserIdAsync(userId);
            if (tenant == null)
                return NotFound();

            var balance = await _tenantService.GetTenantBalanceAsync(tenant.Id);
            var maintenance = await _maintenanceService.GetRequestsByTenantAsync(tenant.Id);
            var notifications = await _notificationService.GetUserNotificationsAsync(userId);

            var vm = new DashboardStatsViewModel
            {
                CurrentBalance = balance?.OutstandingBalance ?? 0m,
                NextPaymentDate = balance?.LastPaymentDate ?? (DateTime?)null,
                ActiveMaintenanceRequests = maintenance?.Count ?? 0,
                InProgressRequests = maintenance?.Count(m => m.Status == "In Progress") ?? 0,
                UnreadNotifications = notifications?.Count(n => !n.IsRead) ?? 0,
                UnreadMessages = 0,
                LeaseStatus = tenant.IsLeaseActive ? "Active" : "Expired",
                LeaseEndDate = tenant.LeaseEndDate ?? DateTime.MinValue
            };

            return Json(vm);
        }

        [HttpGet]
        [Route("/Tenant/GetRecentActivity")]
        public async Task<IActionResult> GetRecentActivity()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var tenant = await _tenantService.GetTenantByUserIdAsync(userId);
            if (tenant == null)
                return NotFound();

            // Compose recent activity from notifications and maintenance requests
            var notifications = await _notificationService.GetUserNotificationsAsync(userId);
            var maintenance = await _maintenanceService.GetRequestsByTenantAsync(tenant.Id);

            var activities = new List<RecentActivityViewModel>();

            if (notifications != null)
            {
                activities.AddRange(notifications.Select(n => new RecentActivityViewModel
                {
                    Id = n.Id,
                    Title = n.Title,
                    Description = n.Message,
                    CreatedDate = n.CreatedAt,
                    ActivityType = "Notification",
                    RelatedEntityId = n.Id.ToString()
                }));
            }

            if (maintenance != null)
            {
                activities.AddRange(maintenance.Select(m => new RecentActivityViewModel
                {
                    Id = m.Id,
                    Title = m.Title,
                    Description = m.Description,
                    CreatedDate = m.RequestedAt,
                    ActivityType = "Maintenance",
                    RelatedEntityId = m.Id.ToString()
                }));
            }

            // Order by date desc and take top 50
            var ordered = activities.OrderByDescending(a => a.CreatedDate).Take(50).ToList();
            return Json(ordered);
        }
    }
}

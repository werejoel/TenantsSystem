using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TenantsManagementApp.Data;
using Microsoft.Extensions.Logging;
using TenantsManagementApp.ViewModels.TenantsDashboard;
using TenantsManagementApp.Models;
using System.Security.Claims;

namespace TenantsManagementApp.Controllers
{
    [Authorize(Roles = "Tenant,Admin")]
    public class TenantsDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TenantsDashboardController> _logger;

        public TenantsDashboardController(ApplicationDbContext context, ILogger<TenantsDashboardController> logger)
        {
            _context = context;
            _logger = logger;
        }
        
        public IActionResult Index() => View();

        [HttpGet]
        public IActionResult GetTenantProfile()
        {
            try
            {
                var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!Guid.TryParse(userIdStr, out var userId))
                    return Unauthorized();

                var tenant = _context.Tenants
                    .FirstOrDefault(t => t.UserId == userId && t.IsActive);

                if (tenant == null)
                    return NotFound(new { message = "Tenant profile not found" });

                var user = _context.Users.FirstOrDefault(u => u.Id == userId);
                var house = tenant.HouseId.HasValue ? _context.Houses.FirstOrDefault(h => h.Id == tenant.HouseId.Value) : null;

                return Json(new TenantProfileViewModel
                {
                    TenantId = tenant.Id.ToString(),
                    FullName = user?.FullName ?? "",
                    Email = user?.Email ?? "",
                    UnitNumber = house?.Name ?? "N/A",
                    PropertyName = house?.Name ?? "N/A",
                    LeaseStartDate = tenant.LeaseStartDate ?? DateTime.MinValue,
                    LeaseEndDate = tenant.LeaseEndDate ?? DateTime.MinValue
                });
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "GetTenantProfile failed");
                return StatusCode(500, new { message = "Failed to load tenant profile. Please try again." });
            }
        }

        [HttpGet]
        public IActionResult GetDashboardStats()
        {
            try
            {
                var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                _logger?.LogInformation($"GetDashboardStats - UserIdStr: {userIdStr}");
                
                if (!Guid.TryParse(userIdStr, out var userId))
                {
                    _logger?.LogWarning("GetDashboardStats - Failed to parse userId");
                    return Unauthorized();
                }

                // Get tenant WITHOUT includes to avoid circular reference issues
                var tenant = _context.Tenants
                    .Where(t => t.UserId == userId && t.IsActive)
                    .FirstOrDefault();

                if (tenant == null)
                {
                    _logger?.LogWarning($"GetDashboardStats - Tenant not found for userId: {userId}");
                    return Json(new DashboardStatsViewModel
                    {
                        CurrentBalance = 0,
                        NextPaymentDate = null,
                        ActiveMaintenanceRequests = 0,
                        InProgressRequests = 0,
                        UnreadNotifications = 0,
                        UnreadMessages = 0,
                        LeaseStatus = "Inactive",
                        LeaseEndDate = DateTime.MinValue
                    });
                }

                _logger?.LogInformation($"GetDashboardStats - Found tenant: {tenant.Id}");

                try
                {
                    // Query payments separately
                    var paymentList = _context.Payments
                        .Where(p => p.TenantId == tenant.Id)
                        .ToList();
                    _logger?.LogInformation($"GetDashboardStats - Loaded {paymentList.Count} payments");

                    // Query charges separately
                    var chargeList = _context.Charges
                        .Where(c => c.TenantId == tenant.Id)
                        .ToList();
                    _logger?.LogInformation($"GetDashboardStats - Loaded {chargeList.Count} charges");

                    decimal totalPaid = paymentList.Count > 0 ? paymentList.Sum(p => p.AmountPaid) : 0m;
                    decimal totalCharges = chargeList.Count > 0 ? chargeList.Sum(c => c.Amount) : 0m;
                    decimal balance = totalCharges - totalPaid;

                    var activeRequests = _context.MaintenanceRequests
                        .Count(r => r.TenantId == tenant.Id && r.Status != "Completed");

                    var inProgressRequests = _context.MaintenanceRequests
                        .Count(r => r.TenantId == tenant.Id && r.Status == "In Progress");

                    var unreadNotifications = _context.Notifications
                        .Count(n => n.UserId == userId && !n.IsRead);

                    var lastPayment = paymentList
                        .OrderByDescending(p => p.PaymentDate)
                        .FirstOrDefault();

                    var leaseEndDate = tenant.LeaseEndDate ?? DateTime.MinValue;
                    var leaseStartDate = tenant.LeaseStartDate ?? DateTime.MinValue;
                    bool isLeaseActive = leaseStartDate != DateTime.MinValue && leaseEndDate != DateTime.MinValue && 
                                         DateTime.Now >= leaseStartDate && DateTime.Now <= leaseEndDate;

                    var stats = new DashboardStatsViewModel
                    {
                        CurrentBalance = balance,
                        NextPaymentDate = lastPayment != null ? lastPayment.PaymentDate.AddMonths(1) : (DateTime?)null,
                        ActiveMaintenanceRequests = activeRequests,
                        InProgressRequests = inProgressRequests,
                        UnreadNotifications = unreadNotifications,
                        UnreadMessages = 0,
                        LeaseStatus = isLeaseActive ? "Active" : "Inactive",
                        LeaseEndDate = leaseEndDate
                    };

                    _logger?.LogInformation("GetDashboardStats - Success");
                    return Json(stats);
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, $"GetDashboardStats - Error during data processing: {ex.Message}");
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"GetDashboardStats - Exception: {ex.Message}, Inner: {ex.InnerException?.Message}");
                return StatusCode(500, new { 
                    message = "Failed to load dashboard statistics",
                    error = ex.Message,
                    innerError = ex.InnerException?.Message
                });
            }
        }

        [HttpGet]
        public IActionResult GetRecentActivity()
        {
            try
            {
                var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!Guid.TryParse(userIdStr, out var userId))
                    return Unauthorized();

                var tenant = _context.Tenants
                    .FirstOrDefault(t => t.UserId == userId && t.IsActive);

                if (tenant == null)
                    return NotFound(new { message = "Tenant not found" });

                var activities = _context.Payments
                    .Where(p => p.TenantId == tenant.Id)
                    .OrderByDescending(p => p.PaymentDate)
                    .Take(5)
                    .Select(p => new RecentActivityViewModel
                    {
                        Id = p.Id,
                        Title = "Payment",
                        Description = p.Notes ?? "Payment received",
                        CreatedDate = p.PaymentDate,
                        ActivityType = "Payment",
                        RelatedEntityId = p.Id.ToString()
                    })
                    .ToList();

                return Json(activities);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "GetRecentActivity failed");
                return StatusCode(500, new { message = "Failed to load recent activity. Please try again." });
            }
        }

        [HttpGet]
        public IActionResult GetPaymentHistory()
        {
            try
            {
                var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!Guid.TryParse(userIdStr, out var userId))
                    return Unauthorized();

                var tenant = _context.Tenants
                    .FirstOrDefault(t => t.UserId == userId && t.IsActive);

                if (tenant == null)
                    return NotFound(new { message = "Tenant not found" });

                var payments = _context.Payments
                    .Where(p => p.TenantId == tenant.Id)
                    .OrderByDescending(p => p.PaymentDate)
                    .Select(p => new PaymentHistoryViewModel
                    {
                        Id = p.Id,
                        PaymentDate = p.PaymentDate,
                        Description = p.Notes ?? "Payment",
                        Amount = p.AmountPaid,
                        Status = "Completed",
                        PaymentMethod = p.PaymentMethod ?? "Cash",
                        TransactionId = p.Id.ToString()
                    })
                    .ToList();

                return Json(payments);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "GetPaymentHistory failed");
                return StatusCode(500, new { message = "Failed to load payment history. Please try again." });
            }
        }

        [HttpGet]
        public IActionResult GetMaintenanceRequests()
        {
            try
            {
                var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!Guid.TryParse(userIdStr, out var userId))
                    return Unauthorized();

                var tenant = _context.Tenants
                    .FirstOrDefault(t => t.UserId == userId && t.IsActive);

                if (tenant == null)
                    return NotFound(new { message = "Tenant not found" });

                var requests = _context.MaintenanceRequests
                    .Where(r => r.TenantId == tenant.Id)
                    .OrderByDescending(r => r.RequestedAt)
                    .Select(r => new MaintenanceRequestViewModel
                    {
                        Id = r.Id,
                        Title = r.Title,
                        Description = r.Description,
                        Priority = r.Priority,
                        Status = r.Status,
                        SubmittedDate = r.RequestedAt,
                        CompletedDate = r.CompletedAt,
                        Photos = new List<string>()
                    })
                    .ToList();

                return Json(requests);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "GetMaintenanceRequests failed");
                return StatusCode(500, new { message = "Failed to load maintenance requests. Please try again." });
            }
        }

        [HttpGet]
        public IActionResult GetDocuments()
        {
            try
            {
                var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!Guid.TryParse(userIdStr, out var userId))
                    return Unauthorized();

                var tenant = _context.Tenants
                    .FirstOrDefault(t => t.UserId == userId && t.IsActive);

                if (tenant == null)
                    return NotFound(new { message = "Tenant not found" });

                var docs = _context.Documents
                    .Where(d => d.TenantId == tenant.Id)
                    .OrderByDescending(d => d.UploadedAt)
                    .ToList()
                    .Select(d => {
                        long parsedSize = 0;
                        long.TryParse(d.FileSize ?? "0", out parsedSize);
                        return new DocumentViewModel
                        {
                            Id = d.Id,
                            FileName = d.FileName,
                            FileType = d.DocumentType,
                            FileSize = parsedSize,
                            Category = d.DocumentType,
                            UploadDate = d.UploadedAt,
                            Description = string.Empty
                        };
                    })
                    .ToList();

                return Json(docs);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "GetDocuments failed");
                return StatusCode(500, new { message = "Failed to load documents. Please try again." });
            }
        }

        [HttpGet]
        public IActionResult GetPaymentMethods()
        {
            try
            {
                var methods = new List<PaymentMethodViewModel>
                {
                    new PaymentMethodViewModel
                    {
                        Id = 1,
                        CardType = "visa",
                        MaskedCardNumber = "**** **** **** 1234",
                        ExpiryDate = "12/27",
                        IsDefault = true,
                        IsAutoPayEnabled = false
                    }
                };
                return Json(methods);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "GetPaymentMethods failed");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitMaintenanceRequest([FromBody] CreateMaintenanceRequestViewModel model)
        {
            try
            {
                if (model == null)
                    return BadRequest(new { message = "Invalid request payload" });

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "SubmitMaintenanceRequest failed");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetAnnouncements()
        {
            try
            {
                var announcements = new List<AnnouncementViewModel>
                {
                    new AnnouncementViewModel
                    {
                        Id = 1,
                        Title = "Pool Maintenance",
                        Content = "The pool will be closed for maintenance on Saturday.",
                        Priority = "High",
                        PostedDate = DateTime.Now.AddDays(-1),
                        ExpiryDate = DateTime.Now.AddDays(2),
                        Attachments = new List<AttachmentViewModel>()
                    }
                };
                return Json(announcements);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "GetAnnouncements failed");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetUpcomingEvents()
        {
            try
            {
                var events = new List<EventViewModel>
                {
                    new EventViewModel
                    {
                        Id = 1,
                        Title = "Community BBQ",
                        Description = "Join us for a BBQ at the main park!",
                        EventDate = DateTime.Now.AddDays(5),
                        Location = "Main Park",
                        RequiresRSVP = true,
                        MaxAttendees = 100,
                        CurrentAttendees = 25
                    }
                };
                return Json(events);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "GetUpcomingEvents failed");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetEmergencyContacts()
        {
            try
            {
                var contacts = new List<EmergencyContactViewModel>
                {
                    new EmergencyContactViewModel
                    {
                        Id = 1,
                        Title = "Management Office",
                        PhoneNumber = "(256) 705672545",
                        Email = "office@apartments.com",
                        Hours = "9am-5pm",
                        IsEmergencyOnly = false
                    },
                    new EmergencyContactViewModel
                    {
                        Id = 2,
                        Title = "Emergency Maintenance",
                        PhoneNumber = "(256) 789-251487",
                        Email = "",
                        Hours = "24/7",
                        IsEmergencyOnly = true
                    }
                };
                return Json(contacts);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "GetEmergencyContacts failed");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetWeatherInfo()
        {
            try
            {
                var weather = new WeatherInfoViewModel
                {
                    Temperature = "72",
                    Condition = "Sunny",
                    Location = "Your City"
                };
                return Json(weather);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "GetWeatherInfo failed");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}

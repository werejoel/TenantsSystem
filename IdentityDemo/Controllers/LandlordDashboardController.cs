using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TenantsManagementApp.Data;
using Microsoft.Extensions.Logging;
using TenantsManagementApp.ViewModels.LandlordDashboard;
using TenantsManagementApp.Models;

namespace TenantsManagementApp.Controllers
{
    [Authorize(Roles = "Landlord,Admin")]
    public class LandlordDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LandlordDashboardController> _logger;

        public LandlordDashboardController(ApplicationDbContext context, ILogger<LandlordDashboardController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index() => View();

        // Lightweight endpoint to verify the controller is reachable (no auth required)
        [HttpGet]
        [AllowAnonymous]
        [Route("LandlordDashboard/Ping")]
        public IActionResult Ping()
        {
            return Json(new { ok = true, timestamp = DateTime.UtcNow });
        }

        #region Profile Management

        [HttpGet]
        [Route("LandlordDashboard/GetLandlordProfile")]
        public IActionResult GetLandlordProfile()
        {
            try
            {
                var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? User.Identity?.Name;
                if (string.IsNullOrEmpty(userEmail))
                    return Unauthorized();

                var landlord = _context.Landlords
                    .Include(l => l.Houses)
                    .Where(l => l.Email != null && l.Email.ToLower() == userEmail.ToLower() && l.IsActive)
                    .FirstOrDefault();

                if (landlord == null)
                    return NotFound(new { message = "Landlord profile not found" });

                // Each House represents a single rentable unit in the current data model
                var totalUnits = landlord.Houses.Count;
                var occupiedUnits = _context.Tenants
                    .Count(t => t.IsActive && t.HouseId.HasValue && landlord.Houses.Select(h => h.Id).Contains(t.HouseId.Value));

                var profile = new LandlordProfileViewModel
                {
                    LandlordId = landlord.Id.ToString(),
                    FullName = landlord.FullName,
                    Email = landlord.Email ?? string.Empty,
                    Phone = landlord.Phone ?? string.Empty,
                    Address = landlord.Address ?? string.Empty,
                    TotalProperties = landlord.Houses.Count,
                    TotalUnits = totalUnits,
                    OccupiedUnits = occupiedUnits,
                    VacantUnits = totalUnits - occupiedUnits,
                    MemberSince = landlord.CreatedAt
                };

                return Json(profile);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "GetLandlordProfile failed");
                return StatusCode(500, new { message = "Failed to load landlord profile. Please try again." });
            }
        }

        #endregion

        #region Dashboard Statistics

        [HttpGet]
        [Route("LandlordDashboard/GetDashboardStats")]
        public IActionResult GetDashboardStats()
        {
            try
            {
                var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? User.Identity?.Name;
                if (string.IsNullOrEmpty(userEmail))
                    return Unauthorized();

                var landlord = _context.Landlords
                    .Include(l => l.Houses)
                    .Where(l => l.Email != null && l.Email.ToLower() == userEmail.ToLower() && l.IsActive)
                    .FirstOrDefault();

                if (landlord == null)
                    return NotFound(new { message = "Landlord not found" });

                var propertyIds = landlord.Houses.Select(h => h.Id).ToList();
                var tenantIds = _context.Tenants
                    .Where(t => t.HouseId.HasValue && propertyIds.Contains(t.HouseId.Value))
                    .Select(t => t.Id)
                    .ToList();

                var totalRevenue = _context.Payments
                    .Where(p => tenantIds.Contains(p.TenantId))
                    .Sum(p => p.AmountPaid);

                var currentMonth = DateTime.Now.Month;
                var currentYear = DateTime.Now.Year;
                var monthlyRevenue = _context.Payments
                    .Where(p => tenantIds.Contains(p.TenantId)
                        && p.PaymentDate.Month == currentMonth
                        && p.PaymentDate.Year == currentYear)
                    .Sum(p => p.AmountPaid);

                var totalCharges = _context.Charges
                    .Where(c => tenantIds.Contains(c.TenantId))
                    .Sum(c => c.Amount);

                var outstandingBalance = totalCharges - totalRevenue;

                // Each House represents a single rentable unit in the current data model
                var totalUnits = landlord.Houses.Count;
                var occupiedUnits = _context.Tenants
                    .Count(t => t.IsActive && t.HouseId.HasValue && propertyIds.Contains(t.HouseId.Value));

                var pendingMaintenance = _context.MaintenanceRequests
                    .Count(r => tenantIds.Contains(r.TenantId) && r.Status == "Pending");

                var maintenanceInProgress = _context.MaintenanceRequests
                    .Count(r => tenantIds.Contains(r.TenantId) && r.Status == "In Progress");

                var leaseExpiringThisMonth = _context.Tenants
                    .Count(t => t.IsActive
                        && t.HouseId.HasValue
                        && propertyIds.Contains(t.HouseId.Value)
                        && t.LeaseEndDate.HasValue
                        && t.LeaseEndDate.Value.Month == currentMonth
                        && t.LeaseEndDate.Value.Year == currentYear);

                var overduePayments = _context.Charges
                    .Where(c => tenantIds.Contains(c.TenantId)
                        && c.DueDate < DateTime.Now)
                    .Count();

                var stats = new LandlordDashboardStatsViewModel
                {
                    TotalRevenue = totalRevenue,
                    MonthlyRevenue = monthlyRevenue,
                    OutstandingBalance = outstandingBalance,
                    ExpectedRevenue = totalCharges,
                    TotalProperties = landlord.Houses.Count,
                    TotalUnits = totalUnits,
                    OccupiedUnits = occupiedUnits,
                    VacantUnits = totalUnits - occupiedUnits,
                    OccupancyRate = totalUnits > 0 ? (double)occupiedUnits / totalUnits * 100 : 0,
                    ActiveTenants = occupiedUnits,
                    PendingMaintenanceRequests = pendingMaintenance,
                    MaintenanceInProgress = maintenanceInProgress,
                    LeaseExpiringThisMonth = leaseExpiringThisMonth,
                    OverduePayments = overduePayments,
                    MaintenanceCosts = 0
                };

                return Json(stats);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "GetDashboardStats failed");
                return StatusCode(500, new { message = "Failed to load dashboard statistics. Please try again." });
            }
        }

        #endregion

        #region Property Management

        [HttpGet]
        [Route("LandlordDashboard/GetProperties")]
        public IActionResult GetProperties()
        {
            try
            {
                var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? User.Identity?.Name;
                if (string.IsNullOrEmpty(userEmail))
                    return Unauthorized();

                var landlord = _context.Landlords
                    .Include(l => l.Houses)
                    .Where(l => l.Email != null && l.Email.ToLower() == userEmail.ToLower() && l.IsActive)
                    .FirstOrDefault();

                if (landlord == null)
                    return NotFound(new { message = "Landlord not found" });

                var properties = landlord.Houses.Select(h =>
                {
                    var tenants = _context.Tenants
                        .Where(t => t.HouseId == h.Id && t.IsActive)
                        .ToList();

                    var tenantIds = tenants.Select(t => t.Id).ToList();

                    var revenue = _context.Payments
                        .Where(p => tenantIds.Contains(p.TenantId)
                            && p.PaymentDate.Month == DateTime.Now.Month
                            && p.PaymentDate.Year == DateTime.Now.Year)
                        .Sum(p => p.AmountPaid);

                    var charges = _context.Charges
                        .Where(c => tenantIds.Contains(c.TenantId))
                        .Sum(c => c.Amount);

                    var payments = _context.Payments
                        .Where(p => tenantIds.Contains(p.TenantId))
                        .Sum(p => p.AmountPaid);

                    var pendingMaintenance = _context.MaintenanceRequests
                        .Count(r => tenantIds.Contains(r.TenantId)
                            && (r.Status == "Pending" || r.Status == "In Progress"));

                    return new PropertyOverviewViewModel
                    {
                        Id = h.Id,
                        PropertyName = h.Name,
                        Address = h.Location ?? "N/A",
                        TotalUnits = 1,
                        OccupiedUnits = tenants.Count,
                        VacantUnits = 1 - tenants.Count,
                        OccupancyRate = 1 > 0 ? (double)tenants.Count / 1 * 100 : 0,
                        MonthlyRevenue = revenue,
                        OutstandingBalance = charges - payments,
                        PendingMaintenance = pendingMaintenance,
                        PropertyType = "Residential"
                    };
                }).ToList();

                return Json(properties);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "GetProperties failed");
                return StatusCode(500, new { message = "Failed to load properties. Please try again." });
            }
        }

        [HttpGet]
        [Route("LandlordDashboard/GetPropertyPerformance")]
        public IActionResult GetPropertyPerformance()
        {
            try
            {
                var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? User.Identity?.Name;
                if (string.IsNullOrEmpty(userEmail))
                    return Unauthorized();

                var landlord = _context.Landlords
                    .Include(l => l.Houses)
                    .Where(l => l.Email != null && l.Email.ToLower() == userEmail.ToLower() && l.IsActive)
                    .FirstOrDefault();

                if (landlord == null)
                    return NotFound(new { message = "Landlord not found" });

                var performance = landlord.Houses.Select(h =>
                {
                    var tenants = _context.Tenants
                        .Where(t => t.HouseId == h.Id && t.IsActive)
                        .ToList();

                    var tenantIds = tenants.Select(t => t.Id).ToList();

                    var revenue = _context.Payments
                        .Where(p => tenantIds.Contains(p.TenantId))
                        .Sum(p => p.AmountPaid);

                    var maintenanceCount = _context.MaintenanceRequests
                        .Count(r => tenantIds.Contains(r.TenantId));

                    return new PropertyPerformanceViewModel
                    {
                        PropertyId = h.Id,
                        PropertyName = h.Name,
                        OccupancyRate = 1 > 0 ? (double)tenants.Count / 1 * 100 : 0,
                        Revenue = revenue,
                        Expenses = 0,
                        NetOperatingIncome = revenue,
                        ROI = 0,
                        MaintenanceRequests = maintenanceCount,
                        TenantSatisfactionScore = 0,
                        AverageTenancyDuration = 0
                    };
                }).ToList();

                return Json(performance);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "GetPropertyPerformance failed");
                return StatusCode(500, new { message = "Failed to load property performance. Please try again." });
            }
        }

        #endregion

        #region Tenant Management

        [HttpGet]
        [Route("LandlordDashboard/GetTenants")]
        public IActionResult GetTenants()
        {
            try
            {
                var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? User.Identity?.Name;
                if (string.IsNullOrEmpty(userEmail))
                    return Unauthorized();

                var landlord = _context.Landlords
                    .Include(l => l.Houses)
                    .Where(l => l.Email != null && l.Email.ToLower() == userEmail.ToLower() && l.IsActive)
                    .FirstOrDefault();

                if (landlord == null)
                    return NotFound(new { message = "Landlord not found" });

                var propertyIds = landlord.Houses.Select(h => h.Id).ToList();

                var tenants = _context.Tenants
                    .Include(t => t.House)
                    .Include(t => t.User)
                    .Where(t => t.HouseId.HasValue && propertyIds.Contains(t.HouseId.Value))
                    .Select(t => new TenantOverviewViewModel
                    {
                        Id = t.Id,
                        TenantName = t.FullName,
                        Email = t.User != null ? t.User.Email : string.Empty,
                        Phone = t.User != null ? t.User.PhoneNumber ?? string.Empty : string.Empty,
                        UnitNumber = t.House != null ? t.House.Name : "N/A",
                        PropertyName = t.House != null ? t.House.Name : "N/A",
                        LeaseStartDate = t.LeaseStartDate ?? DateTime.MinValue,
                        LeaseEndDate = t.LeaseEndDate ?? DateTime.MinValue,
                        MonthlyRent = t.House != null ? t.House.Price : 0m,
                        CurrentBalance = _context.Charges.Where(c => c.TenantId == t.Id).Sum(c => c.Amount)
                            - _context.Payments.Where(p => p.TenantId == t.Id).Sum(p => p.AmountPaid),
                        PaymentStatus = t.IsActive ? "Active" : "Inactive",
                        IsLeaseActive = t.IsLeaseActive,
                        DaysUntilLeaseExpiry = t.LeaseEndDate.HasValue
                            ? (int)(t.LeaseEndDate.Value - DateTime.Now).TotalDays
                            : 0
                    })
                    .OrderBy(t => t.TenantName)
                    .ToList();

                return Json(tenants);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "GetTenants failed");
                return StatusCode(500, new { message = "Failed to load tenants. Please try again." });
            }
        }

        #endregion

        #region Financial Management

        [HttpGet]
        [Route("LandlordDashboard/GetRevenueSummary")]
        public IActionResult GetRevenueSummary()
        {
            try
            {
                var landlord = _context.Landlords
                    .Include(l => l.Houses)
                    .Where(l => l.IsActive)
                    .FirstOrDefault();

                if (landlord == null)
                    return NotFound(new { message = "Landlord not found" });

                var propertyIds = landlord.Houses.Select(h => h.Id).ToList();
                var tenantIds = _context.Tenants
                    .Where(t => t.HouseId.HasValue && propertyIds.Contains(t.HouseId.Value))
                    .Select(t => t.Id)
                    .ToList();

                var totalCharges = _context.Charges
                    .Where(c => tenantIds.Contains(c.TenantId))
                    .Sum(c => c.Amount);

                var collectedRevenue = _context.Payments
                    .Where(p => tenantIds.Contains(p.TenantId))
                    .Sum(p => p.AmountPaid);

                var overdueCharges = _context.Charges
                    .Where(c => tenantIds.Contains(c.TenantId) && c.DueDate < DateTime.Now)
                    .Sum(c => c.Amount);

                var overduePayments = _context.Payments
                    .Where(p => tenantIds.Contains(p.TenantId) && p.PaymentDate < DateTime.Now)
                    .Sum(p => p.AmountPaid);

                var monthlyBreakdown = _context.Payments
                    .Where(p => tenantIds.Contains(p.TenantId)
                        && p.PaymentDate >= DateTime.Now.AddMonths(-6))
                    .GroupBy(p => new { p.PaymentDate.Year, p.PaymentDate.Month })
                    .Select(g => new MonthlyRevenueViewModel
                    {
                        Month = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMMM"),
                        Year = g.Key.Year,
                        Revenue = g.Sum(p => p.AmountPaid),
                        Expenses = 0,
                        NetIncome = g.Sum(p => p.AmountPaid)
                    })
                    .OrderByDescending(m => m.Year)
                    .ThenByDescending(m => m.Month)
                    .ToList();

                var propertyBreakdown = landlord.Houses.Select(h =>
                {
                    var houseTenantIds = _context.Tenants
                        .Where(t => t.HouseId == h.Id)
                        .Select(t => t.Id)
                        .ToList();

                    var revenue = _context.Payments
                        .Where(p => houseTenantIds.Contains(p.TenantId))
                        .Sum(p => p.AmountPaid);

                    return new PropertyRevenueViewModel
                    {
                        PropertyId = h.Id,
                        PropertyName = h.Name,
                        Revenue = revenue,
                        Expenses = 0,
                        NetIncome = revenue
                    };
                }).ToList();

                var summary = new RevenueSummaryViewModel
                {
                    TotalRevenue = totalCharges,
                    CollectedRevenue = collectedRevenue,
                    PendingRevenue = totalCharges - collectedRevenue,
                    OverdueRevenue = overdueCharges - overduePayments,
                    MonthlyBreakdown = monthlyBreakdown,
                    PropertyBreakdown = propertyBreakdown
                };

                return Json(summary);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "GetRevenueSummary failed");
                return StatusCode(500, new { message = "Failed to load revenue summary. Please try again." });
            }
        }

        [HttpGet]
        [Route("LandlordDashboard/GetPaymentTracking")]
        public IActionResult GetPaymentTracking()
        {
            try
            {
                var landlord = _context.Landlords
                    .Include(l => l.Houses)
                    .Where(l => l.IsActive)
                    .FirstOrDefault();

                if (landlord == null)
                    return NotFound(new { message = "Landlord not found" });

                var propertyIds = landlord.Houses.Select(h => h.Id).ToList();

                var payments = _context.Payments
                    .Include(p => p.Tenant)
                        .ThenInclude(t => t.House)
                    .Where(p => p.Tenant.HouseId.HasValue
                        && propertyIds.Contains(p.Tenant.HouseId.Value))
                    .OrderByDescending(p => p.PaymentDate)
                    .Take(50)
                    .Select(p => new PaymentTrackingViewModel
                    {
                        Id = p.Id,
                        TenantName = p.Tenant.FullName,
                        UnitNumber = p.Tenant.House != null ? p.Tenant.House.Name : "N/A",
                        PropertyName = p.Tenant.House != null ? p.Tenant.House.Name : "N/A",
                        PaymentDate = p.PaymentDate,
                        DueDate = DateTime.Now,
                        Amount = p.AmountPaid,
                        Status = "Completed",
                        PaymentMethod = p.PaymentMethod ?? "Cash",
                        DaysOverdue = 0,
                        TransactionId = p.Id.ToString()
                    })
                    .ToList();

                return Json(payments);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "GetPaymentTracking failed");
                return StatusCode(500, new { message = "Failed to load payment tracking. Please try again." });
            }
        }

        [HttpGet]
        [Route("LandlordDashboard/GetFinancialReport")]
        public IActionResult GetFinancialReport()
        {
            try
            {
                var landlord = _context.Landlords
                    .Include(l => l.Houses)
                    .Where(l => l.IsActive)
                    .FirstOrDefault();

                if (landlord == null)
                    return NotFound(new { message = "Landlord not found" });

                var propertyIds = landlord.Houses.Select(h => h.Id).ToList();
                var tenantIds = _context.Tenants
                    .Where(t => t.HouseId.HasValue && propertyIds.Contains(t.HouseId.Value))
                    .Select(t => t.Id)
                    .ToList();

                var totalIncome = _context.Payments
                    .Where(p => tenantIds.Contains(p.TenantId))
                    .Sum(p => p.AmountPaid);

                var report = new FinancialReportViewModel
                {
                    ReportDate = DateTime.Now,
                    ReportPeriod = DateTime.Now.ToString("MMMM yyyy"),
                    TotalIncome = totalIncome,
                    TotalExpenses = 0,
                    NetIncome = totalIncome,
                    ProfitMargin = 0,
                    IncomeBreakdown = new List<IncomeBreakdownViewModel>
                    {
                        new IncomeBreakdownViewModel { Category = "Rent", Amount = totalIncome, Percentage = 100 }
                    },
                    ExpenseBreakdown = new List<ExpenseBreakdownViewModel>()
                };

                return Json(report);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "GetFinancialReport failed");
                return StatusCode(500, new { message = "Failed to load financial report. Please try again." });
            }
        }

        #endregion

        #region Maintenance Management

        [HttpGet]
        [Route("LandlordDashboard/GetMaintenanceRequests")]
        public IActionResult GetMaintenanceRequests()
        {
            try
            {
                var landlord = _context.Landlords
                    .Include(l => l.Houses)
                    .Where(l => l.IsActive)
                    .FirstOrDefault();

                if (landlord == null)
                    return NotFound(new { message = "Landlord not found" });

                var propertyIds = landlord.Houses.Select(h => h.Id).ToList();

                var requests = _context.MaintenanceRequests
                    .Include(r => r.Tenant)
                        .ThenInclude(t => t.House)
                    .Where(r => r.Tenant.HouseId.HasValue
                        && propertyIds.Contains(r.Tenant.HouseId.Value))
                    .OrderByDescending(r => r.RequestedAt)
                    .Select(r => new LandlordMaintenanceRequestViewModel
                    {
                        Id = r.Id,
                        Title = r.Title,
                        Description = r.Description,
                        TenantName = r.Tenant.FullName,
                        UnitNumber = r.Tenant.House != null ? r.Tenant.House.Name : "N/A",
                        PropertyName = r.Tenant.House != null ? r.Tenant.House.Name : "N/A",
                        Priority = r.Priority,
                        Status = r.Status,
                        SubmittedDate = r.RequestedAt,
                        CompletedDate = r.CompletedAt,
                        EstimatedCost = null,
                        ActualCost = null,
                        AssignedTo = string.Empty
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("LandlordDashboard/UpdateMaintenanceRequest")]
        public IActionResult UpdateMaintenanceRequest([FromBody] UpdateMaintenanceRequestViewModel model)
        {
            try
            {
                if (model == null)
                    return BadRequest(new { message = "Invalid request payload" });

                var request = _context.MaintenanceRequests.Find(model.RequestId);
                if (request == null)
                    return NotFound(new { message = "Maintenance request not found" });

                request.Status = model.Status;
                _context.SaveChanges();

                return Json(new { success = true, message = "Maintenance request updated successfully" });
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "UpdateMaintenanceRequest failed");
                return StatusCode(500, new { message = "Failed to update maintenance request. Please try again." });
            }
        }

        #endregion

        #region Lease Management

        [HttpGet]
        [Route("LandlordDashboard/GetLeaseManagement")]
        public IActionResult GetLeaseManagement()
        {
            try
            {
                var landlord = _context.Landlords
                    .Include(l => l.Houses)
                    .Where(l => l.IsActive)
                    .FirstOrDefault();

                if (landlord == null)
                    return NotFound(new { message = "Landlord not found" });

                var propertyIds = landlord.Houses.Select(h => h.Id).ToList();

                var leases = _context.Tenants
                    .Include(t => t.House)
                    .Where(t => t.HouseId.HasValue && propertyIds.Contains(t.HouseId.Value))
                    .Select(t => new LeaseManagementViewModel
                    {
                        Id = t.Id,
                        TenantName = t.FullName,
                        UnitNumber = t.House != null ? t.House.Name : "N/A",
                        PropertyName = t.House != null ? t.House.Name : "N/A",
                        LeaseStartDate = t.LeaseStartDate ?? DateTime.MinValue,
                        LeaseEndDate = t.LeaseEndDate ?? DateTime.MinValue,
                        MonthlyRent = t.House != null ? t.House.Price : 0m,
                        SecurityDeposit = 0,
                        LeaseStatus = t.IsLeaseActive ? "Active" : "Expired",
                        DaysRemaining = t.LeaseEndDate.HasValue
                            ? (int)(t.LeaseEndDate.Value - DateTime.Now).TotalDays
                            : 0,
                        AutoRenew = false,
                        LeaseType = "Fixed Term"
                    })
                    .OrderBy(l => l.DaysRemaining)
                    .ToList();

                return Json(leases);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "GetLeaseManagement failed");
                return StatusCode(500, new { message = "Failed to load lease management. Please try again." });
            }
        }

        #endregion

        #region Vacancy Analytics

        [HttpGet]
        [Route("LandlordDashboard/GetVacancyAnalytics")]
        public IActionResult GetVacancyAnalytics()
        {
            try
            {
                var landlord = _context.Landlords
                    .Include(l => l.Houses)
                    .Where(l => l.IsActive)
                    .FirstOrDefault();

                if (landlord == null)
                    return NotFound(new { message = "Landlord not found" });

                var vacancies = new List<VacancyAnalyticsViewModel>();

                foreach (var house in landlord.Houses)
                {
                    var occupiedCount = _context.Tenants
                        .Count(t => t.HouseId == house.Id && t.IsActive);

                    if (occupiedCount < 1)
                    {
                        vacancies.Add(new VacancyAnalyticsViewModel
                        {
                            PropertyId = house.Id,
                            PropertyName = house.Name,
                            UnitNumber = house.Name,
                            VacantSince = null,
                            DaysVacant = 0,
                            MonthlyRent = house.Price,
                            LostRevenue = 0,
                            Reason = "Not Specified",
                            IsListedForRent = true
                        });
                    }
                }

                return Json(vacancies);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "GetVacancyAnalytics failed");
                return StatusCode(500, new { message = "Failed to load vacancy analytics. Please try again." });
            }
        }

        #endregion

        #region Activity & Notifications

        [HttpGet]
        [Route("LandlordDashboard/GetRecentActivity")]
        public IActionResult GetRecentActivity()
        {
            try
            {
                var landlord = _context.Landlords
                    .Include(l => l.Houses)
                    .Where(l => l.IsActive)
                    .FirstOrDefault();

                if (landlord == null)
                    return NotFound(new { message = "Landlord not found" });

                var propertyIds = landlord.Houses.Select(h => h.Id).ToList();
                var tenantIds = _context.Tenants
                    .Where(t => t.HouseId.HasValue && propertyIds.Contains(t.HouseId.Value))
                    .Select(t => t.Id)
                    .ToList();

                var activities = _context.Payments
                    .Include(p => p.Tenant)
                        .ThenInclude(t => t.House)
                    .Where(p => tenantIds.Contains(p.TenantId))
                    .OrderByDescending(p => p.PaymentDate)
                    .Take(10)
                    .Select(p => new LandlordRecentActivityViewModel
                    {
                        Id = p.Id,
                        Title = "Payment Received",
                        Description = $"Payment of ${p.AmountPaid} received from {p.Tenant.FullName}",
                        ActivityDate = p.PaymentDate,
                        ActivityType = "Payment",
                        RelatedEntityId = p.Id.ToString(),
                        PropertyName = p.Tenant.House != null ? p.Tenant.House.Name : "N/A",
                        Icon = "dollar-sign"
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
        [Route("LandlordDashboard/GetAlerts")]
        public IActionResult GetAlerts()
        {
            try
            {
                var alerts = new List<LandlordAlertViewModel>
                {
                    new LandlordAlertViewModel
                    {
                        Id = Guid.NewGuid(),
                        Title = "Lease Expiring Soon",
                        Message = "3 leases are expiring within the next 30 days",
                        AlertType = "Warning",
                        Priority = "High",
                        CreatedDate = DateTime.Now,
                        IsRead = false,
                        ActionUrl = "/LandlordDashboard/LeaseManagement"
                    }
                };

                return Json(alerts);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "GetAlerts failed");
                return StatusCode(500, new { message = "Failed to load alerts. Please try again." });
            }
        }

        #endregion

        #region Documents

        [HttpGet]
        [Route("LandlordDashboard/GetDocuments")]
        public IActionResult GetDocuments()
        {
            try
            {
                var landlord = _context.Landlords
                    .Include(l => l.Houses)
                    .Where(l => l.IsActive)
                    .FirstOrDefault();

                if (landlord == null)
                    return NotFound(new { message = "Landlord not found" });

                var propertyIds = landlord.Houses.Select(h => h.Id).ToList();
                var tenantIds = _context.Tenants
                    .Where(t => t.HouseId.HasValue && propertyIds.Contains(t.HouseId.Value))
                    .Select(t => t.Id)
                    .ToList();

                var docs = _context.Documents
                    .Where(d => d.TenantId.HasValue && tenantIds.Contains(d.TenantId.Value))
                    .OrderByDescending(d => d.UploadedAt)
                    .ToList()
                    .Select(d => {
                        long parsedSize = 0;
                        long.TryParse(d.FileSize ?? "0", out parsedSize);
                        return new LandlordDocumentViewModel
                        {
                            Id = d.Id,
                            FileName = d.FileName,
                            DocumentType = d.DocumentType,
                            Category = d.DocumentType,
                            FileSize = parsedSize,
                            UploadDate = d.UploadedAt,
                            RelatedTo = "Tenant",
                            RelatedEntityId = d.TenantId?.ToString() ?? string.Empty,
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

        #endregion

        #region Expense Tracking

        [HttpGet]
        [Route("LandlordDashboard/GetExpenses")]
        public IActionResult GetExpenses()
        {
            try
            {
                var expenses = new List<ExpenseTrackingViewModel>();
                return Json(expenses);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "GetExpenses failed");
                return StatusCode(500, new { message = "Failed to load expenses. Please try again." });
            }
        }

        #endregion
    }
}
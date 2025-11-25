using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TenantsManagementApp.Data;
using TenantsManagementApp.DTOS;
using TenantsManagementApp.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenantsManagementApp.Services
{
    public class TenantService : ITenantService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<TenantService> _logger;

        public TenantService(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<TenantService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<Tenant>> GetAllTenantsAsync()
        {
            _logger.LogInformation("Retrieving all active tenants");
            try
            {
                var tenants = await _context.Tenants
                    .Include(t => t.User)
                    .Include(t => t.House)
                    .Where(t => t.IsActive)
                    .OrderBy(t => t.User.FirstName)
                    .ToListAsync();

                _logger.LogInformation("Successfully retrieved {Count} active tenants", tenants.Count);
                return tenants;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all tenants");
                throw;
            }
        }

        public async Task<Tenant?> GetTenantByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid tenant ID: {TenantId}", id);
                return null;
            }

            _logger.LogInformation("Retrieving tenant with ID {TenantId}", id);
            try
            {
                var tenant = await _context.Tenants
                    .Include(t => t.User)
                    .Include(t => t.House)
                    .ThenInclude(h => h.Landlord)
                    .FirstOrDefaultAsync(t => t.Id == id && t.IsActive);

                if (tenant == null)
                {
                    _logger.LogWarning("Tenant with ID {TenantId} not found or inactive", id);
                }
                else
                {
                    _logger.LogInformation("Successfully retrieved tenant with ID {TenantId}", id);
                }

                return tenant;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tenant with ID {TenantId}", id);
                throw;
            }
        }

        public async Task<Tenant?> GetTenantByUserIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                _logger.LogWarning("Invalid user ID: {UserId}", userId);
                return null;
            }

            _logger.LogInformation("Retrieving tenant for user ID {UserId}", userId);
            try
            {
                var tenant = await _context.Tenants
                    .Include(t => t.User)
                    .Include(t => t.House)
                    .FirstOrDefaultAsync(t => t.UserId == userId && t.IsActive);

                if (tenant == null)
                {
                    _logger.LogWarning("Tenant for user ID {UserId} not found or inactive", userId);
                }
                else
                {
                    _logger.LogInformation("Successfully retrieved tenant for user ID {UserId}", userId);
                }

                return tenant;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tenant for user ID {UserId}", userId);
                throw;
            }
        }

        public async Task<Tenant> CreateTenantAsync(Tenant tenant)
        {
            if (tenant == null)
            {
                _logger.LogWarning("Attempted to create null tenant");
                throw new ArgumentNullException(nameof(tenant));
            }

            if (tenant.UserId == Guid.Empty)
            {
                _logger.LogWarning("Invalid UserId for tenant creation");
                throw new ArgumentException("UserId cannot be empty", nameof(tenant.UserId));
            }

            _logger.LogInformation("Creating tenant for user ID {UserId}", tenant.UserId);
            try
            {
                _context.Tenants.Add(tenant);
                await _context.SaveChangesAsync();

                var user = await _userManager.FindByIdAsync(tenant.UserId.ToString());
                if (user != null)
                {
                    var result = await _userManager.AddToRoleAsync(user, "Tenant");
                    if (!result.Succeeded)
                    {
                        _logger.LogWarning("Failed to assign Tenant role to user ID {UserId}: {Errors}",
                            tenant.UserId, string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                    else
                    {
                        _logger.LogInformation("Successfully assigned Tenant role to user ID {UserId}", tenant.UserId);
                    }
                }
                else
                {
                    _logger.LogWarning("User with ID {UserId} not found for tenant role assignment", tenant.UserId);
                }

                _logger.LogInformation("Successfully created tenant with ID {TenantId}", tenant.Id);
                return tenant;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating tenant for user ID {UserId}", tenant.UserId);
                throw;
            }
        }

        public async Task<Tenant?> UpdateTenantAsync(Tenant tenant)
        {
            if (tenant == null)
            {
                _logger.LogWarning("Attempted to update null tenant");
                return null;
            }

            if (tenant.Id <= 0)
            {
                _logger.LogWarning("Invalid tenant ID: {TenantId}", tenant.Id);
                return null;
            }

            _logger.LogInformation("Updating tenant with ID {TenantId}", tenant.Id);
            try
            {
                var existingTenant = await _context.Tenants.FindAsync(tenant.Id);
                if (existingTenant == null)
                {
                    _logger.LogWarning("Tenant with ID {TenantId} not found", tenant.Id);
                    return null;
                }

                existingTenant.HouseId = tenant.HouseId;
                existingTenant.LeaseStartDate = tenant.LeaseStartDate;
                existingTenant.LeaseEndDate = tenant.LeaseEndDate;
                existingTenant.SecurityDeposit = tenant.SecurityDeposit;
                existingTenant.IsActive = tenant.IsActive;

                await _context.SaveChangesAsync();
                _logger.LogInformation("Successfully updated tenant with ID {TenantId}", tenant.Id);
                return existingTenant;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating tenant with ID {TenantId}", tenant.Id);
                throw;
            }
        }

        public async Task<bool> DeleteTenantAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid tenant ID: {TenantId}", id);
                return false;
            }

            _logger.LogInformation("Deleting tenant with ID {TenantId}", id);
            try
            {
                var tenant = await _context.Tenants.FindAsync(id);
                if (tenant == null)
                {
                    _logger.LogWarning("Tenant with ID {TenantId} not found", id);
                    return false;
                }

                tenant.IsActive = false;
                await _context.SaveChangesAsync();
                _logger.LogInformation("Successfully deactivated tenant with ID {TenantId}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting tenant with ID {TenantId}", id);
                throw;
            }
        }

        public async Task<bool> AssignToHouseAsync(int tenantId, int houseId)
        {
            if (tenantId <= 0 || houseId <= 0)
            {
                _logger.LogWarning("Invalid tenant ID {TenantId} or house ID {HouseId}", tenantId, houseId);
                return false;
            }

            _logger.LogInformation("Assigning tenant {TenantId} to house {HouseId}", tenantId, houseId);
            try
            {
                var result = await _context.AssignTenantToHouseAsync(tenantId, houseId);
                if (result)
                {
                    _logger.LogInformation("Successfully assigned tenant {TenantId} to house {HouseId}", tenantId, houseId);
                }
                else
                {
                    _logger.LogWarning("Failed to assign tenant {TenantId} to house {HouseId}", tenantId, houseId);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning tenant {TenantId} to house {HouseId}", tenantId, houseId);
                throw;
            }
        }

        public async Task<TenantBalanceDto?> GetTenantBalanceAsync(int tenantId)
        {
            if (tenantId <= 0)
            {
                _logger.LogWarning("Invalid tenant ID: {TenantId}", tenantId);
                return null;
            }

            _logger.LogInformation("Retrieving balance for tenant {TenantId}", tenantId);
            try
            {
                var balance = await _context.GetTenantBalanceAsync(tenantId);
                if (balance == null)
                {
                    _logger.LogWarning("Balance not found for tenant {TenantId}", tenantId);
                }
                else
                {
                    _logger.LogInformation("Successfully retrieved balance for tenant {TenantId}", tenantId);
                }
                return balance;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving balance for tenant {TenantId}", tenantId);
                throw;
            }
        }

        public async Task<PaymentSummaryDto?> GetPaymentSummaryAsync(int tenantId, DateTime fromDate, DateTime toDate)
        {
            if (tenantId <= 0)
            {
                _logger.LogWarning("Invalid tenant ID: {TenantId}", tenantId);
                return null;
            }

            if (fromDate > toDate)
            {
                _logger.LogWarning("Invalid date range: FromDate {FromDate} is after ToDate {ToDate}", fromDate, toDate);
                return null;
            }

            _logger.LogInformation("Retrieving payment summary for tenant {TenantId} from {FromDate} to {ToDate}", tenantId, fromDate, toDate);
            try
            {
                var summary = await _context.GetTenantPaymentSummaryAsync(tenantId, fromDate, toDate);
                if (summary == null)
                {
                    _logger.LogWarning("Payment summary not found for tenant {TenantId}", tenantId);
                    return new PaymentSummaryDto
                    {
                        TotalPayments = 0,
                        TotalAmount = 0,
                        FromDate = fromDate,
                        ToDate = toDate,
                        Payments = new List<Payment>()
                    };
                }

                _logger.LogInformation("Successfully retrieved payment summary for tenant {TenantId} with {Count} payments", tenantId, summary.TotalPayments);
                return summary;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving payment summary for tenant {TenantId} from {FromDate} to {ToDate}", tenantId, fromDate, toDate);
                throw;
            }
        }
    }
}
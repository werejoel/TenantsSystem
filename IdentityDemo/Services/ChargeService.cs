using Microsoft.EntityFrameworkCore;
using TenantsManagementApp.Data;
using TenantsManagementApp.Models;

namespace TenantsManagementApp.Services
{
    public class ChargeService : IChargeService
    {
        private readonly ApplicationDbContext _context;
        public ChargeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Charge>> GetChargesByTenantAsync(int tenantId)
        {
            return await _context.Charges
                .Include(c => c.House)
                .Include(c => c.PaymentCharges)
                .Where(c => c.TenantId == tenantId)
                .OrderByDescending(c => c.DueDate)
                .ToListAsync();
        }

        public async Task<Charge> CreateChargeAsync(Charge charge)
        {
            _context.Charges.Add(charge);
            await _context.SaveChangesAsync();
            return charge;
        }

        public async Task<List<Charge>> GetOverdueChargesAsync()
        {
            return await _context.Charges
                .Include(c => c.House)
                .Include(c => c.Tenant)
                .Where(c => c.DueDate < DateTime.Now && c.Status != "Paid")
                .OrderBy(c => c.DueDate)
                .ToListAsync();
        }

        public async Task<int> CreateMonthlyRentChargesAsync(DateTime forMonth)
        {
            // Get all active tenants with their houses
            var activeTenants = await _context.Tenants
                .Include(t => t.House)
                .Where(t => t.IsActive)
                .ToListAsync();

            var chargesCreated = 0;
            var dueDate = new DateTime(forMonth.Year, forMonth.Month, DateTime.DaysInMonth(forMonth.Year, forMonth.Month));

            foreach (var tenant in activeTenants)
            {
                // Check if charge already exists for this month
                var existingCharge = await _context.Charges
                    .FirstOrDefaultAsync(c => c.TenantId == tenant.Id &&
                                            c.ChargeType == "Rent" &&
                                            c.DueDate.Month == forMonth.Month &&
                                            c.DueDate.Year == forMonth.Year);

                if (existingCharge == null && tenant.House != null && tenant.HouseId.HasValue)
                {
                    var charge = new Charge
                    {
                        TenantId = tenant.Id,
                        HouseId = tenant.HouseId.Value, // Use .Value to get the int from int?
                        Amount = tenant.House.Price, // Using Price from House model
                        ChargeType = "Rent",
                        Description = $"Monthly rent for {forMonth:MMMM yyyy}",
                        DueDate = dueDate,
                        Status = "Pending"
                        // CreatedAt and UpdatedAt will be set automatically by your UpdateTimestamps method
                    };

                    _context.Charges.Add(charge);
                    chargesCreated++;
                }
            }

            await _context.SaveChangesAsync();
            return chargesCreated;
        }

        public async Task<bool> MarkChargeAsPaidAsync(int chargeId)
        {
            var charge = await _context.Charges.FindAsync(chargeId);
            if (charge == null) return false;

            charge.Status = "Paid";
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
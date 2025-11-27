using TenantsManagementApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace TenantsManagementApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataSeedingController : ControllerBase
    {
        private readonly IServiceProvider _services;
        public DataSeedingController(IServiceProvider services)
        {
            _services = services;
        }
        [HttpPost("seed-dummy-users")]
        public async Task<IActionResult> SeedDummyUsers()
        {
            await IdentityUserSeeder.SeedUsersAsync(_services);
            return Ok("Dummy users have been seeded successfully.");
        }
        [HttpPost("seed-dummy-claims")]
        public async Task<IActionResult> SeedDummyClaims()
        {
            await ClaimSeeder.SeedClaimsMaster(_services);
            return Ok("Dummy claims have been seeded successfully.");
        }

        [HttpPost("seed-test-charges")]
        public async Task<IActionResult> SeedTestCharges()
        {
            try
            {
                using (var scope = _services.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    
                    // Get the first tenant that doesn't have charges
                    var tenant = await dbContext.Tenants.FirstOrDefaultAsync(t => t.IsActive && !t.Charges.Any());
                    if (tenant == null)
                    {
                        return BadRequest("No eligible tenant found or all tenants already have charges");
                    }

                    // Get a house (create one if needed)
                    var house = await dbContext.Houses.FirstOrDefaultAsync();
                    if (house == null)
                    {
                        return BadRequest("No houses found in the database");
                    }

                    // Create sample charges
                    var charges = new List<TenantsManagementApp.Models.Charge>
                    {
                        new TenantsManagementApp.Models.Charge
                        {
                            TenantId = tenant.Id,
                            HouseId = house.Id,
                            ChargeType = "Rent - November",
                            Amount = 850000,
                            DueDate = DateTime.Now.AddDays(3),
                            Status = "Pending",
                            CreatedAt = DateTime.Now
                        },
                        new TenantsManagementApp.Models.Charge
                        {
                            TenantId = tenant.Id,
                            HouseId = house.Id,
                            ChargeType = "Water Bill",
                            Amount = 48000,
                            DueDate = DateTime.Now.AddDays(1),
                            Status = "Pending",
                            CreatedAt = DateTime.Now
                        },
                        new TenantsManagementApp.Models.Charge
                        {
                            TenantId = tenant.Id,
                            HouseId = house.Id,
                            ChargeType = "Electricity",
                            Amount = 92000,
                            DueDate = DateTime.Now.AddDays(5),
                            Status = "Pending",
                            CreatedAt = DateTime.Now
                        }
                    };

                    await dbContext.Charges.AddRangeAsync(charges);
                    await dbContext.SaveChangesAsync();

                    return Ok(new { message = "Test charges created successfully", tenantId = tenant.Id, chargeCount = charges.Count });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TenantsManagementApp.DTOS;
using TenantsManagementApp.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace TenantsManagementApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        private readonly ILogger<ApplicationDbContext> _logger;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ILogger<ApplicationDbContext> logger = null)
            : base(options)
        {
            _logger = logger;
        }

        // TMS DbSets
        public DbSet<Address> Addresses { get; set; }
        public DbSet<ClaimMaster> ClaimMasters { get; set; }
        public DbSet<Landlord> Landlords { get; set; }
        public DbSet<House> Houses { get; set; }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Charge> Charges { get; set; }
        public DbSet<PaymentCharge> PaymentCharges { get; set; }
        public DbSet<MaintenanceRequest> MaintenanceRequests { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Renaming Identity Tables
            builder.Entity<ApplicationUser>().ToTable("Users");
            builder.Entity<ApplicationRole>().ToTable("Roles");
            builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
            builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
            builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");
            builder.Entity<ClaimMaster>().ToTable("ClaimMasters");

            // Configure ApplicationUser entity
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.HasIndex(e => e.NationalId)
                      .IsUnique()
                      .HasFilter("[NationalId] IS NOT NULL");

                entity.HasOne(u => u.Tenant)
                      .WithOne(t => t.User)
                      .HasForeignKey<Tenant>(t => t.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.UserRoles)
                      .WithOne()
                      .HasForeignKey(ur => ur.UserId)
                      .IsRequired();
            });

            // Landlord entity
            builder.Entity<Landlord>(entity =>
            {
                entity.HasIndex(e => e.Email)
                      .IsUnique()
                      .HasFilter("[Email] IS NOT NULL");
            });

            // House entity
            builder.Entity<House>(entity =>
            {
                entity.HasOne(h => h.Landlord)
                      .WithMany(l => l.Houses)
                      .HasForeignKey(h => h.LandlordId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(h => h.Price).HasPrecision(18, 2);
            });

            // Tenant entity
            builder.Entity<Tenant>(entity =>
            {
                entity.HasOne(t => t.House)
                      .WithMany(h => h.Tenants)
                      .HasForeignKey(t => t.HouseId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.Property(t => t.SecurityDeposit).HasPrecision(18, 2);
            });

            // Payment entity
            builder.Entity<Payment>(entity =>
            {
                entity.HasOne(p => p.Tenant)
                      .WithMany(t => t.Payments)
                      .HasForeignKey(p => p.TenantId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.House)
                      .WithMany(h => h.Payments)
                      .HasForeignKey(p => p.HouseId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(p => p.AmountPaid).HasPrecision(18, 2);
            });

            // Charge entity
            builder.Entity<Charge>(entity =>
            {
                entity.HasOne(c => c.Tenant)
                      .WithMany(t => t.Charges)
                      .HasForeignKey(c => c.TenantId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.House)
                      .WithMany(h => h.Charges)
                      .HasForeignKey(c => c.HouseId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(c => c.Amount).HasPrecision(18, 2);
            });

            // PaymentCharge junction entity
            builder.Entity<PaymentCharge>(entity =>
            {
                entity.HasOne(pc => pc.Payment)
                      .WithMany(p => p.PaymentCharges)
                      .HasForeignKey(pc => pc.PaymentId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(pc => pc.Charge)
                      .WithMany(c => c.PaymentCharges)
                      .HasForeignKey(pc => pc.ChargeId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(pc => pc.AmountPaid).HasPrecision(18, 2);
                entity.HasIndex(pc => new { pc.PaymentId, pc.ChargeId }).IsUnique();
            });

            // MaintenanceRequest entity
            builder.Entity<MaintenanceRequest>(entity =>
            {
                entity.HasOne(mr => mr.Tenant)
                      .WithMany(t => t.MaintenanceRequests)
                      .HasForeignKey(mr => mr.TenantId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(mr => mr.House)
                      .WithMany(h => h.MaintenanceRequests)
                      .HasForeignKey(mr => mr.HouseId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Document entity
            builder.Entity<Document>(entity =>
            {
                entity.HasOne(d => d.Tenant)
                      .WithMany(t => t.Documents)
                      .HasForeignKey(d => d.TenantId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.House)
                      .WithMany(h => h.Documents)
                      .HasForeignKey(d => d.HouseId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasCheckConstraint("CK_Document_TenantOrHouse",
                    "[TenantId] IS NOT NULL OR [HouseId] IS NOT NULL");
            });

            // Notification entity
            builder.Entity<Notification>(entity =>
            {
                entity.HasOne(n => n.User)
                      .WithMany(u => u.Notifications)
                      .HasForeignKey(n => n.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Seed initial Roles using HasData
            var adminRoleId = Guid.Parse("c8d89a25-4b96-4f20-9d79-7f8a54c5213d");
            var userRoleId = Guid.Parse("b92f0a3e-573b-4b12-8db1-2ccf6d58a34a");
            var managerRoleId = Guid.Parse("d7f4a42e-1c1b-4c9f-8a50-55f6b234e8e2");
            var guestRoleId = Guid.Parse("f2e6b8a1-9d43-4a7c-9f32-71d7c5dbe9f0");
            var tenantRoleId = Guid.Parse("a3c7e92f-4d6b-4e8a-9c1f-7e5a8b9d2c4e");

            builder.Entity<ApplicationRole>().HasData(
                new ApplicationRole
                {
                    Id = adminRoleId,
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    Description = "Administrator role with full permissions.",
                    IsActive = true,
                    CreatedOn = new DateTime(2025, 8, 4),
                    ModifiedOn = new DateTime(2025, 8, 4)
                },
                new ApplicationRole
                {
                    Id = userRoleId,
                    Name = "User",
                    NormalizedName = "USER",
                    Description = "Standard user role.",
                    IsActive = true,
                    CreatedOn = new DateTime(2025, 8, 4),
                    ModifiedOn = new DateTime(2025, 8, 4)
                },
                new ApplicationRole
                {
                    Id = managerRoleId,
                    Name = "Manager",
                    NormalizedName = "MANAGER",
                    Description = "Manager role with moderate permissions.",
                    IsActive = true,
                    CreatedOn = new DateTime(2025, 8, 4),
                    ModifiedOn = new DateTime(2025, 8, 4)
                },
                new ApplicationRole
                {
                    Id = guestRoleId,
                    Name = "Guest",
                    NormalizedName = "GUEST",
                    Description = "Guest role with limited access.",
                    IsActive = true,
                    CreatedOn = new DateTime(2025, 8, 4),
                    ModifiedOn = new DateTime(2025, 8, 4)
                },
                new ApplicationRole
                {
                    Id = tenantRoleId,
                    Name = "Tenant",
                    NormalizedName = "TENANT",
                    Description = "Tenant role for property renters.",
                    IsActive = true,
                    CreatedOn = new DateTime(2025, 8, 4),
                    ModifiedOn = new DateTime(2025, 8, 4)
                }
            );

            // Seed sample data for TMS
            SeedTmsData(builder);
        }

        private void SeedTmsData(ModelBuilder builder)
        {
            var adminUserId = Guid.Parse("a1b2c3d4-e5f6-7890-1234-567890abcdef");
            var hasher = new PasswordHasher<ApplicationUser>();

            builder.Entity<ApplicationUser>().HasData(
                new ApplicationUser
                {
                    Id = adminUserId,
                    UserName = "admin@tms.com",
                    NormalizedUserName = "ADMIN@TMS.COM",
                    Email = "admin@tms.com",
                    NormalizedEmail = "ADMIN@TMS.COM",
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(null, "Admin@123"),
                    SecurityStamp = Guid.NewGuid().ToString(),
                    FirstName = "System",
                    LastName = "Administrator",
                    PhoneNumber = "+256700000000",
                    IsActive = true,
                    CreatedOn = DateTime.UtcNow,
                    ModifiedOn = DateTime.UtcNow
                }
            );

            builder.Entity<IdentityUserRole<Guid>>().HasData(
                new IdentityUserRole<Guid>
                {
                    RoleId = Guid.Parse("c8d89a25-4b96-4f20-9d79-7f8a54c5213d"),
                    UserId = adminUserId
                }
            );

            builder.Entity<Landlord>().HasData(
                new Landlord
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@email.com",
                    Phone = "+256700000001",
                    Address = "Kampala, Uganda",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            );

            builder.Entity<House>().HasData(
                new House
                {
                    Id = 1,
                    LandlordId = 1,
                    Name = "House A1",
                    Location = "Ntinda, Kampala",
                    Model = "Single Room",
                    Price = 300000m,
                    Status = "Vacant",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new House
                {
                    Id = 2,
                    LandlordId = 1,
                    Name = "House A2",
                    Location = "Ntinda, Kampala",
                    Model = "Double Room",
                    Price = 450000m,
                    Status = "Vacant",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new House
                {
                    Id = 3,
                    LandlordId = 1,
                    Name = "House A3",
                    Location = "Ndejje, Kampala",
                    Model = "Single Room",
                    Price = 600000m,
                    Status = "Vacant",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new House
                {
                    Id = 4,
                    LandlordId = 1,
                    Name = "House A4",
                    Location = "Nsambya, Kampala",
                    Model = "Double Room",
                    Price = 450000m,
                    Status = "Vacant",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            );
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                if (entityEntry.State == EntityState.Added)
                {
                    ((BaseEntity)entityEntry.Entity).CreatedAt = DateTime.UtcNow;
                }
                ((BaseEntity)entityEntry.Entity).UpdatedAt = DateTime.UtcNow;
            }

            var userEntries = ChangeTracker.Entries<ApplicationUser>()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var userEntry in userEntries)
            {
                if (userEntry.State == EntityState.Added && !userEntry.Entity.CreatedOn.HasValue)
                {
                    userEntry.Entity.CreatedOn = DateTime.UtcNow;
                }
                userEntry.Entity.ModifiedOn = DateTime.UtcNow;
            }
        }

        public async Task<bool> AssignTenantToHouseAsync(int tenantId, int houseId)
        {
            if (tenantId <= 0 || houseId <= 0)
            {
                _logger?.LogWarning("Invalid tenant ID {TenantId} or house ID {HouseId}", tenantId, houseId);
                return false;
            }

            _logger?.LogInformation("Attempting to assign tenant {TenantId} to house {HouseId}", tenantId, houseId);
            try
            {
                var tenant = await Tenants.FindAsync(tenantId);
                var house = await Houses.FindAsync(houseId);

                if (tenant == null || !tenant.IsActive || house == null || !house.IsActive)
                {
                    _logger?.LogWarning("Tenant {TenantId} or house {HouseId} not found or inactive", tenantId, houseId);
                    return false;
                }

                if (house.Status != "Vacant")
                {
                    _logger?.LogWarning("House {HouseId} is not vacant", houseId);
                    return false;
                }

                tenant.HouseId = houseId;
                house.Status = "Occupied";

                await SaveChangesAsync();
                _logger?.LogInformation("Successfully assigned tenant {TenantId} to house {HouseId}", tenantId, houseId);
                return true;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error assigning tenant {TenantId} to house {HouseId}", tenantId, houseId);
                throw;
            }
        }

        public async Task<TenantBalanceDto?> GetTenantBalanceAsync(int tenantId)
        {
            if (tenantId <= 0)
            {
                _logger?.LogWarning("Invalid tenant ID: {TenantId}", tenantId);
                return null;
            }

            _logger?.LogInformation("Calculating balance for tenant {TenantId}", tenantId);
            try
            {
                var tenant = await Tenants
                    .Include(t => t.User)
                    .Include(t => t.Charges)
                    .Include(t => t.Payments)
                    .ThenInclude(p => p.PaymentCharges)
                    .FirstOrDefaultAsync(t => t.Id == tenantId && t.IsActive);

                if (tenant == null)
                {
                    _logger?.LogWarning("Tenant {TenantId} not found or inactive", tenantId);
                    return null;
                }

                decimal totalCharges = tenant.Charges
                    .Where(c => c.Status != "Paid")
                    .Sum(c => c.Amount);

                decimal totalPayments = tenant.Payments
                    .SelectMany(p => p.PaymentCharges)
                    .Sum(pc => pc.AmountPaid);

                decimal balance = totalCharges - totalPayments;
                decimal outstandingBalance = balance > 0 ? balance : 0;
                decimal overpaymentAmount = balance < 0 ? Math.Abs(balance) : 0;

                DateTime? lastPaymentDate = tenant.Payments
                    .OrderByDescending(p => p.PaymentDate)
                    .Select(p => p.PaymentDate)
                    .FirstOrDefault();

                var overdueCharges = tenant.Charges
                    .Where(c => c.IsOverdue)
                    .ToList();

                var tenantName = $"{tenant.User.FirstName ?? ""} {tenant.User.LastName ?? ""}".Trim();
                if (string.IsNullOrEmpty(tenantName))
                {
                    _logger?.LogWarning("Tenant {TenantId} has no valid name", tenantId);
                    tenantName = "Unknown Tenant";
                }

                return new TenantBalanceDto
                {
                    TenantId = tenantId,
                    TenantName = tenantName,
                    TotalCharges = totalCharges,
                    TotalPayments = totalPayments,
                    OutstandingBalance = outstandingBalance,
                    OverpaymentAmount = overpaymentAmount,
                    LastPaymentDate = lastPaymentDate ?? DateTime.MinValue,
                    OverdueCharges = overdueCharges
                };
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error calculating balance for tenant {TenantId}", tenantId);
                throw;
            }
        }

        public async Task<PaymentSummaryDto?> GetPaymentSummaryAsync(DateTime fromDate, DateTime toDate)
        {
            if (fromDate > toDate)
            {
                _logger?.LogWarning("Invalid date range: FromDate {FromDate} is after ToDate {ToDate}", fromDate, toDate);
                return null;
            }

            _logger?.LogInformation("Retrieving payment summary from {FromDate} to {ToDate}", fromDate, toDate);
            try
            {
                var payments = await Payments
                    .Include(p => p.PaymentCharges)
                    .Where(p => p.PaymentDate >= fromDate && p.PaymentDate <= toDate)
                    .ToListAsync();

                var totalPayments = payments.Count;

                var totalAmount = payments
                    .SelectMany(p => p.PaymentCharges)
                    .Sum(pc => pc.AmountPaid);

                return new PaymentSummaryDto
                {
                    TotalPayments = totalPayments,
                    TotalAmount = totalAmount,
                    FromDate = fromDate,
                    ToDate = toDate,
                    Payments = payments
                };
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error retrieving payment summary from {FromDate} to {ToDate}", fromDate, toDate);
                throw;
            }
        }

        public async Task<PaymentSummaryDto?> GetTenantPaymentSummaryAsync(int tenantId, DateTime fromDate, DateTime toDate)
        {
            if (tenantId <= 0)
            {
                _logger?.LogWarning("Invalid tenant ID: {TenantId}", tenantId);
                return null;
            }

            if (fromDate > toDate)
            {
                _logger?.LogWarning("Invalid date range: FromDate {FromDate} is after ToDate {ToDate}", fromDate, toDate);
                return null;
            }

            _logger?.LogInformation("Retrieving payment summary for tenant {TenantId} from {FromDate} to {ToDate}", tenantId, fromDate, toDate);
            try
            {
                var tenant = await Tenants
                    .Include(t => t.Payments)
                    .ThenInclude(p => p.PaymentCharges)
                    .FirstOrDefaultAsync(t => t.Id == tenantId && t.IsActive);

                if (tenant == null)
                {
                    _logger?.LogWarning("Tenant {TenantId} not found or inactive", tenantId);
                    return null;
                }

                var payments = tenant.Payments
                    .Where(p => p.PaymentDate >= fromDate && p.PaymentDate <= toDate)
                    .ToList();

                var totalPayments = payments.Count;

                var totalAmount = payments
                    .SelectMany(p => p.PaymentCharges)
                    .Sum(pc => pc.AmountPaid);

                return new PaymentSummaryDto
                {
                    TotalPayments = totalPayments,
                    TotalAmount = totalAmount,
                    FromDate = fromDate,
                    ToDate = toDate,
                    Payments = payments
                };
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error retrieving payment summary for tenant {TenantId} from {FromDate} to {ToDate}", tenantId, fromDate, toDate);
                throw;
            }
        }
    }
}
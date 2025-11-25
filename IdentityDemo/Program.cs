using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TenantsManagementApp.Data;
using TenantsManagementApp.Models;
using TenantsManagementApp.Services;

namespace TenantsManagementApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            // Add services to the container.
            builder.Services.AddControllersWithViews(options =>
            {
                options.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter());
            });
            //DbContext
            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                //// Password settings
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = true; // Must include digits
                options.Password.RequireLowercase = true; // Must include lowercase letters
                options.Password.RequireUppercase = true;// Must include uppercase letters
                options.Password.RequireNonAlphanumeric = true;// Must include special characters
                options.Password.RequiredLength = 6;// Minimum length 6       
                options.Password.RequiredUniqueChars = 4;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // Set token valid for 30 minutes
            builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromMinutes(30);
            });

            // Registering Custom Services
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IClaimsService, ClaimsService>();

            // Domain services used by the tenant dashboard
            builder.Services.AddScoped<ITenantService, TenantService>();
            builder.Services.AddScoped<IMaintenanceService, MaintenanceService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<INotificationService, NotificationService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<FlutterwaveService>();
            builder.Services.AddHttpClient();
            builder.Services.AddHangfireServer();



            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";  
                options.AccessDeniedPath = "/Account/AccessDenied";
            });


            builder.Services.AddHangfire(config => config
           .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
           .UseSimpleAssemblyNameTypeSerializer()
           .UseDefaultTypeSerializer()
           .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Defining Hybrid Policies (Roles + Claims)
            builder.Services.AddAuthorization(options =>
            {
                // ===== Read users =====
                // Allow: Admins or Managers (broad access)
                options.AddPolicy("ViewUsersPolicy", policy =>
                    policy.RequireAssertion(ctx =>
                        ctx.User.IsInRole("Admin") ||
                        ctx.User.IsInRole("Manager") ||
                        ctx.User.HasClaim("Permission", "ViewUsers")));

                // ===== Create users =====
                // Allow: Admins OR explicit AddUser permission
                options.AddPolicy("AddUserPolicy", policy =>
                    policy.RequireAssertion(ctx =>
                        ctx.User.IsInRole("Admin") ||
                        ctx.User.HasClaim("Permission", "AddUser")));

                // ===== Edit users =====
                // Allow: Admins OR explicit EditUser permission
                options.AddPolicy("EditUserPolicy", policy =>
                    policy.RequireAssertion(ctx =>
                        ctx.User.IsInRole("Admin") ||
                        ctx.User.HasClaim("Permission", "EditUser")));

                // ===== Delete users =====
                // Allow: Admins AND explicit DeleteUser permission
                options.AddPolicy("DeleteUserPolicy", policy =>
                    policy.RequireAssertion(ctx =>
                        ctx.User.IsInRole("Admin") &&
                        ctx.User.HasClaim("Permission", "DeleteUser")));

                // ===== Manage users =====
                // Allow: Admins OR (EditUser OR ManageUsers) permission
                options.AddPolicy("ManageUsersPolicy", policy =>
                    policy.RequireAssertion(ctx =>
                        ctx.User.IsInRole("Admin") ||
                        ctx.User.HasClaim("Permission", "EditUser") ||
                        ctx.User.HasClaim("Permission", "ManageUsers")));

                // ===== Manage roles on users (sensitive) =====
                // Require: Admin role AND the full role-management permission set
                // (Change to suit your model; this keeps least-privilege explicit.)
                options.AddPolicy("ManageRolesPolicy", policy =>
                    policy.RequireAssertion(ctx =>
                        ctx.User.IsInRole("Admin") &&
                        ctx.User.HasClaim("Permission", "AddRole") &&
                        ctx.User.HasClaim("Permission", "EditRole") &&
                        ctx.User.HasClaim("Permission", "DeleteRole")));

                // ===== Manage user-claims =====
                // Allow: Admins OR explicit ManageUserClaims permission
                options.AddPolicy("ManageUserClaimsPolicy", policy =>
                     policy.RequireAssertion(ctx =>
                     ctx.User.IsInRole("Admin") ||
                     ctx.User.HasClaim("Permission", "ManageUserClaims")));
            });
            // Logging
            builder.Services.AddLogging(Logging =>
            {
                Logging.AddConsole();
                Logging.AddDebug();
            });

            var app = builder.Build();

            // HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseHangfireDashboard();
            app.UseAuthentication(); // Enables the authentication
            app.UseAuthorization();  // Enables authorization
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}");

            app.Run();
        }

    }
}
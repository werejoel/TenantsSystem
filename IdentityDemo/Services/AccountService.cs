using TenantsManagementApp.Services;
using TenantsManagementApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using TenantsManagementApp.Models;

namespace TenantsManagementApp.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public AccountService(UserManager<ApplicationUser> userManager,
                              SignInManager<ApplicationUser> signInManager,
                              RoleManager<ApplicationRole> roleManager,
                              IEmailService emailService,
                              IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task<IdentityResult> RegisterUserAsync(RegisterViewModel model)
        {
            // Generate a 6-digit confirmation code
            var confirmationCode = new Random().Next(100000, 999999).ToString();
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                DateOfBirth = model.DateOfBirth,
                IsActive = true,
                PhoneNumber = model.PhoneNumber,
                CreatedOn = DateTime.UtcNow,
                EmailConfirmationCode = confirmationCode,
                EmailConfirmationCodeExpiresOn = DateTime.UtcNow.AddMinutes(10),
                LastConfirmationCodeResendOn = DateTime.UtcNow
            };


            IdentityResult result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return result;

            // Assign selected role from registration form
            var selectedRole = string.IsNullOrWhiteSpace(model.Role) ? "User" : model.Role;

            // Ensure the role exists (create it if missing) to avoid AddToRoleAsync failures
            if (!await _roleManager.RoleExistsAsync(selectedRole))
            {
                var newRole = new ApplicationRole
                {
                    Id = Guid.NewGuid(),
                    Name = selectedRole,
                    NormalizedName = selectedRole.ToUpperInvariant(),
                    IsActive = true,
                    CreatedOn = DateTime.UtcNow,
                    ModifiedOn = DateTime.UtcNow
                };
                await _roleManager.CreateAsync(newRole);
            }

            IdentityResult roleAssignResult = await _userManager.AddToRoleAsync(user, selectedRole);
            if (!roleAssignResult.Succeeded)
            {
                // Return the role assignment failure so the controller can show errors
                return roleAssignResult;
            }

            // Send confirmation code to user's email
            bool emailSent = true;
            try
            {
                await _emailService.SendEmailConfirmationCodeAsync(user.Email, user.FirstName, user.EmailConfirmationCode);
            }
            catch
            {
                emailSent = false;
            }

            // Return a tuple-like result (IdentityResult + emailSent)
            if (emailSent)
                return result;
            else
                return IdentityResult.Success; // Indicate user creation succeeded, but email failed
        }

        public async Task<IdentityResult> ConfirmEmailAsync(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                return IdentityResult.Failed(new IdentityError { Description = "Invalid token or user ID." });

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });

            var decodedBytes = WebEncoders.Base64UrlDecode(token);
            var decodedToken = Encoding.UTF8.GetString(decodedBytes);

            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (result.Succeeded)
            {
                var baseUrl = _configuration["AppSettings:BaseUrl"] ?? throw new InvalidOperationException("BaseUrl is not configured.");
                var loginLink = $"{baseUrl}/Account/Login";

                await _emailService.SendAccountCreatedEmailAsync(user.Email!, user.FirstName!, loginLink);
            }

            return result;
        }

        public async Task<IdentityResult> ConfirmEmailWithCodeAsync(Guid userId, string code)
        {
            if (userId == Guid.Empty || string.IsNullOrEmpty(code))
                return IdentityResult.Failed(new IdentityError { Description = "Invalid code or user ID." });

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });

            // Check if the code matches and is not expired
            if (user.EmailConfirmationCode != code)
                return IdentityResult.Failed(new IdentityError { Description = "Invalid confirmation code." });

            if (user.EmailConfirmationCodeExpiresOn < DateTime.UtcNow)
                return IdentityResult.Failed(new IdentityError { Description = "Confirmation code has expired." });

            // Mark email as confirmed
            user.EmailConfirmed = true;
            user.EmailConfirmationCode = null;
            user.EmailConfirmationCodeExpiresOn = null;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                var baseUrl = _configuration["AppSettings:BaseUrl"] ?? throw new InvalidOperationException("BaseUrl is not configured.");
                var loginLink = $"{baseUrl}/Account/Login";

                await _emailService.SendAccountCreatedEmailAsync(user.Email!, user.FirstName!, loginLink);
            }

            return result;
        }

        public async Task<SignInResult> LoginUserAsync(LoginViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return SignInResult.Failed;

            if (!await _userManager.IsEmailConfirmedAsync(user))
                return SignInResult.NotAllowed;

            var result = await _signInManager.PasswordSignInAsync(user.UserName!, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                // Update LastLogin
                user.LastLogin = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);
            }

            return result;
        }

        public async Task LogoutUserAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task SendEmailConfirmationAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.", nameof(email));

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // Prevent user enumeration by not disclosing existence
                return;
            }

            if (await _userManager.IsEmailConfirmedAsync(user))
            {
                // Email already confirmed; no action needed
                return;
            }

            var token = await GenerateEmailConfirmationTokenAsync(user);

            var baseUrl = _configuration["AppSettings:BaseUrl"] ?? throw new InvalidOperationException("BaseUrl is not configured.");
            var confirmationLink = $"{baseUrl}/Account/ConfirmEmail?userId={user.Id}&token={token}";

            await _emailService.SendResendConfirmationEmailAsync(user.Email!, user.FirstName!, confirmationLink);
        }

        public async Task SendEmailConfirmationCodeAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.", nameof(email));

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return; // Don't reveal if user exists

            if (await _userManager.IsEmailConfirmedAsync(user))
                return; // Already confirmed

            // Generate new code if needed
            if (string.IsNullOrEmpty(user.EmailConfirmationCode) ||
                user.EmailConfirmationCodeExpiresOn < DateTime.UtcNow)
            {
                user.EmailConfirmationCode = new Random().Next(100000, 999999).ToString();
                user.EmailConfirmationCodeExpiresOn = DateTime.UtcNow.AddMinutes(10);
                user.LastConfirmationCodeResendOn = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);
            }

            await _emailService.SendEmailConfirmationCodeAsync(user.Email!, user.FirstName!, user.EmailConfirmationCode);
        }

        public async Task<bool> ResendConfirmationCodeAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || await _userManager.IsEmailConfirmedAsync(user))
                return false;

            // Check if enough time has passed since last resend (prevent spam)
            if (user.LastConfirmationCodeResendOn.HasValue &&
                user.LastConfirmationCodeResendOn.Value.AddMinutes(1) > DateTime.UtcNow)
                return false; // Too soon to resend

            // Generate new code
            user.EmailConfirmationCode = new Random().Next(100000, 999999).ToString();
            user.EmailConfirmationCodeExpiresOn = DateTime.UtcNow.AddMinutes(10);
            user.LastConfirmationCodeResendOn = DateTime.UtcNow;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return false;

            try
            {
                await _emailService.SendEmailConfirmationCodeAsync(user.Email!, user.FirstName!, user.EmailConfirmationCode);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<ProfileViewModel> GetUserProfileByEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentException("Email cannot be null or empty.", nameof(email));

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                throw new ArgumentException("User not found.", nameof(email));

            return new ProfileViewModel
            {
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                LastLoggedIn = user.LastLogin,
                CreatedOn = user.CreatedOn,
                DateOfBirth = user.DateOfBirth
            };
        }

        //Helper Method
        private async Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            return encodedToken;
        }
    }
}
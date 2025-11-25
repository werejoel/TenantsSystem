using TenantsManagementApp.Services;
using TenantsManagementApp.ViewModels;
using TenantsManagementApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace TenantsManagementApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<AccountController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(IAccountService accountService, ILogger<AccountController> logger, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _accountService = accountService;
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            var model = new RegisterViewModel
            {
                AvailableRoles = new List<SelectListItem>
                {
                    new SelectListItem { Value = "Tenant", Text = "Tenant" },
                    new SelectListItem { Value = "Landlord", Text = "Landlord" }
                    //more roles to be added if needed
                }
            };
            return View(model);
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);

                var result = await _accountService.RegisterUserAsync(model);

                if (result.Succeeded)
                {
                    // Store email in TempData to prefill login/confirmation pages
                    TempData["Email"] = model.Email;

                    // If email sending failed within the service, signal that to the UI
                    if (result == IdentityResult.Success)
                    {
                        TempData["EmailError"] = "Registration succeeded, but confirmation email could not be sent. Please contact support.";
                    }

                    // Redirect user immediately to the Login page after registration
                    return RedirectToAction("Login");
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for email: {Email}", model.Email);
                ModelState.AddModelError("", "An unexpected error occurred. Please try again later.");
                return View(model);
            }
        }

        // GET: /Account/RegistrationConfirmation
        [HttpGet]
        public IActionResult RegistrationConfirmation()
        {
            return View();
        }

        // GET: /Account/ConfirmEmailCode
        [HttpGet]
        public async Task<IActionResult> ConfirmEmailCode()
        {
            var model = new ConfirmEmailCodeViewModel();
            // If email is passed via TempData, prefill it
            if (TempData.ContainsKey("Email"))
                model.Email = TempData["Email"]?.ToString() ?? string.Empty;

            // Try to get user and set code expiration and resend wait
            if (!string.IsNullOrEmpty(model.Email))
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    if (user.EmailConfirmationCodeExpiresOn.HasValue)
                        TempData["CodeExpiresOn"] = user.EmailConfirmationCodeExpiresOn.Value.ToLocalTime().ToString("HH:mm:ss");
                    if (user.LastConfirmationCodeResendOn.HasValue)
                    {
                        var secondsLeft = 120 - (int)(DateTime.UtcNow - user.LastConfirmationCodeResendOn.Value).TotalSeconds;
                        if (secondsLeft > 0)
                            TempData["ResendWait"] = secondsLeft;
                    }
                }
            }
            return View(model);
        }

        // POST: /Account/ConfirmEmailCode
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmEmailCode(ConfirmEmailCodeViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return View(model);
            }

            // Check if code is expired
            if (user.EmailConfirmationCodeExpiresOn.HasValue &&
                user.EmailConfirmationCodeExpiresOn.Value < DateTime.UtcNow)
            {
                ModelState.AddModelError("", "Confirmation code has expired. Please request a new one.");
                return View(model);
            }

            // Use the AccountService method to confirm email with code
            var result = await _accountService.ConfirmEmailWithCodeAsync(user.Id, model.Code);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Email confirmed successfully!";
                return RedirectToAction("Login");
            }
            else
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
                return View(model);
            }
        }

        // POST: /Account/ResendEmailCode
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendEmailCode(ConfirmEmailCodeViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Email))
            {
                TempData["ResendError"] = "Please enter your email to resend the code.";
                return RedirectToAction("ConfirmEmailCode");
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                TempData["ResendError"] = "User not found.";
                return RedirectToAction("ConfirmEmailCode");
            }

            // Limit resend attempts to once every 2 minutes
            if (user.LastConfirmationCodeResendOn.HasValue && (DateTime.UtcNow - user.LastConfirmationCodeResendOn.Value).TotalSeconds < 120)
            {
                TempData["ResendError"] = "You can request a new code every 2 minutes. Please wait before trying again.";
                return RedirectToAction("ConfirmEmailCode");
            }

            try
            {
                // Use the AccountService method to resend confirmation code
                var success = await _accountService.ResendConfirmationCodeAsync(model.Email);

                if (success)
                {
                    TempData["ResendSuccess"] = "A new confirmation code has been sent to your email.";
                }
                else
                {
                    TempData["ResendError"] = "Failed to send confirmation code. Please try again later.";
                }
            }
            catch
            {
                TempData["ResendError"] = "Failed to send confirmation code. Please try again later.";
            }

            return RedirectToAction("ConfirmEmailCode");
        }

        // GET: /Account/ConfirmEmail
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            try
            {
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                    return BadRequest("Invalid email confirmation request.");

                var result = await _accountService.ConfirmEmailAsync(userId, token);

                if (result.Succeeded)
                    return View("EmailConfirmed");

                // Combine errors into one message or pass errors to the view
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                return View("Error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming email for UserId: {UserId}", userId);
                ModelState.AddModelError("", "An unexpected error occurred during email confirmation.");
                return View("Error");
            }
        }

        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? ReturnUrl = null)
        {
            ViewData["ReturnUrl"] = ReturnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            if (!user.IsActive)
            {
                ModelState.AddModelError(string.Empty, "Your account is inactive. Please contact support.");
                return View(model);
            }

            // Allow Landlords to sign-in immediately even if EmailConfirmed is false.
            // Validate their password, then sign them in manually.
            if (!user.EmailConfirmed && await _userManager.IsInRoleAsync(user, "Landlord"))
            {
                var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
                    if (passwordValid)
                    {
                        await _signInManager.SignInAsync(user, model.RememberMe);
                        return RedirectToAction("Index", "LandlordDashboard");
                    }

                // If password is invalid, show generic invalid attempt message
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                // Priority: Admin > Landlord > Tenant
                if (await _userManager.IsInRoleAsync(user, "Admin"))
                    return RedirectToAction("Index", "Home");
                else if (await _userManager.IsInRoleAsync(user, "Landlord"))
                    return RedirectToAction("Index", "LandlordDashboard");
                else if (await _userManager.IsInRoleAsync(user, "Tenant"))
                    return RedirectToAction("Index", "TenantsDashboard");
            }
            else if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Your account is locked out. Please try again later or contact support.");
                return View(model);
            }
            else if (result.IsNotAllowed)
            {
                if (!user.EmailConfirmed)
                    ModelState.AddModelError(string.Empty, "You must confirm your email before logging in.");
                else
                    ModelState.AddModelError(string.Empty, "You are not allowed to log in. Please contact support.");
                return View(model);
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }

        //HttpGet
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Login", "Account");

            try
            {
                var model = await _accountService.GetUserProfileByEmailAsync(email);
                return View(model);
            }
            catch (ArgumentException)
            {
                return View("Error");
            }
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _accountService.LogoutUserAsync();
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                // Optionally redirect to error page or home with message
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: /Account/ResendEmailConfirmation
        [HttpGet]
        public IActionResult ResendEmailConfirmation()
        {
            return View();
        }

        // POST: /Account/ResendEmailConfirmation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendEmailConfirmation(ResendConfirmationEmailViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);

                await _accountService.SendEmailConfirmationAsync(model.Email);
                TempData["ResendSuccess"] = "A new confirmation code has been sent to your email.";
                return View("ResendEmailConfirmationSuccess");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email confirmation to: {Email}", model.Email);
                ModelState.AddModelError("", "An unexpected error occurred. Please try again later.");
                return View(model);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult AccessDenied(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
    }
}
using TenantsManagementApp.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace TenantsManagementApp.Services
{
    public interface IAccountService
    {
        Task<IdentityResult> RegisterUserAsync(RegisterViewModel model);
        Task<IdentityResult> ConfirmEmailAsync(string userId, string token);
        Task<IdentityResult> ConfirmEmailWithCodeAsync(Guid userId, string code);
        Task<SignInResult> LoginUserAsync(LoginViewModel model);
        Task LogoutUserAsync();
        Task SendEmailConfirmationAsync(string email);
        Task SendEmailConfirmationCodeAsync(string email);
        Task<ProfileViewModel> GetUserProfileByEmailAsync(string email);
        Task<bool> ResendConfirmationCodeAsync(string email);
    }
}
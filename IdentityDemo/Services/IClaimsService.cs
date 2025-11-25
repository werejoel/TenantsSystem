using TenantsManagementApp.ViewModels;
using TenantsManagementApp.ViewModels.Claims;
using TenantsManagementApp.ViewModels.Roles;

namespace TenantsManagementApp.Services
{
    public interface IClaimsService
    {
        Task<PagedResult<ClaimListItemViewModel>> GetPagedClaimsAsync(string? search, string? category, int pageNumber, int pageSize);
        Task<ClaimEditViewModel?> GetClaimByIdAsync(Guid id);
        Task<bool> DeleteClaimAsync(Guid id);
        Task<(bool Success, string Message)> CreateClaimAsync(ClaimEditViewModel model);
        Task<(bool Success, string Message)> UpdateClaimAsync(ClaimEditViewModel model);
    }
}
using TenantsManagementApp.ViewModels.Roles;
using Microsoft.AspNetCore.Identity;
using TenantsManagementApp.ViewModels;

namespace TenantsManagementApp.Services
{
    public interface IRoleService
    {
        Task<PagedResult<RoleListItemViewModel>> GetRolesAsync(RoleListFilterViewModel filter);
        Task<(IdentityResult Result, Guid? RoleId)> CreateAsync(RoleCreateViewModel model);
        Task<RoleEditViewModel?> GetForEditAsync(Guid id);
        Task<IdentityResult> UpdateAsync(RoleEditViewModel model);
        Task<IdentityResult> DeleteAsync(Guid id);
        Task<RoleDetailsViewModel?> GetDetailsAsync(Guid id, int pageNumber, int pageSize);

    //New methods for Role Claims Management
    Task<RoleClaimsEditViewModel?> GetClaimsForEditAsync(Guid roleId);
    Task<IdentityResult> UpdateClaimsAsync(Guid roleId, IEnumerable<Guid> selectedClaimIds);

    // User-role assignment methods
    Task<List<UserInRoleViewModel>> GetAllUsersAsync();
    Task<IdentityResult> UpdateUsersInRoleAsync(Guid roleId, List<UserInRoleAssignmentViewModel> assignments);
    }
}
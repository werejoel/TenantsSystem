using System;
using System.Collections.Generic;

namespace TenantsManagementApp.ViewModels.Roles
{
    public class ManageUsersInRoleViewModel
    {
        public Guid RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public List<UserInRoleAssignmentViewModel> Users { get; set; } = new();
    }

    public class UserInRoleAssignmentViewModel
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public bool IsAssigned { get; set; }
    }
}

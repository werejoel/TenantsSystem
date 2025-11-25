namespace TenantsManagementApp.ViewModels.Roles
{
    public class RoleClaimCheckboxItem
    {
        public Guid ClaimId { get; set; }         // Primary key of ClaimMaster
        public string ClaimType { get; set; } = string.Empty; // e.g., "Permission"
        public string ClaimValue { get; set; } = string.Empty; // e.g., "ViewUsers"
        public string Category { get; set; } = string.Empty;  //Role, Both
        public string? Description { get; set; }               // Optional admin help text
        public bool IsSelected { get; set; }                   // Whether the user currently has it
    }
}
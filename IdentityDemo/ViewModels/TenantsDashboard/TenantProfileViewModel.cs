namespace TenantsManagementApp.ViewModels.TenantsDashboard
{
    public class TenantProfileViewModel
    {
        public string TenantId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UnitNumber { get; set; } = string.Empty;
        public string PropertyName { get; set; } = string.Empty;
        public DateTime LeaseStartDate { get; set; }
        public DateTime LeaseEndDate { get; set; }
    }
}

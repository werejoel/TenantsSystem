namespace TenantsManagementApp.ViewModels.TenantsDashboard
{
    public class EmergencyContactViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Hours { get; set; } = string.Empty;
        public bool IsEmergencyOnly { get; set; }
    }
}

namespace TenantsManagementApp.ViewModels.LandlordDashboard
{
    public class LandlordAlertViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string AlertType { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public bool IsRead { get; set; }
        public string ActionUrl { get; set; } = string.Empty;
    }
}

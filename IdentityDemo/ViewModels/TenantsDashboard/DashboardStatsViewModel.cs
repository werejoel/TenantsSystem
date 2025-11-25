namespace TenantsManagementApp.ViewModels.TenantsDashboard
{
    public class DashboardStatsViewModel
    {
        public decimal CurrentBalance { get; set; }
        public DateTime? NextPaymentDate { get; set; }
        public int ActiveMaintenanceRequests { get; set; }
        public int InProgressRequests { get; set; }
        public int UnreadNotifications { get; set; }
        public int UnreadMessages { get; set; }
        public string LeaseStatus { get; set; } = string.Empty;
        public DateTime LeaseEndDate { get; set; }
    }
}

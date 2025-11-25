namespace TenantsManagementApp.ViewModels.LandlordDashboard
{
    public class TenantOverviewViewModel
    {
    public int Id { get; set; }
        public string TenantName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string UnitNumber { get; set; } = string.Empty;
        public string PropertyName { get; set; } = string.Empty;
        public DateTime LeaseStartDate { get; set; }
        public DateTime LeaseEndDate { get; set; }
        public decimal MonthlyRent { get; set; }
        public decimal CurrentBalance { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public bool IsLeaseActive { get; set; }
        public int DaysUntilLeaseExpiry { get; set; }
    }
}

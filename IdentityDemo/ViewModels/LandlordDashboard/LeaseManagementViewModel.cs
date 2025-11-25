namespace TenantsManagementApp.ViewModels.LandlordDashboard
{
    public class LeaseManagementViewModel
    {
    public int Id { get; set; }
        public string TenantName { get; set; } = string.Empty;
        public string UnitNumber { get; set; } = string.Empty;
        public string PropertyName { get; set; } = string.Empty;
        public DateTime LeaseStartDate { get; set; }
        public DateTime LeaseEndDate { get; set; }
        public decimal MonthlyRent { get; set; }
        public decimal SecurityDeposit { get; set; }
        public string LeaseStatus { get; set; } = string.Empty;
        public int DaysRemaining { get; set; }
        public bool AutoRenew { get; set; }
        public string LeaseType { get; set; } = string.Empty;
    }
}

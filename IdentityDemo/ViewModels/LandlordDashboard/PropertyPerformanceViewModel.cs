namespace TenantsManagementApp.ViewModels.LandlordDashboard
{
    public class PropertyPerformanceViewModel
    {
    public int PropertyId { get; set; }
        public string PropertyName { get; set; } = string.Empty;
        public double OccupancyRate { get; set; }
        public decimal Revenue { get; set; }
        public decimal Expenses { get; set; }
        public decimal NetOperatingIncome { get; set; }
        public double ROI { get; set; }
        public int MaintenanceRequests { get; set; }
        public double TenantSatisfactionScore { get; set; }
        public int AverageTenancyDuration { get; set; }
    }
}

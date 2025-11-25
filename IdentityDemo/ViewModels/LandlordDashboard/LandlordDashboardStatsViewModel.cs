namespace TenantsManagementApp.ViewModels.LandlordDashboard
{
    public class LandlordDashboardStatsViewModel
    {
        public decimal TotalRevenue { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public decimal OutstandingBalance { get; set; }
        public decimal ExpectedRevenue { get; set; }
        public int TotalProperties { get; set; }
        public int TotalUnits { get; set; }
        public int OccupiedUnits { get; set; }
        public int VacantUnits { get; set; }
        public double OccupancyRate { get; set; }
        public int ActiveTenants { get; set; }
        public int PendingMaintenanceRequests { get; set; }
        public int MaintenanceInProgress { get; set; }
        public int LeaseExpiringThisMonth { get; set; }
        public int OverduePayments { get; set; }
        public decimal MaintenanceCosts { get; set; }
    }
}

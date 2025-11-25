namespace TenantsManagementApp.ViewModels.LandlordDashboard
{
    public class PropertyOverviewViewModel
    {
    public int Id { get; set; }
        public string PropertyName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int TotalUnits { get; set; }
        public int OccupiedUnits { get; set; }
        public int VacantUnits { get; set; }
        public double OccupancyRate { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public decimal OutstandingBalance { get; set; }
        public int PendingMaintenance { get; set; }
        public string PropertyType { get; set; } = string.Empty;
    }
}

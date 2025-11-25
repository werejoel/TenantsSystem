namespace TenantsManagementApp.ViewModels.LandlordDashboard
{
    public class RevenueSummaryViewModel
    {
        public decimal TotalRevenue { get; set; }
        public decimal CollectedRevenue { get; set; }
        public decimal PendingRevenue { get; set; }
        public decimal OverdueRevenue { get; set; }
        public List<MonthlyRevenueViewModel> MonthlyBreakdown { get; set; } = new List<MonthlyRevenueViewModel>();
        public List<PropertyRevenueViewModel> PropertyBreakdown { get; set; } = new List<PropertyRevenueViewModel>();
    }
}

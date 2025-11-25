namespace TenantsManagementApp.ViewModels.LandlordDashboard
{
    public class ExpenseBreakdownViewModel
    {
        public string Category { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public double Percentage { get; set; }
    }
}

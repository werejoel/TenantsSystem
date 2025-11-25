namespace TenantsManagementApp.ViewModels.LandlordDashboard
{
    public class MonthlyRevenueViewModel
    {
        public string Month { get; set; } = string.Empty;
        public int Year { get; set; }
        public decimal Revenue { get; set; }
        public decimal Expenses { get; set; }
        public decimal NetIncome { get; set; }
    }
}

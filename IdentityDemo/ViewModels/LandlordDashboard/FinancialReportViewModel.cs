namespace TenantsManagementApp.ViewModels.LandlordDashboard
{
    public class FinancialReportViewModel
    {
        public DateTime ReportDate { get; set; }
        public string ReportPeriod { get; set; } = string.Empty;
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetIncome { get; set; }
        public decimal ProfitMargin { get; set; }
        public List<IncomeBreakdownViewModel> IncomeBreakdown { get; set; } = new List<IncomeBreakdownViewModel>();
        public List<ExpenseBreakdownViewModel> ExpenseBreakdown { get; set; } = new List<ExpenseBreakdownViewModel>();
    }
}

namespace TenantsManagementApp.ViewModels.LandlordDashboard
{
    public class PropertyRevenueViewModel
    {
    public int PropertyId { get; set; }
        public string PropertyName { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public decimal Expenses { get; set; }
        public decimal NetIncome { get; set; }
    }
}

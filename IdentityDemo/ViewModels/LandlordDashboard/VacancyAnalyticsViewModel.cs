namespace TenantsManagementApp.ViewModels.LandlordDashboard
{
    public class VacancyAnalyticsViewModel
    {
    public int PropertyId { get; set; }
        public string PropertyName { get; set; } = string.Empty;
        public string UnitNumber { get; set; } = string.Empty;
        public DateTime? VacantSince { get; set; }
        public int DaysVacant { get; set; }
        public decimal MonthlyRent { get; set; }
        public decimal LostRevenue { get; set; }
        public string Reason { get; set; } = string.Empty;
        public bool IsListedForRent { get; set; }
    }
}

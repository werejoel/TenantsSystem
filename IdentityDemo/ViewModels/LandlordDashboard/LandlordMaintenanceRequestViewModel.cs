namespace TenantsManagementApp.ViewModels.LandlordDashboard
{
    public class LandlordMaintenanceRequestViewModel
    {
    public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string TenantName { get; set; } = string.Empty;
        public string UnitNumber { get; set; } = string.Empty;
        public string PropertyName { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime SubmittedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public decimal? EstimatedCost { get; set; }
        public decimal? ActualCost { get; set; }
        public string AssignedTo { get; set; } = string.Empty;
    }
}

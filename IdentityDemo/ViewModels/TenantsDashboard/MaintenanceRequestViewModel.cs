namespace TenantsManagementApp.ViewModels.TenantsDashboard
{
    public class MaintenanceRequestViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime SubmittedDate { get; set; }
        public DateTime? ScheduledDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string AssignedTechnician { get; set; } = string.Empty;
        public string TechnicianPhone { get; set; } = string.Empty;
        public List<string> Photos { get; set; } = new List<string>();
        public string TenantNotes { get; set; } = string.Empty;
        public string TechnicianNotes { get; set; } = string.Empty;
    }
}

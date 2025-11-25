namespace TenantsManagementApp.ViewModels.TenantsDashboard
{
    public class EventViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public bool RequiresRSVP { get; set; }
        public int? MaxAttendees { get; set; }
        public int CurrentAttendees { get; set; }
    }
}

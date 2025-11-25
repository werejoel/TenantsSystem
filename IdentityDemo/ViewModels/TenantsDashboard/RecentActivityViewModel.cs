namespace TenantsManagementApp.ViewModels.TenantsDashboard
{
    public class RecentActivityViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string ActivityType { get; set; } = string.Empty;
        public string RelatedEntityId { get; set; } = string.Empty;
    }
}

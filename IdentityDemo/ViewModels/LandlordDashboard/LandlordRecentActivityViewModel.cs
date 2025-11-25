namespace TenantsManagementApp.ViewModels.LandlordDashboard
{
    public class LandlordRecentActivityViewModel
    {
    public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime ActivityDate { get; set; }
        public string ActivityType { get; set; } = string.Empty;
        public string RelatedEntityId { get; set; } = string.Empty;
        public string PropertyName { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
    }


}

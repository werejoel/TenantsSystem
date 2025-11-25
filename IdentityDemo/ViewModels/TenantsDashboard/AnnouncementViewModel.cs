namespace TenantsManagementApp.ViewModels.TenantsDashboard
{
    public class AnnouncementViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Priority { get; set; }
        public DateTime PostedDate { get; set; } = DateTime.MinValue;
        public DateTime? ExpiryDate { get; set; }
        public List<AttachmentViewModel> Attachments { get; set; } = new List<AttachmentViewModel>();
    }
}

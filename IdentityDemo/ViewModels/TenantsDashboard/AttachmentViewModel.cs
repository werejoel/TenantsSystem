namespace TenantsManagementApp.ViewModels.TenantsDashboard
{
    public class AttachmentViewModel
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public long FileSize { get; set; }
    }
}

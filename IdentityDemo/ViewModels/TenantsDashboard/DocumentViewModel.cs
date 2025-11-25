namespace TenantsManagementApp.ViewModels.TenantsDashboard
{
    public class DocumentViewModel
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string Category { get; set; } = string.Empty;
        public DateTime UploadDate { get; set; } = DateTime.MinValue;
        public string Description { get; set; } = string.Empty;
    }
}

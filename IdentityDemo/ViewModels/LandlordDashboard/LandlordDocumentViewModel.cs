namespace TenantsManagementApp.ViewModels.LandlordDashboard
{
    public class LandlordDocumentViewModel
    {
    public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime UploadDate { get; set; }
        public string RelatedTo { get; set; } = string.Empty;
        public string RelatedEntityId { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}

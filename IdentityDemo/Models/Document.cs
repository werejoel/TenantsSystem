using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TenantsManagementApp.Models
{
    public class Document : BaseEntity
    {
        [ForeignKey("Tenant")]
        public int? TenantId { get; set; }

        [ForeignKey("House")]
        public int? HouseId { get; set; }

        [Required]
        [StringLength(100)]
        public string DocumentType { get; set; } = null!;

        [Required]
        [StringLength(255)]
        public string FileName { get; set; } = null!;

        [Required]
        [StringLength(500)]
        public string FilePath { get; set; } = null!;

        [StringLength(50)]
        public string? FileSize { get; set; }

        [StringLength(100)]
        public string? MimeType { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual Tenant? Tenant { get; set; }
        public virtual House? House { get; set; }
    }
}

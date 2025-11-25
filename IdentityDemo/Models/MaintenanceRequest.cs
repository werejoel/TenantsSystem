using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TenantsManagementApp.Models
{
    public class MaintenanceRequest : BaseEntity
    {
        [ForeignKey("Tenant")]
        public int TenantId { get; set; }

        [ForeignKey("House")]
        public int HouseId { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; } = null!;

        [Required]
        [StringLength(2000)]
        public string Description { get; set; } = null!;

        [StringLength(50)]
        public string Priority { get; set; } = "Medium";

        [StringLength(50)]
        public string Status { get; set; } = "Pending";

        [StringLength(2000)]
        public string? ManagerNotes { get; set; }

        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

        public DateTime? CompletedAt { get; set; }

        // Navigation Properties
        public virtual Tenant Tenant { get; set; } = null!;
        public virtual House House { get; set; } = null!;

        // Computed Properties
        [NotMapped]
        public int DaysOpen => (DateTime.UtcNow - RequestedAt).Days;

        [NotMapped]
        public bool IsCompleted => Status == "Completed" && CompletedAt.HasValue;
    }
}

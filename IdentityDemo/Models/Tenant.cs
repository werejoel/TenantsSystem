using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace TenantsManagementApp.Models
{
    public class Tenant : BaseEntity
    {
        [ForeignKey("User")]
        public Guid UserId { get; set; }

        [ForeignKey("House")]
        public int? HouseId { get; set; } // Nullable to allow tenants without assigned houses

        public DateTime? LeaseStartDate { get; set; }

        public DateTime? LeaseEndDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SecurityDeposit { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation Properties
    public virtual ApplicationUser? User { get; set; }
        public virtual House? House { get; set; }
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public virtual ICollection<Charge> Charges { get; set; } = new List<Charge>();
        public virtual ICollection<MaintenanceRequest> MaintenanceRequests { get; set; } = new List<MaintenanceRequest>();
        public virtual ICollection<Document> Documents { get; set; } = new List<Document>();

        // Computed Properties
        [NotMapped]
        public string FullName => User?.FullName ?? "";

        [NotMapped]
        public bool IsLeaseActive => LeaseStartDate.HasValue && LeaseEndDate.HasValue &&
                                   DateTime.Now >= LeaseStartDate && DateTime.Now <= LeaseEndDate;
    }
}

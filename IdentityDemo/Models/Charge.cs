using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TenantsManagementApp.Models
{
    public class Charge : BaseEntity
    {
        [ForeignKey("Tenant")]
        public int TenantId { get; set; }

        [ForeignKey("House")]
        public int HouseId { get; set; }

        [Required]
        [StringLength(100)]
        public string ChargeType { get; set; } = null!;

        [StringLength(500)]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public DateTime DueDate { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Pending";

        // Navigation Properties
        public virtual Tenant Tenant { get; set; } = null!;
        public virtual House House { get; set; } = null!;
        public virtual ICollection<PaymentCharge> PaymentCharges { get; set; } = new List<PaymentCharge>();

        // Computed Properties
        [NotMapped]
        public decimal AmountPaid => PaymentCharges?.Sum(pc => pc.AmountPaid) ?? 0;

        [NotMapped]
        public decimal OutstandingAmount => Amount - AmountPaid;

        [NotMapped]
        public bool IsOverdue => Status != "Paid" && DueDate < DateTime.Now;
    }
}

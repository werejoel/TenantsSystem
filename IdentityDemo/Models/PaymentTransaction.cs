using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TenantsManagementApp.Models
{
    public class PaymentTransaction : BaseEntity
    {
        [StringLength(255)]
        public string? TxRef { get; set; }

        [StringLength(255)]
        public string? FlwRef { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [StringLength(10)]
        public string? Currency { get; set; }

        [StringLength(50)]
        public string? Status { get; set; }

        [StringLength(50)]
        public string? Provider { get; set; }

        // Link back to tenant/payment if known
        [ForeignKey("Tenant")]
        public int? TenantId { get; set; }
        public virtual Tenant? Tenant { get; set; }

        [ForeignKey("Payment")]
        public int? PaymentId { get; set; }
        public virtual Payment? Payment { get; set; }

        // Customer info
        [StringLength(200)]
        public string? CustomerName { get; set; }
        [StringLength(200)]
        public string? CustomerEmail { get; set; }
        [StringLength(50)]
        public string? CustomerPhone { get; set; }

        // Raw JSON payload from provider for auditing
        public string? RawResponse { get; set; }

        public DateTime? PaymentDate { get; set; }

        public bool Verified { get; set; } = false;
    }
}

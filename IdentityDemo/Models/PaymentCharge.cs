using System.ComponentModel.DataAnnotations.Schema;

namespace TenantsManagementApp.Models
{
    public class PaymentCharge : BaseEntity
    {
        [ForeignKey("Payment")]
        public int PaymentId { get; set; }

        [ForeignKey("Charge")]
        public int ChargeId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal AmountPaid { get; set; }

        // Navigation Properties
        public virtual Payment Payment { get; set; } = null!;
        public virtual Charge Charge { get; set; } = null!;
    }
}

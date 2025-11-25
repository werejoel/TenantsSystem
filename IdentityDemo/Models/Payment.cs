using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TenantsManagementApp.Models
{
    public class Payment : BaseEntity
    {
        [Required(ErrorMessage = "Please select a tenant.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid tenant.")]
        [ForeignKey("Tenant")]
        public int TenantId { get; set; }

        [Required(ErrorMessage = "Please select a house.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid house.")]
        [ForeignKey("House")]
        public int HouseId { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Amount Paid")]
        public decimal AmountPaid { get; set; }

        [Required(ErrorMessage = "Payment date is required.")]
        [Display(Name = "Payment Date")]
        public DateTime PaymentDate { get; set; }

        [Display(Name = "Period Start")]
        public DateTime? PeriodStart { get; set; }

        [Display(Name = "Period End")]
        public DateTime? PeriodEnd { get; set; }

        [Required(ErrorMessage = "Payment method is required.")]
        [StringLength(100, ErrorMessage = "Payment method cannot exceed 100 characters.")]
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; } = null!;

        [StringLength(255, ErrorMessage = "Transaction reference cannot exceed 255 characters.")]
        [Display(Name = "Transaction Reference")]
        public string? TransactionReference { get; set; }

        [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters.")]
        public string? Notes { get; set; }

        // Navigation Properties
        public virtual Tenant? Tenant { get; set; }
        public virtual House? House { get; set; }
        public virtual ICollection<PaymentCharge> PaymentCharges { get; set; } = new List<PaymentCharge>();
    }
}
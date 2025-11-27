using System.ComponentModel.DataAnnotations;

namespace TenantsManagementApp.Models.FlutterWave
{
    public class InitiatePaymentRequest
    {
        [Required] 
        public int TenantId { get; set; }
        [Required]
        public int HouseId { get; set; }
        [Required] 
        public decimal AmountPaid { get; set; }
        [Required]
        public string Provider { get; set; }
        [Required] 
        public string Purpose { get; set; }
        [Required]
        public DateTime PaymentDate { get; set; }
        public DateTime? PeriodStart { get; set; }
        public DateTime? PeriodEnd { get; set; }
        public string? Notes { get; set; }
        public string? RedirectUrl { get; set; }
        [Required] 
        public List<int> ChargeIds { get; set; } // Charges to be paid
    }
}

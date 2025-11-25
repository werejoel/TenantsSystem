using TenantsManagementApp.Models;

namespace TenantsManagementApp.DTOS
{
    public class PaymentSummaryDto
    {
        public int TotalPayments { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<Payment> Payments { get; set; } = new List<Payment>();
    }
}

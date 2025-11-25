using TenantsManagementApp.Models;

namespace TenantsManagementApp.DTOS
{
    public class TenantBalanceDto
    {
        public int TenantId { get; set; }
        public string TenantName { get; set; } = null!;
        public decimal TotalCharges { get; set; }
        public decimal TotalPayments { get; set; }
        public decimal OutstandingBalance { get; set; }
        public decimal OverpaymentAmount { get; set; }
        public DateTime LastPaymentDate { get; set; }
        public List<Charge> OverdueCharges { get; set; } = new List<Charge>();
    }
}

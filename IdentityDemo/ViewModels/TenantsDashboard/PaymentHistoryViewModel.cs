namespace TenantsManagementApp.ViewModels.TenantsDashboard
{
    public class PaymentHistoryViewModel
    {
        public int Id { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
    }
}

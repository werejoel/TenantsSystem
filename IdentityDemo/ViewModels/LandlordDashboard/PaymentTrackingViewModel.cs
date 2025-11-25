namespace TenantsManagementApp.ViewModels.LandlordDashboard
{
    public class PaymentTrackingViewModel
    {
    public int Id { get; set; }
        public string TenantName { get; set; } = string.Empty;
        public string UnitNumber { get; set; } = string.Empty;
        public string PropertyName { get; set; } = string.Empty;
        public DateTime PaymentDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public int DaysOverdue { get; set; }
        public string TransactionId { get; set; } = string.Empty;
    }
}

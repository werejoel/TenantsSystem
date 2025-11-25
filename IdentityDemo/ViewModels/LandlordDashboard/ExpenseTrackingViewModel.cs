namespace TenantsManagementApp.ViewModels.LandlordDashboard
{
    public class ExpenseTrackingViewModel
    {
        public Guid Id { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string PropertyName { get; set; } = string.Empty;
        public string Vendor { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public bool IsRecurring { get; set; }
        public string ReceiptUrl { get; set; } = string.Empty;
    }

}

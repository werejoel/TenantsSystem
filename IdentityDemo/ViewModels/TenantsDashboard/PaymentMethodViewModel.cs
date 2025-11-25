namespace TenantsManagementApp.ViewModels.TenantsDashboard
{
    public class PaymentMethodViewModel
    {
        public int Id { get; set; }
        public string CardType { get; set; } = string.Empty;
        public string MaskedCardNumber { get; set; } = string.Empty;
        public string ExpiryDate { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
        public bool IsAutoPayEnabled { get; set; }
    }
}

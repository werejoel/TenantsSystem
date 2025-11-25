namespace TenantsManagementApp.Models.FlutterWave
{
    public class WebhookTransaction
    {
        public long Id { get; set; }
        public string TxRef { get; set; }
        public string FlwRef { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; }
        public Customer Customer { get; set; }
    }
}

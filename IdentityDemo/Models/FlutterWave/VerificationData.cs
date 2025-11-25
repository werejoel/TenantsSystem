namespace TenantsManagementApp.Models.FlutterWave
{
    public class VerificationData
    {
        public long Id { get; set; }
        public string TxRef { get; set; }
        public string FlwRef { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; }
        public decimal ChargedAmount { get; set; }
    }
}

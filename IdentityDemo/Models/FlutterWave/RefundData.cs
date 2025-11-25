namespace TenantsManagementApp.Models.FlutterWave
{
    public class RefundData
    {
        public long Id { get; set; }
        public string TxRef { get; set; }
        public string FlwRef { get; set; }
        public decimal AmountRefunded { get; set; }
        public string Status { get; set; }
    }
}

namespace TenantsManagementApp.Models.FlutterWave
{
    public class ChargeResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public ChargeData Data { get; set; }
        public Meta Meta { get; set; }
    }
}

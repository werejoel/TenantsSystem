namespace TenantsManagementApp.ViewModels.LandlordDashboard
{
    public class LandlordProfileViewModel
    {
        public string LandlordId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int TotalProperties { get; set; }
        public int TotalUnits { get; set; }
        public int OccupiedUnits { get; set; }
        public int VacantUnits { get; set; }
        public DateTime MemberSince { get; set; }
    }
}

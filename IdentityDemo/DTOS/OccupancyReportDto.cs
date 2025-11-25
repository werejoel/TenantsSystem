using TenantsManagementApp.Models;

namespace TenantsManagementApp.DTOS
{
    public class OccupancyReportDto
    {
        public int TotalUnits { get; set; }
        public int OccupiedUnits { get; set; }
        public int VacantUnits { get; set; }
        public int UnderMaintenanceUnits { get; set; }
        public decimal OccupancyRate { get; set; }
        public List<House> VacantHouses { get; set; } = new List<House>();
    }
}

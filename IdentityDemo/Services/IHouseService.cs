using TenantsManagementApp.DTOS;
using TenantsManagementApp.Models;

namespace TenantsManagementApp.Services
{
    public interface IHouseService
    {
        Task<List<House>> GetAllHousesAsync();
        Task<House?> GetHouseByIdAsync(int id);
        Task<House> CreateHouseAsync(House house);
        Task<House?> UpdateHouseAsync(House house);
        Task<bool> DeleteHouseAsync(int id);
        Task<List<House>> GetVacantHousesAsync();
        Task<OccupancyReportDto> GetOccupancyReportAsync();
    }
}

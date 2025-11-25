using Microsoft.EntityFrameworkCore;
using TenantsManagementApp.Data;
using TenantsManagementApp.DTOS;
using TenantsManagementApp.Models;

namespace TenantsManagementApp.Services
{
    public class HouseService : IHouseService
    {
        private readonly ApplicationDbContext _context;

        public HouseService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<House>> GetAllHousesAsync()
        {
            return await _context.Houses
                .Include(h => h.Landlord)
                .Where(h => h.IsActive)
                .OrderBy(h => h.Name)
                .ToListAsync();
        }

        public async Task<House?> GetHouseByIdAsync(int id)
        {
            return await _context.Houses
                .Include(h => h.Landlord)
                .Include(h => h.Tenants)
                .ThenInclude(t => t.User)
                .FirstOrDefaultAsync(h => h.Id == id && h.IsActive);
        }

        public async Task<House> CreateHouseAsync(House house)
        {
            _context.Houses.Add(house);
            await _context.SaveChangesAsync();
            return house;
        }

        public async Task<House?> UpdateHouseAsync(House house)
        {
            var existingHouse = await _context.Houses.FindAsync(house.Id);
            if (existingHouse == null) return null;

            existingHouse.Name = house.Name;
            existingHouse.Location = house.Location;
            existingHouse.Model = house.Model;
            existingHouse.Price = house.Price;
            existingHouse.Status = house.Status;
            existingHouse.LandlordId = house.LandlordId;

            await _context.SaveChangesAsync();
            return existingHouse;
        }

        public async Task<bool> DeleteHouseAsync(int id)
        {
            var house = await _context.Houses.FindAsync(id);
            if (house == null) return false;

            house.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<House>> GetVacantHousesAsync()
        {
            return await _context.Houses
                .Include(h => h.Landlord)
                .Where(h => h.IsActive && h.Status == "Vacant")
                .OrderBy(h => h.Name)
                .ToListAsync();
        }

        public async Task<OccupancyReportDto> GetOccupancyReportAsync()
        {
            // Get all active houses (total units)
            var totalUnits = await _context.Houses.CountAsync(h => h.IsActive);

            // Get occupied houses (houses with active tenants or status "Occupied")
            var occupiedUnits = await _context.Houses
                .Where(h => h.IsActive && (h.Status == "Occupied" || h.Tenants.Any(t => t.IsActive)))
                .CountAsync();

            // Get vacant houses with their details
            var vacantHouses = await _context.Houses
                .Include(h => h.Landlord)
                .Where(h => h.IsActive && h.Status == "Vacant")
                .OrderBy(h => h.Name)
                .ToListAsync();

            var vacantUnits = vacantHouses.Count;

            // Get houses under maintenance
            var underMaintenanceUnits = await _context.Houses
                .CountAsync(h => h.IsActive && h.Status == "Under Maintenance");

            // Calculate occupancy rate
            var occupancyRate = totalUnits > 0 ? (decimal)occupiedUnits / totalUnits * 100 : 0;

            return new OccupancyReportDto
            {
                TotalUnits = totalUnits,
                OccupiedUnits = occupiedUnits,
                VacantUnits = vacantUnits,
                UnderMaintenanceUnits = underMaintenanceUnits,
                OccupancyRate = Math.Round(occupancyRate, 2),
                VacantHouses = vacantHouses
            };
        }
    }
}
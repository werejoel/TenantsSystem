using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using TenantsManagementApp.Data;
using TenantsManagementApp.Models;

namespace TenantsManagementApp.Controllers
{
    public class HousesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HousesController> _logger;

        public HousesController(ApplicationDbContext context, ILogger<HousesController> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET: Houses
        public async Task<IActionResult> Index()
        {
            try
            {
                var houses = await _context.Houses
                    .Include(h => h.Landlord)
                    .ToListAsync();
                return View(houses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving houses list.");
                ModelState.AddModelError(string.Empty, "An error occurred while retrieving the houses list.");
                return View();
            }
        }

        // GET: Houses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Details action called with null id.");
                return NotFound();
            }

            try
            {
                var house = await _context.Houses
                    .Include(h => h.Landlord)
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (house == null)
                {
                    _logger.LogWarning($"House with id {id} not found.");
                    return NotFound();
                }
                return View(house);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving details for house id {id}.");
                ModelState.AddModelError(string.Empty, "An error occurred while retrieving house details.");
                return View();
            }
        }

        // GET: Houses/Create
        public IActionResult Create()
        {
            try
            {
                // Load active landlords
                var landlords = _context.Landlords.Where(l => l.IsActive).ToList();
                var house = new House();
                if (!landlords.Any())
                {
                    _logger.LogWarning("No active landlords found in the database.");
                    ModelState.AddModelError(string.Empty, "No active landlords are available. Please create a landlord first.");
                    ViewBag.LandlordId = new SelectList(Enumerable.Empty<object>(), "Id", "FullName");
                    return View(house);
                }

                // Log available landlords
                _logger.LogInformation("Available landlords: {Landlords}", string.Join("; ", landlords.Select(l => $"Id={l.Id}, FullName={l.FullName}, IsActive={l.IsActive}")));
                ViewBag.LandlordId = new SelectList(landlords, "Id", "FullName");
                _logger.LogInformation($"Loaded {landlords.Count} active landlords for dropdown.");
                return View(house);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading Create view.");
                ModelState.AddModelError(string.Empty, "Unable to load the create page. Please try again.");
                ViewData["LandlordId"] = new SelectList(Enumerable.Empty<object>(), "Id", "FullName");
                return View(new House());
            }
        }

        // POST: Houses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LandlordId,Name,Location,Model,Price,Status,IsActive")] House house)
        {
            try
            {
                // Log all form data
                var formData = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());
                _logger.LogInformation("Submitted form data: {FormData}", string.Join("; ", formData.Select(kvp => $"{kvp.Key}={kvp.Value}")));

                // Explicitly check LandlordId
                if (house.LandlordId <= 0 || !_context.Landlords.Any(l => l.Id == house.LandlordId && l.IsActive))
                {
                    ModelState.AddModelError("LandlordId", "Please select a valid, active landlord.");
                    _logger.LogWarning($"Invalid LandlordId: {house.LandlordId}");
                    Console.Error.WriteLine("Invalid LandlordId: " + house.LandlordId);
                }

                if (ModelState.IsValid)
                {
                    _context.Add(house);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"House '{house.Name}' created successfully with id {house.Id}, LandlordId {house.LandlordId}.");
                    Console.WriteLine($"House '{house.Name}' created successfully with id {house.Id}, LandlordId {house.LandlordId}.");
                    return RedirectToAction(nameof(Index));
                }

                // Log validation errors
                var validationErrors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                _logger.LogWarning("Model validation failed for House creation: {Errors}", string.Join("; ", validationErrors));
                foreach (var error in validationErrors)
                {
                    ModelState.AddModelError(string.Empty, error);
                    Console.Error.WriteLine("Validation error: " + error);
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while creating house with LandlordId {LandlordId}.", house.LandlordId);
                ModelState.AddModelError(string.Empty, "Failed to save the house to the database. The selected landlord may not exist or is inactive.");
                Console.Error.WriteLine("Database error: " + ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating house.");
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again later.");
                Console.Error.WriteLine("Unexpected error: " + ex.Message);
            }

            // Reload the landlord dropdown
            var landlords = _context.Landlords.Where(l => l.IsActive).ToList();
            ViewBag.LandlordId = new SelectList(landlords, "Id", "FullName", house.LandlordId);
            return View(house);
        }

        // GET: Houses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Edit action called with null id.");
                return NotFound();
            }

            try
            {
                var house = await _context.Houses.FindAsync(id);
                if (house == null)
                {
                    _logger.LogWarning($"House with id {id} not found.");
                    return NotFound();
                }
                var landlords = _context.Landlords.Where(l => l.IsActive).ToList();
                ViewData["LandlordId"] = new SelectList(landlords, "Id", "FullName", house.LandlordId);
                return View(house);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading Edit view for house id {id}.");
                ModelState.AddModelError(string.Empty, "Unable to load the edit page. Please try again.");
                return View();
            }
        }

        // POST: Houses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LandlordId,Name,Location,Model,Price,Status,IsActive,Id,CreatedAt")] House house)
        {
            if (id != house.Id)
            {
                _logger.LogWarning($"Mismatched house id in Edit action: {id} != {house.Id}.");
                return NotFound();
            }

            try
            {
                // Log all form data
                var formData = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());
                _logger.LogInformation("Submitted form data for Edit: {FormData}", string.Join("; ", formData.Select(kvp => $"{kvp.Key}={kvp.Value}")));

                // Explicitly check LandlordId
                if (house.LandlordId <= 0 || !_context.Landlords.Any(l => l.Id == house.LandlordId && l.IsActive))
                {
                    ModelState.AddModelError("LandlordId", "Please select a valid, active landlord.");
                    _logger.LogWarning($"Invalid LandlordId in Edit: {house.LandlordId}");
                }

                if (ModelState.IsValid)
                {
                    _context.Update(house);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"House id {id} updated successfully.");
                    return RedirectToAction(nameof(Index));
                }

                // Log validation errors
                var validationErrors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                _logger.LogWarning("Model validation failed for House edit: {Errors}", string.Join("; ", validationErrors));
                foreach (var error in validationErrors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!HouseExists(house.Id))
                {
                    _logger.LogWarning($"House with id {id} not found during edit.");
                    return NotFound();
                }
                _logger.LogError(ex, $"Concurrency error updating house id {id}.");
                ModelState.AddModelError(string.Empty, "The house was modified by another user. Please refresh and try again.");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Database error updating house id {id}.");
                ModelState.AddModelError(string.Empty, "Failed to update the house in the database. The selected landlord may not exist or is inactive.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error updating house id {id}.");
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again later.");
            }

            var landlords = _context.Landlords.Where(l => l.IsActive).ToList();
            ViewData["LandlordId"] = new SelectList(landlords, "Id", "FullName", house.LandlordId);
            return View(house);
        }

        // GET: Houses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Delete action called with null id.");
                return NotFound();
            }

            try
            {
                var house = await _context.Houses
                    .Include(h => h.Landlord)
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (house == null)
                {
                    _logger.LogWarning($"House with id {id} not found.");
                    return NotFound();
                }
                return View(house);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading Delete view for house id {id}.");
                ModelState.AddModelError(string.Empty, "Unable to load the delete page. Please try again.");
                return View();
            }
        }

        // POST: Houses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var house = await _context.Houses.FindAsync(id);
                if (house != null)
                {
                    _context.Houses.Remove(house);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"House id {id} deleted successfully.");
                }
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Database error deleting house id {id}.");
                ModelState.AddModelError(string.Empty, "Failed to delete the house from the database. It may be referenced by other records.");
                var house = await _context.Houses.Include(h => h.Landlord).FirstOrDefaultAsync(m => m.Id == id);
                return View(house ?? new House());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error deleting house id {id}.");
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again later.");
                var house = await _context.Houses.Include(h => h.Landlord).FirstOrDefaultAsync(m => m.Id == id);
                return View(house ?? new House());
            }
        }

        private bool HouseExists(int id)
        {
            return _context.Houses.Any(e => e.Id == id);
        }
    }
}
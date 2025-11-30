using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TenantsManagementApp.Data;
using TenantsManagementApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace TenantsManagementApp.Controllers
{
    [Authorize(Roles = "Landlord,Tenant,Admin")]
    public class MaintenanceRequestsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MaintenanceRequestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // JSON: GET /Maintenance/GetRequests
        [HttpGet]
        [Route("/Maintenance/GetRequests")]
        public async Task<IActionResult> GetRequests()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var tenant = await _context.Tenants.FirstOrDefaultAsync(t => t.UserId == userId && t.IsActive);
            if (tenant == null)
                return Json(new List<object>());

            var requests = await _context.MaintenanceRequests
                .Where(r => r.TenantId == tenant.Id)
                .OrderByDescending(r => r.RequestedAt)
                .Select(r => new {
                    id = r.Id,
                    title = r.Title,
                    description = r.Description,
                    category = r.Priority, // model uses Priority; no Category property
                    priority = r.Priority,
                    status = r.Status,
                    submittedDate = r.RequestedAt,
                    scheduledDate = r.CompletedAt, // no ScheduledAt property; reuse CompletedAt as nullable
                    completedDate = r.CompletedAt,
                    assignedTechnician = r.ManagerNotes != null ? "Assigned" : string.Empty,
                    technicianPhone = string.Empty
                }).ToListAsync();

            return Json(requests);
        }

        // JSON: POST /Maintenance/SubmitRequest
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/Maintenance/SubmitRequest")]
        public async Task<IActionResult> SubmitRequest([FromBody] MaintenanceRequest input)
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!Guid.TryParse(userIdClaim, out var userId))
                    return Unauthorized();

                var tenant = await _context.Tenants.FirstOrDefaultAsync(t => t.UserId == userId && t.IsActive);
                if (tenant == null)
                    return BadRequest(new { success = false, message = "Tenant not found" });

                if (tenant.HouseId == null)
                {
                    return BadRequest(new { success = false, message = "Tenant does not have a HouseId assigned." });
                }

                var request = new MaintenanceRequest
                {
                    TenantId = tenant.Id,
                    HouseId = (int)tenant.HouseId,
                    Title = input.Title,
                    Description = input.Description,
                    // Category = input.Category, // Removed because 'Category' does not exist in the model
                    Priority = input.Priority,
                    Status = "Pending",
                    RequestedAt = DateTime.UtcNow
                };

                _context.MaintenanceRequests.Add(request);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Request submitted", id = request.Id });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: MaintenanceRequests
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.MaintenanceRequests.Include(m => m.House).Include(m => m.Tenant);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: MaintenanceRequests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var maintenanceRequest = await _context.MaintenanceRequests
                .Include(m => m.House)
                .Include(m => m.Tenant)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (maintenanceRequest == null)
            {
                return NotFound();
            }

            return View(maintenanceRequest);
        }

        // GET: MaintenanceRequests/Create
        public IActionResult Create()
        {
            ViewData["HouseId"] = new SelectList(_context.Houses, "Id", "Name");
            ViewData["TenantId"] = new SelectList(_context.Tenants, "Id", "Id");
            return View();
        }

        // POST: MaintenanceRequests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenantId,HouseId,Title,Description,Priority,Status,ManagerNotes,RequestedAt,CompletedAt,Id,CreatedAt,UpdatedAt")] MaintenanceRequest maintenanceRequest)
        {
            if (ModelState.IsValid)
            {
                _context.Add(maintenanceRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["HouseId"] = new SelectList(_context.Houses, "Id", "Name", maintenanceRequest.HouseId);
            ViewData["TenantId"] = new SelectList(_context.Tenants, "Id", "Id", maintenanceRequest.TenantId);
            return View(maintenanceRequest);
        }

        // GET: MaintenanceRequests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var maintenanceRequest = await _context.MaintenanceRequests.FindAsync(id);
            if (maintenanceRequest == null)
            {
                return NotFound();
            }
            ViewData["HouseId"] = new SelectList(_context.Houses, "Id", "Name", maintenanceRequest.HouseId);
            ViewData["TenantId"] = new SelectList(_context.Tenants, "Id", "Id", maintenanceRequest.TenantId);
            return View(maintenanceRequest);
        }

        // POST: MaintenanceRequests/Edit/5.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TenantId,HouseId,Title,Description,Priority,Status,ManagerNotes,RequestedAt,CompletedAt,Id,CreatedAt,UpdatedAt")] MaintenanceRequest maintenanceRequest)
        {
            if (id != maintenanceRequest.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(maintenanceRequest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MaintenanceRequestExists(maintenanceRequest.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["HouseId"] = new SelectList(_context.Houses, "Id", "Name", maintenanceRequest.HouseId);
            ViewData["TenantId"] = new SelectList(_context.Tenants, "Id", "Id", maintenanceRequest.TenantId);
            return View(maintenanceRequest);
        }

        // GET: MaintenanceRequests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var maintenanceRequest = await _context.MaintenanceRequests
                .Include(m => m.House)
                .Include(m => m.Tenant)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (maintenanceRequest == null)
            {
                return NotFound();
            }

            return View(maintenanceRequest);
        }

        // POST: MaintenanceRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var maintenanceRequest = await _context.MaintenanceRequests.FindAsync(id);
            if (maintenanceRequest != null)
            {
                _context.MaintenanceRequests.Remove(maintenanceRequest);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MaintenanceRequestExists(int id)
        {
            return _context.MaintenanceRequests.Any(e => e.Id == id);
        }
    }
}

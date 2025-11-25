using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TenantsManagementApp.Data;
using TenantsManagementApp.Models;

namespace TenantsManagementApp.Controllers
{

	[Authorize(Roles = "Admin")]
	public class TenantsController : Controller
	{
		private readonly ApplicationDbContext _context;

		public TenantsController(ApplicationDbContext context)
		{
			_context = context;
		}

		// GET: Tenants
		public async Task<IActionResult> Index()
		{
			var applicationDbContext = _context.Tenants.Include(t => t.House).Include(t => t.User);
			return View(await applicationDbContext.ToListAsync());
		}

		// GET: Tenants/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var tenant = await _context.Tenants
				.Include(t => t.House)
				.Include(t => t.User)
				.FirstOrDefaultAsync(m => m.Id == id);
			if (tenant == null)
			{
				return NotFound();
			}

			return View(tenant);
		}

		// GET: Tenants/Create
		public IActionResult Create()
		{
			ViewData["HouseId"] = new SelectList(_context.Houses, "Id", "Name");
			ViewData["UserId"] = new SelectList(_context.Users, "Id", "FirstName");
			return View(new Tenant());
		}

		// POST: Tenants/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("UserId,HouseId,LeaseStartDate,LeaseEndDate,SecurityDeposit,IsActive,Id,CreatedAt,UpdatedAt")] Tenant tenant)
		{
			if (ModelState.IsValid)
			{
				_context.Add(tenant);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			// Populate navigation properties
			if (tenant.UserId != Guid.Empty)
			{
				tenant.User = await _context.Users.FindAsync(tenant.UserId);
			}
			if (tenant.HouseId.HasValue)
			{
				tenant.House = await _context.Houses.FindAsync(tenant.HouseId.Value);
			}
			ViewData["HouseId"] = new SelectList(_context.Houses, "Id", "Name", tenant.HouseId);
			ViewData["UserId"] = new SelectList(_context.Users, "Id", "FirstName", tenant.UserId);
			return View(tenant);
		}

		// GET: Tenants/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var tenant = await _context.Tenants.FindAsync(id);
			if (tenant == null)
			{
				return NotFound();
			}
			ViewData["HouseId"] = new SelectList(_context.Houses, "Id", "Name", tenant.HouseId);
			ViewData["UserId"] = new SelectList(_context.Users, "Id", "FirstName", tenant.UserId);
			return View(tenant);
		}

		// POST: Tenants/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("UserId,HouseId,LeaseStartDate,LeaseEndDate,SecurityDeposit,IsActive,Id,CreatedAt,UpdatedAt")] Tenant tenant)
		{
			if (id != tenant.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(tenant);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!TenantExists(tenant.Id))
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
			ViewData["HouseId"] = new SelectList(_context.Houses, "Id", "Name", tenant.HouseId);
			ViewData["UserId"] = new SelectList(_context.Users, "Id", "FirstName", tenant.UserId);
			return View(tenant);
		}

		// GET: Tenants/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			try
			{
				if (id == null)
				{
					TempData["Error"] = "No tenant ID provided.";
					return RedirectToAction("Index");
				}

				var tenant = await _context.Tenants
					.Include(t => t.House)
					.Include(t => t.User)
					.FirstOrDefaultAsync(m => m.Id == id);
				if (tenant == null)
				{
					TempData["Error"] = $"Tenant with ID {id} not found.";
					return RedirectToAction("Index");
				}

				return View(tenant);
			}
			catch (Exception ex)
			{
				TempData["Error"] = $"An error occurred: {ex.Message}";
				return RedirectToAction("Index");
			}
		}

		// POST: Tenants/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			try
			{
				var tenant = await _context.Tenants.FindAsync(id);
				if (tenant != null)
				{
					_context.Tenants.Remove(tenant);
					await _context.SaveChangesAsync();
					TempData["Success"] = $"Tenant with ID {id} deleted successfully.";
				}
				else
				{
					TempData["Error"] = $"Tenant with ID {id} not found for deletion.";
				}
			}
			catch (Exception ex)
			{
				TempData["Error"] = $"An error occurred during deletion: {ex.Message}";
			}
			return RedirectToAction(nameof(Index));
		}

		private bool TenantExists(int id)
		{
			return _context.Tenants.Any(e => e.Id == id);
		}
	}
}

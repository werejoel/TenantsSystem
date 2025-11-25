using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TenantsManagementApp.Data;
using TenantsManagementApp.Models;

namespace TenantsManagementApp.Controllers
{
    public class ChargesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChargesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Charges
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Charges.Include(c => c.House).Include(c => c.Tenant);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Charges/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var charge = await _context.Charges
                .Include(c => c.House)
                .Include(c => c.Tenant)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (charge == null)
            {
                return NotFound();
            }

            return View(charge);
        }

        // GET: Charges/Create
        public IActionResult Create()
        {
            ViewData["HouseId"] = new SelectList(_context.Houses, "Id", "Name");
            ViewData["TenantId"] = new SelectList(_context.Tenants, "Id", "Id");
            return View();
        }

        // POST: Charges/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenantId,HouseId,ChargeType,Description,Amount,DueDate,Status,Id,CreatedAt,UpdatedAt")] Charge charge)
        {
            if (ModelState.IsValid)
            {
                _context.Add(charge);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["HouseId"] = new SelectList(_context.Houses, "Id", "Name", charge.HouseId);
            ViewData["TenantId"] = new SelectList(_context.Tenants, "Id", "Id", charge.TenantId);
            return View(charge);
        }

        // GET: Charges/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var charge = await _context.Charges.FindAsync(id);
            if (charge == null)
            {
                return NotFound();
            }
            ViewData["HouseId"] = new SelectList(_context.Houses, "Id", "Name", charge.HouseId);
            ViewData["TenantId"] = new SelectList(_context.Tenants, "Id", "Id", charge.TenantId);
            return View(charge);
        }

        // POST: Charges/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TenantId,HouseId,ChargeType,Description,Amount,DueDate,Status,Id,CreatedAt,UpdatedAt")] Charge charge)
        {
            if (id != charge.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(charge);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChargeExists(charge.Id))
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
            ViewData["HouseId"] = new SelectList(_context.Houses, "Id", "Name", charge.HouseId);
            ViewData["TenantId"] = new SelectList(_context.Tenants, "Id", "Id", charge.TenantId);
            return View(charge);
        }

        // GET: Charges/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var charge = await _context.Charges
                .Include(c => c.House)
                .Include(c => c.Tenant)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (charge == null)
            {
                return NotFound();
            }

            return View(charge);
        }

        // POST: Charges/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var charge = await _context.Charges.FindAsync(id);
            if (charge != null)
            {
                _context.Charges.Remove(charge);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChargeExists(int id)
        {
            return _context.Charges.Any(e => e.Id == id);
        }
    }
}

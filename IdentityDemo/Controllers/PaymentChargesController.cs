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
    [Authorize(Roles = "Landlord,Admin")]
    public class PaymentChargesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PaymentChargesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PaymentCharges
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.PaymentCharges.Include(p => p.Charge).Include(p => p.Payment);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: PaymentCharges/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paymentCharge = await _context.PaymentCharges
                .Include(p => p.Charge)
                .Include(p => p.Payment)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (paymentCharge == null)
            {
                return NotFound();
            }

            return View(paymentCharge);
        }

        // GET: PaymentCharges/Create
        public IActionResult Create()
        {
            ViewData["ChargeId"] = new SelectList(_context.Charges, "Id", "ChargeType");
            ViewData["PaymentId"] = new SelectList(_context.Payments, "Id", "Id");
            return View();
        }

        // POST: PaymentCharges/Create
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PaymentId,ChargeId,AmountPaid,Id,CreatedAt,UpdatedAt")] PaymentCharge paymentCharge)
        {
            if (ModelState.IsValid)
            {
                _context.Add(paymentCharge);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ChargeId"] = new SelectList(_context.Charges, "Id", "ChargeType", paymentCharge.ChargeId);
            ViewData["PaymentId"] = new SelectList(_context.Payments, "Id", "Id", paymentCharge.PaymentId);
            return View(paymentCharge);
        }

        // GET: PaymentCharges/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paymentCharge = await _context.PaymentCharges.FindAsync(id);
            if (paymentCharge == null)
            {
                return NotFound();
            }
            ViewData["ChargeId"] = new SelectList(_context.Charges, "Id", "ChargeType", paymentCharge.ChargeId);
            ViewData["PaymentId"] = new SelectList(_context.Payments, "Id", "Id", paymentCharge.PaymentId);
            return View(paymentCharge);
        }

        // POST: PaymentCharges/Edit/5
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PaymentId,ChargeId,AmountPaid,Id,CreatedAt,UpdatedAt")] PaymentCharge paymentCharge)
        {
            if (id != paymentCharge.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(paymentCharge);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaymentChargeExists(paymentCharge.Id))
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
            ViewData["ChargeId"] = new SelectList(_context.Charges, "Id", "ChargeType", paymentCharge.ChargeId);
            ViewData["PaymentId"] = new SelectList(_context.Payments, "Id", "Id", paymentCharge.PaymentId);
            return View(paymentCharge);
        }

        // GET: PaymentCharges/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paymentCharge = await _context.PaymentCharges
                .Include(p => p.Charge)
                .Include(p => p.Payment)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (paymentCharge == null)
            {
                return NotFound();
            }

            return View(paymentCharge);
        }

        // POST: PaymentCharges/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var paymentCharge = await _context.PaymentCharges.FindAsync(id);
            if (paymentCharge != null)
            {
                _context.PaymentCharges.Remove(paymentCharge);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PaymentChargeExists(int id)
        {
            return _context.PaymentCharges.Any(e => e.Id == id);
        }
    }
}

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TenantsManagementApp.Data;
using TenantsManagementApp.Models;
using TenantsManagementApp.Services;

namespace TenantsManagementApp.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IPaymentService _paymentService;

        public PaymentsController(ApplicationDbContext context, IPaymentService paymentService)
        {
            _context = context;
            _paymentService = paymentService;
        }

        // JSON: GET /Payment/GetHistory
        [HttpGet]
        [Route("/Payment/GetHistory")]
        public async Task<IActionResult> GetHistory()
        {
            // Attempt to resolve tenant from current user
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var tenant = await _context.Tenants.FirstOrDefaultAsync(t => t.UserId == userId && t.IsActive);
            if (tenant == null)
                return Json(new List<object>());

            var payments = await _context.Payments
                .Where(p => p.TenantId == tenant.Id)
                .OrderByDescending(p => p.PaymentDate)
                .Select(p => new {
                    id = p.Id,
                    paymentDate = p.PaymentDate,
                    description = p.Notes ?? string.Empty,
                    amount = p.AmountPaid,
                    status = "Completed",
                    paymentMethod = p.PaymentMethod ?? string.Empty,
                    transactionId = p.TransactionReference ?? string.Empty
                }).ToListAsync();

            return Json(payments);
        }

        // JSON: GET /Payment/GetPaymentMethods
        [HttpGet]
        [Route("/Payment/GetPaymentMethods")]
        public async Task<IActionResult> GetPaymentMethods()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();
            var methods = new List<object>();
            return Json(methods);
        }

        // GET: Payments
        public async Task<IActionResult> Index()
        {
            var payments = _context.Payments
                .Include(p => p.House)
                .Include(p => p.Tenant)
                .AsNoTracking();
            return View(await payments.ToListAsync());
        }

        // GET: Payments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payments
                .Include(p => p.House)
                .Include(p => p.Tenant)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        // GET: Payments/Create
        public IActionResult Create()
        {
            var tenants = _context.Tenants.Include(t => t.User).ToList();
            var houses = _context.Houses.ToList();
            ViewBag.TenantId = new SelectList(tenants, "Id", "FullName");
            ViewBag.HouseId = new SelectList(houses, "Id", "Name");
            return View();
        }

    // GET: Payment/AddPaymentMethod
    [HttpGet]
    [Route("/Payment/AddPaymentMethod")]
    public async Task<IActionResult> AddPaymentMethod()
        {
            // Resolve tenant from current user
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                return RedirectToAction("Index", "Home");
            var tenant = await _context.Tenants.Include(t => t.User).FirstOrDefaultAsync(t => t.UserId == userId && t.IsActive);
            if (tenant == null)
            {
                TempData["ErrorMessage"] = "Tenant profile not found.";
                return RedirectToAction("Index", "Home");
            }

            // Load outstanding charges for the tenant to allow selection
            var charges = await _context.Charges
                .Where(c => c.TenantId == tenant.Id && c.Status != "Paid")
                .OrderBy(c => c.DueDate)
                .AsNoTracking()
                .ToListAsync();

            // Provide minimal data to the view via ViewBag
            ViewBag.TenantId = tenant.Id;
            ViewBag.HouseId = tenant.HouseId ?? 0;
            ViewBag.PhoneNumber = tenant.User?.PhoneNumber ?? string.Empty;
            ViewBag.Charges = charges;

            return View();
        }

        // POST: /Payment/InitiateMobilePayment
        // Accepts a JSON payload from the client to initiate a mobile money payment.
        // Returns JSON: { success: bool, message: string, transactionReference?: string, paymentId?: int }
        [HttpPost]
        [Route("/Payment/InitiateMobilePayment")]
        public async Task<IActionResult> InitiateMobilePayment([FromBody] InitiatePaymentDto dto)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { success = false, message = "Unauthorized" });

            var tenant = await _context.Tenants.FirstOrDefaultAsync(t => t.UserId == userId && t.IsActive);
            if (tenant == null)
                return BadRequest(new { success = false, message = "Tenant not found" });

            try
            {
                //call the payment provider (Flutterwave)
                // with a generated transaction reference and return it to the client.
                var txRef = Guid.NewGuid().ToString();

                // Build the internal InitiatePaymentRequest used by PaymentService
                if (!tenant.HouseId.HasValue || tenant.HouseId.Value <= 0)
                {
                    return BadRequest(new { success = false, message = "Tenant is not assigned to a house. Cannot create payment." });
                }

                var req = new TenantsManagementApp.Models.FlutterWave.InitiatePaymentRequest
                {
                    TenantId = tenant.Id,
                    HouseId = tenant.HouseId.Value,
                    AmountPaid = dto.Amount,
                    PaymentDate = DateTime.Now,
                    Provider = dto.Provider ?? "",
                    Purpose = dto.Purpose ?? string.Empty,
                    PeriodStart = null,
                    PeriodEnd = null,
                    Notes = dto.Purpose,
                    RedirectUrl = null,
                    ChargeIds = dto.SelectedChargeIds ?? new List<int>()
                };

                // Use PaymentService which calls FlutterwaveService internally and persist payment
                var payment = await _paymentService.CreatePaymentAsync(req);
                return Json(new { success = true, message = "Payment initiated", transactionReference = payment.TransactionReference, paymentId = payment.Id });
            }
            catch (Exception ex)
            {
                // Log full exception for debugging
                Console.WriteLine("InitiateMobilePayment error: " + ex.ToString());
                return StatusCode(500, new { success = false, message = "An error occurred while initiating payment" });
            }
        }

        public class InitiatePaymentDto
        {
            public string? Provider { get; set; }
            public string? Phone { get; set; }
            public decimal Amount { get; set; }
            public string? Purpose { get; set; }
            public List<int> SelectedChargeIds { get; set; } = new List<int>();
        }
        // POST: Payments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenantId,HouseId,AmountPaid,PaymentDate,PeriodStart,PeriodEnd,PaymentMethod,TransactionReference,Notes")] Payment payment)
        {
            var tenants = _context.Tenants.Include(t => t.User).ToList();
            var houses = _context.Houses.ToList();
            ViewBag.TenantId = new SelectList(tenants, "Id", "FullName", payment.TenantId);
            ViewBag.HouseId = new SelectList(houses, "Id", "Name", payment.HouseId);
            payment.CreatedAt = DateTime.Now;
            payment.UpdatedAt = DateTime.Now;
            if (ModelState.IsValid)
            {
                _context.Add(payment);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Payment created successfully.";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Invalid payment data. Please check the form and try again.";
            return View(payment);
        }


        // GET: Payments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payments.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            if (payment == null)
            {
                return NotFound();
            }

            var tenants = _context.Tenants.Include(t => t.User).AsNoTracking().ToList();
            var houses = _context.Houses.AsNoTracking().ToList();

            if (!tenants.Any() || !houses.Any())
            {
                TempData["ErrorMessage"] = "Cannot edit payment: No tenants or houses available.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.TenantId = new SelectList(tenants, "Id", "FullName", payment.TenantId);
            ViewBag.HouseId = new SelectList(houses, "Id", "Name", payment.HouseId);
            return View(payment);
        }

        // POST: Payments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TenantId,HouseId,AmountPaid,PaymentDate,PeriodStart,PeriodEnd,PaymentMethod,TransactionReference,Notes,CreatedAt")] Payment payment)
        {
            if (id != payment.Id)
            {
                return NotFound();
            }
            var tenants = _context.Tenants.Include(t => t.User).AsNoTracking().ToList();
            var houses = _context.Houses.AsNoTracking().ToList();
            ViewBag.TenantId = new SelectList(tenants, "Id", "FullName", payment.TenantId);
            ViewBag.HouseId = new SelectList(houses, "Id", "Name", payment.HouseId);

            // Validate foreign keys
            if (payment.TenantId > 0 && !_context.Tenants.Any(t => t.Id == payment.TenantId))
            {
                ModelState.AddModelError("TenantId", "Selected tenant does not exist.");
            }
            if (payment.HouseId > 0 && !_context.Houses.Any(h => h.Id == payment.HouseId))
            {
                ModelState.AddModelError("HouseId", "Selected house does not exist.");
            }

            // Validate period dates if provided
            if (payment.PeriodStart.HasValue && payment.PeriodEnd.HasValue && payment.PeriodEnd < payment.PeriodStart)
            {
                ModelState.AddModelError("PeriodEnd", "Period End cannot be before Period Start.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    payment.UpdatedAt = DateTime.Now;
                    _context.Update(payment);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Payment updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaymentExists(payment.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                catch (DbUpdateException ex)
                {
                    Console.WriteLine($"DbUpdateException: {ex.InnerException?.Message ?? ex.Message}");
                    ModelState.AddModelError("", "An error occurred while updating the payment. Please ensure all data is valid.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected error: {ex.Message}");
                    ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
                }
            }
            else
            {
                // Log ModelState errors
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Console.WriteLine("ModelState Errors: " + string.Join("; ", errors));
            }

            return View(payment);
        }

        // GET: Payments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payments
                .Include(p => p.House)
                .Include(p => p.Tenant)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        // POST: Payments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var payment = await _context.Payments.FindAsync(id);
                if (payment == null)
                {
                    TempData["ErrorMessage"] = "Payment not found.";
                }
                else
                {
                    _context.Payments.Remove(payment);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Payment deleted successfully.";
                }
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.InnerException?.Message ?? ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while deleting the payment.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                TempData["ErrorMessage"] = "An unexpected error occurred.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool PaymentExists(int id)
        {
            return _context.Payments.Any(e => e.Id == id);
        }
    }
}
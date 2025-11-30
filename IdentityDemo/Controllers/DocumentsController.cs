using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TenantsManagementApp.Data;
using TenantsManagementApp.ViewModels.TenantsDashboard;

namespace TenantsManagementApp.Controllers
{
    [Authorize(Roles = "Tenant,Landlord,Admin")]
    public class DocumentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DocumentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("/Documents/GetTenantDocuments")]
        public async Task<IActionResult> GetTenantDocuments()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var tenant = await _context.Tenants.FirstOrDefaultAsync(t => t.UserId == userId && t.IsActive);
            if (tenant == null)
                return Json(new List<DocumentViewModel>());

            var docs = await _context.Documents
                .Where(d => d.TenantId == tenant.Id || d.HouseId == tenant.HouseId)
                .OrderByDescending(d => d.UploadedAt)
                .ToListAsync();

            var vm = docs.Select(d =>
            {
                long parsedSize = 0L;
                long.TryParse(d.FileSize ?? "0", out parsedSize);
                return new DocumentViewModel
                {
                    Id = d.Id,
                    FileName = d.FileName,
                    FileType = d.MimeType ?? string.Empty,
                    FileSize = parsedSize,
                    Category = d.DocumentType,
                    UploadDate = d.UploadedAt,
                    Description = d.FilePath
                };
            }).ToList();

            return Json(vm);

            return Json(docs);
        }
    }
}

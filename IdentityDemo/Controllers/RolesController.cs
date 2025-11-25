using TenantsManagementApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TenantsManagementApp.ViewModels.Roles;
namespace TenantsManagementApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {
        private readonly IRoleService _roleService;
        private readonly ILogger<RolesController> _logger;
        public RolesController(IRoleService roleService, ILogger<RolesController> logger)
        {
            _roleService = roleService;
            _logger = logger;
        }
        // GET: /Roles
        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] RoleListFilterViewModel filter)
        {
            try
            {
                var result = await _roleService.GetRolesAsync(filter);
                ViewBag.Filter = filter;
                return View(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching roles in Index action.");
                return View("Error");
            }
        }
        // GET: /Roles/Create
        [HttpGet]
        public IActionResult Create()
        {
            try
            {
                return View(new RoleCreateViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rendering Create Role form.");
                return View("Error");
            }
        }
        // POST: /Roles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoleCreateViewModel model)
        {
            try
            {
                // DataAnnotations validation first
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                var (result, id) = await _roleService.CreateAsync(model);
                if (result.Succeeded)
                {
                    TempData["Success"] = $"Role '{model.Name}' created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                // Map IdentityResult errors to MODEL-LEVEL errors
                foreach (var e in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, e.Description);
                }
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating role '{RoleName}'.", model?.Name);
                return View("Error");
            }
        }
        // GET: /Roles/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var vm = await _roleService.GetForEditAsync(id);
                if (vm == null) return NotFound();
                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching role for edit. RoleId: {RoleId}", id);
                return View("Error");
            }
        }
        // POST: /Roles/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RoleEditViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                var result = await _roleService.UpdateAsync(model);
                if (result.Succeeded)
                {
                    TempData["Success"] = $"Role '{model.Name}' updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                // Map IdentityResult errors to MODEL-LEVEL errors
                foreach (var e in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, e.Description);
                }
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating role '{RoleName}'.", model?.Name);
                return View("Error");
            }
        }
        // GET: /Roles/Details/{id}?page=1&pageSize=4
        [HttpGet]
        public async Task<IActionResult> Details(Guid id, int pageNumber = 1, int pageSize = 4)
        {
            try
            {
                var vm = await _roleService.GetDetailsAsync(id, pageNumber, pageSize);
                if (vm == null) return NotFound();
                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching role details. RoleId: {RoleId}", id);
                return View("Error");
            }
        }
        // POST: /Roles/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var result = await _roleService.DeleteAsync(id);
                if (result.Succeeded)
                {
                    TempData["Success"] = "Role deleted.";
                }
                else
                {
                    TempData["Error"] = string.Join(" ", result.Errors.Select(e => e.Description));
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting role. RoleId: {RoleId}", id);
                return View("Error");
            }
        }

        //ManageClaims (GET):
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> ManageClaims(Guid id)
        {
            var roleClaimsEditViewModel = await _roleService.GetClaimsForEditAsync(id);
            if (roleClaimsEditViewModel == null)
            {
                TempData["Error"] = "The role was not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(roleClaimsEditViewModel);
        }

        //ManageClaims (POST):
        [Authorize(Roles = "Admin")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageClaims(RoleClaimsEditViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var selected = model.Claims.Where(c => c.IsSelected).Select(c => c.ClaimId).ToList();
                var result = await _roleService.UpdateClaimsAsync(model.RoleId, selected);

                if (result.Succeeded)
                {
                    TempData["Success"] = "Role claims were updated successfully.";
                    return RedirectToAction(nameof(Details), new { id = model.RoleId });
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                var reload = await _roleService.GetClaimsForEditAsync(model.RoleId);
                return View(reload ?? model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in Managing Role Claims. RoleName: {model.RoleName}");
                return View("Error");
            }
        }

        // GET: /Roles/ManageUsers/{roleId}
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> ManageUsers(Guid roleId)
        {
            var role = await _roleService.GetForEditAsync(roleId);
            if (role == null)
            {
                TempData["Error"] = "Role not found.";
                return RedirectToAction(nameof(Index));
            }
            var allUsers = await _roleService.GetAllUsersAsync();
            var assignedUserIds = (await _roleService.GetDetailsAsync(roleId, 1, int.MaxValue))?.Users.Items.Select(u => u.Id).ToHashSet() ?? new HashSet<Guid>();
            var vm = new ManageUsersInRoleViewModel
            {
                RoleId = roleId,
                RoleName = role.Name,
                Users = allUsers.Select(u => new UserInRoleAssignmentViewModel
                {
                    UserId = u.Id,
                    Email = u.Email,
                    IsAssigned = assignedUserIds.Contains(u.Id)
                }).ToList()
            };
            return View(vm);
        }

        // POST: /Roles/ManageUsers
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageUsers(ManageUsersInRoleViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var assignments = model.Users.Select(u => new UserInRoleAssignmentViewModel
            {
                UserId = u.UserId,
                Email = u.Email,
                IsAssigned = u.IsAssigned
            }).ToList();
            var result = await _roleService.UpdateUsersInRoleAsync(model.RoleId, assignments);
            if (result.Succeeded)
            {
                TempData["Success"] = "User assignments updated.";
                return RedirectToAction("Details", new { id = model.RoleId });
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }
    }
}
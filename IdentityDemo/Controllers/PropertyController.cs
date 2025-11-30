using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TenantsManagementApp.Controllers
{
    [Authorize(Roles = "Tenant,Landlord,Admin")]
    public class PropertyController : Controller
    {
        public IActionResult ScheduleTour()
        {
            //scheduling logic to be implemented
            return View();
        }
    }
}

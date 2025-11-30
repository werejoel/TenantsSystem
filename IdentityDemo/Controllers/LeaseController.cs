using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TenantsManagementApp.Controllers
{
    [Authorize(Roles = "Tenant,Landlord,Admin")]
    public class LeaseController : Controller
    {
        public IActionResult RenewLease()
        {
            //lease renewal logic to be implemented
            return View();
        }
    }
}

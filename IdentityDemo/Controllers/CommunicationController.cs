using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TenantsManagementApp.Controllers
{
    [Authorize(Roles = "Tenant,Landlord,Admin")]
    public class CommunicationController : Controller
    {
        public IActionResult Messages()
        {
            //Fetch messages for the tenant logic
            return View();
        }

        public IActionResult Notifications()
        {
            //Fetch notifications for the tenant logic
            return View();
        }
    }
}

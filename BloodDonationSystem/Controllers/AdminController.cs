using Microsoft.AspNetCore.Mvc;

namespace BloodDonationSystem.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Admin()
        {
            return View();
        }
    }
}

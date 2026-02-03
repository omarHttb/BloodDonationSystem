using BloodDonationSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonationSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly IUserService _userService;
        public AdminController(IUserService userService)
        {
            _userService = userService;
        }
        public IActionResult Admin()
        {
            return View();
        }



        public IActionResult BloodTypes()
        {
            return View();
        }

        public IActionResult DonationApprovals()
        {
            return View();
        }

        public IActionResult Reports()
        {
            return View();
        }

        public async Task<IActionResult> Users()
        {
            var userList = await _userService.GetAllUsersWithDetailsAsync();

            return View(userList);
        }
    }
}

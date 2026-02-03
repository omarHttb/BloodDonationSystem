using BloodDonationSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonationSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly IUserService _userService;
        private readonly IBloodTypeService _bloodTypeService;
        public AdminController(IUserService userService , IBloodTypeService bloodTypeService)
        {
            _bloodTypeService = bloodTypeService;
            _userService = userService;
        }
        public IActionResult Admin()
        {
            return View();
        }



        public async Task<IActionResult> BloodTypes()
        {
            var bloodtypes = await _bloodTypeService.GetAllBloodTypeAsync();    

            return View(bloodtypes);
        }

        public async Task<IActionResult> DonationApprovals()
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

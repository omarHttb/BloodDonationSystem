using BloodDonationSystem.Models;
using BloodDonationSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonationSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly IUserRoleService _userRoleService;
        public AccountController(IUserService userService, IUserRoleService userRoleService)
        {
            _userService = userService;
            _userRoleService = userRoleService;
        }
        public IActionResult Register()
        {
            return View();
        }
        public IActionResult Login()
        {

            return View("Login");
        }

        [HttpPost]
        public IActionResult UserLogin(User user)
        {
            var result = _userService.LoginUser(user).Result;
            if (result == true)
            {
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError("password", "Invalid username or password.");

            return View("Login");
        }

        public IActionResult RegisterAccount(User user) 
        {
            var result = _userService.RegisterUserAsync(user).Result;
            if (result != -1)
            {
                var userRole = new UserRole
                {
                    UserId = result,
                    RoleId = 1
                };
                _userRoleService.CreateUserRoleAsync(userRole);
                return RedirectToAction("Login");
            }
            ModelState.AddModelError("Name", "This username is already taken.");
            return View("Register");
        }
    }
}

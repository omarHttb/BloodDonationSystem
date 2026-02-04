using BloodDonationSystem.Models;
using BloodDonationSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        public async Task<IActionResult> UserLogin(User user)
        {
            int loggedInUserId = await _userService.LoginUser(user);

            if (loggedInUserId != -1) 
            {
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Name),
            new Claim("UserID", loggedInUserId.ToString())
        };

                var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");
                var authProperties = new AuthenticationProperties { IsPersistent = true };

                await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(claimsIdentity), authProperties);

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View("Login");
        }

        [HttpPost]
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

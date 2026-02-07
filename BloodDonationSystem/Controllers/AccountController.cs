using BloodDonationSystem.Models;
using BloodDonationSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
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
        public async Task <IActionResult> RegisterAccount(User user) 
        {

            if (user.PhoneNumber.IsNullOrEmpty())
            {
            ModelState.AddModelError("PhoneNumber", "Phone Number is Required.");

            }
            else if (await _userService.IsPhoneNumberExist(user.PhoneNumber))
            {
                ModelState.AddModelError("PhoneNumber", "This phone number is already taken.");
            }
            if (user.Email.IsNullOrEmpty())
            {
                ModelState.AddModelError("Email", "Email is Required.");

            }
            else if (await _userService.IsEmailExist(user.Email))
            {
                ModelState.AddModelError("PhoneNumber", "This Email is already taken.");
            }

            if (user.Name.IsNullOrEmpty())
            {
                ModelState.AddModelError("Name", "Username is Required.");

            }
            else if (await _userService.IsUsernameExist(user.Name))
            {
                ModelState.AddModelError("Name", "This Username is already taken.");
                
            }

            var result = await _userService.RegisterUserAsync(user);
            if (result != -1)
            {
                var userRole = new UserRole
                {
                    UserId = result,
                    RoleId = 1
                };
               await _userRoleService.CreateUserRoleAsync(userRole);
                return RedirectToAction("Login");
            }
            return View("Register");
        }
    }
}

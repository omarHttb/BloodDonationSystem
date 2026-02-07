using BloodDonationSystem.Models;
using BloodDonationSystem.Services;
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
        private readonly IOtpService _otpService;
        private readonly TokenService _tokenService;
        public AccountController(IUserService userService, IUserRoleService userRoleService, IOtpService otpService, TokenService tokenService)
        {
            _userService = userService;
            _userRoleService = userRoleService;
            _otpService = otpService;
            _tokenService = tokenService;
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
                // Credentials OK! Save info to TempData for the next step
                TempData["LoginUserId"] = loggedInUserId.ToString();
                TempData["LoginUserName"] = user.Name;

                return RedirectToAction("LoginOTP");
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View("Login");
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAccount(User user)
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

            if (!ModelState.IsValid) return View("Register", user);

            // Store the user object as a JSON string in TempData
            TempData["PendingUser"] = System.Text.Json.JsonSerializer.Serialize(user);

            return RedirectToAction("RegisterOTP");
 
            
        }

        
        public async Task<IActionResult> RegisterOTP()
        {
           var otp = await _otpService.GenerateOtpAsync();
            return View("RegisterOTP", otp);
        }

        [HttpPost]
        public async Task<IActionResult> VerifyRegisterOTP(string userEnteredOtp)
        {
            // 1. Validate the OTP via your service
            bool isValid = await _otpService.ValidateOtpAsync(userEnteredOtp);

            if (isValid && TempData["PendingUser"] is string userJson)
            {
                // 2. Retrieve and deserialize the user
                var user = System.Text.Json.JsonSerializer.Deserialize<User>(userJson);

                // 3. FINALLY save to database
                var result = await _userService.RegisterUserAsync(user);
                if (result != -1)
                {
                    await _userRoleService.CreateUserRoleAsync(new UserRole { UserId = result, RoleId = 1 });
                    return RedirectToAction("Login");
                }
            }

            ModelState.AddModelError("", "Invalid OTP or Session Expired.");
            return View("RegisterOTP");
        }


        public async Task<IActionResult> LoginOTP()
        {
            var otp = await _otpService.GenerateOtpAsync();
            return View("LoginOTP", otp);
        }

        [HttpPost]
        public async Task<IActionResult> VerifyLoginOTP(string userEnteredOtp)
        {
            bool isValid = await _otpService.ValidateOtpAsync(userEnteredOtp);

            if (isValid && TempData["LoginUserId"] is string userId && TempData["LoginUserName"] is string userName)
            {
                var roles = await _userService.GetUserRolesAsync(int.Parse(userId));

                var token = _tokenService.CreateToken(userId, userName, roles);
                Response.Cookies.Append("X-Access-Token", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, 
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTime.UtcNow.AddHours(8)
                });

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid OTP or Session Expired.");
            return View("LoginOTP");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpPost] 
        public IActionResult Logout()
        {
            Response.Cookies.Delete("X-Access-Token");


            return RedirectToAction("Login");
        }
    }
}

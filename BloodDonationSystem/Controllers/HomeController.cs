using BloodDonationSystem.Models;
using BloodDonationSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.Security.Claims;

namespace BloodDonationSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDonorService _donorService;

        public HomeController(ILogger<HomeController> logger , IDonorService donorService)
        {
            _logger = logger;
            _donorService = donorService;
        }

        [Authorize(Roles = "Admin")]
        [Authorize(Roles = "Hospital")]
        [Authorize(Roles = "Donor")]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId.IsNullOrEmpty() || userId == "0")
            {
                return RedirectToAction("Login", "Account");
            }

            var donor =  await _donorService.GetDonorByUserIdAsync(int.Parse(userId));

            return View(donor);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Authorize(Roles = "Hospital")]
        [Authorize(Roles = "Donor")]
        public async Task<IActionResult> CreateDonor(Donor donor)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId.IsNullOrEmpty() || userId == "0")
            {
                return RedirectToAction("Login", "Account");
            }

            donor.UserId = int.Parse(userId);

            await _donorService.CreateDonorAsync(donor);

            return View("Index");
        }
        [Authorize(Roles = "Admin")]
        [Authorize(Roles = "Hospital")]
        [Authorize(Roles = "Donor")]
        public IActionResult Privacy()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        [Authorize(Roles = "Hospital")]
        [Authorize(Roles = "Donor")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [Authorize(Roles = "Admin")]
        [Authorize(Roles = "Hospital")]
        [Authorize(Roles = "Donor")]
        [HttpPost]

        public async Task<IActionResult> SetDonorToAvailable()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId.IsNullOrEmpty() || userId == "0")
            {
                return RedirectToAction("Login", "Account");
            }
            var donor = await _donorService.GetDonorByUserIdAsync(int.Parse(userId));
            if (donor == null)
            {
                return RedirectToAction("Index");
            }
            await _donorService.UpdateDonorToAvailable(donor);
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin")]
        [Authorize(Roles = "Hospital")]
        [Authorize(Roles = "Donor")]
        public async Task<IActionResult> SetDonorToUnAvailable()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


            if (userId.IsNullOrEmpty() || userId == "0")
            {
                return RedirectToAction("Login", "Account");
            }
            var donor = await _donorService.GetDonorByUserIdAsync(int.Parse(userId));
            if (donor == null)
            {
                return RedirectToAction("Index");
            }
            await _donorService.UpdateDonorToUnAvailable(donor);
            return RedirectToAction("Index");
        }

    }
}

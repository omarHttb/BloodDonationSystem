using BloodDonationSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonationSystem.Controllers
{
    public class DonorsController : Controller
    {
        private readonly IDonorService _donorService;

        public DonorsController(IDonorService donorService)
        {
            _donorService = donorService;
        }
        public async Task<IActionResult> Donors()
        {
            
            var donors = await _donorService.GetDonorManagementData();
            
            return View(donors);
        }

        public  async Task<IActionResult> UpdateBloodType(int DonorId, int BloodTypeId)
        {
            var donors = await _donorService.GetDonorManagementData();

            var result = await _donorService.UpdateDonorBloodType(DonorId, BloodTypeId);

            return View("Donors", donors);


        }
    }
}

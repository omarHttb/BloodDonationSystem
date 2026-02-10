using BloodDonationSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonationSystem.Controllers
{
    public class BloodBankController : Controller
    {

        private readonly IBloodBankService _bloodBankService;

        public BloodBankController(IBloodBankService bloodBankService, IBloodBankHistoryService bloodBankHistoryService)
        {
            _bloodBankService = bloodBankService;
        }

        [Authorize(Roles = "Admin, Hospital")]
        public async Task<IActionResult> BloodBank()
        {

            var bloodBanks = await _bloodBankService.GetAllBloodBanksAsync();

            return View(bloodBanks);
        }


        [Authorize(Roles = "Admin, Hospital")]

        public async Task<IActionResult> TakeFromBloodBank(int BloodBankId, int quantityTaken)
        {
            var bloodBank = await _bloodBankService.SubtractFromBloodBank(BloodBankId, quantityTaken);

            var bloodBanks = await _bloodBankService.GetAllBloodBanksAsync();

            return View("BloodBank", bloodBanks);
        }
    }
}

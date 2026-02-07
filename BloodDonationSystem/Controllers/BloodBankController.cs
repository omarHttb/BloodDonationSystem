using BloodDonationSystem.Services.Interfaces;
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

        public async Task<IActionResult> BloodBank()
        {

            var bloodBanks = await _bloodBankService.GetAllBloodBanksAsync();

            return View(bloodBanks);
        }

        public async Task<IActionResult> TakeFromBloodBank(int BloodBankId, int quantityTaken)
        {
            var bloodBank = await _bloodBankService.SubtractFromBloodBank(BloodBankId, quantityTaken);

            var bloodBanks = await _bloodBankService.GetAllBloodBanksAsync();

            return View("BloodBank", bloodBanks);
        }
    }
}

using BloodDonationSystem.Services;
using BloodDonationSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonationSystem.Controllers
{
    public class DonationsController : Controller
    {

        private readonly IDonationService _donationService;

        public DonationsController(IDonationService donationService)
        {
            _donationService = donationService;
        }


        public async Task< IActionResult> Donations()
        {
            var DonationsWithBloodRequestAndDonor = await _donationService.GetAllDonationsWithBloodRequestAndDonor();

            return View(DonationsWithBloodRequestAndDonor);
        }

        [HttpPost]
        public async Task<IActionResult> CompleteDonation(int donationId, int quantityDonated, int quantityNeeded)
        {
            var donation = await _donationService.GetDonationByIdAsync(donationId);

            if (donation == null)
                return NotFound();



            if (quantityDonated == 0) {
                        ModelState.AddModelError(
                    "QuantityDonatedIsZero",
                    $"Donated quantity cannot be zero"
                );

                        var donations = await _donationService.GetAllDonationsWithBloodRequestAndDonor();
                        return View("Donations", donations);
            }

            if (quantityDonated > quantityNeeded)
            {
                ModelState.AddModelError(
                    "QuantityDonated",
                    $"Donated quantity cannot exceed required quantity ({quantityNeeded})."
                );

                var donations = await _donationService.GetAllDonationsWithBloodRequestAndDonor();
                return View("Donations", donations);
            }

            donation.Quantity = quantityDonated;

            await _donationService.CompleteDonation(donation);

            return RedirectToAction("Donations");
        }
    }
}

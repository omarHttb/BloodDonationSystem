using BloodDonationSystem.Models;
using BloodDonationSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BloodDonationSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly IUserService _userService;
        private readonly IBloodTypeService _bloodTypeService;
        private readonly IBloodRequestService _bloodRequestService;
        private readonly IDonationService _donationService;
        public AdminController(IUserService userService , IBloodTypeService bloodTypeService, IBloodRequestService bloodRequestService, IDonationService donationService)
        {
            _bloodTypeService = bloodTypeService;
            _userService = userService;
            _bloodRequestService = bloodRequestService;
            _donationService = donationService;

        }
        //[Authorize(Roles = "Admin")]
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
            var DonationsWithBloodRequestAndDonor = await _donationService.GetAllDonationsWithBloodRequestAndDonor();

            return View("DonationApprovals", DonationsWithBloodRequestAndDonor);
        }

        public async Task<IActionResult> BloodRequestApprovals()
        {
            var bloodRequests = await _bloodRequestService.GetAllBloodRequestWithBloodTypesAsync();


            return View("BloodRequestsApprovals",bloodRequests);
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

        [HttpPost]
        
        public async Task<IActionResult> Approve(int id)
        {
            var success = await _bloodRequestService.ApproveBloodRequest(id);

            if (!success) return NotFound();

            var bloodRequests = await _bloodRequestService.GetAllBloodRequestWithBloodTypesAsync();


            return View("BloodRequestsApprovals", bloodRequests);
        }

        [HttpPost]
        public async Task<IActionResult> Reject(int id)
        {
            var success = await _bloodRequestService.DisApproveBloodRequest(id);

            if (!success) return NotFound();


            var bloodRequests = await _bloodRequestService.GetAllBloodRequestWithBloodTypesAsync();

            return View("BloodRequestsApprovals", bloodRequests);
        }

        public async Task<IActionResult> RejectDonation(int BloodRequestId)
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


            var donation = await _donationService.GetDonationByBloodRequestIdAsync(BloodRequestId);

            await _donationService.RejectDonation(donation);

            var DonationsWithBloodRequestAndDonor = await _donationService.GetAllDonationsWithBloodRequestAndDonor();

            return View("DonationApprovals", DonationsWithBloodRequestAndDonor);


        }


        public async Task<IActionResult> ApproveDonation(int BloodRequestId)
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


            var donation = await _donationService.GetDonationByBloodRequestIdAsync(BloodRequestId);

            await _donationService.ApproveDonation(donation);

            var DonationsWithBloodRequestAndDonor = await _donationService.GetAllDonationsWithBloodRequestAndDonor();

            return View("DonationApprovals", DonationsWithBloodRequestAndDonor);


        }
    }
}

using BloodDonationSystem.DTOS;
using BloodDonationSystem.Models;
using BloodDonationSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly IDonorService _donorService;
        private readonly IBloodBankService _bloodBankService;
        private readonly IRoleService _roleService;
        private readonly IUserRoleService _userRoleService;
        public AdminController(IUserService userService , IBloodTypeService bloodTypeService, IBloodRequestService bloodRequestService,
            IDonationService donationService, IDonorService donorService, IBloodBankService bloodBankService, IRoleService roleService, IUserRoleService userRoleService)
        {
            _bloodTypeService = bloodTypeService;
            _userService = userService;
            _bloodRequestService = bloodRequestService;
            _donationService = donationService;
            _donorService = donorService;
            _bloodBankService = bloodBankService;
            _roleService = roleService;
            _userRoleService = userRoleService;
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


        public async Task< IActionResult> Reports()
        {
            var totalDonors = await _donorService.TotalNumberOfDonors();
            var totalDonations = await _donationService.TotalNumberOfDonations();
            var totalNumberOfBloodRequests = await _bloodRequestService.TotalNumberOfBloodRequests();
            var bloodBanks = await _bloodBankService.GetAllBloodBanksAsync();

            var report = new StatusReportsDTO
            {
                TotalDonors = totalDonors,
                TotalDonations = totalDonations,
                TotalBloodRequests = totalNumberOfBloodRequests,
                BloodBanks = bloodBanks
            };

            return View(report);
        }

        public async Task<IActionResult> Users()
        {
            var userList = await _userService.GetAllUsersWithDetailsAsync();
            var roles = await _roleService.GetAllRoleAsync();
            ViewBag.AllRoles = roles;
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


        [HttpPost]
        public async Task<IActionResult> UpdateUser(int Id, List<int> RoleIds)
        {
            // Id is the UserId from the hidden input in your modal
            // RoleIds is the list of checked checkbox values
            var success = await _userRoleService.UpdateUserRolesAsync  (Id, RoleIds);

            if (!success)
            {
                return RedirectToAction("Users");
            }

            return RedirectToAction("Users");
        }
        //[HttpPost]
        //TODO
        //public async Task<IActionResult> UpdateUser(string Id, List<string> Roles)
        //{
        //    var user = await _userManager.FindByIdAsync(Id);
        //    if (user == null) return NotFound();

        //    var currentRoles = await _userManager.GetRolesAsync(user);

        //    await _userManager.RemoveFromRolesAsync(user, currentRoles);

        //    // 3. Add new selected roles
        //    if (Roles != null && Roles.Any())
        //    {
        //        await _userManager.AddToRolesAsync(user, Roles);
        //    }

        //    return RedirectToAction("AllUsers");
        //}
    }
}

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
        private readonly IBloodBankHistoryService _bloodBankHistoryService;
        private readonly IRoleService _roleService;
        private readonly IUserRoleService _userRoleService;
        public AdminController(IUserService userService , IBloodTypeService bloodTypeService, IBloodRequestService bloodRequestService,
            IDonationService donationService, IDonorService donorService, IBloodBankService bloodBankService, IRoleService roleService, IUserRoleService userRoleService
            , IBloodBankHistoryService bloodBankHistoryService)
        {
            _bloodTypeService = bloodTypeService;
            _userService = userService;
            _bloodRequestService = bloodRequestService;
            _donationService = donationService;
            _donorService = donorService;
            _bloodBankService = bloodBankService;
            _roleService = roleService;
            _userRoleService = userRoleService;
            _bloodBankHistoryService = bloodBankHistoryService;
        }

        [Authorize(Roles = "Admin")]
        public async Task< IActionResult> Admin()
        {
            var BloodBankHistory = await _bloodBankHistoryService.GetAllBloodBankHistoryAsync();
            return View(BloodBankHistory);
        }

        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> BloodTypes()
        {
            var bloodtypes = await _bloodTypeService.GetAllBloodTypeAsync();    

            return View(bloodtypes);
        }

        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> DonationApprovals()
        {
            var DonationsWithBloodRequestAndDonor = await _donationService.GetAllDonationsWithBloodRequestAndDonor();

            return View("DonationApprovals", DonationsWithBloodRequestAndDonor);
        }

        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> BloodRequestApprovals()
        {
            var bloodRequests = await _bloodRequestService.GetAllBloodRequestWithBloodTypesAsync();


            return View("BloodRequestsApprovals",bloodRequests);
        }

        [Authorize(Roles = "Admin")]

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

        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Users()
        {
            var userList = await _userService.GetAllUsersWithDetailsAsync();
            var roles = await _roleService.GetAllRoleAsync();
            ViewBag.AllRoles = roles;
            var bloodTypes = await _bloodTypeService.GetAllBloodTypeAsync();
            ViewBag.BloodTypes = bloodTypes;

            return View(userList);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Approve(int id)
        {
            var success = await _bloodRequestService.ApproveBloodRequest(id);

            if (!success) return NotFound();

            var bloodRequests = await _bloodRequestService.GetAllBloodRequestWithBloodTypesAsync();


            return View("BloodRequestsApprovals", bloodRequests);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Reject(int id)
        {
            var success = await _bloodRequestService.DisApproveBloodRequest(id);

            if (!success) return NotFound();


            var bloodRequests = await _bloodRequestService.GetAllBloodRequestWithBloodTypesAsync();

            return View("BloodRequestsApprovals", bloodRequests);
        }
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> RejectDonation(int BloodRequestId)
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


            var donation = await _donationService.GetDonationByBloodRequestIdAsync(BloodRequestId);

            await _donationService.RejectDonation(donation);

            var DonationsWithBloodRequestAndDonor = await _donationService.GetAllDonationsWithBloodRequestAndDonor();

            return View("DonationApprovals", DonationsWithBloodRequestAndDonor);


        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveDonation(int BloodRequestId)
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


            var donation = await _donationService.GetDonationByBloodRequestIdAsync(BloodRequestId);

            await _donationService.ApproveDonation(donation);

            var DonationsWithBloodRequestAndDonor = await _donationService.GetAllDonationsWithBloodRequestAndDonor();

            return View("DonationApprovals", DonationsWithBloodRequestAndDonor);

        }


        [HttpPost]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> UpdateUser(int Id, List<int> RoleIds ,string username,string email,string phoneNumber,int bloodTypeId, bool isAvailable)
        {
            var success = await _userRoleService.UpdateUserRolesAsync  (Id, RoleIds);

            var updatedUser = new User()
            {
                Id = Id,
                Name = username,
                Email = email,
                PhoneNumber = phoneNumber,
            };
            await _userService.UpdateUserAsync(Id, updatedUser);
            
            var donor = await _donorService.GetDonorByUserIdAsync(Id);
            if (donor != null)
            {
               await _donorService.UpdateDonorBloodType(donor.Id, bloodTypeId);

                await _donorService.setDonorAvailability(isAvailable, donor.Id);
            }


            if (!success)
            {
                return RedirectToAction("Users");
            }

            return RedirectToAction("Users");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> MakeUserADoner(int id)
        {

            var newDoner = new Donor() {
                UserId = id,
                IsAvailable = false

            };

            await _donorService.CreateDonorAsync(newDoner);

            return RedirectToAction("Users");
        }
    }
}

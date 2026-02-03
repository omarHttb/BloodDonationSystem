using BloodDonationSystem.Models;
using BloodDonationSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonationSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly IUserService _userService;
        private readonly IBloodTypeService _bloodTypeService;
        private readonly IBloodRequestService _bloodRequestService;
        public AdminController(IUserService userService , IBloodTypeService bloodTypeService, IBloodRequestService bloodRequestService)
        {
            _bloodTypeService = bloodTypeService;
            _userService = userService;
            _bloodRequestService = bloodRequestService;
        }
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
            return View();
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
    }
}

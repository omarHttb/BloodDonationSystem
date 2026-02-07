using BloodDonationSystem.DTOS;
using BloodDonationSystem.Models;
using BloodDonationSystem.Services;
using BloodDonationSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BloodDonationSystem.Controllers
{
    public class BloodRequestController : Controller
    {

        private readonly IBloodTypeService _bloodTypeService;
        private readonly IBloodRequestService _bloodRequestService;
        private readonly IDonationService _donationService;
        private readonly IDonorService _donorService;
        private readonly IBloodCompatibilityService _bloodCompatibilityService;
        public BloodRequestController(IBloodTypeService bloodTypeService, IBloodRequestService bloodRequestService,  
            IDonationService donationService, IDonorService donorService, IBloodCompatibilityService bloodCompatibilityService)
        {
            _bloodTypeService = bloodTypeService;
            _bloodRequestService = bloodRequestService;
            _donationService = donationService;
            _donorService = donorService;
            _bloodCompatibilityService = bloodCompatibilityService;
        }

        private async Task<CreateBloodRequestDTO> BloodRequestManagementPageDTO()
        {
            var sourceData = await _bloodRequestService.GetAllBloodRequestWithBloodTypesAsync();

            var getAllBloodTypes = await _bloodTypeService.GetAllBloodTypeAsync();

            var viewModel = new CreateBloodRequestDTO
            {
                BloodTypes = getAllBloodTypes.Select(bt => new BloodTypeSelectionDTO
                {
                    BloodTypeId = bt.Id,
                    BloodTypeName = bt.BloodTypeName,
                    Quantity = 0
                }).ToList(),

                BloodRequests = sourceData.Select(req => new BloodRequestsDTO
                {
                    Id = req.Id,
                    BloodRequestDate = req.BloodRequestDate,
                    BloodTypeName = req.BloodType.BloodTypeName,
                    IsBloodRequestActive = req.IsActive,
                    IsBloodRequestApproved = req.isApproved,
                    QuantityRequested = req.Quantity,
                })
            };
            return viewModel;
        }


        public async Task<IActionResult> BloodRequestManagement()
        {


            var viewModel = await BloodRequestManagementPageDTO();

            return View("BloodRequestManagement", viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> CreateBloodRequest(CreateBloodRequestDTO bloodrequest)
        {
            var viewModel = new object();

            var selectedBloodType = bloodrequest.BloodTypes
                .FirstOrDefault(x => x.BloodTypeId == bloodrequest.SelectedBloodTypeId);

            if (selectedBloodType == null || selectedBloodType.Quantity <= 0)
            {
                 viewModel = await BloodRequestManagementPageDTO();

                ModelState.AddModelError("", "Please select a valid blood type and quantity.");
                return View("BloodRequestManagement", viewModel);
            }

            var typeIdToSave = bloodrequest.SelectedBloodTypeId;
            var quantityToSave = selectedBloodType.Quantity;

            BloodRequest newBloodRequest = new BloodRequest
            {
                BloodTypeId = typeIdToSave,
                isApproved = false,
                IsActive = false,
                BloodRequestDate = DateTime.Now,
                Quantity = quantityToSave,

            };
            //TODO : CREATE BLOOD REQUEEST
            await _bloodRequestService.CreateBloodRequestAsync(newBloodRequest);


             viewModel = await BloodRequestManagementPageDTO();

            return View("BloodRequestManagement", viewModel);

        }

        [HttpPost]

        public async Task<IActionResult> Activate(int id)
        {
            var success = await _bloodRequestService.ActivateBloodRequest(id);

            if (!success) return NotFound();

            var viewModel = await BloodRequestManagementPageDTO();

            return View("BloodRequestManagement", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> DeActivate(int id)
        {
            var success = await _bloodRequestService.DeActivateBloodRequest(id);

            if (!success) return NotFound();

            var viewModel = await BloodRequestManagementPageDTO();

            return View("BloodRequestManagement", viewModel);
        }

        public async Task<IActionResult> ApprovedBloodRequests(int id)
        {
            var userId = User.FindFirst("UserID")?.Value;

            var donor = await _donorService.GetDonorByUserIdAsync(int.Parse(userId));


            var approvedBloodRequestDTO =  await _bloodRequestService.GetAllApprovedBloodRequest(int.Parse(userId));

            var DidUserCompleteHisDonationTimeLimit = await _donationService.DidUserCompleteHisDonationTimeLimit(donor.Id);

            if (!DidUserCompleteHisDonationTimeLimit)
            {
                approvedBloodRequestDTO.DidUserCompleteHisDonationTimeLimit = DidUserCompleteHisDonationTimeLimit;
                ModelState.AddModelError("DonationTimeLimit", "You have recently donated blood, you can only donate every 60 days, please check the blood donation guide at home page for more information");
                return View("ApprovedBloodRequests", approvedBloodRequestDTO);
            }

            return View("ApprovedBloodRequests", approvedBloodRequestDTO);
        }

        public async Task<IActionResult> CreateDonationRequest(int BloodRequestId,string patientBloodType, DateOnly WhenUserWantToDonate)
        {
            var approvedBloodRequestDTO = new object();
            var userId =  User.FindFirst("UserID")?.Value;

            var donor = await _donorService.GetDonorByUserIdAsync(int.Parse(userId));

            if (WhenUserWantToDonate < DateOnly.FromDateTime(DateTime.Now.AddDays(-1)))
            {
                ModelState.AddModelError("WhenUserWantToDonate", "You must pick a future date to donate");

                approvedBloodRequestDTO = await _bloodRequestService.GetAllApprovedBloodRequest(int.Parse(userId));

                return View("ApprovedBloodRequests", approvedBloodRequestDTO);

            }

            bool canDonorDonate =  _bloodCompatibilityService.CanDonate(donor.BloodType.BloodTypeName, patientBloodType);

            if (!canDonorDonate)
            {

                ModelState.AddModelError("BloodIncompatible", "Your blood type is not compatible with patient blood type,please check a blood donation guide");

                approvedBloodRequestDTO = await _bloodRequestService.GetAllApprovedBloodRequest(int.Parse(userId));

                return View("ApprovedBloodRequests", approvedBloodRequestDTO);
            }

            var DonationRequest = new Donation
            {
                //TODO : FIX DONATION REQUEST CREATION

                DonorId = donor.Id,
                StatusId = 2,
                Quantity = 0,
                BloodRequestId = BloodRequestId,
                DonationSubmitDate = DateTime.Now
            };
            var createdDonationRequest = await _donationService.CreateDonationAsync(DonationRequest);

             approvedBloodRequestDTO = await _bloodRequestService.GetAllApprovedBloodRequest(int.Parse(userId));

            return View("ApprovedBloodRequests", approvedBloodRequestDTO);


        }

        public async Task<IActionResult> CancelDonation( int BloodRequestId)
        {

            var userId = User.FindFirst("UserID")?.Value;


            var donation = await _donationService.GetDonationByBloodRequestIdAsync(BloodRequestId);

            await _donationService.CancelDonation(donation);

            var approvedBloodRequestDTO = await _bloodRequestService.GetAllApprovedBloodRequest(int.Parse(userId));

            return View("ApprovedBloodRequests", approvedBloodRequestDTO);


        }

        public async Task<IActionResult> ReactivateDonation(int BloodRequestId)
        {

            var userId = User.FindFirst("UserID")?.Value;


            var donation = await _donationService.GetDonationByBloodRequestIdAsync(BloodRequestId);

            await _donationService.ReactivateDonation(donation);

            var approvedBloodRequestDTO = await _bloodRequestService.GetAllApprovedBloodRequest(int.Parse(userId));

            return View("ApprovedBloodRequests", approvedBloodRequestDTO);


        }



    }
}

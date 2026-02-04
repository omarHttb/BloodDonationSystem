using BloodDonationSystem.DTOS;
using BloodDonationSystem.Models;
using BloodDonationSystem.Services;
using BloodDonationSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonationSystem.Controllers
{
    public class BloodRequestController : Controller
    {

        private readonly IBloodTypeService _bloodTypeService;
        private readonly IBloodRequestService _bloodRequestService;
        private readonly IBloodRequestBloodTypeService _bloodRequestBloodTypeService;
        private readonly IDonationService _donationService;
        private readonly IDonorService _donorService;
        public BloodRequestController(IBloodTypeService bloodTypeService, IBloodRequestService bloodRequestService, IBloodRequestBloodTypeService bloodRequestBloodTypeService, 
            IDonationService donationService, IDonorService donorService)
        {
            _bloodTypeService = bloodTypeService;
            _bloodRequestService = bloodRequestService;
            _donationService = donationService;
            _bloodRequestBloodTypeService = bloodRequestBloodTypeService;
            _donorService = donorService;
        }


        private async Task<CreateBloodRequestDTO> GetAllBloodRequestAndBloodTypes()
        {
            var allBloodTypes = await _bloodTypeService.GetAllBloodTypeAsync();
            List<BloodRequest> allBloodRequests = await _bloodRequestService.GetAllBloodRequestWithBloodTypesAsync();
            var bloodRequestsAndBloodTypes = new CreateBloodRequestDTO();
            bloodRequestsAndBloodTypes.BloodTypes = allBloodTypes.Select(bt => new BloodTypeSelectionDTO
            {
                BloodTypeId = bt.Id,
                BloodTypeName = bt.BloodTypeName,
                IsSelected = false,
                Quantity = 0
            }).ToList();
            bloodRequestsAndBloodTypes.BloodRequests = allBloodRequests
            .SelectMany(br => br.bloodRequestBloodTypes, (br, bbt) => new BloodRequestsDTO
            {
                Id = br.Id, // The ID of the Blood Request (this will repeat for rows 1, 1, etc.)
                // Data from the Child (Specific Blood Type & Quantity)
                BloodTypeName = bbt.BloodType.BloodTypeName,
                QuantityRequested = bbt.Quantity, // The specific quantity for THIS blood type
                // Data from the Parent (Shared details)
                BloodRequestDate = br.BloodRequestDate,
                IsBloodRequestApproved = br.isApproved,
                IsBloodRequestActive = br.IsActive
            }).ToList();
            return bloodRequestsAndBloodTypes;
        }
        public async Task<IActionResult> BloodRequestManagement()
        {

            var bloodRequestsAndBloodTypes = await GetAllBloodRequestAndBloodTypes();

            return View("BloodRequestManagement", bloodRequestsAndBloodTypes);

        }
        [HttpPost]
        public async Task<IActionResult> CreateBloodRequest(CreateBloodRequestDTO bloodrequest)
        {
            var allBloodTypes = await _bloodTypeService.GetAllBloodTypeAsync();

            var AllBloodRequests = await _bloodRequestService.GetAllBloodRequestWithBloodTypesAsync();

            var bloodRequestsAndBloodTypes = new CreateBloodRequestDTO();

                        bloodRequestsAndBloodTypes.BloodRequests = AllBloodRequests
            .SelectMany(br => br.bloodRequestBloodTypes, (br, bbt) => new BloodRequestsDTO
            {
                Id = br.Id,   
                BloodTypeName = bbt.BloodType.BloodTypeName,
                QuantityRequested = bbt.Quantity,     
                BloodRequestDate = br.BloodRequestDate,
                IsBloodRequestApproved = br.isApproved,
                IsBloodRequestActive = br.IsActive
            }).ToList();
            

            bloodRequestsAndBloodTypes.BloodRequests = AllBloodRequests.Select(br => new BloodRequestsDTO
            {
                Id = br.Id,
                BloodTypeName = br.bloodRequestBloodTypes.Select(bbt => bbt.BloodType.BloodTypeName).FirstOrDefault(),
                QuantityRequested = br.bloodRequestBloodTypes.Select(bbt => bbt.Quantity).FirstOrDefault(),
                BloodRequestDate = br.BloodRequestDate,
                IsBloodRequestApproved = br.isApproved,
                IsBloodRequestActive = br.IsActive
            }).ToList();


            if (!bloodrequest.BloodTypes.Any(x => x.IsSelected))
            {

                ModelState.AddModelError("", "You must select at least one blood type.");

                return View("BloodRequest", bloodRequestsAndBloodTypes);
            }
            var request = new BloodRequest
            {
                BloodRequestDate = DateTime.Now,
                isApproved = false,
                IsActive = false
            };
           var RequestWithId =  await _bloodRequestService.CreateBloodRequestAsync(request);
            
            foreach (var bloodType in bloodrequest.BloodTypes)
            {
                if (bloodType.IsSelected && bloodType.Quantity > 0)
                {
                    var bloodRequestBloodType = new BloodRequestBloodType
                    {
                        BloodRequestId = RequestWithId.Id,
                        BloodTypeId = bloodType.BloodTypeId,
                        Quantity = bloodType.Quantity
                    };
                    await _bloodRequestBloodTypeService.CreateBloodRequestBloodTypeAsync(bloodRequestBloodType);
                }
            }


            return View("BloodRequestManagement", bloodRequestsAndBloodTypes);

        }

        [HttpPost]

        public async Task<IActionResult> Activate(int id)
        {
            var success = await _bloodRequestService.ActivateBloodRequest(id);

            if (!success) return NotFound();

            var bloodRequestsAndBloodTypes = await GetAllBloodRequestAndBloodTypes();

            return View("BloodRequestManagement", bloodRequestsAndBloodTypes);
        }

        [HttpPost]
        public async Task<IActionResult> DeActivate(int id)
        {
            var success = await _bloodRequestService.DeActivateBloodRequest(id);

            if (!success) return NotFound();

            var bloodRequestsAndBloodTypes = await GetAllBloodRequestAndBloodTypes();



            return View("BloodRequestManagement", bloodRequestsAndBloodTypes);
        }

        public async Task<IActionResult> ApprovedBloodRequests(int id)
        {
            var userId = User.FindFirst("UserID")?.Value;

            var approvedBloodRequestDTO =  await _bloodRequestService.GetAllApprovedBloodRequest(int.Parse(userId));

            return View("ApprovedBloodRequests", approvedBloodRequestDTO);
        }

        public async Task<IActionResult> CreateDonationRequest(int BloodRequestId)
        {

            var userId =  User.FindFirst("UserID")?.Value;

            var donor = await _donorService.GetDonorByUserIdAsync(int.Parse(userId));


            var DonationRequest = new Donation
            {
                DonorId = donor.Id,
                StatusId = 2,
                Quantity = 0,
                BloodRequestId = BloodRequestId,
                DonationSubmitDate = DateTime.Now
            };
            var createdDonationRequest = await _donationService.CreateDonationAsync(DonationRequest);

            var approvedBloodRequestDTO = await _bloodRequestService.GetAllApprovedBloodRequest(int.Parse(userId));

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

        public async Task<IActionResult> RejectDonation(int BloodRequestId)
        {

            var userId = User.FindFirst("UserID")?.Value;


            var donation = await _donationService.GetDonationByBloodRequestIdAsync(BloodRequestId);

            await _donationService.RejectDonation(donation);

            var approvedBloodRequestDTO = await _bloodRequestService.GetAllApprovedBloodRequest(int.Parse(userId));

            return View("ApprovedBloodRequests", approvedBloodRequestDTO);


        }

        public async Task<IActionResult> CompleteDonation(int BloodRequestId)
        {

            var userId = User.FindFirst("UserID")?.Value;


            var donation = await _donationService.GetDonationByBloodRequestIdAsync(BloodRequestId);

            await _donationService.CompleteDonation(donation);

            var approvedBloodRequestDTO = await _bloodRequestService.GetAllApprovedBloodRequest(int.Parse(userId));


            return View("ApprovedBloodRequests", approvedBloodRequestDTO);


        }

        public async Task<IActionResult> ApproveDonation(int BloodRequestId)
        {

            var userId = User.FindFirst("UserID")?.Value;


            var donation = await _donationService.GetDonationByBloodRequestIdAsync(BloodRequestId);

            await _donationService.ApproveDonation(donation);

            var approvedBloodRequestDTO = await _bloodRequestService.GetAllApprovedBloodRequest(int.Parse(userId));



            return View("ApprovedBloodRequests", approvedBloodRequestDTO);


        }

    }
}

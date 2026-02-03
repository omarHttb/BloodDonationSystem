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
        public BloodRequestController(IBloodTypeService bloodTypeService, IBloodRequestService bloodRequestService, IBloodRequestBloodTypeService bloodRequestBloodTypeService)
        {
            _bloodTypeService = bloodTypeService;
            _bloodRequestService = bloodRequestService;
            _bloodRequestBloodTypeService = bloodRequestBloodTypeService;
        }

        public async Task<IActionResult> BloodRequest()
        {
            var allBloodTypes = await _bloodTypeService.GetAllBloodTypeAsync();

            List<BloodRequest> allBloodRequests = await _bloodRequestService.GetAllBloodRequestAsync();

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

            return View(bloodRequestsAndBloodTypes);

        }
        [HttpPost]
        public async Task<IActionResult> CreateBloodRequest(CreateBloodRequestDTO bloodrequest)
        {
            var allBloodTypes = await _bloodTypeService.GetAllBloodTypeAsync();

            var AllBloodRequests = await _bloodRequestService.GetAllBloodRequestAsync();

            var bloodRequestsAndBloodTypes = new CreateBloodRequestDTO();

                        bloodRequestsAndBloodTypes.BloodRequests = AllBloodRequests
            // SelectMany flattens the list: 
            // The first argument selects the collection (bloodRequestBloodTypes)
            // The second argument (br, bbt) gives you access to both the Parent (br) and the specific Child (bbt)
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


            return View("BloodRequest", bloodRequestsAndBloodTypes);

        }

    }
}

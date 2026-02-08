using Microsoft.EntityFrameworkCore;
using BloodDonationSystem.Models;
using BloodDonationSystem.Data;
using BloodDonationSystem.Services.Interfaces;
using BloodDonationSystem.DTOS;

namespace BloodDonationSystem.Services
{
    public class BloodRequestService : IBloodRequestService
    {
        private readonly AppDbContext _context;

        public BloodRequestService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<BloodRequest>> GetAllBloodRequestWithBloodTypesAsync()
        {
            return await _context.BloodRequests.Include(b => b.BloodType).ToListAsync();
                    
        }

        public async Task<BloodRequest> GetBloodRequestByIdAsync(int id)
        {
            return await _context.BloodRequests.FindAsync(id);
        }

        public async Task<BloodRequest> CreateBloodRequestAsync(BloodRequest BloodRequest)
        {
            _context.BloodRequests.Add(BloodRequest);
            await _context.SaveChangesAsync();
            return BloodRequest;
        }


        public async Task<BloodRequest> UpdateBloodRequestAsync(int id, BloodRequest BloodRequest)
        {
            var existingBloodRequest = await _context.BloodRequests.FindAsync(id);
            if (existingBloodRequest == null) return null;

            existingBloodRequest.BloodRequestDate = BloodRequest.BloodRequestDate;
            existingBloodRequest.isApproved = BloodRequest.isApproved;
            existingBloodRequest.IsActive = BloodRequest.IsActive;


            await _context.SaveChangesAsync();
            return existingBloodRequest;
        }


        public async Task<List<BloodRequest>> GetAllBloodRequestAsnyc()
        {
            return await _context.BloodRequests.ToListAsync();
        }

        public async Task<bool> ApproveBloodRequest(int id)
        {

            var request = await _context.BloodRequests.FindAsync(id);
            if (request == null) return false;

            request.isApproved = true;
            request.IsActive = false;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DisApproveBloodRequest(int id)
        {
            var request = await _context.BloodRequests.FindAsync(id);
            if (request == null) return false;

            request.isApproved = false;
            request.IsActive = false;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ActivateBloodRequest(int id)
        {
            var request = await _context.BloodRequests.FindAsync(id);
            if (request == null) return false;

            request.IsActive = true;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeActivateBloodRequest(int id)
        {
            var request = await _context.BloodRequests.FindAsync(id);
            if (request == null) return false;


            request.IsActive = false;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<BloodRequestToSubmitDTO> GetAllApprovedBloodRequest(int UserId)
        {
            var response = new BloodRequestToSubmitDTO();

            var donorRecord = await _context.Donors
                                            .Include(d => d.BloodType)
                                            .FirstOrDefaultAsync(x => x.UserId == UserId);
            Donation? lastDonation = null;
            bool isDonor = donorRecord != null;
            bool doesUserHaveBloodType = donorRecord != null && donorRecord.BloodTypeId != null;
            bool isDonorAvailable = donorRecord != null && donorRecord.IsAvailable;
            bool isBusyWithDonation = false;
            var requestList = new List<ApprovedBloodRequestDTO>();


            if (isDonor)
            {
                lastDonation = await _context.Donations
                                             .Where(d => d.DonorId == donorRecord.Id)
                                             .OrderByDescending(d => d.DonationSubmitDate)
                                             .FirstOrDefaultAsync();

                if (lastDonation != null && (lastDonation.StatusId == 2 || lastDonation.StatusId == 5))
                {
                    isBusyWithDonation = true;
                }
            }

            var rawRequests = await _context.BloodRequests
                                            .Include(br => br.BloodType)
                                            .Where(br => br.isApproved && br.IsActive && br.Quantity > 0)
                                            .OrderByDescending(br => br.BloodRequestDate)
                                            .ToListAsync();


            foreach (var req in rawRequests)
            {
                string rowStatus = "Available For Donation!";

                if (!isDonor)
                {
                    rowStatus = "You are not a donor";
                }
                else if (!doesUserHaveBloodType)
                {
                    rowStatus = "Request the hospital to asign your blood type";
                }
                else if (isBusyWithDonation)
                {
                    if (lastDonation != null && lastDonation.BloodRequestId == req.Id)
                    {
                        if (lastDonation.StatusId == 2) rowStatus = "Pending";
                        else if (lastDonation.StatusId == 5) rowStatus = "Approved";
                    }
                    else
                    {
                        rowStatus = "Existing Donation Pending";
                    }
                }
                else if (!isDonorAvailable)
                {
                    rowStatus = "You are marked Unavailable";
                }

                requestList.Add(new ApprovedBloodRequestDTO
                {
                    Id = req.Id,
                    BloodTypeName = req.BloodType.BloodTypeName,
                    QuantityRequested = req.Quantity,
                    BloodRequestDate = req.BloodRequestDate,
                    IsBloodRequestActive = req.IsActive,
                    Status = rowStatus
                });
            }

            response.ApprovedBloodRequests = requestList;
            response.isUserADonor = isDonor;
            response.DoesUserHaveBloodType = doesUserHaveBloodType;
            response.IsDonorAvailableToDonate = isDonorAvailable;

            return response;
        }

        public  async Task<int> TotalNumberOfBloodRequests()
        {
           return await _context.BloodRequests.CountAsync();
        }
    }
}
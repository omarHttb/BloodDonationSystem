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
            return await _context.BloodRequests
                    .Include(br => br.bloodRequestBloodTypes)      // Pulls the link table
                        .ThenInclude(bbt => bbt.BloodType)        // Pulls the actual Type (A+, O+, etc)
                    .ToListAsync();
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

        public async Task<bool> DeleteBloodRequestAsync(int id)
        {
            var BloodRequest = await _context.BloodRequests.FindAsync(id);
            if (BloodRequest == null) return false;

            _context.BloodRequests.Remove(BloodRequest);
            await _context.SaveChangesAsync();
            return true;
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

            // ❌ DO NOT call Update()
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
            var donorRecord = await _context.Donors.FirstOrDefaultAsync(x => x.UserId == UserId);
            bool isDonor = donorRecord != null;
            bool doesUserHaveBloodType = donorRecord.BloodTypeId != null;
            string donationStatusString = "No Donations Yet";

            var lastDonation = await _context.Donations
                .Include(d => d.Status)
                .Where(d => d.DonorId == donorRecord.Id)
                .OrderByDescending(d => d.DonationDate)
                .FirstOrDefaultAsync();
            if (lastDonation == null)
            {
                donationStatusString = "you did not donate";
            }
            else 
            {         
                donationStatusString = lastDonation.Status.StatusName;
            }


            var approvedRequests = await _context.BloodRequests
                    .Where(br => br.isApproved)
                    .Include(br => br.bloodRequestBloodTypes)
                        .ThenInclude(bbt => bbt.BloodType)
                    .SelectMany(br => br.bloodRequestBloodTypes, (br, bbt) => new ApprovedBloodRequestDTO
                    {
                        Id = br.Id,
                        BloodTypeName = bbt.BloodType.BloodTypeName,
                        QuantityRequested = bbt.Quantity,
                        BloodRequestDate = br.BloodRequestDate,
                        IsBloodRequestActive = br.IsActive,
                        Status = donationStatusString
                    }).ToListAsync();

            var BloodRequestToSubmit = new BloodRequestToSubmitDTO
            {
                isUserADonor = isDonor,
                DoesUserHaveBloodType = doesUserHaveBloodType,
                ApprovedBloodRequests = approvedRequests,
                IsDonorAvailableToDonate = donorRecord.IsAvailable
            };

            return BloodRequestToSubmit;
        }
    }
}
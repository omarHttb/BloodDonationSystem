using BloodDonationSystem.Data;
using BloodDonationSystem.DTOS;
using BloodDonationSystem.Models;
using BloodDonationSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BloodDonationSystem.Services
{
    public class DonationService : IDonationService
    {
        private readonly AppDbContext _context;

        public DonationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Donation>> GetAllDonationAsync()
        {
            return await _context.Donations.ToListAsync();
        }

        public async Task<Donation> GetDonationByIdAsync(int id)
        {
            return await _context.Donations.FindAsync(id);
        }

        public async Task<Donation> CreateDonationAsync(Donation Donation)
        {
           var lastDonation = await _context.Donations
                              .Where(d => d.DonorId == Donation.DonorId && d.BloodRequestId == Donation.BloodRequestId)
                              .OrderByDescending(d => d.DonationSubmitDate)
                              .FirstOrDefaultAsync();

            if (lastDonation != null)
            {
                if (lastDonation.StatusId == 3)
                {
                    lastDonation.StatusId = 2; // Reset to Pending
                    lastDonation.DonationSubmitDate = DateTime.Now;
                    lastDonation.BloodRequestId = Donation.BloodRequestId;
                    await _context.SaveChangesAsync();
                    return Donation;

                }
            }
            
                _context.Donations.Add(Donation);
                await _context.SaveChangesAsync();
                return Donation;
            
        }

        public async Task<Donation> UpdateDonationAsync(int id, Donation Donation)
        {
            var existingDonation = await _context.Donations.FindAsync(id);
            if (existingDonation == null) return null;

            existingDonation.DonorId = Donation.DonorId;
            existingDonation.DonationDate = Donation.DonationDate;
            existingDonation.Quantity = Donation.Quantity;
            existingDonation.Status = Donation.Status;
            existingDonation.BloodRequestId = Donation.BloodRequestId;

            await _context.SaveChangesAsync();
            return existingDonation;
        }

        public async Task<bool> DeleteDonationAsync(int id)
        {
            var Donation = await _context.Donations.FindAsync(id);
            if (Donation == null) return false;

            _context.Donations.Remove(Donation);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> TotalNumberOfDonations()
        {
           return await _context.Donations.CountAsync();
        }

        public async Task<Donation> GetDonationByBloodRequestIdAsync(int bloodRequestId)
        {
            return await _context.Donations.FirstOrDefaultAsync(d => d.BloodRequestId == bloodRequestId);
        }

        public async Task<bool> CancelDonation(Donation donation)
        {
            donation.StatusId = 3;

           await _context.SaveChangesAsync();

            return true;

        }

        public async Task<bool> CompleteDonation(Donation donation)
        {
            var bloodRequest = _context.BloodRequests.FindAsync(donation.BloodRequestId).Result;

            bloodRequest.Quantity -= donation.Quantity;

            donation.DonationDate = DateTime.Now;

            donation.StatusId = 1;

            await  _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RejectDonation(Donation donation)
        {
            donation.StatusId = 4;

           await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ApproveDonation(Donation donation)
        {
            donation.StatusId = 5;

           await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ReactivateDonation(Donation donation)
        {
            donation.StatusId = 2;

            await  _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<DonationsWithBloodRequestAndDonorDTO>> GetAllDonationsWithBloodRequestAndDonor()
        {
            // Use .Select() to map the entity properties directly to your DTO constructor
            var donations = await _context.Donations
                .Include(d => d.BloodRequest).ThenInclude(br => br.BloodType)
                .Include(d => d.Donor).ThenInclude(donor => donor.BloodType)
                .Include(d => d.Status)
               .Select(d => new DonationsWithBloodRequestAndDonorDTO
               {
                   DonationId = d.Id,
                   BloodRequestId = d.BloodRequestId,
                   RequestedBloodType = d.BloodRequest.BloodType.BloodTypeName,
                   DonatorName = d.Donor.User.Name,
                   DonatorBloodType = d.Donor.BloodType.BloodTypeName,
                   DonationStatus = d.Status.StatusName,
                   DonationQuantity = d.Quantity,
                   DonationDateSubmitted = d.DonationSubmitDate,
                   IsDonationActive = d.BloodRequest.IsActive,
                   QuantityRequested = d.BloodRequest.Quantity,
                   DonationDate = d.DonationDate,
                   BloodRequestDate = d.BloodRequest.BloodRequestDate,
                   IsDonatorAvailable = d.Donor.IsAvailable,
               }).ToListAsync();

            return donations;
        }

        public async Task<bool> DidUserCompleteHisDonationTimeLimit(int donorId)
        {
            var lastDonation = await _context.Donations
                      .Where(d => d.DonorId == donorId )
                      .OrderByDescending(d => d.DonationSubmitDate)
                      .FirstOrDefaultAsync();
            if (lastDonation != null
                && lastDonation.DonationDate.HasValue
                && (DateTime.Now - lastDonation.DonationDate.Value).TotalDays < 60)
            {
                return false;
            }

            return true;
        }
    }
}
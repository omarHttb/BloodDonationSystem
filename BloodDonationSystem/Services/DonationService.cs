using Microsoft.EntityFrameworkCore;
using BloodDonationSystem.Models;
using BloodDonationSystem.Data;
using BloodDonationSystem.Services.Interfaces;

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

        public Task<int> TotalNumberOfDonations()
        {
           return _context.Donations.CountAsync();
        }
    }
}
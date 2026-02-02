using Microsoft.EntityFrameworkCore;
using BloodDonationSystem.Models;
using BloodDonationSystem.Data;
using BloodDonationSystem.Services.Interfaces;

namespace BloodDonationSystem.Services
{
    public class DonorService : IDonorService
    {
        private readonly AppDbContext _context;

        public DonorService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Donor>> GetAllDonorAsync()
        {
            return await _context.Donors.ToListAsync();
        }

        public async Task<Donor> GetDonorByIdAsync(int id)
        {
            return await _context.Donors.FindAsync(id);
        }

        public async Task<Donor> CreateDonorAsync(Donor Donor)
        {
            _context.Donors.Add(Donor);
            await _context.SaveChangesAsync();
            return Donor;
        }

        public async Task<Donor> UpdateDonorAsync(int id, Donor Donor)
        {
            var existingDonor = await _context.Donors.FindAsync(id);
            if (existingDonor == null) return null;

            existingDonor.UserId = Donor.UserId;
            existingDonor.BloodTypeId = Donor.BloodTypeId;
            existingDonor.IsAvailable = Donor.IsAvailable;


            await _context.SaveChangesAsync();
            return existingDonor;
        }

        public async Task<bool> DeleteDonorAsync(int id)
        {
            var Donor = await _context.Donors.FindAsync(id);
            if (Donor == null) return false;

            _context.Donors.Remove(Donor);
            await _context.SaveChangesAsync();
            return true;
        }

        public Task<int> TotalNumberOfDonors()
        {
            return _context.Donors.CountAsync();    
        }

        public Task<int> TotalBloodAvailableByType(int bloodTypeId)
        {
            return _context.Donors
                .Where(d => d.BloodTypeId == bloodTypeId && d.IsAvailable)
                .CountAsync();
        }
    }
}
using Microsoft.EntityFrameworkCore;
using BloodDonationSystem.Models;
using BloodDonationSystem.Data;
using BloodDonationSystem.Services.Interfaces;
using BloodDonationSystem.DTOS;

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

        public Task<Donor> GetDonorByUserIdAsync(int userId)
        {
            var Donor = _context.Donors
                .FirstOrDefaultAsync(d => d.UserId == userId);

            if(Donor == null)
            {
                return null;
            }

            return Donor;
        }

        public async Task<DonorsAndBloodTypesDTO> GetDonorManagementData()
        {
            // 1. Get all donors (fast, single query)
            var donorList = await _context.Donors
                .Include(d => d.User)
                .Include(d => d.BloodType)
                .Select(d => new DonorDTO
                {
                    DonorId = d.Id,
                    isAvailable = d.IsAvailable,
                    DonorName = d.User.Name,
                    BloodType = d.BloodType.BloodTypeName,
                    // DO NOT fetch the whole bloodTypes list here
                })
                .ToListAsync();

            // 2. Get all blood types (fast, single query)
            var bloodTypeList = await _context.BloodType.ToListAsync();

            // 3. Combine them into your wrapper DTO
            return new DonorsAndBloodTypesDTO
            {
                Donors = donorList,
                bloodTypes = bloodTypeList
            };
        }

        public Task<bool> UpdateDonorBloodType(int donorId, int bloodTypeId)
        {
            _context.Donors
                .Where(d => d.Id == donorId)
                .ExecuteUpdateAsync(d => d.SetProperty(donor => donor.BloodTypeId, bloodTypeId));
            return Task.FromResult(true);
        }

        public async Task<bool> UpdateDonorToAvailable(Donor donor)
        {
            donor.IsAvailable = true;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateDonorToUnAvailable(Donor donor)
        {
            donor.IsAvailable = false;

            await _context.SaveChangesAsync();

            return true;

        }
    }
}
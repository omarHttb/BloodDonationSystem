using Microsoft.EntityFrameworkCore;
using BloodDonationSystem.Models;
using BloodDonationSystem.Data;
using BloodDonationSystem.Services.Interfaces;

namespace BloodDonationSystem.Services
{
    public class BloodTypeService : IBloodTypeService
    {
        private readonly AppDbContext _context;

        public BloodTypeService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<BloodType>> GetAllBloodTypeAsync()
        {
            return await _context.BloodType.ToListAsync();
        }

        public async Task<BloodType> GetBloodTypeByIdAsync(int id)
        {
            return await _context.BloodType.FindAsync(id);
        }

        public async Task<BloodType> CreateBloodTypeAsync(BloodType BloodType)
        {
            _context.BloodType.Add(BloodType);
            await _context.SaveChangesAsync();
            return BloodType;
        }

        public async Task<BloodType> UpdateBloodTypeAsync(int id, BloodType BloodType)
        {
            var existingBloodType = await _context.BloodType.FindAsync(id);
            if (existingBloodType == null) return null;

            existingBloodType.Id = BloodType.Id;

            await _context.SaveChangesAsync();
            return existingBloodType;
        }

        public async Task<bool> DeleteBloodTypeAsync(int id)
        {
            var BloodType = await _context.BloodTypes.FindAsync(id);
            if (BloodType == null) return false;

            _context.BloodTypes.Remove(BloodType);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
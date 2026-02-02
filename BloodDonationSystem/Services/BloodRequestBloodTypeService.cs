using Microsoft.EntityFrameworkCore;
using BloodDonationSystem.Models;
using BloodDonationSystem.Data;
using BloodDonationSystem.Services.Interfaces;

namespace BloodDonationSystem.Services
{
    public class BloodRequestBloodTypeService : IBloodRequestBloodTypeService
    {
        private readonly AppDbContext _context;

        public BloodRequestBloodTypeService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<BloodRequestBloodType>> GetAllBloodRequestBloodTypeAsync()
        {
            return await _context.BloodRequestBloodTypes.ToListAsync();
        }

        public async Task<BloodRequestBloodType> GetBloodRequestBloodTypeByIdAsync(int id)
        {

            return await _context.BloodRequestBloodTypes.FindAsync(id);
        }

        public async Task<BloodRequestBloodType> CreateBloodRequestBloodTypeAsync(BloodRequestBloodType BloodRequestBloodType)
        {
            _context.BloodRequestBloodTypes.Add(BloodRequestBloodType);
            await _context.SaveChangesAsync();
            return BloodRequestBloodType;
        }

        public async Task<BloodRequestBloodType> UpdateBloodRequestBloodTypeAsync(int id, BloodRequestBloodType BloodRequestBloodType)
        {
            var existingBloodRequestBloodType = await _context.BloodRequestBloodTypes.FindAsync(id);
            if (existingBloodRequestBloodType == null) return null;

            existingBloodRequestBloodType.BloodTypeId = BloodRequestBloodType.BloodTypeId;
            existingBloodRequestBloodType.BloodRequestId = BloodRequestBloodType.BloodRequestId;
            existingBloodRequestBloodType.Quantity = BloodRequestBloodType.Quantity;


            await _context.SaveChangesAsync();
            return existingBloodRequestBloodType;
        }

        public async Task<bool> DeleteBloodRequestBloodTypeAsync(int id)
        {
            var BloodRequestBloodType = await _context.BloodRequestBloodTypes.FindAsync(id);
            if (BloodRequestBloodType == null) return false;

            _context.BloodRequestBloodTypes.Remove(BloodRequestBloodType);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
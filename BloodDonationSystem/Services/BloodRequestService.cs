using Microsoft.EntityFrameworkCore;
using BloodDonationSystem.Models;
using BloodDonationSystem.Data;
using BloodDonationSystem.Services.Interfaces;

namespace BloodDonationSystem.Services
{
    public class BloodRequestService : IBloodRequestService
    {
        private readonly AppDbContext _context;

        public BloodRequestService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<BloodRequest>> GetAllBloodRequestAsync()
        {
            // Simple SELECT * FROM BloodRequests
            return await _context.BloodRequests.ToListAsync();
        }

        public async Task<BloodRequest> GetBloodRequestByIdAsync(int id)
        {
            // FindAsync is the most efficient for primary key lookups
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

            existingBloodRequest.Id = BloodRequest.Id;

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
    }
}
using Microsoft.EntityFrameworkCore;
using BloodDonationSystem.Models;
using BloodDonationSystem.Data;
using BloodDonationSystem.Services.Interfaces;

namespace BloodDonationSystem.Services
{
    public class StatusService : IStatusService
    {
        private readonly AppDbContext _context;

        public StatusService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Status>> GetAllStatusAsync()
        {
            return await _context.Status.ToListAsync();
        }

        public async Task<Status> GetStatusByIdAsync(int id)
        {
            // FindAsync is the most efficient for primary key lookups
            return await _context.Status.FindAsync(id);
        }

        public async Task<Status> CreateStatusAsync(Status Status)
        {
            _context.Status.Add(Status);
            await _context.SaveChangesAsync();
            return Status;
        }

        public async Task<Status> UpdateStatusAsync(int id, Status Status)
        {
            var existingStatus = await _context.Status.FindAsync(id);
            if (existingStatus == null) return null;

            // Updating properties manually
            existingStatus.Id = Status.Id;
            // Map any other properties from your Status model here
            // existingStatus.Name = Status.Name; 

            await _context.SaveChangesAsync();
            return existingStatus;
        }

        public async Task<bool> DeleteStatusAsync(int id)
        {
            var Status = await _context.Statuss.FindAsync(id);
            if (Status == null) return false;

            _context.Statuss.Remove(Status);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
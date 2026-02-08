using BloodDonationSystem.Data;
using BloodDonationSystem.Models;
using BloodDonationSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BloodDonationSystem.Services
{
    public class BloodBankHistoryService : IBloodBankHistoryService
    {
        private readonly AppDbContext _context;

        public BloodBankHistoryService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<bool> CreateBloodBankHistoryAsync(BloodBankHistory bloodBankHistory)
        {

          await  _context.BloodBankHistory.AddAsync(bloodBankHistory);
          _context.SaveChanges();
          return true;


        }

        public async Task<List<BloodBankHistory>> GetAllBloodBankHistoryAsync()
        {
            return await _context.BloodBankHistory.Include(b => b.BloodBank).ThenInclude(b=> b.BloodType).ToListAsync();   
        }

        public async Task<BloodBankHistory> GetBloodBankHistoryByIdAsync(int id)
        {
            var bloodBankHistory = await _context.BloodBankHistory.FindAsync(id);

            if (bloodBankHistory == null)
            {
                return new BloodBankHistory();
            }
            return bloodBankHistory;
        }
    }
}

using BloodDonationSystem.Data;
using BloodDonationSystem.Models;
using BloodDonationSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BloodDonationSystem.Services
{
    public class BloodBankService : IBloodBankService
    {
        private readonly AppDbContext _context;
        private readonly IBloodBankHistoryService _bloodBankHistoryService;

        public BloodBankService(AppDbContext context ,IBloodBankHistoryService bloodBankHistoryService)
        {
            _bloodBankHistoryService = bloodBankHistoryService;
            _context = context;
        }
        public async Task<bool> AddToBloodBank(int BloodBankId, int quantity)
        {
            var bloodBank = await _context.BloodBanks.FindAsync(BloodBankId);
            
            if (bloodBank == null)
            {
                return false;
            }
            bloodBank.quantity += quantity;
            _context.SaveChanges();

            return false;
        }

        public async Task<List<BloodBank>> GetAllBloodBanksAsync()
        {
            return await _context.BloodBanks.Include(b => b.BloodType).ToListAsync();
        }

        public async Task<BloodBank> GetBloodBankByIdAsync(int id)
        {
            var bloodbank = await _context.BloodBanks.FindAsync(id);
            if (bloodbank == null)
            {
                return new BloodBank();
            }

            return bloodbank;
        }

        public async Task<BloodBank> GetBloodBankFromBloodTypeIdAsync(int BloodTypeId)
        {
            var bloodbank = await _context.BloodBanks.Where(b => b.BloodTypeId == BloodTypeId).FirstOrDefaultAsync();
            if (bloodbank == null)
            {
                return new BloodBank();
            }

            return bloodbank;
        }

        public async Task<bool> SubtractFromBloodBank(int BloodBankId, int quantity)
        {
            var bloodBank = await _context.BloodBanks.FindAsync(BloodBankId)
    ;
            if (bloodBank == null)
            {
                return false;
            }
            if(bloodBank.quantity < quantity)
            {
                return false;
            }

          await  _bloodBankHistoryService.CreateBloodBankHistoryAsync(new BloodBankHistory
            {
                BloodBankId = bloodBank.Id,
                TransactionDate = DateTime.Now,
                QuantityTransaction = -quantity,
                IsBloodAdded = false
            });

            bloodBank.quantity -= quantity;
            _context.SaveChanges();

            return true;

        }

 
    }
}

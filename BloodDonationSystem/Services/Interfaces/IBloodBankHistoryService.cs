using BloodDonationSystem.Models;

namespace BloodDonationSystem.Services.Interfaces
{
    public interface IBloodBankHistoryService
    {

        Task<List<BloodBankHistory>> GetAllBloodBankHistoryAsync();

        Task<BloodBankHistory> GetBloodBankHistoryByIdAsync(int id);

        Task<bool> CreateBloodBankHistoryAsync(BloodBankHistory bloodBankHistory);

    }
}

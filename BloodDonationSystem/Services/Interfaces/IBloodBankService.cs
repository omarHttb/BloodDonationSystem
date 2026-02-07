using BloodDonationSystem.Models;

namespace BloodDonationSystem.Services.Interfaces
{
    public interface IBloodBankService
    {
        Task<List<BloodBank>> GetAllBloodBanksAsync();

        Task<BloodBank> GetBloodBankByIdAsync(int id);

        Task<BloodBank> GetBloodBankFromBloodTypeIdAsync(int BloodTypeId);

        Task<bool> AddToBloodBank (int BloodBankId, int quantity);

        Task<bool> SubtractFromBloodBank(int BloodBankId, int quantity);    
    }
}

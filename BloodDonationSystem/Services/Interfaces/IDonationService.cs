using BloodDonationSystem.Models;

namespace BloodDonationSystem.Services.Interfaces

{
    public interface IDonationService 
    {
        Task<Donation> GetDonationByIdAsync(int id);
        Task<List<Donation>> GetAllDonationAsync();
        Task<Donation> CreateDonationAsync(Donation donation);
        Task<Donation> UpdateDonationAsync(int id, Donation donation);
        Task<bool> DeleteDonationAsync(int id);
    }
}
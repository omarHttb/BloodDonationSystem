using BloodDonationSystem.DTOS;
using BloodDonationSystem.Models;

namespace BloodDonationSystem.Services.Interfaces

{
    public interface IDonationService 
    {
        Task<Donation> GetDonationByIdAsync(int id);

        Task<List<DonationsWithBloodRequestAndDonorDTO>> GetAllDonationsWithBloodRequestAndDonor();

        Task<Donation> GetDonationByBloodRequestIdAsync(int bloodRequestId);
        Task<bool> CancelDonation(Donation donation);
        Task<bool> CompleteDonation(Donation donation);
        Task<bool> RejectDonation(Donation donation);
        Task<bool> ApproveDonation(Donation donation);
        Task<bool> ReactivateDonation(Donation donation);

        Task<List<Donation>> GetAllDonationAsync();
        Task<Donation> CreateDonationAsync(Donation donation);
        Task<Donation> UpdateDonationAsync(int id, Donation donation);
        Task<bool> DeleteDonationAsync(int id);

        Task<int> TotalNumberOfDonations();

    }
}
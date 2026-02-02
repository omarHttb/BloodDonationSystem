using BloodDonationSystem.Models;

namespace BloodDonationSystem.Services.Interfaces

{
    public interface IDonorService 
    {
        Task<Donor> GetDonorByIdAsync(int id);
        Task<List<Donor>> GetAllDonorAsync();
        Task<Donor> CreateDonorAsync(Donor donor);
        Task<Donor> UpdateDonorAsync(int id, Donor donor);
        Task<bool> DeleteDonorAsync(int id);
        Task<int> TotalNumberOfDonors();
        Task<int> TotalBloodAvailableByType(int bloodTypeId);
    }
}
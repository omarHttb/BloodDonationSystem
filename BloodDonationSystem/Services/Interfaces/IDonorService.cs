using BloodDonationSystem.DTOS;
using BloodDonationSystem.Models;

namespace BloodDonationSystem.Services.Interfaces

{
    public interface IDonorService 
    {

        Task<Donor> GetDonorByUserIdAsync(int userId);

        Task<bool> UpdateDonorToAvailable(Donor donor);
        Task<bool> UpdateDonorToUnAvailable(Donor donor);


        Task<bool> UpdateDonorBloodType(int donorId, int bloodTypeId);
        Task <DonorsAndBloodTypesDTO> GetDonorManagementData();
        Task<Donor> GetDonorByIdAsync(int id);
        Task<List<Donor>> GetAllDonorAsync();
        Task<Donor> CreateDonorAsync(Donor donor);
        Task<Donor> UpdateDonorAsync(int id, Donor donor);
        Task<bool> DeleteDonorAsync(int id);
        Task<int> TotalNumberOfDonors();
        Task<int> TotalBloodAvailableByType(int bloodTypeId);
    }
}
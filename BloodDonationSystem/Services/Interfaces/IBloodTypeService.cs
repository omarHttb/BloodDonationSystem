using BloodDonationSystem.Models;

namespace BloodDonationSystem.Services.Interfaces

{
    public interface IBloodTypeService 
    {
        Task<BloodType> GetBloodTypeByIdAsync(int id);
        Task<List<BloodType>> GetAllBloodTypeAsync();
        Task<BloodType> CreateBloodTypeAsync(BloodType bloodType);
        Task<BloodType> UpdateBloodTypeAsync(int id, BloodType bloodType);
        Task<bool> DeleteBloodTypeAsync(int id);
    }
}
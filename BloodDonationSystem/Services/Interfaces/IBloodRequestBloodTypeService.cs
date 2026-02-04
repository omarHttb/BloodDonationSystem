using BloodDonationSystem.Models;

namespace BloodDonationSystem.Services.Interfaces

{
    public interface IBloodRequestBloodTypeService 
    {
        Task<BloodRequestBloodType> GetBloodRequestBloodTypeByIdAsync(int id);
        Task <List<BloodRequestBloodType>> GetAllBloodRequestBloodTypeAsync();
        Task<bool> CreateBloodRequestBloodTypeAsync(BloodRequestBloodType bloodRequestBloodType);
        Task<BloodRequestBloodType> UpdateBloodRequestBloodTypeAsync(int id, BloodRequestBloodType bloodRequestBloodType);
        Task<bool> DeleteBloodRequestBloodTypeAsync(int id);

    }
}
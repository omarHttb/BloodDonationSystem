using BloodDonationSystem.Models;

namespace BloodDonationSystem.Services.Interfaces

{
    public interface IUserRoleService 
    {
        Task<UserRole> GetUserRoleByIdAsync(int id);
        Task<List<UserRole>> GetAllUserRoleAsync();
        Task<UserRole> CreateUserRoleAsync(UserRole userRole);
        Task<UserRole> UpdateUserRoleAsync(int id, UserRole userRole);
        Task<bool> DeleteUserRoleAsync(int id);
    }
}
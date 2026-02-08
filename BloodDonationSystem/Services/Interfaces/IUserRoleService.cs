using BloodDonationSystem.Models;

namespace BloodDonationSystem.Services.Interfaces

{
    public interface IUserRoleService 
    {
        Task<UserRole> GetUserRoleByIdAsync(int id);
        Task<List<UserRole>> GetAllUserRoleAsync();
        Task<UserRole> CreateUserRoleAsync(UserRole userRole);
        Task<bool> UpdateUserRolesAsync(int userId, List<int> roleIds);
        Task<bool> DeleteUserRoleAsync(int id);
    }
}
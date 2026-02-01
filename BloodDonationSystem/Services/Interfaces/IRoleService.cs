using BloodDonationSystem.Models;

namespace BloodDonationSystem.Services.Interfaces

{
    public interface IRoleService 
    {
        Task<Role> GetRoleByIdAsync(int id);
        Task<List<Role>> GetAllRoleAsync();
        Task<Role> CreateRoleAsync(Role role);
        Task<Role> UpdateRoleAsync(int id, Role role);
        Task<bool> DeleteRoleAsync(int id);
    }
}
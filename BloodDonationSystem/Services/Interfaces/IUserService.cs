using BloodDonationSystem.DTOS;
using BloodDonationSystem.Models;

namespace BloodDonationSystem.Services.Interfaces

{
    public interface IUserService 
    {
        Task<User> GetUserByIdAsync(int id);
        Task<List<User>> GetAllUserAsync();
        Task<int> RegisterUserAsync(User user);
        Task<User> UpdateUserAsync(int id, User user);
        Task<bool> DeleteUserAsync(int id);
        
        Task<int> LoginUser(User user);
        Task<List<UserListViewDTO>> GetAllUsersWithDetailsAsync();
    }
}
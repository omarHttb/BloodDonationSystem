using BloodDonationSystem.DTOS;
using BloodDonationSystem.Models;

namespace BloodDonationSystem.Services.Interfaces

{
    public interface IUserService 
    {
        Task<User> GetUserByIdAsync(int id);
        Task<List<User>> GetAllUserAsync();

        Task<List<string>> GetUserRolesAsync(int userId);

        Task<bool> ChangePassword(int userId, string currentPassword, string newPassword, string ConfirmPassword);

        Task<bool> IsUsernameExist(string username);
        Task<bool> IsEmailExist(string Email);
        Task<bool> IsPhoneNumberExist(string PhoneNumber);
        Task<int> RegisterUserAsync(User user);
        Task<User> UpdateUserAsync(int id, User user);
        Task<int> LoginUser(User user);
        Task<List<UserListViewDTO>> GetAllUsersWithDetailsAsync();
    
        Task <List<DonationHistoryDTO>> GetUserDonationHistoryAsync(int userId);
    }
}
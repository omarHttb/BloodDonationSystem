using Microsoft.EntityFrameworkCore;
using BloodDonationSystem.Models;
using BloodDonationSystem.Data;
using BloodDonationSystem.Services.Interfaces;
using BloodDonationSystem.DTOS;
using Microsoft.IdentityModel.Tokens;

namespace BloodDonationSystem.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetAllUserAsync()
        {
            // Simple SELECT * FROM Users
            return await _context.Users.ToListAsync();
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            // FindAsync is the most efficient for primary key lookups
            return await _context.Users.FindAsync(id);
        }

        public async Task<int> RegisterUserAsync(User User)
        {

            if (string.Equals(User.password,User.ConfirmPassword) == false)
            {
                return -1;
            }
            User.password = BCrypt.Net.BCrypt.HashPassword(User.password);
            _context.Users.Add(User);
            await _context.SaveChangesAsync();
            return User.Id;
        }



        public async Task<User> UpdateUserAsync(int id, User User)
        {
            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null) return null;

            bool userNameExists = await _context.Users
                .AnyAsync(u => u.Name.ToLower() == User.Name.ToLower() && u.Id != id);
            if (userNameExists)
            {
                return null;
            }
            bool EmailExist = await _context.Users
                .AnyAsync(u => u.Email.ToLower() == User.Email.ToLower() && u.Id != id);
            if (EmailExist)
            {
                return null;
            }
            bool PhoneNumberExist = await _context.Users
                .AnyAsync(u => u.PhoneNumber == User.PhoneNumber && u.Id != id);
            if (PhoneNumberExist)
            {
                return null;
            }
            
            existingUser.Email = User.Email;
            existingUser.PhoneNumber = User.PhoneNumber;
            existingUser.Name = User.Name;

            await _context.SaveChangesAsync();
            return existingUser;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var User = await _context.Users.FindAsync(id);
            if (User == null) return false;

            _context.Users.Remove(User);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> LoginUser(User user)
        {
            var DbUser = await _context.Users
                .SingleOrDefaultAsync(u => u.Name == user.Name);

            if (DbUser == null)
            {
                DbUser = await _context.Users
                .SingleOrDefaultAsync(u => u.Email == user.Name);
            }

            if (DbUser == null)
            {
                DbUser = await _context.Users
                .SingleOrDefaultAsync(u => u.PhoneNumber == user.Name);
            }

            if (DbUser == null)
            {
                return -1;
            }

            if (user.password.IsNullOrEmpty())
                return -1;

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(user.password, DbUser.password);

            if(!isPasswordValid)
            {
                return -1;
            }
            return DbUser.Id;
        }

        public async Task<List<UserListViewDTO>> GetAllUsersWithDetailsAsync()
        {
            var users = await _context.Users
                .Select(u => new UserListViewDTO
                {
                    Id = u.Id,
                    Username = u.Name, // Ensure this matches your Entity property (e.g., UserName)
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,

                    // FIX 1: Handle nulls safely for Blood Type Name
                    BloodType = (u.Donors != null && u.Donors.BloodType != null)
                                ? u.Donors.BloodType.BloodTypeName
                                : "N/A",

                    // FIX 2: Populate the ID so the Modal knows which option to select
                    bloodTypeId = (u.Donors != null && u.Donors.BloodType != null)
                                  ? u.Donors.BloodType.Id
                                  : 0,

                    // FIX 3: Check for null before accessing IsAvailable
                    isUserAvailableToDonate = (u.Donors != null) ? u.Donors.IsAvailable : false,

                    isUserAdoner = u.Donors != null,

                    Roles = u.UserRoles.Select(ur => ur.Role.RoleName).ToList(),
                    RoleIds = u.UserRoles.Select(ur => ur.RoleId).ToList()
                })
                .ToListAsync();

            return users;
        }

        public async Task<bool> IsUsernameExist(string username)
        {
            bool userNameExists = await _context.Users
                .AnyAsync(u => u.Name.ToLower() == username.ToLower());

            if (userNameExists)
            {

                return true;
            }

           return false;
        }

        public async Task<bool> IsEmailExist(string Email)
        {
            bool EmailExists = await _context.Users
            .AnyAsync(u => u.Email.ToLower() == Email.ToLower());

            if (EmailExists)
            {

                return true;
            }

            return false;

        }

        public async Task<bool> IsPhoneNumberExist(string PhoneNumber)
        {
            bool PhoneNumberExists = await _context.Users
            .AnyAsync(u => u.PhoneNumber.ToLower() == PhoneNumber.ToLower());

            if (PhoneNumberExists)
            {

                return true;
            }

            return false;

        }

        public async Task<List<string>> GetUserRolesAsync(int userId)
        {
            var roles = await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.Role.RoleName)
                .ToListAsync();

            return roles;
        }

        public async Task<bool> ChangePassword(int userId, string currentPassword, string newPassword, string ConfirmPassword)
        {
            var DbUser = await _context.Users
            .SingleOrDefaultAsync(u => u.Id == userId);

            bool passwordMatch = BCrypt.Net.BCrypt.Verify(currentPassword, DbUser.password);

            if(!passwordMatch)
            {
                return false;
            }

            if (string.Equals(newPassword, ConfirmPassword) == false)
            {
                return false;
            }

            newPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);

            DbUser.password = newPassword;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<DonationHistoryDTO>> GetUserDonationHistoryAsync(int userId)
        {
            var donor = _context.Donors.Where(d => d.UserId == userId)
                                        .Include(d => d.Donations)
                                        .ThenInclude(d => d.Status)
                                        .Include(d => d.Donations)
                                        .ThenInclude(d => d.BloodRequest).ThenInclude(br => br.BloodType)
                                        .FirstOrDefault();

            if (donor == null)
            {
                return await Task.FromResult(new List<DonationHistoryDTO>());
            }

            var donationHistory =  donor.Donations.Select(d => new DonationHistoryDTO
            {
                quantity = d.Quantity,
                Status = d.Status.StatusName,
                bloodRequestId = d.BloodRequestId,
                DonationSubmitDate = d.DonationSubmitDate,
                BloodRequestBloodType = d.BloodRequest.BloodType.BloodTypeName,
                DonationDate = d.DonationDate,
                DateDonorChoseToDonate = d.WhenUserWantToDonate,
            }).ToList();


            return await Task.FromResult(donationHistory);
        }
    }
}
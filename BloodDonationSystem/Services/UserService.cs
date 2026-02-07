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

            bool userExists = await _context.Users
                .AnyAsync(u => u.Name.ToLower() == User.Name.ToLower() && u.Id != User.Id);
            if (userExists)
            {
                return null;
            }
            existingUser.password = User.password;
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
                .SingleOrDefaultAsync(u => u.Email == user.Email);
            }

            if (DbUser == null)
            {
                DbUser = await _context.Users
                .SingleOrDefaultAsync(u => u.PhoneNumber == user.PhoneNumber);
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
                    Username = u.Name,

                    BloodType = (u.Donors != null && u.Donors.BloodType != null)
                                ? u.Donors.BloodType.BloodTypeName
                                : "N/A",

                    Roles = u.UserRoles
                        .Select(ur => ur.Role.RoleName)
                        .ToList()
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
    }
}
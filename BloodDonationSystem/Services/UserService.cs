using Microsoft.EntityFrameworkCore;
using BloodDonationSystem.Models;
using BloodDonationSystem.Data;
using BloodDonationSystem.Services.Interfaces;

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
            bool userExists = await _context.Users
                .AnyAsync(u => u.Name.ToLower() == User.Name.ToLower());

            if (userExists)
            {
                // Handle the duplicate (throw exception or return error)
                return -1;
            }
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

        public async Task<bool> LoginUser(User user)
        {
            var DbUser = await _context.Users
                .SingleOrDefaultAsync(u => u.Name == user.Name);

            if (user.Name == null)
            {
                return false;
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(user.password, DbUser.password);

            if(!isPasswordValid)
            {
                return false;
            }
            return true;
        }
    }
}
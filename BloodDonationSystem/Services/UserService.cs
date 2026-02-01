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

        public async Task<User> CreateUserAsync(User User)
        {
            _context.Users.Add(User);
            await _context.SaveChangesAsync();
            return User;
        }

        public async Task<User> UpdateUserAsync(int id, User User)
        {
            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null) return null;

            // Updating properties manually
            existingUser.Id = User.Id;
            // Map any other properties from your User model here
            // existingUser.Name = User.Name; 

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
    }
}
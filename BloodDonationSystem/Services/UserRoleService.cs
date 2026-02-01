using Microsoft.EntityFrameworkCore;
using BloodDonationSystem.Models;
using BloodDonationSystem.Data;
using BloodDonationSystem.Services.Interfaces;

namespace BloodDonationSystem.Services
{
    public class UserRoleService : IUserRoleService
    {
        private readonly AppDbContext _context;

        public UserRoleService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserRole>> GetAllUserRoleAsync()
        {
            // Simple SELECT * FROM UserRoles
            return await _context.UserRoles.ToListAsync();
        }

        public async Task<UserRole> GetUserRoleByIdAsync(int id)
        {
            // FindAsync is the most efficient for primary key lookups
            return await _context.UserRoles.FindAsync(id);
        }

        public async Task<UserRole> CreateUserRoleAsync(UserRole UserRole)
        {
            _context.UserRoles.Add(UserRole);
            await _context.SaveChangesAsync();
            return UserRole;
        }

        public async Task<UserRole> UpdateUserRoleAsync(int id, UserRole UserRole)
        {
            var existingUserRole = await _context.UserRoles.FindAsync(id);
            if (existingUserRole == null) return null;

            // Updating properties manually
            existingUserRole.Id = UserRole.Id;
            // Map any other properties from your UserRole model here
            // existingUserRole.Name = UserRole.Name; 

            await _context.SaveChangesAsync();
            return existingUserRole;
        }

        public async Task<bool> DeleteUserRoleAsync(int id)
        {
            var UserRole = await _context.UserRoles.FindAsync(id);
            if (UserRole == null) return false;

            _context.UserRoles.Remove(UserRole);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
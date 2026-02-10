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
        public async Task<bool> UpdateUserRolesAsync(int userId, List<int> roleIds)
        {
            // 1. Get all current roles for this user
            var currentRoles = await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .ToListAsync();

            // 2. Remove roles that are NOT in the new list
            var rolesToRemove = currentRoles.Where(ur => !roleIds.Contains(ur.RoleId)).ToList();
            if (rolesToRemove.Any())
            {
                
                _context.UserRoles.RemoveRange(rolesToRemove);
            }

            // 3. Add roles that the user DOESN'T have yet
            var existingRoleIds = currentRoles.Select(ur => ur.RoleId).ToList();
            var rolesToAdd = roleIds
                .Where(id => !existingRoleIds.Contains(id))
                .Select(id => new UserRole
                {                  
                    UserId = userId,
                    RoleId = id,
                    RoleAssignDate = DateTime.Now,
                }).ToList();

            if (rolesToAdd.Any())
            {
                await _context.UserRoles.AddRangeAsync(rolesToAdd);
            }

            // 4. Save everything in one transaction
            await _context.SaveChangesAsync();
            return true;
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
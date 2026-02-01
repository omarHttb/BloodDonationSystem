using Microsoft.EntityFrameworkCore;
using BloodDonationSystem.Models;
using BloodDonationSystem.Data;
using BloodDonationSystem.Services.Interfaces;

namespace BloodDonationSystem.Services
{
    public class RoleService : IRoleService
    {
        private readonly AppDbContext _context;

        public RoleService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Role>> GetAllRoleAsync()
        {
            // Simple SELECT * FROM Roles
            return await _context.Roles.ToListAsync();
        }

        public async Task<Role> GetRoleByIdAsync(int id)
        {
            // FindAsync is the most efficient for primary key lookups
            return await _context.Roles.FindAsync(id);
        }

        public async Task<Role> CreateRoleAsync(Role Role)
        {
            _context.Roles.Add(Role);
            await _context.SaveChangesAsync();
            return Role;
        }

        public async Task<Role> UpdateRoleAsync(int id, Role Role)
        {
            var existingRole = await _context.Roles.FindAsync(id);
            if (existingRole == null) return null;

            existingRole.Id = Role.Id;
            await _context.SaveChangesAsync();
            return existingRole;
        }

        public async Task<bool> DeleteRoleAsync(int id)
        {
            var Role = await _context.Roles.FindAsync(id);
            if (Role == null) return false;

            _context.Roles.Remove(Role);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
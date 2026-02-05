using BloodDonationSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BloodDonationSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<BloodRequest> BloodRequests { get; set; }

        public DbSet<BloodType> BloodType { get; set; }
        public DbSet<Donation> Donations { get; set; }

        public DbSet<Donor> Donors { get; set; }

        public DbSet<Status> Status { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
      
     
        }
    }
}

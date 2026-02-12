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

        public DbSet<BloodBank> BloodBanks { get; set; }

        public DbSet<BloodBankHistory> BloodBankHistory { get; set; } 

        public DbSet<BloodType> BloodType { get; set; }
        public DbSet<Donation> Donations { get; set; }

        public DbSet<Donor> Donors { get; set; }

        public DbSet<Status> Status { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var bloodTypes = new List<BloodType>
            {
                new BloodType { Id = 1, BloodTypeName = "A+" },
                new BloodType { Id = 2, BloodTypeName = "A-" },
                new BloodType { Id = 3, BloodTypeName = "B+" },
                new BloodType { Id = 4, BloodTypeName = "B-" },
                new BloodType { Id = 5, BloodTypeName = "AB+" },
                new BloodType { Id = 6, BloodTypeName = "AB-" },
                new BloodType { Id = 7, BloodTypeName = "O+" },
                new BloodType { Id = 8, BloodTypeName = "O-" }
            };
            modelBuilder.Entity<BloodType>().HasData(bloodTypes);


            var bloodBanks = bloodTypes.Select(bt => new BloodBank
            {
                Id = bt.Id, 
                BloodTypeId = bt.Id,
                quantity = 5
            }).ToArray();

            modelBuilder.Entity<BloodBank>().HasData(bloodBanks);


            
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, RoleName = "Donor" },
                new Role { Id = 2, RoleName = "Admin" },
                new Role { Id = 3, RoleName = "Hospital" }
            );


            // --- 4. Seed Statuses ---
            modelBuilder.Entity<Status>().HasData(
                new Status { Id = 1, StatusName = "Completed" },
                new Status { Id = 2, StatusName = "Pending" },
                new Status { Id = 3, StatusName = "Cancelled" },
                new Status { Id = 4, StatusName = "Rejected" },
                new Status { Id = 5, StatusName = "Approved" }
            );



        
             var adminId = 1;

             var hashedPassword = BCrypt.Net.BCrypt.HashPassword("123");

             modelBuilder.Entity<User>().HasData(new User
             {
                 Id = adminId,
                 Name = "admin",
                 Email = "admin@hospital.com",
                 password = hashedPassword,
                 PhoneNumber = "1234567890"
             });


            modelBuilder.Entity<UserRole>().HasData(new UserRole
            {
                Id = 1,
                UserId = adminId, 
                RoleId = 2        
            });

            modelBuilder.Entity<BloodRequest>().HasData(new BloodRequest
            {
                Id = 1,
                BloodRequestDate = DateTime.Now,
                BloodTypeId = 1,
                IsActive = false,
                isApproved = false,
                Quantity = 5,
            });

        }
    }
}

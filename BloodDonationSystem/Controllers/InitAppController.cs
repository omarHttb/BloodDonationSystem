using BloodDonationSystem.Data;
using BloodDonationSystem.Models;
using BloodDonationSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IO;
using System.Text.Json;

namespace BloodDonationSystem.Controllers
{
    public class InitAppController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IUserService _userService;
        private readonly IUserRoleService _roleService;

        public InitAppController(AppDbContext context, IUserRoleService roleService, IUserService userService)
        {
            _context = context;
            _roleService = roleService;
            _userService = userService;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult initApp()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> initAppTask(string connectionString) {

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            var json = System.IO.File.ReadAllText(filePath);

            var configObject = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            var connStrings = JsonSerializer.Deserialize<Dictionary<string, string>>(configObject["ConnectionStrings"].ToString());

            connStrings["DefaultConnection"] = connectionString;

            configObject["ConnectionStrings"] = connStrings;

            var updatedJson = JsonSerializer.Serialize(configObject, new JsonSerializerOptions { WriteIndented = true });

            System.IO.File.WriteAllText(filePath, updatedJson);

            if (await _context.Database.CanConnectAsync())
            {
                ModelState.AddModelError("", "Database already exist, maybe try changing database name in connection string");
                return View("initApp");

            }

            if (connectionString.IsNullOrEmpty())
            {
                ModelState.AddModelError("", "You have to add a connection string first");
                return View("initApp"); 
            }

   


            // 1. Ensure the Database is created and migrations are applied
            // This creates the DB file/tables if they don't exist yet.
            await _context.Database.MigrateAsync();

            bool dataAdded = false;

            if (!await _context.BloodType.AnyAsync())
            {
                var bloodTypes = new List<BloodType>
                {
                new BloodType { BloodTypeName = "A+" },
                new BloodType { BloodTypeName = "A-" },
                new BloodType { BloodTypeName = "B+" },
                new BloodType { BloodTypeName = "B-" },
                new BloodType { BloodTypeName = "AB+" },
                new BloodType { BloodTypeName = "AB-" },
                new BloodType { BloodTypeName = "O+" },
                new BloodType { BloodTypeName = "O-" }
                };

                await _context.BloodType.AddRangeAsync(bloodTypes);
                dataAdded = true;
            }

            if (!await _context.BloodBanks.AnyAsync())
            {
                var bloodBanks = new List<BloodBank>
                {
                 new BloodBank {   BloodTypeId =  _context.BloodType.First(b => b.BloodTypeName == "A+").Id , quantity = 5},
                 new BloodBank {   BloodTypeId =  _context.BloodType.First(b => b.BloodTypeName == "A-").Id , quantity = 5},
                 new BloodBank {   BloodTypeId =  _context.BloodType.First(b => b.BloodTypeName == "O+").Id , quantity = 5},
                 new BloodBank {   BloodTypeId =  _context.BloodType.First(b => b.BloodTypeName == "O-").Id, quantity = 5},
                 new BloodBank {   BloodTypeId =  _context.BloodType.First(b => b.BloodTypeName == "B+").Id ,quantity = 5},
                 new BloodBank {   BloodTypeId =  _context.BloodType.First(b => b.BloodTypeName == "B-").Id,quantity = 5 },
                 new BloodBank {   BloodTypeId =  _context.BloodType.First(b => b.BloodTypeName == "AB+").Id ,quantity = 5},
                 new BloodBank {   BloodTypeId =  _context.BloodType.First(b => b.BloodTypeName == "AB-").Id ,quantity = 5}
                };

                await _context.BloodBanks.AddRangeAsync(bloodBanks);

            }

            if (!await _context.Roles.AnyAsync())
            {
                var roles = new List<Role>
                {
                    new Role { Id = 1, RoleName = "Donor" },
                    new Role { Id = 2, RoleName = "Admin" },
                    new Role { Id = 3, RoleName = "Hospital" }
                };

                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    // Explicitly allow manual ID insertion
                    await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Roles ON");

                    await _context.Roles.AddRangeAsync(roles);
                    await _context.SaveChangesAsync();

                    await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Roles OFF");
                    await transaction.CommitAsync();
                }
            }

            if (!await _context.Status.AnyAsync())
            {
                var Statuses = new List<Status>
                {
                    new Status { Id = 1, StatusName="Completed" },
                    new Status { Id = 2, StatusName="Pending" },
                    new Status { Id = 3, StatusName="Cancelled" },
                    new Status { Id = 4, StatusName="Rejected" },
                    new Status { Id = 5, StatusName="Approved" },
                };
            }


            // 3. Seed Users (Initial Admin)
            if (!await _context.Users.AnyAsync())
            {
                var adminUser = new User
                {
                    Name = "admin",
                    Email = "admin@hospital.com",
                    password = "123", // In real apps, ensure this is hashed!
                    PhoneNumber = "1234567890"
                };

               var userId = await _userService.RegisterUserAsync(adminUser);

                await _roleService.CreateUserRoleAsync(new UserRole { RoleId=2,UserId =userId});
                
                dataAdded = true;
            }

            
            if (dataAdded)
            {
                await _context.SaveChangesAsync();
                return RedirectToAction("Account", "Login");
            }

            return RedirectToAction("Account", "Login");
        }

    }
}

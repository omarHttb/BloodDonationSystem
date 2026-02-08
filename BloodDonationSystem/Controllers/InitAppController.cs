using BloodDonationSystem.Data;
using BloodDonationSystem.Models;
using BloodDonationSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public IActionResult initApp()
        {
            return View();
        }

        public async Task<IActionResult> initAppTask() {
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

            // 4. Save Changes if we added anything
            if (dataAdded)
            {
                await _context.SaveChangesAsync();
                return RedirectToAction("Account", "Login");
            }

            return RedirectToAction("Account", "Login");
        }

    }
}

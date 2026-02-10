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
        public async Task<IActionResult> InitAppTask(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                ModelState.AddModelError("", "You have to add a connection string first");
                return View("initApp");
            }

            // 1. Update appsettings.json
            try
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
                var json = await System.IO.File.ReadAllTextAsync(filePath);
                var configObject = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

                var connStrings = JsonSerializer.Deserialize<Dictionary<string, string>>(configObject["ConnectionStrings"].ToString());
                connStrings["DefaultConnection"] = connectionString;
                configObject["ConnectionStrings"] = connStrings;

                var updatedJson = JsonSerializer.Serialize(configObject, new JsonSerializerOptions { WriteIndented = true });
                await System.IO.File.WriteAllTextAsync(filePath, updatedJson);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Failed to update config: {ex.Message}");
                return View("initApp");
            }

            // 2. Use a FRESH context with the NEW connection string
            // This bypasses the DI context which is still stuck on the old string
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            // Change .UseSqlServer to your specific provider (Sqlite, Npgsql, etc.)
            optionsBuilder.UseSqlServer(connectionString);

            using (var context = new AppDbContext(optionsBuilder.Options))
            {
                try
                {
                    // Apply Migrations
                    await context.Database.MigrateAsync();

                    // --- SEEDING LOGIC ---

                    // 1. Seed BloodTypes (Save immediately so IDs exist for BloodBanks)
                    if (!await context.BloodType.AnyAsync())
                    {
                        var bloodTypes = new List<BloodType>
                {
                    new BloodType { BloodTypeName = "A+" }, new BloodType { BloodTypeName = "A-" },
                    new BloodType { BloodTypeName = "B+" }, new BloodType { BloodTypeName = "B-" },
                    new BloodType { BloodTypeName = "AB+" }, new BloodType { BloodTypeName = "AB-" },
                    new BloodType { BloodTypeName = "O+" }, new BloodType { BloodTypeName = "O-" }
                };
                        await context.BloodType.AddRangeAsync(bloodTypes);
                        await context.SaveChangesAsync();
                    }

                    // 2. Seed BloodBanks (Now we can safely query IDs)
                    if (!await context.BloodBanks.AnyAsync())
                    {
                        var types = await context.BloodType.ToListAsync();
                        var bloodBanks = types.Select(t => new BloodBank
                        {
                            BloodTypeId = t.Id,
                            quantity = 5
                        }).ToList();

                        await context.BloodBanks.AddRangeAsync(bloodBanks);
                    }

                    // 3. Seed Roles
                    if (!await context.Roles.AnyAsync())
                    {
                        var roles = new List<Role>
                {
                    new Role { Id = 1, RoleName = "Donor" },
                    new Role { Id = 2, RoleName = "Admin" },
                    new Role { Id = 3, RoleName = "Hospital" }
                };

                        using var transaction = await context.Database.BeginTransactionAsync();
                        await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Roles ON");
                        await context.Roles.AddRangeAsync(roles);
                        await context.SaveChangesAsync();
                        await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Roles OFF");
                        await transaction.CommitAsync();
                    }

                    // 4. Seed Statuses
                    if (!await context.Status.AnyAsync())
                    {
                        var statuses = new List<Status>
                {
                    new Status { Id = 1, StatusName = "Completed" },
                    new Status { Id = 2, StatusName = "Pending" },
                    new Status { Id = 3, StatusName = "Cancelled" },
                    new Status { Id = 4, StatusName = "Rejected" },
                    new Status { Id = 5, StatusName = "Approved" }
                };

                        using var transaction = await context.Database.BeginTransactionAsync();
                        await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Status ON");
                        await context.Status.AddRangeAsync(statuses);
                        await context.SaveChangesAsync();
                        await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Status OFF");
                        await transaction.CommitAsync();
                    }

                    // 5. Final Save for anything pending
                    await context.SaveChangesAsync();

                    // 6. Seed Admin via Service (if service is registered)
                    if (!await context.Users.AnyAsync())
                    {
                        var adminUser = new User
                        {
                            Name = "admin",
                            Email = "admin@hospital.com",
                            password = BCrypt.Net.BCrypt.HashPassword("123"),
                            PhoneNumber = "1234567890"
                        };

                        // Note: _userService might still use the OLD connection string.
                        // It's safer to create the user directly in the 'context' here.
                        await context.Users.AddAsync(adminUser);
                        await context.SaveChangesAsync();

                        var adminRole = new UserRole { RoleId = 2, UserId = adminUser.Id };
                        await context.UserRoles.AddAsync(adminRole);
                        await context.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Database Setup Failed: {ex.Message}");
                    return View("initApp");
                }
            }

            return RedirectToAction("Login", "Account");
        }
    }
}

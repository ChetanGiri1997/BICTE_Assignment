using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace EmployeeManagementSystem.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Seed Admin Role
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            // Seed Admin User
            var adminUser = new IdentityUser
            {
                UserName = "admin@system.com",
                Email = "admin@system.com",
                EmailConfirmed = true // Optional: Bypasses email confirmation
            };

            var userExists = await userManager.FindByEmailAsync(adminUser.Email);
            if (userExists == null)
            {
                var result = await userManager.CreateAsync(adminUser, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}
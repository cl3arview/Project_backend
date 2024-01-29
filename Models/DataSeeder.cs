using Microsoft.AspNetCore.Identity;

namespace Project_backend.Models
{
    public static class DataSeeder
    {
        public static async Task SeedDataAsync(UserManager<Users> userManager, RoleManager<IdentityRole> roleManager)
        {
            await SeedRolesAsync(roleManager);
            await SeedAdminAsync(userManager);
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
        }

        private static async Task SeedAdminAsync(UserManager<Users> userManager)
        {
            var adminEmail = "admin@gmail.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new Users { UserName = adminEmail, Email = adminEmail };
                var result = await userManager.CreateAsync(adminUser, "Test123@");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}


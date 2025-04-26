using EliteSportsAcademy.Models.Account;
using Microsoft.AspNetCore.Identity;

namespace EliteSportsAcademy.Data
{
    public static class DataSeeder
    {
        public static async Task SeedSAdministrator(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string superAdminEmail = "superadmin@elitesports.com";
            string superAdminPassword = "@bul.789";

            string[] roles = { "SuperAdmin", "Admin", "Instructor", "User" };

            // Ensure roles exist (in case migration not run yet)
            //if (!await roleManager.RoleExistsAsync("Admin"))
            //    await roleManager.CreateAsync(new IdentityRole("Admin"));
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Check if user exists
            var existingUser = await userManager.FindByEmailAsync(superAdminEmail);
            if (existingUser == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "superadmin",
                    Email = superAdminEmail,
                    FirstName = "Elite",
                    LastName = "Admin",
                    EmailConfirmed = true,
                    IsAgree = true
                };

                var result = await userManager.CreateAsync(user, superAdminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }
            // Seed SuperAdmin
            await CreateUserIfNotExists(userManager,email: "superadmin@elitesports.com",username: "superadmin",password: "Super@123",firstName: "Elite",lastName: "Super",role: "SuperAdmin");
            // Seed Admin
            await CreateUserIfNotExists(userManager,email: "admin@elitesports.com",username: "admin",password: "Admin@123",firstName: "Elite",lastName: "Admin",role: "Admin");
        }

        private static async Task CreateUserIfNotExists(UserManager<ApplicationUser> userManager,string email,string username,string password,string firstName,string lastName,string role)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = username,
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    EmailConfirmed = true,
                    IsAgree = true
                };

                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }
    }
}

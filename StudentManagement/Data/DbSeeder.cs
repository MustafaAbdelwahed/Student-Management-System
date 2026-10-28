using CompanySystem.Models;
using Microsoft.AspNetCore.Identity;

namespace CompanySystem.Data
{

    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider services, IConfiguration config)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<AppUser>>();

            foreach (string role in Roles.All)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            var email = config["SeedAdmin:Email"];
            var password = config["SeedAdmin:Password"];
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return;

            if (await userManager.FindByEmailAsync(email) is null)
            {
                AppUser admin = new AppUser
                {
                    UserName = email,
                    Email = email,
                    FullName = "System Admin",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(admin, password);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, Roles.Admin);
            }
        }
    }
}

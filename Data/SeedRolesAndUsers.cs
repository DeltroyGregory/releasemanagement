using Microsoft.AspNetCore.Identity;

namespace rmp.Data;

public static class SeedRolesAndUsers
{
    private static readonly string[] Roles = ["Admin", "ReleaseCoordinator", "Power User", "Reader"];

    public static async Task RunAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

        foreach (var role in Roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Id must match DevAuthHandler's NameIdentifier claim so tasks/releases assigned to "dev-admin"
        // via the UI resolve back to this row (and so GET /api/users has someone to list in local dev).
        const string devAdminId = "dev-admin";
        const string adminEmail = "admin@rmp.local";
        var existing = await userManager.FindByIdAsync(devAdminId);
        if (existing is null)
        {
            var admin = new IdentityUser
            {
                Id = devAdminId,
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
            };

            var adminPassword = Environment.GetEnvironmentVariable("SEED_ADMIN_PASSWORD");
            var result = string.IsNullOrEmpty(adminPassword)
                ? await userManager.CreateAsync(admin)
                : await userManager.CreateAsync(admin, adminPassword);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}

using Microsoft.AspNetCore.Identity;
using rmp.Models;

namespace rmp.Data;

public static class SeedRolesAndUsers
{
    private static readonly string[] Roles = ["Admin", "Release Coordinator", "Power User", "Reader"];

    public static async Task RunAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        // One-time cleanup: earlier seeds (before roles were renamed to match "Power User"'s
        // spaced style) created "ReleaseCoordinator". Rename it in place rather than leaving an
        // orphaned duplicate with no permission grants sitting alongside "Release Coordinator".
        var legacyRole = await roleManager.FindByNameAsync("ReleaseCoordinator");
        if (legacyRole is not null)
        {
            if (await roleManager.RoleExistsAsync("Release Coordinator"))
            {
                // Both now exist (e.g. this seed already ran once against a DB that predates the
                // rename) — reassign anyone still on the legacy role, then drop it.
                foreach (var user in await userManager.GetUsersInRoleAsync("ReleaseCoordinator"))
                {
                    await userManager.RemoveFromRoleAsync(user, "ReleaseCoordinator");
                    await userManager.AddToRoleAsync(user, "Release Coordinator");
                }

                await roleManager.DeleteAsync(legacyRole);
            }
            else
            {
                await roleManager.SetRoleNameAsync(legacyRole, "Release Coordinator");
                await roleManager.UpdateAsync(legacyRole);
            }
        }

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
            var admin = new ApplicationUser
            {
                Id = devAdminId,
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow,
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
        else if (existing.CreatedAt == default)
        {
            // Backfill for a dev-admin row created before CreatedAt existed on ApplicationUser.
            existing.CreatedAt = DateTime.UtcNow;
            await userManager.UpdateAsync(existing);
        }
    }
}

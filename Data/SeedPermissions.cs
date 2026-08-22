using Microsoft.EntityFrameworkCore;
using rmp.Models;

namespace rmp.Data;

public static class SeedPermissions
{
    public static async Task RunAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<AppDbContext>();

        foreach (var (role, keys) in PermissionCatalog.DefaultGrants)
        {
            foreach (var key in keys)
            {
                var exists = await db.RolePermissions
                    .AnyAsync(rp => rp.RoleName == role && rp.PermissionKey == key);

                if (!exists)
                {
                    db.RolePermissions.Add(new RolePermission { RoleName = role, PermissionKey = key });
                }
            }
        }

        await db.SaveChangesAsync();
    }
}

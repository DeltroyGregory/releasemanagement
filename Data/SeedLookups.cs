using Microsoft.EntityFrameworkCore;
using rmp.Models;

namespace rmp.Data;

/// <summary>
/// Only TaskType gets a starter list — Component/AppName/Version are specific to whatever this
/// org actually has, so seeding guesses for those would just be wrong. Admins add them via
/// Admin > Task Fields.
/// </summary>
public static class SeedLookups
{
    private static readonly string[] TaskTypes = ["Bug", "Feature", "Config Change", "Infrastructure"];

    public static async Task RunAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<AppDbContext>();

        foreach (var value in TaskTypes)
        {
            var exists = await db.Lookups.AnyAsync(l => l.Category == LookupItem.TaskType && l.Value == value);
            if (!exists)
            {
                db.Lookups.Add(new LookupItem { Category = LookupItem.TaskType, Value = value });
            }
        }

        await db.SaveChangesAsync();
    }
}

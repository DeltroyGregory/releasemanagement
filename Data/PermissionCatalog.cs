namespace rmp.Data;

public record PermissionKey(string Key, string Area, string Label);

/// <summary>
/// The full set of permission keys the app understands, and the default grants seeded for each
/// non-Admin role. Admin always has every permission implicitly — it's never stored as rows.
/// </summary>
public static class PermissionCatalog
{
    public const string ReleaseCoordinator = "Release Coordinator";
    public const string PowerUser = "Power User";
    public const string Reader = "Reader";

    public static readonly IReadOnlyList<PermissionKey> All =
    [
        new("releases.view", "Releases", "View releases"),
        new("releases.create", "Releases", "Create releases"),
        new("releases.edit", "Releases", "Edit releases"),
        new("releases.delete", "Releases", "Delete releases"),
        new("tasks.manage", "Tasks", "Manage tasks"),
        new("admin.manage_users", "Admin", "Manage users"),
        new("admin.manage_permissions", "Admin", "Manage permissions"),
    ];

    public static readonly IReadOnlyDictionary<string, string[]> DefaultGrants = new Dictionary<string, string[]>
    {
        [ReleaseCoordinator] = ["releases.view", "releases.create", "releases.edit", "releases.delete", "tasks.manage"],
        [PowerUser] = ["releases.view", "releases.create", "releases.edit", "tasks.manage"],
        [Reader] = ["releases.view"],
    };
}

namespace rmp.Models;

/// <summary>
/// A granted permission for a role. Presence of a row means granted — there is no separate
/// "denied" state. Admin is never stored here; it implicitly has every permission (see
/// PermissionCatalog).
/// </summary>
public class RolePermission
{
    public int Id { get; set; }
    public string RoleName { get; set; } = "";
    public string PermissionKey { get; set; } = "";
}

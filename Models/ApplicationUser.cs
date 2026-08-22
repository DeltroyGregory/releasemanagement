using Microsoft.AspNetCore.Identity;

namespace rmp.Models;

/// <summary>
/// Extends Identity's default user with the fields the Admin > Users screen needs that
/// IdentityUser doesn't carry. LastLoginAt is really "last seen making an authenticated request"
/// (set by JitUserProvisioning on every request, not just sign-in) — close enough for the UI.
/// </summary>
public class ApplicationUser : IdentityUser
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
}

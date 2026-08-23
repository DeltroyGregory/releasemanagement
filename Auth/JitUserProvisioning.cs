using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using rmp.Data;
using rmp.Models;

namespace rmp.Auth;

/// <summary>
/// Ensures every authenticated request's principal has a matching AspNetUsers row, so signing in
/// (via DevAuthHandler or real Azure AD) is enough to show up under Admin > Users — no separate
/// registration step. New users default to the Reader role, EXCEPT the address configured in
/// BootstrapAdminEmail, which is promoted to Admin here (checked on every request, not just
/// creation, so it also fixes a row that was already provisioned as Reader before this setting
/// existed). Also stamps LastLoginAt on every authenticated request (really "last seen", not just
/// sign-in — close enough for the UI). Runs a lookup per request; fine at this app's current
/// scale, revisit if that becomes a real cost.
/// </summary>
public class JitUserProvisioning(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, UserManager<ApplicationUser> userManager, IConfiguration configuration)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                var user = await userManager.FindByIdAsync(userId);
                if (user is null)
                {
                    var email = context.User.FindFirst(ClaimTypes.Email)?.Value
                        ?? context.User.FindFirst("preferred_username")?.Value;

                    user = new ApplicationUser
                    {
                        Id = userId,
                        UserName = email ?? userId,
                        Email = email,
                        EmailConfirmed = true,
                        CreatedAt = DateTime.UtcNow,
                        LastLoginAt = DateTime.UtcNow,
                    };

                    var result = await userManager.CreateAsync(user);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, PermissionCatalog.Reader);
                    }
                }
                else
                {
                    user.LastLoginAt = DateTime.UtcNow;
                    await userManager.UpdateAsync(user);
                }

                var bootstrapAdminEmail = configuration["BootstrapAdminEmail"];
                if (!string.IsNullOrWhiteSpace(bootstrapAdminEmail)
                    && string.Equals(user.Email, bootstrapAdminEmail, StringComparison.OrdinalIgnoreCase))
                {
                    var roles = await userManager.GetRolesAsync(user);
                    if (!roles.Contains(PermissionCatalog.Admin))
                    {
                        if (roles.Count > 0)
                        {
                            await userManager.RemoveFromRolesAsync(user, roles);
                        }

                        await userManager.AddToRoleAsync(user, PermissionCatalog.Admin);
                    }
                }
            }
        }

        await next(context);
    }
}

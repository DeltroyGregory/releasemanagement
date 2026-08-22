using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using rmp.Models;

namespace rmp.Auth;

/// <summary>
/// Makes the local AspNetUserRoles table (managed via Admin > Users) the single source of truth
/// for [Authorize(Roles = ...)] checks, in both auth modes — neither DevAuthHandler's own claims
/// nor an Azure AD token's "roles" claim (which would need separately configured Azure AD App
/// Roles) are used for authorization once this runs.
/// </summary>
public class DbRoleClaimsTransformation(UserManager<ApplicationUser> userManager) : IClaimsTransformation
{
    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var identity = principal.Identity as ClaimsIdentity;
        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (identity is null || string.IsNullOrEmpty(userId))
        {
            return principal;
        }

        foreach (var existing in identity.FindAll(ClaimTypes.Role).ToList())
        {
            identity.RemoveClaim(existing);
        }

        var user = await userManager.FindByIdAsync(userId);
        if (user is not null)
        {
            foreach (var role in await userManager.GetRolesAsync(user))
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, role));
            }
        }

        return principal;
    }
}

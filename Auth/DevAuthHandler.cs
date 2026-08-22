using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace rmp.Auth;

/// <summary>
/// Authenticates every request as the seeded dev admin. Only ever wired up in Program.cs when
/// AzureAd:TenantId is unset — a real deployment always has that set and uses AddMicrosoftIdentityWebApi instead.
/// </summary>
public class DevAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string SchemeName = "DevAuth";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // No Role claim here — DbRoleClaimsTransformation adds it from the AspNetUserRoles table,
        // so changing the dev-admin's role via Admin > Users actually takes effect locally too.
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "dev-admin"),
            new Claim("preferred_username", "admin@rmp.local"),
            new Claim(ClaimTypes.Email, "admin@rmp.local"),
        };
        var identity = new ClaimsIdentity(claims, SchemeName);
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), SchemeName);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}

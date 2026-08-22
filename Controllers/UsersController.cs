using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rmp.Data;
using rmp.DTOs;
using rmp.Models;

namespace rmp.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController(AppDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List()
    {
        var users = await db.Users.OrderBy(u => u.Email).ToListAsync();

        var dtos = new List<UserDto>();
        foreach (var user in users)
        {
            dtos.Add(await ToDtoAsync(user));
        }

        return Ok(dtos);
    }

    [HttpGet("roles")]
    public async Task<IActionResult> ListRoles()
    {
        var roles = await roleManager.Roles.Select(r => r.Name!).ToListAsync();
        return Ok(roles.OrderBy(r => r == "Admin" ? 0 : 1).ThenBy(r => r));
    }

    [HttpPost("invite")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Invite(UserInviteDto dto)
    {
        if (!await roleManager.RoleExistsAsync(dto.Role))
        {
            return BadRequest($"Role '{dto.Role}' does not exist.");
        }

        if (await userManager.FindByEmailAsync(dto.Email) is not null)
        {
            return BadRequest("A user with that email already exists.");
        }

        // Placeholder row keyed by a generated id. Reconciling this with the real Azure AD
        // principal id the first time this person actually signs in isn't implemented yet — for
        // now this just pre-assigns a role so it's on record ahead of their first login.
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = dto.Email,
            Email = dto.Email,
            EmailConfirmed = false,
            CreatedAt = DateTime.UtcNow,
        };

        var result = await userManager.CreateAsync(user);
        if (!result.Succeeded)
        {
            return BadRequest(string.Join("; ", result.Errors.Select(e => e.Description)));
        }

        await userManager.AddToRoleAsync(user, dto.Role);

        return Ok(await ToDtoAsync(user));
    }

    [HttpPut("{id}/role")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateRole(string id, UserRoleUpdateDto dto)
    {
        if (!await roleManager.RoleExistsAsync(dto.Role))
        {
            return BadRequest($"Role '{dto.Role}' does not exist.");
        }

        var user = await userManager.FindByIdAsync(id);
        if (user is null)
        {
            return NotFound();
        }

        var currentRoles = await userManager.GetRolesAsync(user);
        if (currentRoles.Count > 0)
        {
            await userManager.RemoveFromRolesAsync(user, currentRoles);
        }

        await userManager.AddToRoleAsync(user, dto.Role);

        return Ok(await ToDtoAsync(user));
    }

    private async Task<UserDto> ToDtoAsync(ApplicationUser user)
    {
        var roles = await userManager.GetRolesAsync(user);
        var isActive = user.LockoutEnd is null || user.LockoutEnd < DateTimeOffset.UtcNow;
        return new UserDto(user.Id, user.Email, user.UserName, roles.FirstOrDefault(), isActive, user.CreatedAt, user.LastLoginAt);
    }
}

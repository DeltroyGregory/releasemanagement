using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rmp.Data;
using rmp.DTOs;

namespace rmp.Controllers;

[ApiController]
[Route("api/permissions")]
[Authorize]
public class PermissionsController(AppDbContext db, RoleManager<IdentityRole> roleManager) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetMatrix()
    {
        var roles = (await roleManager.Roles.Select(r => r.Name!).ToListAsync())
            .OrderBy(r => r == "Admin" ? 0 : 1)
            .ToList();

        var grants = roles.ToDictionary(r => r, _ => new List<string>());

        foreach (var key in PermissionCatalog.All.Select(p => p.Key))
        {
            if (grants.ContainsKey("Admin"))
            {
                grants["Admin"].Add(key);
            }
        }

        var stored = await db.RolePermissions.ToListAsync();
        foreach (var rp in stored)
        {
            if (grants.TryGetValue(rp.RoleName, out var list))
            {
                list.Add(rp.PermissionKey);
            }
        }

        var permissionDtos = PermissionCatalog.All.Select(p => new PermissionKeyDto(p.Key, p.Area, p.Label)).ToList();
        return Ok(new PermissionMatrixDto(permissionDtos, roles, grants));
    }

    [HttpPut]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateMatrix(PermissionMatrixUpdateDto dto)
    {
        var validKeys = PermissionCatalog.All.Select(p => p.Key).ToHashSet();

        foreach (var (role, keys) in dto.Grants)
        {
            if (role == "Admin")
            {
                continue; // Admin is implicit and can't be restricted.
            }

            var existing = await db.RolePermissions.Where(rp => rp.RoleName == role).ToListAsync();
            db.RolePermissions.RemoveRange(existing);

            foreach (var key in keys.Where(validKeys.Contains).Distinct())
            {
                db.RolePermissions.Add(new Models.RolePermission { RoleName = role, PermissionKey = key });
            }
        }

        await db.SaveChangesAsync();
        return await GetMatrix();
    }
}

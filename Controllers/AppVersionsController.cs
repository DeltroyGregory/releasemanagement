using mbm.Data;
using mbm.DTOs;
using mbm.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace mbm.Controllers;

[ApiController]
[Route("api/app-versions")]
[Authorize]
public class AppVersionsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List()
    {
        var items = await db.AppVersions.OrderByDescending(v => v.CreatedAt).Select(v => ToDto(v)).ToListAsync();
        return Ok(items);
    }

    [HttpPost]
    public async Task<IActionResult> Create(AppVersionCreateDto dto)
    {
        var entity = new AppVersion { SystemName = dto.SystemName, VersionLabel = dto.VersionLabel };
        db.AppVersions.Add(entity);
        await db.SaveChangesAsync();
        return Ok(ToDto(entity));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await db.AppVersions.FindAsync(id);
        if (entity is null)
        {
            return NotFound();
        }

        db.AppVersions.Remove(entity);
        await db.SaveChangesAsync();
        return NoContent();
    }

    private static AppVersionDto ToDto(AppVersion v) => new(v.Id, v.SystemName, v.VersionLabel, v.CreatedAt);
}

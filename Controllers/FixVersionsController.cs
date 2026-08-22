using mbm.Data;
using mbm.DTOs;
using mbm.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace mbm.Controllers;

[ApiController]
[Route("api/fix-versions")]
[Authorize]
public class FixVersionsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int? releaseId)
    {
        var query = db.FixVersions.AsQueryable();
        if (releaseId.HasValue)
        {
            query = query.Where(f => f.ReleaseId == releaseId.Value);
        }

        var items = await query.Select(f => ToDto(f)).ToListAsync();
        return Ok(items);
    }

    [HttpPost]
    public async Task<IActionResult> Create(FixVersionCreateDto dto)
    {
        if (!await db.Releases.AnyAsync(r => r.Id == dto.ReleaseId))
        {
            return BadRequest($"Release {dto.ReleaseId} does not exist.");
        }

        var entity = new FixVersion
        {
            ReleaseId = dto.ReleaseId,
            Name = dto.Name,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
        };

        db.FixVersions.Add(entity);
        await db.SaveChangesAsync();
        return Ok(ToDto(entity));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, FixVersionUpdateDto dto)
    {
        var entity = await db.FixVersions.FindAsync(id);
        if (entity is null)
        {
            return NotFound();
        }

        entity.Name = dto.Name;
        entity.StartDate = dto.StartDate;
        entity.EndDate = dto.EndDate;

        await db.SaveChangesAsync();
        return Ok(ToDto(entity));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await db.FixVersions.FindAsync(id);
        if (entity is null)
        {
            return NotFound();
        }

        db.FixVersions.Remove(entity);
        await db.SaveChangesAsync();
        return NoContent();
    }

    private static FixVersionDto ToDto(FixVersion f) => new(f.Id, f.ReleaseId, f.Name, f.StartDate, f.EndDate, f.JiraFixVersionId);
}

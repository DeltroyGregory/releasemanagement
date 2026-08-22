using mbm.Data;
using mbm.DTOs;
using mbm.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace mbm.Controllers;

[ApiController]
[Route("api/release-systems")]
[Authorize]
public class ReleaseSystemsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int? releaseId)
    {
        var query = db.ReleaseSystems.AsQueryable();
        if (releaseId.HasValue)
        {
            query = query.Where(rs => rs.ReleaseId == releaseId.Value);
        }

        var items = await query.Select(rs => ToDto(rs)).ToListAsync();
        return Ok(items);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ReleaseSystemCreateDto dto)
    {
        if (!await db.Releases.AnyAsync(r => r.Id == dto.ReleaseId))
        {
            return BadRequest($"Release {dto.ReleaseId} does not exist.");
        }

        var entity = new ReleaseSystem { ReleaseId = dto.ReleaseId, SystemName = dto.SystemName, Notes = dto.Notes };
        db.ReleaseSystems.Add(entity);
        await db.SaveChangesAsync();

        return Ok(ToDto(entity));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await db.ReleaseSystems.FindAsync(id);
        if (entity is null)
        {
            return NotFound();
        }

        db.ReleaseSystems.Remove(entity);
        await db.SaveChangesAsync();
        return NoContent();
    }

    private static ReleaseSystemDto ToDto(ReleaseSystem rs) => new(rs.Id, rs.ReleaseId, rs.SystemName, rs.Notes);
}

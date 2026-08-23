using rmp.Data;
using rmp.DTOs;
using rmp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace rmp.Controllers;

[ApiController]
[Route("api/releases")]
[Authorize]
public class ReleasesController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] string? status)
    {
        var query = db.Releases.AsQueryable();
        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(r => r.Status == status);
        }

        var releases = await query.OrderByDescending(r => r.CreatedAt).Select(r => ToDto(r)).ToListAsync();
        return Ok(releases);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var release = await db.Releases
            .Include(r => r.Tasks).ThenInclude(t => t.Type)
            .Include(r => r.Tasks).ThenInclude(t => t.Component)
            .Include(r => r.Tasks).ThenInclude(t => t.AppName)
            .Include(r => r.Tasks).ThenInclude(t => t.Version)
            .Include(r => r.ReleaseSystems)
            .Include(r => r.FixVersions)
            .Include(r => r.Comments)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (release is null)
        {
            return NotFound();
        }

        return Ok(ToDetailDto(release));
    }

    [HttpPost]
    public async Task<IActionResult> Create(ReleaseCreateDto dto)
    {
        if (!Enum.TryParse<ReleaseType>(dto.ReleaseType, ignoreCase: true, out var releaseType))
        {
            return BadRequest($"Invalid release type '{dto.ReleaseType}'.");
        }

        var release = new Release
        {
            Name = dto.Name,
            Description = dto.Description,
            ReleaseType = releaseType,
            TargetDate = dto.TargetDate,
            CreatedByUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
        };

        db.Releases.Add(release);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = release.Id }, ToDto(release));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ReleaseUpdateDto dto)
    {
        var release = await db.Releases.FindAsync(id);
        if (release is null)
        {
            return NotFound();
        }

        if (!Enum.TryParse<ReleaseType>(dto.ReleaseType, ignoreCase: true, out var releaseType))
        {
            return BadRequest($"Invalid release type '{dto.ReleaseType}'.");
        }

        release.Name = dto.Name;
        release.Description = dto.Description;
        release.ReleaseType = releaseType;
        release.Status = dto.Status;
        release.TargetDate = dto.TargetDate;

        await db.SaveChangesAsync();
        return Ok(ToDto(release));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var release = await db.Releases.FindAsync(id);
        if (release is null)
        {
            return NotFound();
        }

        db.Releases.Remove(release);
        await db.SaveChangesAsync();
        return NoContent();
    }

    private static ReleaseDto ToDto(Release r) =>
        new(r.Id, r.Name, r.Description, r.ReleaseType.ToString(), r.Status, r.TargetDate, r.CreatedAt, r.CreatedByUserId);

    private static ReleaseDetailDto ToDetailDto(Release r) =>
        new(
            r.Id, r.Name, r.Description, r.ReleaseType.ToString(), r.Status, r.TargetDate, r.CreatedAt, r.CreatedByUserId,
            r.Tasks.OrderByDescending(t => t.CreatedAt).Select(TasksController.ToDto).ToList(),
            r.ReleaseSystems.Select(rs => new ReleaseSystemDto(rs.Id, rs.ReleaseId, rs.SystemName, rs.Notes)).ToList(),
            r.FixVersions.Select(f => new FixVersionDto(f.Id, f.ReleaseId, f.Name, f.StartDate, f.EndDate, f.JiraFixVersionId)).ToList(),
            r.Comments.OrderByDescending(c => c.CreatedAt)
                .Select(c => new CommentDto(c.Id, c.ReleaseId, c.AuthorUserId, c.Body, c.CreatedAt))
                .ToList());
}

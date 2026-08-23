using rmp.Data;
using rmp.DTOs;
using rmp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace rmp.Controllers;

[ApiController]
[Route("api/tasks")]
[Authorize]
public class TasksController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int? releaseId)
    {
        var query = WithLookups(db.TaskItems);
        if (releaseId.HasValue)
        {
            query = query.Where(t => t.ReleaseId == releaseId.Value);
        }

        var tasks = await query.OrderByDescending(t => t.CreatedAt).ToListAsync();
        return Ok(tasks.Select(ToDto));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var task = await WithLookups(db.TaskItems).FirstOrDefaultAsync(t => t.Id == id);
        return task is null ? NotFound() : Ok(ToDto(task));
    }

    [HttpPost]
    public async Task<IActionResult> Create(TaskItemCreateDto dto)
    {
        if (!await db.Releases.AnyAsync(r => r.Id == dto.ReleaseId))
        {
            return BadRequest($"Release {dto.ReleaseId} does not exist.");
        }

        var task = new TaskItem
        {
            ReleaseId = dto.ReleaseId,
            Title = dto.Title,
            Description = dto.Description,
            AssigneeUserId = dto.AssigneeUserId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            TypeId = dto.TypeId,
            ComponentId = dto.ComponentId,
            AppNameId = dto.AppNameId,
            VersionId = dto.VersionId,
        };

        db.TaskItems.Add(task);
        await db.SaveChangesAsync();

        var created = await WithLookups(db.TaskItems).FirstAsync(t => t.Id == task.Id);
        return CreatedAtAction(nameof(Get), new { id = task.Id }, ToDto(created));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, TaskItemUpdateDto dto)
    {
        var task = await db.TaskItems.FindAsync(id);
        if (task is null)
        {
            return NotFound();
        }

        task.Title = dto.Title;
        task.Description = dto.Description;
        task.Status = dto.Status;
        task.AssigneeUserId = dto.AssigneeUserId;
        task.StartDate = dto.StartDate;
        task.EndDate = dto.EndDate;
        task.TypeId = dto.TypeId;
        task.ComponentId = dto.ComponentId;
        task.AppNameId = dto.AppNameId;
        task.VersionId = dto.VersionId;

        await db.SaveChangesAsync();

        var updated = await WithLookups(db.TaskItems).FirstAsync(t => t.Id == id);
        return Ok(ToDto(updated));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var task = await db.TaskItems.FindAsync(id);
        if (task is null)
        {
            return NotFound();
        }

        db.TaskItems.Remove(task);
        await db.SaveChangesAsync();
        return NoContent();
    }

    private static IQueryable<TaskItem> WithLookups(IQueryable<TaskItem> query) =>
        query.Include(t => t.Type).Include(t => t.Component).Include(t => t.AppName).Include(t => t.Version);

    internal static TaskItemDto ToDto(TaskItem t) => new(
        t.Id,
        $"TASK-{t.Id}",
        t.ReleaseId,
        t.Title,
        t.Description,
        t.Status,
        t.AssigneeUserId,
        t.StartDate,
        t.EndDate,
        t.CreatedAt,
        t.TypeId,
        t.Type?.Value,
        t.ComponentId,
        t.Component?.Value,
        t.AppNameId,
        t.AppName?.Value,
        t.VersionId,
        t.Version?.Value);
}

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
        var query = db.TaskItems.AsQueryable();
        if (releaseId.HasValue)
        {
            query = query.Where(t => t.ReleaseId == releaseId.Value);
        }

        var tasks = await query.OrderByDescending(t => t.CreatedAt).Select(t => ToDto(t)).ToListAsync();
        return Ok(tasks);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var task = await db.TaskItems.FindAsync(id);
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
            DueDate = dto.DueDate,
        };

        db.TaskItems.Add(task);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = task.Id }, ToDto(task));
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
        task.DueDate = dto.DueDate;

        await db.SaveChangesAsync();
        return Ok(ToDto(task));
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

    private static TaskItemDto ToDto(TaskItem t) =>
        new(t.Id, t.ReleaseId, t.Title, t.Description, t.Status, t.AssigneeUserId, t.DueDate, t.CreatedAt);
}

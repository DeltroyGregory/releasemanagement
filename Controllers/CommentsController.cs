using System.Security.Claims;
using rmp.Data;
using rmp.DTOs;
using rmp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace rmp.Controllers;

[ApiController]
[Route("api/comments")]
[Authorize]
public class CommentsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int? releaseId)
    {
        var query = db.Comments.AsQueryable();
        if (releaseId.HasValue)
        {
            query = query.Where(c => c.ReleaseId == releaseId.Value);
        }

        var items = await query.OrderByDescending(c => c.CreatedAt).Select(c => ToDto(c)).ToListAsync();
        return Ok(items);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CommentCreateDto dto)
    {
        if (!await db.Releases.AnyAsync(r => r.Id == dto.ReleaseId))
        {
            return BadRequest($"Release {dto.ReleaseId} does not exist.");
        }

        var authorUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        var entity = new Comment { ReleaseId = dto.ReleaseId, AuthorUserId = authorUserId, Body = dto.Body };

        db.Comments.Add(entity);
        await db.SaveChangesAsync();
        return Ok(ToDto(entity));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await db.Comments.FindAsync(id);
        if (entity is null)
        {
            return NotFound();
        }

        db.Comments.Remove(entity);
        await db.SaveChangesAsync();
        return NoContent();
    }

    private static CommentDto ToDto(Comment c) => new(c.Id, c.ReleaseId, c.AuthorUserId, c.Body, c.CreatedAt);
}

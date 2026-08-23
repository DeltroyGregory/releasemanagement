using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rmp.Data;
using rmp.DTOs;
using rmp.Models;

namespace rmp.Controllers;

[ApiController]
[Route("api/lookups")]
[Authorize]
public class LookupsController(AppDbContext db) : ControllerBase
{
    private static readonly HashSet<string> ValidCategories =
        [LookupItem.TaskType, LookupItem.Component, LookupItem.AppName, LookupItem.Version];

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] string? category)
    {
        if (category is not null && !ValidCategories.Contains(category))
        {
            return BadRequest($"Unknown category '{category}'.");
        }

        var query = db.Lookups.AsQueryable();
        if (category is not null)
        {
            query = query.Where(l => l.Category == category);
        }

        var items = await query.OrderBy(l => l.Category).ThenBy(l => l.Value).Select(l => ToDto(l)).ToListAsync();
        return Ok(items);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(LookupItemCreateDto dto)
    {
        if (!ValidCategories.Contains(dto.Category))
        {
            return BadRequest($"Unknown category '{dto.Category}'.");
        }

        if (await db.Lookups.AnyAsync(l => l.Category == dto.Category && l.Value == dto.Value))
        {
            return BadRequest($"'{dto.Value}' already exists in {dto.Category}.");
        }

        var item = new LookupItem { Category = dto.Category, Value = dto.Value };
        db.Lookups.Add(item);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(List), ToDto(item));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, LookupItemUpdateDto dto)
    {
        var item = await db.Lookups.FindAsync(id);
        if (item is null)
        {
            return NotFound();
        }

        if (await db.Lookups.AnyAsync(l => l.Id != id && l.Category == item.Category && l.Value == dto.Value))
        {
            return BadRequest($"'{dto.Value}' already exists in {item.Category}.");
        }

        item.Value = dto.Value;
        await db.SaveChangesAsync();
        return Ok(ToDto(item));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await db.Lookups.FindAsync(id);
        if (item is null)
        {
            return NotFound();
        }

        // No cascading FK from Tasks (SQL Server disallows more than one cascade path into the
        // same target table), so clear references here instead of the database doing it.
        await db.TaskItems.Where(t => t.TypeId == id).ExecuteUpdateAsync(s => s.SetProperty(t => t.TypeId, (int?)null));
        await db.TaskItems.Where(t => t.ComponentId == id).ExecuteUpdateAsync(s => s.SetProperty(t => t.ComponentId, (int?)null));
        await db.TaskItems.Where(t => t.AppNameId == id).ExecuteUpdateAsync(s => s.SetProperty(t => t.AppNameId, (int?)null));
        await db.TaskItems.Where(t => t.VersionId == id).ExecuteUpdateAsync(s => s.SetProperty(t => t.VersionId, (int?)null));

        db.Lookups.Remove(item);
        await db.SaveChangesAsync();
        return NoContent();
    }

    private static LookupItemDto ToDto(LookupItem l) => new(l.Id, l.Category, l.Value);
}

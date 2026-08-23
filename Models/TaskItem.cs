using System.ComponentModel.DataAnnotations.Schema;

namespace rmp.Models;

[Table("Tasks")]
public class TaskItem
{
    public int Id { get; set; }
    public int ReleaseId { get; set; }
    public Release? Release { get; set; }
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public string Status { get; set; } = "Open";
    public string? AssigneeUserId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int? TypeId { get; set; }
    public LookupItem? Type { get; set; }

    public int? ComponentId { get; set; }
    public LookupItem? Component { get; set; }

    public int? AppNameId { get; set; }
    public LookupItem? AppName { get; set; }

    public int? VersionId { get; set; }
    public LookupItem? Version { get; set; }
}

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
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

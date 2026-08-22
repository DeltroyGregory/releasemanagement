namespace rmp.Models;

public enum ReleaseType
{
    Major,
    Minor,
    Patch,
    Hotfix,
}

public class Release
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public ReleaseType ReleaseType { get; set; }
    public string Status { get; set; } = "Planned";
    public DateTime? TargetDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedByUserId { get; set; }

    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    public ICollection<ReleaseSystem> ReleaseSystems { get; set; } = new List<ReleaseSystem>();
    public ICollection<FixVersion> FixVersions { get; set; } = new List<FixVersion>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}

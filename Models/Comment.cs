namespace mbm.Models;

public class Comment
{
    public int Id { get; set; }
    public int ReleaseId { get; set; }
    public Release? Release { get; set; }
    public string AuthorUserId { get; set; } = "";
    public string Body { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

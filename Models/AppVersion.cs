namespace rmp.Models;

public class AppVersion
{
    public int Id { get; set; }
    public string SystemName { get; set; } = "";
    public string VersionLabel { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

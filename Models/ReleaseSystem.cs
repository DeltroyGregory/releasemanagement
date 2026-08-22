namespace rmp.Models;

public class ReleaseSystem
{
    public int Id { get; set; }
    public int ReleaseId { get; set; }
    public Release? Release { get; set; }
    public string SystemName { get; set; } = "";
    public string? Notes { get; set; }
}

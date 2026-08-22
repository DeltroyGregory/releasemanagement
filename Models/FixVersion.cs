namespace mbm.Models;

public class FixVersion
{
    public int Id { get; set; }
    public int ReleaseId { get; set; }
    public Release? Release { get; set; }
    public string Name { get; set; } = "";
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? JiraFixVersionId { get; set; }
}

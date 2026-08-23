namespace rmp.Models;

/// <summary>
/// A single admin-managed reference value (task type, component, app name, or version) — one
/// shared table with a Category discriminator instead of four near-identical tables, since they're
/// all just "a named value in a dropdown" with the same CRUD shape.
/// </summary>
public class LookupItem
{
    public const string TaskType = "TaskType";
    public const string Component = "Component";
    public const string AppName = "AppName";
    public const string Version = "Version";

    public int Id { get; set; }
    public string Category { get; set; } = "";
    public string Value { get; set; } = "";
}

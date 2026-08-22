namespace mbm.DTOs;

public record ReleaseDto(
    int Id,
    string Name,
    string? Description,
    string ReleaseType,
    string Status,
    DateTime? TargetDate,
    DateTime CreatedAt,
    string? CreatedByUserId);

public record ReleaseDetailDto(
    int Id,
    string Name,
    string? Description,
    string ReleaseType,
    string Status,
    DateTime? TargetDate,
    DateTime CreatedAt,
    string? CreatedByUserId,
    IReadOnlyList<TaskItemDto> Tasks,
    IReadOnlyList<ReleaseSystemDto> ReleaseSystems,
    IReadOnlyList<FixVersionDto> FixVersions,
    IReadOnlyList<CommentDto> Comments);

public record ReleaseCreateDto(string Name, string? Description, string ReleaseType, DateTime? TargetDate);

public record ReleaseUpdateDto(string Name, string? Description, string ReleaseType, string Status, DateTime? TargetDate);

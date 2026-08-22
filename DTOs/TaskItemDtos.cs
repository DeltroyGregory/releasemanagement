namespace mbm.DTOs;

public record TaskItemDto(
    int Id,
    int ReleaseId,
    string Title,
    string? Description,
    string Status,
    string? AssigneeUserId,
    DateTime? DueDate,
    DateTime CreatedAt);

public record TaskItemCreateDto(int ReleaseId, string Title, string? Description, string? AssigneeUserId, DateTime? DueDate);

public record TaskItemUpdateDto(string Title, string? Description, string Status, string? AssigneeUserId, DateTime? DueDate);

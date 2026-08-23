namespace rmp.DTOs;

public record TaskItemDto(
    int Id,
    string TaskNumber,
    int ReleaseId,
    string Title,
    string? Description,
    string Status,
    string? AssigneeUserId,
    DateTime? StartDate,
    DateTime? EndDate,
    DateTime CreatedAt,
    int? TypeId,
    string? TypeName,
    int? ComponentId,
    string? ComponentName,
    int? AppNameId,
    string? AppNameValue,
    int? VersionId,
    string? VersionValue);

public record TaskItemCreateDto(
    int ReleaseId,
    string Title,
    string? Description,
    string? AssigneeUserId,
    DateTime? StartDate,
    DateTime? EndDate,
    int? TypeId,
    int? ComponentId,
    int? AppNameId,
    int? VersionId);

public record TaskItemUpdateDto(
    string Title,
    string? Description,
    string Status,
    string? AssigneeUserId,
    DateTime? StartDate,
    DateTime? EndDate,
    int? TypeId,
    int? ComponentId,
    int? AppNameId,
    int? VersionId);

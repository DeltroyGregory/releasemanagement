namespace mbm.DTOs;

public record FixVersionDto(int Id, int ReleaseId, string Name, DateTime? StartDate, DateTime? EndDate, string? JiraFixVersionId);

public record FixVersionCreateDto(int ReleaseId, string Name, DateTime? StartDate, DateTime? EndDate);

public record FixVersionUpdateDto(string Name, DateTime? StartDate, DateTime? EndDate);

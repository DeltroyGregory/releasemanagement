namespace rmp.DTOs;

public record ReleaseSystemDto(int Id, int ReleaseId, string SystemName, string? Notes);

public record ReleaseSystemCreateDto(int ReleaseId, string SystemName, string? Notes);

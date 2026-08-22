namespace rmp.DTOs;

public record AppVersionDto(int Id, string SystemName, string VersionLabel, DateTime CreatedAt);

public record AppVersionCreateDto(string SystemName, string VersionLabel);

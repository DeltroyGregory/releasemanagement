namespace mbm.DTOs;

public record AuthMeDto(string? UserId, string? Email, string? PreferredUsername, IReadOnlyList<string> Roles);

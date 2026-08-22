namespace rmp.DTOs;

public record UserDto(
    string Id,
    string? Email,
    string? UserName,
    string? Role,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? LastLoginAt);

public record UserInviteDto(string Email, string Role);

public record UserRoleUpdateDto(string Role);

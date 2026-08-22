namespace rmp.DTOs;

public record PermissionKeyDto(string Key, string Area, string Label);

/// <summary>
/// Grants keyed by role name. Admin is included with every permission key (implicit, not stored)
/// so the frontend can render it as always-on without special-casing it.
/// </summary>
public record PermissionMatrixDto(
    IReadOnlyList<PermissionKeyDto> Permissions,
    IReadOnlyList<string> Roles,
    IReadOnlyDictionary<string, List<string>> Grants);

/// <summary>Full replacement of grants for the non-Admin roles named in the dictionary.</summary>
public record PermissionMatrixUpdateDto(IReadOnlyDictionary<string, List<string>> Grants);

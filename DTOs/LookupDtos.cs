namespace rmp.DTOs;

public record LookupItemDto(int Id, string Category, string Value);

public record LookupItemCreateDto(string Category, string Value);

public record LookupItemUpdateDto(string Value);

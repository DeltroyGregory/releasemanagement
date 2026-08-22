namespace mbm.DTOs;

public record CommentDto(int Id, int ReleaseId, string AuthorUserId, string Body, DateTime CreatedAt);

public record CommentCreateDto(int ReleaseId, string Body);

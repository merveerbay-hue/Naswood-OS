namespace Naswood.Modules.Platform.Contracts.Files;

public sealed class FileDto
{
    public required Guid Id { get; init; }

    public required string Number { get; init; }

    public required string Name { get; init; }

    public required string OriginalName { get; init; }

    public required string Extension { get; init; }

    public required string ContentType { get; init; }

    public required long SizeBytes { get; init; }

    public string? Checksum { get; init; }

    public required string Category { get; init; }

    public required string Module { get; init; }

    public string? RelatedEntityType { get; init; }

    public string? RelatedEntityId { get; init; }

    public required string CompanyId { get; init; }

    public string? PlantId { get; init; }

    public required int Version { get; init; }

    public required bool IsCurrentVersion { get; init; }

    public Guid? ParentFileId { get; init; }

    public required string Status { get; init; }

    public required string StorageKey { get; init; }

    public required bool PreviewAvailable { get; init; }

    public required IReadOnlyList<string> Tags { get; init; }

    public required DateTimeOffset UploadedAt { get; init; }

    public Guid? UploadedBy { get; init; }
}

public sealed class UpdateFileRequestDto
{
    public string? Name { get; init; }

    public string? Category { get; init; }

    public string? RelatedEntityType { get; init; }

    public string? RelatedEntityId { get; init; }

    public IReadOnlyList<string>? Tags { get; init; }
}

public sealed class PagedFilesDto
{
    public required IReadOnlyList<FileDto> Items { get; init; }

    public required int Page { get; init; }

    public required int PageSize { get; init; }

    public required int TotalCount { get; init; }

    public required int TotalPages { get; init; }
}

public sealed class BulkUploadResultDto
{
    public required IReadOnlyList<FileDto> Uploaded { get; init; }

    public required IReadOnlyList<BulkUploadFailureDto> Failed { get; init; }
}

public sealed class BulkUploadFailureDto
{
    public required string FileName { get; init; }

    public required string Message { get; init; }
}

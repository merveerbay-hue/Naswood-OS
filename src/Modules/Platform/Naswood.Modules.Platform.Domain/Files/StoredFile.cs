using Naswood.BuildingBlocks.Domain;

namespace Naswood.Modules.Platform.Domain.Files;

public enum FileStatus
{
    Uploaded = 0,
    Available = 1,
    Archived = 2,
    Deleted = 3
}

public static class FileErrors
{
    public static Error NotFound() =>
        Error.NotFound("FILE-001", "File was not found.");

    public static Error Validation(string message) =>
        Error.Validation("FILE-002", message);

    public static Error VirusDetected() =>
        Error.Validation("FILE-003", "Upload rejected by virus scan.");

    public static Error PreviewUnavailable() =>
        Error.Validation("FILE-004", "Preview is not available for this file type.");
}

public sealed record FileUploaded : DomainEventBase
{
    public required Guid FileId { get; init; }

    public required string StorageKey { get; init; }
}

public sealed record FileUpdated : DomainEventBase
{
    public required Guid FileId { get; init; }
}

public sealed record FileDeleted : DomainEventBase
{
    public required Guid FileId { get; init; }
}

public sealed record FileVersionCreated : DomainEventBase
{
    public required Guid FileId { get; init; }

    public required Guid PreviousFileId { get; init; }

    public required int Version { get; init; }
}

public sealed class StoredFile : AggregateRoot<Guid>
{
    private readonly List<string> _tags = [];

    private StoredFile()
    {
    }

    private StoredFile(
        Guid id,
        string number,
        string name,
        string originalName,
        string extension,
        string contentType,
        long sizeBytes,
        string? checksum,
        string category,
        string module,
        string? relatedEntityType,
        string? relatedEntityId,
        string companyId,
        string? plantId,
        int version,
        bool isCurrentVersion,
        Guid? parentFileId,
        string storageKey,
        Guid? uploadedBy)
        : base(id)
    {
        Number = number;
        Name = name;
        OriginalName = originalName;
        Extension = extension;
        ContentType = contentType;
        SizeBytes = sizeBytes;
        Checksum = checksum;
        Category = category;
        Module = module;
        RelatedEntityType = relatedEntityType;
        RelatedEntityId = relatedEntityId;
        CompanyId = companyId;
        PlantId = plantId;
        Version = version;
        IsCurrentVersion = isCurrentVersion;
        ParentFileId = parentFileId;
        StorageKey = storageKey;
        Status = FileStatus.Available;
        UploadedAt = DateTimeOffset.UtcNow;
        UploadedBy = uploadedBy;
        UpdatedAt = UploadedAt;
    }

    public string Number { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public string OriginalName { get; private set; } = string.Empty;

    public string Extension { get; private set; } = string.Empty;

    public string ContentType { get; private set; } = string.Empty;

    public long SizeBytes { get; private set; }

    public string? Checksum { get; private set; }

    public string Category { get; private set; } = string.Empty;

    public string Module { get; private set; } = string.Empty;

    public string? RelatedEntityType { get; private set; }

    public string? RelatedEntityId { get; private set; }

    public string CompanyId { get; private set; } = string.Empty;

    public string? PlantId { get; private set; }

    public int Version { get; private set; }

    public bool IsCurrentVersion { get; private set; }

    public Guid? ParentFileId { get; private set; }

    public FileStatus Status { get; private set; }

    public string StorageKey { get; private set; } = string.Empty;

    public string? ThumbnailStorageKey { get; private set; }

    public IReadOnlyList<string> Tags => _tags;

    public DateTimeOffset UploadedAt { get; private set; }

    public Guid? UploadedBy { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public Guid? UpdatedBy { get; private set; }

    public static StoredFile Create(
        string originalName,
        string contentType,
        long sizeBytes,
        string? checksum,
        string storageKey,
        string category,
        string module,
        string? relatedEntityType,
        string? relatedEntityId,
        string companyId,
        string? plantId,
        IEnumerable<string>? tags,
        Guid? uploadedBy)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(originalName);
        ArgumentException.ThrowIfNullOrWhiteSpace(contentType);
        ArgumentException.ThrowIfNullOrWhiteSpace(storageKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(companyId);

        if (sizeBytes < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(sizeBytes));
        }

        var id = UuidV7.NewGuid();
        var extension = Path.GetExtension(originalName).Trim().ToLowerInvariant();
        var safeName = Path.GetFileNameWithoutExtension(originalName).Trim();
        if (string.IsNullOrWhiteSpace(safeName))
        {
            safeName = "file";
        }

        var file = new StoredFile(
            id,
            number: $"FIL-{id.ToString("N")[..8].ToUpperInvariant()}",
            name: safeName,
            originalName: Path.GetFileName(originalName),
            extension,
            contentType.Trim(),
            sizeBytes,
            checksum,
            string.IsNullOrWhiteSpace(category) ? "General" : category.Trim(),
            string.IsNullOrWhiteSpace(module) ? "Platform" : module.Trim(),
            string.IsNullOrWhiteSpace(relatedEntityType) ? null : relatedEntityType.Trim(),
            string.IsNullOrWhiteSpace(relatedEntityId) ? null : relatedEntityId.Trim(),
            companyId.Trim().ToUpperInvariant(),
            string.IsNullOrWhiteSpace(plantId) ? null : plantId.Trim().ToUpperInvariant(),
            version: 1,
            isCurrentVersion: true,
            parentFileId: null,
            storageKey,
            uploadedBy);

        if (tags is not null)
        {
            foreach (var tag in tags.Where(t => !string.IsNullOrWhiteSpace(t)).Select(t => t.Trim()).Distinct(StringComparer.OrdinalIgnoreCase))
            {
                file._tags.Add(tag);
            }
        }

        file.RaiseDomainEvent(new FileUploaded { FileId = file.Id, StorageKey = file.StorageKey });
        return file;
    }

    public Result UpdateMetadata(
        string? name,
        string? category,
        string? relatedEntityType,
        string? relatedEntityId,
        IReadOnlyList<string>? tags,
        Guid? updatedBy,
        DateTimeOffset utcNow)
    {
        if (Status == FileStatus.Deleted)
        {
            return Result.Failure(FileErrors.NotFound());
        }

        if (!string.IsNullOrWhiteSpace(name))
        {
            Name = name.Trim();
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            Category = category.Trim();
        }

        RelatedEntityType = string.IsNullOrWhiteSpace(relatedEntityType) ? RelatedEntityType : relatedEntityType.Trim();
        RelatedEntityId = string.IsNullOrWhiteSpace(relatedEntityId) ? RelatedEntityId : relatedEntityId.Trim();

        if (tags is not null)
        {
            _tags.Clear();
            foreach (var tag in tags.Where(t => !string.IsNullOrWhiteSpace(t)).Select(t => t.Trim()).Distinct(StringComparer.OrdinalIgnoreCase))
            {
                _tags.Add(tag);
            }
        }

        UpdatedAt = utcNow;
        UpdatedBy = updatedBy;
        RaiseDomainEvent(new FileUpdated { FileId = Id });
        return Result.Success();
    }

    public Result SoftDelete(Guid? updatedBy, DateTimeOffset utcNow)
    {
        if (Status == FileStatus.Deleted)
        {
            return Result.Failure(FileErrors.NotFound());
        }

        Status = FileStatus.Deleted;
        IsCurrentVersion = false;
        UpdatedAt = utcNow;
        UpdatedBy = updatedBy;
        RaiseDomainEvent(new FileDeleted { FileId = Id });
        return Result.Success();
    }

    public void MarkSuperseded(DateTimeOffset utcNow)
    {
        IsCurrentVersion = false;
        UpdatedAt = utcNow;
    }

    public static StoredFile CreateVersion(
        StoredFile previous,
        string originalName,
        string contentType,
        long sizeBytes,
        string? checksum,
        string storageKey,
        Guid? uploadedBy)
    {
        var rootId = previous.ParentFileId ?? previous.Id;
        var id = UuidV7.NewGuid();
        var extension = Path.GetExtension(originalName).Trim().ToLowerInvariant();
        var safeName = Path.GetFileNameWithoutExtension(originalName).Trim();
        if (string.IsNullOrWhiteSpace(safeName))
        {
            safeName = previous.Name;
        }

        var file = new StoredFile(
            id,
            number: previous.Number,
            name: safeName,
            originalName: Path.GetFileName(originalName),
            extension,
            contentType.Trim(),
            sizeBytes,
            checksum,
            previous.Category,
            previous.Module,
            previous.RelatedEntityType,
            previous.RelatedEntityId,
            previous.CompanyId,
            previous.PlantId,
            version: previous.Version + 1,
            isCurrentVersion: true,
            parentFileId: rootId,
            storageKey,
            uploadedBy);

        foreach (var tag in previous.Tags)
        {
            file._tags.Add(tag);
        }

        file.RaiseDomainEvent(new FileVersionCreated
        {
            FileId = file.Id,
            PreviousFileId = previous.Id,
            Version = file.Version
        });
        return file;
    }

    public bool IsPreviewable()
    {
        if (ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return ContentType.StartsWith("text/", StringComparison.OrdinalIgnoreCase);
    }
}

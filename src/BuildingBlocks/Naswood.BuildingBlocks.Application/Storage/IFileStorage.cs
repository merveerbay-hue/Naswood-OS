namespace Naswood.BuildingBlocks.Application.Storage;

/// <summary>
/// Result of a successful upload to object storage.
/// </summary>
public sealed record FileStorageObject(
    string StorageKey,
    string ContentType,
    long SizeBytes,
    string? Checksum,
    Uri? Uri);

/// <summary>
/// Download payload returned by storage providers.
/// Caller owns disposing <see cref="Content"/>.
/// </summary>
public sealed record FileStorageDownload(
    Stream Content,
    string ContentType,
    long SizeBytes,
    string? FileName);

/// <summary>
/// Upload request for <see cref="IFileStorage"/>.
/// </summary>
public sealed record FileStorageUploadRequest(
    Stream Content,
    string FileName,
    string ContentType,
    string? Folder = null,
    IReadOnlyDictionary<string, string>? Metadata = null);

/// <summary>
/// Central file storage port. All modules must use this abstraction
/// (ADR-014). Do not call cloud SDKs from business modules directly.
/// </summary>
public interface IFileStorage
{
    Task<FileStorageObject> UploadAsync(
        FileStorageUploadRequest request,
        CancellationToken cancellationToken = default);

    Task<FileStorageDownload> DownloadAsync(
        string storageKey,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(string storageKey, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(string storageKey, CancellationToken cancellationToken = default);

    Task<Uri?> GetUriAsync(
        string storageKey,
        TimeSpan? lifetime = null,
        CancellationToken cancellationToken = default);
}

using Microsoft.Extensions.Options;
using Naswood.BuildingBlocks.Application.Storage;
using Naswood.BuildingBlocks.Domain;

namespace Naswood.BuildingBlocks.Infrastructure.Storage;

public sealed class FileStorageOptions
{
    public const string SectionName = "FileStorage";

    /// <summary>Local | S3 | AzureBlob</summary>
    public string Provider { get; set; } = "Local";

    public LocalFileStorageOptions Local { get; set; } = new();

    public S3FileStorageOptions S3 { get; set; } = new();

    public AzureBlobFileStorageOptions AzureBlob { get; set; } = new();
}

public sealed class LocalFileStorageOptions
{
    public string RootPath { get; set; } = "App_Data/files";
}

public sealed class S3FileStorageOptions
{
    public string BucketName { get; set; } = string.Empty;

    public string Region { get; set; } = string.Empty;

    public string? ServiceUrl { get; set; }

    public string? AccessKey { get; set; }

    public string? SecretKey { get; set; }
}

public sealed class AzureBlobFileStorageOptions
{
    public string ConnectionString { get; set; } = string.Empty;

    public string ContainerName { get; set; } = string.Empty;
}

/// <summary>
/// Development filesystem provider (ADR-014).
/// </summary>
public sealed class LocalFileStorageProvider : IFileStorage
{
    private readonly string _rootPath;

    public LocalFileStorageProvider(IOptions<FileStorageOptions> options)
    {
        _rootPath = Path.GetFullPath(options.Value.Local.RootPath);
        Directory.CreateDirectory(_rootPath);
    }

    public async Task<FileStorageObject> UploadAsync(
        FileStorageUploadRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request.Content);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.FileName);

        var extension = Path.GetExtension(request.FileName);
        var storageKey = string.IsNullOrWhiteSpace(request.Folder)
            ? $"{UuidV7.NewGuid():D}{extension}"
            : $"{request.Folder.Trim().Trim('/')}/{UuidV7.NewGuid():D}{extension}";

        var fullPath = GetFullPath(storageKey);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

        await using (var file = File.Create(fullPath))
        {
            await request.Content.CopyToAsync(file, cancellationToken).ConfigureAwait(false);
        }

        var info = new FileInfo(fullPath);
        return new FileStorageObject(
            storageKey,
            request.ContentType,
            info.Length,
            Checksum: null,
            Uri: new Uri(fullPath));
    }

    public Task<FileStorageDownload> DownloadAsync(
        string storageKey,
        CancellationToken cancellationToken = default)
    {
        var fullPath = GetFullPath(storageKey);
        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException("Stored file was not found.", storageKey);
        }

        Stream content = File.OpenRead(fullPath);
        var info = new FileInfo(fullPath);
        return Task.FromResult(new FileStorageDownload(
            content,
            ContentType: "application/octet-stream",
            info.Length,
            Path.GetFileName(storageKey)));
    }

    public Task DeleteAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        var fullPath = GetFullPath(storageKey);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string storageKey, CancellationToken cancellationToken = default) =>
        Task.FromResult(File.Exists(GetFullPath(storageKey)));

    public Task<Uri?> GetUriAsync(
        string storageKey,
        TimeSpan? lifetime = null,
        CancellationToken cancellationToken = default)
    {
        var fullPath = GetFullPath(storageKey);
        return Task.FromResult<Uri?>(File.Exists(fullPath) ? new Uri(fullPath) : null);
    }

    private string GetFullPath(string storageKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(storageKey);
        var combined = Path.GetFullPath(Path.Combine(_rootPath, storageKey.Replace('\\', '/')));
        if (!combined.StartsWith(_rootPath, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Invalid storage key path.");
        }

        return combined;
    }
}

/// <summary>
/// S3 / MinIO provider contract surface. Implementation deferred (ADR-014).
/// </summary>
public interface IS3FileStorageProvider : IFileStorage
{
}

/// <summary>
/// Placeholder registration until AWS/MinIO SDK wiring is available.
/// </summary>
public sealed class UnimplementedS3FileStorageProvider : IS3FileStorageProvider
{
    public Task<FileStorageObject> UploadAsync(
        FileStorageUploadRequest request,
        CancellationToken cancellationToken = default) =>
        throw CreateException();

    public Task<FileStorageDownload> DownloadAsync(
        string storageKey,
        CancellationToken cancellationToken = default) =>
        throw CreateException();

    public Task DeleteAsync(string storageKey, CancellationToken cancellationToken = default) =>
        throw CreateException();

    public Task<bool> ExistsAsync(string storageKey, CancellationToken cancellationToken = default) =>
        throw CreateException();

    public Task<Uri?> GetUriAsync(
        string storageKey,
        TimeSpan? lifetime = null,
        CancellationToken cancellationToken = default) =>
        throw CreateException();

    private static NotImplementedException CreateException() =>
        new("S3FileStorageProvider is not implemented yet. Use Local provider or complete ADR-014 S3 wiring.");
}

/// <summary>
/// Azure Blob provider contract surface. Implementation deferred (ADR-014).
/// </summary>
public interface IAzureBlobFileStorageProvider : IFileStorage
{
}

/// <summary>
/// Placeholder registration until Azure SDK wiring is available.
/// </summary>
public sealed class UnimplementedAzureBlobFileStorageProvider : IAzureBlobFileStorageProvider
{
    public Task<FileStorageObject> UploadAsync(
        FileStorageUploadRequest request,
        CancellationToken cancellationToken = default) =>
        throw CreateException();

    public Task<FileStorageDownload> DownloadAsync(
        string storageKey,
        CancellationToken cancellationToken = default) =>
        throw CreateException();

    public Task DeleteAsync(string storageKey, CancellationToken cancellationToken = default) =>
        throw CreateException();

    public Task<bool> ExistsAsync(string storageKey, CancellationToken cancellationToken = default) =>
        throw CreateException();

    public Task<Uri?> GetUriAsync(
        string storageKey,
        TimeSpan? lifetime = null,
        CancellationToken cancellationToken = default) =>
        throw CreateException();

    private static NotImplementedException CreateException() =>
        new("AzureBlobFileStorageProvider is not implemented yet. Use Local provider or complete ADR-014 Azure wiring.");
}

using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Application.Storage;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Platform.Application.Audit;
using Naswood.Modules.Platform.Application.Authentication;
using Naswood.Modules.Platform.Contracts.Files;
using Naswood.Modules.Platform.Domain.Files;

namespace Naswood.Modules.Platform.Application.Files;

public sealed class FileUploadOptions
{
    public const string SectionName = "FileUpload";

    public long MaxBytes { get; set; } = 25 * 1024 * 1024;

    public string[] AllowedExtensions { get; set; } =
    [
        ".pdf", ".png", ".jpg", ".jpeg", ".gif", ".webp",
        ".txt", ".csv", ".xlsx", ".docx", ".zip"
    ];
}

public interface IVirusScanner
{
    Task<Result> ScanAsync(Stream content, string fileName, CancellationToken cancellationToken = default);
}

public sealed class NoOpVirusScanner : IVirusScanner
{
    public Task<Result> ScanAsync(Stream content, string fileName, CancellationToken cancellationToken = default) =>
        Task.FromResult(Result.Success());
}

public sealed record FileSearchCriteria(
    string? Name,
    string? Category,
    string? Module,
    string? Extension,
    string? RelatedEntityType,
    string? RelatedEntityId,
    string? CompanyId,
    string? PlantId,
    string? Status,
    bool CurrentOnly,
    int Page,
    int PageSize);

public interface IFileRepository
{
    Task<StoredFile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(StoredFile file, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<StoredFile> Items, int TotalCount)> SearchAsync(
        FileSearchCriteria criteria,
        CancellationToken cancellationToken = default);
}

public sealed record SearchFilesQuery(
    string? Name,
    string? Category,
    string? Module,
    string? Extension,
    string? RelatedEntityType,
    string? RelatedEntityId,
    string? CompanyId,
    string? PlantId,
    string? Status,
    bool CurrentOnly,
    int Page,
    int PageSize) : IQuery<Result<PagedFilesDto>>;

public sealed record GetFileByIdQuery(Guid Id) : IQuery<Result<FileDto>>;

public sealed record UploadFileCommand(
    Stream Content,
    string FileName,
    string ContentType,
    string? Module,
    string? Category,
    string? RelatedEntityType,
    string? RelatedEntityId,
    string? CompanyId,
    string? PlantId,
    IReadOnlyList<string>? Tags) : ICommand<Result<FileDto>>;

public sealed record BulkUploadFilesCommand(
    IReadOnlyList<UploadFileCommand> Files) : ICommand<Result<BulkUploadResultDto>>;

public sealed record UpdateFileCommand(
    Guid Id,
    string? Name,
    string? Category,
    string? RelatedEntityType,
    string? RelatedEntityId,
    IReadOnlyList<string>? Tags) : ICommand<Result<FileDto>>;

public sealed record DeleteFileCommand(Guid Id) : ICommand<Result>;

public sealed record CreateFileVersionCommand(
    Guid Id,
    Stream Content,
    string FileName,
    string ContentType) : ICommand<Result<FileDto>>;

public sealed record DownloadFileQuery(Guid Id) : IQuery<Result<FileStorageDownload>>;

public sealed record PreviewFileQuery(Guid Id) : IQuery<Result<FileStorageDownload>>;

public static class FileDtoMapper
{
    public static FileDto ToDto(StoredFile file) => new()
    {
        Id = file.Id,
        Number = file.Number,
        Name = file.Name,
        OriginalName = file.OriginalName,
        Extension = file.Extension,
        ContentType = file.ContentType,
        SizeBytes = file.SizeBytes,
        Checksum = file.Checksum,
        Category = file.Category,
        Module = file.Module,
        RelatedEntityType = file.RelatedEntityType,
        RelatedEntityId = file.RelatedEntityId,
        CompanyId = file.CompanyId,
        PlantId = file.PlantId,
        Version = file.Version,
        IsCurrentVersion = file.IsCurrentVersion,
        ParentFileId = file.ParentFileId,
        Status = file.Status.ToString(),
        StorageKey = file.StorageKey,
        PreviewAvailable = file.IsPreviewable(),
        Tags = file.Tags.ToArray(),
        UploadedAt = file.UploadedAt,
        UploadedBy = file.UploadedBy
    };
}

public static class FileUploadValidator
{
    public static Result Validate(
        string fileName,
        string contentType,
        long sizeBytes,
        FileUploadOptions options)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return Result.Failure(FileErrors.Validation("File name is required."));
        }

        if (sizeBytes <= 0)
        {
            return Result.Failure(FileErrors.Validation("File is empty."));
        }

        if (sizeBytes > options.MaxBytes)
        {
            return Result.Failure(FileErrors.Validation($"File exceeds maximum size of {options.MaxBytes} bytes."));
        }

        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(extension) ||
            !options.AllowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
        {
            return Result.Failure(FileErrors.Validation($"File extension '{extension}' is not allowed."));
        }

        if (string.IsNullOrWhiteSpace(contentType))
        {
            return Result.Failure(FileErrors.Validation("Content type is required."));
        }

        return Result.Success();
    }
}

public static class FileChecksum
{
    public static async Task<(MemoryStream Buffer, string Hash)> BufferAndHashAsync(
        Stream content,
        CancellationToken cancellationToken)
    {
        var buffer = new MemoryStream();
        await content.CopyToAsync(buffer, cancellationToken).ConfigureAwait(false);
        buffer.Position = 0;
        var hashBytes = await SHA256.HashDataAsync(buffer, cancellationToken).ConfigureAwait(false);
        buffer.Position = 0;
        return (buffer, Convert.ToHexString(hashBytes).ToLowerInvariant());
    }
}

public sealed class SearchFilesQueryHandler : IQueryHandler<SearchFilesQuery, Result<PagedFilesDto>>
{
    private readonly IFileRepository _files;

    public SearchFilesQueryHandler(IFileRepository files) => _files = files;

    public async Task<Result<PagedFilesDto>> HandleAsync(
        SearchFilesQuery query,
        CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _files.SearchAsync(
                new FileSearchCriteria(
                    query.Name,
                    query.Category,
                    query.Module,
                    query.Extension,
                    query.RelatedEntityType,
                    query.RelatedEntityId,
                    query.CompanyId,
                    query.PlantId,
                    query.Status,
                    query.CurrentOnly,
                    page,
                    pageSize),
                cancellationToken)
            .ConfigureAwait(false);

        return Result.Success(new PagedFilesDto
        {
            Items = items.Select(FileDtoMapper.ToDto).ToArray(),
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class GetFileByIdQueryHandler : IQueryHandler<GetFileByIdQuery, Result<FileDto>>
{
    private readonly IFileRepository _files;

    public GetFileByIdQueryHandler(IFileRepository files) => _files = files;

    public async Task<Result<FileDto>> HandleAsync(
        GetFileByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var file = await _files.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);
        if (file is null || file.Status == FileStatus.Deleted)
        {
            return Result.Failure<FileDto>(FileErrors.NotFound());
        }

        return Result.Success(FileDtoMapper.ToDto(file));
    }
}

public sealed class UploadFileCommandHandler : ICommandHandler<UploadFileCommand, Result<FileDto>>
{
    private readonly IFileRepository _files;
    private readonly IFileStorage _storage;
    private readonly IVirusScanner _virusScanner;
    private readonly IPlatformUnitOfWork _unitOfWork;
    private readonly IOutboxWriter _outbox;
    private readonly IAuditWriter _audit;
    private readonly IAuthRequestContext _context;
    private readonly IClock _clock;
    private readonly FileUploadOptions _options;

    public UploadFileCommandHandler(
        IFileRepository files,
        IFileStorage storage,
        IVirusScanner virusScanner,
        IPlatformUnitOfWork unitOfWork,
        IOutboxWriter outbox,
        IAuditWriter audit,
        IAuthRequestContext context,
        IClock clock,
        IOptions<FileUploadOptions> options)
    {
        _files = files;
        _storage = storage;
        _virusScanner = virusScanner;
        _unitOfWork = unitOfWork;
        _outbox = outbox;
        _audit = audit;
        _context = context;
        _clock = clock;
        _options = options.Value;
    }

    public async Task<Result<FileDto>> HandleAsync(
        UploadFileCommand command,
        CancellationToken cancellationToken = default)
    {
        var (buffered, hash) = await FileChecksum.BufferAndHashAsync(command.Content, cancellationToken)
            .ConfigureAwait(false);
        await using (buffered)
        {
            var validation = FileUploadValidator.Validate(
                command.FileName,
                command.ContentType,
                buffered.Length,
                _options);
            if (validation.IsFailure)
            {
                return Result.Failure<FileDto>(validation.Error!);
            }

            var scan = await _virusScanner.ScanAsync(buffered, command.FileName, cancellationToken)
                .ConfigureAwait(false);
            buffered.Position = 0;
            if (scan.IsFailure)
            {
                return Result.Failure<FileDto>(FileErrors.VirusDetected());
            }

            var companyId = string.IsNullOrWhiteSpace(command.CompanyId)
                ? _context.CompanyId ?? "COMP-001"
                : command.CompanyId;
            var plantId = string.IsNullOrWhiteSpace(command.PlantId) ? _context.PlantId : command.PlantId;
            var module = string.IsNullOrWhiteSpace(command.Module) ? "Platform" : command.Module;
            var folder = $"{companyId}/{module}/{_clock.UtcNow:yyyy}/{_clock.UtcNow:MM}";

            var stored = await _storage.UploadAsync(
                    new FileStorageUploadRequest(buffered, command.FileName, command.ContentType, folder),
                    cancellationToken)
                .ConfigureAwait(false);

            var file = StoredFile.Create(
                command.FileName,
                command.ContentType,
                stored.SizeBytes,
                hash,
                stored.StorageKey,
                command.Category ?? "General",
                module,
                command.RelatedEntityType,
                command.RelatedEntityId,
                companyId,
                plantId,
                command.Tags,
                _context.UserId);

            await _files.AddAsync(file, cancellationToken).ConfigureAwait(false);
            await PersistAsync(file, "FileUploaded", cancellationToken).ConfigureAwait(false);
            return Result.Success(FileDtoMapper.ToDto(file));
        }
    }

    private async Task PersistAsync(StoredFile file, string action, CancellationToken cancellationToken)
    {
        await _audit.WriteAsync(
                new Domain.Audit.AuditWriteModel
                {
                    OccurredAt = _clock.UtcNow,
                    UserId = _context.UserId,
                    Module = "Administration",
                    Entity = "File",
                    EntityId = file.Id.ToString("D"),
                    Action = action,
                    NewValuesJson = JsonSerializer.Serialize(new
                    {
                        file.Number,
                        file.OriginalName,
                        file.StorageKey,
                        file.SizeBytes
                    }),
                    CorrelationId = _context.CorrelationId,
                    CompanyId = _context.CompanyId,
                    PlantId = _context.PlantId,
                    IpAddress = _context.IpAddress,
                    SessionId = _context.SessionId
                },
                cancellationToken)
            .ConfigureAwait(false);

        foreach (var domainEvent in file.DomainEvents)
        {
            await _outbox.EnqueueAsync(
                    domainEvent.GetType().Name,
                    domainEvent,
                    _context.UserId,
                    _context.CorrelationId,
                    _clock.UtcNow,
                    cancellationToken)
                .ConfigureAwait(false);
        }

        file.ClearDomainEvents();
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}

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

public sealed class BulkUploadFilesCommandHandler
    : ICommandHandler<BulkUploadFilesCommand, Result<BulkUploadResultDto>>
{
    private readonly IDispatcher _dispatcher;

    public BulkUploadFilesCommandHandler(IDispatcher dispatcher) => _dispatcher = dispatcher;

    public async Task<Result<BulkUploadResultDto>> HandleAsync(
        BulkUploadFilesCommand command,
        CancellationToken cancellationToken = default)
    {
        var uploaded = new List<FileDto>();
        var failed = new List<BulkUploadFailureDto>();

        foreach (var file in command.Files)
        {
            var result = await _dispatcher.SendAsync(file, cancellationToken).ConfigureAwait(false);
            if (result.IsSuccess)
            {
                uploaded.Add(result.Value);
            }
            else
            {
                failed.Add(new BulkUploadFailureDto
                {
                    FileName = file.FileName,
                    Message = result.Error?.Message ?? "Upload failed."
                });
            }
        }

        return Result.Success(new BulkUploadResultDto
        {
            Uploaded = uploaded,
            Failed = failed
        });
    }
}

public sealed class UpdateFileCommandHandler : ICommandHandler<UpdateFileCommand, Result<FileDto>>
{
    private readonly IFileRepository _files;
    private readonly IPlatformUnitOfWork _unitOfWork;
    private readonly IOutboxWriter _outbox;
    private readonly IAuditWriter _audit;
    private readonly IAuthRequestContext _context;
    private readonly IClock _clock;

    public UpdateFileCommandHandler(
        IFileRepository files,
        IPlatformUnitOfWork unitOfWork,
        IOutboxWriter outbox,
        IAuditWriter audit,
        IAuthRequestContext context,
        IClock clock)
    {
        _files = files;
        _unitOfWork = unitOfWork;
        _outbox = outbox;
        _audit = audit;
        _context = context;
        _clock = clock;
    }

    public async Task<Result<FileDto>> HandleAsync(
        UpdateFileCommand command,
        CancellationToken cancellationToken = default)
    {
        var file = await _files.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (file is null)
        {
            return Result.Failure<FileDto>(FileErrors.NotFound());
        }

        var update = file.UpdateMetadata(
            command.Name,
            command.Category,
            command.RelatedEntityType,
            command.RelatedEntityId,
            command.Tags,
            _context.UserId,
            _clock.UtcNow);
        if (update.IsFailure)
        {
            return Result.Failure<FileDto>(update.Error!);
        }

        await PersistAsync(file, "FileUpdated", cancellationToken).ConfigureAwait(false);
        return Result.Success(FileDtoMapper.ToDto(file));
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
                    NewValuesJson = JsonSerializer.Serialize(new { file.Name, file.Category, file.Tags }),
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

public sealed class DeleteFileCommandHandler : ICommandHandler<DeleteFileCommand, Result>
{
    private readonly IFileRepository _files;
    private readonly IFileStorage _storage;
    private readonly IPlatformUnitOfWork _unitOfWork;
    private readonly IOutboxWriter _outbox;
    private readonly IAuditWriter _audit;
    private readonly IAuthRequestContext _context;
    private readonly IClock _clock;

    public DeleteFileCommandHandler(
        IFileRepository files,
        IFileStorage storage,
        IPlatformUnitOfWork unitOfWork,
        IOutboxWriter outbox,
        IAuditWriter audit,
        IAuthRequestContext context,
        IClock clock)
    {
        _files = files;
        _storage = storage;
        _unitOfWork = unitOfWork;
        _outbox = outbox;
        _audit = audit;
        _context = context;
        _clock = clock;
    }

    public async Task<Result> HandleAsync(
        DeleteFileCommand command,
        CancellationToken cancellationToken = default)
    {
        var file = await _files.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (file is null)
        {
            return Result.Failure(FileErrors.NotFound());
        }

        var deleted = file.SoftDelete(_context.UserId, _clock.UtcNow);
        if (deleted.IsFailure)
        {
            return deleted;
        }

        await _storage.DeleteAsync(file.StorageKey, cancellationToken).ConfigureAwait(false);

        await _audit.WriteAsync(
                new Domain.Audit.AuditWriteModel
                {
                    OccurredAt = _clock.UtcNow,
                    UserId = _context.UserId,
                    Module = "Administration",
                    Entity = "File",
                    EntityId = file.Id.ToString("D"),
                    Action = "FileDeleted",
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
        return Result.Success();
    }
}

public sealed class CreateFileVersionCommandHandler
    : ICommandHandler<CreateFileVersionCommand, Result<FileDto>>
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

    public CreateFileVersionCommandHandler(
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
        CreateFileVersionCommand command,
        CancellationToken cancellationToken = default)
    {
        var previous = await _files.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
        if (previous is null || previous.Status == FileStatus.Deleted || !previous.IsCurrentVersion)
        {
            return Result.Failure<FileDto>(FileErrors.NotFound());
        }

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

            var folder =
                $"{previous.CompanyId}/{previous.Module}/{_clock.UtcNow:yyyy}/{_clock.UtcNow:MM}";
            var stored = await _storage.UploadAsync(
                    new FileStorageUploadRequest(buffered, command.FileName, command.ContentType, folder),
                    cancellationToken)
                .ConfigureAwait(false);

            previous.MarkSuperseded(_clock.UtcNow);
            var next = StoredFile.CreateVersion(
                previous,
                command.FileName,
                command.ContentType,
                stored.SizeBytes,
                hash,
                stored.StorageKey,
                _context.UserId);

            await _files.AddAsync(next, cancellationToken).ConfigureAwait(false);

            await _audit.WriteAsync(
                    new Domain.Audit.AuditWriteModel
                    {
                        OccurredAt = _clock.UtcNow,
                        UserId = _context.UserId,
                        Module = "Administration",
                        Entity = "File",
                        EntityId = next.Id.ToString("D"),
                        Action = "FileVersionCreated",
                        NewValuesJson = JsonSerializer.Serialize(new
                        {
                            next.Number,
                            next.Version,
                            PreviousId = previous.Id
                        }),
                        CorrelationId = _context.CorrelationId,
                        CompanyId = _context.CompanyId,
                        PlantId = _context.PlantId,
                        IpAddress = _context.IpAddress,
                        SessionId = _context.SessionId
                    },
                    cancellationToken)
                .ConfigureAwait(false);

            foreach (var domainEvent in next.DomainEvents)
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

            next.ClearDomainEvents();
            await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return Result.Success(FileDtoMapper.ToDto(next));
        }
    }
}

public sealed class DownloadFileQueryHandler : IQueryHandler<DownloadFileQuery, Result<FileStorageDownload>>
{
    private readonly IFileRepository _files;
    private readonly IFileStorage _storage;

    public DownloadFileQueryHandler(IFileRepository files, IFileStorage storage)
    {
        _files = files;
        _storage = storage;
    }

    public async Task<Result<FileStorageDownload>> HandleAsync(
        DownloadFileQuery query,
        CancellationToken cancellationToken = default)
    {
        var file = await _files.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);
        if (file is null || file.Status == FileStatus.Deleted)
        {
            return Result.Failure<FileStorageDownload>(FileErrors.NotFound());
        }

        var download = await _storage.DownloadAsync(file.StorageKey, cancellationToken).ConfigureAwait(false);
        return Result.Success(new FileStorageDownload(
            download.Content,
            file.ContentType,
            file.SizeBytes,
            file.OriginalName));
    }
}

public sealed class PreviewFileQueryHandler : IQueryHandler<PreviewFileQuery, Result<FileStorageDownload>>
{
    private readonly IFileRepository _files;
    private readonly IFileStorage _storage;

    public PreviewFileQueryHandler(IFileRepository files, IFileStorage storage)
    {
        _files = files;
        _storage = storage;
    }

    public async Task<Result<FileStorageDownload>> HandleAsync(
        PreviewFileQuery query,
        CancellationToken cancellationToken = default)
    {
        var file = await _files.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);
        if (file is null || file.Status == FileStatus.Deleted)
        {
            return Result.Failure<FileStorageDownload>(FileErrors.NotFound());
        }

        if (!file.IsPreviewable())
        {
            return Result.Failure<FileStorageDownload>(FileErrors.PreviewUnavailable());
        }

        var download = await _storage.DownloadAsync(file.StorageKey, cancellationToken).ConfigureAwait(false);
        return Result.Success(new FileStorageDownload(
            download.Content,
            file.ContentType,
            file.SizeBytes,
            file.OriginalName));
    }
}

using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Platform.Contracts.Audit;
using Naswood.Modules.Platform.Domain.Audit;

namespace Naswood.Modules.Platform.Application.Audit;

public sealed record AuditSearchCriteria(
    string? Module,
    string? Entity,
    string? EntityId,
    string? Action,
    Guid? UserId,
    Guid? SessionId,
    string? CompanyId,
    string? PlantId,
    DateTimeOffset? From,
    DateTimeOffset? To,
    int Page,
    int PageSize);

public interface IAuditWriter
{
    Task WriteAsync(AuditWriteModel model, CancellationToken cancellationToken = default);
}

public interface IAuditQueryRepository
{
    Task<AuditLogEntry?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<AuditLogEntry> Items, int TotalCount)> SearchAsync(
        AuditSearchCriteria criteria,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AuditLogEntry>> ListForExportAsync(
        AuditSearchCriteria criteria,
        CancellationToken cancellationToken = default);
}

public sealed record SearchAuditLogsQuery(
    string? Module,
    string? Entity,
    string? EntityId,
    string? Action,
    Guid? UserId,
    Guid? SessionId,
    string? CompanyId,
    string? PlantId,
    DateTimeOffset? From,
    DateTimeOffset? To,
    int Page,
    int PageSize) : IQuery<Result<PagedAuditLogsDto>>;

public sealed record GetAuditLogByIdQuery(Guid Id) : IQuery<Result<AuditLogDto>>;

public sealed record GetAuditByEntityQuery(
    string EntityId,
    string? Entity,
    int Page,
    int PageSize) : IQuery<Result<PagedAuditLogsDto>>;

public sealed record ExportAuditLogsQuery(
    string? Module,
    string? Entity,
    string? EntityId,
    string? Action,
    Guid? UserId,
    DateTimeOffset? From,
    DateTimeOffset? To) : IQuery<Result<string>>;

public static class AuditDtoMapper
{
    public static AuditLogDto ToDto(AuditLogEntry entry) => new()
    {
        Id = entry.Id,
        OccurredAt = entry.OccurredAt,
        UserId = entry.UserId,
        Username = entry.Username,
        Module = entry.Module,
        Entity = entry.Entity,
        EntityId = entry.EntityId,
        Action = entry.Action,
        OldValuesJson = entry.OldValuesJson,
        NewValuesJson = entry.NewValuesJson,
        IpAddress = entry.IpAddress,
        SessionId = entry.SessionId,
        CorrelationId = entry.CorrelationId,
        CompanyId = entry.CompanyId,
        PlantId = entry.PlantId,
        Severity = entry.Severity,
        Status = entry.Status
    };
}

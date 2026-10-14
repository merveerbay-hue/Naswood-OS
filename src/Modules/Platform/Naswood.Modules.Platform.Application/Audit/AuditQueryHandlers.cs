using System.Text;
using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Platform.Contracts.Audit;
using Naswood.Modules.Platform.Domain.Audit;

namespace Naswood.Modules.Platform.Application.Audit;

public sealed class SearchAuditLogsQueryHandler : IQueryHandler<SearchAuditLogsQuery, Result<PagedAuditLogsDto>>
{
    private readonly IAuditQueryRepository _audit;

    public SearchAuditLogsQueryHandler(IAuditQueryRepository audit) => _audit = audit;

    public async Task<Result<PagedAuditLogsDto>> HandleAsync(
        SearchAuditLogsQuery query,
        CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _audit.SearchAsync(
                new AuditSearchCriteria(
                    query.Module,
                    query.Entity,
                    query.EntityId,
                    query.Action,
                    query.UserId,
                    query.SessionId,
                    query.CompanyId,
                    query.PlantId,
                    query.From,
                    query.To,
                    page,
                    pageSize),
                cancellationToken)
            .ConfigureAwait(false);

        return Result.Success(new PagedAuditLogsDto
        {
            Items = items.Select(AuditDtoMapper.ToDto).ToArray(),
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class GetAuditLogByIdQueryHandler : IQueryHandler<GetAuditLogByIdQuery, Result<AuditLogDto>>
{
    private readonly IAuditQueryRepository _audit;

    public GetAuditLogByIdQueryHandler(IAuditQueryRepository audit) => _audit = audit;

    public async Task<Result<AuditLogDto>> HandleAsync(
        GetAuditLogByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var entry = await _audit.GetByIdAsync(query.Id, cancellationToken).ConfigureAwait(false);
        if (entry is null)
        {
            return Result.Failure<AuditLogDto>(AuditErrors.NotFound());
        }

        return Result.Success(AuditDtoMapper.ToDto(entry));
    }
}

public sealed class GetAuditByEntityQueryHandler : IQueryHandler<GetAuditByEntityQuery, Result<PagedAuditLogsDto>>
{
    private readonly IAuditQueryRepository _audit;

    public GetAuditByEntityQueryHandler(IAuditQueryRepository audit) => _audit = audit;

    public async Task<Result<PagedAuditLogsDto>> HandleAsync(
        GetAuditByEntityQuery query,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query.EntityId))
        {
            return Result.Failure<PagedAuditLogsDto>(AuditErrors.Validation("Entity id is required."));
        }

        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var (items, total) = await _audit.SearchAsync(
                new AuditSearchCriteria(
                    null,
                    query.Entity,
                    query.EntityId,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    page,
                    pageSize),
                cancellationToken)
            .ConfigureAwait(false);

        return Result.Success(new PagedAuditLogsDto
        {
            Items = items.Select(AuditDtoMapper.ToDto).ToArray(),
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
            TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}

public sealed class ExportAuditLogsQueryHandler : IQueryHandler<ExportAuditLogsQuery, Result<string>>
{
    private readonly IAuditQueryRepository _audit;

    public ExportAuditLogsQueryHandler(IAuditQueryRepository audit) => _audit = audit;

    public async Task<Result<string>> HandleAsync(
        ExportAuditLogsQuery query,
        CancellationToken cancellationToken = default)
    {
        var items = await _audit.ListForExportAsync(
                new AuditSearchCriteria(
                    query.Module,
                    query.Entity,
                    query.EntityId,
                    query.Action,
                    query.UserId,
                    null,
                    null,
                    null,
                    query.From,
                    query.To,
                    1,
                    10_000),
                cancellationToken)
            .ConfigureAwait(false);

        var sb = new StringBuilder();
        sb.AppendLine("id,occurredAt,userId,username,module,entity,entityId,action,companyId,plantId,correlationId,severity,status");
        foreach (var item in items)
        {
            sb.Append(item.Id);
            sb.Append(',');
            sb.Append(item.OccurredAt.ToString("O"));
            sb.Append(',');
            sb.Append(item.UserId);
            sb.Append(',');
            sb.Append(Escape(item.Username));
            sb.Append(',');
            sb.Append(Escape(item.Module));
            sb.Append(',');
            sb.Append(Escape(item.Entity));
            sb.Append(',');
            sb.Append(Escape(item.EntityId));
            sb.Append(',');
            sb.Append(Escape(item.Action));
            sb.Append(',');
            sb.Append(Escape(item.CompanyId));
            sb.Append(',');
            sb.Append(Escape(item.PlantId));
            sb.Append(',');
            sb.Append(Escape(item.CorrelationId));
            sb.Append(',');
            sb.Append(Escape(item.Severity));
            sb.Append(',');
            sb.Append(Escape(item.Status));
            sb.AppendLine();
        }

        return Result.Success(sb.ToString());
    }

    private static string Escape(string? value)
    {
        var text = value ?? string.Empty;
        if (text.Contains(',') || text.Contains('"'))
        {
            return $"\"{text.Replace("\"", "\"\"", StringComparison.Ordinal)}\"";
        }

        return text;
    }
}

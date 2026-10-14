using Microsoft.EntityFrameworkCore;
using Naswood.Modules.Platform.Application.Audit;
using Naswood.Modules.Platform.Domain.Audit;
using Naswood.Modules.Platform.Infrastructure.Persistence;

namespace Naswood.Modules.Platform.Infrastructure.Audit;

public sealed class AuditLogRepository : IAuditWriter, IAuditQueryRepository
{
    private readonly PlatformDbContext _db;

    public AuditLogRepository(PlatformDbContext db) => _db = db;

    public async Task WriteAsync(AuditWriteModel model, CancellationToken cancellationToken = default)
    {
        var entry = AuditLogEntry.Create(model);
        await _db.AuditLogs.AddAsync(entry, cancellationToken).ConfigureAwait(false);
    }

    public Task<AuditLogEntry?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _db.AuditLogs.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<(IReadOnlyList<AuditLogEntry> Items, int TotalCount)> SearchAsync(
        AuditSearchCriteria criteria,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilters(_db.AuditLogs.AsNoTracking(), criteria);
        var total = await query.CountAsync(cancellationToken).ConfigureAwait(false);
        var items = await query
            .OrderByDescending(x => x.OccurredAt)
            .Skip((criteria.Page - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
        return (items, total);
    }

    public async Task<IReadOnlyList<AuditLogEntry>> ListForExportAsync(
        AuditSearchCriteria criteria,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilters(_db.AuditLogs.AsNoTracking(), criteria);
        return await query
            .OrderByDescending(x => x.OccurredAt)
            .Take(10_000)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    private static IQueryable<AuditLogEntry> ApplyFilters(
        IQueryable<AuditLogEntry> query,
        AuditSearchCriteria criteria)
    {
        if (!string.IsNullOrWhiteSpace(criteria.Module))
        {
            var value = criteria.Module.Trim();
            query = query.Where(x => EF.Functions.ILike(x.Module, value));
        }

        if (!string.IsNullOrWhiteSpace(criteria.Entity))
        {
            var value = criteria.Entity.Trim();
            query = query.Where(x => x.Entity != null && EF.Functions.ILike(x.Entity, value));
        }

        if (!string.IsNullOrWhiteSpace(criteria.EntityId))
        {
            var value = criteria.EntityId.Trim();
            query = query.Where(x => x.EntityId == value);
        }

        if (!string.IsNullOrWhiteSpace(criteria.Action))
        {
            var value = criteria.Action.Trim();
            query = query.Where(x => EF.Functions.ILike(x.Action, $"%{value}%"));
        }

        if (criteria.UserId is not null)
        {
            query = query.Where(x => x.UserId == criteria.UserId);
        }

        if (criteria.SessionId is not null)
        {
            query = query.Where(x => x.SessionId == criteria.SessionId);
        }

        if (!string.IsNullOrWhiteSpace(criteria.CompanyId))
        {
            var value = criteria.CompanyId.Trim();
            query = query.Where(x => x.CompanyId == value);
        }

        if (!string.IsNullOrWhiteSpace(criteria.PlantId))
        {
            var value = criteria.PlantId.Trim();
            query = query.Where(x => x.PlantId == value);
        }

        if (criteria.From is not null)
        {
            query = query.Where(x => x.OccurredAt >= criteria.From);
        }

        if (criteria.To is not null)
        {
            query = query.Where(x => x.OccurredAt <= criteria.To);
        }

        return query;
    }
}

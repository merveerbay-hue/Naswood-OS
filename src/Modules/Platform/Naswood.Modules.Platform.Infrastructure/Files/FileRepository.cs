using Microsoft.EntityFrameworkCore;
using Naswood.Modules.Platform.Application.Files;
using Naswood.Modules.Platform.Domain.Files;
using Naswood.Modules.Platform.Infrastructure.Persistence;

namespace Naswood.Modules.Platform.Infrastructure.Files;

public sealed class FileRepository : IFileRepository
{
    private readonly PlatformDbContext _db;

    public FileRepository(PlatformDbContext db) => _db = db;

    public Task<StoredFile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _db.Files.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task AddAsync(StoredFile file, CancellationToken cancellationToken = default) =>
        await _db.Files.AddAsync(file, cancellationToken).ConfigureAwait(false);

    public async Task<(IReadOnlyList<StoredFile> Items, int TotalCount)> SearchAsync(
        FileSearchCriteria criteria,
        CancellationToken cancellationToken = default)
    {
        var query = _db.Files.AsNoTracking().AsQueryable();

        if (criteria.CurrentOnly)
        {
            query = query.Where(x => x.IsCurrentVersion);
        }

        if (!string.IsNullOrWhiteSpace(criteria.Status) &&
            Enum.TryParse<FileStatus>(criteria.Status, true, out var status))
        {
            query = query.Where(x => x.Status == status);
        }
        else
        {
            query = query.Where(x => x.Status != FileStatus.Deleted);
        }

        if (!string.IsNullOrWhiteSpace(criteria.Name))
        {
            var value = criteria.Name.Trim();
            query = query.Where(x =>
                EF.Functions.ILike(x.Name, $"%{value}%") ||
                EF.Functions.ILike(x.OriginalName, $"%{value}%") ||
                EF.Functions.ILike(x.Number, $"%{value}%"));
        }

        if (!string.IsNullOrWhiteSpace(criteria.Category))
        {
            var value = criteria.Category.Trim();
            query = query.Where(x => EF.Functions.ILike(x.Category, value));
        }

        if (!string.IsNullOrWhiteSpace(criteria.Module))
        {
            var value = criteria.Module.Trim();
            query = query.Where(x => EF.Functions.ILike(x.Module, value));
        }

        if (!string.IsNullOrWhiteSpace(criteria.Extension))
        {
            var value = criteria.Extension.Trim().ToLower();
            if (!value.StartsWith('.'))
            {
                value = "." + value;
            }

            query = query.Where(x => x.Extension == value);
        }

        if (!string.IsNullOrWhiteSpace(criteria.RelatedEntityType))
        {
            var value = criteria.RelatedEntityType.Trim();
            query = query.Where(x => x.RelatedEntityType == value);
        }

        if (!string.IsNullOrWhiteSpace(criteria.RelatedEntityId))
        {
            var value = criteria.RelatedEntityId.Trim();
            query = query.Where(x => x.RelatedEntityId == value);
        }

        if (!string.IsNullOrWhiteSpace(criteria.CompanyId))
        {
            var value = criteria.CompanyId.Trim().ToUpper();
            query = query.Where(x => x.CompanyId == value);
        }

        if (!string.IsNullOrWhiteSpace(criteria.PlantId))
        {
            var value = criteria.PlantId.Trim().ToUpper();
            query = query.Where(x => x.PlantId == value);
        }

        var total = await query.CountAsync(cancellationToken).ConfigureAwait(false);
        var items = await query
            .OrderByDescending(x => x.UploadedAt)
            .Skip((criteria.Page - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return (items, total);
    }
}

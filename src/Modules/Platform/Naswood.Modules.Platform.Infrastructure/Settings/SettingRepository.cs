using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Naswood.Modules.Platform.Application.Authentication;
using Naswood.Modules.Platform.Application.Settings;
using Naswood.Modules.Platform.Domain.Settings;
using Naswood.Modules.Platform.Infrastructure.Persistence;

namespace Naswood.Modules.Platform.Infrastructure.Settings;

public sealed class SettingRepository : ISettingRepository
{
    private readonly PlatformDbContext _db;

    public SettingRepository(PlatformDbContext db) => _db = db;

    public Task<SettingEntry?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _db.Settings.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<bool> KeyExistsAsync(
        string key,
        SettingScope scope,
        string? companyId,
        string? plantId,
        Guid? userId,
        Guid? excludingId,
        CancellationToken cancellationToken = default)
    {
        var normalizedKey = key.Trim();
        var normalizedCompany = string.IsNullOrWhiteSpace(companyId) ? null : companyId.Trim().ToUpperInvariant();
        var normalizedPlant = string.IsNullOrWhiteSpace(plantId) ? null : plantId.Trim().ToUpperInvariant();

        return _db.Settings.AnyAsync(
            x => x.IsActive &&
                 x.Key == normalizedKey &&
                 x.Scope == scope &&
                 x.CompanyId == normalizedCompany &&
                 x.PlantId == normalizedPlant &&
                 x.UserId == userId &&
                 (!excludingId.HasValue || x.Id != excludingId.Value),
            cancellationToken);
    }

    public async Task AddAsync(SettingEntry setting, CancellationToken cancellationToken = default) =>
        await _db.Settings.AddAsync(setting, cancellationToken).ConfigureAwait(false);

    public async Task<(IReadOnlyList<SettingEntry> Items, int TotalCount)> SearchAsync(
        SettingSearchCriteria criteria,
        CancellationToken cancellationToken = default)
    {
        var query = _db.Settings.AsNoTracking().Where(x => x.IsActive);

        if (!string.IsNullOrWhiteSpace(criteria.Category))
        {
            var value = criteria.Category.Trim();
            query = query.Where(x => EF.Functions.ILike(x.Category, value));
        }

        if (!string.IsNullOrWhiteSpace(criteria.Key))
        {
            var value = criteria.Key.Trim();
            query = query.Where(x => EF.Functions.ILike(x.Key, $"%{value}%"));
        }

        if (!string.IsNullOrWhiteSpace(criteria.Scope) &&
            Enum.TryParse<SettingScope>(criteria.Scope, true, out var scope))
        {
            query = query.Where(x => x.Scope == scope);
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
            .OrderBy(x => x.Category)
            .ThenBy(x => x.Key)
            .Skip((criteria.Page - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return (items, total);
    }

    public async Task<IReadOnlyList<SettingEntry>> ListActiveAsync(CancellationToken cancellationToken = default) =>
        await _db.Settings.AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Category)
            .ThenBy(x => x.Key)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

    public Task<bool> AnyAsync(CancellationToken cancellationToken = default) =>
        _db.Settings.AnyAsync(cancellationToken);
}

public sealed class SettingsBootstrapHostedService : IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SettingsBootstrapHostedService> _logger;

    public SettingsBootstrapHostedService(
        IServiceScopeFactory scopeFactory,
        ILogger<SettingsBootstrapHostedService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var environment = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();
        if (environment.IsEnvironment("Testing"))
        {
            return;
        }

        var settings = scope.ServiceProvider.GetRequiredService<ISettingRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IPlatformUnitOfWork>();
        if (await settings.AnyAsync(cancellationToken).ConfigureAwait(false))
        {
            return;
        }

        foreach (var setting in SettingsCatalogSeed.CreateDefaults())
        {
            await settings.AddAsync(setting, cancellationToken).ConfigureAwait(false);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        _logger.LogInformation("Seeded default platform settings.");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

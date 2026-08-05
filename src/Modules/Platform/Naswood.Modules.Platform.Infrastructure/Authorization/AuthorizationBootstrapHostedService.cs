using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Naswood.Modules.Platform.Application.Authentication;
using Naswood.Modules.Platform.Application.Authorization;

namespace Naswood.Modules.Platform.Infrastructure.Authorization;

public sealed class AuthorizationBootstrapHostedService : IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AuthorizationBootstrapHostedService> _logger;

    public AuthorizationBootstrapHostedService(
        IServiceScopeFactory scopeFactory,
        ILogger<AuthorizationBootstrapHostedService> logger)
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

        var permissions = scope.ServiceProvider.GetRequiredService<IPermissionCatalogRepository>();
        var roles = scope.ServiceProvider.GetRequiredService<IRoleCatalogRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IPlatformUnitOfWork>();
        var catalog = AuthorizationCatalogSeed.CreatePermissions();

        if (!await permissions.AnyAsync(cancellationToken).ConfigureAwait(false))
        {
            await permissions.AddRangeAsync(catalog, cancellationToken).ConfigureAwait(false);
            _logger.LogInformation("Seeded {Count} permissions.", catalog.Count);
        }
        else
        {
            // Upsert newly introduced permission codes without wiping existing catalog.
            var existing = await permissions.GetAllActiveAsync(cancellationToken).ConfigureAwait(false);
            var existingCodes = existing.Select(p => p.Code).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var missing = catalog.Where(p => !existingCodes.Contains(p.Code)).ToArray();
            if (missing.Length > 0)
            {
                await permissions.AddRangeAsync(missing, cancellationToken).ConfigureAwait(false);
                _logger.LogInformation("Seeded {Count} newly added permissions.", missing.Length);
            }
        }

        if (!await roles.AnyAsync(cancellationToken).ConfigureAwait(false))
        {
            // Use in-memory catalog (not DB query) so permission codes are present before SaveChanges.
            await roles.AddAsync(AuthorizationCatalogSeed.CreateAdministratorRole(catalog), cancellationToken)
                .ConfigureAwait(false);
            await roles.AddAsync(AuthorizationCatalogSeed.CreateReadOnlyRole(), cancellationToken)
                .ConfigureAwait(false);
            _logger.LogInformation("Seeded Administrator and ReadOnly roles.");
        }

        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

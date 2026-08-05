using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Naswood.Modules.Platform.Application.Authentication;
using Naswood.Modules.Platform.Application.Users;

namespace Naswood.Modules.Platform.Infrastructure.Users;

public sealed class OrganizationBootstrapHostedService : IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OrganizationBootstrapHostedService> _logger;

    public OrganizationBootstrapHostedService(
        IServiceScopeFactory scopeFactory,
        ILogger<OrganizationBootstrapHostedService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var organization = scope.ServiceProvider.GetRequiredService<IOrganizationReferenceRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IPlatformUnitOfWork>();

        if (await organization.AnyAsync(cancellationToken).ConfigureAwait(false))
        {
            return;
        }

        await organization.SeedAsync(
                OrganizationCatalogSeed.CreateCompanies(),
                OrganizationCatalogSeed.CreatePlants(),
                OrganizationCatalogSeed.CreateDepartments(),
                OrganizationCatalogSeed.CreatePositions(),
                cancellationToken)
            .ConfigureAwait(false);
        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        _logger.LogInformation("Seeded organization reference catalog for User Management.");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

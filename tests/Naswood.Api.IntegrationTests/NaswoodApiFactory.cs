using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Naswood.Modules.Platform.Application.Authentication;
using Naswood.Modules.Platform.Application.Authorization;
using Naswood.Modules.Platform.Application.Settings;
using Naswood.Modules.Platform.Application.Users;
using Naswood.Modules.Platform.Domain.Authentication;
using Naswood.Modules.Platform.Infrastructure.Persistence;

namespace Naswood.Api.IntegrationTests;

[CollectionDefinition(Name)]
public sealed class ApiCollection : ICollectionFixture<NaswoodApiFactory>
{
    public const string Name = "api";
}

public sealed class NaswoodApiFactory : WebApplicationFactory<Program>
{
    public const string TestConnectionString =
        "Host=127.0.0.1;Port=5432;Database=naswood_os_test;Username=naswood;Password=naswood;Include Error Detail=true";

    private readonly object _gate = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting("ConnectionStrings:Platform", TestConnectionString);
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Platform"] = TestConnectionString,
                ["Authentication:SigningKey"] = "IntegrationTestSigningKey_MustBeAtLeast32Chars!",
                ["Authentication:Issuer"] = "Naswood.OS",
                ["Authentication:Audience"] = "Naswood.OS",
                ["Authentication:BootstrapAdmin:Enabled"] = "false",
                ["Authentication:BcryptWorkFactor"] = "4"
            });
        });
    }

    public async Task ResetDatabaseAsync()
    {
        // Serialize resets across tests sharing one factory.
        await Task.Yield();
        lock (_gate)
        {
            ResetDatabaseCore().GetAwaiter().GetResult();
        }
    }

    private async Task ResetDatabaseCore()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlatformDbContext>();
        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();

        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var users = scope.ServiceProvider.GetRequiredService<IAuthUserRepository>();
        var permissions = scope.ServiceProvider.GetRequiredService<IPermissionCatalogRepository>();
        var roles = scope.ServiceProvider.GetRequiredService<IRoleCatalogRepository>();
        var organization = scope.ServiceProvider.GetRequiredService<IOrganizationReferenceRepository>();
        var settings = scope.ServiceProvider.GetRequiredService<ISettingRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IPlatformUnitOfWork>();

        await organization.SeedAsync(
            OrganizationCatalogSeed.CreateCompanies(),
            OrganizationCatalogSeed.CreatePlants(),
            OrganizationCatalogSeed.CreateDepartments(),
            OrganizationCatalogSeed.CreatePositions());

        foreach (var setting in SettingsCatalogSeed.CreateDefaults())
        {
            await settings.AddAsync(setting);
        }

        var catalog = AuthorizationCatalogSeed.CreatePermissions();
        await permissions.AddRangeAsync(catalog);
        await roles.AddAsync(AuthorizationCatalogSeed.CreateAdministratorRole(catalog));
        await roles.AddAsync(AuthorizationCatalogSeed.CreateReadOnlyRole());

        var admin = AuthUser.Create(
            "admin",
            "Administrator",
            "admin@naswood.local",
            hasher.Hash("Naswood!Admin1"),
            ["COMP-001"],
            ["PLANT-001"],
            ["Administrator"]);

        await users.AddAsync(admin);
        await unitOfWork.SaveChangesAsync();
    }
}

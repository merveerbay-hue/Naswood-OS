using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Naswood.Modules.Platform.Infrastructure.Persistence;

/// <summary>
/// Used only by <c>dotnet ef</c> to build the model when adding or scripting migrations.
/// Override the database with NASWOOD_DESIGN_CONNECTION; commands that only read the model never connect.
/// </summary>
public sealed class PlatformDbContextDesignTimeFactory : IDesignTimeDbContextFactory<PlatformDbContext>
{
    public PlatformDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("NASWOOD_DESIGN_CONNECTION")
            ?? "Host=127.0.0.1;Port=5432;Database=naswood_os;Username=naswood;Password=naswood";
        var options = new DbContextOptionsBuilder<PlatformDbContext>()
            .UseNpgsql(connectionString, PlatformDbContext.ConfigureNpgsql)
            .Options;
        return new PlatformDbContext(options);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Naswood.Modules.Business.Infrastructure.Persistence;

/// <summary>
/// Used only by <c>dotnet ef</c> to build the model when adding or scripting migrations.
/// Override the database with NASWOOD_DESIGN_CONNECTION; commands that only read the model never connect.
/// </summary>
public sealed class BusinessDbContextDesignTimeFactory : IDesignTimeDbContextFactory<BusinessDbContext>
{
    public BusinessDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("NASWOOD_DESIGN_CONNECTION")
            ?? "Host=127.0.0.1;Port=5432;Database=naswood_os;Username=naswood;Password=naswood";
        var options = new DbContextOptionsBuilder<BusinessDbContext>()
            .UseNpgsql(connectionString, BusinessDbContext.ConfigureNpgsql)
            .Options;
        return new BusinessDbContext(options);
    }
}

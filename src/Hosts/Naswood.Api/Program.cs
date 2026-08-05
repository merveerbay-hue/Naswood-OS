using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Naswood.BuildingBlocks.Infrastructure;
using Naswood.BuildingBlocks.Infrastructure.Storage;
using Naswood.Modules.Platform.Application;
using Naswood.Modules.Platform.Infrastructure;
using Naswood.Modules.Platform.Infrastructure.Persistence;
using Naswood.Modules.Platform.Presentation;
using Naswood.Modules.Business.Application;
using Naswood.Modules.Business.Infrastructure;
using Naswood.Modules.Business.Presentation;
using Naswood.Modules.Business.Infrastructure.Persistence;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddPlatformPresentation()
    .AddBusinessPresentation();

builder.Services.AddPlatformApplication();
builder.Services.AddBusinessApplication();
builder.Services.AddPlatformInfrastructure(builder.Configuration);
builder.Services.AddBusinessInfrastructure(builder.Configuration);
builder.Services.AddBuildingBlocksInfrastructure(
    typeof(Naswood.Modules.Platform.Application.DependencyInjection).Assembly,
    typeof(Naswood.Modules.Business.Application.DependencyInjection).Assembly);
builder.Services.AddFileStorage(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PlatformDbContext>();
    await db.Database.EnsureCreatedAsync();
    var businessDb = scope.ServiceProvider.GetRequiredService<BusinessDbContext>();
    // Same PostgreSQL database as Platform — EnsureCreated is a no-op once DB exists.
    // CreateTables also fails once any business table exists, so apply the model script
    // statement-by-statement and ignore "already exists" for incremental entity adds.
    var businessCreator = businessDb.Database.GetService<IRelationalDatabaseCreator>();
    try
    {
        await businessCreator.CreateTablesAsync();
    }
    catch (Exception ex) when (ex.Message.Contains("already exists", StringComparison.OrdinalIgnoreCase))
    {
        var script = businessDb.Database.GenerateCreateScript();
        foreach (var statement in SplitPostgresStatements(script))
        {
            try
            {
                await businessDb.Database.ExecuteSqlRawAsync(statement).ConfigureAwait(false);
            }
            catch (Exception statementEx) when (
                statementEx.Message.Contains("already exists", StringComparison.OrdinalIgnoreCase)
                || statementEx.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
            {
                // Table/index/schema already provisioned.
            }
        }
    }
}

app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

static IEnumerable<string> SplitPostgresStatements(string script)
{
    var statements = new List<string>();
    var buffer = new StringBuilder();
    var inDollar = false;

    for (var i = 0; i < script.Length; i++)
    {
        if (script[i] == '$' && i + 3 < script.Length && script.AsSpan(i, 4).SequenceEqual("$EF$".AsSpan()))
        {
            inDollar = !inDollar;
            buffer.Append("$EF$");
            i += 3;
            continue;
        }

        if (script[i] == ';' && !inDollar)
        {
            var statement = buffer.ToString().Trim();
            if (statement.Length > 0)
            {
                statements.Add(statement);
            }

            buffer.Clear();
            continue;
        }

        buffer.Append(script[i]);
    }

    var tail = buffer.ToString().Trim();
    if (tail.Length > 0)
    {
        statements.Add(tail);
    }

    return statements;
}

public partial class Program;

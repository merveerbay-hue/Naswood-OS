using Microsoft.EntityFrameworkCore;
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
    // Both modules share one PostgreSQL database; each keeps its own migration history in its schema.
    var platformDb = scope.ServiceProvider.GetRequiredService<PlatformDbContext>();
    await platformDb.Database.MigrateAsync();
    var businessDb = scope.ServiceProvider.GetRequiredService<BusinessDbContext>();
    await businessDb.Database.MigrateAsync();
    await BusinessStartupDataFixes.ApplyAsync(businessDb);
}

app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program;

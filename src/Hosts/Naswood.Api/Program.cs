using Microsoft.EntityFrameworkCore;
using Naswood.BuildingBlocks.Infrastructure;
using Naswood.BuildingBlocks.Infrastructure.Storage;
using Naswood.Modules.Platform.Application;
using Naswood.Modules.Platform.Infrastructure;
using Naswood.Modules.Platform.Infrastructure.Persistence;
using Naswood.Modules.Platform.Presentation;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddPlatformPresentation();

builder.Services.AddPlatformApplication();
builder.Services.AddPlatformInfrastructure(builder.Configuration);
builder.Services.AddBuildingBlocksInfrastructure(
    typeof(Naswood.Modules.Platform.Application.DependencyInjection).Assembly);
builder.Services.AddFileStorage(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PlatformDbContext>();
    await db.Database.EnsureCreatedAsync();
}

app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program;

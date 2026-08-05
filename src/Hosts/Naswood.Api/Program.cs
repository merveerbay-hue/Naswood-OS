using Naswood.BuildingBlocks.Infrastructure;
using Naswood.Modules.Platform.Application;
using Naswood.Modules.Platform.Infrastructure;
using Naswood.Modules.Platform.Presentation;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddPlatformPresentation();

builder.Services.AddPlatformApplication();
builder.Services.AddPlatformInfrastructure();
builder.Services.AddBuildingBlocksInfrastructure(
    typeof(Naswood.Modules.Platform.Application.DependencyInjection).Assembly);

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.MapControllers();

app.Run();

/// <summary>
/// Exposes the Program entry point for WebApplicationFactory integration tests.
/// </summary>
public partial class Program;

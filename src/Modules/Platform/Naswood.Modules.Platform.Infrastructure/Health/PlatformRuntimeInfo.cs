using System.Diagnostics;
using System.Reflection;
using Naswood.Modules.Platform.Application.Health;

namespace Naswood.Modules.Platform.Infrastructure.Health;

public sealed class PlatformRuntimeInfo : IPlatformRuntimeInfo
{
    private readonly DateTimeOffset _startedAt = DateTimeOffset.UtcNow;

    public string Version { get; } =
        Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "0.0.0";

    public TimeSpan Uptime => DateTimeOffset.UtcNow - _startedAt;
}

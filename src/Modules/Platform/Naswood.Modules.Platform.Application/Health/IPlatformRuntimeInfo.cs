namespace Naswood.Modules.Platform.Application.Health;

/// <summary>
/// Runtime facts for health reporting. Host provides the implementation.
/// </summary>
public interface IPlatformRuntimeInfo
{
    string Version { get; }

    TimeSpan Uptime { get; }
}

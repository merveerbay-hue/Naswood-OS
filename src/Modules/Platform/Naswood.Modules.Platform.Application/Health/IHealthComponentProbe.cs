using Naswood.Modules.Platform.Domain.Health;

namespace Naswood.Modules.Platform.Application.Health;

/// <summary>
/// Port for probing an infrastructure dependency.
/// Implementations live in Infrastructure; Domain/Application remain framework-free.
/// </summary>
public interface IHealthComponentProbe
{
    string Name { get; }

    Task<HealthComponent> CheckAsync(CancellationToken cancellationToken = default);
}

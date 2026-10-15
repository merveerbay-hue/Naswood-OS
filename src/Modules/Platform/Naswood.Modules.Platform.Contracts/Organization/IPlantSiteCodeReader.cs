namespace Naswood.Modules.Platform.Contracts.Organization;

/// <summary>
/// Read-only view of a plant record's site code (ADR-002 §6) for other modules.
/// </summary>
public interface IPlantSiteCodeReader
{
    /// <summary>
    /// Returns the three-letter site code of the active plant record, or null when the plant
    /// does not exist, is inactive or has no site code.
    /// </summary>
    Task<string?> GetSiteCodeAsync(string plantId, CancellationToken cancellationToken = default);
}

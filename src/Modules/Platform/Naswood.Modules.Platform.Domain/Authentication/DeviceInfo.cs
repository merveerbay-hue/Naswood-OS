using Naswood.BuildingBlocks.Domain;

namespace Naswood.Modules.Platform.Domain.Authentication;

public sealed class DeviceInfo : ValueObject
{
    private DeviceInfo()
    {
    }

    public string? DeviceId { get; private set; }

    public string? DeviceName { get; private set; }

    public string? Browser { get; private set; }

    public string? OperatingSystem { get; private set; }

    public string? IpAddress { get; private set; }

    public string? Country { get; private set; }

    public DeviceInfo(
        string? deviceId,
        string? deviceName,
        string? browser,
        string? operatingSystem,
        string? ipAddress,
        string? country)
    {
        DeviceId = Normalize(deviceId);
        DeviceName = Normalize(deviceName);
        Browser = Normalize(browser);
        OperatingSystem = Normalize(operatingSystem);
        IpAddress = Normalize(ipAddress);
        Country = Normalize(country);
    }

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return DeviceId;
        yield return DeviceName;
        yield return Browser;
        yield return OperatingSystem;
        yield return IpAddress;
        yield return Country;
    }
}

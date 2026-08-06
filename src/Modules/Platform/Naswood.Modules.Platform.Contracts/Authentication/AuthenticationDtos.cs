namespace Naswood.Modules.Platform.Contracts.Authentication;

public sealed class LoginRequestDto
{
    public required string Username { get; init; }

    public required string Password { get; init; }

    public bool RememberMe { get; init; }

    public string? CompanyId { get; init; }

    public string? PlantId { get; init; }

    public string? DeviceId { get; init; }

    public string? DeviceName { get; init; }

    public string? Browser { get; init; }

    public string? OperatingSystem { get; init; }
}

public sealed class RefreshTokenRequestDto
{
    public required string RefreshToken { get; init; }
}

public sealed class RevokeTokenRequestDto
{
    public required string RefreshToken { get; init; }
}

public sealed class AuthenticationResponseDto
{
    public required string AccessToken { get; init; }

    public required string RefreshToken { get; init; }

    public required string TokenType { get; init; }

    public required int ExpiresIn { get; init; }

    public required AuthenticatedUserDto User { get; init; }
}

public sealed class AuthenticatedUserDto
{
    public required string Id { get; init; }

    public required string Username { get; init; }

    public required string Name { get; init; }

    public string? Email { get; init; }

    public required string CompanyId { get; init; }

    public required string PlantId { get; init; }

    public required IReadOnlyList<string> Roles { get; init; }
}

public sealed class CurrentUserDto
{
    public required string Id { get; init; }

    public required string Username { get; init; }

    public required string Name { get; init; }

    public string? Email { get; init; }

    public required string CompanyId { get; init; }

    public required string PlantId { get; init; }

    public required Guid SessionId { get; init; }

    public required IReadOnlyList<string> Roles { get; init; }
}

public sealed class SessionDto
{
    public required Guid Id { get; init; }

    public required string Status { get; init; }

    public required string CompanyId { get; init; }

    public required string PlantId { get; init; }

    public required DateTimeOffset CreatedAt { get; init; }

    public required DateTimeOffset LastActivityAt { get; init; }

    public required DateTimeOffset AbsoluteExpiresAt { get; init; }

    public required DateTimeOffset RefreshExpiresAt { get; init; }

    public string? DeviceId { get; init; }

    public string? DeviceName { get; init; }

    public string? Browser { get; init; }

    public string? OperatingSystem { get; init; }

    public string? IpAddress { get; init; }
}

using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Platform.Contracts.Authentication;

namespace Naswood.Modules.Platform.Application.Authentication;

public sealed record LoginCommand(
    string Username,
    string Password,
    bool RememberMe,
    string? CompanyId,
    string? PlantId,
    string? DeviceId,
    string? DeviceName,
    string? Browser,
    string? OperatingSystem) : ICommand<Result<AuthenticationResponseDto>>;

public sealed record LogoutCommand : ICommand<Result>;

public sealed record RefreshTokenCommand(string RefreshToken) : ICommand<Result<AuthenticationResponseDto>>;

public sealed record RevokeTokenCommand(string RefreshToken) : ICommand<Result>;

public sealed record GetCurrentUserQuery : IQuery<Result<CurrentUserDto>>;

public sealed record GetCurrentSessionQuery : IQuery<Result<SessionDto>>;

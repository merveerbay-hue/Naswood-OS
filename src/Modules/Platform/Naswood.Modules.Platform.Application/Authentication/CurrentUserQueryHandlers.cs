using Microsoft.Extensions.Options;
using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Platform.Contracts.Authentication;
using Naswood.Modules.Platform.Domain.Authentication;

namespace Naswood.Modules.Platform.Application.Authentication;

public sealed class GetCurrentUserQueryHandler : IQueryHandler<GetCurrentUserQuery, Result<CurrentUserDto>>
{
    private readonly IAuthUserRepository _users;
    private readonly IAuthSessionRepository _sessions;
    private readonly IAuthRequestContext _requestContext;
    private readonly IClock _clock;
    private readonly AuthenticationOptions _options;

    public GetCurrentUserQueryHandler(
        IAuthUserRepository users,
        IAuthSessionRepository sessions,
        IAuthRequestContext requestContext,
        IClock clock,
        IOptions<AuthenticationOptions> options)
    {
        _users = users;
        _sessions = sessions;
        _requestContext = requestContext;
        _clock = clock;
        _options = options.Value;
    }

    public async Task<Result<CurrentUserDto>> HandleAsync(
        GetCurrentUserQuery query,
        CancellationToken cancellationToken = default)
    {
        if (_requestContext.UserId is null || _requestContext.SessionId is null)
        {
            return Result.Failure<CurrentUserDto>(AuthErrors.TokenInvalid());
        }

        var session = await _sessions
            .GetByIdAsync(_requestContext.SessionId.Value, cancellationToken)
            .ConfigureAwait(false);

        if (session is null)
        {
            return Result.Failure<CurrentUserDto>(AuthErrors.SessionExpired());
        }

        var usable = session.CheckUsable(_clock.UtcNow, TimeSpan.FromMinutes(_options.IdleTimeoutMinutes));
        if (usable.IsFailure)
        {
            return Result.Failure<CurrentUserDto>(usable.Error!);
        }

        var user = await _users.GetByIdAsync(_requestContext.UserId.Value, cancellationToken).ConfigureAwait(false);
        if (user is null)
        {
            return Result.Failure<CurrentUserDto>(AuthErrors.TokenInvalid());
        }

        return Result.Success(new CurrentUserDto
        {
            Id = user.Id.ToString("D"),
            Username = user.Username,
            Name = user.DisplayName,
            Email = user.Email,
            CompanyId = session.CompanyId,
            PlantId = session.PlantId,
            SessionId = session.Id,
            Roles = user.Roles.ToArray()
        });
    }
}

public sealed class GetCurrentSessionQueryHandler : IQueryHandler<GetCurrentSessionQuery, Result<SessionDto>>
{
    private readonly IAuthSessionRepository _sessions;
    private readonly IAuthRequestContext _requestContext;
    private readonly IClock _clock;
    private readonly AuthenticationOptions _options;

    public GetCurrentSessionQueryHandler(
        IAuthSessionRepository sessions,
        IAuthRequestContext requestContext,
        IClock clock,
        IOptions<AuthenticationOptions> options)
    {
        _sessions = sessions;
        _requestContext = requestContext;
        _clock = clock;
        _options = options.Value;
    }

    public async Task<Result<SessionDto>> HandleAsync(
        GetCurrentSessionQuery query,
        CancellationToken cancellationToken = default)
    {
        if (_requestContext.SessionId is null)
        {
            return Result.Failure<SessionDto>(AuthErrors.TokenInvalid());
        }

        var session = await _sessions
            .GetByIdAsync(_requestContext.SessionId.Value, cancellationToken)
            .ConfigureAwait(false);

        if (session is null)
        {
            return Result.Failure<SessionDto>(AuthErrors.SessionExpired());
        }

        var usable = session.CheckUsable(_clock.UtcNow, TimeSpan.FromMinutes(_options.IdleTimeoutMinutes));
        if (usable.IsFailure)
        {
            return Result.Failure<SessionDto>(usable.Error!);
        }

        return Result.Success(new SessionDto
        {
            Id = session.Id,
            Status = session.Status.ToString(),
            CompanyId = session.CompanyId,
            PlantId = session.PlantId,
            CreatedAt = session.CreatedAt,
            LastActivityAt = session.LastActivityAt,
            AbsoluteExpiresAt = session.AbsoluteExpiresAt,
            RefreshExpiresAt = session.RefreshExpiresAt,
            DeviceId = session.Device.DeviceId,
            DeviceName = session.Device.DeviceName,
            Browser = session.Device.Browser,
            OperatingSystem = session.Device.OperatingSystem,
            IpAddress = session.Device.IpAddress
        });
    }
}

using Microsoft.Extensions.Options;
using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Platform.Contracts.Authentication;
using Naswood.Modules.Platform.Domain.Authentication;

namespace Naswood.Modules.Platform.Application.Authentication;

/// <summary>
/// Üst Yönetici: switch working plant among authorized factories without changing Ana Üs.
/// </summary>
public sealed class SwitchWorkingPlantCommandHandler
    : ICommandHandler<SwitchWorkingPlantCommand, Result<AuthenticationResponseDto>>
{
    private readonly IAuthUserRepository _users;
    private readonly IAuthSessionRepository _sessions;
    private readonly ITokenService _tokenService;
    private readonly IPlatformUnitOfWork _unitOfWork;
    private readonly IOutboxWriter _outbox;
    private readonly IClock _clock;
    private readonly IAuthRequestContext _requestContext;
    private readonly AuthenticationOptions _options;

    public SwitchWorkingPlantCommandHandler(
        IAuthUserRepository users,
        IAuthSessionRepository sessions,
        ITokenService tokenService,
        IPlatformUnitOfWork unitOfWork,
        IOutboxWriter outbox,
        IClock clock,
        IAuthRequestContext requestContext,
        IOptions<AuthenticationOptions> options)
    {
        _users = users;
        _sessions = sessions;
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
        _outbox = outbox;
        _clock = clock;
        _requestContext = requestContext;
        _options = options.Value;
    }

    public async Task<Result<AuthenticationResponseDto>> HandleAsync(
        SwitchWorkingPlantCommand command,
        CancellationToken cancellationToken = default)
    {
        if (_requestContext.UserId is null || _requestContext.SessionId is null)
            return Result.Failure<AuthenticationResponseDto>(AuthErrors.TokenInvalid());

        if (string.IsNullOrWhiteSpace(command.PlantId))
            return Result.Failure<AuthenticationResponseDto>(AuthErrors.Validation("PlantId is required."));

        var now = _clock.UtcNow;
        var session = await _sessions.GetByIdAsync(_requestContext.SessionId.Value, cancellationToken)
            .ConfigureAwait(false);
        if (session is null)
            return Result.Failure<AuthenticationResponseDto>(AuthErrors.SessionExpired());

        var usable = session.CheckUsable(now, TimeSpan.FromMinutes(_options.IdleTimeoutMinutes));
        if (usable.IsFailure)
            return Result.Failure<AuthenticationResponseDto>(usable.Error!);

        var user = await _users.GetByIdAsync(_requestContext.UserId.Value, cancellationToken).ConfigureAwait(false);
        if (user is null)
            return Result.Failure<AuthenticationResponseDto>(AuthErrors.TokenInvalid());

        if (!PlantVisibilityPolicy.CanSwitchPlant(user.Roles))
            return Result.Failure<AuthenticationResponseDto>(AuthErrors.PlantSwitchForbidden());

        var target = command.PlantId.Trim();
        var visible = PlantVisibilityPolicy.VisiblePlantIds(
            user.Roles,
            user.HomePlantId,
            user.PlantIds.ToArray());
        if (!visible.Any(p => string.Equals(p, target, StringComparison.OrdinalIgnoreCase)))
            return Result.Failure<AuthenticationResponseDto>(AuthErrors.PlantNotAllowed());

        // Also require the plant to be on the account assignment (defense in depth).
        if (!user.PlantIds.Contains(target, StringComparer.OrdinalIgnoreCase))
            return Result.Failure<AuthenticationResponseDto>(AuthErrors.PlantNotAllowed());

        var accessTokenId = UuidV7.NewGuid();
        session.SwitchWorkingPlant(target, accessTokenId, now);

        var access = _tokenService.CreateAccessToken(
            user,
            session.Id,
            accessTokenId,
            session.CompanyId,
            target,
            now);

        await _outbox.EnqueueAsync(
                "WorkingPlantSwitched",
                new
                {
                    userId = user.Id,
                    sessionId = session.Id,
                    plantId = target,
                    homePlantId = user.HomePlantId
                },
                user.Id,
                _requestContext.CorrelationId,
                now,
                cancellationToken)
            .ConfigureAwait(false);

        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        var home = user.HomePlantId ?? target;
        return Result.Success(new AuthenticationResponseDto
        {
            AccessToken = access.Token,
            // Refresh token unchanged — FE keeps existing refresh, swaps access only.
            RefreshToken = string.Empty,
            TokenType = "Bearer",
            ExpiresIn = access.ExpiresInSeconds,
            User = new AuthenticatedUserDto
            {
                Id = user.Id.ToString("D"),
                Username = user.Username,
                Name = user.DisplayName,
                Email = user.Email,
                CompanyId = session.CompanyId,
                PlantId = target,
                HomePlantId = home,
                PlantIds = visible,
                CanSwitchPlant = true,
                Roles = user.Roles.ToArray()
            }
        });
    }
}

public sealed class GetVisiblePlantsQueryHandler
    : IQueryHandler<GetVisiblePlantsQuery, Result<IReadOnlyList<VisiblePlantDto>>>
{
    private readonly IAuthUserRepository _users;
    private readonly IAuthRequestContext _requestContext;
    private readonly Naswood.Modules.Platform.Application.Users.IOrganizationReferenceRepository _organization;

    public GetVisiblePlantsQueryHandler(
        IAuthUserRepository users,
        IAuthRequestContext requestContext,
        Naswood.Modules.Platform.Application.Users.IOrganizationReferenceRepository organization)
    {
        _users = users;
        _requestContext = requestContext;
        _organization = organization;
    }

    public async Task<Result<IReadOnlyList<VisiblePlantDto>>> HandleAsync(
        GetVisiblePlantsQuery query,
        CancellationToken cancellationToken = default)
    {
        if (_requestContext.UserId is null)
            return Result.Failure<IReadOnlyList<VisiblePlantDto>>(AuthErrors.TokenInvalid());

        var user = await _users.GetByIdAsync(_requestContext.UserId.Value, cancellationToken).ConfigureAwait(false);
        if (user is null)
            return Result.Failure<IReadOnlyList<VisiblePlantDto>>(AuthErrors.TokenInvalid());

        var home = user.HomePlantId;
        var visible = PlantVisibilityPolicy.VisiblePlantIds(user.Roles, home, user.PlantIds.ToArray());
        var items = new List<VisiblePlantDto>();
        foreach (var code in visible)
        {
            var plant = await _organization.GetPlantByCodeAsync(code, cancellationToken).ConfigureAwait(false);
            items.Add(new VisiblePlantDto
            {
                Code = code,
                Name = plant?.Name ?? code,
                IsHome = !string.IsNullOrWhiteSpace(home)
                         && string.Equals(code, home, StringComparison.OrdinalIgnoreCase)
            });
        }

        return Result.Success<IReadOnlyList<VisiblePlantDto>>(items);
    }
}

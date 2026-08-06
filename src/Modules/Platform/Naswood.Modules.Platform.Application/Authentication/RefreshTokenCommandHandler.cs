using Microsoft.Extensions.Options;
using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Platform.Contracts.Authentication;
using Naswood.Modules.Platform.Domain.Authentication;

namespace Naswood.Modules.Platform.Application.Authentication;

public sealed class RefreshTokenCommandHandler
    : ICommandHandler<RefreshTokenCommand, Result<AuthenticationResponseDto>>
{
    private readonly IAuthUserRepository _users;
    private readonly IAuthSessionRepository _sessions;
    private readonly ITokenService _tokenService;
    private readonly IPlatformUnitOfWork _unitOfWork;
    private readonly IOutboxWriter _outbox;
    private readonly IClock _clock;
    private readonly IAuthRequestContext _requestContext;
    private readonly AuthenticationOptions _options;

    public RefreshTokenCommandHandler(
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
        RefreshTokenCommand command,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.RefreshToken))
        {
            return Result.Failure<AuthenticationResponseDto>(AuthErrors.RefreshTokenInvalid());
        }

        var now = _clock.UtcNow;
        var hash = _tokenService.HashRefreshToken(command.RefreshToken);
        var session = await _sessions.GetByRefreshTokenHashAsync(hash, cancellationToken).ConfigureAwait(false);
        if (session is null)
        {
            return Result.Failure<AuthenticationResponseDto>(AuthErrors.RefreshTokenInvalid());
        }

        var usable = session.EnsureUsable(now, TimeSpan.FromMinutes(_options.IdleTimeoutMinutes));
        if (usable.IsFailure)
        {
            await PersistDomainEventsAsync(session, cancellationToken).ConfigureAwait(false);
            await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return Result.Failure<AuthenticationResponseDto>(usable.Error!);
        }

        var user = await _users.GetByIdAsync(session.UserId, cancellationToken).ConfigureAwait(false);
        if (user is null)
        {
            session.Revoke(now, logout: false);
            await PersistDomainEventsAsync(session, cancellationToken).ConfigureAwait(false);
            await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return Result.Failure<AuthenticationResponseDto>(AuthErrors.TokenInvalid());
        }

        var eligibility = user.EnsureCanAuthenticate(now);
        if (eligibility.IsFailure)
        {
            session.Revoke(now, logout: false);
            await PersistDomainEventsAsync(session, cancellationToken).ConfigureAwait(false);
            await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return Result.Failure<AuthenticationResponseDto>(eligibility.Error!);
        }

        var accessTokenId = UuidV7.NewGuid();
        var refreshToken = _tokenService.CreateRefreshToken();
        session.RotateTokens(
            accessTokenId,
            _tokenService.HashRefreshToken(refreshToken),
            now,
            TimeSpan.FromDays(_options.RefreshTokenDays));

        var access = _tokenService.CreateAccessToken(
            user,
            session.Id,
            accessTokenId,
            session.CompanyId,
            session.PlantId,
            now);

        await PersistDomainEventsAsync(session, cancellationToken).ConfigureAwait(false);
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.Success(new AuthenticationResponseDto
        {
            AccessToken = access.Token,
            RefreshToken = refreshToken,
            TokenType = "Bearer",
            ExpiresIn = access.ExpiresInSeconds,
            User = new AuthenticatedUserDto
            {
                Id = user.Id.ToString("D"),
                Username = user.Username,
                Name = user.DisplayName,
                Email = user.Email,
                CompanyId = session.CompanyId,
                PlantId = session.PlantId,
                Roles = user.Roles.ToArray()
            }
        });
    }

    private async Task PersistDomainEventsAsync(AggregateRoot<Guid> aggregate, CancellationToken cancellationToken)
    {
        foreach (var domainEvent in aggregate.DomainEvents)
        {
            await _outbox.EnqueueAsync(
                    domainEvent.GetType().Name,
                    domainEvent,
                    aggregate.Id,
                    _requestContext.CorrelationId,
                    domainEvent.OccurredAt,
                    cancellationToken)
                .ConfigureAwait(false);
        }

        aggregate.ClearDomainEvents();
    }
}

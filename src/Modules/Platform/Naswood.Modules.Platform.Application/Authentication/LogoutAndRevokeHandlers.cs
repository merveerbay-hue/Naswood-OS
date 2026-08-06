using Microsoft.Extensions.Options;
using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Platform.Domain.Authentication;

namespace Naswood.Modules.Platform.Application.Authentication;

public sealed class LogoutCommandHandler : ICommandHandler<LogoutCommand, Result>
{
    private readonly IAuthSessionRepository _sessions;
    private readonly IPlatformUnitOfWork _unitOfWork;
    private readonly IOutboxWriter _outbox;
    private readonly IClock _clock;
    private readonly IAuthRequestContext _requestContext;
    private readonly AuthenticationOptions _options;

    public LogoutCommandHandler(
        IAuthSessionRepository sessions,
        IPlatformUnitOfWork unitOfWork,
        IOutboxWriter outbox,
        IClock clock,
        IAuthRequestContext requestContext,
        IOptions<AuthenticationOptions> options)
    {
        _sessions = sessions;
        _unitOfWork = unitOfWork;
        _outbox = outbox;
        _clock = clock;
        _requestContext = requestContext;
        _options = options.Value;
    }

    public async Task<Result> HandleAsync(LogoutCommand command, CancellationToken cancellationToken = default)
    {
        if (_requestContext.SessionId is null)
        {
            return Result.Failure(AuthErrors.TokenInvalid());
        }

        var session = await _sessions
            .GetByIdAsync(_requestContext.SessionId.Value, cancellationToken)
            .ConfigureAwait(false);

        if (session is null)
        {
            return Result.Failure(AuthErrors.SessionExpired());
        }

        var now = _clock.UtcNow;
        session.EnsureUsable(now, TimeSpan.FromMinutes(_options.IdleTimeoutMinutes));
        session.Revoke(now, logout: true);

        foreach (var domainEvent in session.DomainEvents)
        {
            await _outbox.EnqueueAsync(
                    domainEvent.GetType().Name,
                    domainEvent,
                    session.UserId,
                    _requestContext.CorrelationId,
                    domainEvent.OccurredAt,
                    cancellationToken)
                .ConfigureAwait(false);
        }

        session.ClearDomainEvents();
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success();
    }
}

public sealed class RevokeTokenCommandHandler : ICommandHandler<RevokeTokenCommand, Result>
{
    private readonly IAuthSessionRepository _sessions;
    private readonly ITokenService _tokenService;
    private readonly IPlatformUnitOfWork _unitOfWork;
    private readonly IOutboxWriter _outbox;
    private readonly IClock _clock;
    private readonly IAuthRequestContext _requestContext;

    public RevokeTokenCommandHandler(
        IAuthSessionRepository sessions,
        ITokenService tokenService,
        IPlatformUnitOfWork unitOfWork,
        IOutboxWriter outbox,
        IClock clock,
        IAuthRequestContext requestContext)
    {
        _sessions = sessions;
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
        _outbox = outbox;
        _clock = clock;
        _requestContext = requestContext;
    }

    public async Task<Result> HandleAsync(RevokeTokenCommand command, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.RefreshToken))
        {
            return Result.Failure(AuthErrors.RefreshTokenInvalid());
        }

        var hash = _tokenService.HashRefreshToken(command.RefreshToken);
        var session = await _sessions.GetByRefreshTokenHashAsync(hash, cancellationToken).ConfigureAwait(false);
        if (session is null)
        {
            return Result.Failure(AuthErrors.RefreshTokenInvalid());
        }

        session.Revoke(_clock.UtcNow, logout: false);

        foreach (var domainEvent in session.DomainEvents)
        {
            await _outbox.EnqueueAsync(
                    domainEvent.GetType().Name,
                    domainEvent,
                    session.UserId,
                    _requestContext.CorrelationId,
                    domainEvent.OccurredAt,
                    cancellationToken)
                .ConfigureAwait(false);
        }

        session.ClearDomainEvents();
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success();
    }
}

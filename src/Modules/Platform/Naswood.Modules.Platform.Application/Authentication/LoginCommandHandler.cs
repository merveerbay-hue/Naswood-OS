using Microsoft.Extensions.Options;
using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Platform.Contracts.Authentication;
using Naswood.Modules.Platform.Domain.Authentication;

namespace Naswood.Modules.Platform.Application.Authentication;

public sealed class LoginCommandHandler
    : ICommandHandler<LoginCommand, Result<AuthenticationResponseDto>>
{
    private readonly IAuthUserRepository _users;
    private readonly IAuthSessionRepository _sessions;
    private readonly ILoginHistoryRepository _loginHistory;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IPlatformUnitOfWork _unitOfWork;
    private readonly IOutboxWriter _outbox;
    private readonly IClock _clock;
    private readonly IAuthRequestContext _requestContext;
    private readonly AuthenticationOptions _options;

    public LoginCommandHandler(
        IAuthUserRepository users,
        IAuthSessionRepository sessions,
        ILoginHistoryRepository loginHistory,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IPlatformUnitOfWork unitOfWork,
        IOutboxWriter outbox,
        IClock clock,
        IAuthRequestContext requestContext,
        IOptions<AuthenticationOptions> options)
    {
        _users = users;
        _sessions = sessions;
        _loginHistory = loginHistory;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
        _outbox = outbox;
        _clock = clock;
        _requestContext = requestContext;
        _options = options.Value;
    }

    public async Task<Result<AuthenticationResponseDto>> HandleAsync(
        LoginCommand command,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.Username) || string.IsNullOrWhiteSpace(command.Password))
        {
            return Result.Failure<AuthenticationResponseDto>(
                AuthErrors.Validation("Username and password are required."));
        }

        var now = _clock.UtcNow;
        var device = CreateDevice(command);
        var username = command.Username.Trim();

        var user = await _users.GetByUsernameAsync(username, cancellationToken).ConfigureAwait(false);
        if (user is null)
        {
            await RecordFailureAsync(null, username, "INVALID_CREDENTIALS", device, now, cancellationToken)
                .ConfigureAwait(false);
            return Result.Failure<AuthenticationResponseDto>(AuthErrors.InvalidCredentials());
        }

        var eligibility = user.EnsureCanAuthenticate(now);
        if (eligibility.IsFailure)
        {
            await RecordFailureAsync(
                    user.Id,
                    username,
                    eligibility.Error!.Code,
                    device,
                    now,
                    cancellationToken)
                .ConfigureAwait(false);
            return Result.Failure<AuthenticationResponseDto>(eligibility.Error!);
        }

        if (!_passwordHasher.Verify(command.Password, user.PasswordHash))
        {
            user.RegisterFailedLogin(now);
            var reason = user.IsLocked ? "ACCOUNT_LOCKED" : "INVALID_CREDENTIALS";
            await RecordFailureAsync(user.Id, username, reason, device, now, cancellationToken)
                .ConfigureAwait(false);
            await PersistDomainEventsAsync(user, cancellationToken).ConfigureAwait(false);
            await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return Result.Failure<AuthenticationResponseDto>(
                user.IsLocked ? AuthErrors.AccountLocked() : AuthErrors.InvalidCredentials());
        }

        var context = user.ResolveCompanyAndPlant(command.CompanyId, command.PlantId);
        if (context.IsFailure)
        {
            await RecordFailureAsync(
                    user.Id,
                    username,
                    context.Error!.Code,
                    device,
                    now,
                    cancellationToken)
                .ConfigureAwait(false);
            await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return Result.Failure<AuthenticationResponseDto>(context.Error!);
        }

        var accessTokenId = UuidV7.NewGuid();
        var refreshToken = _tokenService.CreateRefreshToken();
        var refreshHash = _tokenService.HashRefreshToken(refreshToken);

        var session = AuthSession.Create(
            user.Id,
            accessTokenId,
            refreshHash,
            context.Value.CompanyId,
            context.Value.PlantId,
            device,
            command.RememberMe,
            now,
            TimeSpan.FromHours(_options.AbsoluteSessionHours),
            TimeSpan.FromDays(_options.RefreshTokenDays));

        var access = _tokenService.CreateAccessToken(
            user,
            session.Id,
            accessTokenId,
            context.Value.CompanyId,
            context.Value.PlantId,
            now);

        user.RegisterSuccessfulLogin(now, session.Id);

        await _sessions.AddAsync(session, cancellationToken).ConfigureAwait(false);
        await _loginHistory.AddAsync(
                LoginHistoryEntry.Success(
                    user.Id,
                    username,
                    session.Id,
                    device,
                    now,
                    _requestContext.CorrelationId),
                cancellationToken)
            .ConfigureAwait(false);

        await PersistDomainEventsAsync(user, cancellationToken).ConfigureAwait(false);
        await PersistDomainEventsAsync(session, cancellationToken).ConfigureAwait(false);
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.Success(MapResponse(access, refreshToken, user, context.Value.CompanyId, context.Value.PlantId));
    }

    private DeviceInfo CreateDevice(LoginCommand command) =>
        new(
            command.DeviceId,
            command.DeviceName,
            command.Browser,
            command.OperatingSystem,
            _requestContext.IpAddress,
            country: null);

    private async Task RecordFailureAsync(
        Guid? userId,
        string username,
        string reason,
        DeviceInfo device,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        await _loginHistory.AddAsync(
                LoginHistoryEntry.Failure(
                    userId,
                    username,
                    reason,
                    device,
                    now,
                    _requestContext.CorrelationId),
                cancellationToken)
            .ConfigureAwait(false);

        await _outbox.EnqueueAsync(
                "AuthenticationFailed",
                new
                {
                    username,
                    reason,
                    ipAddress = device.IpAddress
                },
                userId,
                _requestContext.CorrelationId,
                now,
                cancellationToken)
            .ConfigureAwait(false);
    }

    private async Task PersistDomainEventsAsync(AggregateRoot<Guid> aggregate, CancellationToken cancellationToken)
    {
        foreach (var domainEvent in aggregate.DomainEvents)
        {
            await _outbox.EnqueueAsync(
                    domainEvent.GetType().Name.Replace("Auth", string.Empty, StringComparison.Ordinal),
                    domainEvent,
                    aggregate.Id,
                    _requestContext.CorrelationId,
                    domainEvent.OccurredAt,
                    cancellationToken)
                .ConfigureAwait(false);
        }

        aggregate.ClearDomainEvents();
    }

    private static AuthenticationResponseDto MapResponse(
        IssuedAccessToken access,
        string refreshToken,
        AuthUser user,
        string companyId,
        string plantId) =>
        new()
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
                CompanyId = companyId,
                PlantId = plantId,
                Roles = user.Roles.ToArray()
            }
        };
}

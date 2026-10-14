using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Naswood.Modules.Platform.Application.Authentication;
using Naswood.Modules.Platform.Domain.Authentication;

namespace Naswood.Modules.Platform.Infrastructure.Authentication;

public sealed class AuthBootstrapHostedService : IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AuthBootstrapHostedService> _logger;

    public AuthBootstrapHostedService(
        IServiceScopeFactory scopeFactory,
        ILogger<AuthBootstrapHostedService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var options = scope.ServiceProvider.GetRequiredService<IOptions<AuthenticationOptions>>().Value;
        if (!options.BootstrapAdmin.Enabled)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(options.BootstrapAdmin.Password))
        {
            _logger.LogWarning("Bootstrap admin is enabled but password is empty; skipping seed.");
            return;
        }

        var users = scope.ServiceProvider.GetRequiredService<IAuthUserRepository>();
        if (await users.AnyAsync(cancellationToken).ConfigureAwait(false))
        {
            return;
        }

        ValidateBootstrapPassword(options.BootstrapAdmin.Password);

        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IPlatformUnitOfWork>();

        var admin = AuthUser.Create(
            options.BootstrapAdmin.Username,
            options.BootstrapAdmin.DisplayName,
            options.BootstrapAdmin.Email,
            hasher.Hash(options.BootstrapAdmin.Password),
            [options.BootstrapAdmin.CompanyId],
            [options.BootstrapAdmin.PlantId],
            [options.BootstrapAdmin.Role]);

        await users.AddAsync(admin, cancellationToken).ConfigureAwait(false);
        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        _logger.LogInformation("Bootstrap administrator '{Username}' created.", admin.Username);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private static void ValidateBootstrapPassword(string password)
    {
        // Password policy from Authentication.md / Security.md
        if (password.Length < 12 ||
            !password.Any(char.IsUpper) ||
            !password.Any(char.IsLower) ||
            !password.Any(char.IsDigit) ||
            !password.Any(ch => !char.IsLetterOrDigit(ch)))
        {
            throw new InvalidOperationException(
                "Bootstrap admin password must be at least 12 characters and include uppercase, lowercase, number and special character.");
        }
    }
}

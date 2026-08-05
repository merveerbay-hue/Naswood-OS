using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Naswood.Modules.Platform.Application.Authentication;
using Naswood.Modules.Platform.Application.Authorization;
using Naswood.Modules.Platform.Application.Health;
using Naswood.Modules.Platform.Infrastructure.Authentication;
using Naswood.Modules.Platform.Infrastructure.Authorization;
using Naswood.Modules.Platform.Infrastructure.Health;
using Naswood.Modules.Platform.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;

namespace Naswood.Modules.Platform.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddPlatformInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<AuthenticationOptions>(configuration.GetSection(AuthenticationOptions.SectionName));

        var connectionString = configuration.GetConnectionString("Platform")
            ?? throw new InvalidOperationException("Connection string 'Platform' is required.");

        services.AddDbContext<PlatformDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddHttpContextAccessor();
        services.AddScoped<IAuthUserRepository, AuthUserRepository>();
        services.AddScoped<IAuthSessionRepository, AuthSessionRepository>();
        services.AddScoped<ILoginHistoryRepository, LoginHistoryRepository>();
        services.AddScoped<IPlatformUnitOfWork, PlatformUnitOfWork>();
        services.AddScoped<IOutboxWriter, EfOutboxWriter>();
        services.AddScoped<IAuthRequestContext, HttpAuthRequestContext>();
        services.AddSingleton<IClock, SystemClock>();
        services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();
        services.AddSingleton<ITokenService, JwtTokenService>();
        services.AddHostedService<AuthBootstrapHostedService>();

        var environment = configuration["ASPNETCORE_ENVIRONMENT"]
            ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            ?? "Production";
        if (!string.Equals(environment, "Testing", StringComparison.OrdinalIgnoreCase))
        {
            services.AddHostedService<AuthorizationBootstrapHostedService>();
        }

        services.AddMemoryCache();
        services.AddScoped<IPermissionCatalogRepository, PermissionCatalogRepository>();
        services.AddScoped<IRoleCatalogRepository, RoleCatalogRepository>();
        services.AddScoped<IAuthorizationHistoryRepository, AuthorizationHistoryRepository>();
        services.AddSingleton<IPermissionCache, MemoryPermissionCache>();
        services.AddScoped<IAuthorizationEngine, AuthorizationEngine>();
        services.AddScoped<IEffectivePermissionService, EffectivePermissionService>();
        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddSingleton<IAuthorizationPolicyProvider, DynamicPermissionPolicyProvider>();

        services.AddSingleton<IPlatformRuntimeInfo, PlatformRuntimeInfo>();
        services.AddScoped<IHealthComponentProbe, ApplicationHealthProbe>();
        services.AddScoped<IHealthComponentProbe, DatabaseHealthProbe>();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<AuthenticationOptions>>((jwtOptions, authOptionsAccessor) =>
            {
                var authOptions = authOptionsAccessor.Value;
                jwtOptions.MapInboundClaims = false;
                jwtOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = authOptions.Issuer,
                    ValidAudience = authOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authOptions.SigningKey)),
                    ClockSkew = TimeSpan.FromSeconds(30),
                    NameClaimType = JwtRegisteredClaimNames.Sub,
                    RoleClaimType = ClaimTypes.Role
                };
            });

        services.AddAuthorization();

        var authOptions = configuration.GetSection(AuthenticationOptions.SectionName).Get<AuthenticationOptions>()
            ?? new AuthenticationOptions();

        if (string.IsNullOrWhiteSpace(authOptions.SigningKey) || authOptions.SigningKey.Length < 32)
        {
            throw new InvalidOperationException("Authentication:SigningKey must be at least 32 characters.");
        }

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.AddPolicy("auth-login", httpContext =>
            {
                var env = httpContext.RequestServices.GetRequiredService<IHostEnvironment>();
                var permitLimit = env.IsEnvironment("Development") || env.IsEnvironment("Testing")
                    ? 1000
                    : 5;

                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = permitLimit,
                        Window = TimeSpan.FromMinutes(15),
                        QueueLimit = 0
                    });
            });
        });

        return services;
    }
}

using Microsoft.EntityFrameworkCore;
using Naswood.Modules.Platform.Domain.Authentication;

namespace Naswood.Modules.Platform.Infrastructure.Persistence;

public sealed class PlatformDbContext : DbContext
{
    public PlatformDbContext(DbContextOptions<PlatformDbContext> options)
        : base(options)
    {
    }

    public DbSet<AuthUser> AuthUsers => Set<AuthUser>();

    public DbSet<AuthSession> AuthSessions => Set<AuthSession>();

    public DbSet<LoginHistoryEntry> LoginHistory => Set<LoginHistoryEntry>();

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("platform");

        modelBuilder.Entity<AuthUser>(entity =>
        {
            entity.ToTable("auth_users");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Username).HasMaxLength(100).IsRequired();
            entity.HasIndex(x => x.Username).IsUnique();
            entity.Property(x => x.DisplayName).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(320);
            entity.Property(x => x.PasswordHash).HasMaxLength(200).IsRequired();
            entity.Property(x => x.LockReason).HasMaxLength(500);
            entity.Ignore(x => x.DomainEvents);
            entity.Ignore(x => x.CompanyIds);
            entity.Ignore(x => x.PlantIds);
            entity.Ignore(x => x.Roles);

            entity.Property<List<string>>("_companyIds")
                .HasField("_companyIds")
                .HasColumnName("company_ids")
                .HasColumnType("text[]");

            entity.Property<List<string>>("_plantIds")
                .HasField("_plantIds")
                .HasColumnName("plant_ids")
                .HasColumnType("text[]");

            entity.Property<List<string>>("_roles")
                .HasField("_roles")
                .HasColumnName("roles")
                .HasColumnType("text[]");
        });

        modelBuilder.Entity<AuthSession>(entity =>
        {
            entity.ToTable("auth_sessions");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.RefreshTokenHash).HasMaxLength(128).IsRequired();
            entity.HasIndex(x => x.RefreshTokenHash).IsUnique();
            entity.Property(x => x.CompanyId).HasMaxLength(64).IsRequired();
            entity.Property(x => x.PlantId).HasMaxLength(64).IsRequired();
            entity.Ignore(x => x.DomainEvents);

            entity.OwnsOne(x => x.Device, device =>
            {
                device.Property(d => d.DeviceId).HasColumnName("device_id").HasMaxLength(100);
                device.Property(d => d.DeviceName).HasColumnName("device_name").HasMaxLength(200);
                device.Property(d => d.Browser).HasColumnName("browser").HasMaxLength(200);
                device.Property(d => d.OperatingSystem).HasColumnName("operating_system").HasMaxLength(200);
                device.Property(d => d.IpAddress).HasColumnName("ip_address").HasMaxLength(64);
                device.Property(d => d.Country).HasColumnName("country").HasMaxLength(100);
            });
        });

        modelBuilder.Entity<LoginHistoryEntry>(entity =>
        {
            entity.ToTable("auth_login_history");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Username).HasMaxLength(100).IsRequired();
            entity.Property(x => x.FailureReason).HasMaxLength(100);
            entity.Property(x => x.CorrelationId).HasMaxLength(64).IsRequired();

            entity.OwnsOne(x => x.Device, device =>
            {
                device.Property(d => d.DeviceId).HasColumnName("device_id").HasMaxLength(100);
                device.Property(d => d.DeviceName).HasColumnName("device_name").HasMaxLength(200);
                device.Property(d => d.Browser).HasColumnName("browser").HasMaxLength(200);
                device.Property(d => d.OperatingSystem).HasColumnName("operating_system").HasMaxLength(200);
                device.Property(d => d.IpAddress).HasColumnName("ip_address").HasMaxLength(64);
                device.Property(d => d.Country).HasColumnName("country").HasMaxLength(100);
            });
        });

        modelBuilder.Entity<OutboxMessage>(entity =>
        {
            entity.ToTable("outbox_messages");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.EventType).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Payload).HasColumnType("jsonb").IsRequired();
            entity.Property(x => x.CorrelationId).HasMaxLength(64).IsRequired();
            entity.Property(x => x.Source).HasMaxLength(100).IsRequired();
            entity.HasIndex(x => x.ProcessedAt);
        });
    }
}

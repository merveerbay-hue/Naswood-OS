using Microsoft.EntityFrameworkCore;
using Naswood.Modules.Platform.Application.Users;
using Naswood.Modules.Platform.Domain.Authentication;
using Naswood.Modules.Platform.Domain.Authorization;
using Naswood.Modules.Platform.Domain.Organization;

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

    public DbSet<PermissionDefinition> Permissions => Set<PermissionDefinition>();

    public DbSet<RoleDefinition> Roles => Set<RoleDefinition>();

    public DbSet<AuthorizationHistoryEntry> AuthorizationHistory => Set<AuthorizationHistoryEntry>();

    public DbSet<CompanyReference> Companies => Set<CompanyReference>();

    public DbSet<PlantReference> Plants => Set<PlantReference>();

    public DbSet<DepartmentReference> Departments => Set<DepartmentReference>();

    public DbSet<PositionReference> Positions => Set<PositionReference>();

    public DbSet<UserHistoryEntry> UserHistory => Set<UserHistoryEntry>();

    public DbSet<Naswood.Modules.Platform.Domain.Audit.AuditLogEntry> AuditLogs =>
        Set<Naswood.Modules.Platform.Domain.Audit.AuditLogEntry>();

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
            entity.HasIndex(x => x.Email)
                .IsUnique()
                .HasFilter("\"Email\" IS NOT NULL AND \"IsDeleted\" = FALSE");
            entity.Property(x => x.PasswordHash).HasMaxLength(200).IsRequired();
            entity.Property(x => x.LockReason).HasMaxLength(500);
            entity.Property(x => x.EmployeeNumber).HasMaxLength(30);
            entity.HasIndex(x => x.EmployeeNumber)
                .IsUnique()
                .HasFilter("\"EmployeeNumber\" IS NOT NULL AND \"IsDeleted\" = FALSE");
            entity.Property(x => x.FirstName).HasMaxLength(100);
            entity.Property(x => x.LastName).HasMaxLength(100);
            entity.Property(x => x.Phone).HasMaxLength(50);
            entity.Property(x => x.MobilePhone).HasMaxLength(50);
            entity.Property(x => x.AvatarUrl).HasMaxLength(500);
            entity.Property(x => x.DepartmentCode).HasMaxLength(30);
            entity.Property(x => x.PositionCode).HasMaxLength(30);
            entity.Property(x => x.CostCenter).HasMaxLength(50);
            entity.Property(x => x.EmploymentType).HasMaxLength(50);
            entity.Property(x => x.EmployeeCategory).HasMaxLength(50);
            entity.Property(x => x.Language).HasMaxLength(10);
            entity.Property(x => x.TimeZone).HasMaxLength(50);
            entity.Property(x => x.DateFormat).HasMaxLength(30);
            entity.Property(x => x.NumberFormat).HasMaxLength(30);
            entity.Property(x => x.Currency).HasMaxLength(10);
            entity.Property(x => x.Theme).HasMaxLength(30);
            entity.Property(x => x.DeleteReason).HasMaxLength(500);
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(30);
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

        modelBuilder.Entity<PermissionDefinition>(entity =>
        {
            entity.ToTable("permissions");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Code).HasMaxLength(200).IsRequired();
            entity.HasIndex(x => x.Code).IsUnique();
            entity.Property(x => x.Module).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Entity).HasMaxLength(100);
            entity.Property(x => x.Action).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Field).HasMaxLength(100);
            entity.Property(x => x.DisplayName).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Category).HasMaxLength(100);
            entity.Property(x => x.Description).HasMaxLength(1000);
            entity.Ignore(x => x.DomainEvents);
            entity.Ignore(x => x.DependsOn);
            entity.Property<List<string>>("_dependsOn")
                .HasField("_dependsOn")
                .HasColumnName("depends_on")
                .HasColumnType("text[]");
        });

        modelBuilder.Entity<RoleDefinition>(entity =>
        {
            entity.ToTable("roles");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Code).HasMaxLength(100).IsRequired();
            entity.HasIndex(x => x.Code).IsUnique();
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(1000);
            entity.Property(x => x.CompanyCode).HasMaxLength(20);
            entity.Property(x => x.PlantCode).HasMaxLength(20);
            entity.Property(x => x.DepartmentCode).HasMaxLength(30);
            entity.Property(x => x.Category).HasMaxLength(100);
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(30);
            entity.Ignore(x => x.DomainEvents);
            entity.Ignore(x => x.PermissionCodes);
            entity.Property<List<string>>("_permissionCodes")
                .HasField("_permissionCodes")
                .HasColumnName("permission_codes")
                .HasColumnType("text[]");
        });

        modelBuilder.Entity<AuthorizationHistoryEntry>(entity =>
        {
            entity.ToTable("authorization_history");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Permission).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Reason).HasMaxLength(500);
            entity.Property(x => x.DenialCode).HasMaxLength(50);
            entity.Property(x => x.CompanyId).HasMaxLength(64);
            entity.Property(x => x.PlantId).HasMaxLength(64);
            entity.Property(x => x.ResourceOwnerId).HasMaxLength(64);
            entity.Property(x => x.Field).HasMaxLength(100);
            entity.Property(x => x.CorrelationId).HasMaxLength(64).IsRequired();
            entity.HasIndex(x => x.OccurredAt);
        });

        modelBuilder.Entity<CompanyReference>(entity =>
        {
            entity.ToTable("org_companies");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Code).HasMaxLength(20).IsRequired();
            entity.HasIndex(x => x.Code).IsUnique();
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entity.Ignore(x => x.DomainEvents);
        });

        modelBuilder.Entity<PlantReference>(entity =>
        {
            entity.ToTable("org_plants");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Code).HasMaxLength(20).IsRequired();
            entity.HasIndex(x => x.Code).IsUnique();
            entity.Property(x => x.Name).HasMaxLength(150).IsRequired();
            entity.Property(x => x.CompanyCode).HasMaxLength(20).IsRequired();
            entity.Ignore(x => x.DomainEvents);
        });

        modelBuilder.Entity<DepartmentReference>(entity =>
        {
            entity.ToTable("org_departments");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Code).HasMaxLength(20).IsRequired();
            entity.HasIndex(x => x.Code).IsUnique();
            entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
            entity.Ignore(x => x.DomainEvents);
        });

        modelBuilder.Entity<PositionReference>(entity =>
        {
            entity.ToTable("org_positions");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Code).HasMaxLength(20).IsRequired();
            entity.HasIndex(x => x.Code).IsUnique();
            entity.Property(x => x.Title).HasMaxLength(100).IsRequired();
            entity.Ignore(x => x.DomainEvents);
        });

        modelBuilder.Entity<UserHistoryEntry>(entity =>
        {
            entity.ToTable("user_history");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Action).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Details).HasMaxLength(1000);
            entity.Property(x => x.CorrelationId).HasMaxLength(64).IsRequired();
            entity.HasIndex(x => x.OccurredAt);
        });

        modelBuilder.Entity<Naswood.Modules.Platform.Domain.Audit.AuditLogEntry>(entity =>
        {
            entity.ToTable("audit_logs");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Module).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Entity).HasMaxLength(100);
            entity.Property(x => x.EntityId).HasMaxLength(100);
            entity.Property(x => x.Action).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Username).HasMaxLength(100);
            entity.Property(x => x.OldValuesJson).HasColumnType("jsonb");
            entity.Property(x => x.NewValuesJson).HasColumnType("jsonb");
            entity.Property(x => x.IpAddress).HasMaxLength(64);
            entity.Property(x => x.Browser).HasMaxLength(200);
            entity.Property(x => x.Device).HasMaxLength(200);
            entity.Property(x => x.OperatingSystem).HasMaxLength(200);
            entity.Property(x => x.CorrelationId).HasMaxLength(64).IsRequired();
            entity.Property(x => x.CompanyId).HasMaxLength(64);
            entity.Property(x => x.PlantId).HasMaxLength(64);
            entity.Property(x => x.Severity).HasMaxLength(30).IsRequired();
            entity.Property(x => x.Status).HasMaxLength(30).IsRequired();
            entity.HasIndex(x => x.OccurredAt);
            entity.HasIndex(x => x.EntityId);
            entity.HasIndex(x => x.UserId);
            entity.HasIndex(x => x.Module);
        });
    }
}

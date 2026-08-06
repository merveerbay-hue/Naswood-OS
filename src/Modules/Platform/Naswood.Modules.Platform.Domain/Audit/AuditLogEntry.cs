using Naswood.BuildingBlocks.Domain;

namespace Naswood.Modules.Platform.Domain.Audit;

public static class AuditErrors
{
    public static Error NotFound() =>
        Error.NotFound("AUDIT-001", "Audit record was not found.");

    public static Error Validation(string message) =>
        Error.Validation("AUDIT-002", message);

    public static Error Forbidden() =>
        Error.Forbidden("AUDIT-003", "Audit access denied.");
}

/// <summary>
/// Immutable audit record. Physical updates/deletes are not supported.
/// </summary>
public sealed class AuditLogEntry : Entity<Guid>
{
    private AuditLogEntry()
    {
    }

    private AuditLogEntry(
        Guid id,
        DateTimeOffset occurredAt,
        Guid? userId,
        string? username,
        string module,
        string? entity,
        string? entityId,
        string action,
        string? oldValuesJson,
        string? newValuesJson,
        string? ipAddress,
        string? browser,
        string? device,
        string? operatingSystem,
        Guid? sessionId,
        string correlationId,
        string? companyId,
        string? plantId,
        string severity,
        string status)
        : base(id)
    {
        OccurredAt = occurredAt;
        CreatedAt = occurredAt;
        UserId = userId;
        Username = username;
        Module = module;
        Entity = entity;
        EntityId = entityId;
        Action = action;
        OldValuesJson = oldValuesJson;
        NewValuesJson = newValuesJson;
        IpAddress = ipAddress;
        Browser = browser;
        Device = device;
        OperatingSystem = operatingSystem;
        SessionId = sessionId;
        CorrelationId = correlationId;
        CompanyId = companyId;
        PlantId = plantId;
        Severity = severity;
        Status = status;
    }

    public DateTimeOffset OccurredAt { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public Guid? UserId { get; private set; }

    public string? Username { get; private set; }

    public string Module { get; private set; } = string.Empty;

    public string? Entity { get; private set; }

    public string? EntityId { get; private set; }

    public string Action { get; private set; } = string.Empty;

    public string? OldValuesJson { get; private set; }

    public string? NewValuesJson { get; private set; }

    public string? IpAddress { get; private set; }

    public string? Browser { get; private set; }

    public string? Device { get; private set; }

    public string? OperatingSystem { get; private set; }

    public Guid? SessionId { get; private set; }

    public string CorrelationId { get; private set; } = string.Empty;

    public string? CompanyId { get; private set; }

    public string? PlantId { get; private set; }

    public string Severity { get; private set; } = "Information";

    public string Status { get; private set; } = "Success";

    public static AuditLogEntry Create(AuditWriteModel model)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(model.Module);
        ArgumentException.ThrowIfNullOrWhiteSpace(model.Action);
        ArgumentException.ThrowIfNullOrWhiteSpace(model.CorrelationId);

        return new AuditLogEntry(
            UuidV7.NewGuid(),
            model.OccurredAt,
            model.UserId,
            string.IsNullOrWhiteSpace(model.Username) ? null : model.Username.Trim(),
            model.Module.Trim(),
            string.IsNullOrWhiteSpace(model.Entity) ? null : model.Entity.Trim(),
            string.IsNullOrWhiteSpace(model.EntityId) ? null : model.EntityId.Trim(),
            model.Action.Trim(),
            model.OldValuesJson,
            model.NewValuesJson,
            model.IpAddress,
            model.Browser,
            model.Device,
            model.OperatingSystem,
            model.SessionId,
            model.CorrelationId.Trim(),
            model.CompanyId,
            model.PlantId,
            string.IsNullOrWhiteSpace(model.Severity) ? "Information" : model.Severity.Trim(),
            string.IsNullOrWhiteSpace(model.Status) ? "Success" : model.Status.Trim());
    }
}

public sealed class AuditWriteModel
{
    public required DateTimeOffset OccurredAt { get; init; }

    public Guid? UserId { get; init; }

    public string? Username { get; init; }

    public required string Module { get; init; }

    public string? Entity { get; init; }

    public string? EntityId { get; init; }

    public required string Action { get; init; }

    public string? OldValuesJson { get; init; }

    public string? NewValuesJson { get; init; }

    public string? IpAddress { get; init; }

    public string? Browser { get; init; }

    public string? Device { get; init; }

    public string? OperatingSystem { get; init; }

    public Guid? SessionId { get; init; }

    public required string CorrelationId { get; init; }

    public string? CompanyId { get; init; }

    public string? PlantId { get; init; }

    public string? Severity { get; init; }

    public string? Status { get; init; }
}

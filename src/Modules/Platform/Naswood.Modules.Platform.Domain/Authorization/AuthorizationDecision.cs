using Naswood.BuildingBlocks.Domain;

namespace Naswood.Modules.Platform.Domain.Authorization;

public sealed class AuthorizationDecision
{
    private AuthorizationDecision(
        bool allowed,
        string permission,
        string? reason,
        string? denialCode)
    {
        Allowed = allowed;
        Permission = permission;
        Reason = reason;
        DenialCode = denialCode;
    }

    public bool Allowed { get; }

    public string Permission { get; }

    public string? Reason { get; }

    public string? DenialCode { get; }

    public static AuthorizationDecision Allow(string permission) =>
        new(true, permission, null, null);

    public static AuthorizationDecision Deny(string permission, Error error) =>
        new(false, permission, error.Message, error.Code);
}

/// <summary>
/// Append-only authorization evaluation record for auditability.
/// </summary>
public sealed class AuthorizationHistoryEntry : Entity<Guid>
{
    private AuthorizationHistoryEntry()
    {
    }

    private AuthorizationHistoryEntry(
        Guid id,
        Guid userId,
        string permission,
        bool allowed,
        string? reason,
        string? denialCode,
        string? companyId,
        string? plantId,
        string? resourceOwnerId,
        string? field,
        string correlationId,
        DateTimeOffset occurredAt)
        : base(id)
    {
        UserId = userId;
        Permission = permission;
        Allowed = allowed;
        Reason = reason;
        DenialCode = denialCode;
        CompanyId = companyId;
        PlantId = plantId;
        ResourceOwnerId = resourceOwnerId;
        Field = field;
        CorrelationId = correlationId;
        OccurredAt = occurredAt;
    }

    public Guid UserId { get; private set; }

    public string Permission { get; private set; } = string.Empty;

    public bool Allowed { get; private set; }

    public string? Reason { get; private set; }

    public string? DenialCode { get; private set; }

    public string? CompanyId { get; private set; }

    public string? PlantId { get; private set; }

    public string? ResourceOwnerId { get; private set; }

    public string? Field { get; private set; }

    public string CorrelationId { get; private set; } = string.Empty;

    public DateTimeOffset OccurredAt { get; private set; }

    public static AuthorizationHistoryEntry Create(
        Guid userId,
        AuthorizationDecision decision,
        string? companyId,
        string? plantId,
        string? resourceOwnerId,
        string? field,
        string correlationId,
        DateTimeOffset occurredAt) =>
        new(
            UuidV7.NewGuid(),
            userId,
            decision.Permission,
            decision.Allowed,
            decision.Reason,
            decision.DenialCode,
            companyId,
            plantId,
            resourceOwnerId,
            field,
            correlationId,
            occurredAt);
}

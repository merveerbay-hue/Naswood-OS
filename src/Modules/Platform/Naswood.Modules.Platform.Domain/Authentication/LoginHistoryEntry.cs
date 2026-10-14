using Naswood.BuildingBlocks.Domain;

namespace Naswood.Modules.Platform.Domain.Authentication;

/// <summary>
/// Append-only authentication attempt record (Login History).
/// </summary>
public sealed class LoginHistoryEntry : Entity<Guid>
{
    private LoginHistoryEntry()
    {
    }

    private LoginHistoryEntry(
        Guid id,
        Guid? userId,
        string username,
        bool succeeded,
        string? failureReason,
        Guid? sessionId,
        DeviceInfo device,
        DateTimeOffset occurredAt,
        string correlationId)
        : base(id)
    {
        UserId = userId;
        Username = username;
        Succeeded = succeeded;
        FailureReason = failureReason;
        SessionId = sessionId;
        Device = device;
        OccurredAt = occurredAt;
        CorrelationId = correlationId;
    }

    public Guid? UserId { get; private set; }

    public string Username { get; private set; } = string.Empty;

    public bool Succeeded { get; private set; }

    public string? FailureReason { get; private set; }

    public Guid? SessionId { get; private set; }

    public DeviceInfo Device { get; private set; } = new(null, null, null, null, null, null);

    public DateTimeOffset OccurredAt { get; private set; }

    public string CorrelationId { get; private set; } = string.Empty;

    public static LoginHistoryEntry Success(
        Guid userId,
        string username,
        Guid sessionId,
        DeviceInfo device,
        DateTimeOffset occurredAt,
        string correlationId) =>
        new(
            UuidV7.NewGuid(),
            userId,
            username,
            true,
            null,
            sessionId,
            device,
            occurredAt,
            correlationId);

    public static LoginHistoryEntry Failure(
        Guid? userId,
        string username,
        string failureReason,
        DeviceInfo device,
        DateTimeOffset occurredAt,
        string correlationId) =>
        new(
            UuidV7.NewGuid(),
            userId,
            username,
            false,
            failureReason,
            null,
            device,
            occurredAt,
            correlationId);
}

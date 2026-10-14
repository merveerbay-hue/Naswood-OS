namespace Naswood.Modules.Business.Domain.Production.En14081;

/// <summary>
/// EN 14081 Structural Production Lot — schema freeze vocabulary and aggregate map.
/// Persistence/UI for child layers land in later phases; do not treat missing children as optional forever.
/// Authority doc: docs/05_Modules/07_Quality/EN14081_Structural_Production_Lot_Master_Schema.md
/// </summary>
public static class StructuralProductionLotSchema
{
    public const string DocumentId = "EN14081-SPL-MASTER-SCHEMA";
    public const string Version = "1.0";

    /// <summary>Lot lifecycle statuses (target). CLASSIFIED ≠ RELEASED.</summary>
    public static class LotStatus
    {
        public const string Draft = "DRAFT";
        public const string InClassification = "IN_CLASSIFICATION";
        public const string Classified = "CLASSIFIED";
        public const string FpcPending = "FPC_PENDING";
        public const string Released = "RELEASED";
        public const string Cancelled = "CANCELLED";

        /// <summary>Foundation aliases accepted until data migrated / callers updated.</summary>
        public const string LegacyInProgress = "IN_PROGRESS";
        public const string LegacyPendingClassification = "PENDING_CLASSIFICATION";
        public const string LegacyPendingQuality = "PENDING_QUALITY";
        public const string LegacyQuarantined = "QUARANTINED";

        public static readonly IReadOnlyList<string> Target =
        [
            Draft, InClassification, Classified, FpcPending, Released, Cancelled
        ];

        public static readonly IReadOnlyList<string> AcceptedForTransition =
        [
            Draft, InClassification, Classified, FpcPending, Released,
            LegacyInProgress, LegacyPendingClassification, LegacyPendingQuality, LegacyQuarantined
        ];

        public static string Normalize(string? status)
        {
            var s = (status ?? string.Empty).Trim().ToUpperInvariant();
            return s switch
            {
                LegacyInProgress or LegacyPendingClassification => InClassification,
                LegacyPendingQuality => FpcPending,
                LegacyQuarantined => FpcPending, // prefer NC HOLD; lot stays gated
                _ => s
            };
        }

        public static bool IsDraft(string? status) =>
            string.Equals(status, Draft, StringComparison.OrdinalIgnoreCase);

        public static bool AllowsCriticalFieldEdit(string? status) => IsDraft(status);
    }

    public static class ClassificationMethod
    {
        public const string Visual = "VISUAL";
        public const string Machine = "MACHINE";

        public static readonly IReadOnlyList<string> All = [Visual, Machine];
    }

    public static class ClassificationResult
    {
        public const string Pass = "PASS";
        public const string Reject = "REJECT";
        public const string Downgrade = "DOWNGRADE";

        // Visual form wording aliases
        public const string Accepted = "ACCEPTED";
        public const string Rejected = "REJECTED";
        public const string Downgraded = "DOWNGRADED";
    }

    public static class MoistureResult
    {
        public const string Pass = "PASS";
        public const string Fail = "FAIL";
        public const string Hold = "HOLD";
    }

    public static class NcDisposition
    {
        public const string Hold = "HOLD";
        public const string Rework = "REWORK";
        public const string Reject = "REJECT";
        public const string ReleaseWithConcession = "RELEASE_WITH_CONCESSION";
    }

    /// <summary>
    /// Ordered child layers of the aggregate. Used for docs, self-tests, and future evidence-pack projection.
    /// </summary>
    public static readonly IReadOnlyList<SchemaLayer> Layers =
    [
        new("LotIdentity", "Lot identity (number, plant, material, method, period)", SchemaLayerState.Implemented),
        new("InputTraceability", "Source batch/balance/package links (no stock posting yet)", SchemaLayerState.ImplementedPartial),
        new("Classification", "ClassificationRecord → Result → StrengthClassAssignment", SchemaLayerState.Planned),
        new("MoistureControl", "Meter + verification + measurements + result", SchemaLayerState.Planned),
        new("VisualClassification", "Annex A–oriented visual grading record", SchemaLayerState.Planned),
        new("MachineClassification", "Machine + settings snapshot + operator + result", SchemaLayerState.Planned),
        new("StrengthClass", "Assigned only via classification result chain", SchemaLayerState.Planned),
        new("DimensionsTolerances", "Nominal vs measured + tolerance result", SchemaLayerState.Planned),
        new("FpcControls", "Control plan executions + acceptance criteria", SchemaLayerState.Planned),
        new("Nonconformity", "Lot-linked NC + disposition", SchemaLayerState.Planned),
        new("CorrectiveAction", "CA + verification under NC", SchemaLayerState.Planned),
        new("EquipmentMeasurement", "Shared measuring device / verification masters", SchemaLayerState.Planned),
        new("Marking", "Product identification / marking record", SchemaLayerState.Planned),
        new("Release", "Release gate evidence (≠ CLASSIFIED)", SchemaLayerState.ImplementedPartial),
        new("AuditEvidence", "Cancel audit + full history projection", SchemaLayerState.ImplementedPartial),
    ];

    /// <summary>Revised delivery order after foundation.</summary>
    public static readonly IReadOnlyList<string> PhaseOrder =
    [
        "FAZ1_VisualClassification",
        "FAZ2_MoistureAndMeterVerification",
        "FAZ3_ClassificationResultAndStrengthClass",
        "FAZ4_FpcPlanAndExecutions",
        "FAZ5_NonconformityAndCorrectiveAction",
        "FAZ6_MachineClassificationAndSettings",
        "FAZ7_StockConsumptionAndGenealogy",
        "FAZ8_MarkingAndProductIdentification",
        "FAZ9_CeAndDop"
    ];
}

public enum SchemaLayerState
{
    Planned = 0,
    ImplementedPartial = 1,
    Implemented = 2
}

public sealed record SchemaLayer(string Code, string Description, SchemaLayerState State);

/// <summary>
/// Planned child shapes (schema freeze). Not EF-mapped yet — prevents premature StrengthClass-on-lot designs.
/// </summary>
public static class StructuralProductionLotPlannedTypes
{
    public sealed record ClassificationRecordShape(
        Guid ProductionLotId,
        string Method,
        string? OperatorId,
        string? CriteriaSetCode,
        string? StandardRef,
        string Result,
        DateTimeOffset? StartedAt,
        DateTimeOffset? CompletedAt);

    public sealed record StrengthClassAssignmentShape(
        Guid ClassificationRecordId,
        string StrengthClassCode,
        string? CriteriaSetCode);

    public sealed record MoistureControlShape(
        Guid ProductionLotId,
        string? MeterCode,
        Guid? MeterVerificationId,
        string Result);

    public sealed record MoistureMeasurementShape(
        Guid MoistureControlId,
        string? SampleRef,
        decimal ValuePercent,
        string? Species,
        string? OperatorId,
        DateTimeOffset MeasuredAt);

    public sealed record VisualClassificationRecordShape(
        Guid ProductionLotId,
        Guid? MoistureControlId,
        string Result);

    public sealed record VisualCharacteristicEvaluationShape(
        Guid VisualClassificationRecordId,
        string CharacteristicCode,
        string? ObservedValue,
        string? LimitRef,
        string EvaluationResult);

    public sealed record MachineClassificationRecordShape(
        Guid ProductionLotId,
        string MachineCode,
        string MachineSettingsSnapshotJson,
        string? OperatorId,
        string Result);

    public sealed record FpcControlExecutionShape(
        Guid ProductionLotId,
        string ControlType,
        string? Frequency,
        string AcceptanceCriteria,
        string Result,
        string? ResponsiblePerson,
        DateTimeOffset RecordedAt);

    public sealed record NonConformityShape(
        Guid ProductionLotId,
        string Number,
        string Reason,
        string Disposition,
        string? DetectionSource);

    public sealed record CorrectiveActionShape(
        Guid NonConformityId,
        string Action,
        string? Owner,
        string VerificationResult);

    public sealed record MarkingRecordShape(
        Guid ProductionLotId,
        string MarkingPayload,
        DateTimeOffset AppliedAt,
        string? AppliedBy);

    public sealed record ReleaseRecordShape(
        Guid ProductionLotId,
        string ReleasedBy,
        DateTimeOffset ReleasedAt,
        string? Notes);
}

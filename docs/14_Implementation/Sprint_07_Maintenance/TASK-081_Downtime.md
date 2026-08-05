# ==============================================================================
# TASK-081 — IMPLEMENTATION
# DOWNTIME MANAGEMENT
# Naswood Operating System (NOS)
# Module: Maintenance Management
# Sprint: Sprint 07 – Maintenance
# Status: Ready for Development
# ==============================================================================

# OBJECTIVE

Implement the Downtime Management aggregate responsible for recording,
classifying, analyzing and reporting all planned and unplanned equipment
downtime across manufacturing operations.

Downtime represents periods during which an Asset or Production Line is unable
to produce according to its planned schedule.

Downtime impacts OEE, Production Capacity and Maintenance KPIs.

Downtime records operational events only.

It never changes Production Orders or Maintenance Work Orders directly.

---

# DOMAIN

Maintenance Management

Aggregate Root

```
Downtime
```

---

# REFERENCES

Implementation must comply with:

- Constitution
- Maintenance_Architecture.md
- Maintenance_Workflow.md
- Maintenance_API.md
- Production_Architecture.md
- OEE Architecture
- TASK-076_Asset.md
- TASK-078_Maintenance_Order.md
- TASK-080_Corrective_Maintenance.md

---

# DEPENDENCIES

Requires completed modules:

- Asset
- Production Line
- Work Center
- Machine
- Production Order
- Maintenance Work Order
- Employee
- OEE

---

# AGGREGATE

```
Downtime
```

Children

```
DowntimeReason

DowntimeCategory

DowntimeComment

DowntimeAttachment

DowntimeHistory

AuditEntry
```

---

# VALUE OBJECTS

```
DowntimeNumber

DowntimeStatus

DowntimeType

DowntimeDuration

DowntimeReasonCode
```

---

# ENUMS

## DowntimeStatus

```text
Open

Running

Resolved

Verified

Closed

Cancelled
```

---

## DowntimeType

```text
Planned

Unplanned
```

---

## DowntimeCategory

```text
Mechanical

Electrical

Hydraulic

Pneumatic

Software

Setup

MaterialShortage

QualityIssue

Operator

UtilityFailure

Maintenance

Safety

Other
```

---

## DowntimeSeverity

```text
Low

Medium

High

Critical
```

---

# ENTITY FIELDS

```text
Id

DowntimeNumber

AssetId

ProductionLineId

WorkCenterId

MachineId

ProductionOrderId

MaintenanceWorkOrderId

Category

ReasonCode

Type

Severity

Status

Description

StartedAt

EndedAt

DurationMinutes

ReportedBy

ResolvedBy

VerifiedBy

CreatedAt

UpdatedAt
```

---

# DOWNTIME REASON

```text
Id

DowntimeId

ReasonCode

ReasonDescription

RootCause

CorrectiveAction

Verified
```

---

# DOMAIN INVARIANTS

Every Downtime references exactly one Asset.

Open Downtime has no End Time.

Closed Downtime is immutable.

Duration is calculated automatically.

Downtime cannot overlap another Active Downtime for the same Asset.

Every Downtime has one Category and one Reason Code.

---

# DOMAIN METHODS

```text
Create()

Start()

Pause()

Resume()

Resolve()

Verify()

Close()

Cancel()

AssignReason()

CalculateDuration()
```

---

# DOMAIN EVENTS

```text
DowntimeStarted

DowntimePaused

DowntimeResumed

DowntimeResolved

DowntimeVerified

DowntimeClosed

DowntimeCancelled

DowntimeReasonAssigned
```

---

# VALIDATIONS

Create

- Asset exists
- Asset Active

Start

- Asset not already in Active Downtime

Resolve

- End Time defined
- Reason assigned

Verify

- Resolution completed

Close

- Verification completed

---

# REPOSITORY

```text
IDowntimeRepository
```

Methods

```csharp
Task<Downtime?> GetByIdAsync(Guid id);

Task<Downtime?> GetByNumberAsync(string downtimeNumber);

Task<IEnumerable<Downtime>> GetOpenAsync();

Task<IEnumerable<Downtime>> GetByAssetAsync(Guid assetId);

Task AddAsync(Downtime entity);

Task UpdateAsync(Downtime entity);
```

---

# COMMANDS

```text
CreateDowntimeCommand

StartDowntimeCommand

PauseDowntimeCommand

ResumeDowntimeCommand

ResolveDowntimeCommand

VerifyDowntimeCommand

CloseDowntimeCommand

CancelDowntimeCommand

AssignDowntimeReasonCommand
```

---

# QUERIES

```text
GetDowntimeByIdQuery

GetDowntimesQuery

GetOpenDowntimesQuery

GetAssetDowntimeHistoryQuery

GetDowntimeStatisticsQuery
```

---

# API ENDPOINTS

```http
GET    /api/v1/maintenance/downtime

GET    /api/v1/maintenance/downtime/{id}

POST   /api/v1/maintenance/downtime

POST   /api/v1/maintenance/downtime/{id}/start

POST   /api/v1/maintenance/downtime/{id}/pause

POST   /api/v1/maintenance/downtime/{id}/resume

POST   /api/v1/maintenance/downtime/{id}/resolve

POST   /api/v1/maintenance/downtime/{id}/verify

POST   /api/v1/maintenance/downtime/{id}/close

POST   /api/v1/maintenance/downtime/{id}/cancel

POST   /api/v1/maintenance/downtime/{id}/reason
```

---

# AUTHORIZATION

```text
maintenance.downtime.read

maintenance.downtime.create

maintenance.downtime.update

maintenance.downtime.resolve

maintenance.downtime.verify

maintenance.downtime.close

maintenance.downtime.cancel
```

---

# DATABASE TABLES

## Downtimes

```text
Id

DowntimeNumber

AssetId

ProductionLineId

WorkCenterId

MachineId

ProductionOrderId

MaintenanceWorkOrderId

Category

ReasonCode

Type

Severity

Status

Description

StartedAt

EndedAt

DurationMinutes

ReportedBy

ResolvedBy

VerifiedBy

CreatedAt

UpdatedAt
```

---

## DowntimeReasons

```text
Id

DowntimeId

ReasonCode

ReasonDescription

RootCause

CorrectiveAction

Verified
```

---

# INDEXES

```text
IX_DowntimeNumber (Unique)

IX_AssetId

IX_ProductionLineId

IX_WorkCenterId

IX_MachineId

IX_Status

IX_Category

IX_StartedAt
```

---

# AUDIT

Audit every

- Downtime creation
- Start
- Pause
- Resume
- Resolution
- Verification
- Closure
- Cancellation
- Reason assignment

Capture

```text
UserId

Timestamp

Action

OldValue

NewValue

CorrelationId
```

---

# TESTS

## Unit Tests

- Create Downtime
- Start Downtime
- Pause Downtime
- Resume Downtime
- Resolve Downtime
- Verify Downtime
- Close Downtime
- Prevent overlapping Downtime
- Calculate Duration automatically
- Prevent modification after Closure

## Integration Tests

- Repository
- Commands
- Queries
- REST API
- Asset integration
- Maintenance Work Order integration
- OEE integration
- Domain Events
- Audit

---

# ACCEPTANCE CRITERIA

- Every Downtime references a valid Asset.
- Planned and Unplanned Downtime are supported.
- Duration is calculated automatically.
- Overlapping Downtime is prevented.
- Downtime integrates with OEE calculations.
- Closed Downtime records become immutable.
- CQRS architecture is respected.
- Domain Events are published.
- API integration tests pass.
- Audit logging is complete.
- All unit and integration tests succeed.

---

# DEFINITION OF DONE

- Domain implemented
- Application layer implemented
- Infrastructure implemented
- REST API completed
- CQRS completed
- Validation rules completed
- Asset integration completed
- OEE integration completed
- Maintenance Work Order integration completed
- Authorization implemented
- Audit implemented
- Domain Events implemented
- Unit tests passing
- Integration tests passing
- Code review approved

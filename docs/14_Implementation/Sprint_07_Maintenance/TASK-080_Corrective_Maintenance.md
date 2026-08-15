# ==============================================================================
# TASK-080 — IMPLEMENTATION
# CORRECTIVE MAINTENANCE
# Naswood Operating System (NOS)
# Module: Maintenance Management
# Sprint: Sprint 07 – Maintenance
# Status: Ready for Development
# ==============================================================================

# OBJECTIVE

Implement the Corrective Maintenance aggregate responsible for managing
maintenance activities required to restore failed assets to their intended
operating condition.

Corrective Maintenance is initiated after equipment failure, abnormal condition
or performance degradation.

Corrective Maintenance executes through Maintenance Work Orders.

It never modifies Asset Master data directly.

---

# DOMAIN

Maintenance Management

Aggregate Root

```
CorrectiveMaintenance
```

---

# REFERENCES

Implementation must comply with:

- Constitution
- Maintenance_Architecture.md
- Maintenance_Workflow.md
- Maintenance_API.md
- TASK-076_Asset.md
- TASK-077_Work_Request.md
- TASK-078_Maintenance_Order.md
- Inventory Architecture
- Notification Architecture

---

# DEPENDENCIES

Requires completed modules:

- Asset
- Work Request
- Maintenance Work Order
- Employee
- Inventory
- Warehouse
- Spare Parts
- Notification

---

# AGGREGATE

```
CorrectiveMaintenance
```

Children

```
FailureAnalysis

CorrectiveTask

RootCauseAnalysis

ConsumedMaterial

LaborEntry

DowntimeRecord

AuditEntry
```

---

# VALUE OBJECTS

```
CorrectiveMaintenanceNumber

FailureType

FailureSeverity

MaintenancePriority

DowntimeDuration
```

---

# ENUMS

## CorrectiveMaintenanceStatus

```text
Draft

Open

Assigned

InProgress

WaitingForParts

WaitingForApproval

Completed

Verified

Closed

Cancelled
```

---

## FailureType

```text
Mechanical

Electrical

Hydraulic

Pneumatic

Software

ControlSystem

OperatorError

UtilityFailure

Other
```

---

## FailureSeverity

```text
Low

Medium

High

Critical

SafetyCritical
```

---

## MaintenancePriority

```text
Low

Medium

High

Critical

Emergency
```

---

# ENTITY FIELDS

```text
Id

CorrectiveMaintenanceNumber

AssetId

WorkRequestId

MaintenanceWorkOrderId

FailureType

FailureSeverity

Priority

Status

FailureCode

FailureDescription

FailureDetectedAt

DowntimeStartedAt

DowntimeEndedAt

RootCauseSummary

VerifiedBy

ClosedBy

CreatedAt

UpdatedAt
```

---

# FAILURE ANALYSIS

```text
Id

CorrectiveMaintenanceId

FailureMode

FailureCause

FailureEffect

DetectionMethod

PerformedBy

PerformedAt
```

---

# ROOT CAUSE ANALYSIS

```text
Id

CorrectiveMaintenanceId

AnalysisMethod

Description

PerformedBy

PerformedAt
```

Supported Methods

- 5 Why
- Fishbone
- Fault Tree
- FMEA
- Custom

---

# DOWNTIME RECORD

```text
Id

CorrectiveMaintenanceId

StartTime

EndTime

DurationMinutes

ProductionImpact

DowntimeReason
```

---

# DOMAIN INVARIANTS

Every Corrective Maintenance references exactly one Asset.

Every Corrective Maintenance generates or references one Maintenance Work Order.

Closed Corrective Maintenance records are immutable.

Downtime cannot be negative.

Verification is mandatory before Closure.

Critical Failures require Root Cause Analysis.

---

# DOMAIN METHODS

```text
Create()

Assign()

Start()

RecordFailure()

StartDowntime()

EndDowntime()

PerformRootCauseAnalysis()

ConsumeMaterial()

RecordLabor()

Complete()

Verify()

Close()

Cancel()
```

---

# DOMAIN EVENTS

```text
CorrectiveMaintenanceCreated

MaintenanceAssigned

FailureRecorded

DowntimeStarted

DowntimeEnded

RootCauseCompleted

MaterialConsumed

LaborRecorded

CorrectiveMaintenanceCompleted

CorrectiveMaintenanceVerified

CorrectiveMaintenanceClosed

CorrectiveMaintenanceCancelled
```

---

# VALIDATIONS

Create

- Asset exists
- Failure Code valid

Assign

- Technician assigned

Start

- Maintenance Work Order released

Complete

- Required maintenance tasks completed

Verify

- Root Cause completed for Critical failures

Close

- Verification completed
- Downtime closed
- Work Order closed

---

# REPOSITORY

```text
ICorrectiveMaintenanceRepository
```

Methods

```csharp
Task<CorrectiveMaintenance?> GetByIdAsync(Guid id);

Task<CorrectiveMaintenance?> GetByNumberAsync(string number);

Task<IEnumerable<CorrectiveMaintenance>> GetOpenAsync();

Task<IEnumerable<CorrectiveMaintenance>> GetByAssetAsync(Guid assetId);

Task AddAsync(CorrectiveMaintenance entity);

Task UpdateAsync(CorrectiveMaintenance entity);
```

---

# COMMANDS

```text
CreateCorrectiveMaintenanceCommand

AssignCorrectiveMaintenanceCommand

StartCorrectiveMaintenanceCommand

RecordFailureCommand

StartDowntimeCommand

EndDowntimeCommand

CompleteRootCauseCommand

ConsumeMaterialCommand

RecordLaborCommand

CompleteCorrectiveMaintenanceCommand

VerifyCorrectiveMaintenanceCommand

CloseCorrectiveMaintenanceCommand

CancelCorrectiveMaintenanceCommand
```

---

# QUERIES

```text
GetCorrectiveMaintenanceByIdQuery

GetCorrectiveMaintenancesQuery

GetOpenCorrectiveMaintenancesQuery

GetAssetCorrectiveHistoryQuery

GetFailureStatisticsQuery
```

---

# API ENDPOINTS

```http
GET    /api/v1/maintenance/corrective

GET    /api/v1/maintenance/corrective/{id}

POST   /api/v1/maintenance/corrective

PUT    /api/v1/maintenance/corrective/{id}

POST   /api/v1/maintenance/corrective/{id}/assign

POST   /api/v1/maintenance/corrective/{id}/start

POST   /api/v1/maintenance/corrective/{id}/record-failure

POST   /api/v1/maintenance/corrective/{id}/start-downtime

POST   /api/v1/maintenance/corrective/{id}/end-downtime

POST   /api/v1/maintenance/corrective/{id}/root-cause

POST   /api/v1/maintenance/corrective/{id}/consume-material

POST   /api/v1/maintenance/corrective/{id}/record-labor

POST   /api/v1/maintenance/corrective/{id}/complete

POST   /api/v1/maintenance/corrective/{id}/verify

POST   /api/v1/maintenance/corrective/{id}/close

POST   /api/v1/maintenance/corrective/{id}/cancel
```

---

# AUTHORIZATION

```text
maintenance.corrective.read

maintenance.corrective.create

maintenance.corrective.assign

maintenance.corrective.execute

maintenance.corrective.verify

maintenance.corrective.close

maintenance.corrective.cancel
```

---

# DATABASE TABLES

## CorrectiveMaintenances

```text
Id

CorrectiveMaintenanceNumber

AssetId

WorkRequestId

MaintenanceWorkOrderId

FailureType

FailureSeverity

Priority

Status

FailureCode

FailureDescription

FailureDetectedAt

DowntimeStartedAt

DowntimeEndedAt

RootCauseSummary

VerifiedBy

ClosedBy

CreatedAt

UpdatedAt
```

---

## FailureAnalyses

```text
Id

CorrectiveMaintenanceId

FailureMode

FailureCause

FailureEffect

DetectionMethod

PerformedBy

PerformedAt
```

---

## DowntimeRecords

```text
Id

CorrectiveMaintenanceId

StartTime

EndTime

DurationMinutes

ProductionImpact

DowntimeReason
```

---

# INDEXES

```text
IX_CorrectiveMaintenanceNumber (Unique)

IX_AssetId

IX_WorkOrderId

IX_Status

IX_Priority

IX_FailureType

IX_FailureSeverity
```

---

# AUDIT

Audit every

- Creation
- Assignment
- Failure recording
- Downtime start/end
- Root Cause Analysis
- Material Consumption
- Labor Recording
- Completion
- Verification
- Closure
- Cancellation

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

- Create Corrective Maintenance
- Record Failure
- Start Downtime
- End Downtime
- Complete Root Cause Analysis
- Consume Material
- Record Labor
- Complete Maintenance
- Verify Maintenance
- Close Maintenance
- Prevent Closure before Verification

## Integration Tests

- Repository
- Commands
- Queries
- REST API
- Asset integration
- Work Order integration
- Inventory integration
- Notification integration
- Domain Events
- Audit

---

# ACCEPTANCE CRITERIA

- Every Corrective Maintenance references a valid Asset.
- Equipment failures are fully traceable.
- Downtime is automatically recorded.
- Critical failures require Root Cause Analysis.
- Maintenance Work Orders remain linked.
- Inventory consumption is performed only through Inventory Transactions.
- Closed Corrective Maintenance records become immutable.
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
- Maintenance Work Order integration completed
- Inventory integration completed
- Notification integration completed
- Authorization implemented
- Audit implemented
- Domain Events implemented
- Unit tests passing
- Integration tests passing
- Code review approved

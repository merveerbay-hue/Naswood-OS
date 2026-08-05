# ==============================================================================
# TASK-068 — IMPLEMENTATION
# IN-PROCESS INSPECTION
# Naswood Operating System (NOS)
# Module: Quality Management
# Sprint: Sprint 06 – Quality
# Status: Ready for Development
# ==============================================================================

# OBJECTIVE

Implement the In-Process Inspection aggregate responsible for performing quality
verification during manufacturing operations.

In-Process Inspection ensures defects are detected immediately while production
is still running, minimizing scrap, rework and production delays.

It records operational quality checkpoints only.

Inspection definitions remain owned by Inspection Plans.

---

# DOMAIN

Quality Management

Aggregate Root

```
InProcessInspection
```

---

# REFERENCES

Implementation must comply with:

- Constitution
- Quality_Architecture.md
- Quality_Workflow.md
- Quality_API.md
- TASK-066_Inspection_Plan.md
- TASK-067_Inspection_Execution.md
- TASK-056_Production_Order.md
- TASK-057_Work_Order.md
- TASK-062_Production_Quality.md

---

# DEPENDENCIES

Requires completed modules:

- Inspection Plan
- Inspection Execution
- Production Order
- Work Order
- Operation
- Product Revision
- Employee

---

# AGGREGATE

```
InProcessInspection
```

Children

```
InspectionCheckpoint

InspectionMeasurement

InspectionDefect

QualityHold

AuditEntry
```

---

# VALUE OBJECTS

```
InspectionCheckpointCode

InspectionStatus

InspectionDecision

InspectionTimestamp

InspectionSequence
```

---

# ENUMS

## InspectionStatus

```text
Planned
Waiting
InProgress
Completed
Failed
Passed
Cancelled
```

---

## InspectionDecision

```text
Accepted
Rejected
Conditional
ReworkRequired
ScrapRequired
OnHold
```

---

# ENTITY FIELDS

```text
Id

InspectionNumber

InspectionPlanRevisionId

ProductionOrderId

WorkOrderId

OperationRevisionId

ProductRevisionId

LotId

SerialId

InspectorId

CheckpointCode

Sequence

Status

Decision

StartedAt

CompletedAt

CreatedAt

UpdatedAt
```

---

# INSPECTION CHECKPOINT

```text
Id

InProcessInspectionId

CheckpointCode

CheckpointName

Mandatory

Sequence

InspectionFrequency
```

---

# INSPECTION MEASUREMENT

```text
Id

CheckpointId

CharacteristicId

MeasuredValue

TargetValue

MinimumValue

MaximumValue

Result

MeasuredAt
```

---

# DOMAIN INVARIANTS

Every In-Process Inspection references one Inspection Plan Revision.

Every Inspection belongs to one Production Operation.

Mandatory Checkpoints cannot be skipped.

Completed Inspections are immutable.

Failed inspections require at least one Defect record.

---

# DOMAIN METHODS

```text
Create()

Start()

ExecuteCheckpoint()

RecordMeasurement()

RecordDefect()

Pass()

Fail()

PlaceOnHold()

Resume()

Complete()

Cancel()
```

---

# DOMAIN EVENTS

```text
InProcessInspectionCreated

InspectionStarted

CheckpointCompleted

MeasurementRecorded

DefectRecorded

InspectionPassed

InspectionFailed

ProductionPlacedOnHold

InspectionCompleted

InspectionCancelled
```

---

# VALIDATIONS

Create

- Inspection Plan exists
- Operation exists
- Work Order active

Start

- Inspector assigned

Complete

- All mandatory checkpoints completed

Pass

- No critical defects

Fail

- Defect recorded

Hold

- Quality issue identified

---

# REPOSITORY

```text
IInProcessInspectionRepository
```

Methods

```csharp
Task<InProcessInspection?> GetByIdAsync(Guid id);

Task<IEnumerable<InProcessInspection>> GetByWorkOrderAsync(Guid workOrderId);

Task<IEnumerable<InProcessInspection>> GetOpenAsync();

Task AddAsync(InProcessInspection entity);

Task UpdateAsync(InProcessInspection entity);
```

---

# COMMANDS

```text
CreateInProcessInspectionCommand

StartInspectionCommand

ExecuteCheckpointCommand

RecordMeasurementCommand

RecordDefectCommand

PassInspectionCommand

FailInspectionCommand

PlaceInspectionOnHoldCommand

ResumeInspectionCommand

CompleteInspectionCommand

CancelInspectionCommand
```

---

# QUERIES

```text
GetInProcessInspectionByIdQuery

GetInProcessInspectionsQuery

GetWorkOrderInspectionsQuery

GetOpenInProcessInspectionsQuery
```

---

# API ENDPOINTS

```http
GET    /api/v1/quality/in-process-inspections

GET    /api/v1/quality/in-process-inspections/{id}

POST   /api/v1/quality/in-process-inspections

POST   /api/v1/quality/in-process-inspections/{id}/start

POST   /api/v1/quality/in-process-inspections/{id}/checkpoint

POST   /api/v1/quality/in-process-inspections/{id}/measurement

POST   /api/v1/quality/in-process-inspections/{id}/defect

POST   /api/v1/quality/in-process-inspections/{id}/pass

POST   /api/v1/quality/in-process-inspections/{id}/fail

POST   /api/v1/quality/in-process-inspections/{id}/hold

POST   /api/v1/quality/in-process-inspections/{id}/resume

POST   /api/v1/quality/in-process-inspections/{id}/complete

POST   /api/v1/quality/in-process-inspections/{id}/cancel
```

---

# AUTHORIZATION

```text
quality.inprocess.read

quality.inprocess.create

quality.inprocess.execute

quality.inprocess.measure

quality.inprocess.hold

quality.inprocess.complete

quality.inprocess.cancel
```

---

# DATABASE TABLES

## InProcessInspections

```text
Id

InspectionNumber

InspectionPlanRevisionId

ProductionOrderId

WorkOrderId

OperationRevisionId

ProductRevisionId

LotId

SerialId

InspectorId

CheckpointCode

Sequence

Status

Decision

StartedAt

CompletedAt

CreatedAt

UpdatedAt
```

---

## InspectionCheckpoints

```text
Id

InProcessInspectionId

CheckpointCode

CheckpointName

Mandatory

Sequence

InspectionFrequency
```

---

## InspectionMeasurements

```text
Id

CheckpointId

CharacteristicId

MeasuredValue

TargetValue

MinimumValue

MaximumValue

Result

MeasuredAt
```

---

# INDEXES

```text
IX_InspectionNumber (Unique)

IX_ProductionOrderId

IX_WorkOrderId

IX_OperationRevisionId

IX_ProductRevisionId

IX_Status

IX_InspectorId
```

---

# AUDIT

Audit every

- Inspection creation
- Checkpoint execution
- Measurement recording
- Defect recording
- Hold placement
- Resume
- Completion
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

- Create In-Process Inspection
- Execute mandatory checkpoints
- Record measurements
- Record defects
- Pass inspection
- Fail inspection
- Place production on hold
- Resume inspection
- Prevent completion with missing checkpoints

## Integration Tests

- Repository
- Commands
- Queries
- REST API
- Production integration
- Quality Hold integration
- Domain Events
- Audit

---

# ACCEPTANCE CRITERIA

- Every inspection references an Inspection Plan Revision.
- Mandatory checkpoints are enforced.
- Failed inspections automatically support Production Hold.
- Measurements are fully traceable.
- Completed inspections become immutable.
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
- Production integration completed
- Authorization implemented
- Audit implemented
- Domain Events implemented
- Unit tests passing
- Integration tests passing
- Code review approved

# ==============================================================================
# TASK-067 — IMPLEMENTATION
# INSPECTION EXECUTION
# Naswood Operating System (NOS)
# Module: Quality Management
# Sprint: Sprint 06 – Quality
# Status: Ready for Development
# ==============================================================================

# OBJECTIVE

Implement the Inspection Execution aggregate responsible for executing quality
inspections defined by Inspection Plans during Incoming, In-Process and Final
Quality Control.

Inspection Execution records actual measurements, inspection decisions and
quality outcomes.

Inspection definitions belong to Inspection Plans.

Inspection Execution stores inspection results only.

---

# DOMAIN

Quality Management

Aggregate Root

```
InspectionExecution
```

---

# REFERENCES

Implementation must comply with:

- Constitution
- Quality_Architecture.md
- Quality_Workflow.md
- Quality_API.md
- TASK-066_Inspection_Plan.md
- TASK-062_Production_Quality.md
- Production Order
- Work Order
- Product Revision

---

# DEPENDENCIES

Requires completed modules:

- Inspection Plan
- Product Revision
- Production Order
- Work Order
- Employee
- Quality Management

---

# AGGREGATE

```
InspectionExecution
```

Children

```
InspectionResult

Measurement

Defect

Attachment

AuditEntry
```

---

# VALUE OBJECTS

```
InspectionNumber

InspectionStatus

InspectionDecision

InspectionDate

SamplingResult
```

---

# ENUMS

## InspectionStatus

```text
Planned
Assigned
InProgress
Completed
Verified
Approved
Cancelled
```

---

## InspectionDecision

```text
Accepted
Rejected
Conditional
OnHold
ReworkRequired
```

---

# ENTITY FIELDS

```text
Id

InspectionNumber

InspectionPlanRevisionId

ProductionOrderId

WorkOrderId

ProductRevisionId

LotId

SerialId

InspectorId

InspectionDate

Status

Decision

StartedAt

CompletedAt

VerifiedAt

CreatedAt

UpdatedAt
```

---

# MEASUREMENT

```text
Id

InspectionExecutionId

CharacteristicId

MeasuredValue

TargetValue

MinimumValue

MaximumValue

Result

Comment

MeasuredAt
```

---

# DOMAIN INVARIANTS

Every Inspection Execution references exactly one Inspection Plan Revision.

Inspection Results cannot exist without an Inspection Execution.

Completed inspections become immutable.

Inspection Decisions are derived from recorded measurements.

Rejected inspections require at least one Defect record.

---

# DOMAIN METHODS

```text
Create()

AssignInspector()

Start()

RecordMeasurement()

RecordDefect()

Complete()

Verify()

Approve()

Cancel()
```

---

# DOMAIN EVENTS

```text
InspectionExecutionCreated

InspectionStarted

MeasurementRecorded

DefectRecorded

InspectionCompleted

InspectionVerified

InspectionApproved

InspectionRejected

InspectionCancelled
```

---

# VALIDATIONS

Create

- Inspection Plan exists
- Product Revision exists

Start

- Inspector assigned

Complete

- All mandatory characteristics measured
- Required samples completed

Approve

- Verification completed

Reject

- At least one Defect recorded

---

# REPOSITORY

```text
IInspectionExecutionRepository
```

Methods

```csharp
Task<InspectionExecution?> GetByIdAsync(Guid id);

Task<IEnumerable<InspectionExecution>> GetByProductionOrderAsync(Guid productionOrderId);

Task<IEnumerable<InspectionExecution>> GetOpenAsync();

Task AddAsync(InspectionExecution entity);

Task UpdateAsync(InspectionExecution entity);
```

---

# COMMANDS

```text
CreateInspectionExecutionCommand

AssignInspectorCommand

StartInspectionCommand

RecordMeasurementCommand

RecordDefectCommand

CompleteInspectionCommand

VerifyInspectionCommand

ApproveInspectionCommand

CancelInspectionCommand
```

---

# QUERIES

```text
GetInspectionExecutionByIdQuery

GetInspectionExecutionsQuery

GetProductionOrderInspectionsQuery

GetOpenInspectionsQuery
```

---

# API ENDPOINTS

```http
GET    /api/v1/quality/inspections

GET    /api/v1/quality/inspections/{id}

POST   /api/v1/quality/inspections

POST   /api/v1/quality/inspections/{id}/assign

POST   /api/v1/quality/inspections/{id}/start

POST   /api/v1/quality/inspections/{id}/measurement

POST   /api/v1/quality/inspections/{id}/defect

POST   /api/v1/quality/inspections/{id}/complete

POST   /api/v1/quality/inspections/{id}/verify

POST   /api/v1/quality/inspections/{id}/approve

POST   /api/v1/quality/inspections/{id}/cancel
```

---

# AUTHORIZATION

```text
quality.inspection.read

quality.inspection.create

quality.inspection.execute

quality.inspection.verify

quality.inspection.approve

quality.inspection.cancel
```

---

# DATABASE TABLES

## InspectionExecutions

```text
Id

InspectionNumber

InspectionPlanRevisionId

ProductionOrderId

WorkOrderId

ProductRevisionId

LotId

SerialId

InspectorId

InspectionDate

Status

Decision

StartedAt

CompletedAt

VerifiedAt

CreatedAt

UpdatedAt
```

---

## InspectionMeasurements

```text
Id

InspectionExecutionId

CharacteristicId

MeasuredValue

TargetValue

MinimumValue

MaximumValue

Result

Comment

MeasuredAt
```

---

## InspectionDefects

```text
Id

InspectionExecutionId

DefectCode

Severity

Description

Disposition

CreatedAt
```

---

# INDEXES

```text
IX_InspectionNumber (Unique)

IX_InspectionPlanRevisionId

IX_ProductionOrderId

IX_WorkOrderId

IX_ProductRevisionId

IX_Status

IX_InspectorId
```

---

# AUDIT

Audit every

- Assignment
- Start
- Measurement
- Defect creation
- Completion
- Verification
- Approval
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

- Create Inspection Execution
- Assign Inspector
- Record Measurements
- Record Defects
- Complete Inspection
- Verify Inspection
- Approve Inspection
- Reject Inspection
- Prevent modification after Completion

## Integration Tests

- Repository
- Commands
- Queries
- REST API
- Domain Events
- Audit

---

# ACCEPTANCE CRITERIA

- Inspection Execution always references an Inspection Plan Revision.
- Measurements are recorded against defined characteristics.
- Inspection Decisions are automatically derived from results.
- Rejected inspections require recorded defects.
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
- Authorization implemented
- Audit implemented
- Domain Events implemented
- Unit tests passing
- Integration tests passing
- Code review approved

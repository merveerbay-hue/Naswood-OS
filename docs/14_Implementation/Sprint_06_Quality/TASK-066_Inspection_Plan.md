# ==============================================================================
# TASK-066 — IMPLEMENTATION
# INSPECTION PLAN
# Naswood Operating System (NOS)
# Module: Quality Management
# Sprint: Sprint 06 – Quality
# Status: Ready for Development
# ==============================================================================

# OBJECTIVE

Implement the Inspection Plan aggregate responsible for defining reusable,
version-controlled quality inspection procedures for Incoming Quality,
In-Process Quality and Final Quality Control.

Inspection Plans define **what** should be inspected.

Production Execution records **how** inspections were performed.

Inspection Plans are engineering definitions and never store inspection results.

---

# DOMAIN

Quality Management

Aggregate Root

```
InspectionPlan
```

---

# REFERENCES

Implementation must comply with:

- Constitution
- Quality_Architecture.md
- Quality_Workflow.md
- Quality_API.md
- TASK-062_Production_Quality.md
- Product Revision
- Operation
- Production Order

---

# DEPENDENCIES

Requires completed modules:

- Product
- Product Revision
- Operation
- Unit of Measure
- Quality Management

---

# AGGREGATE

```
InspectionPlan
```

Children

```
InspectionCharacteristic

InspectionSampling

InspectionSpecification

InspectionAttachment

AuditEntry
```

---

# VALUE OBJECTS

```
InspectionPlanCode

InspectionPlanRevision

InspectionType

SamplingMethod

EffectivePeriod
```

---

# ENUMS

## InspectionPlanStatus

```text
Draft
UnderReview
Approved
Released
Active
Superseded
Archived
```

---

## InspectionType

```text
Incoming

InProcess

Final

Packaging

Shipment
```

---

## SamplingMethod

```text
100Percent

FixedQuantity

Percentage

ANSI_Z1_4

ISO2859

CustomerDefined
```

---

# ENTITY FIELDS

```text
Id

InspectionPlanCode

Name

Description

ProductRevisionId

OperationRevisionId

InspectionType

SamplingMethod

Status

Revision

EffectiveFrom

EffectiveTo

CreatedBy

ApprovedBy

ReleasedAt

CreatedAt

UpdatedAt
```

---

# INSPECTION CHARACTERISTIC

```text
Id

InspectionPlanId

CharacteristicCode

CharacteristicName

DataType

UnitOfMeasureId

TargetValue

MinimumValue

MaximumValue

Tolerance

Mandatory

Sequence
```

---

# DOMAIN INVARIANTS

Inspection Plans are immutable after Release.

Only one Active Revision may exist.

Inspection Characteristics belong to one Inspection Plan.

Released Inspection Plans cannot be edited.

Changes require a new Revision.

Historical inspections always reference the original Inspection Plan Revision.

---

# DOMAIN METHODS

```text
Create()

AddCharacteristic()

UpdateCharacteristic()

Approve()

Release()

Activate()

Supersede()

Archive()
```

---

# DOMAIN EVENTS

```text
InspectionPlanCreated

InspectionCharacteristicAdded

InspectionPlanApproved

InspectionPlanReleased

InspectionPlanActivated

InspectionPlanSuperseded

InspectionPlanArchived
```

---

# VALIDATIONS

Create

- Product Revision exists
- Inspection Type valid

Release

- At least one Characteristic exists
- Sampling Method defined
- All mandatory fields completed

Activate

- No other Active Revision exists

Supersede

- Replacement Revision exists

---

# REPOSITORY

```text
IInspectionPlanRepository
```

Methods

```csharp
Task<InspectionPlan?> GetByIdAsync(Guid id);

Task<InspectionPlan?> GetActiveAsync(Guid productRevisionId, InspectionType type);

Task<IEnumerable<InspectionPlan>> GetByProductAsync(Guid productRevisionId);

Task AddAsync(InspectionPlan entity);

Task UpdateAsync(InspectionPlan entity);
```

---

# COMMANDS

```text
CreateInspectionPlanCommand

AddInspectionCharacteristicCommand

ApproveInspectionPlanCommand

ReleaseInspectionPlanCommand

ActivateInspectionPlanCommand

SupersedeInspectionPlanCommand

ArchiveInspectionPlanCommand
```

---

# QUERIES

```text
GetInspectionPlanByIdQuery

GetInspectionPlansQuery

GetInspectionPlansByProductQuery

GetActiveInspectionPlanQuery
```

---

# API ENDPOINTS

```http
GET    /api/v1/quality/inspection-plans

GET    /api/v1/quality/inspection-plans/{id}

POST   /api/v1/quality/inspection-plans

PUT    /api/v1/quality/inspection-plans/{id}

POST   /api/v1/quality/inspection-plans/{id}/approve

POST   /api/v1/quality/inspection-plans/{id}/release

POST   /api/v1/quality/inspection-plans/{id}/activate

POST   /api/v1/quality/inspection-plans/{id}/archive
```

---

# AUTHORIZATION

```text
quality.inspectionplan.read

quality.inspectionplan.create

quality.inspectionplan.update

quality.inspectionplan.approve

quality.inspectionplan.release

quality.inspectionplan.activate

quality.inspectionplan.archive
```

---

# DATABASE TABLES

## InspectionPlans

```text
Id

InspectionPlanCode

Name

Description

ProductRevisionId

OperationRevisionId

InspectionType

SamplingMethod

Status

Revision

EffectiveFrom

EffectiveTo

CreatedBy

ApprovedBy

ReleasedAt

CreatedAt

UpdatedAt
```

---

## InspectionCharacteristics

```text
Id

InspectionPlanId

CharacteristicCode

CharacteristicName

DataType

UnitOfMeasureId

TargetValue

MinimumValue

MaximumValue

Tolerance

Mandatory

Sequence
```

---

# INDEXES

```text
IX_InspectionPlanCode (Unique)

IX_ProductRevisionId

IX_InspectionType

IX_Status

IX_Revision

IX_EffectiveFrom
```

---

# AUDIT

Audit every

- Creation
- Characteristic changes
- Approval
- Release
- Activation
- Supersede
- Archive

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

- Create Inspection Plan
- Add Characteristics
- Validate Sampling Method
- Release Inspection Plan
- Activate Revision
- Prevent editing after Release
- Archive Inspection Plan

## Integration Tests

- Repository
- Commands
- Queries
- REST API
- Domain Events
- Audit

---

# ACCEPTANCE CRITERIA

- Inspection Plans support revision management.
- Inspection Characteristics are reusable.
- Only one Active Revision exists.
- Released plans are immutable.
- Historical inspections remain linked to their original revision.
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

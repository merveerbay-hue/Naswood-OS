# ==============================================================================
# TASK-064 — IMPLEMENTATION
# REWORK
# Naswood Operating System (NOS)
# Module: Production Execution
# Sprint: Sprint 05 – Production Operations
# Status: Ready for Development
# ==============================================================================

# OBJECTIVE

Implement the Rework aggregate responsible for managing products that require
additional manufacturing operations after failing production or quality
inspection.

Rework allows non-conforming products to be corrected through controlled
manufacturing processes while preserving genealogy, production history and
quality traceability.

Rework is not a new Production Order.

It is an execution workflow attached to an existing Production Order.

---

# DOMAIN

Production Execution

Aggregate Root

```
ReworkOrder
```

---

# REFERENCES

Implementation must comply with:

- Constitution
- ADR-012 Product Capability Profile
- Production_Architecture.md
- Production_Workflow.md
- Production_API.md
- TASK-056_Production_Order.md
- TASK-057_Work_Order.md
- TASK-060_WIP.md
- TASK-062_Production_Quality.md
- TASK-063_Production_Scrap.md
- TASK-064_Genealogy.md

---

# DEPENDENCIES

Requires completed modules:

- Production Order
- Work Order
- WIP
- Production Quality
- Production Scrap
- Genealogy
- Inventory

---

# AGGREGATE

```
ReworkOrder
```

Children

```
ReworkOperation

ReworkMaterial

ReworkInspection

ReworkHistory

AuditEntry
```

---

# VALUE OBJECTS

```
ReworkNumber

ReworkStatus

ReworkReason

Disposition

ReworkQuantity
```

---

# ENUMS

## ReworkStatus

```text
Draft
Requested
Approved
Released
InProgress
QualityVerification
Completed
Closed
Cancelled
```

---

## ReworkReason

```text
QualityFailure
DimensionError
SurfaceDefect
MachineFailure
AssemblyError
CustomerReturn
EngineeringChange
Other
```

---

## ReworkDisposition

```text
Repair
Reprocess
Reinspect
Scrap
Release
```

---

# ENTITY FIELDS

```text
Id

ReworkNumber

ProductionOrderId

WorkOrderId

ProductRevisionId

LotId

SerialId

Status

Reason

Disposition

Quantity

RequestedBy

ApprovedBy

ReleasedAt

CompletedAt

ClosedAt

CreatedAt

UpdatedAt
```

---

# REWORK OPERATION

```text
Id

ReworkOrderId

OperationRevisionId

WorkCenterId

MachineId

Sequence

Status

StartedAt

CompletedAt
```

---

# DOMAIN INVARIANTS

Every Rework Order belongs to one Production Order.

Every Rework references one Product Revision.

Quantity must be greater than zero.

Rework Quantity cannot exceed rejected quantity.

Completed Rework is immutable.

Every Rework preserves genealogy.

Every Rework requires Quality Verification before release.

---

# DOMAIN METHODS

```text
Create()

Request()

Approve()

Release()

Start()

CompleteOperation()

SubmitForQuality()

ApproveQuality()

RejectQuality()

Close()

Cancel()

ConvertToScrap()
```

---

# DOMAIN EVENTS

```text
ReworkRequested

ReworkApproved

ReworkReleased

ReworkStarted

ReworkCompleted

ReworkQualityApproved

ReworkQualityRejected

ReworkClosed

ReworkCancelled

ReworkConvertedToScrap
```

---

# VALIDATIONS

Create

- Production Order exists
- Product Revision exists
- Quantity > 0

Approve

- Valid Rework Reason
- Valid Disposition

Release

- Quality approval completed

Complete

- All Rework Operations completed

Close

- Final Quality Approval completed

Convert To Scrap

- Quality rejected
- Scrap record successfully created

---

# REPOSITORY

```text
IReworkOrderRepository
```

Methods

```csharp
Task<ReworkOrder?> GetByIdAsync(Guid id);

Task<IEnumerable<ReworkOrder>> GetByProductionOrderAsync(Guid productionOrderId);

Task<IEnumerable<ReworkOrder>> GetOpenAsync();

Task AddAsync(ReworkOrder entity);

Task UpdateAsync(ReworkOrder entity);
```

---

# COMMANDS

```text
CreateReworkCommand

ApproveReworkCommand

ReleaseReworkCommand

StartReworkCommand

CompleteReworkCommand

ApproveReworkQualityCommand

RejectReworkQualityCommand

CloseReworkCommand

CancelReworkCommand

ConvertReworkToScrapCommand
```

---

# QUERIES

```text
GetReworkByIdQuery

GetReworksQuery

GetProductionOrderReworksQuery

GetOpenReworksQuery
```

---

# API ENDPOINTS

```http
GET    /api/v1/production/reworks

GET    /api/v1/production/reworks/{id}

POST   /api/v1/production/reworks

POST   /api/v1/production/reworks/{id}/approve

POST   /api/v1/production/reworks/{id}/release

POST   /api/v1/production/reworks/{id}/start

POST   /api/v1/production/reworks/{id}/complete

POST   /api/v1/production/reworks/{id}/quality-approve

POST   /api/v1/production/reworks/{id}/quality-reject

POST   /api/v1/production/reworks/{id}/close

POST   /api/v1/production/reworks/{id}/cancel

POST   /api/v1/production/reworks/{id}/convert-to-scrap
```

---

# AUTHORIZATION

```text
production.rework.read

production.rework.create

production.rework.approve

production.rework.execute

production.rework.quality

production.rework.close

production.rework.cancel
```

---

# DATABASE TABLE

## ReworkOrders

```text
Id

ReworkNumber

ProductionOrderId

WorkOrderId

ProductRevisionId

LotId

SerialId

Status

Reason

Disposition

Quantity

RequestedBy

ApprovedBy

ReleasedAt

CompletedAt

ClosedAt

CreatedAt

UpdatedAt
```

---

## ReworkOperations

```text
Id

ReworkOrderId

OperationRevisionId

WorkCenterId

MachineId

Sequence

Status

StartedAt

CompletedAt
```

---

# INDEXES

```text
IX_ReworkNumber (Unique)

IX_ProductionOrderId

IX_WorkOrderId

IX_ProductRevisionId

IX_LotId

IX_Status

IX_Reason
```

---

# AUDIT

Audit every

- Request
- Approval
- Release
- Operation completion
- Quality decision
- Closure
- Cancellation
- Conversion to Scrap

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

- Create Rework
- Approve Rework
- Release Rework
- Execute Rework Operations
- Pass Quality Verification
- Fail Quality Verification
- Convert Rework to Scrap
- Close Rework
- Prevent modification after Close

## Integration Tests

- Repository
- Commands
- Queries
- REST API
- Quality integration
- Scrap integration
- Genealogy integration
- Domain Events
- Audit

---

# ACCEPTANCE CRITERIA

- Rework is initiated only from rejected production or quality results.
- Rework preserves genealogy and production history.
- Every Rework passes through Quality Verification.
- Failed Rework can be converted to Scrap.
- Closed Rework becomes immutable.
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
- Quality integration completed
- Scrap integration completed
- Genealogy integration completed
- Authorization implemented
- Audit implemented
- Domain Events implemented
- Unit tests passing
- Integration tests passing
- Code review approved

# ==============================================================================
# TASK-060 — IMPLEMENTATION
# WORK IN PROCESS (WIP)
# Naswood Operating System (NOS)
# Module: Production Execution
# Sprint: Sprint 05 – Production Operations
# Status: Completed
# ==============================================================================

# OBJECTIVE

Implement the Work In Process (WIP) aggregate responsible for tracking products
that have entered manufacturing but have not yet become Finished Goods.

The WIP module provides complete visibility into partially completed products,
their current operation, quantity, location and manufacturing status.

WIP represents production state.

It does not represent inventory ownership.

Finished inventory is created only by Production Output.

---

# DOMAIN

Production Execution

Aggregate Root

```
WorkInProcess
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
- TASK-058_Material_Consumption.md
- TASK-059_Production_Confirmation.md

---

# DEPENDENCIES

Requires completed modules:

- Production Order
- Work Order
- Production Confirmation
- Product Revision
- Routing Revision
- Warehouse
- Production Output

---

# AGGREGATE

```
WorkInProcess
```

Children

```
WipOperation

WipLocation

WipQuantity

WipHistory

AuditEntry
```

---

# VALUE OBJECTS

```
WipNumber

WipStatus

CurrentOperation

CurrentLocation

RemainingQuantity
```

---

# ENUMS

## WipStatus

```text
Created
Waiting
Queued
InProcess
Paused
QualityHold
ReadyForNextOperation
ReadyForOutput
Completed
Cancelled
```

---

# ENTITY FIELDS

```text
Id

WipNumber

ProductionOrderId

WorkOrderId

ProductRevisionId

CurrentOperationRevisionId

CurrentWorkCenterId

CurrentMachineId

CurrentWarehouseId

Status

CurrentQuantity

AcceptedQuantity

RejectedQuantity

RemainingQuantity

CurrentStep

StartedAt

LastMovedAt

CompletedAt

CreatedAt

UpdatedAt
```

---

# DOMAIN INVARIANTS

Every WIP belongs to exactly one Production Order.

A WIP record represents one Product Revision.

Current Quantity must always be greater than or equal to zero.

Accepted Quantity + Rejected Quantity ≤ Current Quantity.

Completed WIP becomes read-only.

Finished Goods are created only through Production Output.

---

# DOMAIN METHODS

```text
Create()

MoveToOperation()

AssignMachine()

Pause()

Resume()

PlaceOnQualityHold()

ReleaseQualityHold()

ConfirmOperation()

MoveToNextOperation()

Complete()

Cancel()

UpdateQuantity()
```

---

# DOMAIN EVENTS

```text
WipCreated

WipMoved

WipPaused

WipResumed

WipQualityHoldPlaced

WipQualityHoldReleased

WipOperationCompleted

WipCompleted
```

---

# VALIDATIONS

Create

- Production Order exists
- Product Revision exists
- Planned Quantity > 0

Move To Operation

- Target Operation exists
- Previous Operation completed
- Assigned Work Center active

Confirm Operation

- Production Confirmation exists
- Operation completed

Complete

- Final Routing Operation completed
- Ready for Production Output

---

# REPOSITORY

```text
IWorkInProcessRepository
```

Methods

```csharp
Task<WorkInProcess?> GetByIdAsync(Guid id);

Task<WorkInProcess?> GetByNumberAsync(string number);

Task<IEnumerable<WorkInProcess>> GetActiveAsync();

Task<IEnumerable<WorkInProcess>> GetByProductionOrderAsync(Guid productionOrderId);

Task AddAsync(WorkInProcess entity);

Task UpdateAsync(WorkInProcess entity);
```

---

# COMMANDS

```text
CreateWipCommand

MoveWipCommand

PauseWipCommand

ResumeWipCommand

ConfirmOperationCommand

MoveToNextOperationCommand

CompleteWipCommand

CancelWipCommand
```

---

# QUERIES

```text
GetWipByIdQuery

GetActiveWipQuery

GetProductionOrderWipQuery

GetMachineWipQuery

GetWorkCenterWipQuery
```

---

# API ENDPOINTS

```http
GET    /api/v1/production/wip

GET    /api/v1/production/wip/{id}

POST   /api/v1/production/wip

POST   /api/v1/production/wip/{id}/move

POST   /api/v1/production/wip/{id}/pause

POST   /api/v1/production/wip/{id}/resume

POST   /api/v1/production/wip/{id}/confirm

POST   /api/v1/production/wip/{id}/complete

POST   /api/v1/production/wip/{id}/cancel
```

---

# AUTHORIZATION

```text
production.wip.read

production.wip.create

production.wip.update

production.wip.move

production.wip.complete

production.wip.cancel
```

---

# DATABASE TABLE

## WorkInProcess

```text
Id

WipNumber

ProductionOrderId

WorkOrderId

ProductRevisionId

CurrentOperationRevisionId

CurrentWorkCenterId

CurrentMachineId

CurrentWarehouseId

Status

CurrentQuantity

AcceptedQuantity

RejectedQuantity

RemainingQuantity

CurrentStep

StartedAt

LastMovedAt

CompletedAt

CreatedAt

UpdatedAt
```

---

# INDEXES

```text
IX_WipNumber (Unique)

IX_ProductionOrderId

IX_WorkOrderId

IX_Status

IX_CurrentWorkCenterId

IX_CurrentMachineId

IX_CurrentWarehouseId
```

---

# AUDIT

Audit every

- Status change
- Operation movement
- Quantity update
- Machine assignment
- Quality Hold
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

- Create WIP
- Move between operations
- Pause and Resume
- Place Quality Hold
- Complete WIP
- Prevent invalid operation sequence
- Prevent completion before final operation

## Integration Tests

- Repository
- Commands
- Queries
- REST API
- Domain Events
- Audit

---

# ACCEPTANCE CRITERIA

- WIP is automatically created after Production Order release.
- WIP progresses according to Routing sequence.
- Current operation is always known.
- Quantity changes are fully traceable.
- WIP completes only after the final operation.
- Production Output can only be created from Completed WIP.
- CQRS architecture is respected.
- Domain Events are published.
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
```

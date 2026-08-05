# ==============================================================================
# TASK-057 — IMPLEMENTATION
# WORK ORDER
# Naswood Operating System (NOS)
# Module: Production Execution
# Sprint: Sprint 05 – Production Operations
# Status: Completed
# ==============================================================================

# OBJECTIVE

Implement the Work Order aggregate responsible for executing individual
manufacturing operations within a Production Order.

A Production Order defines **what** must be produced.

A Work Order defines **which operation** is executed, **where**, **when**,
**by whom** and **on which machine**.

Every Production Order consists of one or more Work Orders generated from the
pinned Routing Revision.

---

# DOMAIN

Production Execution

Aggregate Root

```
WorkOrder
```

---

# REFERENCES

Implementation must comply with:

- Constitution
- ADR-012 Product Capability Profile
- Production_Architecture.md
- Production_Workflow.md
- Production_API.md
- TASK-047_Routing.md
- TASK-054_Operation.md
- TASK-056_Production_Order.md

---

# DEPENDENCIES

Requires completed modules:

- Production Order
- Routing
- Operation
- Machine
- Work Center
- Shift
- Labor Tracking

---

# AGGREGATE

```
WorkOrder
```

Children

```
MachineAssignment

OperatorAssignment

ExecutionLog

QualityCheckpoint

DowntimeEntry

ScrapEntry
```

---

# VALUE OBJECTS

```
WorkOrderNumber

WorkOrderStatus

OperationSequence

PlannedDuration

ActualDuration

ExecutionProgress
```

---

# ENUMS

## WorkOrderStatus

```text
Planned
Ready
Released
InProgress
Paused
Completed
Verified
Closed
Cancelled
OnHold
```

---

# ENTITY FIELDS

```text
Id

WorkOrderNumber

ProductionOrderId

OperationRevisionId

RoutingRevisionId

WorkCenterId

MachineId

ShiftId

Sequence

Status

PlannedStart

PlannedFinish

ActualStart

ActualFinish

PlannedQuantity

CompletedQuantity

ScrapQuantity

RemainingQuantity

Progress

ReleasedAt

CompletedAt

ClosedAt

CreatedAt

UpdatedAt
```

---

# DOMAIN INVARIANTS

A Work Order belongs to exactly one Production Order.

A Work Order represents exactly one Routing Operation.

Released Work Orders are immutable except execution data.

Completed Quantity cannot exceed Planned Quantity.

Completed Work Orders cannot return to Ready state.

Closed Work Orders are immutable.

---

# DOMAIN METHODS

```text
Create()

Release()

Start()

Pause()

Resume()

Complete()

Verify()

Close()

Cancel()

PutOnHold()

ResumeFromHold()

AssignMachine()

AssignOperator()

RegisterOutput()

RegisterScrap()

RegisterDowntime()

UpdateProgress()
```

---

# DOMAIN EVENTS

```text
WorkOrderCreated

WorkOrderReleased

WorkOrderStarted

WorkOrderPaused

WorkOrderResumed

MachineAssigned

OperatorAssigned

WorkOrderCompleted

WorkOrderVerified

WorkOrderClosed

WorkOrderCancelled
```

---

# VALIDATIONS

Create

- Production Order exists
- Operation exists
- Work Center exists
- Planned Quantity > 0

Release

- Production Order Released
- Routing Revision Active
- Machine Capability valid

Start

- Machine Available
- Shift Active
- Previous Operations completed (if sequential)

Complete

- Required Quality Checkpoints completed
- Output recorded
- Remaining Quantity = 0

Verify

- Quality accepted
- No unresolved holds

---

# REPOSITORY

```text
IWorkOrderRepository
```

Methods

```csharp
Task<WorkOrder?> GetByIdAsync(Guid id);

Task<IEnumerable<WorkOrder>> GetByProductionOrderAsync(Guid productionOrderId);

Task<IEnumerable<WorkOrder>> GetOpenAsync();

Task AddAsync(WorkOrder entity);

Task UpdateAsync(WorkOrder entity);
```

---

# COMMANDS

```text
CreateWorkOrderCommand

ReleaseWorkOrderCommand

StartWorkOrderCommand

PauseWorkOrderCommand

ResumeWorkOrderCommand

AssignMachineCommand

AssignOperatorCommand

CompleteWorkOrderCommand

VerifyWorkOrderCommand

CloseWorkOrderCommand

CancelWorkOrderCommand
```

---

# QUERIES

```text
GetWorkOrderByIdQuery

GetWorkOrdersQuery

GetProductionOrderWorkOrdersQuery

GetMachineWorkOrdersQuery

GetOpenWorkOrdersQuery
```

---

# API ENDPOINTS

```http
GET    /api/v1/production/work-orders

GET    /api/v1/production/work-orders/{id}

POST   /api/v1/production/work-orders

POST   /api/v1/production/work-orders/{id}/release

POST   /api/v1/production/work-orders/{id}/start

POST   /api/v1/production/work-orders/{id}/pause

POST   /api/v1/production/work-orders/{id}/resume

POST   /api/v1/production/work-orders/{id}/assign-machine

POST   /api/v1/production/work-orders/{id}/assign-operator

POST   /api/v1/production/work-orders/{id}/complete

POST   /api/v1/production/work-orders/{id}/verify

POST   /api/v1/production/work-orders/{id}/close

POST   /api/v1/production/work-orders/{id}/cancel
```

---

# AUTHORIZATION

```text
production.workorder.read

production.workorder.create

production.workorder.release

production.workorder.execute

production.workorder.assign.machine

production.workorder.assign.operator

production.workorder.complete

production.workorder.verify

production.workorder.close

production.workorder.cancel
```

---

# DATABASE TABLE

```text
WorkOrders
```

Primary Columns

```text
Id

WorkOrderNumber

ProductionOrderId

OperationRevisionId

RoutingRevisionId

WorkCenterId

MachineId

ShiftId

Sequence

Status

PlannedQuantity

CompletedQuantity

ScrapQuantity

RemainingQuantity

Progress

PlannedStart

PlannedFinish

ActualStart

ActualFinish

ReleasedAt

CompletedAt

ClosedAt

CreatedAt

UpdatedAt
```

Indexes

```text
IX_WorkOrderNumber (Unique)

IX_ProductionOrderId

IX_Status

IX_WorkCenterId

IX_MachineId

IX_ShiftId

IX_Sequence
```

---

# AUDIT

Audit every

- Status transition
- Machine assignment
- Operator assignment
- Progress update
- Quantity update
- Completion
- Verification
- Closure

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

- Create Work Order
- Release Work Order
- Assign Machine
- Assign Operator
- Execute lifecycle transitions
- Prevent invalid state transitions
- Prevent completion without required validations

## Integration Tests

- Repository
- Commands
- Queries
- REST API
- Domain Events
- Authorization
- Audit

---

# ACCEPTANCE CRITERIA

- Work Orders are automatically generated from the released Routing Revision.
- Every Work Order references exactly one Operation Revision.
- Machine and Operator assignments are traceable.
- Lifecycle follows workflow rules.
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
```

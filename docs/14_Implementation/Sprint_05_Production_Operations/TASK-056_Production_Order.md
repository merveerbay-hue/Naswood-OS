# ==============================================================================
# TASK-056 — IMPLEMENTATION
# PRODUCTION ORDER
# Naswood Operating System (NOS)
# Module: Production Execution
# Sprint: Sprint 05 – Production Operations
# Status: Ready for Development
# ==============================================================================

# OBJECTIVE

Implement the Production Order aggregate responsible for managing the execution
lifecycle of manufacturing orders.

This task converts the Production Master definitions (Product Revision, BOM
Revision, Routing Revision and Capability Profile) into executable Production
Orders while preserving immutable engineering references.

---

# DOMAIN

Production Execution

Aggregate Root

```
ProductionOrder
```

---

# REFERENCES

Implementation must comply with:

- Constitution
- ADR-012 Product Capability Profile
- Production_Architecture.md
- Production_Workflow.md
- Production_API.md
- TASK-056_Production_Order.md (Design)

---

# DEPENDENCIES

Requires completed modules:

- Product
- Product Revision
- Capability Profile
- BOM
- Routing
- Warehouse
- Planning

---

# AGGREGATE

```
ProductionOrder
```

Children

```
ProductionOperation

MaterialRequirement

LaborEntry

ProductionOutput

ProductionScrap

ProductionDowntime
```

---

# VALUE OBJECTS

```
ProductionOrderNumber

ProductionStatus

ProductionPriority

PlannedQuantity

CompletedQuantity

RemainingQuantity

DueDate
```

---

# ENUMS

## ProductionOrderStatus

```text
Draft
Planned
Released
Ready
InProgress
Paused
Completed
Closed
Cancelled
OnHold
Archived
```

## ProductionPriority

```text
Low
Normal
High
Urgent
```

---

# ENTITY FIELDS

```text
Id

OrderNumber

ProductRevisionId

CapabilityProfileId

BomRevisionId

RoutingRevisionId

WarehouseId

PlannedQuantity

CompletedQuantity

ScrapQuantity

RemainingQuantity

Priority

Status

PlannedStart

PlannedFinish

ActualStart

ActualFinish

PlannerId

ReleasedAt

CompletedAt

ClosedAt

CreatedAt

UpdatedAt
```

---

# DOMAIN INVARIANTS

Engineering references are immutable after Release.

A Production Order always references:

- Product Revision
- Capability Profile
- BOM Revision
- Routing Revision

Released orders cannot change engineering references.

Completed quantity cannot exceed planned quantity.

Negative quantities are prohibited.

Closed orders are immutable.

---

# DOMAIN METHODS

```text
Create()

Release()

Start()

Pause()

Resume()

Complete()

Close()

Cancel()

PutOnHold()

ResumeFromHold()

UpdateProgress()

RegisterOutput()

RegisterScrap()

RegisterDowntime()
```

---

# DOMAIN EVENTS

```text
ProductionOrderCreated

ProductionOrderReleased

ProductionOrderStarted

ProductionOrderPaused

ProductionOrderResumed

ProductionOrderCompleted

ProductionOrderClosed

ProductionOrderCancelled

ProductionOrderPutOnHold
```

---

# VALIDATIONS

Create

- Product Revision exists
- Capability Profile exists
- BOM Revision exists
- Routing Revision exists
- Warehouse exists
- Quantity > 0

Release

- Status = Planned
- BOM Released
- Routing Released
- Capability Profile Active

Complete

- All required operations completed
- Output posted
- Open quality holds resolved

Close

- Status = Completed

---

# REPOSITORY

```text
IProductionOrderRepository
```

Methods

```csharp
Task<ProductionOrder?> GetByIdAsync(Guid id);

Task<ProductionOrder?> GetByNumberAsync(string number);

Task AddAsync(ProductionOrder order);

Task UpdateAsync(ProductionOrder order);

Task<bool> ExistsAsync(string orderNumber);
```

---

# COMMANDS

```text
CreateProductionOrderCommand

ReleaseProductionOrderCommand

StartProductionOrderCommand

PauseProductionOrderCommand

ResumeProductionOrderCommand

CompleteProductionOrderCommand

CloseProductionOrderCommand

CancelProductionOrderCommand
```

---

# QUERIES

```text
GetProductionOrderByIdQuery

GetProductionOrdersQuery

GetOpenProductionOrdersQuery

GetReleasedProductionOrdersQuery
```

---

# API ENDPOINTS

```http
GET    /api/v1/production/orders

GET    /api/v1/production/orders/{id}

POST   /api/v1/production/orders

POST   /api/v1/production/orders/{id}/release

POST   /api/v1/production/orders/{id}/start

POST   /api/v1/production/orders/{id}/pause

POST   /api/v1/production/orders/{id}/resume

POST   /api/v1/production/orders/{id}/complete

POST   /api/v1/production/orders/{id}/close

POST   /api/v1/production/orders/{id}/cancel
```

---

# AUTHORIZATION

```text
production.order.read

production.order.create

production.order.release

production.order.execute

production.order.complete

production.order.close

production.order.cancel
```

---

# DATABASE TABLE

```text
ProductionOrders
```

Primary Columns

```text
Id

OrderNumber

ProductRevisionId

CapabilityProfileId

BomRevisionId

RoutingRevisionId

WarehouseId

Status

Priority

PlannedQuantity

CompletedQuantity

ScrapQuantity

RemainingQuantity

PlannedStart

PlannedFinish

ActualStart

ActualFinish

PlannerId

ReleasedAt

CompletedAt

ClosedAt

CreatedAt

UpdatedAt
```

Indexes

```text
IX_OrderNumber (Unique)

IX_Status

IX_ProductRevisionId

IX_WarehouseId

IX_PlannedStart

IX_PlannerId
```

---

# AUDIT

Audit every:

- Status transition
- Quantity update
- Release
- Completion
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

- Create Production Order
- Prevent invalid quantities
- Prevent engineering reference changes after release
- Complete lifecycle transitions
- Prevent invalid transitions

## Integration Tests

- Repository
- Commands
- Queries
- API endpoints
- Domain events

---

# ACCEPTANCE CRITERIA

- Production Orders can be created successfully.
- Engineering references are pinned and immutable after release.
- Lifecycle transitions follow workflow rules.
- CQRS pattern is respected.
- Domain Events are published correctly.
- Repository tests pass.
- API endpoints pass integration tests.
- Audit logging is complete.
- All unit and integration tests succeed.

---

# DEFINITION OF DONE

- Domain implemented
- Application layer implemented
- Infrastructure implemented
- REST API completed
- Validation rules completed
- Authorization implemented
- Audit implemented
- Events implemented
- Unit tests passing
- Integration tests passing
- Code review approved
```

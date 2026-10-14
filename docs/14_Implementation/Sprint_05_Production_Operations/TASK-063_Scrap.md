# ==============================================================================
# TASK-063 — IMPLEMENTATION
# PRODUCTION SCRAP
# Naswood Operating System (NOS)
# Module: Production Execution
# Sprint: Sprint 05 – Production Operations
# Status: Completed
# ==============================================================================

# OBJECTIVE

Implement the Production Scrap aggregate responsible for recording all
manufacturing losses generated during production.

Production Scrap records rejected materials, semi-finished products and
finished products while preserving production history, genealogy, inventory
integrity and manufacturing analytics.

Scrap recording never directly changes Inventory.

Inventory adjustments are performed only through Inventory Transactions.

---

# DOMAIN

Production Execution

Aggregate Root

```
ProductionScrap
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
- TASK-059_Production_Confirmation.md
- TASK-060_WIP.md
- Inventory Architecture
- Quality Architecture

---

# DEPENDENCIES

Requires completed modules:

- Production Order
- Work Order
- Product Revision
- WIP
- Quality
- Inventory
- Genealogy

---

# AGGREGATE

```
ProductionScrap
```

Children

```
ScrapLine

ScrapReason

ScrapDisposition

InventoryReference

GenealogyReference

AuditEntry
```

---

# VALUE OBJECTS

```
ScrapNumber

ScrapStatus

ScrapType

ScrapQuantity

ScrapReasonCode
```

---

# ENUMS

## ScrapStatus

```text
Draft
Recorded
Approved
Posted
Cancelled
```

---

## ScrapType

```text
Material

Process

Setup

Machine

Quality

Rework

Packaging

Other
```

---

## ScrapDisposition

```text
Destroy

Recycle

Rework

ReturnToStock

ReturnToSupplier
```

---

# ENTITY FIELDS

```text
Id

ScrapNumber

ProductionOrderId

WorkOrderId

ProductRevisionId

OperationRevisionId

LotId

SerialId

Status

ScrapType

ScrapReasonCode

Disposition

Quantity

UnitOfMeasureId

RecordedBy

ApprovedBy

PostedAt

CreatedAt

UpdatedAt
```

---

# SCRAP LINE

```text
Id

ProductionScrapId

ProductRevisionId

LotId

SerialId

Quantity

ReasonCode

Disposition

Comments
```

---

# DOMAIN INVARIANTS

Every Scrap belongs to one Production Order.

Every Scrap belongs to one Work Order.

Quantity must be greater than zero.

Scrap Quantity cannot exceed produced quantity.

Every Scrap requires one standardized Reason Code.

Posted Scrap is immutable.

Inventory is updated only through Inventory Transactions.

Genealogy is preserved for every Scrap record.

---

# DOMAIN METHODS

```text
Create()

AddScrapLine()

Approve()

Post()

Cancel()

AssignReason()

AssignDisposition()

ValidateQuantity()

GenerateInventoryAdjustment()

LinkGenealogy()
```

---

# DOMAIN EVENTS

```text
ScrapRecorded

ScrapApproved

ScrapPosted

ScrapCancelled

InventoryAdjustmentRequested

ScrapGenealogyLinked
```

---

# VALIDATIONS

Create

- Production Order exists
- Work Order exists
- Product Revision exists
- Quantity > 0

Approve

- Reason Code selected
- Disposition selected

Post

- Approval completed
- Inventory Adjustment created
- Genealogy linked

Cancel

- Status ≠ Posted

---

# REPOSITORY

```text
IProductionScrapRepository
```

Methods

```csharp
Task<ProductionScrap?> GetByIdAsync(Guid id);

Task<IEnumerable<ProductionScrap>> GetByProductionOrderAsync(Guid productionOrderId);

Task<IEnumerable<ProductionScrap>> GetByWorkOrderAsync(Guid workOrderId);

Task AddAsync(ProductionScrap entity);

Task UpdateAsync(ProductionScrap entity);
```

---

# COMMANDS

```text
CreateProductionScrapCommand

AddScrapLineCommand

ApproveProductionScrapCommand

PostProductionScrapCommand

CancelProductionScrapCommand

AssignScrapReasonCommand

AssignDispositionCommand
```

---

# QUERIES

```text
GetProductionScrapByIdQuery

GetProductionScrapsQuery

GetProductionOrderScrapsQuery

GetWorkOrderScrapsQuery

GetScrapReasonsQuery
```

---

# API ENDPOINTS

```http
GET    /api/v1/production/scrap

GET    /api/v1/production/scrap/{id}

POST   /api/v1/production/scrap

POST   /api/v1/production/scrap/{id}/approve

POST   /api/v1/production/scrap/{id}/post

POST   /api/v1/production/scrap/{id}/cancel

GET    /api/v1/production/scrap/reasons
```

---

# AUTHORIZATION

```text
production.scrap.read

production.scrap.create

production.scrap.approve

production.scrap.post

production.scrap.cancel
```

---

# DATABASE TABLE

## ProductionScraps

```text
Id

ScrapNumber

ProductionOrderId

WorkOrderId

ProductRevisionId

OperationRevisionId

LotId

SerialId

Status

ScrapType

ScrapReasonCode

Disposition

Quantity

UnitOfMeasureId

RecordedBy

ApprovedBy

PostedAt

CreatedAt

UpdatedAt
```

---

## ProductionScrapLines

```text
Id

ProductionScrapId

ProductRevisionId

LotId

SerialId

Quantity

ReasonCode

Disposition

Comments
```

---

# INDEXES

```text
IX_ScrapNumber (Unique)

IX_ProductionOrderId

IX_WorkOrderId

IX_ProductRevisionId

IX_LotId

IX_Status

IX_ScrapReasonCode
```

---

# AUDIT

Audit every

- Scrap creation
- Quantity update
- Reason assignment
- Disposition assignment
- Approval
- Posting
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

- Create Scrap
- Add Scrap Line
- Assign Reason
- Assign Disposition
- Prevent invalid quantity
- Approve Scrap
- Post Scrap
- Cancel Scrap
- Prevent modification after Posting

## Integration Tests

- Repository
- Commands
- Queries
- REST API
- Inventory integration
- Genealogy integration
- Domain Events
- Audit

---

# ACCEPTANCE CRITERIA

- Scrap is linked to Production Order and Work Order.
- Scrap quantities are validated against production quantities.
- Standardized Reason Codes are enforced.
- Inventory adjustments occur only through Inventory Transactions.
- Genealogy remains complete.
- Posted Scrap becomes immutable.
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
- Inventory integration completed
- Genealogy integration completed
- Authorization implemented
- Audit implemented
- Domain Events implemented
- Unit tests passing
- Integration tests passing
- Code review approved

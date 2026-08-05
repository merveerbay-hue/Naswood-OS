# ==============================================================================
# TASK-058 — IMPLEMENTATION
# MATERIAL CONSUMPTION
# Naswood Operating System (NOS)
# Module: Production Execution
# Sprint: Sprint 05 – Production Operations
# Status: Ready for Development
# ==============================================================================

# OBJECTIVE

Implement the Material Consumption aggregate responsible for recording the
actual consumption of inventory materials during manufacturing.

Material Consumption is the execution implementation of Material Issue.

It consumes inventory through Inventory Transactions while maintaining complete
lot genealogy and production traceability.

No inventory balance may ever be modified directly by Production.

---

# DOMAIN

Production Execution

Aggregate Root

```
MaterialConsumption
```

---

# REFERENCES

Implementation must comply with:

- Constitution
- ADR-012 Product Capability Profile
- Production_Architecture.md
- Production_Workflow.md
- Production_API.md
- TASK-057_Material_Issue.md
- TASK-056_Production_Order.md
- Inventory Architecture

---

# DEPENDENCIES

Requires completed modules:

- Production Order
- Work Order
- Inventory
- Warehouse
- Material
- Lot
- Product Revision

---

# AGGREGATE

```
MaterialConsumption
```

Children

```
MaterialConsumptionLine

LotAllocation

SerialAllocation

InventoryTransactionReference

AuditEntry
```

---

# VALUE OBJECTS

```
ConsumptionNumber

ConsumptionStatus

ConsumptionQuantity

ConsumptionMode

IssueDate
```

---

# ENUMS

## ConsumptionStatus

```text
Draft
Reserved
Posted
PartiallyPosted
Completed
Cancelled
```

## ConsumptionMode

```text
Manual

Barcode

QRCode

Backflush

Automatic

Return
```

---

# ENTITY FIELDS

```text
Id

ConsumptionNumber

ProductionOrderId

WorkOrderId

OperationRevisionId

WarehouseId

Status

ConsumptionMode

PostingDate

PostedBy

CreatedAt

UpdatedAt
```

---

# MATERIAL CONSUMPTION LINE

```text
Id

MaterialConsumptionId

ProductRevisionId

InventoryMaterialId

LotId

SerialId

RequiredQuantity

ConsumedQuantity

ReturnedQuantity

RemainingQuantity

UnitOfMeasureId
```

---

# DOMAIN INVARIANTS

Material Consumption always belongs to one Production Order.

Each Consumption Line references one Product Revision.

Inventory Material must exist.

Lot must exist if Lot Tracking is enabled.

Serial must exist if Serial Tracking is enabled.

Consumed Quantity > 0.

Consumed Quantity cannot exceed available inventory.

Posted Consumption is immutable.

Inventory balances are modified only by Inventory Transactions.

---

# DOMAIN METHODS

```text
Create()

Reserve()

AddLine()

AllocateLot()

AllocateSerial()

Post()

Cancel()

ReturnMaterial()

ValidateInventory()

GenerateInventoryTransaction()
```

---

# DOMAIN EVENTS

```text
MaterialConsumptionCreated

MaterialReserved

MaterialConsumed

MaterialReturned

InventoryTransactionRequested

ConsumptionCancelled
```

---

# VALIDATIONS

Create

- Production Order exists
- Work Order exists
- Warehouse exists

Reserve

- Material available
- Lot available
- Quantity > 0

Post

- Inventory available
- Valid Lot
- Valid Serial
- Inventory Transaction created successfully

Return

- Original consumption exists
- Returned quantity ≤ Consumed quantity

---

# REPOSITORY

```text
IMaterialConsumptionRepository
```

Methods

```csharp
Task<MaterialConsumption?> GetByIdAsync(Guid id);

Task<IEnumerable<MaterialConsumption>> GetByProductionOrderAsync(Guid productionOrderId);

Task AddAsync(MaterialConsumption entity);

Task UpdateAsync(MaterialConsumption entity);
```

---

# COMMANDS

```text
CreateMaterialConsumptionCommand

ReserveMaterialCommand

AllocateLotCommand

AllocateSerialCommand

PostMaterialConsumptionCommand

ReturnMaterialCommand

CancelMaterialConsumptionCommand
```

---

# QUERIES

```text
GetMaterialConsumptionByIdQuery

GetMaterialConsumptionsQuery

GetProductionOrderConsumptionsQuery

GetWarehouseConsumptionsQuery
```

---

# API ENDPOINTS

```http
GET    /api/v1/production/material-consumptions

GET    /api/v1/production/material-consumptions/{id}

POST   /api/v1/production/material-consumptions

POST   /api/v1/production/material-consumptions/{id}/reserve

POST   /api/v1/production/material-consumptions/{id}/post

POST   /api/v1/production/material-consumptions/{id}/return

POST   /api/v1/production/material-consumptions/{id}/cancel
```

---

# AUTHORIZATION

```text
production.material.read

production.material.consume

production.material.reserve

production.material.return

production.material.cancel
```

---

# DATABASE TABLES

## MaterialConsumptions

```text
Id

ConsumptionNumber

ProductionOrderId

WorkOrderId

WarehouseId

Status

ConsumptionMode

PostingDate

PostedBy

CreatedAt

UpdatedAt
```

---

## MaterialConsumptionLines

```text
Id

MaterialConsumptionId

ProductRevisionId

InventoryMaterialId

LotId

SerialId

RequiredQuantity

ConsumedQuantity

ReturnedQuantity

RemainingQuantity

UnitOfMeasureId
```

---

# INDEXES

```text
IX_ConsumptionNumber (Unique)

IX_ProductionOrderId

IX_WorkOrderId

IX_WarehouseId

IX_Status

IX_PostingDate
```

---

# AUDIT

Audit every

- Reservation
- Lot allocation
- Serial allocation
- Posting
- Return
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

- Create Consumption
- Reserve Material
- Allocate Lot
- Allocate Serial
- Prevent over-consumption
- Post Consumption
- Return Material
- Prevent invalid returns

## Integration Tests

- Repository
- Commands
- Queries
- Inventory Transaction integration
- API endpoints
- Domain Events
- Audit

---

# ACCEPTANCE CRITERIA

- Material Consumption references Production Order and Work Order.
- Inventory is consumed only through Inventory Transactions.
- Lot and Serial traceability are preserved.
- Returns create reverse Inventory Transactions.
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
- Authorization implemented
- Audit implemented
- Domain Events implemented
- Unit tests passing
- Integration tests passing
- Code review approved
```

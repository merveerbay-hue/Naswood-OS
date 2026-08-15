# ==============================================================================
# TASK-082 — IMPLEMENTATION
# SPARE PARTS MANAGEMENT
# Naswood Operating System (NOS)
# Module: Maintenance Management
# Sprint: Sprint 07 – Maintenance
# Status: Ready for Development
# ==============================================================================

# OBJECTIVE

Implement the Spare Parts Management aggregate responsible for managing all
maintenance spare parts, consumables and service materials used during
maintenance activities.

Spare Parts are inventory-controlled maintenance materials.

Inventory ownership belongs entirely to the Inventory module.

The Maintenance module only reserves and consumes spare parts through
Inventory Transactions.

---

# DOMAIN

Maintenance Management

Aggregate Root

```
SparePart
```

---

# REFERENCES

Implementation must comply with:

- Constitution
- Maintenance_Architecture.md
- Maintenance_Workflow.md
- Maintenance_API.md
- Inventory_Architecture.md
- Product Capability Profile
- TASK-076_Asset.md
- TASK-078_Maintenance_Order.md

---

# DEPENDENCIES

Requires completed modules:

- Product
- Product Revision
- Capability Profile
- Inventory
- Warehouse
- Supplier
- Unit Of Measure

---

# AGGREGATE

```
SparePart
```

Children

```
SparePartAlternative

CompatibleAsset

PreferredSupplier

MinMaxPolicy

AuditEntry
```

---

# VALUE OBJECTS

```
SparePartCode

Criticality

ProcurementType

StockPolicy

ReorderPolicy
```

---

# ENUMS

## SparePartStatus

```text
Draft

Active

Obsolete

Discontinued

Archived
```

---

## SparePartCriticality

```text
Low

Medium

High

Critical
```

---

## ProcurementType

```text
Purchased

Manufactured

Subcontracted
```

---

## StockPolicy

```text
Stocked

NonStocked

VendorManaged

Consignment
```

---

# ENTITY FIELDS

```text
Id

ProductRevisionId

CapabilityProfileId

SparePartCode

Description

Status

Criticality

ProcurementType

StockPolicy

PreferredWarehouseId

DefaultSupplierId

MinimumStock

MaximumStock

ReorderPoint

EconomicOrderQuantity

LeadTimeDays

CreatedAt

UpdatedAt
```

---

# COMPATIBLE ASSET

```text
Id

SparePartId

AssetId

CompatibilityLevel

Notes
```

---

# ALTERNATIVE PART

```text
Id

SparePartId

AlternativeProductRevisionId

Priority

Approved
```

---

# DOMAIN INVARIANTS

Every Spare Part references one Product Revision.

Product Capability Profile must allow Purchasing and Inventory.

Inventory quantities are never stored in this aggregate.

One Spare Part Code is unique.

Archived Spare Parts cannot be assigned to Maintenance Work Orders.

---

# DOMAIN METHODS

```text
Create()

Activate()

AssignSupplier()

AssignWarehouse()

AddCompatibleAsset()

AddAlternative()

UpdateStockPolicy()

Archive()
```

---

# DOMAIN EVENTS

```text
SparePartCreated

SparePartActivated

CompatibleAssetAdded

AlternativeAdded

StockPolicyUpdated

SparePartArchived
```

---

# VALIDATIONS

Create

- Product Revision exists
- Capability Profile supports Purchasing
- Capability Profile supports Inventory

Activate

- Default Supplier exists
- Warehouse assigned

Assign Compatible Asset

- Asset exists

Archive

- No open reservations

---

# REPOSITORY

```text
ISparePartRepository
```

Methods

```csharp
Task<SparePart?> GetByIdAsync(Guid id);

Task<SparePart?> GetByCodeAsync(string sparePartCode);

Task<IEnumerable<SparePart>> GetActiveAsync();

Task<IEnumerable<SparePart>> GetByAssetAsync(Guid assetId);

Task AddAsync(SparePart entity);

Task UpdateAsync(SparePart entity);
```

---

# COMMANDS

```text
CreateSparePartCommand

ActivateSparePartCommand

AssignSupplierCommand

AssignWarehouseCommand

AddCompatibleAssetCommand

AddAlternativePartCommand

UpdateStockPolicyCommand

ArchiveSparePartCommand
```

---

# QUERIES

```text
GetSparePartByIdQuery

GetSparePartsQuery

GetAssetSparePartsQuery

GetCriticalSparePartsQuery

GetLowStockSparePartsQuery
```

---

# API ENDPOINTS

```http
GET    /api/v1/maintenance/spare-parts

GET    /api/v1/maintenance/spare-parts/{id}

POST   /api/v1/maintenance/spare-parts

PUT    /api/v1/maintenance/spare-parts/{id}

POST   /api/v1/maintenance/spare-parts/{id}/activate

POST   /api/v1/maintenance/spare-parts/{id}/compatible-asset

POST   /api/v1/maintenance/spare-parts/{id}/alternative

POST   /api/v1/maintenance/spare-parts/{id}/stock-policy

POST   /api/v1/maintenance/spare-parts/{id}/archive
```

---

# AUTHORIZATION

```text
maintenance.sparepart.read

maintenance.sparepart.create

maintenance.sparepart.update

maintenance.sparepart.activate

maintenance.sparepart.archive
```

---

# DATABASE TABLES

## SpareParts

```text
Id

ProductRevisionId

CapabilityProfileId

SparePartCode

Description

Status

Criticality

ProcurementType

StockPolicy

PreferredWarehouseId

DefaultSupplierId

MinimumStock

MaximumStock

ReorderPoint

EconomicOrderQuantity

LeadTimeDays

CreatedAt

UpdatedAt
```

---

## CompatibleAssets

```text
Id

SparePartId

AssetId

CompatibilityLevel

Notes
```

---

## AlternativeSpareParts

```text
Id

SparePartId

AlternativeProductRevisionId

Priority

Approved
```

---

# INDEXES

```text
IX_SparePartCode (Unique)

IX_ProductRevisionId

IX_Status

IX_Criticality

IX_DefaultSupplierId

IX_PreferredWarehouseId
```

---

# INVENTORY INTEGRATION

Maintenance never updates stock quantities directly.

Spare Part usage is performed through:

```text
Inventory Reservation

↓

Inventory Issue

↓

Maintenance Material Consumption

↓

Inventory Transaction

↓

Inventory Balance Update
```

Inventory remains the System of Record.

---

# AUDIT

Audit every

- Spare Part creation
- Activation
- Supplier assignment
- Warehouse assignment
- Compatible Asset assignment
- Alternative Part assignment
- Stock Policy update
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

- Create Spare Part
- Activate Spare Part
- Assign Supplier
- Assign Warehouse
- Add Compatible Asset
- Add Alternative Part
- Archive Spare Part
- Prevent duplicate Spare Part Code

## Integration Tests

- Repository
- Commands
- Queries
- REST API
- Product integration
- Inventory integration
- Supplier integration
- Domain Events
- Audit

---

# ACCEPTANCE CRITERIA

- Every Spare Part references a Product Revision.
- Capability Profile validation is enforced.
- Inventory ownership remains within the Inventory module.
- Compatible Assets are maintained.
- Alternative Spare Parts are supported.
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
- Product integration completed
- Inventory integration completed
- Supplier integration completed
- Authorization implemented
- Audit implemented
- Domain Events implemented
- Unit tests passing
- Integration tests passing
- Code review approved

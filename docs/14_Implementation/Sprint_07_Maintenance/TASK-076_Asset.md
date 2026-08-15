# ==============================================================================
# TASK-076 — IMPLEMENTATION
# ASSET MANAGEMENT
# Naswood Operating System (NOS)
# Module: Maintenance Management
# Sprint: Sprint 07 – Maintenance
# Status: Ready for Development
# ==============================================================================

# OBJECTIVE

Implement the Asset aggregate responsible for managing all maintainable physical
assets used throughout the enterprise.

Assets represent machines, equipment, production lines, utilities, vehicles and
other maintainable resources.

The Asset module is the master data source for Maintenance Management.

It does not manage maintenance activities directly.

---

# DOMAIN

Maintenance Management

Aggregate Root

```
Asset
```

---

# REFERENCES

Implementation must comply with:

- Constitution
- Maintenance_Architecture.md
- Maintenance_Workflow.md
- Maintenance_API.md
- Machine Master
- Work Center
- Location Master
- Document Management

---

# DEPENDENCIES

Requires completed modules:

- Organization
- Location
- Work Center
- Employee
- Document Management

---

# AGGREGATE

```
Asset
```

Children

```
AssetComponent

AssetSpecification

AssetLocation

AssetDocument

AssetWarranty

AuditEntry
```

---

# VALUE OBJECTS

```
AssetCode

AssetStatus

AssetCategory

AssetCriticality

CommissionDate
```

---

# ENUMS

## AssetStatus

```text
Draft
Active
InService
OutOfService
UnderMaintenance
Retired
Disposed
Archived
```

---

## AssetCategory

```text
Machine

Equipment

ProductionLine

Utility

Vehicle

Building

Tool

Infrastructure

Other
```

---

## AssetCriticality

```text
Low

Medium

High

Critical
```

---

# ENTITY FIELDS

```text
Id

AssetCode

AssetName

Description

Category

Criticality

Status

Manufacturer

Model

SerialNumber

AssetTag

WorkCenterId

LocationId

ParentAssetId

CommissionDate

WarrantyStart

WarrantyEnd

PurchaseDate

CreatedAt

UpdatedAt
```

---

# ASSET COMPONENT

```text
Id

AssetId

ComponentCode

ComponentName

PartNumber

Manufacturer

SerialNumber

Status
```

---

# ASSET SPECIFICATION

```text
Id

AssetId

SpecificationName

SpecificationValue

UnitOfMeasureId
```

---

# DOMAIN INVARIANTS

Every Asset has one unique Asset Code.

Asset Code is immutable.

Every Asset belongs to one Category.

Only Active Assets may receive Maintenance Work Orders.

Retired Assets cannot return to Active status.

Asset hierarchy must not contain cycles.

---

# DOMAIN METHODS

```text
Create()

Activate()

AssignLocation()

AssignWorkCenter()

AddComponent()

AddSpecification()

UpdateWarranty()

Retire()

Dispose()

Archive()
```

---

# DOMAIN EVENTS

```text
AssetCreated

AssetActivated

AssetLocationChanged

AssetComponentAdded

AssetWarrantyUpdated

AssetRetired

AssetDisposed

AssetArchived
```

---

# VALIDATIONS

Create

- Asset Code unique
- Category valid
- Location exists

Activate

- Commission Date defined
- Required specifications completed

Retire

- No open Maintenance Work Orders

Dispose

- Asset retired
- No active dependencies

---

# REPOSITORY

```text
IAssetRepository
```

Methods

```csharp
Task<Asset?> GetByIdAsync(Guid id);

Task<Asset?> GetByCodeAsync(string assetCode);

Task<IEnumerable<Asset>> GetActiveAsync();

Task AddAsync(Asset entity);

Task UpdateAsync(Asset entity);
```

---

# COMMANDS

```text
CreateAssetCommand

ActivateAssetCommand

AssignAssetLocationCommand

AssignAssetWorkCenterCommand

AddAssetComponentCommand

UpdateWarrantyCommand

RetireAssetCommand

DisposeAssetCommand

ArchiveAssetCommand
```

---

# QUERIES

```text
GetAssetByIdQuery

GetAssetsQuery

GetActiveAssetsQuery

GetAssetsByWorkCenterQuery

GetAssetsByLocationQuery
```

---

# API ENDPOINTS

```http
GET    /api/v1/maintenance/assets

GET    /api/v1/maintenance/assets/{id}

POST   /api/v1/maintenance/assets

PUT    /api/v1/maintenance/assets/{id}

POST   /api/v1/maintenance/assets/{id}/activate

POST   /api/v1/maintenance/assets/{id}/retire

POST   /api/v1/maintenance/assets/{id}/dispose

POST   /api/v1/maintenance/assets/{id}/archive
```

---

# AUTHORIZATION

```text
maintenance.asset.read

maintenance.asset.create

maintenance.asset.update

maintenance.asset.activate

maintenance.asset.retire

maintenance.asset.dispose

maintenance.asset.archive
```

---

# DATABASE TABLES

## Assets

```text
Id

AssetCode

AssetName

Description

Category

Criticality

Status

Manufacturer

Model

SerialNumber

AssetTag

WorkCenterId

LocationId

ParentAssetId

CommissionDate

WarrantyStart

WarrantyEnd

PurchaseDate

CreatedAt

UpdatedAt
```

---

## AssetComponents

```text
Id

AssetId

ComponentCode

ComponentName

PartNumber

Manufacturer

SerialNumber

Status
```

---

## AssetSpecifications

```text
Id

AssetId

SpecificationName

SpecificationValue

UnitOfMeasureId
```

---

# INDEXES

```text
IX_AssetCode (Unique)

IX_AssetTag

IX_Category

IX_Status

IX_WorkCenterId

IX_LocationId

IX_ParentAssetId
```

---

# AUDIT

Audit every

- Asset creation
- Status changes
- Location changes
- Component additions
- Warranty updates
- Retirement
- Disposal
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

- Create Asset
- Activate Asset
- Assign Location
- Add Component
- Update Warranty
- Retire Asset
- Dispose Asset
- Prevent duplicate Asset Code
- Prevent retirement with open Work Orders

## Integration Tests

- Repository
- Commands
- Queries
- REST API
- Work Center integration
- Document integration
- Domain Events
- Audit

---

# ACCEPTANCE CRITERIA

- Every Asset has a unique Asset Code.
- Asset hierarchy supports parent-child relationships.
- Only Active Assets can receive maintenance.
- Warranty and specification information are traceable.
- Asset lifecycle is fully auditable.
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
- Work Center integration completed
- Authorization implemented
- Audit implemented
- Domain Events implemented
- Unit tests passing
- Integration tests passing
- Code review approved

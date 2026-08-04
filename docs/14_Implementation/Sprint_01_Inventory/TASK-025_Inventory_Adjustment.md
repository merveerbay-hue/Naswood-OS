# TASK-025 Inventory Adjustment

**Sprint:** Sprint_01_Inventory

**Module:** Inventory

**Priority:** Critical

**Estimated Effort:** 10 Hours

**Status:** Planned

---

# Objective

Implement the Inventory Adjustment module.

Inventory Adjustment is the only authorized process for correcting inventory discrepancies after an approved inventory count or exceptional inventory event.

Every adjustment must be fully auditable, approved, and automatically update inventory through inventory transactions.

Direct modification of inventory balances is strictly prohibited.

---

# Business Value

Inventory Adjustment enables:

- Inventory Corrections
- Cycle Count Reconciliation
- Financial Accuracy
- Warehouse Accuracy
- Audit Compliance
- Production Reliability
- Digital Twin Synchronization
- AI Analytics

---

# References

README.md

Cursor_Rules.md

docs/01_Business/Business_Rules.md

docs/01_Business/Factory_Flow.md

docs/05_Modules/06_Inventory/Inventory_Adjustments.md

docs/13_Design/02_Inventory/Inventory_Adjustment.md

---

# Scope

Implement complete Inventory Adjustment including:

- Manual Adjustment
- Count Difference Adjustment
- Damage Adjustment
- Loss Adjustment
- Gain Adjustment
- Quality Adjustment
- Batch Adjustment
- Approval Workflow
- Adjustment History

---

# Adjustment Types

Inventory Count

Damage

Loss

Found Inventory

Quality Hold

Scrap

Expired Material

Production Correction

Manual Adjustment

System Correction

---

# Functional Requirements

The system shall support:

- Create Adjustment
- Update Draft Adjustment
- Submit for Approval
- Approve Adjustment
- Reject Adjustment
- Execute Adjustment
- Cancel Draft Adjustment
- View Adjustment History
- Export Adjustments

---

# Adjustment Header

Adjustment Number

Adjustment Type

Warehouse

Location

Reference Document

Reason

Requested By

Approved By

Adjustment Date

Status

Notes

Created At

Updated At

---

# Adjustment Line

Material

Warehouse

Location

Batch

Lot

Serial Number

Current Quantity

Adjustment Quantity

New Quantity

Unit

Reason

Remarks

---

# Adjustment Status

Draft

Pending Approval

Approved

Rejected

Completed

Cancelled

---

# Business Rules

Adjustment Number must be unique.

Warehouse is required.

Material is required.

Adjustment Quantity cannot be zero.

Reason is mandatory.

Completed Adjustments cannot be modified.

Rejected Adjustments cannot update inventory.

Only Approved Adjustments may affect inventory.

Every Adjustment must create Stock Movements.

Every Adjustment must generate an Audit Log.

Every Adjustment must update Digital Twin.

---

# Inventory Updates

Approved Adjustment must automatically:

Validate Inventory

Create Inventory Transaction

Increase or Decrease Inventory

Update Batch Quantity

Create Stock Movement

Update Inventory History

Update Inventory Valuation

Publish Inventory Event

Update Digital Twin

---

# Adjustment Calculation

New Quantity

=

Current Quantity

+

Adjustment Quantity

Positive Adjustment

Inventory Increase

Negative Adjustment

Inventory Decrease

---

# Validation

Warehouse Required

Material Required

Reason Required

Approval Required

Adjustment Quantity ≠ 0

Batch Validation

Permission Validation

---

# Relationships

Inventory Count

↓

Inventory Adjustment

↓

Inventory

↓

Batch

↓

Stock Movement

↓

Audit Log

↓

Finance

---

# Permissions

InventoryAdjustment.View

InventoryAdjustment.Create

InventoryAdjustment.Update

InventoryAdjustment.Approve

InventoryAdjustment.Reject

InventoryAdjustment.Execute

InventoryAdjustment.Export

---

# API

GET /inventory-adjustments

GET /inventory-adjustments/{id}

POST /inventory-adjustments

PUT /inventory-adjustments/{id}

DELETE /inventory-adjustments/{id}

POST /inventory-adjustments/{id}/submit

POST /inventory-adjustments/{id}/approve

POST /inventory-adjustments/{id}/reject

POST /inventory-adjustments/{id}/execute

GET /inventory-adjustments/search

---

# UI Pages

Inventory Adjustment List

Inventory Adjustment Detail

Create Adjustment

Approval Screen

Adjustment History

Inventory Difference Review

---

# UI Components

Search Box

Warehouse Selector

Location Selector

Material Selector

Batch Selector

Adjustment Grid

Reason Selector

Approval Workflow

Status Badge

Export Button

---

# Database

Table

InventoryAdjustments

Columns

Id

AdjustmentNumber

AdjustmentType

WarehouseId

LocationId

ReferenceDocument

Reason

RequestedBy

ApprovedBy

AdjustmentDate

Status

Notes

CreatedAt

UpdatedAt

CreatedBy

UpdatedBy

---

# Database

Table

InventoryAdjustmentLines

Columns

Id

InventoryAdjustmentId

MaterialId

WarehouseId

LocationId

BatchId

LotId

SerialNumber

CurrentQuantity

AdjustmentQuantity

NewQuantity

Unit

Reason

Remarks

---

# Events

InventoryAdjustmentCreated

InventoryAdjustmentSubmitted

InventoryAdjustmentApproved

InventoryAdjustmentRejected

InventoryAdjustmentExecuted

InventoryIncreased

InventoryDecreased

StockMovementCreated

InventoryUpdated

---

# Audit

Every adjustment must record:

User

Timestamp

Warehouse

Location

Material

Batch

Current Quantity

Adjustment Quantity

New Quantity

Reason

Approval User

Reference Document

---

# Performance Requirements

Support:

- Pagination
- Filtering
- Search
- Multi-line Adjustments
- Bulk Approval

Inventory updates must execute within a single database transaction.

---

# Tests

Create Adjustment

Submit Adjustment

Approve Adjustment

Reject Adjustment

Execute Adjustment

Positive Adjustment

Negative Adjustment

Batch Adjustment

Permission Validation

Duplicate Adjustment Validation

Audit Validation

Performance Test

Concurrent Transaction Test

---

# Acceptance Criteria

✔ Inventory Adjustment CRUD completed

✔ Approval workflow implemented

✔ Positive adjustments supported

✔ Negative adjustments supported

✔ Batch adjustments supported

✔ Inventory automatically updated

✔ Stock Movement automatically created

✔ Inventory valuation updated

✔ Audit Log enabled

✔ OpenAPI documented

✔ Unit Tests passed

✔ Integration Tests passed

✔ Performance requirements satisfied

---

# Deliverables

- Domain Entity
- Repository
- CQRS Commands
- CQRS Queries
- Adjustment Service
- Approval Service
- Inventory Update Service
- Stock Movement Service
- Batch Service
- DTOs
- Validators
- REST API
- Database Migration
- React Pages
- Adjustment Dashboard
- Unit Tests
- Integration Tests
- Swagger Documentation

---

# Cursor Implementation Prompt

Read:

- Cursor_Rules.md
- Business_Rules.md
- Factory_Flow.md
- docs/13_Design/02_Inventory/Inventory_Adjustment.md
- This TASK document

Implement the complete Inventory Adjustment module using:

- .NET 9
- ASP.NET Core
- Clean Architecture
- CQRS
- MediatR
- EF Core
- PostgreSQL
- FluentValidation
- React
- TypeScript

Requirements:

- Inventory must never be modified directly.
- Only approved Inventory Adjustments may change inventory.
- Every adjustment must automatically create Stock Movement records.
- Support positive and negative inventory adjustments.
- Support batch-controlled adjustments.
- Support approval workflow.
- Update inventory valuation after execution.
- All operations must be transactional.
- Publish integration events after successful completion.
- Follow project architecture exactly.
- Do not implement unrelated modules.

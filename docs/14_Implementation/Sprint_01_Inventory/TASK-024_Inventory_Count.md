# TASK-024 Inventory Counting

**Sprint:** Sprint_01_Inventory

**Module:** Inventory

**Priority:** Critical

**Estimated Effort:** 10 Hours

**Status:** Completed

---

# Objective

Implement the Inventory Counting module.

Inventory Counting verifies the physical inventory against the system inventory and generates inventory adjustments when discrepancies are approved.

The module must support complete warehouse counts, cycle counting and location-based counting.

---

# Business Value

Inventory Counting ensures:

- Inventory Accuracy
- Warehouse Reliability
- Financial Accuracy
- Production Continuity
- Audit Compliance
- AI Inventory Analysis
- Digital Twin Synchronization

---

# References

README.md

Cursor_Rules.md

docs/01_Business/Business_Rules.md

docs/01_Business/Factory_Flow.md

docs/05_Modules/06_Inventory/Inventory_Adjustments.md

docs/13_Design/02_Inventory/Inventory_Count.md

---

# Scope

Implement complete Inventory Counting including:

- Full Warehouse Count
- Location Count
- Material Count
- Batch Count
- Cycle Count
- Blind Count
- Count Approval
- Inventory Difference Calculation
- Adjustment Proposal

---

# Count Types

Full Inventory Count

Cycle Count

Warehouse Count

Location Count

Material Count

Batch Count

Blind Count

Spot Check

---

# Functional Requirements

The system shall support:

- Create Count Session
- Assign Counters
- Freeze Inventory
- Record Physical Quantity
- Compare System Quantity
- Calculate Difference
- Generate Adjustment Proposal
- Approve Count
- Close Count Session

---

# Count Header

Count Number

Count Type

Warehouse

Location

Status

Start Date

End Date

Created By

Approved By

Description

---

# Count Line

Material

Warehouse

Location

Batch

Lot

Serial Number

System Quantity

Physical Quantity

Difference

Difference Value

Count Status

Remarks

---

# Count Status

Draft

Open

Counting

Pending Approval

Approved

Completed

Cancelled

---

# Business Rules

Count Number must be unique.

Warehouse is required.

Location is optional.

Inventory may be frozen during counting.

Physical Quantity cannot be negative.

System Quantity cannot be manually modified.

Only approved counts may create adjustments.

Completed counts cannot be edited.

Cancelled counts must not affect inventory.

Blind Counts must hide System Quantity until completion.

---

# Inventory Updates

Inventory Count itself does NOT change inventory.

Approved Count creates:

Inventory Adjustment

↓

Stock Movement

↓

Inventory Update

↓

Audit Log

↓

Digital Twin Update

---

# Difference Calculation

Difference

=

Physical Quantity

-

System Quantity

Positive Difference

Inventory Gain

Negative Difference

Inventory Loss

Zero Difference

No Action Required

---

# Validation

Warehouse Required

Material Required

Physical Quantity >= 0

Duplicate Count Prevention

Count Status Validation

Approval Required

---

# Relationships

Warehouse

↓

Location

↓

Inventory Count

↓

Inventory

↓

Batch

↓

Inventory Adjustment

↓

Stock Movement

---

# Permissions

InventoryCount.View

InventoryCount.Create

InventoryCount.Update

InventoryCount.Approve

InventoryCount.Cancel

InventoryCount.Export

---

# API

GET /inventory-counts

GET /inventory-counts/{id}

POST /inventory-counts

PUT /inventory-counts/{id}

DELETE /inventory-counts/{id}

POST /inventory-counts/{id}/start

POST /inventory-counts/{id}/complete

POST /inventory-counts/{id}/approve

GET /inventory-counts/search

---

# UI Pages

Inventory Count List

Inventory Count Detail

Create Count

Count Session

Difference Review

Approval Screen

Count History

---

# UI Components

Search Box

Warehouse Selector

Location Selector

Batch Selector

Material Grid

Physical Quantity Input

Difference Indicator

Approval Button

Status Badge

Export Button

---

# Database

Table

InventoryCounts

Columns

Id

CountNumber

CountType

WarehouseId

LocationId

Status

StartDate

EndDate

Description

CreatedAt

UpdatedAt

CreatedBy

UpdatedBy

ApprovedBy

ApprovedAt

---

# Database

Table

InventoryCountLines

Columns

Id

InventoryCountId

MaterialId

WarehouseId

LocationId

BatchId

LotId

SerialNumber

SystemQuantity

PhysicalQuantity

Difference

DifferenceValue

CountStatus

Remarks

---

# Events

InventoryCountCreated

InventoryCountStarted

InventoryCountCompleted

InventoryCountApproved

InventoryDifferenceDetected

InventoryAdjustmentRequested

InventoryUpdated

---

# Audit

Every count action must record:

User

Timestamp

Warehouse

Location

Material

Batch

System Quantity

Physical Quantity

Difference

Approval User

Reason

---

# Performance Requirements

Support:

- Pagination
- Filtering
- Search
- Barcode Scanning
- Mobile Counting
- Offline Counting Support

Counting thousands of inventory records must remain performant.

---

# Tests

Create Count

Warehouse Count

Location Count

Batch Count

Blind Count

Difference Calculation

Approval Workflow

Inventory Freeze

Adjustment Creation

Permission Validation

Performance Test

---

# Acceptance Criteria

✔ Inventory Count CRUD completed

✔ Full Warehouse Count supported

✔ Cycle Count supported

✔ Blind Count supported

✔ Difference calculation implemented

✔ Approval workflow implemented

✔ Inventory Adjustment proposal generated

✔ Audit Log enabled

✔ Barcode scanning supported

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
- Count Service
- Difference Calculation Service
- Approval Service
- DTOs
- Validators
- REST API
- Database Migration
- React Pages
- Inventory Count Dashboard
- Unit Tests
- Integration Tests
- Swagger Documentation

---

# Cursor Implementation Prompt

Read:

- Cursor_Rules.md
- Business_Rules.md
- Factory_Flow.md
- docs/13_Design/02_Inventory/Inventory_Count.md
- This TASK document

Implement the complete Inventory Counting module using:

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

- Support full inventory counts and cycle counts.
- Support blind counting.
- Inventory Count must not directly modify inventory.
- Inventory changes must only occur through approved Inventory Adjustments.
- Support barcode scanning and mobile counting.
- All operations must be transactional.
- Publish integration events after successful completion.
- Follow project architecture exactly.
- Do not implement unrelated modules.

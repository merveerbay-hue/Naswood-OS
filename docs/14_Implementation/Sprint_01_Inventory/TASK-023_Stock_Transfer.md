# TASK-023 Stock Transfer

**Sprint:** Sprint_01_Inventory

**Module:** Inventory

**Priority:** Critical

**Estimated Effort:** 10 Hours

**Status:** Planned

---

# Objective

Implement the Stock Transfer module.

Stock Transfer enables controlled movement of inventory between warehouses and locations while maintaining complete inventory integrity, batch traceability and audit history.

Every physical movement inside the factory must be recorded as a Stock Transfer transaction.

---

# Business Value

Stock Transfer is required for:

- Warehouse Operations
- Production Supply
- Internal Logistics
- Inventory Optimization
- Material Staging
- Quality Inspection
- Returns
- Digital Twin
- AI Analytics

Transfers must never change total inventory quantity.

Only inventory location changes.

---

# References

README.md

Cursor_Rules.md

docs/01_Business/Business_Rules.md

docs/01_Business/Factory_Flow.md

docs/05_Modules/06_Inventory/Stock_Movements.md

docs/13_Design/02_Inventory/Stock_Transfer.md

---

# Scope

Implement complete Stock Transfer Management including:

- Warehouse to Warehouse Transfer
- Location to Location Transfer
- Warehouse to Location Transfer
- Production Supply Transfer
- Quality Transfer
- Scrap Transfer
- Batch Transfer
- Barcode Scanning
- Transfer History

---

# Transfer Types

Warehouse Transfer

Location Transfer

Production Supply

Quality Transfer

Scrap Transfer

Return Transfer

Internal Logistics

Manual Transfer

---

# Functional Requirements

The system shall support:

- Create Transfer
- Multi-line Transfer
- Partial Transfer
- Complete Transfer
- Source Warehouse Selection
- Destination Warehouse Selection
- Source Location Selection
- Destination Location Selection
- Batch Transfer
- Barcode Scanning
- Transfer History

---

# Transfer Header

Transfer Number

Transfer Type

Transfer Date

Reference Document

Source Warehouse

Destination Warehouse

Requested By

Approved By

Transferred By

Status

Remarks

Created At

Updated At

---

# Transfer Line

Material

Description

Source Warehouse

Source Location

Destination Warehouse

Destination Location

Batch

Lot

Serial Number

Quantity

Unit

Remarks

---

# Transfer Status

Draft

Pending Approval

Approved

In Progress

Completed

Cancelled

---

# Business Rules

Transfer Number must be unique.

Source Warehouse is required.

Destination Warehouse is required.

Source Location is required.

Destination Location is required.

Source and Destination cannot be the same.

Material is required.

Quantity must be greater than zero.

Available Inventory must be sufficient.

Batch must be transferred together with inventory.

Transfer does not change total inventory.

Completed Transfers cannot be modified.

Cancelled Transfers must reverse inventory movements.

---

# Inventory Updates

Stock Transfer must automatically:

Validate Inventory

Reduce Source Inventory

Increase Destination Inventory

Move Batch Inventory

Create Stock Movement

Update Inventory History

Update Digital Twin

Publish Inventory Event

---

# Validation

Verify Source Warehouse

Verify Destination Warehouse

Verify Source Location

Verify Destination Location

Verify Available Inventory

Verify Batch

Verify Material Status

Reject transaction if validation fails.

---

# Relationships

Warehouse

↓

Location

↓

Stock Transfer

↓

Inventory

↓

Batch

↓

Stock Movement

↓

Audit Log

---

# Permissions

Transfer.View

Transfer.Create

Transfer.Approve

Transfer.Update

Transfer.Cancel

Transfer.Export

Transfer.Print

---

# API

GET /transfers

GET /transfers/{id}

POST /transfers

PUT /transfers/{id}

DELETE /transfers/{id}

POST /transfers/{id}/approve

POST /transfers/{id}/complete

POST /transfers/{id}/cancel

GET /transfers/search

---

# UI Pages

Transfer List

Transfer Detail

Create Transfer

Transfer Approval

Transfer History

Transfer Dashboard

---

# UI Components

Transfer Header

Material Grid

Barcode Scanner

Warehouse Selector

Location Selector

Batch Selector

Quantity Input

Status Badge

Approval Timeline

Print Button

Export Button

---

# Database

Table

StockTransfers

Columns

Id

TransferNumber

TransferType

SourceWarehouseId

DestinationWarehouseId

ReferenceNumber

TransferDate

RequestedBy

ApprovedBy

TransferredBy

Status

Remarks

CreatedAt

UpdatedAt

CreatedBy

UpdatedBy

---

# Database

Table

StockTransferLines

Columns

Id

StockTransferId

MaterialId

SourceWarehouseId

DestinationWarehouseId

SourceLocationId

DestinationLocationId

BatchId

LotId

SerialNumber

Quantity

Unit

Remarks

---

# Events

TransferCreated

TransferApproved

TransferCompleted

TransferCancelled

InventoryTransferred

BatchTransferred

StockMovementCreated

InventoryUpdated

---

# Audit

Every transfer must record:

User

Timestamp

Material

Source Warehouse

Destination Warehouse

Source Location

Destination Location

Batch

Transferred Quantity

Previous Inventory

New Inventory

Reference Document

Reason

---

# Performance Requirements

Support:

- Pagination
- Filtering
- Search
- Barcode Scanning
- Multi-line Transfers

Inventory updates must execute within a single database transaction.

Transfers must be atomic.

---

# Tests

Create Transfer

Warehouse Transfer

Location Transfer

Batch Transfer

Partial Transfer

Inventory Validation

Insufficient Inventory Validation

Approval Workflow

Cancel Transfer

Rollback Inventory

Permission Validation

Performance Test

Concurrent Transaction Test

---

# Acceptance Criteria

✔ Transfer CRUD completed

✔ Warehouse Transfer supported

✔ Location Transfer supported

✔ Batch Transfer supported

✔ Inventory updated automatically

✔ Source inventory reduced

✔ Destination inventory increased

✔ Stock Movement automatically created

✔ Transfer Approval implemented

✔ Audit Log enabled

✔ Barcode supported

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
- Transfer Service
- Approval Service
- Inventory Validation Service
- Inventory Update Service
- Batch Service
- Stock Movement Service
- DTOs
- Validators
- REST API
- Database Migration
- React Pages
- Transfer Dashboard
- Unit Tests
- Integration Tests
- Swagger Documentation

---

# Cursor Implementation Prompt

Read:

- Cursor_Rules.md
- Business_Rules.md
- Factory_Flow.md
- docs/13_Design/02_Inventory/Stock_Transfer.md
- This TASK document

Implement the complete Stock Transfer module using:

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

- Support Warehouse-to-Warehouse transfers.
- Support Location-to-Location transfers.
- Support Batch-controlled transfers.
- Total inventory quantity must never change.
- Automatically create Stock Movement records.
- All operations must be transactional.
- Publish integration events after successful completion.
- Follow project architecture exactly.
- Do not implement unrelated modules.

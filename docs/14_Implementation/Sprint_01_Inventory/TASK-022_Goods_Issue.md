# TASK-022 Goods Issue

**Sprint:** Sprint_01_Inventory

**Module:** Inventory

**Priority:** Critical

**Estimated Effort:** 12 Hours

**Status:** Completed

---

# Objective

Implement the Goods Issue module.

Goods Issue records the physical removal of materials from inventory and automatically updates inventory balances, stock movements and traceability.

Every inventory reduction must be performed through a Goods Issue transaction.

---

# Business Value

Goods Issue is required for:

- Production Material Consumption
- Sales Shipment
- Internal Consumption
- Warehouse Transfer
- Scrap
- Quality Disposal
- Sample Usage
- Inventory Adjustment

Without Goods Issue inventory cannot remain accurate.

---

# References

README.md

Cursor_Rules.md

docs/01_Business/Business_Rules.md

docs/01_Business/Factory_Flow.md

docs/05_Modules/06_Inventory/Inventory.md

docs/13_Design/02_Inventory/Goods_Issue.md

---

# Scope

Implement complete Goods Issue Management including:

- Manual Goods Issue
- Production Consumption
- Sales Shipment
- Internal Consumption
- Scrap Issue
- Batch Selection
- Warehouse Selection
- Location Selection
- Barcode Scanning
- Issue History

---

# Issue Types

Production Consumption

Sales Shipment

Warehouse Transfer

Scrap

Quality Disposal

Sample

Inventory Adjustment

Manual Issue

---

# Functional Requirements

The system shall support:

- Create Goods Issue
- Multi-line Goods Issue
- Partial Issue
- Complete Issue
- Batch Selection
- Warehouse Selection
- Location Selection
- Inventory Validation
- Automatic Inventory Update
- Automatic Stock Movement
- Print Goods Issue Document

---

# Goods Issue Header

Issue Number

Issue Type

Issue Date

Reference Document

Production Order

Sales Order

Warehouse

Issued By

Status

Notes

Created At

Updated At

---

# Goods Issue Line

Material

Description

Warehouse

Location

Batch

Lot

Serial Number

Quantity

Unit

Inventory Status

Quality Status

Remarks

---

# Issue Status

Draft

Picking

Completed

Cancelled

---

# Business Rules

Issue Number must be unique.

Warehouse is required.

Location is required.

Material is required.

Batch is required if Batch Tracking is enabled.

Quantity must be greater than zero.

Available Inventory must be sufficient.

Negative Inventory is not allowed.

Completed Goods Issues cannot be modified.

Cancelled Goods Issues must automatically restore inventory.

Every Goods Issue must create a Stock Movement.

Every Goods Issue must generate an Audit Log.

---

# Inventory Updates

Goods Issue must automatically:

Validate Available Inventory

Reduce Inventory Quantity

Reduce Batch Quantity

Create Stock Movement

Update Material Availability

Update Inventory History

Update Digital Twin

Publish Inventory Event

---

# Inventory Validation

Before posting the transaction:

Verify Warehouse

Verify Location

Verify Batch

Verify Inventory Status

Verify Available Quantity

Verify Reservation

Reject transaction if validation fails.

---

# Relationships

Production Order

↓

Sales Order

↓

Goods Issue

↓

Warehouse

↓

Location

↓

Batch

↓

Inventory

↓

Stock Movement

↓

Shipment

---

# Permissions

GoodsIssue.View

GoodsIssue.Create

GoodsIssue.Update

GoodsIssue.Cancel

GoodsIssue.Print

GoodsIssue.Export

---

# API

GET /goods-issues

GET /goods-issues/{id}

POST /goods-issues

PUT /goods-issues/{id}

DELETE /goods-issues/{id}

POST /goods-issues/{id}/complete

POST /goods-issues/{id}/cancel

GET /goods-issues/search

---

# UI Pages

Goods Issue List

Goods Issue Detail

Create Goods Issue

Production Consumption

Sales Shipment

Goods Issue History

---

# UI Components

Issue Header

Material Grid

Barcode Scanner

Warehouse Selector

Location Selector

Batch Selector

Quantity Input

Available Stock Display

Status Badge

Print Button

Export Button

---

# Database

Table

GoodsIssues

Columns

Id

IssueNumber

IssueType

ReferenceNumber

ProductionOrderId

SalesOrderId

WarehouseId

IssueDate

Status

Remarks

CreatedAt

UpdatedAt

CreatedBy

UpdatedBy

---

# Database

Table

GoodsIssueLines

Columns

Id

GoodsIssueId

MaterialId

WarehouseId

LocationId

BatchId

LotId

SerialNumber

Quantity

Unit

InventoryStatus

QualityStatus

Remarks

---

# Events

GoodsIssueCreated

GoodsIssueCompleted

GoodsIssueCancelled

InventoryDecreased

BatchUpdated

StockMovementCreated

InventoryUpdated

---

# Audit

Every transaction must record:

User

Timestamp

Warehouse

Location

Material

Batch

Issued Quantity

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
- Multi-line Issue

Inventory validation and update must execute within a single database transaction.

---

# Tests

Create Goods Issue

Production Consumption

Sales Shipment

Partial Issue

Inventory Validation

Batch Validation

Insufficient Inventory Validation

Stock Movement Creation

Permission Validation

Cancel Goods Issue

Inventory Rollback

Performance Test

Concurrent Transaction Test

---

# Acceptance Criteria

✔ Goods Issue CRUD completed

✔ Production Consumption supported

✔ Sales Shipment supported

✔ Batch selection implemented

✔ Warehouse selection implemented

✔ Location selection implemented

✔ Inventory validation implemented

✔ Inventory automatically reduced

✔ Stock Movement automatically created

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
- Goods Issue Service
- Inventory Validation Service
- Inventory Update Service
- Batch Service
- Stock Movement Service
- DTOs
- Validators
- REST API
- Database Migration
- React Pages
- Goods Issue Dashboard
- Unit Tests
- Integration Tests
- Swagger Documentation

---

# Cursor Implementation Prompt

Read:

- Cursor_Rules.md
- Business_Rules.md
- Factory_Flow.md
- docs/13_Design/02_Inventory/Goods_Issue.md
- This TASK document

Implement the complete Goods Issue module using:

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

- Goods Issue must never allow negative inventory.
- Every Goods Issue must create Stock Movement records.
- Support Production, Sales, Transfer and Manual Issue scenarios.
- Support Batch, Warehouse and Location validation.
- Support barcode scanning.
- All inventory operations must be transactional.
- Publish integration events after successful completion.
- Follow project architecture exactly.
- Do not implement unrelated modules.

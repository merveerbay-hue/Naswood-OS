# TASK-021 Goods Receipt

**Sprint:** Sprint_01_Inventory

**Module:** Inventory

**Priority:** Critical

**Estimated Effort:** 12 Hours

**Status:** Planned

---

# Objective

Implement the Goods Receipt module.

Goods Receipt records the physical arrival of materials into the warehouse and creates the corresponding inventory transactions.

Every received material must become fully traceable through Warehouse, Location, Batch, Inventory and Audit records.

---

# Business Value

Goods Receipt is the entry point of all physical inventory.

It is required by:

- Purchasing
- Inventory
- Warehouse
- Production
- Quality
- Finance
- AI
- Digital Twin

No inventory can exist without a Goods Receipt transaction.

---

# References

README.md

Cursor_Rules.md

docs/01_Business/Business_Rules.md

docs/01_Business/Factory_Flow.md

docs/05_Modules/06_Inventory/Inventory.md

docs/13_Design/02_Inventory/Goods_Receipt.md

---

# Scope

Implement complete Goods Receipt Management including:

- Manual Goods Receipt
- Purchase Order Receipt
- Production Receipt
- Return Receipt
- Batch Assignment
- Warehouse Assignment
- Location Assignment
- Barcode Scanning
- Quality Hold
- Receipt History

---

# Receipt Types

Purchase Order

Production Order

Customer Return

Supplier Return

Transfer Receipt

Inventory Adjustment

Manual Receipt

---

# Functional Requirements

The system shall support:

- Create Goods Receipt
- Receive Multiple Items
- Partial Receipt
- Complete Receipt
- Batch Assignment
- Warehouse Assignment
- Location Assignment
- Automatic Inventory Update
- Automatic Stock Movement
- Print Labels
- Print Receipt Document

---

# Goods Receipt Header

Receipt Number

Receipt Type

Receipt Date

Reference Document

Purchase Order

Production Order

Supplier

Warehouse

Received By

Status

Notes

Created At

Updated At

---

# Goods Receipt Line

Material

Description

Warehouse

Location

Batch

Lot

Serial Number

Quantity

Unit

Unit Cost

Inventory Status

Quality Status

Remarks

---

# Receipt Status

Draft

Receiving

Quality Hold

Completed

Cancelled

---

# Business Rules

Receipt Number must be unique.

Warehouse is required.

Location is required.

Material is required.

Quantity must be greater than zero.

Batch must exist or be created.

Inventory must be updated automatically.

Stock Movement must be created automatically.

Audit Log is mandatory.

Completed receipts cannot be modified.

Cancelled receipts must reverse inventory.

---

# Inventory Updates

Goods Receipt must automatically:

Create Inventory Record (if not exists)

Increase Inventory Quantity

Create Batch (if required)

Assign Warehouse

Assign Location

Generate Stock Movement

Generate Audit Log

Update Digital Twin

Publish Inventory Event

---

# Relationships

Purchase Order

↓

Goods Receipt

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

Quality

---

# Permissions

GoodsReceipt.View

GoodsReceipt.Create

GoodsReceipt.Update

GoodsReceipt.Cancel

GoodsReceipt.Print

GoodsReceipt.Export

---

# API

GET /goods-receipts

GET /goods-receipts/{id}

POST /goods-receipts

PUT /goods-receipts/{id}

DELETE /goods-receipts/{id}

POST /goods-receipts/{id}/complete

POST /goods-receipts/{id}/cancel

GET /goods-receipts/search

---

# UI Pages

Goods Receipt List

Goods Receipt Detail

Create Goods Receipt

Receive Purchase Order

Receive Production

Goods Receipt History

---

# UI Components

Receipt Header

Material Grid

Barcode Scanner

Warehouse Selector

Location Selector

Batch Selector

Quantity Input

Status Badge

Print Button

Export Button

---

# Database

Table

GoodsReceipts

Columns

Id

ReceiptNumber

ReceiptType

ReferenceNumber

SupplierId

PurchaseOrderId

ProductionOrderId

WarehouseId

ReceiptDate

Status

Remarks

CreatedAt

UpdatedAt

CreatedBy

UpdatedBy

---

# Database

Table

GoodsReceiptLines

Columns

Id

GoodsReceiptId

MaterialId

WarehouseId

LocationId

BatchId

LotId

SerialNumber

Quantity

Unit

UnitCost

InventoryStatus

QualityStatus

Remarks

---

# Events

GoodsReceiptCreated

GoodsReceiptCompleted

GoodsReceiptCancelled

InventoryIncreased

BatchCreated

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

Quantity

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
- Multi-line Receipt

Inventory update must execute within a single database transaction.

---

# Tests

Create Goods Receipt

Receive Purchase Order

Receive Production Order

Partial Receipt

Batch Creation

Inventory Update

Stock Movement Creation

Duplicate Receipt Validation

Permission Validation

Cancel Receipt

Rollback Inventory

Performance Test

---

# Acceptance Criteria

✔ Goods Receipt CRUD completed

✔ Purchase Order receipt supported

✔ Production receipt supported

✔ Batch assignment implemented

✔ Warehouse assignment implemented

✔ Location assignment implemented

✔ Inventory updated automatically

✔ Stock Movement created automatically

✔ Audit Log enabled

✔ Barcode supported

✔ Label printing supported

✔ OpenAPI documented

✔ Unit Tests passed

✔ Integration Tests passed

---

# Deliverables

- Domain Entity
- Repository
- CQRS Commands
- CQRS Queries
- Receipt Service
- Inventory Update Service
- Batch Service
- Stock Movement Service
- DTOs
- Validators
- REST API
- Database Migration
- React Pages
- Receipt Dashboard
- Unit Tests
- Integration Tests
- Swagger Documentation

---

# Cursor Implementation Prompt

Read:

- Cursor_Rules.md
- Business_Rules.md
- Factory_Flow.md
- docs/13_Design/02_Inventory/Goods_Receipt.md
- This TASK document

Implement the complete Goods Receipt module using:

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

- Every Goods Receipt must create Inventory transactions.
- Inventory must never be edited directly.
- Goods Receipt must automatically create Stock Movements.
- Support Purchase Order, Production Order and Manual Receipts.
- Support Batch, Warehouse and Location assignment.
- All operations must be transactional.
- Publish integration events after successful completion.
- Follow project architecture exactly.
- Do not implement unrelated modules.

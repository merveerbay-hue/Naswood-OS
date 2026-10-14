# TASK-031 — Goods Receipt (Purchase Order)

**Module:** Purchasing

**Sprint:** Sprint 02 – Purchasing

**Category:** Warehouse Operations

**Priority:** Critical

**Estimated Effort:** 8 Days

**Status:** Completed

---

# Purpose

Develop the Purchase Order Goods Receipt (GRPO) module for Naswood OS.

The Goods Receipt module records the physical receipt of materials delivered by suppliers against approved Purchase Orders. It validates delivered quantities, updates inventory, initiates quality inspections when required and enables supplier invoice matching.

The Goods Receipt serves as the bridge between Purchasing and Inventory.

---

# Objectives

- Accurate Receiving Process
- Purchase Order Validation
- Inventory Integration
- Batch & Serial Tracking
- Quality Inspection Integration
- Three-Way Matching Support
- Complete Material Traceability

---

# Scope

The Goods Receipt module includes

- Purchase Order Receiving
- Partial Receipt
- Over/Under Delivery Validation
- Batch Registration
- Serial Number Registration
- Warehouse Assignment
- Quality Inspection Trigger
- Inventory Posting
- Receipt Reversal
- Receipt Attachments

Out of Scope

- Purchase Order Creation
- Supplier Invoice
- Inventory Consumption
- Inventory Adjustment

---

# Goods Receipt Architecture

```
Purchase Order

↓

Supplier Delivery

↓

Goods Receipt

↓

Quality Inspection

↓

Inventory

↓

Supplier Invoice

↓

Three-Way Matching
```

---

# Goods Receipt Lifecycle

```
Draft

↓

Receiving

↓

Validated

↓

Posted

↓

Quality Inspection

↓

Completed

or

Reversed
```

Reference

Status_Lifecycle.md

---

# Receipt Sources

Goods Receipts may originate from

- Purchase Order
- Framework Purchase Order
- Service Purchase Order
- Manual Receipt (Authorized Users)

---

# Goods Receipt Header

Each Goods Receipt contains

## General Information

- GR Number
- Receipt Date
- Purchase Order
- Supplier
- Company
- Plant
- Warehouse
- Receiver
- Status

---

## Receipt Lines

Each line contains

- Material Code
- Description
- Ordered Quantity
- Received Quantity
- Remaining Quantity
- Unit
- Batch Number
- Serial Number
- Storage Location
- Inspection Required
- Notes

Reference

Unit_Conversion.md

---

# Receiving Validation

The system validates

- Purchase Order exists
- Purchase Order Released
- Supplier matches Purchase Order
- Material matches Purchase Order
- Warehouse exists
- Quantity tolerance
- Unit of Measure
- Company
- Plant

---

# Partial Receipt

Supports

```
Purchase Order

1000 Pieces

↓

Receipt

400

↓

Remaining

600
```

Remaining quantity remains open.

---

# Multiple Receipts

Supports

```
PO

↓

Receipt 1

↓

Receipt 2

↓

Receipt 3

↓

PO Completed
```

---

# Over Delivery

Supports configurable tolerances.

Example

```
Ordered

1000

Received

1020

Tolerance

5%

↓

Accepted
```

If exceeded

```
Approval Required
```

Reference

Validation_Rules.md

---

# Under Delivery

Supports

- Remaining Open
- Close Remaining
- Backorder

Configured by purchasing policy.

---

# Batch Management

Supports

- Batch Number
- Production Date
- Expiration Date
- Manufacturer Batch
- Internal Batch

Reference

TASK-020_Batch.md

---

# Serial Number Management

Supports

- Machine Components
- Equipment
- Spare Parts
- High Value Assets

Each serial number is individually tracked.

---

# Warehouse Assignment

Supports

- Default Warehouse
- Receiving Warehouse
- Quality Warehouse
- Quarantine Warehouse

Reference

TASK-017_Warehouse.md

---

# Storage Location

Supports

- Rack
- Shelf
- Bin
- Zone

Reference

TASK-018_Location.md

---

# Quality Inspection

Materials requiring inspection

```
Goods Receipt

↓

Quality Hold

↓

Inspection

↓

Accepted

↓

Inventory

or

Rejected

↓

Purchase Return
```

Reference

Quality Module

---

# Inventory Integration

After posting

```
Goods Receipt

↓

Inventory Updated

↓

Available Stock

↓

MRP Updated
```

Reference

Inventory Module

---

# Finance Integration

Supports

- Goods Receipt Value
- GR/IR Posting
- Three-Way Matching

Reference

Finance Module

---

# Attachments

Supports

- Delivery Note
- Packing List
- Supplier Certificate
- Photos
- Transport Documents

Reference

TASK-012_File_Upload.md

---

# Search

Supports

- GR Number
- Purchase Order
- Supplier
- Material
- Warehouse
- Receiver
- Date
- Status

Reference

Search_Filtering.md

---

# Dashboard Information

Displays

- Today's Receipts
- Open Receipts
- Delayed Deliveries
- Partial Receipts
- Inspection Waiting
- Receiving Volume

Reference

TASK-034_Purchasing_Dashboard.md

---

# Reports

Supports

- Goods Receipt Register
- Supplier Delivery Report
- Partial Receipt Report
- Receiving Performance
- Warehouse Receiving Report
- Inspection Pending Report

Reference

TASK-035_Purchasing_Reports.md

---

# API Endpoints

```
GET /api/v1/purchase-goods-receipts

GET /api/v1/purchase-goods-receipts/{id}

POST /api/v1/purchase-goods-receipts

PUT /api/v1/purchase-goods-receipts/{id}

POST /api/v1/purchase-goods-receipts/{id}/post

POST /api/v1/purchase-goods-receipts/{id}/reverse

GET /api/v1/purchase-goods-receipts/search
```

Reference

Purchasing_API.md

---

# Validation Rules

The system validates

- Purchase Order is Released.
- Supplier matches Purchase Order.
- Material exists.
- Quantity > 0.
- Warehouse exists.
- Storage Location exists.
- Batch required when configured.
- Serial numbers are unique.
- Receipt cannot exceed configured tolerance.
- Posted receipts cannot be edited.

Reference

Validation_Rules.md

---

# Security

Supports

- Role-Based Access
- Warehouse Authorization
- Company Isolation
- Plant Isolation
- Purchase Authorization

Reference

Security.md

Permission_Model.md

---

# Audit

Records

- Goods Receipt Created
- Receipt Posted
- Receipt Reversed
- Quantity Changed
- Batch Registered
- Serial Registered
- Warehouse Changed

Reference

Audit_Log.md

Logging.md

---

# Notifications

Supports

- Goods Received
- Partial Delivery
- Over Delivery
- Inspection Required
- Receipt Reversed
- Warehouse Capacity Warning

Reference

Notification_System.md

---

# Events

Publishes

- GoodsReceiptCreated
- GoodsReceiptPosted
- GoodsReceiptReversed
- InventoryUpdated
- QualityInspectionRequested
- PurchaseOrderPartiallyReceived
- PurchaseOrderCompleted

Reference

Event_Model.md

Integration_Events.md

---

# Mobile Support

Supports

- Barcode Receiving
- QR Code Receiving
- Batch Entry
- Serial Number Scan
- Photo Attachment
- Warehouse Selection
- Offline Receiving

Reference

Purchasing_Mobile.md

---

# Performance

Targets

- Goods Receipt Creation < 1 second
- Barcode Scan < 300 ms
- Receipt Posting < 2 seconds
- Inventory Update < 2 seconds
- Support 2,000,000+ receipt transactions

Reference

Performance.md

Caching.md

---

# Naswood Examples

### Example 1

```
Purchase Order

↓

100 m³ Spruce Timber

↓

Supplier Delivery

↓

Goods Receipt

↓

Quality Moisture Inspection

↓

Warehouse Stock
```

---

### Example 2

```
PUR Adhesive

↓

Batch Number Recorded

↓

Expiration Date Stored

↓

Inventory Updated
```

---

### Example 3

```
CNC Machine Spare Parts

↓

Serial Numbers Scanned

↓

Warehouse Location Assigned

↓

Asset Traceability Enabled
```

---

# Acceptance Criteria

The Goods Receipt module shall

- Receive materials against released Purchase Orders.
- Support partial, complete and multiple receipts.
- Validate receiving tolerances.
- Support batch and serial number tracking.
- Integrate with Inventory and Quality.
- Support receipt reversal.
- Publish inventory and procurement events.
- Follow all shared platform standards.

---

# Dependencies

Depends On

- TASK-017_Warehouse.md
- TASK-018_Location.md
- TASK-020_Batch.md
- TASK-026_Supplier.md
- TASK-030_Purchase_Order.md
- TASK-012_File_Upload.md
- Purchasing_Workflow.md
- Validation_Rules.md

---

# Related Documents

Purchasing_Architecture.md

Purchasing_API.md

Purchasing_Workflow.md

TASK-017_Warehouse.md

TASK-018_Location.md

TASK-020_Batch.md

TASK-026_Supplier.md

TASK-030_Purchase_Order.md

TASK-032_Purchase_Return.md

TASK-033_Supplier_Invoice.md

TASK-034_Purchasing_Dashboard.md

TASK-035_Purchasing_Reports.md

Security.md

Permission_Model.md

Validation_Rules.md

Performance.md

Caching.md

Search_Filtering.md

Unit_Conversion.md

Audit_Log.md

Logging.md

Notification_System.md

Event_Model.md

Integration_Events.md

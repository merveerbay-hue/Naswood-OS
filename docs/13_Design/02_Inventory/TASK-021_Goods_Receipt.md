> **UX authority:** Live receiving UI is [`INV_Receiving_Workbench.md`](../../00_Product/Process_Screens/INV_Receiving_Workbench.md) — full truck-to-post Workbench (not CRUD). Spine: Depo select + lot by material category (`INV_Receiving_Wizard.md`). This TASK is historical.

# TASK-021 — Goods Receipt

**Module:** Inventory

**Category:** Transaction

**Version:** 1.0

**Status:** Approved

---

# Purpose

The Goods Receipt transaction records the physical receipt of materials into inventory.

It increases inventory quantities, creates immutable inventory transactions, updates warehouse balances and provides complete traceability from supplier or production source to warehouse storage.

Goods Receipt is the official entry point for inventory into Naswood OS.

---

# Objectives

- Accurate Inventory Receiving
- Real-Time Stock Updates
- Complete Material Traceability
- Warehouse Visibility
- Quality Integration
- Production Integration
- Financial Integration

---

# Scope

Goods Receipt supports inventory receipts from

- Purchase Orders
- Production Orders
- Subcontractors
- Customer Returns
- Internal Transfers
- Manual Adjustments (Authorized)
- Inventory Corrections

Goods Receipt does NOT

- Approve Purchase Orders
- Create Material Definition (engineering track — Definition Designer)
- Calculate Inventory Cost
- Process Supplier Invoices

---

# Business Rules

- Every Goods Receipt generates one or more inventory transactions.
- Inventory increases only after successful posting.
- Goods Receipt cannot be edited after posting.
- Corrections require reversal or adjustment transactions.
- Batch-controlled materials require batch assignment.
- Serial-controlled materials require serial registration.
- Quality inspection may be mandatory before inventory becomes available.

---

# Transaction Lifecycle

```
Draft

↓

Validated

↓

Posted

↓

Completed

↓

Archived
```

Only **Posted** transactions update inventory.

Reference

Status_Lifecycle.md

---

# Receipt Sources

Supported sources

| Source | Description |
|----------|-------------|
| Purchase Order | Supplier delivery |
| Production Order | Finished production |
| Stock Transfer | Internal transfer receipt |
| Customer Return | Returned products |
| Subcontractor | Outsourced production |
| Inventory Adjustment | Authorized correction |

---

# Receiving Workflow

```
Receive Delivery

↓

Select Source Document

↓

Validate Material

↓

Scan Barcode

↓

Assign Batch / Serial

↓

Assign Warehouse

↓

Assign Location

↓

Quality Inspection (Optional)

↓

Post Goods Receipt

↓

Inventory Updated

↓

Events Published
```

---

# Inventory Impact

Goods Receipt

- Increases On Hand Quantity
- Updates Available Quantity
- Creates Inventory Transaction
- Updates Batch Inventory
- Updates Warehouse Inventory
- Updates Location Inventory

Reference

TASK-019_Inventory.md

---

# Warehouse Integration

Goods Receipt requires

- Warehouse
- Storage Location
- Receiving Area
- Putaway Location

Reference

TASK-017_Warehouse.md

TASK-018_Location.md

---

# Batch Handling

If material is batch-controlled

Required

- Batch Number
- Batch Status
- Production Date
- Supplier Batch (Optional)

Reference

TASK-020_Batch.md

---

# Serial Number Handling

For serialized materials

Required

- Unique Serial Number
- Validation
- Duplicate Check

---

# Quality Integration

If Quality Inspection is enabled

Workflow

```
Goods Receipt

↓

Quality Hold

↓

Inspection

↓

Released

↓

Available Inventory
```

Inventory remains unavailable until released.

Reference

06_Quality

---

# Putaway

Supports

- Automatic Putaway
- Manual Putaway
- AI Recommended Location

Reference

Inventory_Mobile.md

---

# Barcode Support

Supports

- Material Barcode
- Batch Barcode
- GS1 Barcode
- QR Code

Scanning automatically retrieves

- Material
- Batch
- Warehouse
- Default Location

Reference

Barcode_Strategy.md

QRCode_Strategy.md

---

# Mobile Workflow

Warehouse Operator

```
Login

↓

Scan Purchase Order

↓

Scan Material

↓

Scan Batch

↓

Enter Quantity

↓

Select Location

↓

Confirm

↓

Receipt Posted
```

Reference

Inventory_Mobile.md

---

# Validation Rules

Before posting

The system validates

- Source document exists
- Material exists
- Warehouse exists
- Location exists
- Quantity > 0
- Batch requirement
- Serial requirement
- Warehouse status
- Location status
- Duplicate serial numbers

Reference

Validation_Rules.md

---

# Inventory Transaction

Posting creates

- Inventory Ledger Entry
- Warehouse Movement
- Batch Movement
- Stock Balance Update

Transactions are immutable.

Reference

TASK-019_Inventory.md

---

# Events

Publishing

- GoodsReceived
- InventoryUpdated
- BatchCreated
- BatchUpdated
- MaterialReceived

Reference

Event_Model.md

Integration_Events.md

---

# Notifications

Supports

- Receipt Completed
- Quality Inspection Required
- Warehouse Capacity Warning
- Receiving Error
- Batch Validation Error

Reference

Notification_System.md

---

# Dashboard

Goods Receipt contributes to

- Today's Receipts
- Incoming Inventory
- Warehouse Activity
- Receiving Performance
- Inventory Trend

Reference

Inventory_Dashboard.md

---

# Reports

Included in

- Goods Receipt Report
- Stock Card
- Inventory Movement Report
- Warehouse Activity Report
- Batch Traceability

Reference

Inventory_Reports.md

---

# API

Primary endpoints

```
GET /goods-receipts

GET /goods-receipts/{id}

POST /goods-receipts

POST /goods-receipts/{id}/post

POST /goods-receipts/{id}/cancel

GET /goods-receipts/{id}/history
```

Reference

Inventory_API.md

---

# Permissions

Typical permissions

- View Goods Receipt
- Create Goods Receipt
- Post Goods Receipt
- Reverse Goods Receipt
- View Inventory
- Receive by Warehouse

Reference

Permission_Model.md

---

# Audit

The following actions are audited

- Goods Receipt Created
- Goods Receipt Posted
- Goods Receipt Reversed
- Quantity Changed Before Posting
- Warehouse Changed
- Batch Assigned
- User Actions

Reference

Audit_Log.md

---

# Performance

The system shall

- Post receipts in less than 2 seconds.
- Support bulk receiving.
- Support concurrent warehouse operators.
- Optimize barcode scanning.
- Update inventory in real time.

Reference

Performance.md

Caching.md

Concurrency.md

---

# Security

Goods Receipt follows

- Role-Based Authorization
- Warehouse Authorization
- Secure API Access
- Complete Audit Logging

Reference

Security.md

Permission_Model.md

---

# Naswood Implementation

Typical receiving scenarios

## Supplier Receipt

Supplier

↓

Raw Timber

↓

RAW Warehouse

↓

Quality Inspection

↓

Available Inventory

---

## Production Receipt

Production Line

↓

Finished Product

↓

FG Warehouse

↓

Shipment Ready

---

## Thermowood Receipt

Kiln Exit

↓

Thermowood Batch

↓

THW Warehouse

↓

Moisture Verification

↓

Available Inventory

---

## CLT / Panel Receipt

Production

↓

Finished Panel

↓

Finished Goods Warehouse

↓

Packing

↓

Shipment

---

# Acceptance Criteria

The Goods Receipt module shall

- Support multiple receipt sources.
- Increase inventory automatically.
- Support batch and serial tracking.
- Integrate with warehouse and quality.
- Publish inventory events.
- Support barcode and mobile workflows.
- Prevent unauthorized inventory changes.
- Follow all shared platform standards.

---

# Related Documents

Inventory_Architecture.md

Inventory_Dashboard.md

Inventory_API.md

Inventory_Mobile.md

Inventory_Reports.md

TASK-016_Material.md

TASK-017_Warehouse.md

TASK-018_Location.md

TASK-019_Inventory.md

TASK-020_Batch.md

TASK-022_Goods_Issue.md

TASK-023_Stock_Transfer.md

Permission_Model.md

Validation_Rules.md

Barcode_Strategy.md

QRCode_Strategy.md

Performance.md

Caching.md

Concurrency.md

Security.md

Audit_Log.md

Status_Lifecycle.md

Notification_System.md

AI_Copilot.md

Event_Model.md

Integration_Events.md

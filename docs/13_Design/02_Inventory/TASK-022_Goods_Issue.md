# TASK-022 — Goods Issue

**Module:** Inventory

**Category:** Transaction

**Version:** 1.0

**Status:** Approved

---

# Purpose

The Goods Issue transaction records the physical removal of inventory from a warehouse or storage location.

It decreases inventory quantities, creates immutable inventory transactions, updates warehouse balances and maintains complete traceability of material consumption and shipment activities.

Goods Issue is the official exit point for inventory within Naswood OS.

---

# Objectives

- Accurate Inventory Consumption
- Real-Time Stock Updates
- Complete Material Traceability
- Production Material Supply
- Shipment Processing
- Inventory Accuracy
- Financial Integration

---

# Scope

Goods Issue supports inventory consumption for

- Production Orders
- Sales Orders
- Maintenance Work Orders
- Internal Consumption
- Sample Requests
- Scrap Disposal
- Customer Replacement
- Inventory Adjustment
- Manual Authorized Issue

Goods Issue does NOT

- Create Sales Orders
- Close Production Orders
- Generate Financial Documents
- Approve Inventory Adjustments

---

# Business Rules

- Every Goods Issue creates one or more inventory transactions.
- Inventory decreases only after successful posting.
- Posted Goods Issues cannot be edited.
- Corrections require reversal or adjustment transactions.
- Batch-controlled materials require batch selection.
- Serial-controlled materials require serial validation.
- Available inventory must be sufficient unless negative stock is permitted.

---

# Transaction Lifecycle

```
Draft

↓

Validated

↓

Reserved (Optional)

↓

Posted

↓

Completed

↓

Archived
```

Only **Posted** transactions reduce inventory.

Reference

Status_Lifecycle.md

---

# Issue Sources

Supported issue sources

| Source | Description |
|----------|-------------|
| Production Order | Material consumption |
| Sales Order | Customer shipment |
| Maintenance Order | Spare part usage |
| Internal Request | Department consumption |
| Scrap Disposal | Material disposal |
| Inventory Adjustment | Authorized correction |
| Sample Request | Product sample issue |

---

# Goods Issue Workflow

```
Select Source Document

↓

Validate Material

↓

Validate Available Stock

↓

Scan Barcode

↓

Select Batch / Serial

↓

Select Warehouse

↓

Select Location

↓

Confirm Quantity

↓

Post Goods Issue

↓

Inventory Updated

↓

Events Published
```

---

# Inventory Impact

Goods Issue

- Decreases On Hand Quantity
- Decreases Available Quantity
- Updates Reserved Quantity (if applicable)
- Creates Inventory Transaction
- Updates Batch Inventory
- Updates Warehouse Inventory
- Updates Location Inventory

Reference

TASK-019_Inventory.md

---

# Warehouse Integration

Goods Issue requires

- Source Warehouse
- Source Location
- Material Availability
- Warehouse Authorization

Reference

TASK-017_Warehouse.md

TASK-018_Location.md

---

# Batch Handling

For batch-controlled materials

Required

- Batch Selection
- Batch Availability Validation
- Batch Status Validation

Only available batches may be issued.

Reference

TASK-020_Batch.md

---

# Serial Number Handling

For serialized materials

Required

- Serial Number Selection
- Ownership Validation
- Duplicate Prevention
- Traceability Recording

---

# Reservation Integration

If inventory is reserved

Workflow

```
Reservation

↓

Goods Issue

↓

Reservation Released

↓

Inventory Updated
```

Reserved inventory is consumed before available inventory.

Reference

TASK-019_Inventory.md

---

# Production Integration

Production consumption

```
Production Order

↓

Material Issue

↓

Work Center

↓

Production Consumption

↓

Inventory Updated
```

Reference

05_Production

---

# Sales Integration

Shipment workflow

```
Sales Order

↓

Picking

↓

Goods Issue

↓

Shipment

↓

Inventory Updated
```

Reference

04_Sales

---

# Maintenance Integration

Maintenance workflow

```
Maintenance Order

↓

Spare Part Issue

↓

Maintenance Completed
```

Reference

07_Maintenance

---

# Barcode Support

Supports

- Material Barcode
- Batch Barcode
- Serial Barcode
- GS1 Barcode
- QR Code

Scanning retrieves

- Material
- Available Quantity
- Batch
- Warehouse
- Location

Reference

Barcode_Strategy.md

QRCode_Strategy.md

---

# Mobile Workflow

Warehouse Operator

```
Login

↓

Scan Work Order

↓

Scan Material

↓

Scan Batch / Serial

↓

Confirm Quantity

↓

Post Issue

↓

Inventory Updated
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
- Available quantity is sufficient
- Batch requirement
- Serial requirement
- Reservation validity
- Warehouse status
- Location status
- User permissions

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

# Negative Stock

Behavior depends on company policy.

Supported options

- Not Allowed
- Allowed with Authorization
- Allowed by Warehouse Policy

Reference

Negative_Stock.md

---

# Events

Publishing

- GoodsIssued
- InventoryUpdated
- BatchConsumed
- ReservationReleased
- MaterialIssued

Reference

Event_Model.md

Integration_Events.md

---

# Notifications

Supports

- Goods Issue Completed
- Low Stock Warning
- Negative Stock Alert
- Reservation Failure
- Batch Validation Error
- Authorization Failure

Reference

Notification_System.md

---

# Dashboard

Goods Issue contributes to

- Today's Issues
- Outgoing Inventory
- Material Consumption
- Warehouse Activity
- Inventory Trend
- Production Consumption

Reference

Inventory_Dashboard.md

---

# Reports

Included in

- Goods Issue Report
- Stock Card
- Inventory Movement Report
- Production Consumption Report
- Shipment Report
- Batch Traceability

Reference

Inventory_Reports.md

---

# API

Primary endpoints

```
GET /goods-issues

GET /goods-issues/{id}

POST /goods-issues

POST /goods-issues/{id}/post

POST /goods-issues/{id}/reverse

GET /goods-issues/{id}/history
```

Reference

Inventory_API.md

---

# Permissions

Typical permissions

- View Goods Issue
- Create Goods Issue
- Post Goods Issue
- Reverse Goods Issue
- Issue from Warehouse
- Override Negative Stock

Reference

Permission_Model.md

---

# Audit

The following actions are audited

- Goods Issue Created
- Goods Issue Posted
- Goods Issue Reversed
- Quantity Changed Before Posting
- Batch Selected
- Serial Assigned
- User Actions

Reference

Audit_Log.md

---

# Performance

The system shall

- Post Goods Issues in less than 2 seconds.
- Support high-volume warehouse operations.
- Validate stock in real time.
- Support concurrent warehouse operators.
- Optimize barcode scanning performance.

Reference

Performance.md

Caching.md

Concurrency.md

---

# Security

Goods Issue follows

- Role-Based Authorization
- Warehouse Authorization
- Secure API Access
- Complete Audit Logging

Reference

Security.md

Permission_Model.md

---

# Naswood Implementation

Typical issue scenarios

## Production Consumption

```
RAW Warehouse

↓

Finger Joint Line

↓

Production Consumption

↓

Inventory Reduced
```

---

## Thermowood Production

```
Dry Lumber

↓

Thermowood Kiln

↓

Production Consumption

↓

Thermowood Process
```

---

## Finished Goods Shipment

```
FG Warehouse

↓

Picking

↓

Goods Issue

↓

Truck Loading

↓

Customer Shipment
```

---

## Maintenance Consumption

```
MRO Warehouse

↓

Maintenance Work Order

↓

Spare Part Issue

↓

Equipment Repair
```

---

# Acceptance Criteria

The Goods Issue module shall

- Support multiple issue sources.
- Reduce inventory automatically.
- Validate available stock.
- Support batch and serial tracking.
- Integrate with Production, Sales and Maintenance.
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

TASK-021_Goods_Receipt.md

TASK-023_Stock_Transfer.md

TASK-024_Inventory_Count.md

TASK-025_Inventory_Adjustment.md

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

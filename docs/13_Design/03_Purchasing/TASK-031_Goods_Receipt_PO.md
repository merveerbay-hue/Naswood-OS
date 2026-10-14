# TASK-031 — Goods Receipt (Purchase Order)

**Module:** Purchasing

**Category:** Transaction

**Version:** 1.0

**Status:** Approved

---

# Purpose

The Goods Receipt (Purchase Order) transaction records the physical receipt of materials delivered by a supplier against an approved Purchase Order.

It verifies delivered quantities, updates procurement status and initiates inventory receiving through the Inventory module.

Goods Receipt confirms that ordered materials have been physically received and accepted according to purchasing, warehouse and quality policies.

---

# Objectives

- Verify Supplier Deliveries
- Control Purchase Order Fulfillment
- Support Inventory Receiving
- Enable Three-Way Matching
- Improve Supplier Performance Measurement
- Ensure Complete Procurement Traceability

---

# Scope

Goods Receipt (PO) supports

- Standard Purchase Orders
- Partial Deliveries
- Multiple Deliveries
- Service Confirmation
- Batch Receiving
- Serial Number Receiving
- Quality Inspection
- Warehouse Receiving

Goods Receipt does NOT

- Update Inventory Directly
- Create Purchase Orders
- Process Supplier Invoices
- Execute Supplier Payments

Inventory updates are handled by the Inventory module.

---

# Business Rules

- Every Goods Receipt references one Purchase Order.
- One Purchase Order may have multiple Goods Receipts.
- Delivered quantity cannot exceed ordered quantity unless over-delivery tolerance permits.
- Posted Goods Receipts cannot be modified.
- Reversals require a Return or Reversal transaction.
- Every receipt is fully auditable.

---

# Goods Receipt Lifecycle

```
Draft

↓

Receiving

↓

Validation

↓

Quality Inspection (Optional)

↓

Posted

↓

Inventory Receipt

↓

Completed
```

Reference

Status_Lifecycle.md

---

# Receipt Sources

Supported sources

| Source | Description |
|---------|-------------|
| Purchase Order | Standard procurement |
| Blanket Purchase Order | Scheduled procurement |
| Framework Agreement | Contract procurement |
| Service Purchase Order | Service confirmation |

---

# Goods Receipt Header

Each Goods Receipt contains

- Receipt Number
- Purchase Order
- Supplier
- Company
- Plant
- Warehouse
- Receipt Date
- Delivery Note
- Status
- Receiver

---

# Receipt Lines

Each receipt line contains

- Material
- Ordered Quantity
- Received Quantity
- Remaining Quantity
- Unit
- Batch Number
- Serial Number
- Warehouse
- Storage Location
- Remarks

Reference

Measurement_System.md

---

# Receiving Workflow

```
Purchase Order

↓

Supplier Delivery

↓

Warehouse Receiving

↓

Barcode Scan

↓

Quantity Verification

↓

Batch / Serial Registration

↓

Quality Inspection (Optional)

↓

Post Goods Receipt

↓

Inventory Goods Receipt

↓

Purchase Order Updated
```

---

# Partial Delivery

Supports

```
PO Quantity

100

↓

Receipt 1

40

↓

Receipt 2

30

↓

Receipt 3

30

↓

Purchase Order Complete
```

The Purchase Order remains open until all quantities are received.

---

# Over Delivery

Supports configurable tolerances

Example

```
Ordered

100

Received

102

Tolerance

5%

↓

Accepted
```

If tolerance is exceeded

- Approval Required
- Receipt Blocked
- Supplier Notification

---

# Under Delivery

Supports

- Remaining Quantity Tracking
- Backorder Management
- Supplier Follow-Up

Purchase Order remains partially open.

---

# Batch Receiving

Batch-controlled materials require

- Batch Number
- Supplier Batch
- Production Date
- Expiration Date (Optional)

Reference

TASK-020_Batch.md

---

# Serial Number Receiving

Serialized materials require

- Serial Registration
- Duplicate Validation
- Ownership Assignment

Reference

TASK-019_Inventory.md

---

# Warehouse Integration

After posting

```
Purchasing

↓

Inventory Goods Receipt

↓

Warehouse

↓

Storage Location

↓

Inventory Updated
```

Reference

02_Inventory

---

# Quality Integration

If inspection is required

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

Rejected materials may generate

- Purchase Return
- NCR
- Supplier Complaint

Reference

06_Quality

---

# Purchase Order Update

Posting updates

- Received Quantity
- Remaining Quantity
- Delivery Status
- Purchase Order Status

Possible statuses

- Open
- Partially Received
- Fully Received
- Closed

Reference

TASK-030_Purchase_Order.md

---

# Three-Way Matching

Goods Receipt participates in

```
Purchase Order

↓

Goods Receipt

↓

Supplier Invoice
```

Matching validates

- Quantity
- Supplier
- Material
- Price

Reference

TASK-033_Supplier_Invoice.md

---

# Supplier Performance

Receipt contributes to supplier KPIs

- On-Time Delivery
- Quantity Accuracy
- Delivery Completeness
- Packaging Quality
- Documentation Accuracy

Reference

TASK-026_Supplier.md

---

# Barcode Support

Supports

- Material Barcode
- GS1 Barcode
- QR Code
- Batch Barcode
- Serial Barcode

Reference

Barcode_Strategy.md

QRCode_Strategy.md

---

# Mobile Workflow

```
Open Purchase Order

↓

Scan Material

↓

Scan Batch

↓

Enter Quantity

↓

Assign Warehouse

↓

Confirm Receipt

↓

Inventory Updated
```

Reference

Purchasing_Mobile.md

Inventory_Mobile.md

---

# Validation Rules

The system validates

- Purchase Order exists.
- Supplier matches Purchase Order.
- Material exists.
- Warehouse exists.
- Storage Location exists.
- Quantity is positive.
- Over-delivery tolerance.
- Batch requirement.
- Serial requirement.
- User permissions.

Reference

Validation_Rules.md

---

# Dashboard

Goods Receipt contributes to

- Today's Receipts
- Pending Deliveries
- Supplier Delivery Performance
- Warehouse Receiving
- Purchase Order Fulfillment

Reference

TASK-034_Purchasing_Dashboard.md

---

# Reports

Included in

- Goods Receipt Report
- Purchase Order Status
- Supplier Delivery Report
- Receiving Performance
- Warehouse Receiving Report
- Procurement KPI Report

Reference

TASK-035_Purchasing_Reports.md

---

# API

Primary endpoints

```
GET /purchase-goods-receipts

GET /purchase-goods-receipts/{id}

POST /purchase-goods-receipts

POST /purchase-goods-receipts/{id}/post

POST /purchase-goods-receipts/{id}/reverse

GET /purchase-goods-receipts/{id}/history
```

Reference

Purchasing_API.md

---

# Events

Publishing

- PurchaseGoodsReceiptCreated
- PurchaseGoodsReceiptPosted
- PurchaseGoodsReceiptReversed
- PurchaseOrderUpdated
- SupplierDeliveryCompleted
- InventoryReceiptRequested

Reference

Event_Model.md

Integration_Events.md

---

# Notifications

Supports

- Goods Receipt Posted
- Partial Delivery
- Delivery Delay
- Quality Inspection Required
- Over Delivery Approval
- Supplier Delivery Completed

Reference

Notification_System.md

---

# Permissions

Typical permissions

- View Goods Receipt
- **Receive goods** → Inventory Receiving Workbench (`INV_Receiving_Workbench.md`) — not Create GR form
- Post Goods Receipt
- Reverse Goods Receipt
- Receive Purchase Orders
- Manage Warehouse Assignment

Reference

Permission_Model.md

---

# Audit

The following actions are audited

- Goods Receipt Created
- Goods Receipt Posted
- Quantity Modified
- Batch Registered
- Serial Registered
- Warehouse Changed
- User Actions

Reference

Audit_Log.md

---

# Performance

The system shall

- Post receipts in less than 2 seconds.
- Support concurrent warehouse receiving.
- Process barcode scans in real time.
- Support bulk receiving.
- Synchronize with Inventory immediately.

Reference

Performance.md

Caching.md

Concurrency.md

---

# Security

Goods Receipt follows

- Role-Based Authorization
- Warehouse Authorization
- Purchasing Authorization
- Secure API Access
- Complete Audit Logging

Reference

Security.md

Permission_Model.md

---

# AI Integration

AI may assist with

- Delivery Delay Prediction
- Supplier Performance Analysis
- Receiving Time Optimization
- Duplicate Delivery Detection
- Quantity Anomaly Detection
- Warehouse Putaway Recommendation

Reference

AI_Copilot.md

---

# Naswood Implementation

Typical receiving scenarios

## Raw Timber

```
Supplier

↓

Purchase Order

↓

Log Yard

↓

Quality Inspection

↓

RAW Warehouse
```

---

## Chemicals

```
Supplier

↓

Receiving

↓

Batch Registration

↓

Quality Hold

↓

Production Warehouse
```

---

## Machinery

```
Supplier

↓

Factory Acceptance

↓

Goods Receipt

↓

Asset Registration

↓

Maintenance
```

---

## Packaging Materials

```
Supplier

↓

Goods Receipt

↓

FG Warehouse

↓

Production Consumption
```

---

# Acceptance Criteria

The Goods Receipt (Purchase Order) module shall

- Support complete and partial deliveries.
- Validate Purchase Orders before receipt.
- Support batch and serial-controlled materials.
- Integrate with Inventory and Quality.
- Support three-way matching.
- Track supplier delivery performance.
- Publish procurement events.
- Follow all shared platform standards.

---

# Related Documents

Purchasing_Architecture.md

Purchasing_API.md

Purchasing_Mobile.md

TASK-026_Supplier.md

TASK-027_Purchase_Request.md

TASK-028_RFQ.md

TASK-029_Supplier_Quotation.md

TASK-030_Purchase_Order.md

TASK-032_Purchase_Return.md

TASK-033_Supplier_Invoice.md

TASK-034_Purchasing_Dashboard.md

TASK-035_Purchasing_Reports.md

Approval_Workflow.md

Permission_Model.md

Validation_Rules.md

Measurement_System.md

Barcode_Strategy.md

QRCode_Strategy.md

Performance.md

Caching.md

Concurrency.md

Security.md

Audit_Log.md

Notification_System.md

AI_Copilot.md

Event_Model.md

Integration_Events.md

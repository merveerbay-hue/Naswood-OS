# TASK-032 — Purchase Return

**Module:** Purchasing

**Sprint:** Sprint 02 – Purchasing

**Category:** Procurement

**Priority:** High

**Estimated Effort:** 7 Days

**Status:** Planned

---

# Purpose

Develop the Purchase Return module for Naswood OS.

The Purchase Return module manages the return of materials to suppliers due to quality issues, incorrect deliveries, over deliveries, damaged goods or commercial agreements.

It ensures complete traceability between Goods Receipt, Quality Inspection, Inventory, Finance and Supplier Credit Notes.

---

# Objectives

- Standardize Supplier Returns
- Complete Return Traceability
- Inventory Adjustment
- Quality Integration
- Supplier Credit Tracking
- Financial Reconciliation
- Digital Approval Workflow

---

# Scope

The Purchase Return module includes

- Purchase Return Creation
- Return Authorization
- Quality-Based Returns
- Quantity Returns
- Partial Returns
- Return Shipment
- Supplier Credit Tracking
- Inventory Update
- Return Closure
- Return Documentation

Out of Scope

- Purchase Orders
- Goods Receipt
- Supplier Invoice Posting
- Supplier Payment

---

# Purchase Return Architecture

```
Goods Receipt

↓

Quality Inspection

↓

Purchase Return

↓

Supplier

↓

Credit Note

↓

Finance

↓

Closed
```

---

# Purchase Return Lifecycle

```
Draft

↓

Submitted

↓

Under Review

↓

Approved

↓

Ready for Shipment

↓

Returned

↓

Supplier Confirmed

↓

Credit Received

↓

Closed

or

Cancelled
```

Reference

Status_Lifecycle.md

---

# Return Reasons

Supports

- Damaged Material
- Incorrect Material
- Incorrect Quantity
- Failed Quality Inspection
- Expired Material
- Supplier Recall
- Packaging Damage
- Transport Damage
- Commercial Return

---

# Return Sources

Purchase Returns may originate from

- Goods Receipt
- Quality Inspection
- Warehouse Inspection
- Manual Return
- Supplier Recall

---

# Return Header

Each Purchase Return contains

## General Information

- Return Number
- Return Date
- Supplier
- Purchase Order
- Goods Receipt
- Company
- Plant
- Warehouse
- Return Reason
- Status

---

## Return Lines

Each line contains

- Material Code
- Description
- Returned Quantity
- Unit
- Batch Number
- Serial Number
- Return Reason
- Quality Decision
- Credit Expected
- Notes

Reference

Unit_Conversion.md

---

# Return Validation

The system validates

- Goods Receipt exists.
- Supplier matches Goods Receipt.
- Material exists.
- Return quantity does not exceed received quantity.
- Warehouse exists.
- Return reason is provided.

---

# Quality Integration

Quality-based returns follow

```
Goods Receipt

↓

Inspection

↓

Rejected

↓

Purchase Return

↓

Supplier
```

Inspection reports remain linked to the return.

Reference

Quality Module

---

# Inventory Integration

After approval

```
Purchase Return

↓

Inventory Reduced

↓

Stock Updated

↓

Availability Updated
```

Returned stock is removed from available inventory.

Reference

Inventory Module

---

# Batch & Serial Tracking

Supports

- Batch Traceability
- Serial Number Traceability
- Manufacturing Date
- Expiration Date

Reference

TASK-020_Batch.md

---

# Partial Returns

Supports

```
Received

1000

↓

Return

200

↓

Remaining

800
```

Multiple return transactions are supported.

---

# Supplier Confirmation

Supplier may

- Accept Return
- Reject Return
- Request Inspection
- Issue Credit Note
- Send Replacement

---

# Credit Note

Supports

- Full Credit
- Partial Credit
- Replacement Material
- Commercial Discount

Reference

Finance Module

---

# Replacement Material

Optional workflow

```
Purchase Return

↓

Supplier Replacement

↓

Goods Receipt

↓

Inventory
```

---

# Attachments

Supports

- Inspection Report
- Photos
- Delivery Documents
- Supplier Communication
- Credit Note
- Return Authorization

Reference

TASK-012_File_Upload.md

---

# Search

Supports

- Return Number
- Supplier
- Purchase Order
- Goods Receipt
- Material
- Return Reason
- Status
- Date

Reference

Search_Filtering.md

---

# Dashboard Information

Displays

- Open Returns
- Returned Value
- Supplier Return Rate
- Pending Credit Notes
- Return Reasons
- Return Trend

Reference

TASK-034_Purchasing_Dashboard.md

---

# Reports

Supports

- Purchase Return Register
- Returns by Supplier
- Returns by Material
- Quality Returns
- Credit Note Status
- Return Cost Analysis

Reference

TASK-035_Purchasing_Reports.md

---

# API Endpoints

```
GET /api/v1/purchase-returns

GET /api/v1/purchase-returns/{id}

POST /api/v1/purchase-returns

PUT /api/v1/purchase-returns/{id}

DELETE /api/v1/purchase-returns/{id}

POST /api/v1/purchase-returns/{id}/submit

POST /api/v1/purchase-returns/{id}/approve

POST /api/v1/purchase-returns/{id}/ship

POST /api/v1/purchase-returns/{id}/close

POST /api/v1/purchase-returns/{id}/cancel
```

Reference

Purchasing_API.md

---

# Validation Rules

The system validates

- Supplier is Active.
- Goods Receipt exists.
- Material exists.
- Return Quantity > 0.
- Return Quantity ≤ Received Quantity.
- Warehouse exists.
- Batch Number is required when enabled.
- Serial Number is unique.
- Approved returns cannot be edited.
- Closed returns are read-only.

Reference

Validation_Rules.md

---

# Security

Supports

- Role-Based Access
- Warehouse Authorization
- Purchasing Authorization
- Company Isolation
- Plant Isolation

Reference

Security.md

Permission_Model.md

---

# Audit

Records

- Purchase Return Created
- Updated
- Submitted
- Approved
- Shipped
- Supplier Confirmed
- Credit Received
- Closed
- Cancelled

Reference

Audit_Log.md

Logging.md

---

# Notifications

Supports

- Return Approval Required
- Return Approved
- Return Shipped
- Supplier Response Received
- Credit Note Received
- Return Closed

Reference

Notification_System.md

---

# Events

Publishes

- PurchaseReturnCreated
- PurchaseReturnApproved
- PurchaseReturnShipped
- PurchaseReturnClosed
- SupplierCreditReceived
- InventoryAdjusted

Reference

Event_Model.md

Integration_Events.md

---

# Mobile Support

Supports

- Create Return
- Barcode Scan
- Batch Scan
- Serial Scan
- Photo Upload
- View Return Status
- Supplier Contact

Reference

Purchasing_Mobile.md

---

# Performance

Targets

- Return Creation < 1 second
- Return Search < 300 ms
- Inventory Update < 2 seconds
- Credit Status Update < 500 ms
- Support 500,000+ return transactions

Reference

Performance.md

Caching.md

---

# Naswood Examples

### Example 1

```
Goods Receipt

↓

Spruce Timber

↓

Moisture Above Specification

↓

Quality Reject

↓

Purchase Return

↓

Supplier Credit Note
```

---

### Example 2

```
PUR Adhesive

↓

Expired Batch

↓

Warehouse Inspection

↓

Purchase Return

↓

Replacement Delivery
```

---

### Example 3

```
Machine Bearing

↓

Damaged During Transport

↓

Photo Attached

↓

Supplier Approved Return

↓

Replacement Sent
```

---

# Acceptance Criteria

The Purchase Return module shall

- Support returns against Goods Receipts.
- Support quality-based and commercial returns.
- Support partial returns and replacement workflows.
- Integrate with Inventory, Quality and Finance.
- Track supplier credit notes.
- Maintain complete return history.
- Publish procurement lifecycle events.
- Follow all shared platform standards.

---

# Dependencies

Depends On

- TASK-020_Batch.md
- TASK-026_Supplier.md
- TASK-030_Purchase_Order.md
- TASK-031_Goods_Receipt_PO.md
- TASK-012_File_Upload.md
- Purchasing_Workflow.md
- Validation_Rules.md

---

# Related Documents

Purchasing_Architecture.md

Purchasing_API.md

Purchasing_Workflow.md

TASK-020_Batch.md

TASK-026_Supplier.md

TASK-030_Purchase_Order.md

TASK-031_Goods_Receipt_PO.md

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

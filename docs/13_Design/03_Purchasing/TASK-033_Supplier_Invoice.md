# TASK-032 — Purchase Return

**Module:** Purchasing

**Category:** Transaction

**Version:** 1.0

**Status:** Approved

---

# Purpose

The Purchase Return transaction manages the return of materials, products or services received from suppliers that do not meet purchasing, quality or operational requirements.

Purchase Returns ensure that rejected materials are removed from inventory, suppliers are notified, financial corrections are initiated and complete traceability is maintained throughout the procurement lifecycle.

Purchase Returns are directly linked to Goods Receipts and Purchase Orders.

---

# Objectives

- Standardize Supplier Returns
- Improve Supplier Quality
- Protect Inventory Accuracy
- Support Financial Corrections
- Maintain Complete Traceability
- Support Regulatory Compliance
- Improve Supplier Performance

---

# Scope

Purchase Return supports

- Damaged Materials
- Quality Rejections
- Incorrect Deliveries
- Excess Deliveries
- Wrong Materials
- Supplier Recall
- Warranty Returns
- Service Returns
- Return to Supplier (RTS)

Purchase Return does NOT

- Modify Purchase Orders
- Reverse Supplier Payments
- Create Inventory Adjustments
- Process Credit Notes

These processes are handled by Inventory and Finance modules.

---

# Business Rules

- Every Purchase Return references a Goods Receipt.
- Every Purchase Return references a Supplier.
- One Goods Receipt may have multiple Purchase Returns.
- Return quantity cannot exceed received quantity.
- Posted Purchase Returns are immutable.
- Financial correction requires Supplier Credit Note.
- All return transactions are fully auditable.

---

# Purchase Return Lifecycle

```
Draft

↓

Submitted

↓

Approval

↓

Supplier Notification

↓

Return Shipment

↓

Supplier Confirmation

↓

Credit Note

↓

Completed

↓

Closed
```

Reference

Status_Lifecycle.md

Approval_Workflow.md

---

# Return Types

| Type | Description |
|-------|-------------|
| Damaged Material | Physical damage |
| Quality Rejection | Failed inspection |
| Incorrect Material | Wrong item delivered |
| Excess Delivery | Over-delivered quantity |
| Supplier Recall | Supplier initiated recall |
| Warranty Return | Warranty replacement |
| Service Return | Rejected service |
| Administrative Return | Documentation error |

---

# Purchase Return Header

Each Purchase Return contains

- Return Number
- Purchase Order
- Goods Receipt
- Supplier
- Company
- Plant
- Warehouse
- Return Date
- Return Type
- Status
- Responsible User

---

# Purchase Return Lines

Each return line contains

- Material
- Batch
- Serial Number (Optional)
- Returned Quantity
- Unit
- Return Reason
- Warehouse
- Storage Location
- Remarks

Reference

Measurement_System.md

---

# Return Workflow

```
Goods Receipt

↓

Quality Inspection

↓

Return Decision

↓

Create Purchase Return

↓

Approval

↓

Supplier Notification

↓

Return Shipment

↓

Inventory Updated

↓

Credit Note
```

---

# Return Reasons

Supports configurable reason codes

- Damaged
- Incorrect Quantity
- Incorrect Material
- Quality Failure
- Expired Material
- Packaging Damage
- Transportation Damage
- Supplier Recall
- Warranty Issue
- Administrative Error

---

# Quality Integration

Purchase Returns may originate from

- Incoming Inspection
- Laboratory Test
- Production Rejection
- Supplier Audit
- Customer Complaint

Rejected materials remain blocked until the return process is completed.

Reference

06_Quality

---

# Inventory Integration

After approval

```
Purchase Return

↓

Inventory Return Transaction

↓

Inventory Reduced

↓

Warehouse Updated
```

Inventory updates are executed by the Inventory module.

Reference

02_Inventory

---

# Financial Integration

Purchase Return triggers

```
Purchase Return

↓

Supplier Credit Note

↓

Accounts Payable Adjustment

↓

Financial Reconciliation
```

Reference

08_Finance

---

# Supplier Notification

The system may notify suppliers by

- Email
- Supplier Portal
- API Integration
- EDI (Future)

Notification includes

- Return Number
- Material
- Quantity
- Return Reason
- Supporting Documents

---

# Supplier Credit Note

Supports

- Full Credit
- Partial Credit
- Replacement Material
- Financial Refund

Credit Note is linked to

- Purchase Order
- Goods Receipt
- Purchase Return

---

# Replacement Process

Optional workflow

```
Purchase Return

↓

Supplier Replacement

↓

New Goods Receipt

↓

Inventory Updated
```

---

# Batch Handling

Batch-controlled materials require

- Batch Validation
- Batch Traceability
- Supplier Batch Reference

Reference

TASK-020_Batch.md

---

# Serial Number Handling

Serialized materials require

- Serial Validation
- Warranty Validation
- Ownership Verification

---

# Attachments

Supports

- Photos
- Quality Reports
- Inspection Reports
- Supplier Correspondence
- Credit Notes
- Shipping Documents

Reference

File_Storage.md

---

# Mobile Workflow

```
Scan Material

↓

Select Return Reason

↓

Capture Photos

↓

Submit Return

↓

Manager Approval

↓

Return Shipment
```

Reference

Purchasing_Mobile.md

---

# Validation Rules

The system validates

- Goods Receipt exists.
- Supplier matches Goods Receipt.
- Returned quantity ≤ received quantity.
- Material exists.
- Warehouse exists.
- Return reason is mandatory.
- Batch requirement.
- Serial requirement.
- Required approvals completed.

Reference

Validation_Rules.md

---

# Dashboard

Purchase Return contributes to

- Open Returns
- Supplier Return Rate
- Quality Rejections
- Return Value
- Pending Credit Notes
- Supplier Performance

Reference

TASK-034_Purchasing_Dashboard.md

---

# Reports

Included in

- Purchase Return Report
- Supplier Return Analysis
- Return Reason Analysis
- Supplier Quality Report
- Credit Note Report
- Procurement KPI Report

Reference

TASK-035_Purchasing_Reports.md

---

# API

Primary endpoints

```
GET /purchase-returns

GET /purchase-returns/{id}

POST /purchase-returns

PUT /purchase-returns/{id}

POST /purchase-returns/{id}/submit

POST /purchase-returns/{id}/approve

POST /purchase-returns/{id}/ship

POST /purchase-returns/{id}/close

GET /purchase-returns/{id}/history
```

Reference

Purchasing_API.md

---

# Events

Publishing

- PurchaseReturnCreated
- PurchaseReturnSubmitted
- PurchaseReturnApproved
- PurchaseReturnShipped
- PurchaseReturnCompleted
- SupplierCreditRequested

Reference

Event_Model.md

Integration_Events.md

---

# Notifications

Supports

- Purchase Return Created
- Approval Required
- Supplier Notified
- Credit Note Received
- Replacement Shipment
- Return Completed

Reference

Notification_System.md

---

# Permissions

Typical permissions

- View Purchase Return
- Create Purchase Return
- Approve Purchase Return
- Ship Return
- Close Return
- View Financial Impact

Reference

Permission_Model.md

---

# Audit

The following actions are audited

- Purchase Return Created
- Return Reason Changed
- Approval Decision
- Shipment Confirmed
- Credit Note Linked
- Attachment Added
- User Actions

Reference

Audit_Log.md

---

# Performance

The system shall

- Create returns in less than 2 seconds.
- Support bulk return processing.
- Synchronize Inventory immediately.
- Support concurrent warehouse users.
- Cache supplier and purchase history.

Reference

Performance.md

Caching.md

Concurrency.md

---

# Security

Purchase Return follows

- Role-Based Authorization
- Purchasing Authorization
- Warehouse Authorization
- Secure API Access
- Complete Audit Logging

Reference

Security.md

Permission_Model.md

---

# AI Integration

AI may assist with

- Return Trend Analysis
- Supplier Quality Prediction
- Root Cause Analysis
- Fraud Detection
- Return Cost Analysis
- Supplier Risk Scoring

Reference

AI_Copilot.md

---

# Naswood Implementation

Typical return scenarios

## Damaged Timber

```
Supplier Delivery

↓

Goods Receipt

↓

Quality Inspection

↓

Purchase Return

↓

Supplier Replacement
```

---

## Chemical Quality Failure

```
Incoming Inspection

↓

Laboratory Failure

↓

Purchase Return

↓

Credit Note

↓

Replacement Shipment
```

---

## Wrong Material Delivery

```
Supplier Shipment

↓

Warehouse Receiving

↓

Material Mismatch

↓

Purchase Return

↓

Correct Delivery
```

---

## Machine Spare Part Warranty

```
Maintenance

↓

Fault Detection

↓

Warranty Return

↓

Supplier Replacement
```

---

# Acceptance Criteria

The Purchase Return module shall

- Support configurable return reasons.
- Integrate with Goods Receipt and Purchase Orders.
- Support supplier credit notes.
- Support batch and serial-controlled materials.
- Integrate with Inventory, Quality and Finance.
- Maintain complete audit and traceability.
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

TASK-031_Goods_Receipt_PO.md

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

File_Storage.md

AI_Copilot.md

Event_Model.md

Integration_Events.md

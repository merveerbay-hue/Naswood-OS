# TASK-025 — Inventory Adjustment

**Module:** Inventory

**Category:** Transaction

**Version:** 1.0

**Status:** Approved

---

# Purpose

The Inventory Adjustment transaction records authorized corrections to inventory quantities or inventory status when discrepancies are identified between system records and physical inventory.

Inventory Adjustments ensure inventory accuracy while maintaining complete traceability, auditability and approval control.

Inventory Adjustments are exceptional transactions and shall only be performed through approved business processes.

---

# Objectives

- Maintain Inventory Accuracy
- Correct Inventory Discrepancies
- Preserve Complete Audit Trail
- Support Financial Reconciliation
- Ensure Regulatory Compliance
- Prevent Unauthorized Inventory Changes

---

# Scope

Inventory Adjustment supports

- Quantity Adjustment
- Status Adjustment
- Batch Adjustment
- Location Adjustment
- Damage Adjustment
- Scrap Adjustment
- Inventory Correction
- Cycle Count Variance Resolution

Inventory Adjustment does NOT support

- Routine Goods Receipt
- Routine Goods Issue
- Warehouse Transfers
- Production Transactions

These operations are handled through their dedicated transaction types.

---

# Business Rules

- Every adjustment requires a valid reason code.
- Every adjustment shall reference a source document when applicable.
- Inventory adjustments require authorization based on company policy.
- Posted adjustments are immutable.
- Reversals require a new adjustment transaction.
- All adjustments are fully auditable.
- Inventory balances shall never be edited directly.

---

# Adjustment Types

| Type | Description |
|-------|-------------|
| Quantity Increase | Inventory gain |
| Quantity Decrease | Inventory loss |
| Damage | Damaged inventory |
| Scrap | Scrap declaration |
| Status Change | Inventory status update |
| Batch Correction | Batch reassignment |
| Location Correction | Storage correction |
| Opening Balance | Initial inventory loading |

---

# Adjustment Reasons

Typical reason codes

- Physical Count Variance
- Damaged Material
- Supplier Error
- Production Error
- Picking Error
- Receiving Error
- Lost Inventory
- Found Inventory
- Quality Reclassification
- Administrative Correction

Reason Codes shall be configurable by the system administrator.

---

# Transaction Lifecycle

```
Draft

↓

Submitted

↓

Under Review

↓

Approved

↓

Posted

↓

Completed

↓

Archived
```

Only **Posted** adjustments update inventory.

Reference

Status_Lifecycle.md

Approval_Workflow.md

---

# Adjustment Workflow

```
Inventory Discrepancy

↓

Create Adjustment

↓

Select Reason Code

↓

Enter Quantity

↓

Attach Evidence

↓

Manager Approval

↓

Post Adjustment

↓

Inventory Updated

↓

Audit Completed
```

---

# Inventory Impact

Inventory Adjustment may

- Increase On Hand Quantity
- Decrease On Hand Quantity
- Change Available Quantity
- Change Inventory Status
- Update Batch Inventory
- Update Warehouse Inventory
- Update Location Inventory

Every adjustment generates immutable inventory ledger entries.

Reference

TASK-019_Inventory.md

---

# Inventory Sources

Adjustments may originate from

- Inventory Count
- Warehouse Inspection
- Quality Inspection
- Production Review
- Customer Return Investigation
- Internal Audit
- System Correction

Reference

TASK-024_Inventory_Count.md

---

# Warehouse Integration

Adjustment requires

- Warehouse
- Location
- Material
- Quantity
- Reason Code

Reference

TASK-017_Warehouse.md

TASK-018_Location.md

---

# Batch Handling

For batch-controlled materials

Required

- Batch Validation
- Batch Availability
- Batch Status
- Batch History

Batch traceability shall never be broken.

Reference

TASK-020_Batch.md

---

# Serial Number Handling

For serialized materials

Required

- Serial Validation
- Serial Ownership Verification
- Duplicate Prevention
- Complete Traceability

---

# Financial Impact

Inventory Adjustment may affect inventory valuation.

After posting

```
Inventory Adjustment

↓

Finance Notification

↓

Inventory Valuation Update

↓

General Ledger Posting
```

Financial posting is handled by the Finance module.

Reference

08_Finance

---

# Evidence Management

The system supports attachment of

- Photos
- Documents
- Inspection Reports
- Count Sheets
- Quality Reports

Attachments become part of the permanent audit record.

Reference

File_Storage.md

---

# Approval Workflow

Approval requirements may depend on

- Adjustment Value
- Adjustment Quantity
- Material Type
- Warehouse
- Reason Code

Example

| Adjustment | Approval |
|------------|----------|
| ±5 Units | Warehouse Supervisor |
| ±100 Units | Warehouse Manager |
| High Value Material | Operations Manager |
| Financial Impact | Finance Approval |

Reference

Approval_Workflow.md

---

# Validation Rules

Before posting

The system validates

- Material exists.
- Warehouse exists.
- Location exists.
- Quantity is greater than zero.
- Reason code is mandatory.
- Required approvals completed.
- Batch requirement.
- Serial requirement.
- User permissions.

Reference

Validation_Rules.md

---

# Inventory Ledger

Posting creates

```
Inventory Ledger Entry

↓

Stock Balance Update

↓

Audit Entry

↓

Event Publication
```

Inventory Ledger entries are immutable.

---

# Barcode Support

Supports

- Material Barcode
- Batch Barcode
- Serial Barcode
- Location Barcode
- QR Code

Reference

Barcode_Strategy.md

QRCode_Strategy.md

---

# Mobile Workflow

Warehouse Supervisor

```
Login

↓

Scan Material

↓

Scan Location

↓

Enter Adjustment

↓

Select Reason

↓

Capture Photo

↓

Submit Approval

↓

Post Adjustment
```

Reference

Inventory_Mobile.md

---

# Dashboard

Inventory Adjustment contributes to

- Adjustment Summary
- Daily Adjustments
- Inventory Accuracy
- Variance Trends
- Adjustment Value
- Audit Exceptions

Reference

Inventory_Dashboard.md

---

# Reports

Included in

- Inventory Adjustment Report
- Inventory Variance Report
- Inventory Accuracy Report
- Audit Report
- Stock Card
- Material History

Reference

Inventory_Reports.md

---

# API

Primary endpoints

```
GET /inventory-adjustments

GET /inventory-adjustments/{id}

POST /inventory-adjustments

POST /inventory-adjustments/{id}/submit

POST /inventory-adjustments/{id}/approve

POST /inventory-adjustments/{id}/post

POST /inventory-adjustments/{id}/reverse

GET /inventory-adjustments/{id}/history
```

Reference

Inventory_API.md

---

# Events

Publishing

- InventoryAdjustmentCreated
- InventoryAdjustmentSubmitted
- InventoryAdjustmentApproved
- InventoryAdjusted
- InventoryReversed

Reference

Event_Model.md

Integration_Events.md

---

# Notifications

Supports

- Adjustment Submitted
- Approval Required
- Adjustment Approved
- Adjustment Rejected
- Inventory Updated
- Financial Review Required

Reference

Notification_System.md

---

# Permissions

Typical permissions

- View Adjustment
- Create Adjustment
- Submit Adjustment
- Approve Adjustment
- Post Adjustment
- Reverse Adjustment

Reference

Permission_Model.md

---

# Audit

The following actions are audited

- Adjustment Created
- Quantity Changed
- Reason Selected
- Approval Granted
- Adjustment Posted
- Adjustment Reversed
- Evidence Attached
- User Actions

Reference

Audit_Log.md

---

# Performance

The system shall

- Process adjustments in less than 2 seconds.
- Support concurrent users.
- Update inventory immediately after posting.
- Maintain complete adjustment history.
- Optimize audit queries.

Reference

Performance.md

Caching.md

Concurrency.md

---

# Security

Inventory Adjustment follows

- Role-Based Authorization
- Warehouse Authorization
- Multi-Level Approval
- Secure API Access
- Complete Audit Logging

Reference

Security.md

Permission_Model.md

---

# AI Integration

AI may assist with

- Variance Pattern Detection
- Fraud Detection
- Adjustment Risk Analysis
- Root Cause Analysis
- Inventory Accuracy Prediction
- Anomaly Detection

Reference

AI_Copilot.md

---

# Naswood Implementation

Typical adjustment scenarios

## Cycle Count Difference

```
Cycle Count

↓

Variance Detected

↓

Adjustment Request

↓

Supervisor Approval

↓

Inventory Updated
```

---

## Damaged Lumber

```
Warehouse Inspection

↓

Damaged Material

↓

Scrap Adjustment

↓

Scrap Warehouse
```

---

## Thermowood Quality Rejection

```
Quality Inspection

↓

Batch Rejected

↓

Status Adjustment

↓

Quality Hold Warehouse
```

---

## Production Loss

```
Production Consumption

↓

Unexpected Loss

↓

Inventory Adjustment

↓

Cost Analysis
```

---

# Acceptance Criteria

The Inventory Adjustment module shall

- Support multiple adjustment types.
- Require configurable approval workflows.
- Record mandatory reason codes.
- Preserve complete audit history.
- Support attachments as evidence.
- Integrate with Finance and Quality.
- Publish inventory events.
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

TASK-022_Goods_Issue.md

TASK-023_Stock_Transfer.md

TASK-024_Inventory_Count.md

Approval_Workflow.md

Audit_Log.md

Permission_Model.md

Validation_Rules.md

Barcode_Strategy.md

QRCode_Strategy.md

File_Storage.md

Performance.md

Caching.md

Concurrency.md

Security.md

Notification_System.md

AI_Copilot.md

Event_Model.md

Integration_Events.md

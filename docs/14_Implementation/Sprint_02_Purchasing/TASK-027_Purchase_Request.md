# TASK-027 — Purchase Request

**Module:** Purchasing

**Sprint:** Sprint 02 – Purchasing

**Category:** Procurement

**Priority:** Critical

**Estimated Effort:** 8 Days

**Status:** Planned

---

# Purpose

Develop the Purchase Request (PR) module for Naswood OS.

The Purchase Request module initiates the procurement lifecycle by allowing departments to formally request materials, services or assets required for operations.

Every purchasing process begins with an approved Purchase Request before progressing to RFQ, Purchase Order and Goods Receipt.

---

# Objectives

- Standardize Material Requests
- Approval-Based Procurement
- Budget Control
- MRP Integration
- Inventory Integration
- Complete Traceability
- Digital Approval Workflow

---

# Scope

The Purchase Request module includes

- Purchase Request Creation
- Material Requests
- Service Requests
- Approval Workflow
- Budget Validation
- Inventory Availability Check
- MRP Integration
- Attachment Management
- Status Tracking
- Request Cancellation

Out of Scope

- RFQ
- Purchase Order
- Supplier Selection
- Supplier Invoice

These are handled by subsequent procurement modules.

---

# Purchase Request Architecture

```
Department

↓

Purchase Request

↓

Approval Workflow

↓

Purchasing

↓

RFQ / Purchase Order
```

---

# Purchase Request Lifecycle

```
Draft

↓

Submitted

↓

Under Review

↓

Approved

↓

Partially Ordered

↓

Fully Ordered

↓

Closed

or

Rejected

or

Cancelled
```

Reference

Status_Lifecycle.md

---

# Purchase Request Types

Supports

- Raw Material
- Consumables
- Packaging
- Spare Parts
- Services
- Equipment
- Maintenance
- Office Supplies
- Capital Expenditure (CAPEX)

---

# Request Sources

Purchase Requests may originate from

- Manual Entry
- Production Planning
- MRP
- Inventory Reorder
- Maintenance Work Order
- Engineering
- Sales Project
- AI Recommendation

---

# Purchase Request Header

Each Purchase Request contains

## General Information

- PR Number
- Request Date
- Company
- Plant
- Department
- Requester
- Required Date
- Currency
- Priority
- Status

Reference

Currency.md

---

## Line Information

Each line contains

- Material Code
- Description
- Quantity
- Unit
- Required Date
- Warehouse
- Cost Center
- Project
- Estimated Price
- Notes

Reference

Unit_Conversion.md

---

# Priority Levels

Supports

- Low
- Normal
- High
- Urgent
- Emergency

Priority affects approval routing and purchasing deadlines.

---

# Inventory Check

Before approval the system automatically checks

- Current Inventory
- Reserved Quantity
- Available Quantity
- Open Purchase Orders
- Safety Stock

Possible outcomes

```
Available

↓

No Purchase Required
```

or

```
Insufficient Stock

↓

Continue Procurement
```

Reference

Inventory Module

---

# Budget Validation

Supports

- Budget Availability
- Cost Center Validation
- Project Budget Validation
- Approval Thresholds

Budget validation occurs before approval.

Reference

Finance Module

---

# Approval Workflow

Example

```
Requester

↓

Department Manager

↓

Budget Owner

↓

Purchasing Manager

↓

Approved
```

Approval rules may vary according to

- Company
- Plant
- Department
- Total Amount
- Material Category

Reference

Approval_Workflow.md

---

# MRP Integration

Production Planning may automatically generate

```
MRP

↓

Purchase Request

↓

Approval

↓

Purchasing
```

Reference

Production Module

---

# Inventory Integration

Purchase Requests validate

- Warehouse
- Material Availability
- Safety Stock
- Reorder Point

Reference

Inventory Module

---

# Attachments

Supports

- Technical Drawings
- Specifications
- Supplier References
- Photos
- Quotations
- Project Documents

Reference

TASK-012_File_Upload.md

---

# Search

Supports

- PR Number
- Material
- Department
- Requester
- Status
- Required Date
- Company
- Plant
- Priority

Reference

Search_Filtering.md

---

# Dashboard Information

Displays

- Open Requests
- Pending Approvals
- Approved Requests
- Urgent Requests
- PR Cycle Time
- PR by Department

Reference

TASK-034_Purchasing_Dashboard.md

---

# Reports

Supports

- Purchase Request List
- Open Purchase Requests
- Approval Waiting List
- Department Requests
- Budget Utilization
- Request Aging

Reference

TASK-035_Purchasing_Reports.md

---

# API Endpoints

```
GET /api/v1/purchase-requests

GET /api/v1/purchase-requests/{id}

POST /api/v1/purchase-requests

PUT /api/v1/purchase-requests/{id}

DELETE /api/v1/purchase-requests/{id}

POST /api/v1/purchase-requests/{id}/submit

POST /api/v1/purchase-requests/{id}/approve

POST /api/v1/purchase-requests/{id}/reject

POST /api/v1/purchase-requests/{id}/cancel
```

Reference

Purchasing_API.md

---

# Validation Rules

The system validates

- Company exists.
- Plant exists.
- Material exists.
- Quantity > 0.
- Unit is valid.
- Required Date is valid.
- Cost Center exists.
- Warehouse exists.
- Budget is available.
- Approved requests cannot be edited.

Reference

Validation_Rules.md

---

# Security

Supports

- Role-Based Access
- Department Authorization
- Company Isolation
- Plant Isolation
- Budget Authorization

Reference

Security.md

Permission_Model.md

---

# Audit

Records

- Purchase Request Created
- Updated
- Submitted
- Approved
- Rejected
- Cancelled
- Closed

Reference

Audit_Log.md

Logging.md

---

# Notifications

Supports

- Approval Required
- Request Approved
- Request Rejected
- Budget Exceeded
- Urgent Request
- Request Cancelled

Reference

Notification_System.md

---

# Events

Publishes

- PurchaseRequestCreated
- PurchaseRequestSubmitted
- PurchaseRequestApproved
- PurchaseRequestRejected
- PurchaseRequestCancelled
- PurchaseRequestClosed

Reference

Event_Model.md

Integration_Events.md

---

# Mobile Support

Supports

- Create Purchase Request
- View Requests
- Approve Requests
- Reject Requests
- Attachment Upload
- Barcode Material Lookup

Reference

Purchasing_Mobile.md

---

# Performance

Targets

- PR Creation < 1 second
- PR Search < 300 ms
- Approval < 500 ms
- Support 1,000,000+ Purchase Requests

Reference

Performance.md

Caching.md

---

# Naswood Examples

Example 1

```
Production

↓

Needs 500 m³ Spruce Timber

↓

MRP Creates Purchase Request

↓

Approval

↓

Purchasing
```

Example 2

```
Maintenance

↓

Bearing Replacement

↓

Purchase Request

↓

Urgent

↓

Immediate Approval
```

Example 3

```
Warehouse

↓

Adhesive Stock Below Minimum

↓

Automatic Purchase Request

↓

Buyer Review
```

---

# Acceptance Criteria

The Purchase Request module shall

- Support manual and automatic request creation.
- Validate inventory and budget before approval.
- Support configurable approval workflows.
- Integrate with Inventory, Production and Finance.
- Manage attachments and supporting documents.
- Publish procurement events.
- Maintain complete audit history.
- Follow all shared platform standards.

---

# Dependencies

Depends On

- TASK-026_Supplier.md
- TASK-012_File_Upload.md
- TASK-013_Audit_Log.md
- TASK-014_Settings.md
- Purchasing_Workflow.md
- Validation_Rules.md

---

# Related Documents

Purchasing_Architecture.md

Purchasing_API.md

Purchasing_Workflow.md

TASK-026_Supplier.md

TASK-028_RFQ.md

TASK-029_Supplier_Quotation.md

TASK-030_Purchase_Order.md

TASK-034_Purchasing_Dashboard.md

TASK-035_Purchasing_Reports.md

Approval_Workflow.md

Security.md

Permission_Model.md

Validation_Rules.md

Performance.md

Caching.md

Search_Filtering.md

Currency.md

Unit_Conversion.md

Audit_Log.md

Logging.md

Notification_System.md

Event_Model.md

Integration_Events.md

# TASK-027 — Purchase Request

**Module:** Purchasing

**Category:** Transaction

**Version:** 1.0

**Status:** Approved

---

# Purpose

The Purchase Request (PR) is the initial procurement document used to request the purchase of materials, services or subcontracting work.

It represents an internal business requirement and serves as the starting point of the procurement lifecycle.

A Purchase Request does not create any financial obligation. It must pass the defined approval workflow before it can be converted into a Request for Quotation (RFQ) or Purchase Order.

---

# Objectives

- Standardize Procurement Requests
- Centralize Purchase Demands
- Ensure Approval Compliance
- Improve Procurement Planning
- Enable Budget Control
- Support Manufacturing Operations
- Provide Complete Auditability

---

# Scope

Purchase Request supports

- Material Requests
- Service Requests
- Spare Parts Requests
- Tool Requests
- Chemical Requests
- Packaging Requests
- Capital Equipment Requests
- Maintenance Requests

Purchase Request does NOT

- Create Purchase Orders
- Receive Inventory
- Approve Supplier Selection
- Generate Financial Transactions

---

# Business Rules

- Every Purchase Request has a unique document number.
- Every Purchase Request belongs to one Company and one Plant.
- A PR may contain multiple request lines.
- Every request line references one Material or Service.
- PR approval is mandatory before procurement.
- Approved PRs cannot be edited.
- Closed PRs are immutable.
- Every action is audited.

---

# Purchase Request Lifecycle

```
Draft

↓

Submitted

↓

Department Approval

↓

Purchasing Review

↓

Approved

↓

RFQ / Purchase Order

↓

Completed

↓

Closed
```

Reference

Status_Lifecycle.md

Approval_Workflow.md

---

# Purchase Request Types

| Type | Description |
|-------|-------------|
| Material Request | Inventory materials |
| Service Request | External services |
| Spare Parts | Maintenance materials |
| Packaging | Packaging materials |
| Chemical | Production chemicals |
| Tool Request | Production tools |
| Investment | Machinery & Equipment |
| Emergency Purchase | Urgent procurement |

---

# Purchase Request Information

## Header

Each Purchase Request contains

- PR Number
- Company
- Plant
- Department
- Requester
- Request Date
- Required Date
- Priority
- Status
- Currency
- Remarks

---

## Request Lines

Each line contains

- Material
- Description
- Quantity
- Unit of Measure
- Estimated Price
- Warehouse
- Delivery Location
- Required Date
- Material Group

Reference

Material.md

Measurement_System.md

---

# Priority Levels

Supports

- Low
- Normal
- High
- Urgent
- Critical

Priority may influence approval workflow.

---

# Request Sources

Purchase Requests may originate from

- Manual Request
- Production Planning
- MRP
- Low Stock Alert
- Maintenance Work Order
- AI Recommendation
- Project Procurement

---

# Approval Workflow

Typical approval sequence

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

Approval levels are configurable based on

- Total Amount
- Material Group
- Department
- Budget
- Company Policy

Reference

Approval_Workflow.md

---

# Budget Validation

Before approval

The system validates

- Budget Availability
- Cost Center
- Project Budget
- Purchase Limits

If budget is insufficient

- Approval may be blocked
- Additional approval may be required

---

# Inventory Integration

Before creating a Purchase Request

The system may verify

- Current Stock
- Reserved Stock
- Incoming Purchase Orders
- Safety Stock

AI may recommend not purchasing if inventory is sufficient.

Reference

02_Inventory

---

# Production Integration

Purchase Requests may be automatically generated from

- Production Orders
- Material Shortages
- MRP
- Production Planning

Reference

05_Production

---

# Maintenance Integration

Maintenance may create requests for

- Spare Parts
- Consumables
- Equipment
- Services

Reference

07_Maintenance

---

# AI Integration

AI assists with

- Supplier Suggestions
- Budget Prediction
- Price Estimation
- Lead Time Prediction
- Duplicate Request Detection
- Stock Availability Check
- Demand Forecast

Reference

AI_Copilot.md

---

# Attachments

Supports

- Technical Drawings
- Specifications
- Photos
- PDFs
- Supplier Catalogs
- Emails

Reference

File_Storage.md

---

# Mobile Workflow

```
Create PR

↓

Attach Documents

↓

Submit

↓

Manager Approval

↓

Purchasing Review

↓

Approved
```

Reference

Purchasing_Mobile.md

---

# Barcode Support

When requesting inventory materials

Supports

- Material Barcode
- QR Code
- Scanner Lookup

Reference

Barcode_Strategy.md

QRCode_Strategy.md

---

# Validation Rules

Before submission

The system validates

- Requester exists.
- Material exists.
- Quantity > 0.
- Required Date is valid.
- Department assigned.
- Cost Center assigned.
- Budget validation completed.
- Warehouse exists.
- Material is active.

Reference

Validation_Rules.md

---

# RFQ Integration

Approved Purchase Requests may generate

```
Purchase Request

↓

RFQ

↓

Supplier Quotation
```

One PR may create multiple RFQs.

Reference

TASK-028_RFQ.md

---

# Purchase Order Integration

Depending on procurement policy

```
Purchase Request

↓

Purchase Order
```

or

```
Purchase Request

↓

RFQ

↓

Quotation

↓

Purchase Order
```

Reference

TASK-030_Purchase_Order.md

---

# Dashboard

Purchase Request contributes to

- Open Requests
- Pending Approvals
- Budget Usage
- Emergency Requests
- Approval Time
- Procurement Pipeline

Reference

TASK-034_Purchasing_Dashboard.md

---

# Reports

Included in

- Purchase Request Report
- Approval Report
- Department Requests
- Budget Consumption
- Procurement Pipeline
- Material Demand Report

Reference

TASK-035_Purchasing_Reports.md

---

# API

Primary endpoints

```
GET /purchase-requests

GET /purchase-requests/{id}

POST /purchase-requests

PUT /purchase-requests/{id}

POST /purchase-requests/{id}/submit

POST /purchase-requests/{id}/approve

POST /purchase-requests/{id}/reject

POST /purchase-requests/{id}/cancel

GET /purchase-requests/{id}/history
```

Reference

Purchasing_API.md

---

# Events

Publishing

- PurchaseRequestCreated
- PurchaseRequestSubmitted
- PurchaseRequestApproved
- PurchaseRequestRejected
- PurchaseRequestCancelled
- PurchaseRequestConvertedToRFQ
- PurchaseRequestConvertedToPO

Reference

Event_Model.md

Integration_Events.md

---

# Notifications

Supports

- Request Submitted
- Approval Required
- Request Approved
- Request Rejected
- Budget Warning
- Urgent Purchase Alert

Reference

Notification_System.md

---

# Permissions

Typical permissions

- View Purchase Request
- Create Purchase Request
- Edit Draft Request
- Submit Request
- Approve Request
- Reject Request
- Cancel Request

Reference

Permission_Model.md

---

# Audit

The following actions are audited

- PR Created
- PR Updated
- PR Submitted
- Approval Decision
- Budget Validation
- Line Changes
- Attachment Added
- User Actions

Reference

Audit_Log.md

---

# Performance

The system shall

- Create PR in less than 1 second.
- Support bulk request lines.
- Process approvals in real time.
- Cache material lookups.
- Support concurrent editing of draft documents.

Reference

Performance.md

Caching.md

Concurrency.md

---

# Security

Purchase Request follows

- Role-Based Authorization
- Department Authorization
- Budget Authorization
- Secure API Access
- Complete Audit Logging

Reference

Security.md

Permission_Model.md

---

# Naswood Implementation

Typical Purchase Request examples

| Department | Example |
|------------|---------|
| Production | Lumber, Glue, Chemicals |
| Thermowood | Kiln Consumables |
| Maintenance | Bearings, Motors |
| Packaging | Stretch Film, Labels |
| Projects | New Machinery |
| Office | IT Equipment |

Purchase Requests may originate manually or automatically from MRP, Production Planning or AI-based inventory forecasting.

---

# Acceptance Criteria

The Purchase Request module shall

- Support configurable request types.
- Support multi-level approval workflows.
- Validate budgets and materials.
- Integrate with Inventory, Production and Finance.
- Generate RFQs or Purchase Orders.
- Support mobile approval.
- Publish procurement events.
- Follow all shared platform standards.

---

# Related Documents

Purchasing_Architecture.md

Purchasing_API.md

Purchasing_Mobile.md

TASK-026_Supplier.md

TASK-028_RFQ.md

TASK-029_Supplier_Quotation.md

TASK-030_Purchase_Order.md

TASK-031_Goods_Receipt_PO.md

TASK-034_Purchasing_Dashboard.md

TASK-035_Purchasing_Reports.md

Approval_Workflow.md

Permission_Model.md

Validation_Rules.md

Material.md

Performance.md

Caching.md

Concurrency.md

Security.md

Audit_Log.md

Notification_System.md

Event_Model.md

Integration_Events.md

File_Storage.md

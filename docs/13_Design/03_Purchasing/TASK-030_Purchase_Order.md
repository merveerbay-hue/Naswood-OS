# TASK-030 — Purchase Order

**Module:** Purchasing

**Category:** Transaction

**Version:** 1.0

**Status:** Approved

---

# Purpose

The Purchase Order (PO) is the official procurement contract issued to a supplier for purchasing materials, services or subcontracted work.

It represents a legally binding commitment between Naswood and the supplier after completion of the purchasing approval process.

Purchase Orders are the foundation for Goods Receipt, Supplier Invoice processing and financial commitments.

---

# Objectives

- Standardize Procurement
- Control Purchasing Commitments
- Improve Supplier Communication
- Enable Inventory Planning
- Support Three-Way Matching
- Ensure Financial Control
- Maintain Full Procurement Traceability

---

# Scope

Purchase Order supports

- Material Purchasing
- Service Purchasing
- Blanket Purchase Orders
- Contract Orders
- Framework Agreements
- Partial Deliveries
- Partial Invoicing
- Multi-Currency Purchasing
- Multi-Plant Purchasing

Purchase Order does NOT

- Receive Inventory
- Process Supplier Payments
- Create Accounting Entries
- Manage Inventory Balances

---

# Business Rules

- Every Purchase Order has a unique document number.
- Every Purchase Order belongs to one supplier.
- Purchase Orders are generated from an approved Purchase Request or RFQ unless company policy allows direct purchasing.
- Posted Purchase Orders cannot be modified.
- Closed Purchase Orders are immutable.
- Every Purchase Order is fully auditable.

---

# Purchase Order Lifecycle

```
Draft

↓

Submitted

↓

Approval

↓

Released

↓

Supplier Confirmation

↓

Partial Delivery

↓

Complete Delivery

↓

Invoice Matching

↓

Closed
```

Reference

Status_Lifecycle.md

Approval_Workflow.md

---

# Purchase Order Types

| Type | Description |
|-------|-------------|
| Standard PO | Standard procurement |
| Service PO | Service procurement |
| Blanket PO | Long-term purchasing |
| Contract PO | Contract-based procurement |
| Framework PO | Framework agreement |
| Emergency PO | Urgent purchasing |
| Capital Investment PO | Machinery & Equipment |

---

# Purchase Order Header

Each Purchase Order contains

- PO Number
- Company
- Plant
- Purchasing Organization
- Supplier
- Currency
- Order Date
- Delivery Date
- Payment Terms
- Incoterms
- Buyer
- Status

---

# Purchase Order Lines

Each line contains

- Material
- Description
- Quantity
- Unit
- Unit Price
- Discount
- Tax
- Total Price
- Delivery Location
- Warehouse
- Required Date
- Material Group

Reference

Measurement_System.md

Currency.md

---

# Approval Workflow

Typical approval chain

```
Buyer

↓

Purchasing Manager

↓

Finance

↓

Operations Director

↓

Released
```

Approval depends on

- Order Value
- Material Category
- Supplier
- Budget
- Company Policy

Reference

Approval_Workflow.md

---

# Supplier Confirmation

Suppliers may

- Accept Purchase Order
- Reject Purchase Order
- Request Revision
- Confirm Partial Delivery
- Confirm Delivery Schedule

Confirmation history is permanently stored.

---

# Delivery Scheduling

Supports

- Single Delivery
- Multiple Deliveries
- Partial Delivery
- Scheduled Deliveries

Each schedule includes

- Delivery Date
- Quantity
- Delivery Location

---

# Partial Delivery

Supports

```
Purchase Order

100 Units

↓

Delivery 1

40 Units

↓

Delivery 2

30 Units

↓

Delivery 3

30 Units
```

The Purchase Order remains open until all scheduled deliveries are completed.

---

# Goods Receipt Integration

Workflow

```
Purchase Order

↓

Goods Receipt

↓

Inventory Updated
```

Receipt validation includes

- Ordered Quantity
- Delivered Quantity
- Remaining Quantity

Reference

TASK-031_Goods_Receipt_PO.md

---

# Three-Way Matching

Supplier invoices are validated against

```
Purchase Order

↓

Goods Receipt

↓

Supplier Invoice
```

Matching verifies

- Quantity
- Price
- Supplier
- Currency

Reference

TASK-033_Supplier_Invoice.md

---

# Inventory Integration

Purchase Orders provide

- Expected Receipts
- Incoming Inventory
- Material Availability Forecast

Inventory is updated only after Goods Receipt.

Reference

02_Inventory

---

# Finance Integration

Purchase Orders contribute to

- Procurement Commitments
- Budget Control
- Accounts Payable
- Cash Flow Forecast

Financial postings occur after invoice approval.

Reference

08_Finance

---

# Quality Integration

Materials may require

- Incoming Inspection
- Quality Hold
- Supplier Quality Review

Quality release determines inventory availability.

Reference

06_Quality

---

# AI Integration

AI assists with

- Supplier Recommendation
- Price Benchmarking
- Delivery Risk Prediction
- Budget Forecast
- Lead Time Prediction
- Procurement Optimization
- Duplicate PO Detection

Reference

AI_Copilot.md

---

# Attachments

Supports

- Supplier Contracts
- Technical Drawings
- Specifications
- Purchase Terms
- Certificates
- Emails
- PDFs

Reference

File_Storage.md

---

# Mobile Workflow

```
Review Purchase Order

↓

Approve

↓

Release

↓

Supplier Notification

↓

Track Delivery
```

Reference

Purchasing_Mobile.md

---

# Validation Rules

Before release

The system validates

- Supplier is Active.
- Material exists.
- Budget approved.
- Currency is valid.
- Delivery dates are valid.
- Required approvals completed.
- Warehouse exists.
- Payment terms defined.

Reference

Validation_Rules.md

---

# Dashboard

Purchase Order contributes to

- Open Purchase Orders
- Pending Deliveries
- Procurement Spend
- Supplier Commitments
- Delivery Performance
- Approval Queue

Reference

TASK-034_Purchasing_Dashboard.md

---

# Reports

Included in

- Purchase Order Report
- Open PO Report
- Supplier Commitment Report
- Delivery Schedule Report
- Procurement Spend Report
- Purchase History

Reference

TASK-035_Purchasing_Reports.md

---

# API

Primary endpoints

```
GET /purchase-orders

GET /purchase-orders/{id}

POST /purchase-orders

PUT /purchase-orders/{id}

POST /purchase-orders/{id}/submit

POST /purchase-orders/{id}/approve

POST /purchase-orders/{id}/release

POST /purchase-orders/{id}/close

POST /purchase-orders/{id}/cancel

GET /purchase-orders/{id}/history
```

Reference

Purchasing_API.md

---

# Events

Publishing

- PurchaseOrderCreated
- PurchaseOrderSubmitted
- PurchaseOrderApproved
- PurchaseOrderReleased
- PurchaseOrderConfirmed
- PurchaseOrderClosed
- PurchaseOrderCancelled

Reference

Event_Model.md

Integration_Events.md

---

# Notifications

Supports

- Purchase Order Released
- Supplier Confirmation Received
- Delivery Delay
- Partial Delivery
- Purchase Order Closed

Reference

Notification_System.md

---

# Permissions

Typical permissions

- View Purchase Order
- Create Purchase Order
- Edit Draft Purchase Order
- Approve Purchase Order
- Release Purchase Order
- Close Purchase Order
- Cancel Purchase Order

Reference

Permission_Model.md

---

# Audit

The following actions are audited

- Purchase Order Created
- Purchase Order Updated
- Approval Decision
- Release
- Supplier Confirmation
- Delivery Schedule Change
- Cancellation
- User Actions

Reference

Audit_Log.md

---

# Performance

The system shall

- Create Purchase Orders in less than 2 seconds.
- Support thousands of active Purchase Orders.
- Support concurrent buyer operations.
- Cache supplier and material master data.
- Process approvals in real time.

Reference

Performance.md

Caching.md

Concurrency.md

---

# Security

Purchase Order follows

- Role-Based Authorization
- Purchasing Authorization
- Budget Authorization
- Company Authorization
- Secure API Access
- Complete Audit Logging

Reference

Security.md

Permission_Model.md

---

# Naswood Implementation

Typical Purchase Order scenarios

## Timber Procurement

```
Approved RFQ

↓

Purchase Order

↓

Supplier Confirmation

↓

Goods Receipt

↓

Inventory
```

---

## Thermowood Chemicals

```
Production Planning

↓

Purchase Order

↓

Scheduled Deliveries

↓

Quality Inspection

↓

Warehouse
```

---

## Machinery Procurement

```
Investment Approval

↓

Purchase Order

↓

Supplier Acceptance

↓

Factory Acceptance Test

↓

Delivery

↓

Commissioning
```

---

## Packaging Materials

```
Monthly Requirement

↓

Blanket Purchase Order

↓

Weekly Deliveries

↓

Goods Receipt
```

---

# Acceptance Criteria

The Purchase Order module shall

- Support multiple Purchase Order types.
- Support configurable approval workflows.
- Manage delivery schedules and partial deliveries.
- Integrate with Goods Receipt and Supplier Invoice.
- Support three-way matching.
- Support AI-assisted procurement decisions.
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

TASK-031_Goods_Receipt_PO.md

TASK-032_Purchase_Return.md

TASK-033_Supplier_Invoice.md

TASK-034_Purchasing_Dashboard.md

TASK-035_Purchasing_Reports.md

Approval_Workflow.md

Permission_Model.md

Validation_Rules.md

Currency.md

Measurement_System.md

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

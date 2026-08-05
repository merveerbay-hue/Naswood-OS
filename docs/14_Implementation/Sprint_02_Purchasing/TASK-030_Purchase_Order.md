# TASK-030 — Purchase Order

**Module:** Purchasing

**Sprint:** Sprint 02 – Purchasing

**Category:** Procurement

**Priority:** Critical

**Estimated Effort:** 9 Days

**Status:** Planned

---

# Purpose

Develop the Purchase Order (PO) module for Naswood OS.

The Purchase Order module manages the complete purchasing commitment between Naswood and approved suppliers. It transforms approved Purchase Requests or awarded RFQs into legally binding procurement documents while integrating Inventory, Finance, Production and Quality.

Every external procurement transaction is executed through a Purchase Order.

---

# Objectives

- Standardize Purchase Orders
- Supplier Commitment
- Approval Workflow
- Budget Control
- Inventory Integration
- Finance Integration
- Complete Procurement Traceability

---

# Scope

The Purchase Order module includes

- Purchase Order Creation
- Supplier Selection
- Approval Workflow
- Purchase Order Release
- Delivery Scheduling
- Partial Deliveries
- Purchase Order Revision
- Purchase Order Cancellation
- Purchase Order Closure
- Document Attachments

Out of Scope

- Goods Receipt
- Supplier Invoice
- Supplier Payment

These processes are managed by their respective modules.

---

# Purchase Order Architecture

```
Purchase Request

↓

RFQ (Optional)

↓

Supplier Award

↓

Purchase Order

↓

Approval

↓

Release

↓

Supplier Delivery

↓

Goods Receipt
```

---

# Purchase Order Lifecycle

```
Draft

↓

Submitted

↓

Under Approval

↓

Approved

↓

Released

↓

Partially Received

↓

Fully Received

↓

Closed

or

Cancelled
```

Reference

Status_Lifecycle.md

---

# Purchase Order Types

Supports

- Standard Purchase Order
- Blanket Purchase Order
- Framework Agreement
- Service Purchase Order
- Capital Purchase Order
- Urgent Purchase Order
- Subcontract Purchase Order

---

# Purchase Order Sources

Purchase Orders may originate from

- Approved Purchase Request
- Awarded RFQ
- Framework Agreement
- Manual Entry
- MRP Recommendation
- Maintenance Procurement

---

# Purchase Order Header

Each Purchase Order contains

## General Information

- PO Number
- Supplier
- Company
- Plant
- Buyer
- Currency
- Purchase Type
- Order Date
- Delivery Address
- Payment Terms
- Delivery Terms
- Status

Reference

Currency.md

---

## Purchase Order Lines

Each line contains

- Material Code
- Description
- Quantity
- Unit
- Unit Price
- Discount
- Tax
- Net Amount
- Delivery Date
- Warehouse
- Cost Center
- Project
- Notes

Reference

Unit_Conversion.md

---

# Commercial Information

Supports

- Payment Terms
- Incoterms
- Currency
- Discount
- Freight
- Insurance
- Tax
- Warranty

---

# Delivery Scheduling

Supports

- Single Delivery
- Multiple Deliveries
- Partial Deliveries
- Delivery Calendar

Example

```
1000 Pieces

↓

300

↓

300

↓

400
```

---

# Approval Workflow

Example

```
Buyer

↓

Purchasing Manager

↓

Finance

↓

General Manager

↓

Approved
```

Approval rules may depend on

- Company
- Plant
- Total Amount
- Material Group
- Budget
- Supplier

Reference

Approval_Workflow.md

---

# Purchase Order Release

Only approved Purchase Orders may be released.

Workflow

```
Approved

↓

Released

↓

Supplier Notification

↓

Supplier Confirmation
```

Release generates supplier notification automatically.

---

# Supplier Confirmation

Supplier may

- Accept
- Reject
- Request Revision
- Confirm Delivery Date

Every response is recorded.

---

# Purchase Order Revision

Supports

- Quantity Changes
- Price Changes
- Delivery Date Changes
- Additional Lines
- Supplier Notes

Every revision creates a new version.

Previous versions remain available.

---

# Partial Deliveries

Supports

```
Purchase Order

↓

Delivery 1

↓

Goods Receipt

↓

Delivery 2

↓

Goods Receipt

↓

Purchase Order Closed
```

Remaining quantity is tracked automatically.

---

# Inventory Integration

Released Purchase Orders integrate with

- Goods Receipt
- Warehouse
- Inventory Reservation
- Material Availability

Reference

Inventory Module

---

# Finance Integration

Supports

- Budget Commitment
- Three-Way Matching
- Supplier Invoice
- Accounts Payable

Reference

Finance Module

---

# Attachments

Supports

- Supplier Contract
- Technical Drawings
- Specifications
- Terms & Conditions
- Price Agreement
- Supplier Confirmation

Reference

TASK-012_File_Upload.md

---

# Search

Supports

- PO Number
- Supplier
- Buyer
- Material
- Status
- Company
- Plant
- Delivery Date

Reference

Search_Filtering.md

---

# Dashboard Information

Displays

- Open Purchase Orders
- Orders Awaiting Approval
- Released Orders
- Deliveries Due
- Delayed Deliveries
- Purchase Value
- Supplier Performance

Reference

TASK-034_Purchasing_Dashboard.md

---

# Reports

Supports

- Purchase Order Register
- Open Purchase Orders
- Purchase by Supplier
- Purchase by Material
- Delivery Performance
- Budget Utilization
- Purchase Value Analysis

Reference

TASK-035_Purchasing_Reports.md

---

# API Endpoints

```
GET /api/v1/purchase-orders

GET /api/v1/purchase-orders/{id}

POST /api/v1/purchase-orders

PUT /api/v1/purchase-orders/{id}

DELETE /api/v1/purchase-orders/{id}

POST /api/v1/purchase-orders/{id}/submit

POST /api/v1/purchase-orders/{id}/approve

POST /api/v1/purchase-orders/{id}/release

POST /api/v1/purchase-orders/{id}/cancel

POST /api/v1/purchase-orders/{id}/close
```

Reference

Purchasing_API.md

---

# Validation Rules

The system validates

- Supplier is Active.
- Purchase Request is Approved.
- RFQ Award exists when required.
- Material exists.
- Quantity > 0.
- Unit Price > 0.
- Currency is valid.
- Delivery Date is valid.
- Warehouse exists.
- Budget is available.
- Released Purchase Orders cannot be edited.
- Closed Purchase Orders are read-only.

Reference

Validation_Rules.md

---

# Security

Supports

- Role-Based Access
- Buyer Authorization
- Company Isolation
- Plant Isolation
- Budget Authorization
- Financial Data Protection

Reference

Security.md

Permission_Model.md

---

# Audit

Records

- Purchase Order Created
- Updated
- Submitted
- Approved
- Released
- Revised
- Cancelled
- Closed
- Supplier Confirmation Received

Reference

Audit_Log.md

Logging.md

---

# Notifications

Supports

- Approval Required
- Purchase Order Approved
- Purchase Order Released
- Supplier Confirmation Received
- Delivery Delayed
- Purchase Order Cancelled
- Purchase Order Closed

Reference

Notification_System.md

---

# Events

Publishes

- PurchaseOrderCreated
- PurchaseOrderSubmitted
- PurchaseOrderApproved
- PurchaseOrderReleased
- PurchaseOrderCancelled
- PurchaseOrderClosed
- SupplierConfirmedPurchaseOrder

Reference

Event_Model.md

Integration_Events.md

---

# Mobile Support

Supports

- View Purchase Orders
- Purchase Order Approval
- Release Purchase Orders
- Supplier Contact
- Attachment Viewing
- Delivery Status Tracking

Purchase Order editing remains desktop-first.

Reference

Purchasing_Mobile.md

---

# Performance

Targets

- Purchase Order Creation < 1 second
- Purchase Order Search < 300 ms
- Approval < 500 ms
- Release < 1 second
- Support 1,000,000+ Purchase Orders

Reference

Performance.md

Caching.md

---

# Naswood Examples

### Example 1

```
Approved Purchase Request

↓

Spruce Timber

↓

Awarded Supplier

↓

Purchase Order

↓

Release

↓

Supplier Delivery
```

---

### Example 2

```
Framework Agreement

↓

Monthly Adhesive Order

↓

Purchase Order

↓

Partial Deliveries

↓

Goods Receipt
```

---

### Example 3

```
Maintenance Spare Parts

↓

Urgent Purchase Order

↓

Same-Day Approval

↓

Supplier Confirmation

↓

Immediate Shipment
```

---

# Acceptance Criteria

The Purchase Order module shall

- Create Purchase Orders from approved Purchase Requests or RFQs.
- Support configurable approval workflows.
- Support supplier confirmations and delivery scheduling.
- Support partial deliveries and purchase order revisions.
- Integrate with Inventory, Finance and Goods Receipt.
- Maintain complete version history.
- Publish procurement lifecycle events.
- Follow all shared platform standards.

---

# Dependencies

Depends On

- TASK-026_Supplier.md
- TASK-027_Purchase_Request.md
- TASK-028_RFQ.md
- TASK-029_Supplier_Quotation.md
- TASK-012_File_Upload.md
- Purchasing_Workflow.md
- Validation_Rules.md

---

# Related Documents

Purchasing_Architecture.md

Purchasing_API.md

Purchasing_Workflow.md

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

Security.md

Permission_Model.md

Validation_Rules.md

Currency.md

Unit_Conversion.md

Performance.md

Caching.md

Search_Filtering.md

Audit_Log.md

Logging.md

Notification_System.md

Event_Model.md

Integration_Events.md

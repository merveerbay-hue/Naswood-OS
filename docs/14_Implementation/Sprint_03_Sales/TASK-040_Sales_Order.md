# TASK-040 — Sales Order

**Module:** Sales

**Sprint:** Sprint 03 – Sales

**Category:** Order Management

**Priority:** Critical

**Estimated Effort:** 10 Days

**Status:** Planned

---

# Purpose

Develop the Sales Order module for Naswood OS.

The Sales Order module manages the complete customer order lifecycle from quotation acceptance through production planning, inventory allocation, delivery, invoicing and payment.

It serves as the central operational document connecting Sales, Production, Inventory, Logistics, Finance and CRM.

Every customer order is executed through the Sales Order module.

---

# Objectives

- Standardized Order Management
- Customer Commitment
- Production Integration
- Inventory Allocation
- Delivery Scheduling
- Financial Integration
- Complete Order Traceability

---

# Scope

The Sales Order module includes

- Sales Order Creation
- Order Approval
- Inventory Reservation
- Production Request
- Delivery Scheduling
- Partial Deliveries
- Order Revision
- Order Cancellation
- Order Completion
- Customer Communication

Out of Scope

- Customer Invoice
- Customer Payment
- Production Execution
- Shipment Tracking

---

# Sales Order Architecture

```
Opportunity

↓

Quotation

↓

Customer Acceptance

↓

Sales Order

↓

Approval

↓

Inventory Allocation

↓

Production

↓

Delivery

↓

Invoice

↓

Payment
```

---

# Sales Order Lifecycle

```
Draft

↓

Submitted

↓

Approved

↓

Released

↓

Inventory Reserved

↓

Production

↓

Ready for Delivery

↓

Partially Delivered

↓

Delivered

↓

Invoiced

↓

Completed

or

Cancelled
```

Reference

Status_Lifecycle.md

---

# Sales Order Sources

Sales Orders may originate from

- Accepted Quotation
- Contract
- Framework Agreement
- Dealer Portal
- Manual Entry
- E-Commerce (Future)
- API Integration

---

# Sales Order Header

Each Sales Order contains

## General Information

- Sales Order Number
- Customer
- Company
- Plant
- Salesperson
- Order Date
- Requested Delivery Date
- Currency
- Payment Terms
- Delivery Terms
- Status

Reference

Currency.md

---

## Customer Information

- Customer Name
- Billing Address
- Shipping Address
- Contact Person
- Phone
- Email

---

## Order Lines

Each order line contains

- Product Code
- Description
- Quantity
- Unit
- Unit Price
- Discount
- Tax
- Net Amount
- Total Amount
- Warehouse
- Delivery Date
- Production Required
- Notes

Reference

Unit_Conversion.md

---

# Product Types

Supports

- CLT Panels
- Glulam
- Thermowood
- Solid Wood Panels
- Timber
- Pellet
- Custom Manufacturing
- Engineering Services

---

# Inventory Allocation

The system checks

- Available Stock
- Reserved Stock
- Incoming Stock
- Production Availability

Possible outcomes

```
Stock Available

↓

Reserve Inventory
```

or

```
Insufficient Stock

↓

Generate Production Order
```

Reference

Inventory Module

---

# Production Integration

For manufactured products

```
Sales Order

↓

Production Order

↓

Production Planning

↓

Manufacturing

↓

Finished Goods

↓

Delivery
```

Reference

Production Module

---

# Delivery Scheduling

Supports

- Single Delivery
- Partial Deliveries
- Multiple Shipments
- Delivery Calendar

Example

```
1000 m² Thermowood

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
Salesperson

↓

Sales Manager

↓

Commercial Director

↓

Approved
```

Approval rules depend on

- Order Value
- Discount
- Customer Credit
- Product Category

Reference

Approval_Workflow.md

---

# Credit Validation

Before approval the system validates

- Customer Credit Limit
- Outstanding Balance
- Overdue Invoices
- Credit Hold Status

If validation fails

```
Finance Approval Required
```

Reference

Finance Module

---

# Order Revision

Supports

- Quantity Changes
- Product Changes
- Delivery Date Changes
- Price Changes
- Discount Changes

Every revision creates

```
Version 1

↓

Version 2

↓

Version 3
```

Complete revision history is preserved.

---

# Partial Delivery

Supports

```
Sales Order

↓

Delivery 1

↓

Invoice

↓

Delivery 2

↓

Invoice

↓

Completed
```

Remaining quantities remain open.

---

# Cancellation

Supports

- Complete Cancellation
- Partial Cancellation

Cancellation reasons

- Customer Request
- Stock Unavailable
- Payment Issue
- Production Issue
- Commercial Decision

---

# Customer Communication

Supports

- Order Confirmation
- Delivery Notification
- Delay Notification
- Shipment Notification
- Completion Notification

Reference

Notification_System.md

---

# Attachments

Supports

- Signed Quotation
- Customer Purchase Order
- Drawings
- Specifications
- Contracts
- Technical Documents

Reference

TASK-012_File_Upload.md

---

# Search

Supports

- Sales Order Number
- Customer
- Product
- Salesperson
- Status
- Delivery Date
- Company
- Plant

Reference

Search_Filtering.md

---

# Dashboard Information

Displays

- Open Sales Orders
- Orders Awaiting Approval
- Orders in Production
- Ready for Delivery
- Delayed Orders
- Monthly Sales Value

Reference

TASK-042_Sales_Dashboard.md

---

# Reports

Supports

- Sales Order Register
- Open Sales Orders
- Sales Orders by Customer
- Sales Orders by Product
- Delivery Performance
- Order Aging
- Revenue Analysis

Reference

TASK-043_Sales_Reports.md

---

# API Endpoints

```
GET /api/v1/sales-orders

GET /api/v1/sales-orders/{id}

POST /api/v1/sales-orders

PUT /api/v1/sales-orders/{id}

DELETE /api/v1/sales-orders/{id}

POST /api/v1/sales-orders/{id}/submit

POST /api/v1/sales-orders/{id}/approve

POST /api/v1/sales-orders/{id}/release

POST /api/v1/sales-orders/{id}/cancel

POST /api/v1/sales-orders/{id}/complete
```

Reference

Sales_API.md

---

# Validation Rules

The system validates

- Customer is Active.
- Customer Credit is valid.
- Product exists.
- Quantity > 0.
- Unit Price ≥ 0.
- Currency exists.
- Warehouse exists.
- Delivery Date is valid.
- Approved orders cannot be modified without revision.
- Completed orders are read-only.

Reference

Validation_Rules.md

---

# Security

Supports

- Role-Based Access
- Sales Territory Authorization
- Company Isolation
- Plant Isolation
- Credit Authorization
- Pricing Authorization

Reference

Security.md

Permission_Model.md

---

# Audit

Records

- Sales Order Created
- Updated
- Submitted
- Approved
- Released
- Revised
- Cancelled
- Completed
- Inventory Reserved
- Production Requested

Reference

Audit_Log.md

Logging.md

---

# Notifications

Supports

- Approval Required
- Sales Order Approved
- Inventory Reserved
- Production Started
- Delivery Scheduled
- Order Delayed
- Order Completed

Reference

Notification_System.md

---

# Events

Publishes

- SalesOrderCreated
- SalesOrderSubmitted
- SalesOrderApproved
- SalesOrderReleased
- InventoryReserved
- ProductionOrderCreated
- SalesOrderCancelled
- SalesOrderCompleted

Reference

Event_Model.md

Integration_Events.md

---

# Mobile Support

Supports

- View Sales Orders
- Approve Orders
- Customer Signature
- Delivery Status
- Attachments
- Order Tracking

Sales Order editing remains desktop-first.

Reference

Sales_Mobile.md

---

# Performance

Targets

- Sales Order Creation < 1 second
- Inventory Check < 500 ms
- Order Search < 300 ms
- Production Request < 2 seconds
- Support 2,000,000+ Sales Orders

Reference

Performance.md

Caching.md

---

# Naswood Examples

### Example 1

```
Customer

↓

CLT Building Project

↓

Accepted Quotation

↓

Sales Order

↓

Production

↓

Delivery
```

---

### Example 2

```
Dealer

↓

Thermowood Stock Order

↓

Inventory Available

↓

Immediate Delivery

↓

Invoice
```

---

### Example 3

```
Export Customer

↓

Glulam Order

↓

Production

↓

Container Shipment

↓

Completed
```

---

# Acceptance Criteria

The Sales Order module shall

- Create Sales Orders from accepted quotations.
- Validate customer credit automatically.
- Reserve inventory or trigger production.
- Support partial deliveries and revisions.
- Integrate with Production, Inventory and Finance.
- Maintain complete order history.
- Publish sales lifecycle events.
- Follow all shared platform standards.

---

# Dependencies

Depends On

- TASK-036_Customer.md
- TASK-038_Opportunity.md
- TASK-039_Quotation.md
- TASK-012_File_Upload.md
- Sales_Workflow.md
- Validation_Rules.md

---

# Related Documents

Sales_Architecture.md

Sales_API.md

Sales_Workflow.md

Sales_Mobile.md

TASK-036_Customer.md

TASK-037_Lead.md

TASK-038_Opportunity.md

TASK-039_Quotation.md

TASK-041_Customer_Invoice.md

TASK-042_Sales_Dashboard.md

TASK-043_Sales_Reports.md

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

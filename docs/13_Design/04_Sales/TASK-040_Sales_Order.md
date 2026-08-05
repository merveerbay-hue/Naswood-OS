# TASK-040 — Sales Order

**Module:** Sales

**Document Type:** Design Specification

**Version:** 1.0

**Status:** Approved

**Owner:** Sales Product Team

---

# Purpose

The Sales Order module manages confirmed customer orders after quotation acceptance.

A Sales Order represents the official commercial agreement between Naswood and the customer. It controls production planning, inventory allocation, logistics, invoicing and financial processing.

The Sales Order is the primary operational document driving downstream ERP processes.

---

# Design Goals

The module is designed to

- Standardize order management
- Integrate Sales with Production
- Reserve Inventory
- Trigger Manufacturing
- Support Partial Deliveries
- Track Order Lifecycle
- Provide Full Commercial Traceability

---

# Screen Layout

```
────────────────────────────────────────────────────────────

Sales Order List

────────────────────────────────────────────────────────────

Search

Filters

Order Grid

────────────────────────────────────────────────────────────

+ New Sales Order

Release

Reserve

Export

────────────────────────────────────────────────────────────
```

Selecting an order opens the Sales Order Detail screen.

---

# Sales Order Detail Layout

```
────────────────────────────────────────────────────────────

Sales Order Header

────────────────────────────────────────────────────────────

General

Customer

Products

Production

Inventory

Shipment

Invoice

Documents

Timeline

Notes

────────────────────────────────────────────────────────────
```

---

# Sales Order Header

Displays

- Sales Order Number
- Customer
- Quotation
- Status
- Order Date
- Delivery Date
- Currency
- Total Amount
- Salesperson
- Company
- Plant

Actions

- Edit
- Approve
- Release
- Reserve Inventory
- Create Production Order
- Create Shipment
- Cancel
- Print
- Export PDF

---

# Sales Order Status

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

Ready for Shipment

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

---

# Tab — General

Stores

## Basic Information

- Sales Order Number
- Customer
- Quotation Reference
- Order Date
- Requested Delivery Date
- Salesperson
- Currency
- Exchange Rate

## Commercial Information

- Payment Terms
- Delivery Terms
- Incoterms
- Order Priority
- Customer Reference

---

# Order Types

Supports

- Standard Order
- Project Order
- Export Order
- Dealer Order
- Stock Order
- Sample Order
- Framework Order
- Internal Order

---

# Tab — Customer

Displays

- Customer Information
- Delivery Address
- Invoice Address
- Contact Person
- Credit Status
- Outstanding Balance

Reference

TASK-036_Customer.md

---

# Tab — Products

Supports unlimited order lines.

Each line contains

- Product Code
- Product Name
- Description
- Quantity
- Unit
- Reserved Quantity
- Produced Quantity
- Delivered Quantity
- Remaining Quantity
- Unit Price
- Discount
- Tax
- Total

Supports

- Manufactured Products
- Stock Products
- Service Lines

---

# Production Integration

For manufactured products

```
Sales Order

↓

MRP

↓

Production Planning

↓

Production Order

↓

Manufacturing
```

Displays

- Production Status
- Planned Start
- Planned Finish
- Completion %
- Assigned Production Order

Reference

Production Module

---

# Inventory Integration

Displays

- Available Quantity
- Reserved Quantity
- Warehouse
- Batch
- Serial Number

Supports

- Automatic Reservation
- Manual Reservation
- Warehouse Selection

Reference

Inventory Module

---

# Shipment Integration

Displays

- Shipment Number
- Planned Shipment Date
- Carrier
- Vehicle
- Shipment Status
- Tracking Number

Supports

- Partial Shipment
- Multiple Shipments
- Export Shipment

Reference

TASK-041_Shipment.md

---

# Invoice Integration

Displays

- Invoice Number
- Invoice Date
- Invoice Amount
- Payment Status
- Outstanding Balance

Supports

- Partial Invoice
- Advance Invoice
- Final Invoice

Reference

TASK-043_Customer_Invoice.md

---

# Order Allocation

Allocation logic

```
Sales Order

↓

Available Inventory ?

↓

YES

↓

Reserve Stock

↓

Shipment
```

or

```
NO

↓

Production Planning

↓

Manufacturing

↓

Shipment
```

---

# Credit Control

Before release

System checks

- Credit Limit
- Outstanding Receivables
- Overdue Invoices
- Customer Status

If validation fails

```
Finance Approval Required
```

---

# Approval Workflow

```
Sales Representative

↓

Sales Manager

↓

Commercial Director

↓

Finance (Optional)

↓

Approved
```

Approval depends on

- Order Value
- Margin
- Credit Risk
- Discount %

---

# Delivery Schedule

Supports

- Single Delivery
- Partial Delivery
- Scheduled Deliveries
- Customer Requested Dates

Each delivery stores

- Planned Date
- Quantity
- Warehouse
- Shipment Status

---

# Revision Management

Supports unlimited revisions.

```
Rev 0

↓

Rev 1

↓

Rev 2
```

Each revision stores

- Revision Reason
- Modified By
- Approval History
- Change Comparison

Released orders require approval before modification.

---

# Attachments

Supports

- Signed Quotation
- Purchase Order
- Contracts
- Drawings
- Delivery Instructions
- Customer Documents

Reference

TASK-012_File_Upload.md

---

# Timeline

Displays

```
Quotation Accepted

↓

Sales Order Created

↓

Approved

↓

Inventory Reserved

↓

Production Started

↓

Shipment Created

↓

Delivered

↓

Invoice Issued

↓

Completed
```

---

# Notes

Supports

- Internal Notes
- Production Notes
- Logistics Notes
- Customer Notes

Supports mentions and attachments.

---

# Search

Supports

- Sales Order Number
- Customer
- Product
- Quotation
- Shipment
- Invoice
- Salesperson
- Status

Supports fuzzy search.

---

# Filters

Supports

- Status
- Customer
- Salesperson
- Order Type
- Delivery Date
- Currency
- Company
- Plant

---

# Sales Order KPIs

Displays

- Open Orders
- Released Orders
- Orders in Production
- Ready for Shipment
- Delayed Orders
- Completed Orders
- Order Value
- On-Time Delivery %

---

# User Actions

Users may

- Create Sales Order
- Edit Sales Order
- Approve
- Release
- Reserve Inventory
- Generate Production
- Create Shipment
- Cancel
- Duplicate
- Export PDF

---

# Validation Rules

The system validates

- Sales Order Number is unique.
- Customer is required.
- At least one order line is required.
- Delivery Date is required.
- Currency is required.
- Credit validation must pass before Release.
- Released orders require approval before modification.
- Completed orders are read-only.

---

# Permissions

Supports

- View Sales Order
- Create Sales Order
- Edit Sales Order
- Delete Sales Order
- Approve
- Release
- Reserve Inventory
- Create Shipment
- Cancel
- Export PDF

Reference

Permission_Model.md

---

# Notifications

Triggers

- Sales Order Created
- Approval Requested
- Order Released
- Inventory Reserved
- Production Started
- Shipment Ready
- Delivery Completed
- Order Cancelled

Reference

Notification_System.md

---

# Audit

Records

- Sales Order Created
- Updated
- Approved
- Released
- Inventory Reserved
- Production Created
- Shipment Created
- Delivered
- Cancelled

Reference

Audit_Log.md

---

# Mobile Behavior

Supports

- View Orders
- Order Status
- Production Status
- Shipment Tracking
- Delivery Schedule
- Customer Communication
- Offline Viewing

Reference

Sales_Mobile.md

---

# API References

```
GET    /sales-orders

GET    /sales-orders/{id}

POST   /sales-orders

PUT    /sales-orders/{id}

DELETE /sales-orders/{id}

POST   /sales-orders/{id}/approve

POST   /sales-orders/{id}/release

POST   /sales-orders/{id}/reserve

POST   /sales-orders/{id}/cancel

GET    /sales-orders/search
```

Reference

Sales_API.md

---

# Related Modules

- Customer
- Opportunity
- Quotation
- Production
- Inventory
- Shipment
- Delivery
- Customer Invoice
- Finance
- Dashboard

---

# UI Components

Uses standard platform components

- Data Grid
- Product Grid
- Search Box
- Filter Panel
- Timeline
- KPI Cards
- Status Badge
- Attachment Viewer
- Approval History
- Progress Indicator

Reference

Dashboard_Layout.md

Navigation.md

Theme.md

---

# Naswood Examples

### Example 1 — CLT Building

```
Sales Order

SO-2026-001254

↓

Customer

ABC Construction

↓

Product

3 Layer CLT

↓

Production Required

Yes

↓

Status

Production
```

---

### Example 2 — Thermowood Export

```
Sales Order

SO-2026-001488

↓

Customer

Nord Timber GmbH

↓

Currency

EUR

↓

Shipment

Hamburg Port

↓

Status

Ready for Shipment
```

---

### Example 3 — Dealer Order

```
Sales Order

SO-2026-001612

↓

Dealer

İstanbul Dealer

↓

Products

Thermowood Decking

Pellet

↓

Partial Delivery

Enabled
```

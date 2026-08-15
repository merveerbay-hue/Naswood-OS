> **UX authority note:** `+ New` / Create wireframes in this TASK are historical. Live CTAs: [`Sales_Screens.md`](./Sales_Screens.md) · [`Screen_Types.md`](../Common/Screen_Types.md) § Create matrix · Process_Screens.

# TASK-043 — Customer Invoice

**Module:** Sales

**Document Type:** Design Specification

**Version:** 1.0

**Status:** Approved

**Owner:** Sales Product Team

---

# Purpose

The Customer Invoice module manages the complete customer invoicing process after goods or services have been delivered.

A Customer Invoice represents the official financial document issued to the customer for delivered products or completed services. It integrates Sales, Logistics, Finance and Tax systems while supporting e-Invoice (e-Fatura), export invoices and multi-currency accounting.

The Customer Invoice module is the final commercial document in the Sales process before payment collection.

---

# Design Goals

The module is designed to

- Standardize customer invoicing
- Support automatic invoice generation
- Integrate with Delivery and Finance
- Support e-Invoice and e-Archive
- Manage credit notes
- Track payment status
- Ensure tax compliance

---

# Screen Layout

```
────────────────────────────────────────────────────────────

Customer Invoice List

────────────────────────────────────────────────────────────

Search

Filters

Invoice Grid

────────────────────────────────────────────────────────────

**Issue invoice** / Fatura kes

Issue

Cancel

Export PDF

────────────────────────────────────────────────────────────
```

Selecting an invoice opens the Customer Invoice Detail screen.

---

# Customer Invoice Detail Layout

```
────────────────────────────────────────────────────────────

Invoice Header

────────────────────────────────────────────────────────────

General

Customer

Lines

Taxes

Payments

Attachments

Timeline

Notes

────────────────────────────────────────────────────────────
```

---

# Invoice Header

Displays

- Invoice Number
- Invoice Type
- Customer
- Sales Order
- Delivery
- Status
- Invoice Date
- Due Date
- Currency
- Total Amount
- Company
- Plant

Actions

- Edit
- Issue
- Print
- Export PDF
- Send Email
- Cancel
- Create Credit Note

---

# Invoice Status

```
Draft

↓

Approved

↓

Issued

↓

Sent

↓

Partially Paid

↓

Paid

↓

Closed

or

Cancelled

or

Credit Note Issued
```

---

# Invoice Types

Supports

- Sales Invoice
- Export Invoice
- Dealer Invoice
- Project Invoice
- Advance Invoice
- Partial Invoice
- Final Invoice
- Proforma Invoice
- Credit Note
- Debit Note

---

# Tab — General

Stores

## Basic Information

- Invoice Number
- Invoice Date
- Due Date
- Currency
- Exchange Rate
- Payment Terms
- Tax Category

## References

- Sales Order
- Delivery Note
- Shipment
- Customer PO
- Contract Number

---

# Tab — Customer

Displays

- Customer Name
- Invoice Address
- Tax Office
- Tax Number
- VAT Number
- Contact Person
- Email

Reference

TASK-036_Customer.md

---

# Tab — Invoice Lines

Supports unlimited invoice lines.

Each line contains

- Product Code
- Product Name
- Description
- Quantity
- Unit
- Unit Price
- Discount
- Tax Rate
- Tax Amount
- Line Total

Supports

- Stock Products
- Manufactured Products
- Service Items
- Freight
- Installation

---

# Tax Calculation

Supports

- VAT
- Export Tax Exemption
- Withholding Tax
- Stamp Duty
- Environmental Fees

Automatically calculates

```
Subtotal

↓

Discount

↓

Tax

↓

Grand Total
```

Supports country-specific tax rules.

---

# Currency Support

Supports

- TRY
- EUR
- USD
- GBP
- CHF
- Other ISO currencies

Displays

- Exchange Rate
- Currency Difference
- Local Currency Equivalent

---

# Payment Information

Displays

- Payment Terms
- Due Date
- Outstanding Balance
- Paid Amount
- Remaining Balance
- Payment Method

Supports

- Bank Transfer
- Credit Card
- Cash
- Letter of Credit
- Open Account

---

# E-Invoice Integration

Supports

- e-Fatura
- e-Arşiv
- UBL XML
- Digital Signature
- Government Validation
- Status Tracking

Statuses

```
Generated

↓

Validated

↓

Sent

↓

Accepted

↓

Archived
```

---

# Credit Note Management

Supports

```
Invoice

↓

Credit Note Request

↓

Approval

↓

Credit Note

↓

Finance Posting
```

Stores

- Reason Code
- Reference Invoice
- Credit Amount
- Approval

---

# Finance Integration

Automatically posts

- Accounts Receivable
- Revenue
- Tax
- Cost of Goods Sold
- Currency Differences

Reference

Finance Module

---

# Delivery Integration

Invoices may be generated from

- One Delivery
- Multiple Deliveries
- Partial Deliveries

Reference

TASK-042_Delivery.md

---

# Payment Tracking

Displays

- Invoice Status
- Payment History
- Due Amount
- Aging
- Collection Status

Supports

- Partial Payments
- Installments
- Early Payment Discounts

---

# Attachments

Supports

- Invoice PDF
- e-Invoice XML
- Delivery Note
- Signed POD
- Customer PO
- Supporting Documents

Reference

TASK-012_File_Upload.md

---

# Timeline

Displays

```
Invoice Created

↓

Approved

↓

Issued

↓

Sent

↓

Customer Received

↓

Payment Received

↓

Closed
```

Every event is timestamped.

---

# Notes

Supports

- Finance Notes
- Customer Notes
- Collection Notes
- Internal Notes

Supports mentions and attachments.

---

# Search

Supports

- Invoice Number
- Customer
- Sales Order
- Delivery
- Tax Number
- Currency
- Status
- Invoice Date

Supports fuzzy search.

---

# Filters

Supports

- Invoice Status
- Invoice Type
- Customer
- Currency
- Due Date
- Payment Status
- Company
- Plant

---

# Invoice KPIs

Displays

- Total Invoices
- Issued Today
- Outstanding Invoices
- Overdue Invoices
- Paid Invoices
- Collection Rate
- DSO (Days Sales Outstanding)
- Invoice Value

---

# User Actions

Users may

- Create Invoice
- Edit Draft Invoice
- Approve
- Issue Invoice
- Send Email
- Print PDF
- Generate e-Invoice
- Create Credit Note
- Cancel Invoice

---

# Validation Rules

The system validates

- Invoice Number is unique.
- Customer is required.
- At least one invoice line is required.
- Invoice Date is required.
- Due Date ≥ Invoice Date.
- Currency is required.
- Tax calculation must be valid.
- Issued invoices cannot be edited.
- Cancelled invoices require a cancellation reason.
- Credit Notes require a reference invoice.

---

# Permissions

Supports

- View Invoice
- Create Invoice
- Edit Draft
- Approve
- Issue Invoice
- Cancel Invoice
- Create Credit Note
- Export PDF
- View Financial Information

Reference

Permission_Model.md

---

# Notifications

Triggers

- Invoice Created
- Invoice Approved
- Invoice Issued
- Invoice Sent
- Payment Received
- Invoice Overdue
- Credit Note Created

Reference

Notification_System.md

---

# Audit

Records

- Invoice Created
- Updated
- Approved
- Issued
- Sent
- Payment Recorded
- Credit Note Created
- Cancelled

Reference

Audit_Log.md

---

# Mobile Behavior

Supports

- Invoice Lookup
- PDF Preview
- Email Invoice
- Payment Status
- Customer Statement
- e-Invoice Status

Editing is desktop-first.

Reference

Sales_Mobile.md

---

# API References

```http
GET    /customer-invoices

GET    /customer-invoices/{id}

POST   /customer-invoices

PUT    /customer-invoices/{id}

DELETE /customer-invoices/{id}

POST   /customer-invoices/{id}/approve

POST   /customer-invoices/{id}/issue

POST   /customer-invoices/{id}/cancel

POST   /customer-invoices/{id}/credit-note

GET    /customer-invoices/search
```

Reference

Sales_API.md

---

# Related Modules

- Customer
- Sales Order
- Shipment
- Delivery
- Finance
- Accounts Receivable
- Tax
- Dashboard
- Reports

---

# UI Components

Uses standard platform components

- Data Grid
- Invoice Line Grid
- Search Box
- Filter Panel
- Status Badge
- Timeline
- PDF Viewer
- Attachment Viewer
- KPI Cards
- Payment Timeline

Reference

Dashboard_Layout.md

Navigation.md

Theme.md

---

# Naswood Examples

### Example 1 — CLT Project Invoice

```
Invoice

INV-2026-004812

↓

Customer

ABC Construction

↓

Reference

SO-2026-001254

↓

Amount

€1,250,000

↓

Status

Issued
```

---

### Example 2 — Export Invoice

```
Invoice

EXP-2026-000183

↓

Customer

Nord Timber GmbH

↓

Currency

EUR

↓

Tax

Export Exempt

↓

Status

Sent
```

---

### Example 3 — Dealer Invoice

```
Invoice

INV-2026-005624

↓

Dealer

İstanbul Dealer

↓

Products

Thermowood Decking

Pellet

↓

Payment Terms

30 Days

↓

Status

Partially Paid
```

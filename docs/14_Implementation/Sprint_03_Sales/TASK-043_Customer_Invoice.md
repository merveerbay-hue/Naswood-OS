# TASK-043 — Customer Invoice

**Module:** Sales

**Sprint:** Sprint 03 – Sales

**Category:** Accounts Receivable

**Priority:** Critical

**Estimated Effort:** 8 Days

**Status:** Planned

---

# Purpose

Develop the Customer Invoice module for Naswood OS.

The Customer Invoice module manages the complete customer billing process following successful delivery. It generates invoices from completed deliveries, validates tax information, integrates with Finance and tracks invoice lifecycle until payment.

The module provides complete traceability from Sales Order through Delivery to Accounts Receivable.

---

# Objectives

- Digital Customer Billing
- Automatic Invoice Generation
- Tax Compliance
- Finance Integration
- Accounts Receivable Management
- Invoice Traceability
- Customer Financial Visibility

---

# Scope

The Customer Invoice module includes

- Customer Invoice Creation
- Automatic Invoice Generation
- Invoice Approval
- Tax Calculation
- Credit Note Management
- Invoice Cancellation
- Invoice Status Tracking
- Finance Integration
- E-Invoice Support
- Attachment Management

Out of Scope

- Customer Payments
- General Ledger Posting
- Financial Reporting
- Debt Collection

---

# Customer Invoice Architecture

```
Sales Order

↓

Delivery

↓

Customer Invoice

↓

Finance

↓

Accounts Receivable

↓

Customer Payment
```

---

# Customer Invoice Lifecycle

```
Draft

↓

Generated

↓

Validated

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

or

Cancelled

or

Credit Note Issued
```

Reference

Status_Lifecycle.md

---

# Invoice Sources

Customer invoices may originate from

- Completed Delivery
- Sales Order
- Service Completion
- Manual Invoice
- API Integration

---

# Customer Invoice Header

Each Customer Invoice contains

## General Information

- Invoice Number
- Customer
- Sales Order
- Delivery Number
- Invoice Date
- Due Date
- Company
- Plant
- Currency
- Status

Reference

Currency.md

---

## Invoice Lines

Each invoice line contains

- Product Code
- Description
- Quantity
- Unit
- Unit Price
- Discount
- Tax
- Net Amount
- Gross Amount
- Delivery Reference

Reference

Unit_Conversion.md

---

# Financial Information

Supports

- Currency
- Exchange Rate
- VAT
- Withholding Tax
- Freight Charges
- Insurance
- Additional Charges
- Total Amount

---

# Invoice Generation

Workflow

```
Completed Delivery

↓

Invoice Generation

↓

Validation

↓

Approval

↓

Customer Invoice
```

Supports

- Automatic Generation
- Manual Generation
- Consolidated Invoice
- Partial Invoice

---

# Invoice Validation

The system validates

- Customer exists
- Delivery completed
- Sales Order exists
- Tax Code
- Currency
- Pricing
- Discounts
- Payment Terms

---

# Approval Workflow

Example

```
Sales

↓

Finance

↓

Accounting

↓

Approved
```

Approval rules depend on

- Invoice Amount
- Discount
- Customer Credit
- Tax Amount

Reference

Approval_Workflow.md

---

# Tax Validation

Supports

- VAT
- Withholding Tax
- Tax Exemption
- Export Tax Rules

Tax rules are provided by Finance.

---

# Credit Notes

Supports

- Full Credit Note
- Partial Credit Note
- Commercial Discount
- Returned Products
- Pricing Correction

Each Credit Note references the original invoice.

---

# Finance Integration

After approval

```
Customer Invoice

↓

Accounts Receivable

↓

General Ledger

↓

Payment Schedule
```

Reference

Finance Module

---

# Payment Status

Supports

- Not Due
- Due
- Partially Paid
- Paid
- Overdue
- Cancelled

Payments are managed by the Customer Payment module.

---

# E-Invoice Support

Supports

- E-Invoice
- E-Archive
- PDF Invoice
- XML Export

Future integrations

- Government E-Invoice Systems
- External ERP

---

# Attachments

Supports

- Invoice PDF
- Delivery Note
- Customer Purchase Order
- Credit Note
- Supporting Documents

Reference

TASK-012_File_Upload.md

---

# Search

Supports

- Invoice Number
- Customer
- Sales Order
- Delivery Number
- Invoice Date
- Due Date
- Status

Reference

Search_Filtering.md

---

# Dashboard Information

Displays

- Today's Invoices
- Outstanding Receivables
- Overdue Invoices
- Paid Invoices
- Invoice Value
- Collection Performance

Reference

TASK-045_Sales_Dashboard.md

---

# Reports

Supports

- Customer Invoice Register
- Invoice Aging
- Accounts Receivable
- Invoice by Customer
- VAT Report
- Outstanding Invoices
- Sales Revenue

Reference

TASK-046_Sales_Reports.md

---

# API Endpoints

```
GET /api/v1/customer-invoices

GET /api/v1/customer-invoices/{id}

POST /api/v1/customer-invoices

PUT /api/v1/customer-invoices/{id}

DELETE /api/v1/customer-invoices/{id}

POST /api/v1/customer-invoices/{id}/approve

POST /api/v1/customer-invoices/{id}/issue

POST /api/v1/customer-invoices/{id}/cancel

POST /api/v1/customer-invoices/{id}/credit-note

GET /api/v1/customer-invoices/search
```

Reference

Sales_API.md

---

# Validation Rules

The system validates

- Customer is Active.
- Delivery is Completed.
- Sales Order exists.
- Invoice Number is unique.
- Currency exists.
- Tax Code exists.
- Invoice Amount ≥ 0.
- Approved invoices cannot be edited.
- Paid invoices are read-only.

Reference

Validation_Rules.md

---

# Security

Supports

- Role-Based Access
- Sales Authorization
- Finance Authorization
- Company Isolation
- Plant Isolation
- Financial Data Protection

Reference

Security.md

Permission_Model.md

---

# Audit

Records

- Customer Invoice Created
- Validated
- Approved
- Issued
- Sent
- Credit Note Created
- Cancelled
- Payment Status Updated

Reference

Audit_Log.md

Logging.md

---

# Notifications

Supports

- Invoice Issued
- Invoice Sent
- Invoice Due Soon
- Overdue Invoice
- Credit Note Created
- Payment Received

Reference

Notification_System.md

---

# Events

Publishes

- CustomerInvoiceCreated
- CustomerInvoiceApproved
- CustomerInvoiceIssued
- CustomerInvoiceSent
- CustomerInvoiceCancelled
- CreditNoteCreated
- CustomerInvoicePaid

Reference

Event_Model.md

Integration_Events.md

---

# Mobile Support

Supports

- View Invoices
- Invoice PDF
- Invoice Status
- Customer Balance
- Payment Status

Invoice editing remains desktop-first.

Reference

Sales_Mobile.md

---

# Performance

Targets

- Invoice Generation < 2 seconds
- Invoice Search < 300 ms
- PDF Generation < 3 seconds
- Invoice Approval < 500 ms
- Support 2,000,000+ customer invoices

Reference

Performance.md

Caching.md

---

# Naswood Examples

### Example 1

```
Sales Order

↓

CLT Delivery

↓

Customer Invoice

↓

Accounts Receivable
```

---

### Example 2

```
Thermowood Delivery

↓

Automatic Invoice

↓

E-Invoice

↓

Customer Email
```

---

### Example 3

```
Export Customer

↓

EUR Invoice

↓

Tax Validation

↓

Finance Posting
```

---

# Acceptance Criteria

The Customer Invoice module shall

- Generate invoices from completed deliveries.
- Support automatic and manual invoice generation.
- Validate taxes and commercial information.
- Integrate with Finance and Accounts Receivable.
- Support credit notes and invoice cancellation.
- Publish invoice lifecycle events.
- Support E-Invoice readiness.
- Follow all shared platform standards.

---

# Dependencies

Depends On

- TASK-040_Sales_Order.md
- TASK-041_Shipment.md
- TASK-042_Delivery.md
- TASK-012_File_Upload.md
- Sales_Workflow.md
- Validation_Rules.md

---

# Related Documents

Sales_Architecture.md

Sales_API.md

Sales_Workflow.md

Sales_Mobile.md

TASK-040_Sales_Order.md

TASK-041_Shipment.md

TASK-042_Delivery.md

TASK-044_Customer_Payment.md

TASK-045_Sales_Dashboard.md

TASK-046_Sales_Reports.md

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

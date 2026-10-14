# TASK-033 — Supplier Invoice

**Module:** Purchasing

**Sprint:** Sprint 02 – Purchasing

**Category:** Accounts Payable

**Priority:** Critical

**Estimated Effort:** 8 Days

**Status:** Completed

---

# Purpose

Develop the Supplier Invoice module for Naswood OS.

The Supplier Invoice module manages supplier invoices received after Goods Receipt, validates them through the Three-Way Matching process and transfers approved invoices to the Finance module for payment processing.

The module ensures financial accuracy, procurement compliance and complete traceability between Purchasing, Inventory and Finance.

---

# Objectives

- Digital Supplier Invoice Management
- Three-Way Matching
- Financial Accuracy
- Approval Workflow
- Tax Validation
- Accounts Payable Integration
- Complete Procurement Traceability

---

# Scope

The Supplier Invoice module includes

- Supplier Invoice Registration
- Invoice Validation
- Three-Way Matching
- Tax Calculation
- Invoice Approval
- Invoice Rejection
- Credit Note Processing
- Attachment Management
- Finance Integration
- Invoice Status Tracking

Out of Scope

- Purchase Orders
- Goods Receipt
- Supplier Payment
- General Ledger Posting

---

# Supplier Invoice Architecture

```
Purchase Order

↓

Goods Receipt

↓

Supplier Invoice

↓

Three-Way Matching

↓

Approval

↓

Finance

↓

Payment
```

---

# Supplier Invoice Lifecycle

```
Draft

↓

Received

↓

Validation

↓

Three-Way Matching

↓

Under Approval

↓

Approved

↓

Transferred to Finance

↓

Paid

or

Rejected

or

Cancelled
```

Reference

Status_Lifecycle.md

---

# Invoice Sources

Supplier invoices may originate from

- Goods Receipt
- Supplier Portal
- Manual Entry
- Email Import (Future)
- E-Invoice Integration
- API Integration

---

# Supplier Invoice Header

Each Supplier Invoice contains

## General Information

- Invoice Number
- Supplier
- Purchase Order
- Goods Receipt
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

- Material Code
- Description
- Quantity
- Unit
- Unit Price
- Discount
- Tax
- Net Amount
- Gross Amount
- Purchase Order Line
- Goods Receipt Line

Reference

Unit_Conversion.md

---

# Financial Information

Supports

- Invoice Currency
- Exchange Rate
- VAT
- Withholding Tax
- Freight Cost
- Insurance
- Additional Charges
- Total Amount

---

# Three-Way Matching

Invoice validation compares

```
Purchase Order

↓

Goods Receipt

↓

Supplier Invoice
```

Validation includes

- Supplier
- Material
- Quantity
- Unit Price
- Currency
- Tax
- Total Amount

---

# Matching Results

Possible outcomes

```
Matched

↓

Approval
```

```
Price Difference

↓

Buyer Review
```

```
Quantity Difference

↓

Warehouse Review
```

```
Tax Difference

↓

Finance Review
```

Tolerance limits are configurable.

Reference

Validation_Rules.md

---

# Invoice Approval

Example workflow

```
Buyer

↓

Finance

↓

Accounting

↓

Approved
```

Approval rules may depend on

- Invoice Amount
- Company
- Supplier
- Cost Center
- Budget
- Tax Amount

Reference

Approval_Workflow.md

---

# Tax Validation

Supports

- VAT Validation
- Tax Code Validation
- Withholding Tax
- Tax Percentage Validation

Tax rules are provided by Finance.

---

# Credit Notes

Supports

- Full Credit Note
- Partial Credit Note
- Purchase Return Credit
- Commercial Discount Credit

Credit Notes remain linked to the original invoice.

---

# Finance Integration

After approval

```
Supplier Invoice

↓

Accounts Payable

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

- Unpaid
- Scheduled
- Partially Paid
- Paid
- Cancelled

Payment processing is handled by Finance.

---

# Attachments

Supports

- Supplier Invoice PDF
- E-Invoice XML
- Delivery Note
- Credit Note
- Tax Documents
- Supporting Documents

Reference

TASK-012_File_Upload.md

---

# Search

Supports

- Invoice Number
- Supplier
- Purchase Order
- Goods Receipt
- Due Date
- Invoice Date
- Status
- Currency

Reference

Search_Filtering.md

---

# Dashboard Information

Displays

- Pending Invoices
- Awaiting Approval
- Invoice Value
- Matching Exceptions
- Due Soon
- Overdue Invoices

Reference

TASK-034_Purchasing_Dashboard.md

---

# Reports

Supports

- Supplier Invoice Register
- Invoice Aging
- Invoice by Supplier
- Matching Exceptions
- VAT Report
- Outstanding Payables

Reference

TASK-035_Purchasing_Reports.md

---

# API Endpoints

```
GET /api/v1/supplier-invoices

GET /api/v1/supplier-invoices/{id}

POST /api/v1/supplier-invoices

PUT /api/v1/supplier-invoices/{id}

DELETE /api/v1/supplier-invoices/{id}

POST /api/v1/supplier-invoices/{id}/validate

POST /api/v1/supplier-invoices/{id}/approve

POST /api/v1/supplier-invoices/{id}/reject

POST /api/v1/supplier-invoices/{id}/transfer-finance

GET /api/v1/supplier-invoices/search
```

Reference

Purchasing_API.md

---

# Validation Rules

The system validates

- Supplier is Active.
- Purchase Order exists.
- Goods Receipt exists.
- Invoice Number is unique per Supplier.
- Currency is valid.
- Invoice Date is valid.
- Due Date is valid.
- Tax Code exists.
- Three-Way Matching completed.
- Approved invoices cannot be edited.
- Paid invoices are read-only.

Reference

Validation_Rules.md

---

# Security

Supports

- Role-Based Access
- Purchasing Authorization
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

- Supplier Invoice Created
- Updated
- Validated
- Three-Way Match Completed
- Approved
- Rejected
- Transferred to Finance
- Payment Status Updated

Reference

Audit_Log.md

Logging.md

---

# Notifications

Supports

- Invoice Received
- Matching Exception
- Approval Required
- Invoice Approved
- Invoice Rejected
- Due Date Reminder
- Overdue Invoice

Reference

Notification_System.md

---

# Events

Publishes

- SupplierInvoiceCreated
- SupplierInvoiceValidated
- ThreeWayMatchCompleted
- SupplierInvoiceApproved
- SupplierInvoiceRejected
- SupplierInvoiceTransferred
- SupplierInvoicePaid

Reference

Event_Model.md

Integration_Events.md

---

# Mobile Support

Supports

- View Supplier Invoices
- Invoice Approval
- Attachment Viewing
- Invoice Search
- Due Date Tracking

Invoice creation remains desktop-first.

Reference

Purchasing_Mobile.md

---

# Performance

Targets

- Invoice Creation < 1 second
- Three-Way Matching < 2 seconds
- Invoice Search < 300 ms
- Approval < 500 ms
- Support 2,000,000+ supplier invoices

Reference

Performance.md

Caching.md

---

# Naswood Examples

### Example 1

```
Purchase Order

↓

Goods Receipt

↓

Supplier Invoice

↓

Three-Way Matching

↓

Approved

↓

Finance
```

---

### Example 2

```
Timber Supplier

↓

Invoice Quantity

↓

Matches Goods Receipt

↓

Automatic Approval
```

---

### Example 3

```
Adhesive Supplier

↓

Invoice Price Higher

↓

Tolerance Exceeded

↓

Buyer Review Required
```

---

# Acceptance Criteria

The Supplier Invoice module shall

- Register supplier invoices digitally.
- Validate invoices using Three-Way Matching.
- Support configurable approval workflows.
- Integrate with Purchasing, Inventory and Finance.
- Track invoice payment status.
- Support credit notes and tax validation.
- Publish invoice lifecycle events.
- Follow all shared platform standards.

---

# Dependencies

Depends On

- TASK-026_Supplier.md
- TASK-030_Purchase_Order.md
- TASK-031_Goods_Receipt_PO.md
- TASK-032_Purchase_Return.md
- TASK-012_File_Upload.md
- Purchasing_Workflow.md
- Validation_Rules.md

---

# Related Documents

Purchasing_Architecture.md

Purchasing_API.md

Purchasing_Workflow.md

TASK-026_Supplier.md

TASK-030_Purchase_Order.md

TASK-031_Goods_Receipt_PO.md

TASK-032_Purchase_Return.md

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

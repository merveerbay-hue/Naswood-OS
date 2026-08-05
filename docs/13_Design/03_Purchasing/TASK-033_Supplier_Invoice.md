# TASK-033 — Supplier Invoice

**Module:** Purchasing

**Category:** Financial Transaction

**Version:** 1.0

**Status:** Approved

---

# Purpose

The Supplier Invoice represents the financial document issued by a supplier for delivered materials, services or subcontracted work.

Supplier Invoices validate procurement transactions through Three-Way Matching with the Purchase Order and Goods Receipt before being transferred to the Finance module for payment processing.

Supplier Invoice management ensures procurement accuracy, financial compliance and complete auditability.

---

# Objectives

- Standardize Supplier Invoice Processing
- Support Three-Way Matching
- Improve Financial Accuracy
- Prevent Duplicate Payments
- Maintain Procurement Traceability
- Integrate Purchasing with Finance
- Support AI-Based Invoice Validation

---

# Scope

Supplier Invoice supports

- Material Invoices
- Service Invoices
- Credit Notes
- Debit Notes
- Partial Invoices
- Multiple Goods Receipts
- Multiple Purchase Orders
- Tax Validation
- Invoice Approval
- Finance Integration

Supplier Invoice does NOT

- Execute Payments
- Manage General Ledger
- Process Bank Transactions
- Maintain Accounts Payable

These processes belong to the Finance module.

---

# Business Rules

- Every Supplier Invoice belongs to one supplier.
- Every invoice references at least one Purchase Order.
- Every invoice references one or more Goods Receipts.
- Duplicate supplier invoice numbers are not permitted.
- Approved invoices become read-only.
- Financial posting occurs only after successful validation.
- Every invoice is permanently auditable.

---

# Supplier Invoice Lifecycle

```
Draft

↓

Imported / Entered

↓

Validation

↓

Three-Way Matching

↓

Approval

↓

Posted

↓

Transferred to Finance

↓

Payment

↓

Closed
```

Reference

Status_Lifecycle.md

Approval_Workflow.md

---

# Invoice Types

| Type | Description |
|-------|-------------|
| Material Invoice | Inventory purchases |
| Service Invoice | External services |
| Advance Invoice | Prepayment |
| Partial Invoice | Partial delivery billing |
| Final Invoice | Final settlement |
| Credit Note | Supplier credit |
| Debit Note | Supplier debit |

---

# Invoice Header

Each Supplier Invoice contains

- Invoice Number
- Supplier
- Purchase Order
- Company
- Plant
- Currency
- Invoice Date
- Due Date
- Payment Terms
- Tax Information
- Total Amount
- Status

---

# Invoice Lines

Each invoice line contains

- Material / Service
- Purchase Order Line
- Goods Receipt Reference
- Quantity
- Unit Price
- Discount
- Tax
- Line Total
- Currency
- Cost Center (Optional)

Reference

Currency.md

Measurement_System.md

---

# Three-Way Matching

Supplier Invoice validation requires

```
Purchase Order

↓

Goods Receipt

↓

Supplier Invoice
```

The system compares

- Supplier
- Material
- Quantity
- Unit Price
- Currency
- Tax
- Purchase Order Status
- Goods Receipt Status

Only successful matches may proceed automatically.

---

# Matching Results

Possible outcomes

| Result | Description |
|---------|-------------|
| Full Match | Automatic approval |
| Partial Match | Manual review |
| Quantity Mismatch | Approval required |
| Price Mismatch | Approval required |
| Duplicate Invoice | Rejected |
| Missing Goods Receipt | Blocked |

---

# Partial Invoice

Supports

```
Purchase Order

100 Units

↓

Goods Receipt

40 Units

↓

Invoice

40 Units

↓

Remaining Balance

60 Units
```

Multiple invoices may be linked to a single Purchase Order.

---

# Credit Notes

Supports

- Purchase Return Credit
- Price Correction
- Commercial Discount
- Warranty Credit
- Supplier Compensation

Credit Notes are linked to

- Purchase Return
- Purchase Order
- Original Invoice

Reference

TASK-032_Purchase_Return.md

---

# Tax Validation

Supports

- VAT
- Withholding Tax
- Tax Exemption
- Reverse Charge
- Multi-Tax Scenarios

Tax calculation follows company configuration.

---

# Currency Handling

Supports

- Local Currency
- Supplier Currency
- Exchange Rate
- Invoice Currency
- Accounting Currency

Exchange rates follow platform currency rules.

Reference

Currency.md

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

↓

Payment Processing
```

Reference

08_Finance

---

# Purchasing Integration

Workflow

```
Purchase Request

↓

Purchase Order

↓

Goods Receipt

↓

Supplier Invoice

↓

Finance
```

Reference

Purchasing_Architecture.md

---

# Inventory Integration

Invoice validation verifies

- Goods Receipt Posted
- Received Quantity
- Purchase Order Completion
- Material Acceptance

Inventory balances are not modified.

Reference

02_Inventory

---

# AI Integration

AI assists with

- Duplicate Invoice Detection
- Fraud Detection
- Price Anomaly Detection
- OCR Invoice Reading
- Matching Suggestions
- Payment Prediction
- Invoice Risk Analysis

Reference

AI_Copilot.md

---

# Attachments

Supports

- PDF Invoice
- XML e-Invoice
- Credit Note
- Delivery Documents
- Contracts
- Purchase Order
- Goods Receipt

Reference

File_Storage.md

---

# Mobile Workflow

```
View Invoice

↓

Review Matching

↓

Approve

↓

Finance Transfer
```

Reference

Purchasing_Mobile.md

---

# Validation Rules

The system validates

- Supplier exists.
- Purchase Order exists.
- Goods Receipt exists.
- Invoice number is unique.
- Currency is valid.
- Tax is valid.
- Invoice amount is positive.
- Three-Way Matching completed.
- Required approvals completed.

Reference

Validation_Rules.md

---

# Dashboard

Supplier Invoice contributes to

- Pending Invoices
- Invoice Approval Queue
- Three-Way Match Rate
- Accounts Payable
- Invoice Aging
- Payment Forecast

Reference

TASK-034_Purchasing_Dashboard.md

---

# Reports

Included in

- Supplier Invoice Report
- Invoice Aging Report
- Three-Way Matching Report
- Invoice Approval Report
- Accounts Payable Report
- Procurement Cost Report

Reference

TASK-035_Purchasing_Reports.md

---

# API

Primary endpoints

```
GET /supplier-invoices

GET /supplier-invoices/{id}

POST /supplier-invoices

PUT /supplier-invoices/{id}

POST /supplier-invoices/{id}/validate

POST /supplier-invoices/{id}/approve

POST /supplier-invoices/{id}/post

POST /supplier-invoices/{id}/cancel

GET /supplier-invoices/{id}/matching
```

Reference

Purchasing_API.md

---

# Events

Publishing

- SupplierInvoiceCreated
- SupplierInvoiceValidated
- SupplierInvoiceMatched
- SupplierInvoiceApproved
- SupplierInvoicePosted
- SupplierInvoiceTransferredToFinance
- SupplierInvoiceCancelled

Reference

Event_Model.md

Integration_Events.md

---

# Notifications

Supports

- Invoice Received
- Matching Failed
- Approval Required
- Invoice Approved
- Finance Transfer Completed
- Duplicate Invoice Warning

Reference

Notification_System.md

---

# Permissions

Typical permissions

- View Supplier Invoice
- Create Supplier Invoice
- Validate Invoice
- Approve Invoice
- Post Invoice
- Cancel Invoice
- View Financial Information

Reference

Permission_Model.md

---

# Audit

The following actions are audited

- Invoice Created
- Invoice Imported
- Validation Completed
- Matching Result
- Approval Decision
- Finance Transfer
- Attachment Added
- User Actions

Reference

Audit_Log.md

---

# Performance

The system shall

- Validate invoices in less than 2 seconds.
- Support bulk invoice imports.
- Execute Three-Way Matching in real time.
- Cache Purchase Orders and Goods Receipts.
- Support concurrent finance users.

Reference

Performance.md

Caching.md

Concurrency.md

---

# Security

Supplier Invoice follows

- Role-Based Authorization
- Purchasing Authorization
- Finance Authorization
- Secure API Access
- Complete Audit Logging

Reference

Security.md

Permission_Model.md

---

# Naswood Implementation

Typical invoice scenarios

## Raw Timber Procurement

```
Purchase Order

↓

Goods Receipt

↓

Supplier Invoice

↓

Three-Way Matching

↓

Finance
```

---

## Thermowood Chemicals

```
Supplier Delivery

↓

Batch Receiving

↓

Invoice

↓

Payment Approval
```

---

## Machinery Purchase

```
Capital Purchase Order

↓

Equipment Delivery

↓

Supplier Invoice

↓

Asset Registration

↓

Finance
```

---

## Packaging Materials

```
Blanket Purchase Order

↓

Weekly Deliveries

↓

Monthly Supplier Invoice

↓

Three-Way Matching
```

---

# Acceptance Criteria

The Supplier Invoice module shall

- Support multiple invoice types.
- Perform configurable Three-Way Matching.
- Prevent duplicate invoices.
- Support multi-currency and tax validation.
- Integrate with Purchasing, Inventory and Finance.
- Support attachments and OCR-ready invoice processing.
- Publish procurement and finance events.
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

TASK-030_Purchase_Order.md

TASK-031_Goods_Receipt_PO.md

TASK-032_Purchase_Return.md

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

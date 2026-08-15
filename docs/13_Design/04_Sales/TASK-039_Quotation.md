> **UX authority note:** `+ New` / Create wireframes in this TASK are historical. Live CTAs: [`Sales_Screens.md`](./Sales_Screens.md) · [`Screen_Types.md`](../Common/Screen_Types.md) § Create matrix · Process_Screens.

# TASK-039 — Quotation

**Module:** Sales

**Document Type:** Design Specification

**Version:** 1.0

**Status:** Approved

**Owner:** Sales Product Team

---

# Purpose

The Quotation module manages the complete quotation lifecycle within Naswood ERP.

A quotation is the official commercial proposal presented to a customer based on one or more opportunities. It contains products, pricing, discounts, taxes, delivery conditions, commercial terms and approval workflows.

The Quotation module serves as the bridge between CRM and Sales Order Management.

---

# Design Goals

The module is designed to

- Standardize quotation preparation
- Support configurable pricing
- Manage quotation revisions
- Automate approval workflows
- Generate professional PDF quotations
- Track customer acceptance
- Integrate with Sales Orders

---

# Screen Layout

```
────────────────────────────────────────────────────────────

Quotation List

────────────────────────────────────────────────────────────

Search

Filters

Quotation Grid

────────────────────────────────────────────────────────────

**Prepare quotation** / Teklif hazırla

Duplicate

Export PDF

Approve

────────────────────────────────────────────────────────────
```

Selecting a quotation opens the Quotation Detail screen.

---

# Quotation Detail Layout

```
────────────────────────────────────────────────────────────

Quotation Header

────────────────────────────────────────────────────────────

General

Customer

Products

Pricing

Terms

Approval

Attachments

Timeline

Notes

────────────────────────────────────────────────────────────
```

---

# Quotation Header

Displays

- Quotation Number
- Revision
- Customer
- Opportunity
- Status
- Currency
- Valid Until
- Total Amount
- Salesperson
- Company
- Plant

Actions

- Edit
- Submit
- Approve
- Reject
- Duplicate
- Generate PDF
- Email Customer
- Convert to Sales Order

---

# Quotation Status

```
Draft

↓

Submitted

↓

Under Review

↓

Approved

↓

Sent

↓

Negotiation

↓

Accepted

↓

Converted

or

Rejected

or

Expired

or

Cancelled
```

---

# Tab — General

Stores

## Basic Information

- Quotation Number
- Revision Number
- Opportunity
- Customer
- Salesperson
- Currency
- Exchange Rate

## Commercial Information

- Quotation Date
- Valid Until
- Incoterms
- Delivery Terms
- Payment Terms

---

# Tab — Customer

Displays

- Customer
- Contact Person
- Delivery Address
- Invoice Address
- Customer Reference
- Customer Notes

Reference

TASK-036_Customer.md

---

# Tab — Products

Supports unlimited quotation lines.

Each line contains

- Product Code
- Product Name
- Description
- Quantity
- Unit
- Unit Price
- Discount %
- Discount Amount
- Net Unit Price
- Tax
- Total

Supports

- Manual Line
- BOM Product
- Manufactured Product
- Stock Product

---

# Product Configuration

Supports

- Product Variants
- Dimensions
- Custom Specifications
- Surface Finish
- Wood Species
- Packaging Type

Examples

- CLT Panels
- Thermowood
- Glulam
- Solid Panels
- Pellet

---

# Tab — Pricing

Displays

- Gross Amount
- Line Discounts
- Header Discount
- Freight
- Insurance
- Tax
- Grand Total
- Margin
- Cost Estimate

Supports

- Multi Currency
- Customer Price Lists
- Dealer Discounts
- Campaign Discounts
- Project Pricing

---

# Tab — Commercial Terms

Stores

- Delivery Time
- Delivery Method
- Incoterms
- Warranty
- Validity Period
- Packaging Conditions
- Installation Included
- Export Conditions

---

# Approval Workflow

```
Sales Representative

↓

Sales Manager

↓

Commercial Director

↓

General Manager

↓

Approved
```

Approval levels are determined by

- Discount Percentage
- Margin
- Total Amount
- Customer Category

---

# Revision Management

Supports unlimited revisions.

```
Rev 0

↓

Rev 1

↓

Rev 2

↓

Rev 3
```

Each revision stores

- Revision Date
- Changed By
- Change Reason
- Previous Version
- Comparison

---

# PDF Generation

Generates professional quotation documents.

Contents

- Cover Page
- Customer Information
- Product Table
- Technical Specifications
- Commercial Terms
- Pricing Summary
- Signature Area

Supports

- Company Branding
- Dealer Branding
- Multi-language
- Multi-currency

---

# Email Integration

Supports

- Send PDF
- Send as Link
- Approval Request
- Reminder Email
- Revision Notification

Tracks

- Sent Date
- Delivery Status
- Read Confirmation

---

# Customer Acceptance

Supports

- Manual Approval
- Customer Portal Approval
- Digital Signature
- Email Confirmation

Accepted quotations can be converted into Sales Orders.

---

# Tab — Attachments

Supports

- Drawings
- Technical Specifications
- Product Images
- Contracts
- Customer RFQ
- BOQ Files
- Calculation Sheets

Reference

TASK-012_File_Upload.md

---

# Tab — Timeline

Displays

```
Quotation Created

↓

Submitted

↓

Approved

↓

Sent

↓

Customer Viewed

↓

Negotiation

↓

Accepted

↓

Sales Order Created
```

---

# Tab — Notes

Supports

- Internal Notes
- Customer Notes
- Technical Notes
- Approval Notes

Supports rich text and mentions.

---

# Search

Supports

- Quotation Number
- Customer
- Opportunity
- Salesperson
- Product
- Status
- Date
- Currency

Supports fuzzy search.

---

# Filters

Supports

- Status
- Customer
- Salesperson
- Date
- Currency
- Valid Until
- Opportunity
- Company
- Plant

---

# Quotation KPIs

Displays

- Total Quotations
- Open Quotations
- Accepted Quotations
- Rejected Quotations
- Expired Quotations
- Average Quotation Value
- Conversion Rate
- Approval Time

---

# User Actions

Users may

- Create Quotation
- Edit Quotation
- Duplicate Quotation
- Submit for Approval
- Approve
- Reject
- Generate PDF
- Email Customer
- Accept
- Convert to Sales Order
- Archive

---

# Validation Rules

The system validates

- Quotation Number is unique.
- Customer is required.
- At least one product line is required.
- Valid Until must be greater than Quotation Date.
- Currency is required.
- Unit Price cannot be negative.
- Discounts cannot exceed authorization limits.
- Accepted quotations cannot be edited.
- Expired quotations cannot be converted.

---

# Permissions

Supports

- View Quotation
- Create Quotation
- Edit Quotation
- Delete Quotation
- Submit
- Approve
- Reject
- Export PDF
- Email Customer
- Convert to Sales Order

Reference

Permission_Model.md

---

# Notifications

Triggers

- Quotation Created
- Approval Requested
- Approved
- Rejected
- Customer Viewed
- Expiring Soon
- Accepted
- Converted to Sales Order

Reference

Notification_System.md

---

# Audit

Records

- Quotation Created
- Revised
- Submitted
- Approved
- Rejected
- Sent
- Viewed
- Accepted
- Converted

Reference

Audit_Log.md

---

# Mobile Behavior

Supports

- View Quotations
- Create Quotation
- Approve
- Reject
- PDF Preview
- Customer Signature
- Email Customer
- Offline Viewing

Reference

Sales_Mobile.md

---

# API References

```
GET    /quotations

GET    /quotations/{id}

POST   /quotations

PUT    /quotations/{id}

DELETE /quotations/{id}

POST   /quotations/{id}/approve

POST   /quotations/{id}/reject

POST   /quotations/{id}/send

POST   /quotations/{id}/duplicate

GET    /quotations/{id}/pdf
```

Reference

Sales_API.md

---

# Related Modules

- Customer
- Opportunity
- Sales Order
- Pricing
- Approval Workflow
- CRM
- Dashboard
- Reports
- Finance

---

# UI Components

Uses standard platform components

- Data Grid
- Product Grid
- Search Box
- Filter Panel
- Approval Timeline
- Revision History
- PDF Preview
- Attachment Viewer
- KPI Cards
- Status Badge

Reference

Dashboard_Layout.md

Navigation.md

Theme.md

---

# Naswood Examples

### Example 1 — CLT Building Project

```
Quotation

QT-2026-00452

↓

Customer

ABC Construction

↓

Product

3 Layer CLT

↓

Value

€1,250,000
```

---

### Example 2 — Thermowood Dealer

```
Quotation

QT-2026-00581

↓

Customer

Nord Timber GmbH

↓

Product

Thermowood

↓

Currency

EUR
```

---

### Example 3 — Glulam Sports Hall

```
Quotation

QT-2026-00617

↓

Products

Glulam Beams

Steel Connectors

Installation

↓

Status

Approved

↓

Revision

Rev 2
```

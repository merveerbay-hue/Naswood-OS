# TASK-039 — Sales Quotation

**Module:** Sales

**Sprint:** Sprint 03 – Sales

**Category:** Sales

**Priority:** Critical

**Estimated Effort:** 9 Days

**Status:** Completed

---

# Purpose

Develop the Sales Quotation module for Naswood OS.

The Sales Quotation module manages the complete quotation process from customer request to quotation approval, revision, negotiation and conversion into a Sales Order.

It enables the Sales team to prepare accurate quotations, maintain pricing consistency, monitor quotation performance and improve quotation-to-order conversion.

---

# Objectives

- Standardized Quotation Process
- Customer Proposal Management
- Pricing Control
- Discount Authorization
- Revision Management
- Sales Pipeline Integration
- Complete Commercial Traceability

---

# Scope

The Sales Quotation module includes

- Quotation Creation
- Product Pricing
- Discount Management
- Approval Workflow
- Quotation Revision
- Customer Negotiation
- Version Control
- Quotation Acceptance
- Sales Order Conversion
- Quotation Cancellation

Out of Scope

- Customer Master
- Sales Order Fulfillment
- Customer Invoice
- Customer Payment

---

# Quotation Architecture

```
Lead / Opportunity

↓

Quotation

↓

Approval

↓

Customer Review

↓

Negotiation

↓

Accepted

↓

Sales Order

or

Rejected
```

---

# Quotation Lifecycle

```
Draft

↓

Submitted

↓

Under Review

↓

Approved

↓

Sent to Customer

↓

Negotiation

↓

Accepted

↓

Converted to Sales Order

or

Rejected

or

Expired

or

Cancelled
```

Reference

Status_Lifecycle.md

---

# Quotation Sources

Supports

- Opportunity
- Existing Customer
- Manual Entry
- Dealer Portal
- CRM Opportunity
- Website Inquiry
- AI Recommendation

---

# Quotation Header

Each quotation contains

## General Information

- Quotation Number
- Customer
- Opportunity
- Salesperson
- Company
- Plant
- Currency
- Quotation Date
- Valid Until
- Status

Reference

Currency.md

---

## Customer Information

- Customer Name
- Contact Person
- Billing Address
- Delivery Address
- Payment Terms
- Delivery Terms

---

## Quotation Lines

Each quotation line contains

- Product Code
- Description
- Quantity
- Unit
- Unit Price
- Discount %
- Discount Amount
- Tax
- Net Amount
- Total Amount
- Delivery Time
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
- Architectural Components
- Custom Manufacturing
- Engineering Services

---

# Pricing

Supports

- Standard Price List
- Customer Price List
- Contract Pricing
- Project Pricing
- Promotional Pricing

Automatic pricing is supported.

---

# Discount Management

Supports

- Line Discount
- Document Discount
- Campaign Discount
- Manual Discount
- Customer Discount

Maximum discount limits are role-based.

Reference

Permission_Model.md

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

- Total Amount
- Discount Percentage
- Customer Risk
- Product Category

Reference

Approval_Workflow.md

---

# Revision Management

Supports

- Quantity Changes
- Price Changes
- Discount Changes
- Delivery Updates
- Product Changes

Each revision creates

```
Revision 1

↓

Revision 2

↓

Revision 3
```

Complete revision history is preserved.

---

# Customer Negotiation

Supports

- Counter Offer
- Price Negotiation
- Quantity Negotiation
- Delivery Negotiation
- Discount Negotiation

Every negotiation is logged.

---

# Acceptance

Customer may

- Accept
- Reject
- Request Revision

Accepted quotations automatically create

```
Sales Order
```

Reference

Sales Order Module

---

# Expiration

Each quotation has

- Valid From
- Valid Until

Expired quotations cannot be converted into Sales Orders.

Automatic expiration notifications are supported.

---

# Attachments

Supports

- Technical Drawings
- Product Catalogs
- Specifications
- Render Images
- Contracts
- BOQ
- Customer Documents

Reference

TASK-012_File_Upload.md

---

# Search

Supports

- Quotation Number
- Customer
- Salesperson
- Opportunity
- Product
- Status
- Date Range
- Currency

Reference

Search_Filtering.md

---

# Dashboard Information

Displays

- Draft Quotations
- Approved Quotations
- Quotations Awaiting Approval
- Quotations Sent
- Accepted Quotations
- Expired Quotations
- Conversion Rate

Reference

TASK-042_Sales_Dashboard.md

---

# Reports

Supports

- Quotation Register
- Quotation Conversion Rate
- Quotation Aging
- Quotations by Salesperson
- Discount Analysis
- Lost Quotations
- Quotation Revenue

Reference

TASK-043_Sales_Reports.md

---

# API Endpoints

```
GET /api/v1/quotations

GET /api/v1/quotations/{id}

POST /api/v1/quotations

PUT /api/v1/quotations/{id}

DELETE /api/v1/quotations/{id}

POST /api/v1/quotations/{id}/submit

POST /api/v1/quotations/{id}/approve

POST /api/v1/quotations/{id}/send

POST /api/v1/quotations/{id}/accept

POST /api/v1/quotations/{id}/reject

POST /api/v1/quotations/{id}/convert-order
```

Reference

Sales_API.md

---

# Validation Rules

The system validates

- Quotation Number is unique.
- Customer exists.
- Product exists.
- Quantity > 0.
- Unit Price ≥ 0.
- Currency exists.
- Valid Until ≥ Quotation Date.
- Discount within authorization limits.
- Approved quotations cannot be edited.
- Expired quotations cannot become Sales Orders.

Reference

Validation_Rules.md

---

# Security

Supports

- Role-Based Access
- Sales Territory Authorization
- Company Isolation
- Plant Isolation
- Pricing Authorization
- Discount Authorization

Reference

Security.md

Permission_Model.md

---

# Audit

Records

- Quotation Created
- Updated
- Submitted
- Approved
- Sent
- Revised
- Accepted
- Rejected
- Converted to Sales Order

Reference

Audit_Log.md

Logging.md

---

# Notifications

Supports

- Approval Required
- Quotation Approved
- Quotation Sent
- Customer Accepted
- Customer Rejected
- Expiration Reminder
- High Value Quotation

Reference

Notification_System.md

---

# Events

Publishes

- QuotationCreated
- QuotationSubmitted
- QuotationApproved
- QuotationSent
- QuotationAccepted
- QuotationRejected
- QuotationExpired
- QuotationConvertedToSalesOrder

Reference

Event_Model.md

Integration_Events.md

---

# Mobile Support

Supports

- View Quotations
- Create Quotation
- Customer Presentation
- PDF Preview
- Digital Signature
- Approval
- Customer Acceptance Tracking

Reference

Sales_Mobile.md

---

# Performance

Targets

- Quotation Creation < 1 second
- Pricing Calculation < 500 ms
- Search < 300 ms
- PDF Generation < 3 seconds
- Support 1,000,000+ quotations

Reference

Performance.md

Caching.md

---

# Naswood Examples

### Example 1

```
Architect

↓

CLT Villa Project

↓

Quotation

↓

3 Revisions

↓

Customer Approval

↓

Sales Order
```

---

### Example 2

```
Thermowood Dealer

↓

Annual Price List

↓

Discount Approval

↓

Quotation Sent

↓

Accepted
```

---

### Example 3

```
Export Customer

↓

Glulam Project

↓

EUR Pricing

↓

Negotiation

↓

Contract Award
```

---

# Acceptance Criteria

The Sales Quotation module shall

- Create quotations from opportunities or customers.
- Support pricing, discounts and approvals.
- Maintain complete quotation revision history.
- Support customer negotiations.
- Convert accepted quotations into Sales Orders.
- Publish quotation lifecycle events.
- Integrate with CRM and Sales modules.
- Follow all shared platform standards.

---

# Dependencies

Depends On

- TASK-012_File_Upload.md
- TASK-013_Audit_Log.md
- TASK-014_Settings.md
- TASK-036_Customer.md
- TASK-037_Lead.md
- TASK-038_Opportunity.md
- Sales_API.md
- Validation_Rules.md

---

# Related Documents

Sales_Architecture.md

Sales_API.md

Sales_Workflow.md

TASK-036_Customer.md

TASK-037_Lead.md

TASK-038_Opportunity.md

TASK-040_Sales_Order.md

TASK-041_Customer_Payment.md

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

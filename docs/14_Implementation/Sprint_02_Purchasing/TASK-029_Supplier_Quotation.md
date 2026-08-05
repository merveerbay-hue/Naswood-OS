# TASK-029 — Supplier Quotation

**Module:** Purchasing

**Sprint:** Sprint 02 – Purchasing

**Category:** Procurement

**Priority:** Critical

**Estimated Effort:** 7 Days

**Status:** Planned

---

# Purpose

Develop the Supplier Quotation module for Naswood OS.

The Supplier Quotation module manages all commercial offers submitted by suppliers in response to RFQs. It supports quotation comparison, technical validation, commercial evaluation, negotiation tracking and supplier selection prior to Purchase Order creation.

The module provides complete traceability from RFQ publication to supplier award.

---

# Objectives

- Digital Quotation Management
- Supplier Comparison
- Commercial Evaluation
- Technical Evaluation
- Negotiation Tracking
- Supplier Award Support
- Complete Procurement Traceability

---

# Scope

The Supplier Quotation module includes

- Supplier Quotation Submission
- Commercial Offer Recording
- Technical Evaluation
- Price Comparison
- Currency Conversion
- Negotiation History
- Supplier Ranking
- Award Recommendation
- Quotation Revision
- Quotation Rejection

Out of Scope

- RFQ Creation
- Purchase Order
- Supplier Invoice
- Supplier Payment

---

# Supplier Quotation Architecture

```
RFQ

↓

Supplier Quotation

↓

Technical Evaluation

↓

Commercial Evaluation

↓

Negotiation

↓

Supplier Award

↓

Purchase Order
```

---

# Supplier Quotation Lifecycle

```
Draft

↓

Submitted

↓

Received

↓

Technical Review

↓

Commercial Review

↓

Negotiation

↓

Accepted

↓

Awarded

↓

Converted to Purchase Order

or

Rejected
```

Reference

Status_Lifecycle.md

---

# Quotation Sources

Quotations originate from

- RFQ Response
- Supplier Portal
- Manual Entry
- Email Import (Future)
- API Integration

---

# Quotation Header

Each quotation contains

## General Information

- Quotation Number
- RFQ Number
- Supplier
- Submission Date
- Currency
- Valid Until
- Delivery Terms
- Payment Terms
- Status

Reference

Currency.md

---

## Quotation Lines

Each quotation line contains

- Material Code
- Description
- Quantity
- Unit
- Unit Price
- Total Price
- Discount
- Tax
- Lead Time
- Delivery Date
- Brand
- Manufacturer
- Notes

Reference

Unit_Conversion.md

---

# Commercial Information

Supports

- Unit Price
- Total Amount
- Currency
- Discounts
- Freight Cost
- Insurance
- Incoterms
- Payment Terms
- Warranty Period

---

# Technical Information

Supports

- Material Specification
- Compliance Status
- Alternative Material
- Technical Notes
- Engineering Approval
- Certificates

---

# Currency Management

Supports

- TRY
- USD
- EUR
- GBP

Automatic comparison uses exchange rates from Finance.

Reference

Currency.md

---

# Quotation Comparison

Comparison criteria

- Total Price
- Unit Price
- Delivery Time
- Payment Terms
- Lead Time
- Warranty
- Technical Compliance
- Supplier Performance

Supports side-by-side comparison.

---

# Technical Evaluation

Evaluation includes

- Specification Match
- Material Quality
- Certification Check
- Sample Approval
- Engineering Comments

Possible results

- Approved
- Conditionally Approved
- Rejected

---

# Commercial Evaluation

Evaluation includes

- Best Price
- Total Cost
- Delivery Performance
- Payment Conditions
- Historical Pricing
- Supplier Rating

Automatic scoring is supported.

---

# Negotiation

Supports

- Counter Offer
- Price Revision
- Quantity Revision
- Delivery Revision
- Payment Negotiation

Every negotiation round is version controlled.

---

# Supplier Ranking

Automatic ranking based on

- Commercial Score
- Technical Score
- Delivery Score
- Supplier Performance

Overall score

```
0 - 100
```

Highest qualified supplier is recommended.

---

# Award Decision

Supports

- Single Supplier Award
- Multiple Supplier Award
- Split Quantity Award

Award generates Purchase Order.

---

# Attachments

Supports

- Supplier Proposal
- Price List
- Technical Datasheet
- Certificates
- Product Catalog
- Drawings

Reference

TASK-012_File_Upload.md

---

# Search

Supports

- Quotation Number
- RFQ Number
- Supplier
- Material
- Status
- Buyer
- Submission Date
- Currency

Reference

Search_Filtering.md

---

# Dashboard Information

Displays

- Quotations Received
- Pending Evaluation
- Supplier Ranking
- Average Response Time
- Price Savings
- Award Status

Reference

TASK-034_Purchasing_Dashboard.md

---

# Reports

Supports

- Supplier Quotation List
- Commercial Comparison
- Technical Evaluation Summary
- Supplier Ranking
- Price Trend
- Award Analysis

Reference

TASK-035_Purchasing_Reports.md

---

# API Endpoints

```
GET /api/v1/supplier-quotations

GET /api/v1/supplier-quotations/{id}

POST /api/v1/supplier-quotations

PUT /api/v1/supplier-quotations/{id}

DELETE /api/v1/supplier-quotations/{id}

POST /api/v1/supplier-quotations/{id}/submit

POST /api/v1/supplier-quotations/{id}/evaluate

POST /api/v1/supplier-quotations/{id}/accept

POST /api/v1/supplier-quotations/{id}/reject

POST /api/v1/supplier-quotations/{id}/award
```

Reference

Purchasing_API.md

---

# Validation Rules

The system validates

- RFQ exists.
- Supplier is approved.
- Currency is valid.
- Valid Until date is not expired.
- Unit Price > 0.
- Quantity > 0.
- Technical evaluation completed before award.
- Accepted quotations cannot be edited.
- Awarded quotations are read-only.

Reference

Validation_Rules.md

---

# Security

Supports

- Role-Based Access
- Buyer Authorization
- Company Isolation
- Plant Isolation
- Supplier Data Protection

Reference

Security.md

Permission_Model.md

---

# Audit

Records

- Quotation Created
- Quotation Submitted
- Technical Evaluation
- Commercial Evaluation
- Negotiation Round
- Quotation Accepted
- Quotation Rejected
- Supplier Awarded

Reference

Audit_Log.md

Logging.md

---

# Notifications

Supports

- New Quotation Received
- Evaluation Required
- Negotiation Requested
- Quotation Accepted
- Quotation Rejected
- Supplier Awarded
- Validity Expiring

Reference

Notification_System.md

---

# Events

Publishes

- SupplierQuotationCreated
- SupplierQuotationSubmitted
- SupplierQuotationEvaluated
- SupplierQuotationAccepted
- SupplierQuotationRejected
- SupplierAwarded

Reference

Event_Model.md

Integration_Events.md

---

# Mobile Support

Supports

- View Quotations
- Commercial Comparison
- Technical Review
- Approve Evaluation
- View Attachments

Quotation editing remains desktop-first.

Reference

Purchasing_Mobile.md

---

# Performance

Targets

- Quotation Save < 1 second
- Comparison < 2 seconds
- Supplier Ranking < 1 second
- Search < 300 ms
- Support 1,000,000+ quotations

Reference

Performance.md

Caching.md

---

# Naswood Examples

### Example 1

```
RFQ

↓

5 Timber Suppliers

↓

5 Quotations Received

↓

Commercial Comparison

↓

Supplier Score

↓

Best Supplier Selected
```

---

### Example 2

```
PUR Adhesive RFQ

↓

3 Chemical Suppliers

↓

Technical Approval

↓

Commercial Evaluation

↓

Purchase Order
```

---

### Example 3

```
Machine Spare Part

↓

Manufacturer Quote

↓

Distributor Quote

↓

Engineering Approval

↓

Award
```

---

# Acceptance Criteria

The Supplier Quotation module shall

- Manage supplier commercial offers.
- Support technical and commercial evaluations.
- Compare quotations side-by-side.
- Support negotiation history and quotation revisions.
- Recommend suppliers based on configurable scoring.
- Generate supplier awards for Purchase Orders.
- Publish procurement events.
- Follow all shared platform standards.

---

# Dependencies

Depends On

- TASK-026_Supplier.md
- TASK-027_Purchase_Request.md
- TASK-028_RFQ.md
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

TASK-030_Purchase_Order.md

TASK-034_Purchasing_Dashboard.md

TASK-035_Purchasing_Reports.md

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

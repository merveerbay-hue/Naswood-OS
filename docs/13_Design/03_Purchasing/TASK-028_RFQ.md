# TASK-028 — Request for Quotation (RFQ)

**Module:** Purchasing

**Category:** Procurement

**Version:** 1.0

**Status:** Approved

---

# Purpose

The Request for Quotation (RFQ) process enables Purchasing to request commercial quotations from one or more suppliers for approved procurement requirements.

The RFQ standardizes supplier communication, ensures fair supplier comparison and provides the basis for supplier selection before creating a Purchase Order.

RFQ is one of the core procurement processes within Naswood OS.

---

# Objectives

- Standardize Supplier Quotation Process
- Increase Procurement Transparency
- Support Competitive Purchasing
- Reduce Procurement Costs
- Improve Supplier Selection
- Maintain Complete Auditability
- Support AI-Assisted Procurement

---

# Scope

RFQ supports

- Material Procurement
- Service Procurement
- Multiple Suppliers
- Multiple Materials
- Quotation Collection
- Supplier Comparison
- Technical Evaluation
- Commercial Evaluation
- RFQ Revision
- RFQ Cancellation

RFQ does NOT

- Approve Suppliers
- Receive Inventory
- Create Financial Transactions
- Process Supplier Invoices

---

# Business Rules

- Every RFQ has a unique document number.
- Every RFQ originates from an approved Purchase Request.
- One RFQ may be sent to multiple suppliers.
- One supplier may respond with multiple quotation revisions.
- RFQ cannot be modified after publication.
- Closed RFQs are immutable.
- Every supplier response is recorded.

---

# RFQ Lifecycle

```
Draft

↓

Submitted

↓

Published

↓

Supplier Responses

↓

Evaluation

↓

Awarded

↓

Purchase Order

↓

Closed
```

Reference

Status_Lifecycle.md

---

# RFQ Types

Supports

| Type | Description |
|-------|-------------|
| Material RFQ | Inventory Materials |
| Service RFQ | External Services |
| Maintenance RFQ | Spare Parts |
| Equipment RFQ | Machinery |
| Project RFQ | Capital Investments |
| Framework RFQ | Long-Term Procurement |

---

# RFQ Header

Each RFQ contains

- RFQ Number
- Company
- Plant
- Purchasing Organization
- Buyer
- Currency
- Issue Date
- Closing Date
- Delivery Address
- Status
- Remarks

---

# RFQ Lines

Each RFQ line contains

- Material
- Description
- Quantity
- Unit of Measure
- Required Delivery Date
- Delivery Location
- Technical Specification
- Attachments

Reference

Measurement_System.md

Material.md

---

# Supplier Assignment

Supports

- Single Supplier
- Multiple Suppliers
- Preferred Supplier List
- Approved Vendor List (AVL)

One RFQ may be sent to any number of qualified suppliers.

---

# RFQ Distribution

Distribution methods

- Email
- Supplier Portal
- EDI (Future)
- API Integration

Every publication records

- Date
- User
- Supplier
- Delivery Status

---

# Supplier Response

Suppliers may provide

- Unit Price
- Total Price
- Currency
- Lead Time
- Delivery Date
- Incoterms
- Payment Terms
- Validity Date
- Attachments
- Technical Comments

Multiple quotation revisions are supported until RFQ closing.

---

# Commercial Evaluation

Purchasing compares

- Price
- Discount
- Freight
- Taxes
- Payment Terms
- Total Cost
- Delivery Time

Comparison supports automatic ranking.

---

# Technical Evaluation

Engineering and Production may evaluate

- Technical Compliance
- Material Specifications
- Certifications
- Sustainability
- Manufacturing Capability
- Sample Approval

Technical approval may be mandatory before award.

---

# Supplier Comparison

Supports comparison by

- Lowest Price
- Best Delivery Time
- Highest Quality Score
- Preferred Supplier
- AI Recommendation
- Weighted Scoring

Example

```
Supplier A

Price ★★★★★

Delivery ★★★★

Quality ★★★★

↓

Score 91

Supplier B

Price ★★★★

Delivery ★★★★★

Quality ★★★★★

↓

Score 95
```

---

# RFQ Award

Award methods

- Single Supplier
- Multiple Suppliers
- Split Quantity
- Framework Agreement

Award automatically generates Purchase Orders when configured.

Reference

TASK-030_Purchase_Order.md

---

# AI Integration

AI assists with

- Recommended Suppliers
- Price Prediction
- Delivery Risk Analysis
- Supplier Performance Analysis
- Best Value Recommendation
- Duplicate RFQ Detection
- Historical Price Comparison

Reference

AI_Copilot.md

---

# Purchasing Integration

Workflow

```
Purchase Request

↓

RFQ

↓

Supplier Quotation

↓

Evaluation

↓

Purchase Order
```

Reference

Purchasing_Architecture.md

---

# Attachments

Supports

- Technical Drawings
- CAD Files
- Specifications
- Quality Standards
- Material Lists
- Photos
- PDFs

Reference

File_Storage.md

---

# Mobile Workflow

```
Create RFQ

↓

Select Suppliers

↓

Publish

↓

Monitor Responses

↓

Evaluate

↓

Award Supplier
```

Reference

Purchasing_Mobile.md

---

# Validation Rules

Before publishing

The system validates

- Approved Purchase Request exists.
- RFQ contains at least one supplier.
- RFQ contains at least one material.
- Required delivery date is valid.
- Supplier is Active.
- Technical attachments exist (if required).
- Currency is valid.

Reference

Validation_Rules.md

---

# Dashboard

RFQ contributes to

- Open RFQs
- Pending Responses
- Awarded RFQs
- Supplier Response Rate
- Average RFQ Cycle Time
- Competitive Procurement Ratio

Reference

TASK-034_Purchasing_Dashboard.md

---

# Reports

Included in

- RFQ Report
- Supplier Response Report
- RFQ Cycle Time
- Supplier Participation
- RFQ Award Analysis
- Historical Price Comparison

Reference

TASK-035_Purchasing_Reports.md

---

# API

Primary endpoints

```
GET /rfqs

GET /rfqs/{id}

POST /rfqs

PUT /rfqs/{id}

POST /rfqs/{id}/publish

POST /rfqs/{id}/award

POST /rfqs/{id}/cancel

GET /rfqs/{id}/responses

GET /rfqs/{id}/comparison
```

Reference

Purchasing_API.md

---

# Events

Publishing

- RFQCreated
- RFQPublished
- RFQResponseReceived
- RFQClosed
- RFQAwarded
- RFQCancelled

Reference

Event_Model.md

Integration_Events.md

---

# Notifications

Supports

- RFQ Published
- Supplier Response Received
- RFQ Closing Reminder
- RFQ Awarded
- RFQ Cancelled

Reference

Notification_System.md

---

# Permissions

Typical permissions

- View RFQ
- Create RFQ
- Publish RFQ
- Evaluate Quotations
- Award RFQ
- Cancel RFQ

Reference

Permission_Model.md

---

# Audit

The following actions are audited

- RFQ Created
- RFQ Published
- Supplier Added
- Supplier Removed
- Response Received
- Award Decision
- RFQ Cancelled
- User Actions

Reference

Audit_Log.md

---

# Performance

The system shall

- Publish RFQs in less than 2 seconds.
- Support hundreds of suppliers per RFQ.
- Compare quotations in real time.
- Cache supplier master data.
- Support concurrent buyer operations.

Reference

Performance.md

Caching.md

Concurrency.md

---

# Security

RFQ follows

- Role-Based Authorization
- Purchasing Authorization
- Company Authorization
- Secure API Access
- Complete Audit Logging

Reference

Security.md

Permission_Model.md

---

# Naswood Implementation

Typical RFQ scenarios

## Timber Procurement

```
Production Requirement

↓

Approved PR

↓

RFQ

↓

5 Timber Suppliers

↓

Quotation Comparison

↓

Purchase Order
```

---

## Machinery Procurement

```
Investment Request

↓

Technical Specification

↓

International RFQ

↓

Technical Evaluation

↓

Commercial Evaluation

↓

Purchase Order
```

---

## Chemical Procurement

```
Production Planning

↓

Chemical Requirement

↓

Approved Suppliers

↓

RFQ

↓

Award

↓

Purchase Order
```

---

# Acceptance Criteria

The RFQ module shall

- Support multiple suppliers per RFQ.
- Support quotation revisions.
- Provide technical and commercial evaluation.
- Support AI-assisted supplier recommendations.
- Generate Purchase Orders after award.
- Support attachments and supplier communication.
- Publish procurement events.
- Follow all shared platform standards.

---

# Related Documents

Purchasing_Architecture.md

Purchasing_API.md

Purchasing_Mobile.md

TASK-026_Supplier.md

TASK-027_Purchase_Request.md

TASK-029_Supplier_Quotation.md

TASK-030_Purchase_Order.md

TASK-031_Goods_Receipt_PO.md

TASK-034_Purchasing_Dashboard.md

TASK-035_Purchasing_Reports.md

Approval_Workflow.md

Permission_Model.md

Validation_Rules.md

Material.md

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

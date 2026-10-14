# TASK-028 — RFQ (Request for Quotation)

**Module:** Purchasing

**Sprint:** Sprint 02 – Purchasing

**Category:** Strategic Sourcing

**Priority:** Critical

**Estimated Effort:** 8 Days

**Status:** Completed

---

# Purpose

Develop the Request for Quotation (RFQ) module for Naswood OS.

The RFQ module manages the competitive quotation process by allowing Purchasing to request commercial offers from one or multiple suppliers before creating a Purchase Order.

It ensures transparency, supplier competition, cost optimization and complete traceability throughout the procurement process.

---

# Objectives

- Standardize RFQ Process
- Competitive Supplier Evaluation
- Digital Supplier Communication
- Cost Optimization
- Approval Integration
- Complete Traceability
- Supplier Performance Improvement

---

# Scope

The RFQ module includes

- RFQ Creation
- Supplier Selection
- Supplier Invitation
- RFQ Distribution
- Quotation Collection
- Technical Evaluation
- Commercial Evaluation
- Supplier Award
- RFQ Revision
- RFQ Cancellation

Out of Scope

- Purchase Request
- Purchase Order
- Supplier Invoice
- Supplier Payment

---

# RFQ Architecture

```
Purchase Request

↓

RFQ

↓

Supplier Invitation

↓

Supplier Quotation

↓

Evaluation

↓

Supplier Award

↓

Purchase Order
```

---

# RFQ Lifecycle

```
Draft

↓

Submitted

↓

Published

↓

Waiting for Quotations

↓

Evaluation

↓

Awarded

↓

Converted to Purchase Order

↓

Closed

or

Cancelled
```

Reference

Status_Lifecycle.md

---

# RFQ Types

Supports

- Raw Material RFQ
- Service RFQ
- Equipment RFQ
- Packaging RFQ
- Spare Parts RFQ
- CAPEX RFQ
- Framework RFQ

---

# RFQ Sources

An RFQ may originate from

- Approved Purchase Request
- Manual Creation
- MRP Recommendation
- Project Procurement
- Maintenance Request
- Inventory Replenishment

---

# RFQ Header

Each RFQ contains

## General Information

- RFQ Number
- RFQ Date
- Company
- Plant
- Buyer
- Currency
- RFQ Type
- Closing Date
- Delivery Location
- Status

Reference

Currency.md

---

## RFQ Lines

Each RFQ line contains

- Material Code
- Description
- Quantity
- Unit
- Required Delivery Date
- Warehouse
- Technical Specification
- Estimated Price
- Notes

Reference

Unit_Conversion.md

---

# Supplier Invitation

Supports

- Single Supplier
- Multiple Suppliers
- Preferred Suppliers
- Approved Supplier List

Each supplier receives

- RFQ Document
- Technical Specification
- Attachments
- Submission Deadline

---

# Supplier Response

Suppliers may

- Submit Quotation
- Decline RFQ
- Request Clarification
- Submit Revised Quotation

Each submission records

- Submission Date
- Version
- Commercial Terms
- Attachments

---

# Commercial Evaluation

Comparison criteria

- Unit Price
- Total Price
- Currency
- Payment Terms
- Delivery Time
- Incoterms
- Warranty
- Discounts

Automatic ranking is supported.

---

# Technical Evaluation

Evaluation includes

- Technical Compliance
- Product Quality
- Certifications
- Alternative Materials
- Engineering Approval

Only technically approved quotations proceed to commercial evaluation.

---

# Supplier Award

Workflow

```
Commercial Evaluation

↓

Technical Approval

↓

Supplier Selection

↓

Award

↓

Purchase Order
```

Supports

- Single Award
- Multiple Awards
- Split Quantity Award

---

# RFQ Revision

Supports

- Revised Specifications
- Extended Closing Date
- Additional Suppliers
- Quantity Updates

All revisions are version controlled.

---

# Attachments

Supports

- Technical Drawings
- Specifications
- CAD Files
- Material Lists
- Project Documents
- Photos

Reference

TASK-012_File_Upload.md

---

# Search

Supports

- RFQ Number
- Buyer
- Supplier
- Material
- Status
- Closing Date
- Company
- Plant

Reference

Search_Filtering.md

---

# Dashboard Information

Displays

- Open RFQs
- Closing Soon
- Quotations Received
- Supplier Participation
- Average Response Time
- Award Rate

Reference

TASK-034_Purchasing_Dashboard.md

---

# Reports

Supports

- RFQ List
- Supplier Participation
- RFQ Cycle Time
- Award Summary
- Price Comparison
- Supplier Response Analysis

Reference

TASK-035_Purchasing_Reports.md

---

# API Endpoints

```
GET /api/v1/rfqs

GET /api/v1/rfqs/{id}

POST /api/v1/rfqs

PUT /api/v1/rfqs/{id}

DELETE /api/v1/rfqs/{id}

POST /api/v1/rfqs/{id}/publish

POST /api/v1/rfqs/{id}/cancel

POST /api/v1/rfqs/{id}/award

GET /api/v1/rfqs/{id}/responses
```

Reference

Purchasing_API.md

---

# Validation Rules

The system validates

- RFQ Number is unique.
- Purchase Request is approved.
- At least one RFQ line exists.
- At least one supplier is selected.
- Closing Date is valid.
- Required Delivery Date is valid.
- Currency is valid.
- Published RFQs cannot be deleted.
- Closed RFQs cannot be modified.

Reference

Validation_Rules.md

---

# Security

Supports

- Role-Based Access
- Buyer Authorization
- Company Isolation
- Plant Isolation
- Supplier Visibility Rules

Reference

Security.md

Permission_Model.md

---

# Audit

Records

- RFQ Created
- RFQ Updated
- RFQ Published
- Supplier Invited
- Quotation Received
- Supplier Awarded
- RFQ Cancelled
- RFQ Closed

Reference

Audit_Log.md

Logging.md

---

# Notifications

Supports

- RFQ Published
- Supplier Invitation Sent
- Quotation Received
- Closing Date Reminder
- Supplier Awarded
- RFQ Cancelled

Reference

Notification_System.md

---

# Events

Publishes

- RFQCreated
- RFQPublished
- SupplierInvited
- QuotationReceived
- RFQAwarded
- RFQCancelled
- RFQClosed

Reference

Event_Model.md

Integration_Events.md

---

# Mobile Support

Supports

- View RFQs
- Publish RFQs
- Monitor Responses
- Review Quotations
- Supplier Contact
- Attachment Viewing

Reference

Purchasing_Mobile.md

---

# Performance

Targets

- RFQ Creation < 1 second
- RFQ Search < 300 ms
- Supplier Invitation < 5 seconds
- Quotation Comparison < 2 seconds
- Support 500,000+ RFQs

Reference

Performance.md

Caching.md

---

# Naswood Examples

Example 1

```
Purchase Request

↓

100 m³ Spruce Timber

↓

RFQ

↓

5 Approved Timber Suppliers

↓

Commercial Comparison

↓

Lowest Qualified Supplier

↓

Purchase Order
```

---

Example 2

```
Maintenance

↓

CNC Spare Part

↓

RFQ

↓

Machine Manufacturer

+

Authorized Distributor

↓

Technical Evaluation

↓

Award
```

---

Example 3

```
Production

↓

PUR Adhesive

↓

RFQ

↓

3 Chemical Suppliers

↓

Supplier Comparison

↓

Best Value Selection
```

---

# Acceptance Criteria

The RFQ module shall

- Support competitive supplier quotations.
- Support single and multiple supplier invitations.
- Support technical and commercial evaluations.
- Support supplier award workflows.
- Maintain complete RFQ history and versioning.
- Integrate with Purchase Requests and Purchase Orders.
- Publish procurement events.
- Follow all shared platform standards.

---

# Dependencies

Depends On

- TASK-026_Supplier.md
- TASK-027_Purchase_Request.md
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

TASK-029_Supplier_Quotation.md

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

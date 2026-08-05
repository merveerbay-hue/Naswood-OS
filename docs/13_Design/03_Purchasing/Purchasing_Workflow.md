# Purchasing Workflow

**Module:** Purchasing

**Category:** Workflow

**Version:** 1.0

**Status:** Approved

---

# Purpose

The Purchasing Workflow defines the complete end-to-end procurement lifecycle within Naswood OS.

It standardizes every purchasing activity from the initial material request through supplier selection, purchasing, receiving and invoice processing while ensuring complete traceability, approval control and system integration.

The workflow integrates Purchasing with Inventory, Production, Quality, Finance and Analytics.

---

# Objectives

- Standardize Procurement Processes
- Ensure Approval Compliance
- Improve Supplier Collaboration
- Optimize Purchasing Costs
- Maintain Complete Traceability
- Support Digital Procurement
- Enable AI-Assisted Decision Making

---

# Procurement Lifecycle

```
Material Requirement

↓

Purchase Request

↓

Approval

↓

Request For Quotation

↓

Supplier Quotations

↓

Commercial Evaluation

↓

Technical Evaluation

↓

Award

↓

Purchase Order

↓

Supplier Confirmation

↓

Supplier Delivery

↓

Goods Receipt

↓

Quality Inspection

↓

Inventory

↓

Supplier Invoice

↓

Three-Way Matching

↓

Finance

↓

Payment

↓

Completed
```

---

# Workflow Overview

The Purchasing process consists of six major stages.

1. Procurement Planning
2. Supplier Selection
3. Procurement Execution
4. Receiving
5. Financial Validation
6. Performance Evaluation

---

# Stage 1 — Procurement Planning

The procurement process begins when a material or service requirement is identified.

Request sources

- Production Planning
- MRP
- Inventory Replenishment
- Maintenance
- Engineering
- Project Management
- Manual Request
- AI Recommendation

Workflow

```
Material Requirement

↓

Purchase Request

↓

Department Review
```

Reference

TASK-027_Purchase_Request.md

---

# Stage 2 — Purchase Request Approval

The Purchase Request is reviewed and approved according to company policy.

Typical workflow

```
Requester

↓

Department Manager

↓

Budget Owner

↓

Purchasing Manager

↓

Approved
```

Approval rules may depend on

- Cost
- Material Group
- Budget
- Department
- Plant

Reference

Approval_Workflow.md

---

# Stage 3 — Supplier Selection

After approval

```
Approved Purchase Request

↓

RFQ

↓

Supplier Invitation

↓

Supplier Quotations

↓

Commercial Evaluation

↓

Technical Evaluation

↓

Award
```

Supports

- Single Supplier
- Multiple Suppliers
- Competitive Bidding
- Framework Agreement

Reference

TASK-028_RFQ.md

TASK-029_Supplier_Quotation.md

---

# Supplier Evaluation

Evaluation considers

- Price
- Lead Time
- Quality
- Supplier Score
- Sustainability
- Previous Performance
- Financial Stability

AI may recommend the optimal supplier.

---

# Stage 4 — Purchase Order

After supplier selection

```
Supplier Award

↓

Purchase Order

↓

Approval

↓

Release

↓

Supplier Confirmation
```

Supplier may

- Accept
- Reject
- Request Changes
- Confirm Delivery Schedule

Reference

TASK-030_Purchase_Order.md

---

# Stage 5 — Goods Receipt

After delivery

```
Supplier Delivery

↓

Warehouse Receiving

↓

Barcode Scan

↓

Batch Registration

↓

Quality Inspection

↓

Inventory Receipt
```

Supports

- Partial Deliveries
- Batch Tracking
- Serial Numbers
- Warehouse Assignment

Reference

TASK-031_Goods_Receipt_PO.md

---

# Quality Inspection

If required

```
Goods Receipt

↓

Quality Hold

↓

Inspection

↓

Accepted

or

Rejected
```

Rejected materials create

```
Purchase Return
```

Reference

TASK-032_Purchase_Return.md

---

# Stage 6 — Invoice Processing

Supplier submits invoice

```
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

Matching compares

- Purchase Order
- Goods Receipt
- Supplier Invoice

Reference

TASK-033_Supplier_Invoice.md

---

# Procurement Exception Flows

## Purchase Return

```
Goods Receipt

↓

Quality Failure

↓

Purchase Return

↓

Supplier Credit Note

↓

Replacement (Optional)
```

Reference

TASK-032_Purchase_Return.md

---

## Partial Delivery

```
Purchase Order

↓

Delivery 1

↓

Delivery 2

↓

Delivery 3

↓

Purchase Order Closed
```

---

## Partial Invoice

```
Purchase Order

↓

Goods Receipt

↓

Invoice

↓

Remaining Balance
```

---

## Emergency Procurement

```
Emergency Requirement

↓

Emergency Purchase Request

↓

Fast Approval

↓

Purchase Order

↓

Immediate Delivery
```

---

# Inventory Integration

Inventory interactions

```
Purchase Order

↓

Goods Receipt

↓

Inventory

↓

Warehouse

↓

Production
```

Reference

02_Inventory

---

# Finance Integration

Financial interactions

```
Purchase Order

↓

Commitment

↓

Supplier Invoice

↓

Accounts Payable

↓

Payment
```

Reference

08_Finance

---

# Production Integration

Production may automatically generate

```
Production Planning

↓

Material Requirement

↓

Purchase Request
```

Reference

05_Production

---

# Maintenance Integration

Maintenance may initiate

```
Maintenance Order

↓

Purchase Request

↓

Purchase Order

↓

Goods Receipt
```

Reference

07_Maintenance

---

# Quality Integration

Quality participates in

- Incoming Inspection
- Supplier Evaluation
- Purchase Returns
- Supplier Audits

Reference

06_Quality

---

# Supplier Lifecycle

```
Supplier Registration

↓

Qualification

↓

Approval

↓

RFQ

↓

Purchase Order

↓

Delivery

↓

Performance Evaluation

↓

Requalification
```

Reference

TASK-026_Supplier.md

---

# AI Workflow

AI supports

```
Purchase Request

↓

Supplier Recommendation

↓

Price Prediction

↓

Risk Analysis

↓

Award Recommendation

↓

Delivery Prediction

↓

Supplier Performance Analysis
```

Reference

AI_Copilot.md

---

# Mobile Workflow

Purchasing Mobile supports

```
Purchase Request

↓

Mobile Approval

↓

Purchase Order Review

↓

Goods Receipt

↓

Purchase Return

↓

Dashboard
```

Reference

Purchasing_Mobile.md

---

# Workflow Notifications

Automatic notifications

- Purchase Request Submitted
- Approval Required
- RFQ Published
- Supplier Response Received
- Purchase Order Released
- Delivery Delayed
- Goods Received
- Quality Inspection Required
- Invoice Awaiting Approval
- Payment Ready

Reference

Notification_System.md

---

# Workflow Events

Purchasing publishes

- PurchaseRequestCreated
- RFQPublished
- SupplierQuotationReceived
- PurchaseOrderReleased
- GoodsReceiptPosted
- PurchaseReturnCreated
- SupplierInvoicePosted
- PaymentRequested

Reference

Event_Model.md

Integration_Events.md

---

# Performance Targets

Target KPIs

| KPI | Target |
|------|--------|
| Purchase Request Approval | < 24 Hours |
| RFQ Cycle | < 5 Days |
| Purchase Order Approval | < 24 Hours |
| Goods Receipt Posting | < 10 Minutes |
| Three-Way Matching | < 2 Minutes |
| Invoice Approval | < 24 Hours |

Reference

Performance.md

---

# Security

Workflow enforces

- Role-Based Authorization
- Department Authorization
- Purchasing Authorization
- Budget Authorization
- Complete Audit Logging

Reference

Security.md

Permission_Model.md

---

# Audit

Every workflow step records

- User
- Timestamp
- Status Change
- Approval Decision
- Supplier Actions
- Financial Actions
- Comments

Reference

Audit_Log.md

---

# Success Metrics

The Purchasing workflow measures

- Procurement Cycle Time
- Supplier Response Time
- Supplier On-Time Delivery
- Procurement Cost Savings
- RFQ Participation Rate
- Three-Way Match Success Rate
- Supplier Quality Score
- Purchase Return Rate

---

# Naswood Procurement Workflow

```
Production Planning

↓

MRP

↓

Purchase Request

↓

Approval

↓

RFQ

↓

Approved Suppliers

↓

Supplier Quotations

↓

Commercial & Technical Evaluation

↓

Purchase Order

↓

Supplier Delivery

↓

Warehouse Receiving

↓

Quality Inspection

↓

Inventory

↓

Production Consumption

↓

Supplier Invoice

↓

Three-Way Matching

↓

Finance

↓

Payment

↓

Supplier Performance Evaluation
```

This workflow ensures complete traceability from material demand to supplier payment while integrating Purchasing, Inventory, Production, Quality and Finance into a single digital procurement process.

---

# Acceptance Criteria

The Purchasing Workflow shall

- Cover the complete procurement lifecycle.
- Support configurable approval workflows.
- Integrate with Inventory, Production, Quality and Finance.
- Support RFQ and supplier comparison.
- Support Purchase Returns and Three-Way Matching.
- Support AI-assisted procurement.
- Publish procurement events.
- Follow all shared platform standards.

---

# Related Documents

README.md

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

TASK-033_Supplier_Invoice.md

TASK-034_Purchasing_Dashboard.md

TASK-035_Purchasing_Reports.md

Approval_Workflow.md

Permission_Model.md

Security.md

Audit_Log.md

Performance.md

Notification_System.md

Event_Model.md

Integration_Events.md

AI_Copilot.md

# Sales Workflow

**Module:** Sales

**Version:** 1.0

**Status:** Approved

**Owner:** Naswood ERP Architecture Team

---

# Purpose

The Sales Workflow defines the complete business process of the Sales module within Naswood ERP.

It standardizes every commercial activity from the first customer contact to the final customer payment while integrating CRM, Inventory, Production, Logistics and Finance.

The workflow ensures

- Standardized Processes
- End-to-End Traceability
- Approval Governance
- Manufacturing Integration
- Financial Integration
- Customer Satisfaction

---

# End-to-End Workflow

```
Lead

↓

Qualification

↓

Opportunity

↓

Quotation

↓

Internal Approval

↓

Quotation Sent

↓

Customer Negotiation

↓

Accepted

↓

Sales Order

↓

Inventory Check

↓

Production Planning

↓

Manufacturing

↓

Shipment

↓

Delivery

↓

Customer Invoice

↓

Payment

↓

Completed
```

---

# Workflow Overview

```
CRM

↓

Lead

↓

Opportunity

↓

Quotation

↓

Sales

↓

Production

↓

Inventory

↓

Logistics

↓

Finance
```

---

# Stage 1 — Lead Management

### Trigger

Potential customer identified.

Sources

- Website
- Dealer
- Fair
- Phone Call
- Email
- Social Media
- Referral

Workflow

```
New Lead

↓

Qualification

↓

Assigned Salesperson

↓

Customer Contact

↓

Decision
```

Possible Outcomes

```
Qualified

↓

Opportunity
```

or

```
Rejected

↓

Closed
```

Reference

TASK-037_Lead.md

---

# Stage 2 — Opportunity

Purpose

Evaluate commercial potential.

Workflow

```
Opportunity

↓

Requirement Analysis

↓

Customer Meeting

↓

Budget

↓

Competition Analysis

↓

Proposal Decision
```

Possible Outcomes

```
Prepare Quotation
```

or

```
Lost Opportunity
```

Reference

TASK-038_Opportunity.md

---

# Stage 3 — Quotation

Workflow

```
Quotation Draft

↓

Pricing

↓

Discount

↓

Internal Approval

↓

Customer

↓

Negotiation
```

Possible Outcomes

```
Accepted

↓

Sales Order
```

```
Rejected

↓

Closed
```

```
Revision

↓

New Version
```

Reference

TASK-039_Quotation.md

---

# Quotation Approval Workflow

```
Sales Representative

↓

Sales Manager

↓

Commercial Director

↓

Approved
```

Approval depends on

- Discount %
- Margin
- Project Size
- Customer Type

---

# Stage 4 — Sales Order

Workflow

```
Accepted Quotation

↓

Sales Order

↓

Credit Check

↓

Inventory Check
```

Decision

```
Stock Available ?

YES

↓

Reserve Inventory

↓

Shipment
```

or

```
NO

↓

Production Request

↓

Production Planning
```

Reference

TASK-040_Sales_Order.md

---

# Credit Validation

Before approval

System checks

- Credit Limit
- Outstanding Balance
- Overdue Invoices
- Payment History

If validation fails

```
Finance Approval Required
```

---

# Stage 5 — Production

For manufactured products

```
Sales Order

↓

BOM

↓

Routing

↓

Capacity Planning

↓

Production Order

↓

Manufacturing

↓

Finished Goods
```

Reference

Production Module

---

# Stage 6 — Inventory Reservation

Workflow

```
Sales Order

↓

Available Inventory

↓

Warehouse Selection

↓

Batch Selection

↓

Reservation
```

Supports

- FIFO
- FEFO
- Manual Allocation

Reference

Inventory Module

---

# Stage 7 — Shipment

Workflow

```
Reserved Products

↓

Picking

↓

Packing

↓

Loading

↓

Shipment

↓

Transportation
```

Supports

- Partial Shipment
- Multiple Shipments
- Export Shipment

Reference

TASK-041_Shipment.md

---

# Stage 8 — Delivery

Workflow

```
Shipment

↓

Customer Site

↓

Verification

↓

Customer Signature

↓

Delivery Completed
```

Supports

- Partial Delivery
- Delivery Exception
- Damage Report
- GPS Verification

Reference

TASK-042_Delivery.md

---

# Stage 9 — Customer Invoice

Workflow

```
Completed Delivery

↓

Invoice Generation

↓

Tax Validation

↓

Invoice Approval

↓

Customer Invoice
```

Reference

TASK-043_Customer_Invoice.md

---

# Stage 10 — Payment

Workflow

```
Invoice

↓

Accounts Receivable

↓

Customer Payment

↓

Financial Posting

↓

Completed
```

Reference

Finance Module

---

# Return Workflow

Customer Return

```
Customer Complaint

↓

Return Request

↓

Approval

↓

Goods Inspection

↓

Inventory Return

↓

Credit Note
```

Reference

Sales Return Module

---

# Revision Workflow

Quotation Revision

```
Version 1

↓

Revision

↓

Version 2

↓

Revision

↓

Version 3
```

Sales Order Revision

```
Order

↓

Change Request

↓

Approval

↓

New Revision
```

---

# Cancellation Workflow

Supports

Quotation

Sales Order

Shipment

Reasons

- Customer Cancellation
- Price Disagreement
- Credit Issue
- Production Issue
- Material Shortage

All cancellations require reason codes.

---

# Notification Workflow

Notifications generated during workflow

```
Lead Assigned

↓

Opportunity Created

↓

Quotation Approval

↓

Quotation Accepted

↓

Sales Order Approved

↓

Production Started

↓

Shipment Ready

↓

Delivery Completed

↓

Invoice Issued

↓

Payment Received
```

Reference

Notification_System.md

---

# Integration Workflow

CRM

```
Lead

↓

Opportunity

↓

Customer
```

Inventory

```
Sales Order

↓

Reservation

↓

Goods Issue
```

Production

```
Sales Order

↓

Production Planning

↓

Manufacturing
```

Finance

```
Invoice

↓

Receivable

↓

Payment
```

---

# Exception Workflow

Supports

- Credit Hold
- Inventory Shortage
- Production Delay
- Shipment Delay
- Delivery Failure
- Invoice Rejection

Every exception creates

- Notification
- Audit Record
- Workflow Task

---

# Status Transitions

Lead

```
New

↓

Qualified

↓

Converted

↓

Closed
```

Opportunity

```
Open

↓

Negotiation

↓

Won

↓

Lost
```

Quotation

```
Draft

↓

Submitted

↓

Approved

↓

Sent

↓

Accepted

↓

Closed
```

Sales Order

```
Draft

↓

Approved

↓

Released

↓

Production

↓

Shipment

↓

Completed
```

Shipment

```
Planned

↓

Picking

↓

Loaded

↓

In Transit

↓

Delivered
```

Delivery

```
Planned

↓

Completed

↓

Accepted
```

Invoice

```
Draft

↓

Issued

↓

Sent

↓

Paid
```

---

# AI Workflow

AI assists

- Lead Scoring
- Opportunity Prioritization
- Pricing Suggestions
- Revenue Forecast
- Upselling
- Customer Risk Detection

Reference

AI_Copilot.md

---

# Audit Workflow

Every workflow action records

- User
- Timestamp
- Status
- Previous Value
- New Value
- Company
- Plant
- Device

Reference

Audit_Log.md

---

# Performance Targets

| Workflow Step | Target |
|---------------|--------|
| Lead Creation | <1 sec |
| Opportunity Save | <1 sec |
| Quotation Approval | <2 sec |
| Sales Order Creation | <1 sec |
| Shipment Creation | <2 sec |
| Delivery Confirmation | <1 sec |
| Invoice Generation | <2 sec |

---

# Workflow Summary

```
CRM

↓

Lead

↓

Opportunity

↓

Quotation

↓

Sales Order

↓

Inventory

↓

Production

↓

Shipment

↓

Delivery

↓

Customer Invoice

↓

Payment

↓

Completed
```

---

# Related Documents

Sales_Architecture.md

Sales_API.md

Sales_Mobile.md

Sales_Dashboard.md

Sales_Reports.md

TASK-036_Customer.md

TASK-037_Lead.md

TASK-038_Opportunity.md

TASK-039_Quotation.md

TASK-040_Sales_Order.md

TASK-041_Shipment.md

TASK-042_Delivery.md

TASK-043_Customer_Invoice.md

TASK-044_Sales_Dashboard.md

TASK-045_Sales_Reports.md

Approval_Workflow.md

Notification_System.md

Audit_Log.md

Permission_Model.md

Security.md

Integration_Events.md

Performance.md

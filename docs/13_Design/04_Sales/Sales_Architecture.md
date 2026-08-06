# Sales Architecture

**Module:** Sales

**Version:** 1.0

**Status:** Approved

**Owner:** Naswood ERP Architecture Team

---

# Purpose

The Sales module manages the complete commercial lifecycle from identifying a potential customer to collecting payment after product delivery.

It is the central business module connecting CRM, Production, Inventory, Logistics and Finance.

The architecture is designed to support:

- B2B Sales
- Dealer Network
- Export Sales
- Project Sales
- Manufacturing Sales
- Stock Sales

---

# Architecture Principles

The Sales module follows the Naswood ERP architectural standards.

- Domain Driven Design (DDD)
- Event Driven Architecture
- REST API
- CQRS Ready
- Multi Company
- Multi Plant
- Multi Warehouse
- Multi Currency
- Role Based Security
- Audit Logging
- Mobile First
- API First

Reference

01_Standards.md

---

# High Level Architecture

```
                     CRM

                      │

      ┌───────────────┼────────────────┐

      │               │                │

  Lead Management  Customer       Activities

      │

      ▼

 Opportunity

      │

      ▼

 Quotation

      │

      ▼

 Sales Order

      │

      ├─────────────┐

      ▼             ▼

 Inventory      Production

      │             │

      └──────┬──────┘

             ▼

         Shipment

             ▼

         Delivery

             ▼

     Customer Invoice

             ▼

           Finance
```

---

# Module Structure

```
Sales

├── Customer

├── Quotation

├── Sales Order

├── Shipment

├── Delivery

├── Customer Invoice

├── Dashboard

└── Reports
```

Lead, Opportunity, activities and interactions are owned by CRM and referenced
through versioned contracts.

---

# Product Contract

Sales does not own Product definition or capability behavior.

Every Quotation and Sales Order line stores:

- Product ID
- Product Revision ID
- Capability Profile ID

Sales validates that Sales Mode is `OPTIONAL` or `ENABLED` when the line is
created or revised. Existing released documents retain the profile they
validated; later profile activation does not rewrite historical eligibility.

---

# Business Flow

```
Lead

↓

Qualification

↓

Opportunity

↓

Quotation

↓

Approval

↓

Customer Acceptance

↓

Sales Order

↓

Inventory Reservation

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

# Domain Relationships

```
Customer

│

├── Leads

├── Opportunities

├── Quotations

├── Sales Orders

├── Deliveries

├── Invoices

└── Activities
```

---

# Sales Order Architecture

```
Customer

↓

Quotation

↓

Sales Order

↓

Inventory Check

↓

Production Required ?

↓

YES

↓

Production Order

↓

Manufacturing

↓

Shipment

↓

Delivery

↓

Invoice
```

---

# Manufacturing Integration

Manufacturing products

```
Sales Order

↓

BOM

↓

Routing

↓

Production Planning

↓

Production Order

↓

Manufacturing

↓

Finished Goods

↓

Shipment
```

Stock products

```
Sales Order

↓

Inventory Reservation

↓

Shipment
```

---

# Inventory Integration

The Sales module communicates with Inventory for

- Available Stock
- Reserved Stock
- Batch Selection
- Serial Numbers
- Warehouse Selection
- Goods Issue
- Stock History

Inventory updates occur after

- Shipment
- Delivery
- Returns

---

# Purchasing Integration

Sales communicates with Purchasing when

- Make-to-Order products require purchasing.
- Customer-specific materials must be procured.
- MRP generates purchase requirements.

```
Sales Order

↓

MRP

↓

Purchase Request

↓

Purchase Order
```

---

# Production Integration

Sales creates production demand.

```
Sales Order

↓

Production Planning

↓

Capacity Planning

↓

Production Order

↓

Manufacturing

↓

Finished Goods
```

---

# Logistics Integration

```
Sales Order

↓

Shipment

↓

Loading

↓

Transportation

↓

Delivery

↓

Proof of Delivery
```

Supports

- Partial Shipment
- Export Shipment
- Multiple Deliveries

---

# Finance Integration

```
Delivery

↓

Customer Invoice

↓

Accounts Receivable

↓

Payment

↓

Financial Reporting
```

Supports

- Credit Limit Check
- Payment Terms
- Tax Calculation
- Currency Conversion

---

# CRM Integration

```
Lead

↓

Opportunity

↓

Customer

↓

Activities

↓

Sales
```

CRM provides

- Customer History
- Communication History
- Visit Records
- Sales Activities

---

# Workflow Integration

Approval Workflow

```
Quotation

↓

Sales Manager

↓

Commercial Director

↓

Approved
```

Order Workflow

```
Sales Order

↓

Inventory

↓

Production

↓

Shipment

↓

Delivery
```

---

# Security Architecture

Supports

- Role Based Access
- Sales Territory Security
- Company Isolation
- Plant Isolation
- Customer Visibility Rules
- Price Authorization
- Discount Authorization

Reference

Permission_Model.md

---

# Event Driven Architecture

Published Events

```
LeadCreated

OpportunityCreated

QuotationCreated

QuotationApproved

SalesOrderCreated

InventoryReserved

ProductionRequested

ShipmentCreated

DeliveryCompleted

CustomerInvoiceCreated
```

Consumed Events

```
InventoryReserved

ProductionCompleted

ShipmentDispatched

DeliveryConfirmed

InvoicePaid

CustomerUpdated
```

Reference

Integration_Events.md

---

# API Architecture

```
Client

↓

REST API

↓

Application Layer

↓

Domain Layer

↓

Repository

↓

Database
```

Supports

- REST
- JSON
- JWT Authentication
- Pagination
- Filtering
- Sorting
- Versioning

Reference

Sales_API.md

---

# Mobile Architecture

Mobile supports

- Customer Management
- Lead Management
- Opportunity Tracking
- Quotation Approval
- Sales Order View
- Shipment Tracking
- Delivery Confirmation
- Customer Signature
- Dashboard

Offline support

- Customer Visits
- Delivery
- Signature Capture

Reference

Sales_Mobile.md

---

# Dashboard Architecture

```
Sales KPIs

↓

Pipeline

↓

Revenue

↓

Orders

↓

Deliveries

↓

Invoices

↓

Forecast

↓

AI Insights
```

Reference

Sales_Dashboard.md

---

# Reporting Architecture

Supports

Operational Reports

- Leads
- Opportunities
- Quotations
- Orders
- Shipments
- Deliveries
- Invoices

Management Reports

- Revenue
- Salesperson Performance
- Customer Analysis
- Product Analysis
- Regional Sales

Executive Reports

- Sales KPIs
- Forecast
- Pipeline
- Profitability

Reference

Sales_Reports.md

---

# AI Integration

AI services provide

- Sales Forecast
- Customer Churn Prediction
- Opportunity Scoring
- Dynamic Pricing
- Upselling Suggestions
- Cross-selling Recommendations
- Revenue Prediction

Reference

AI_Copilot.md

---

# Notification Architecture

Notifications generated by

- New Lead
- Opportunity Assignment
- Quotation Approval
- Order Approval
- Shipment Delay
- Delivery Completed
- Invoice Due
- Customer Payment

Reference

Notification_System.md

---

# Audit Architecture

Every business action records

- User
- Timestamp
- Previous Value
- New Value
- IP Address
- Device
- Company
- Plant

Reference

Audit_Log.md

---

# Performance Targets

| Function | Target |
|-----------|---------|
| Customer Search | <300 ms |
| Opportunity Search | <300 ms |
| Sales Order Creation | <1 second |
| Dashboard Load | <2 seconds |
| Report Generation | <5 seconds |
| Invoice Creation | <2 seconds |

---

# Scalability

Supports

- Unlimited Customers
- Unlimited Sales Orders
- Unlimited Quotations
- Unlimited Companies
- Unlimited Plants
- Multiple Countries
- Multiple Languages
- Multiple Time Zones

---

# Naswood Manufacturing Example

```
Customer

↓

Quotation

↓

CLT Building

↓

Sales Order

↓

MRP

↓

Production

↓

Inventory

↓

Shipment

↓

Construction Site Delivery

↓

Customer Invoice

↓

Payment
```

---

# Related Documents

Sales_Workflow.md

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

01_Standards.md

Security.md

Permission_Model.md

Notification_System.md

Audit_Log.md

Integration_Events.md

Performance.md

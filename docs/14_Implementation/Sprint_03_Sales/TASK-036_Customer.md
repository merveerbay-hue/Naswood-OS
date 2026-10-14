# TASK-036 — Customer

**Module:** Sales

**Sprint:** Sprint 03 – Sales

**Category:** Master Data

**Priority:** Critical

**Estimated Effort:** 8 Days

**Status:** Completed

---

# Purpose

Develop the Customer Master module for Naswood OS.

The Customer module serves as the single source of truth for all customer information used throughout Sales, Finance, Production, Logistics and CRM.

Every sales transaction references an approved customer record from this module.

---

# Objectives

- Centralized Customer Management
- Customer Qualification
- Multi-Company Support
- Multi-Currency Sales
- Customer Performance Monitoring
- Credit Control
- Complete Customer Traceability

---

# Scope

The Customer module includes

- Customer Registration
- Customer Profile
- Customer Classification
- Customer Qualification
- Contact Management
- Billing & Shipping Addresses
- Credit Management
- Tax Information
- Customer Documents
- Customer Status Management

Out of Scope

- Quotations
- Sales Orders
- Deliveries
- Customer Invoices
- CRM Activities

These are managed by their respective modules.

---

# Customer Architecture

```
Sales

↓

Customer Service

↓

Validation

↓

Database

↓

Event Bus

↓

Other Modules
```

---

# Customer Lifecycle

```
Lead

↓

Prospect

↓

Pending Approval

↓

Approved

↓

Active

↓

Blocked

↓

Inactive

↓

Archived
```

Reference

Status_Lifecycle.md

---

# Customer Master Data

Every customer contains

## General Information

- Customer Code
- Customer Name
- Short Name
- Customer Type
- Customer Group
- Status

---

## Legal Information

- Tax Number
- Tax Office
- Registration Number
- Country
- Legal Entity Type

---

## Addresses

Supports

- Billing Address
- Shipping Address
- Headquarters
- Branch Address

Each address stores

- Country
- City
- District
- Postal Code
- Full Address
- GPS Coordinates (Optional)

---

## Contact Information

- Primary Contact
- Sales Contact
- Finance Contact
- Phone
- Mobile
- Email
- Website

---

## Financial Information

- Currency
- Payment Terms
- Credit Limit
- Credit Used
- Available Credit
- Customer Risk Level

Reference

Currency.md

---

## Sales Information

- Assigned Sales Representative
- Sales Region
- Sales Channel
- Customer Segment
- Price List
- Discount Group
- Delivery Terms

---

## Customer Classification

Supports

- Distributor
- Dealer
- Retail Customer
- Contractor
- Construction Company
- Architect
- Government
- Export Customer
- OEM
- End User

Multiple classifications are allowed.

---

# Customer Performance

KPIs

- Total Sales
- Sales Frequency
- Average Order Value
- Payment Performance
- Delivery Performance
- Return Rate
- Customer Lifetime Value

Overall Customer Score

```
0 - 100
```

Calculated automatically.

---

# Customer Status

Supports

- Draft
- Pending Approval
- Approved
- Active
- Credit Hold
- Blocked
- Inactive
- Archived

Only **Active** customers may receive quotations and sales orders.

---

# Multi Company

Supports

- Global Customer
- Company Customer
- Plant Customer

---

# Multi Currency

Supports

- TRY
- USD
- EUR
- GBP

Exchange rates are managed by Finance.

Reference

Currency.md

---

# Credit Control

Supports

- Credit Limit
- Credit Hold
- Risk Classification
- Outstanding Balance
- Payment History
- Automatic Credit Check

Orders exceeding credit limits require approval.

---

# Customer Documents

Supports

- Contracts
- Tax Certificates
- Trade Registry Documents
- Credit Agreements
- Technical Specifications
- Customer Drawings

Reference

TASK-012_File_Upload.md

---

# Search

Supports

- Customer Code
- Customer Name
- Tax Number
- Contact
- Sales Representative
- Country
- Customer Type
- Status

Reference

Search_Filtering.md

---

# Dashboard Information

Displays

- Total Customers
- Active Customers
- New Customers
- Customers on Credit Hold
- Customer Sales Volume
- Customer Performance

Reference

Sales Dashboard

---

# Reports

Supports

- Customer List
- Customer Performance
- Customer Sales
- Customer Credit Status
- Customer Aging
- Customer Classification

Reference

Sales Reports

---

# API Endpoints

```
GET /api/v1/customers

GET /api/v1/customers/{id}

POST /api/v1/customers

PUT /api/v1/customers/{id}

DELETE /api/v1/customers/{id}

POST /api/v1/customers/{id}/approve

POST /api/v1/customers/{id}/activate

POST /api/v1/customers/{id}/block

GET /api/v1/customers/search
```

Reference

Sales_API.md

---

# Validation Rules

The system validates

- Customer Code is unique.
- Tax Number is unique.
- Email format is valid.
- Currency exists.
- Credit Limit ≥ 0.
- Mandatory fields are completed.
- Approved customers only may become Active.

Reference

Validation_Rules.md

---

# Security

Supports

- Role-Based Access
- Company Isolation
- Plant Isolation
- Financial Data Protection
- Customer Credit Protection

Reference

Security.md

Permission_Model.md

---

# Audit

Records

- Customer Created
- Customer Updated
- Customer Approved
- Customer Activated
- Customer Blocked
- Credit Limit Changed
- Sales Representative Changed

Reference

Audit_Log.md

Logging.md

---

# Notifications

Supports

- Customer Approval Required
- Credit Limit Exceeded
- Customer Blocked
- Customer Activated
- Customer Credit Warning

Reference

Notification_System.md

---

# Events

Publishes

- CustomerCreated
- CustomerUpdated
- CustomerApproved
- CustomerActivated
- CustomerBlocked
- CustomerArchived

Reference

Event_Model.md

Integration_Events.md

---

# Mobile Support

Supports

- Customer Lookup
- Contact Information
- Credit Status
- Customer Location
- Customer Documents

Customer creation remains desktop-first.

Reference

Sales_Mobile.md

---

# Performance

Targets

- Customer Search < 300 ms
- Customer Save < 1 second
- Customer Lookup < 150 ms
- Support 500,000+ customers

Reference

Performance.md

Caching.md

---

# Naswood Examples

Customer Types

- Timber Dealer
- Construction Company
- Architectural Office
- CLT Manufacturer
- Furniture Factory
- Building Materials Distributor
- Export Customer
- Government Agency

Example

```
Customer

↓

ABC Construction

↓

Contractor

↓

EUR

↓

Credit Limit

500,000 EUR

↓

Active

↓

Customer Score

94
```

---

# Acceptance Criteria

The Customer module shall

- Maintain centralized customer master data.
- Support customer approval and lifecycle management.
- Support multiple companies and currencies.
- Track customer credit and performance.
- Store customer documents securely.
- Integrate with Sales, Finance and CRM modules.
- Publish customer lifecycle events.
- Follow all shared platform standards.

---

# Dependencies

Depends On

- TASK-012_File_Upload.md
- TASK-013_Audit_Log.md
- TASK-014_Settings.md
- Sales_API.md
- Validation_Rules.md

---

# Related Documents

Sales_Architecture.md

Sales_API.md

Sales_Workflow.md

TASK-037_Quotation.md

TASK-038_Sales_Order.md

TASK-039_Delivery.md

TASK-040_Customer_Invoice.md

TASK-041_Customer_Payment.md

TASK-042_Sales_Dashboard.md

TASK-043_Sales_Reports.md

Security.md

Permission_Model.md

Validation_Rules.md

Currency.md

Performance.md

Caching.md

Search_Filtering.md

Audit_Log.md

Logging.md

Notification_System.md

Event_Model.md

Integration_Events.md

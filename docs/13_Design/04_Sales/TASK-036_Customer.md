> **UX authority note:** `+ New` / Create wireframes in this TASK are historical. Live CTAs: [`Sales_Screens.md`](./Sales_Screens.md) · [`Screen_Types.md`](../Common/Screen_Types.md) § Create matrix · Process_Screens.

# TASK-036 — Customer

**Module:** Sales

**Document Type:** Design Specification

**Version:** 1.0

**Status:** Approved

**Owner:** Sales Product Team

---

# Purpose

The Customer module is the master data component of the Sales domain.

It stores every company, dealer, distributor, architect, contractor, builder, government institution and export customer that conducts business with Naswood.

The Customer module is shared across

- CRM
- Sales
- Production
- Inventory
- Purchasing
- Logistics
- Finance
- Service

The customer record acts as the single source of truth (SSOT) for every commercial transaction.

---

# Design Goals

The module is designed to

- Minimize duplicate customer records
- Simplify customer onboarding
- Support B2B manufacturing
- Support export customers
- Support dealer management
- Support multiple delivery addresses
- Support multiple contacts
- Provide 360° customer visibility

---

# Screen Layout

```
────────────────────────────────────────────────────────────

Customer List

────────────────────────────────────────────────────────────

Search

Filters

Customer Grid

────────────────────────────────────────────────────────────

**Add customer** (Explorer — master)

Edit

Delete

Export

Import

────────────────────────────────────────────────────────────
```

Selecting a customer opens the Customer Detail screen.

---

# Customer Detail Layout

```
────────────────────────────────────────────────────────────

Customer Header

────────────────────────────────────────────────────────────

General

Contacts

Addresses

Financial

Sales

Activities

Documents

History

Notes

────────────────────────────────────────────────────────────
```

---

# Customer Header

Displays

- Customer Code
- Customer Name
- Logo
- Status
- Customer Group
- Customer Type
- Assigned Salesperson
- Company
- Plant

Actions

- Edit
- Archive
- Duplicate
- Convert to Dealer
- View Timeline

---

# Tab — General

Stores

## Identification

- Customer Code
- Legal Name
- Commercial Name
- Short Name

## Classification

- Customer Type
- Customer Group
- Industry
- Business Segment
- Dealer Status
- Export Customer

## Tax Information

- Tax Office
- Tax Number
- VAT Number
- Registration Number

## Corporate Information

- Website
- Language
- Currency
- Time Zone

---

# Customer Types

Supports

- Dealer
- Distributor
- Contractor
- Construction Company
- Architect
- Engineer
- Government
- Export Customer
- Retail Customer
- Internal Customer

---

# Customer Status

```
Prospect

↓

Active

↓

On Hold

↓

Blocked

↓

Inactive

↓

Archived
```

---

# Tab — Contacts

Supports unlimited contacts.

Each contact stores

- Name
- Department
- Position
- Mobile Phone
- Office Phone
- Email
- Preferred Language

Flags

- Primary Contact
- Invoice Contact
- Technical Contact
- Purchasing Contact

---

# Tab — Addresses

Supports multiple addresses.

Types

- Headquarters
- Invoice Address
- Delivery Address
- Factory
- Warehouse
- Branch Office

Each address stores

- Country
- City
- District
- Postal Code
- GPS Coordinates

Supports

- Google Maps
- OpenStreetMap

---

# Tab — Financial

Displays

- Payment Terms
- Credit Limit
- Risk Score
- Currency
- Tax Category
- Bank Accounts
- Outstanding Balance
- Available Credit

Reference

Finance Module

---

# Tab — Sales

Displays

- Salesperson
- Sales Region
- Dealer
- Customer Category
- Pricing Group
- Discount Group
- Incoterms
- Delivery Terms

Supports

- Customer Price List
- Special Discounts
- Project Pricing

---

# Tab — Activities

Displays chronological timeline.

Activities

- Phone Calls
- Meetings
- Emails
- Visits
- Quotations
- Sales Orders
- Deliveries
- Invoices

Supports filtering.

---

# Tab — Documents

Stores

- Contracts
- NDA
- Company Registration
- Tax Certificate
- Drawings
- Technical Documents
- Price Agreements

Reference

TASK-012_File_Upload.md

---

# Tab — History

Displays

- Customer Created
- Status Changes
- Credit Limit Changes
- Salesperson Changes
- Address Changes
- Financial Updates

---

# Tab — Notes

Supports

- Internal Notes
- Commercial Notes
- Technical Notes
- Logistics Notes

Notes may contain

- Rich Text
- Attachments
- Mentions

---

# Customer Relationships

Supports

```
Parent Company

↓

Subsidiary

↓

Branch

↓

Project Company
```

Also supports

- Dealer Network
- Distributor Network

---

# Customer Timeline

Displays

```
Lead

↓

Opportunity

↓

Quotation

↓

Sales Order

↓

Shipment

↓

Delivery

↓

Invoice
```

Everything is clickable.

---

# Search

Supports

- Customer Code
- Name
- Tax Number
- Email
- Phone
- City
- Country
- Dealer
- Salesperson

Supports fuzzy search.

---

# Filters

Supports

- Customer Type
- Customer Group
- Status
- Region
- Industry
- Salesperson
- Company
- Plant
- Country

---

# Customer KPIs

Displays

- Total Revenue
- Last Order Date
- Open Quotations
- Open Orders
- Outstanding Balance
- Lifetime Value
- Average Order Value
- Customer Age

---

# User Actions

Users may

- Create Customer
- Edit Customer
- Archive Customer
- Merge Customers
- Import Customers
- Export Customers
- Create Opportunity
- Create Quotation
- Schedule Visit

---

# Validation Rules

The system validates

- Customer Code is unique.
- Customer Name is required.
- Customer Type is required.
- Tax Number must be unique.
- Primary Address required.
- Primary Contact required.
- Currency required.
- Company required.

---

# Permissions

Supports

- View Customer
- Create Customer
- Edit Customer
- Delete Customer
- Archive Customer
- Financial View
- Credit Limit Edit
- Export Customer

Reference

Permission_Model.md

---

# Notifications

Triggers

- New Customer Created
- Customer Assigned
- Credit Limit Changed
- Customer Blocked
- Customer Activated

Reference

Notification_System.md

---

# Audit

Records

- Customer Created
- Customer Updated
- Address Changed
- Contact Changed
- Financial Updated
- Status Changed

Reference

Audit_Log.md

---

# Mobile Behavior

Supports

- Customer Search
- Customer Details
- GPS Navigation
- Call
- Email
- Customer Visit
- Activity Logging
- Photo Upload

Reference

Sales_Mobile.md

---

# API References

```
GET    /customers

GET    /customers/{id}

POST   /customers

PUT    /customers/{id}

DELETE /customers/{id}

GET    /customers/search
```

Reference

Sales_API.md

---

# Related Modules

- Lead
- Opportunity
- Quotation
- Sales Order
- Shipment
- Delivery
- Customer Invoice
- Finance
- CRM
- Activities
- Document Management

---

# UI Components

Uses standard platform components

- Data Grid
- Search Box
- Filter Panel
- Detail Tabs
- Timeline
- Attachment Viewer
- KPI Cards
- Status Badge
- Avatar
- Activity Feed

Reference

Dashboard_Layout.md

Navigation.md

Theme.md

---

# Naswood Examples

### Example 1 — Dealer

```
Customer

ABC Yapı Market

↓

Type

Dealer

↓

Region

İstanbul

↓

Pricing Group

Dealer TR
```

---

### Example 2 — Export Customer

```
Customer

Nord Timber GmbH

↓

Country

Germany

↓

Currency

EUR

↓

Incoterms

DAP
```

---

### Example 3 — Contractor

```
Customer

Mega İnşaat

↓

Project

CLT Hotel

↓

Open Orders

3

↓

Outstanding Balance

Visible to Finance
```

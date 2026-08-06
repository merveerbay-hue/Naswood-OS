# ==============================================================================
# INVENTORY NAVIGATION
# Naswood Operating System (NOS)
# Module: Inventory
# Version: 1.0
# ==============================================================================

# PURPOSE

This document defines the navigation architecture of the Inventory module.

Inventory navigation is warehouse process-oriented rather than entity-oriented.

Users navigate through inventory operations and warehouse workflows instead of
generic CRUD pages.

Every navigation item belongs to a Workspace.

Navigation is role-aware, permission-driven and optimized for desktop, tablet
and handheld barcode terminals.

---

# DESIGN PRINCIPLES

Inventory navigation shall

- follow warehouse operations
- minimize user interaction
- support warehouse operators
- separate operational and administrative functions
- provide contextual actions
- support barcode and QR workflows
- support real-time inventory visibility
- support multi-warehouse operations
- provide breadcrumb navigation
- support favorites and recent transactions
- support global inventory search

Navigation must never expose database entities directly.

---

# NAVIGATION HIERARCHY

```text
Inventory

├── Dashboard

├── Inventory Operations

│   ├── Goods Receipt
│   ├── Goods Issue
│   ├── Stock Transfer
│   ├── Warehouse Transfer
│   ├── Location Transfer
│   ├── Inventory Adjustment
│   ├── Material Reservation
│   └── Opening Balance

├── Receiving

│   ├── Purchase Receipt
│   ├── Production Receipt
│   ├── Customer Return
│   ├── Supplier Return
│   ├── Inspection Receipt
│   └── Putaway

├── Shipping

│   ├── Sales Shipment
│   ├── Production Issue
│   ├── Maintenance Issue
│   ├── Sample Issue
│   └── Loading

├── Warehouse Management

│   ├── Warehouses
│   ├── Zones
│   ├── Locations
│   ├── Storage Rules
│   ├── Putaway Rules
│   └── Picking Rules

├── Inventory Control

│   ├── Cycle Count
│   ├── Physical Inventory
│   ├── Inventory Reconciliation
│   ├── Stock Adjustments
│   └── Variance Analysis

├── Material Tracking

│   ├── Lots
│   ├── Serials
│   ├── Inventory Transactions
│   ├── Traceability
│   └── Genealogy

├── Planning

│   ├── Stock Availability
│   ├── Reservations
│   ├── Safety Stock
│   ├── Reorder Suggestions
│   └── Shortage Analysis

├── Analytics

│   ├── Inventory Turnover
│   ├── Stock Aging
│   ├── ABC Analysis
│   ├── XYZ Analysis
│   ├── Warehouse Utilization
│   └── Inventory Valuation

└── Reports

    ├── Inventory Reports
    ├── Warehouse Reports
    ├── Transaction Reports
    ├── Lot Reports
    ├── Serial Reports
    └── Valuation Reports
```

---

# NAVIGATION LEVELS

## Level 1

Module

```text
Inventory
```

---

## Level 2

Workspace

Example

```text
Receiving
```

---

## Level 3

Business Function

Example

```text
Purchase Receipt
```

---

## Level 4

Contextual Screen

Example

```text
Receiving

>

Purchase Receipt

>

PO-240015

>

Putaway
```

---

# USER ROLE NAVIGATION

## Warehouse Manager

Landing Page

```text
Inventory Dashboard
```

Primary Navigation

```text
Dashboard

Warehouse Management

Inventory Control

Analytics

Reports
```

---

## Warehouse Operator

Landing Page

```text
Inventory Operations
```

Primary Navigation

```text
Goods Receipt

Goods Issue

Transfers

Receiving

Shipping
```

---

## Receiving Operator

Landing Page

```text
Receiving Workspace
```

Primary Navigation

```text
Purchase Receipt

Production Receipt

Inspection Receipt

Putaway
```

---

## Shipping Operator

Landing Page

```text
Shipping Workspace
```

Primary Navigation

```text
Sales Shipment

Production Issue

Loading
```

---

## Inventory Controller

Landing Page

```text
Inventory Control
```

Primary Navigation

```text
Cycle Count

Physical Inventory

Adjustments

Variance Analysis
```

---

## Inventory Planner

Landing Page

```text
Planning Workspace
```

Primary Navigation

```text
Stock Availability

Reservations

Safety Stock

Shortage Analysis
```

---

# BREADCRUMB EXAMPLES

Goods Receipt

```text
Inventory

>

Receiving

>

Purchase Receipt

>

PO-240015
```

---

Stock Transfer

```text
Inventory

>

Inventory Operations

>

Stock Transfer

>

TR-240021
```

---

Cycle Count

```text
Inventory

>

Inventory Control

>

Cycle Count

>

Zone A
```

---

Material Traceability

```text
Inventory

>

Material Tracking

>

Lot

>

LOT-240845
```

---

# CONTEXTUAL ACTIONS

Goods Receipt

```text
Receive

Inspect

Putaway

Print Labels

Post
```

---

Goods Issue

```text
Pick

Scan

Confirm

Post
```

---

Transfer

```text
Scan Source

Scan Destination

Transfer

Complete
```

---

Cycle Count

```text
Start Count

Scan

Recount

Approve

Post Adjustment
```

---

Lot Tracking

```text
View History

View Genealogy

View Quality

Locate Stock
```

---

# QUICK ACCESS

Favorites

Recent Transactions

Pinned Warehouses

Pinned Locations

Pinned Materials

Pinned Reports

---

# GLOBAL SEARCH

Supports

```text
Material

Product

Lot

Serial

Warehouse

Location

Transaction

Reservation

Purchase Receipt

Shipment
```

Global Search returns

- Current Stock
- Warehouse
- Location
- Related Transactions
- Available Actions

---

# MOBILE NAVIGATION

Bottom Navigation

```text
Home

Tasks

Scanner

Notifications

Profile
```

Quick Actions

```text
Receive Material

Issue Material

Transfer Stock

Cycle Count

Locate Material

Scan Barcode

Scan QR Code
```

---

# NOTIFICATIONS

Display

```text
Low Stock

Negative Inventory

Pending Receipts

Pending Shipments

Reservation Conflicts

Cycle Count Due

Quality Hold

Warehouse Capacity Warning
```

Notifications provide direct navigation to the related workspace.

---

# DESIGN RULES

Navigation is warehouse process-driven.

Navigation is workspace-based.

Navigation is permission-aware.

Navigation must never expose generic CRUD menus.

Inventory transactions are always initiated through business workflows.

Users must reach any operational function within three navigation levels.

Every detail screen provides contextual actions instead of generic action bars.

---

# IMPLEMENTATION RULES

Frontend implementation shall

- generate navigation from Workspace definitions
- apply role-based visibility
- apply permission filtering
- support handheld barcode terminals
- support responsive layouts
- support deep linking
- preserve navigation state
- support keyboard shortcuts
- support favorites and recent transactions

Inventory navigation shall be generated from Module, Workspace and Workflow definitions rather than directly from entities or implementation tasks.

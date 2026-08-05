# TASK-041 — Shipment

**Module:** Sales

**Document Type:** Design Specification

**Version:** 1.0

**Status:** Approved

**Owner:** Sales Product Team

---

# Purpose

The Shipment module manages outbound logistics after products are reserved or manufactured.

A Shipment represents the physical movement of goods from a Naswood warehouse or factory to the customer or project site. It coordinates picking, packing, loading, transportation and tracking until delivery.

The Shipment module bridges Sales, Inventory, Logistics and Delivery.

---

# Design Goals

The module is designed to

- Standardize shipment operations
- Optimize warehouse picking
- Support barcode and QR verification
- Enable shipment tracking
- Support partial and consolidated shipments
- Improve delivery performance
- Provide complete logistics traceability

---

# Screen Layout

```
────────────────────────────────────────────────────────────

Shipment List

────────────────────────────────────────────────────────────

Search

Filters

Shipment Grid

────────────────────────────────────────────────────────────

+ New Shipment

Assign Vehicle

Dispatch

Export

────────────────────────────────────────────────────────────
```

Selecting a shipment opens the Shipment Detail screen.

---

# Shipment Detail Layout

```
────────────────────────────────────────────────────────────

Shipment Header

────────────────────────────────────────────────────────────

General

Sales Orders

Products

Picking

Loading

Transportation

Tracking

Documents

Timeline

Notes

────────────────────────────────────────────────────────────
```

---

# Shipment Header

Displays

- Shipment Number
- Shipment Type
- Customer
- Shipment Status
- Warehouse
- Vehicle
- Driver
- Carrier
- Dispatch Date
- Planned Delivery
- Company
- Plant

Actions

- Edit
- Generate Picking List
- Start Picking
- Complete Loading
- Dispatch
- Print Labels
- Export Documents

---

# Shipment Status

```
Draft

↓

Planned

↓

Picking

↓

Packed

↓

Loaded

↓

Dispatched

↓

In Transit

↓

Delivered

or

Cancelled
```

---

# Shipment Types

Supports

- Standard Shipment
- Partial Shipment
- Consolidated Shipment
- Export Shipment
- Dealer Shipment
- Internal Transfer
- Project Shipment
- Sample Shipment

---

# Tab — General

Stores

## Basic Information

- Shipment Number
- Shipment Type
- Warehouse
- Dispatch Date
- Planned Delivery Date
- Priority
- Shipping Method

## Logistics Information

- Carrier
- Driver
- Vehicle
- Trailer
- Tracking Number
- Route

---

# Tab — Sales Orders

Displays linked Sales Orders.

Each record contains

- Sales Order Number
- Customer
- Order Status
- Reserved Quantity
- Shipment Quantity

Supports

- Multiple Sales Orders
- Split Shipment
- Consolidated Shipment

Reference

TASK-040_Sales_Order.md

---

# Tab — Products

Displays

- Product Code
- Product Name
- Batch Number
- Serial Number
- Quantity
- Unit
- Weight
- Volume
- Package Count

Supports

- Barcode Verification
- QR Verification
- Lot Tracking

---

# Tab — Picking

Displays

- Warehouse
- Location
- Bin
- Quantity
- Picker
- Picking Status

Workflow

```
Picking List

↓

Warehouse Scan

↓

Confirmation

↓

Packing
```

Supports handheld barcode scanners.

---

# Tab — Loading

Displays

- Vehicle
- Loading Sequence
- Package Count
- Weight
- Volume
- Loading Status

Supports

- Pallet Loading
- Container Loading
- Truck Loading

---

# Tab — Transportation

Stores

- Carrier
- Vehicle Plate
- Driver
- Route
- Estimated Arrival
- Freight Cost
- GPS Tracking

Supports

- Internal Fleet
- Third-Party Logistics
- Export Logistics

---

# Tab — Tracking

Displays

```
Shipment Planned

↓

Picking

↓

Loaded

↓

Dispatched

↓

In Transit

↓

Delivered
```

Shows

- Current Status
- Current Location
- Estimated Arrival
- Delay Information

---

# Tab — Documents

Supports

- Packing List
- Delivery Note
- Transport Document
- Export Documents
- Customs Documents
- Photos

Reference

TASK-012_File_Upload.md

---

# Tab — Timeline

Displays

```
Shipment Created

↓

Picking Started

↓

Picking Completed

↓

Loaded

↓

Dispatched

↓

Delivered
```

Every action is timestamped.

---

# Tab — Notes

Supports

- Logistics Notes
- Driver Notes
- Warehouse Notes
- Customer Notes

Supports rich text and attachments.

---

# Barcode & QR Support

Supports

- Product Barcode
- Package Barcode
- Pallet Barcode
- Shipment QR Code

Used during

- Picking
- Loading
- Delivery Verification

---

# GPS Tracking

Displays

- Live Vehicle Location
- Current Route
- Distance Remaining
- Estimated Arrival

Future support

- Geofencing
- Traffic Optimization

---

# Search

Supports

- Shipment Number
- Customer
- Sales Order
- Vehicle
- Driver
- Carrier
- Tracking Number
- Warehouse

Supports fuzzy search.

---

# Filters

Supports

- Shipment Status
- Shipment Type
- Warehouse
- Carrier
- Driver
- Dispatch Date
- Delivery Date
- Company
- Plant

---

# Shipment KPIs

Displays

- Planned Shipments
- In Transit
- Delivered Today
- Delayed Shipments
- Average Delivery Time
- Loading Efficiency
- Shipment Accuracy
- On-Time Shipment %

---

# User Actions

Users may

- Create Shipment
- Edit Shipment
- Generate Picking List
- Start Picking
- Confirm Loading
- Dispatch Shipment
- Print Labels
- Cancel Shipment
- Export Documents

---

# Validation Rules

The system validates

- Shipment Number is unique.
- At least one Sales Order is linked.
- Products must be reserved before dispatch.
- Vehicle is required before loading.
- Driver is required before dispatch.
- Dispatched shipments cannot be edited.
- Delivered shipments are read-only.

---

# Permissions

Supports

- View Shipment
- Create Shipment
- Edit Shipment
- Delete Shipment
- Dispatch Shipment
- Cancel Shipment
- Print Documents
- Export Shipment

Reference

Permission_Model.md

---

# Notifications

Triggers

- Shipment Created
- Picking Started
- Shipment Ready
- Shipment Dispatched
- Shipment Delayed
- Shipment Delivered

Reference

Notification_System.md

---

# Audit

Records

- Shipment Created
- Updated
- Picking Started
- Picking Completed
- Loaded
- Dispatched
- Delivered
- Cancelled

Reference

Audit_Log.md

---

# Mobile Behavior

Supports

- Shipment Lookup
- Barcode Scan
- QR Scan
- GPS Tracking
- Photo Upload
- Dispatch Confirmation
- Offline Warehouse Mode

Reference

Sales_Mobile.md

---

# API References

```http
GET    /shipments

GET    /shipments/{id}

POST   /shipments

PUT    /shipments/{id}

DELETE /shipments/{id}

POST   /shipments/{id}/dispatch

POST   /shipments/{id}/complete

GET    /shipments/tracking/{id}
```

Reference

Sales_API.md

---

# Related Modules

- Sales Order
- Inventory
- Warehouse
- Delivery
- Customer Invoice
- Logistics
- Finance
- Dashboard

---

# UI Components

Uses standard platform components

- Data Grid
- Picking Grid
- Barcode Scanner
- QR Scanner
- GPS Map
- Timeline
- Status Badge
- Attachment Viewer
- KPI Cards
- Progress Indicator

Reference

Dashboard_Layout.md

Navigation.md

Theme.md

---

# Naswood Examples

### Example 1 — CLT Building Shipment

```
Shipment

SH-2026-001285

↓

Customer

ABC Construction

↓

Vehicle

Truck 08 ABC 123

↓

Status

In Transit
```

---

### Example 2 — Export Shipment

```
Shipment

SH-2026-001412

↓

Customer

Nord Timber GmbH

↓

Container

MSCU 4578123

↓

Destination

Hamburg Port

↓

Status

Dispatched
```

---

### Example 3 — Dealer Delivery

```
Shipment

SH-2026-001566

↓

Dealer

İstanbul Dealer

↓

Products

Thermowood Decking

Pellet

↓

Shipment Type

Partial Shipment
```

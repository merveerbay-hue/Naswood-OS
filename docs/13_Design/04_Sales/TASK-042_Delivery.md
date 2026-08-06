> **UX authority note:** `+ New` / Create wireframes in this TASK are historical. Live CTAs: [`Sales_Screens.md`](./Sales_Screens.md) · [`Screen_Types.md`](../Common/Screen_Types.md) § Create matrix · Process_Screens.

# TASK-042 — Delivery

**Module:** Sales

**Document Type:** Design Specification

**Version:** 1.0

**Status:** Approved

**Owner:** Sales Product Team

---

# Purpose

The Delivery module manages the final stage of the sales fulfillment process by confirming that shipped goods have been successfully delivered to the customer.

A Delivery represents the legal and operational confirmation that products have reached their destination. It captures customer acceptance, proof of delivery (POD), signatures, photos, delivery exceptions and final logistics information.

The Delivery module bridges Logistics, Sales, Inventory and Finance.

---

# Design Goals

The module is designed to

- Standardize delivery confirmation
- Support Proof of Delivery (POD)
- Capture customer signatures
- Verify delivery using GPS
- Record delivery exceptions
- Support partial deliveries
- Enable automatic invoice creation

---

# Screen Layout

```
────────────────────────────────────────────────────────────

Delivery List

────────────────────────────────────────────────────────────

Search

Filters

Delivery Grid

────────────────────────────────────────────────────────────

Confirm Delivery

Generate POD

Export

────────────────────────────────────────────────────────────
```

Selecting a delivery opens the Delivery Detail screen.

---

# Delivery Detail Layout

```
────────────────────────────────────────────────────────────

Delivery Header

────────────────────────────────────────────────────────────

General

Shipment

Products

Customer Acceptance

Proof of Delivery

Documents

Timeline

Notes

────────────────────────────────────────────────────────────
```

---

# Delivery Header

Displays

- Delivery Number
- Shipment Number
- Customer
- Delivery Status
- Delivery Date
- Driver
- Vehicle
- Warehouse
- Company
- Plant

Actions

- Confirm Delivery
- Capture Signature
- Upload Photos
- Report Exception
- Generate POD
- Print Delivery Note

---

# Delivery Status

```
Planned

↓

Dispatched

↓

Arrived

↓

Unloading

↓

Customer Verification

↓

Accepted

↓

Completed

or

Partially Accepted

or

Rejected

or

Returned
```

---

# Delivery Types

Supports

- Standard Delivery
- Partial Delivery
- Export Delivery
- Dealer Delivery
- Project Site Delivery
- Direct Factory Delivery
- Internal Delivery

---

# Tab — General

Stores

## Basic Information

- Delivery Number
- Shipment Reference
- Delivery Date
- Delivery Time
- Warehouse
- Delivery Method

## Customer Information

- Customer
- Delivery Address
- Contact Person
- Contact Phone

---

# Tab — Shipment

Displays

- Shipment Number
- Dispatch Date
- Vehicle
- Driver
- Carrier
- Tracking Number
- Route

Reference

TASK-041_Shipment.md

---

# Tab — Products

Displays

- Product Code
- Product Name
- Batch Number
- Serial Number
- Delivered Quantity
- Accepted Quantity
- Rejected Quantity
- Unit

Supports

- Barcode Verification
- QR Verification
- Lot Traceability

---

# Customer Acceptance

Supports

- Full Acceptance
- Partial Acceptance
- Rejection

Customer may specify

- Missing Quantity
- Damaged Goods
- Incorrect Product
- Packaging Damage
- Installation Issue

---

# Proof of Delivery (POD)

Stores

- Customer Signature
- Delivery Photos
- GPS Coordinates
- Delivery Timestamp
- Driver Confirmation
- Receiver Name

Generated automatically after confirmation.

---

# Digital Signature

Supports

- Finger Signature
- Stylus Signature
- Electronic Signature

Stores

- Signer Name
- Signature Image
- Timestamp
- GPS Location
- Device ID

---

# GPS Verification

Records

- Delivery Location
- Arrival Time
- Departure Time
- Distance from Planned Location

Supports

- Geofencing
- Route Verification
- Location Accuracy

---

# Delivery Exceptions

Supports

- Customer Not Available
- Wrong Address
- Damaged Goods
- Missing Products
- Delivery Refused
- Weather Delay
- Vehicle Breakdown

Every exception requires

- Reason Code
- Description
- Photos (Optional)
- Responsible Party

---

# Photo Documentation

Supports

- Delivered Products
- Packaging Condition
- Damage Photos
- Site Photos
- Signed Documents

Images are compressed automatically before upload.

---

# Tab — Documents

Supports

- Delivery Note
- Proof of Delivery
- Packing List
- Customer Receipt
- Damage Report
- Transport Documents

Reference

TASK-012_File_Upload.md

---

# Tab — Timeline

Displays

```
Shipment Created

↓

Vehicle Dispatched

↓

Arrived at Customer

↓

Products Unloaded

↓

Customer Verified

↓

Signed

↓

Completed
```

Every event is timestamped.

---

# Tab — Notes

Supports

- Delivery Notes
- Driver Notes
- Customer Comments
- Damage Description
- Internal Notes

Supports rich text and attachments.

---

# Automatic Invoice Trigger

If enabled

```
Delivery Completed

↓

Invoice Generation

↓

Finance Posting
```

Supports

- Automatic
- Manual
- Approval Required

Reference

TASK-043_Customer_Invoice.md

---

# Search

Supports

- Delivery Number
- Shipment Number
- Customer
- Driver
- Vehicle
- Tracking Number
- Delivery Date
- Receiver

Supports fuzzy search.

---

# Filters

Supports

- Delivery Status
- Customer
- Driver
- Warehouse
- Carrier
- Delivery Date
- Company
- Plant

---

# Delivery KPIs

Displays

- Planned Deliveries
- Completed Deliveries
- Partial Deliveries
- Rejected Deliveries
- On-Time Delivery %
- Delivery Accuracy %
- POD Completion %
- Average Delivery Time

---

# User Actions

Users may

- Confirm Delivery
- Capture Signature
- Upload Photos
- Record Exceptions
- Print Delivery Note
- Generate POD
- Complete Delivery

---

# Validation Rules

The system validates

- Delivery Number is unique.
- Shipment must be dispatched.
- Receiver name is required.
- GPS verification is required (optional by company policy).
- Signature is required if configured.
- Completed deliveries cannot be modified.
- Returned deliveries require a reason code.

---

# Permissions

Supports

- View Delivery
- Confirm Delivery
- Upload Photos
- Capture Signature
- Record Exception
- Generate POD
- Export Delivery

Reference

Permission_Model.md

---

# Notifications

Triggers

- Delivery Started
- Delivery Arrived
- Delivery Completed
- Customer Rejected Delivery
- Delivery Exception
- POD Generated

Reference

Notification_System.md

---

# Audit

Records

- Delivery Created
- Delivery Confirmed
- Signature Captured
- GPS Recorded
- Photos Uploaded
- Exception Reported
- Delivery Completed

Reference

Audit_Log.md

---

# Mobile Behavior

Supports

- Delivery Lookup
- GPS Navigation
- Barcode Scan
- QR Scan
- Signature Capture
- Photo Upload
- Offline Confirmation
- Automatic Synchronization

Reference

Sales_Mobile.md

---

# API References

```http
GET    /deliveries

GET    /deliveries/{id}

POST   /deliveries

PUT    /deliveries/{id}

DELETE /deliveries/{id}

POST   /deliveries/{id}/confirm

POST   /deliveries/{id}/accept

POST   /deliveries/{id}/reject

POST   /deliveries/{id}/complete

GET    /deliveries/search
```

Reference

Sales_API.md

---

# Related Modules

- Shipment
- Sales Order
- Customer
- Customer Invoice
- Inventory
- Logistics
- Finance
- Dashboard

---

# UI Components

Uses standard platform components

- Data Grid
- Timeline
- Status Badge
- Barcode Scanner
- QR Scanner
- Signature Pad
- Camera Upload
- GPS Map
- Attachment Viewer
- KPI Cards

Reference

Dashboard_Layout.md

Navigation.md

Theme.md

---

# Naswood Examples

### Example 1 — CLT Construction Site

```
Delivery

DL-2026-000845

↓

Customer

ABC Construction

↓

Project

Hotel CLT

↓

Status

Completed

↓

Customer Signature

Captured
```

---

### Example 2 — Export Delivery

```
Delivery

DL-2026-000912

↓

Customer

Nord Timber GmbH

↓

Destination

Hamburg

↓

POD

Generated

↓

Status

Accepted
```

---

### Example 3 — Dealer Delivery

```
Delivery

DL-2026-001043

↓

Dealer

İstanbul Dealer

↓

Products

Thermowood Decking

↓

GPS Verified

↓

Completed
```

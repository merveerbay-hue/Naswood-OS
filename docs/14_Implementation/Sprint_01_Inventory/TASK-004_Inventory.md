# TASK-004 Inventory

**Sprint:** Sprint_01_Inventory

**Module:** Inventory

**Priority:** Critical

**Estimated Effort:** 12 Hours

**Status:** Planned

---

# Objective

Implement the Inventory Management module.

Inventory is the core module of Naswood OS and maintains the real-time quantity, availability, reservation and valuation of every material stored in the factory.

Every inventory transaction must be fully traceable.

---

# Business Value

Inventory is the foundation of:

- Purchasing
- Warehouse Management
- Production
- Sales
- Logistics
- Quality
- Maintenance
- Finance
- AI
- Digital Twin

Without Inventory, no operational module can function.

---

# References

README.md

Cursor_Rules.md

docs/01_Business/Business_Rules.md

docs/01_Business/Factory_Flow.md

docs/05_Modules/06_Inventory/Inventory.md

docs/13_Design/02_Inventory/Inventory.md

---

# Scope

Implement complete Inventory Management including:

- Inventory Balance
- Available Stock
- Reserved Stock
- Incoming Stock
- Outgoing Stock
- Batch Inventory
- Multi Warehouse
- Multi Location
- Inventory Search
- Inventory Dashboard

---

# Functional Requirements

The system shall support:

- View Inventory
- Search Inventory
- Filter Inventory
- Real-Time Inventory Balance
- Warehouse Inventory
- Location Inventory
- Batch Inventory
- Inventory Reservation
- Inventory Availability
- Inventory History
- Export Inventory

---

# Inventory Fields

Inventory ID

Material

Warehouse

Location

Batch

Lot

Serial Number

On Hand Quantity

Available Quantity

Reserved Quantity

Allocated Quantity

Incoming Quantity

Outgoing Quantity

Unit

Inventory Value

Average Cost

Last Cost

Status

Last Movement Date

Created At

Updated At

---

# Inventory Status

Available

Reserved

Allocated

Blocked

Quality Hold

Returned

Scrap

Expired

---

# Business Rules

Inventory cannot be negative.

Every Inventory record belongs to one Material.

Every Inventory record belongs to one Warehouse.

Every Inventory record belongs to one Location.

Inventory quantity must always equal:

Opening Balance

+

Goods Receipt

+

Production Receipt

+

Transfer In

-

Goods Issue

-

Production Consumption

-

Transfer Out

-

Adjustments

Inventory balance is always calculated from Stock Movements.

Inventory records cannot be manually edited.

Inventory is updated only by transactions.

---

# Inventory Calculation

Available Quantity

=

On Hand

-

Reserved

-

Allocated

Blocked inventory cannot be used.

Reserved inventory cannot be consumed.

Allocated inventory belongs to active Production Orders.

---

# Relationships

Material

↓

Warehouse

↓

Location

↓

Batch

↓

Inventory

↓

Stock Movement

↓

Goods Receipt

↓

Goods Issue

↓

Production

↓

Shipment

---

# Permissions

Inventory.View

Inventory.Export

Inventory.Reserve

Inventory.Release

Inventory.Adjust

Inventory.Count

Inventory.Recalculate

---

# API

GET /inventory

GET /inventory/{id}

GET /inventory/material/{materialId}

GET /inventory/warehouse/{warehouseId}

GET /inventory/location/{locationId}

GET /inventory/batch/{batchId}

GET /inventory/search

GET /inventory/history

POST /inventory/reserve

POST /inventory/release

POST /inventory/recalculate

---

# UI Pages

Inventory Dashboard

Inventory List

Inventory Detail

Inventory History

Inventory Reservation

Inventory Search

Inventory Availability

---

# UI Components

Search Box

Warehouse Filter

Location Filter

Material Filter

Batch Filter

Quantity Summary

Inventory Grid

History Timeline

Export Button

---

# Database

Table

Inventory

Columns

Id

MaterialId

WarehouseId

LocationId

BatchId

LotId

SerialNumber

OnHandQuantity

AvailableQuantity

ReservedQuantity

AllocatedQuantity

IncomingQuantity

OutgoingQuantity

AverageCost

LastCost

InventoryValue

Status

LastMovementDate

CreatedAt

UpdatedAt

CreatedBy

UpdatedBy

---

# Inventory Transactions

Goods Receipt

Goods Issue

Transfer In

Transfer Out

Production Receipt

Production Consumption

Inventory Adjustment

Inventory Count

Reservation

Reservation Release

---

# Events

InventoryCreated

InventoryUpdated

InventoryReserved

InventoryReleased

InventoryAdjusted

InventoryCountCompleted

InventoryRecalculated

---

# Audit

Every Inventory transaction must record:

User

Timestamp

Material

Warehouse

Location

Batch

Previous Quantity

New Quantity

Transaction Type

Reference Document

Reason

---

# Performance Requirements

Inventory queries must support:

- Pagination
- Filtering
- Sorting
- Full Text Search

Inventory balance calculation must be transaction-safe.

Concurrent inventory updates must use optimistic concurrency control.

---

# Tests

Inventory Creation

Inventory Search

Inventory Reservation

Inventory Release

Inventory Recalculation

Warehouse Filtering

Location Filtering

Batch Filtering

Negative Inventory Validation

Permission Validation

Performance Test

Concurrent Transaction Test

---

# Acceptance Criteria

✔ Inventory balances are always accurate

✔ Inventory updates only through transactions

✔ Search works

✔ Filtering works

✔ Reservation works

✔ Availability calculation works

✔ Batch inventory works

✔ Multi warehouse supported

✔ Multi location supported

✔ Audit Log enabled

✔ OpenAPI documented

✔ Unit Tests passed

✔ Integration Tests passed

✔ Performance requirements satisfied

---

# Deliverables

- Domain Entity
- Repository
- CQRS Commands
- CQRS Queries
- Inventory Calculator
- Reservation Service
- DTOs
- Validators
- REST API
- Database Migration
- React Pages
- Dashboard
- Unit Tests
- Integration Tests
- Swagger Documentation

---

# Cursor Implementation Prompt

Read:

- Cursor_Rules.md
- Business_Rules.md
- Factory_Flow.md
- docs/13_Design/02_Inventory/Inventory.md
- This TASK document

Implement the complete Inventory module using:

- .NET 9
- ASP.NET Core
- Clean Architecture
- CQRS
- MediatR
- EF Core
- PostgreSQL
- FluentValidation
- React
- TypeScript

Requirements:

- Inventory must never be updated directly.
- All inventory changes must originate from inventory transactions.
- Support multiple warehouses.
- Support multiple locations.
- Support batch and serial tracking.
- Use optimistic concurrency for stock updates.
- Follow project architecture exactly.
- Do not implement unrelated modules.

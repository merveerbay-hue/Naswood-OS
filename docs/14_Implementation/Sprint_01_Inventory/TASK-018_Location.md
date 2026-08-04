# TASK-018 Location

**Sprint:** Sprint_01_Inventory

**Module:** Inventory

**Priority:** Critical

**Estimated Effort:** 8 Hours

**Status:** Planned

---

# Objective

Implement the Warehouse Location Management module.

A Location represents the physical storage position inside a warehouse. Every inventory transaction must occur within a specific location to ensure full traceability and accurate stock management.

---

# Business Value

Warehouse Locations enable:

- Precise inventory tracking
- Bin-level stock management
- Batch traceability
- Warehouse optimization
- Picking accuracy
- Production material staging

Location Management is required by:

- Inventory
- Goods Receipt
- Goods Issue
- Stock Transfer
- Inventory Count
- Production
- Purchasing
- Sales

---

# References

README.md

Cursor_Rules.md

docs/01_Business/Business_Rules.md

docs/01_Business/Factory_Flow.md

docs/05_Modules/06_Inventory/Locations.md

docs/13_Design/02_Inventory/Location.md

---

# Scope

Implement Warehouse Location Management including:

- Location CRUD
- Warehouse Assignment
- Parent / Child Locations
- Location Capacity
- Location Status
- Location Search
- Barcode Support

---

# Functional Requirements

The system shall support:

- Create Location
- Update Location
- Archive Location
- Activate Location
- Deactivate Location
- Search Locations
- Filter Locations
- Print Location Labels
- Export Location List

---

# Location Fields

Location Code

Location Name

Warehouse

Parent Location

Location Type

Zone

Aisle

Rack

Shelf

Bin

Capacity

Current Occupancy

Barcode

QR Code

Status

Description

Created At

Updated At

Created By

Updated By

---

# Location Types

Receiving

Storage

Production

Picking

Packing

Shipping

Quality

Returns

Scrap

Buffer

Virtual

---

# Business Rules

Every Location belongs to exactly one Warehouse.

Location Code must be unique within the Warehouse.

Inactive Locations cannot receive inventory.

Locations with inventory cannot be deleted.

Capacity cannot be exceeded.

Parent Location must belong to the same Warehouse.

---

# Validation

Warehouse Required

Location Code Required

Location Name Required

Location Type Required

Unique Location Code

Capacity >= Current Occupancy

---

# Relationships

Warehouse

↓

Location

↓

Inventory

↓

Batch

↓

Stock Movement

↓

Goods Receipt

↓

Goods Issue

↓

Transfer

---

# Permissions

Location.View

Location.Create

Location.Update

Location.Delete

Location.Export

Location.Import

---

# API

GET /locations

GET /locations/{id}

POST /locations

PUT /locations/{id}

DELETE /locations/{id}

GET /locations/search

GET /locations/filter

GET /warehouses/{id}/locations

---

# UI Pages

Location List

Location Detail

Create Location

Edit Location

Location Map

Location Dashboard

---

# UI Components

Search Box

Warehouse Filter

Location Tree

Data Grid

Pagination

Status Badge

Barcode Display

QR Code Display

Export Button

Import Button

---

# Database

Table

Locations

Columns

Id

WarehouseId

ParentLocationId

Code

Name

Type

Zone

Aisle

Rack

Shelf

Bin

Capacity

CurrentOccupancy

Barcode

QRCode

Status

Description

CreatedAt

UpdatedAt

CreatedBy

UpdatedBy

---

# Events

LocationCreated

LocationUpdated

LocationActivated

LocationDeactivated

LocationArchived

LocationCapacityChanged

---

# Audit

Every change must record:

User

Timestamp

Old Value

New Value

Reason

---

# Tests

Create Location

Update Location

Archive Location

Search Locations

Filter Locations

Warehouse Assignment

Duplicate Code Validation

Capacity Validation

Permission Validation

---

# Acceptance Criteria

✔ Location CRUD completed

✔ Warehouse assignment works

✔ Parent / Child hierarchy supported

✔ Search works

✔ Filtering works

✔ Barcode support implemented

✔ Capacity validation implemented

✔ Audit Log enabled

✔ OpenAPI documented

✔ Unit Tests passed

✔ Integration Tests passed

---

# Deliverables

- Domain Entity
- Repository
- CQRS Commands
- CQRS Queries
- DTOs
- Validators
- REST API
- Database Migration
- React Pages
- Unit Tests
- Integration Tests
- Swagger Documentation

---

# Cursor Implementation Prompt

Read:

- Cursor_Rules.md
- Business_Rules.md
- Factory_Flow.md
- docs/13_Design/02_Inventory/Location.md
- This TASK document

Implement the complete Warehouse Location module using:

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

- Locations must support unlimited hierarchy.
- Every Location belongs to a Warehouse.
- Inventory movements must always reference a Location.
- Implement barcode and QR code support.
- Follow project architecture exactly.
- Do not implement unrelated modules.

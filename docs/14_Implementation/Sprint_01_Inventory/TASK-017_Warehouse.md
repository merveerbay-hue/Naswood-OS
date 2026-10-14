# TASK-017 Warehouse

**Sprint:** Sprint_01_Inventory

**Module:** Inventory

**Priority:** Critical

**Estimated Effort:** 8 Hours

**Status:** Completed

---

# Objective

Implement the Warehouse Master module.

The Warehouse module is responsible for defining all physical warehouses within Naswood OS.

Warehouses are the highest level of inventory storage and are used throughout Inventory, Purchasing, Production, Sales and Logistics.

---

# Business Value

Without Warehouse Master the Inventory module cannot operate.

Warehouse is required by:

- Inventory
- Stock Movement
- Goods Receipt
- Goods Issue
- Production
- Purchasing
- Sales
- Logistics

---

# References

README.md

Cursor_Rules.md

docs/01_Business/Business_Rules.md

docs/01_Business/Factory_Flow.md

docs/05_Modules/06_Inventory/Warehouse.md

docs/13_Design/02_Inventory/Warehouse.md

---

# Scope

Implement Warehouse Management including:

- Warehouse CRUD
- Warehouse Status
- Warehouse Types
- Default Warehouse
- Warehouse Search
- Warehouse Filtering

---

# Functional Requirements

The system shall support:

- Create Warehouse
- Update Warehouse
- Archive Warehouse
- Activate Warehouse
- Deactivate Warehouse
- Search Warehouse
- Filter Warehouse
- Export Warehouse List

---

# Warehouse Fields

Warehouse Code

Warehouse Name

Description

Warehouse Type

Plant

Address

Manager

Phone

Email

Default Warehouse

Status

Created At

Updated At

Created By

Updated By

---

# Warehouse Types

Raw Material

Production

Semi Finished

Finished Goods

Quality

Returns

Scrap

Temporary

External

Virtual

---

# Business Rules

Warehouse Code must be unique.

Warehouse Name is required.

Warehouse cannot be deleted if inventory exists.

Only one warehouse may be marked as default.

Inactive warehouses cannot receive inventory.

Warehouse type cannot change when inventory exists.

---

# Validation

Warehouse Code Required

Warehouse Name Required

Warehouse Code Unique

Warehouse Type Required

Plant Required

Status Required

---

# Relationships

Warehouse

↓

Locations

↓

Inventory

↓

Batch

↓

Stock Movements

↓

Goods Receipt

↓

Goods Issue

↓

Transfer

---

# Permissions

Warehouse.View

Warehouse.Create

Warehouse.Update

Warehouse.Delete

Warehouse.Export

Warehouse.Import

---

# API

GET /warehouses

GET /warehouses/{id}

POST /warehouses

PUT /warehouses/{id}

DELETE /warehouses/{id}

GET /warehouses/search

GET /warehouses/filter

---

# UI Pages

Warehouse List

Warehouse Detail

Create Warehouse

Edit Warehouse

Warehouse Search

Warehouse Dashboard

---

# UI Components

Search Box

Filters

Data Grid

Pagination

Status Badge

Action Menu

Export Button

Import Button

---

# Database

Table

Warehouses

Columns

Id

Code

Name

Description

Type

PlantId

Address

Manager

Phone

Email

IsDefault

Status

CreatedAt

UpdatedAt

CreatedBy

UpdatedBy

---

# Events

WarehouseCreated

WarehouseUpdated

WarehouseActivated

WarehouseDeactivated

WarehouseArchived

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

Create Warehouse

Update Warehouse

Deactivate Warehouse

Search Warehouse

Filter Warehouse

Permission Validation

Duplicate Code Validation

Default Warehouse Validation

---

# Acceptance Criteria

✔ Warehouse CRUD completed

✔ Search works

✔ Filtering works

✔ Pagination works

✔ Validation completed

✔ Role permissions implemented

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
- Warehouse Design.md
- This TASK document

Implement the complete Warehouse module using:

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

Do not implement unrelated modules.

Follow project architecture exactly.

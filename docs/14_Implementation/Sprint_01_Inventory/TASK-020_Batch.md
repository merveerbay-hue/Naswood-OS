# TASK-020 Batch

**Sprint:** Sprint_01_Inventory

**Module:** Inventory

**Priority:** Critical

**Estimated Effort:** 10 Hours

**Status:** Planned

---

# Objective

Implement the Batch Management module.

A Batch represents a uniquely identifiable production or procurement lot that enables complete traceability of materials throughout the entire manufacturing lifecycle.

Batch Management is mandatory for raw materials, semi-finished goods, finished goods, thermowood products, massive panels and production tracking.

---

# Business Value

Batch Management enables:

- Complete traceability
- Production genealogy
- Quality tracking
- Inventory control
- Recall management
- Material history
- Digital Product Passport
- AI Analytics

Every inventory transaction must reference a Batch whenever batch tracking is enabled.

---

# References

README.md

Cursor_Rules.md

docs/01_Business/Business_Rules.md

docs/01_Business/Factory_Flow.md

docs/05_Modules/06_Inventory/Batch_Inventory.md

docs/13_Design/02_Inventory/Batch.md

---

# Scope

Implement complete Batch Management including:

- Batch Creation
- Batch Assignment
- Batch Traceability
- Batch History
- Batch Genealogy
- Parent / Child Batch
- Batch Status
- Batch Search
- Batch Dashboard

---

# Functional Requirements

The system shall support:

- Create Batch
- Update Batch
- Archive Batch
- Search Batch
- Filter Batch
- Batch Traceability
- Parent / Child Batch
- Batch Inventory
- Batch History
- Batch Genealogy
- Batch Label Printing

---

# Batch Fields

Batch Number

Material

Warehouse

Location

Supplier

Purchase Order

Production Order

Parent Batch

Child Batch

Quantity

Available Quantity

Reserved Quantity

Unit

Manufacturing Date

Receipt Date

Expiration Date

Quality Status

Inventory Status

Country of Origin

Certificate Number

Notes

Status

Created At

Updated At

Created By

Updated By

---

# Batch Status

Created

Received

Available

Reserved

Allocated

Consumed

Blocked

Quality Hold

Released

Completed

Archived

---

# Business Rules

Batch Number must be unique.

Every Batch belongs to one Material.

Every Batch belongs to one Warehouse.

Every Batch belongs to one Location.

A Batch cannot exist without Inventory.

Batch quantity is updated only through inventory transactions.

Consumed batches cannot receive additional inventory.

Blocked batches cannot be used.

Expired batches cannot be allocated.

Quality Hold batches cannot be consumed.

Parent / Child relationships must be preserved.

Batch history can never be deleted.

---

# Batch Traceability

Every Batch must record:

Material

Supplier

Purchase Order

Goods Receipt

Warehouse

Location

Inventory Movements

Production Orders

Quality Inspections

Transfers

Shipment

Customer

This enables complete forward and backward traceability.

---

# Batch Genealogy

Support Parent / Child Batch relationships.

Example:

Log Batch

↓

Drying Batch

↓

Thermowood Batch

↓

Finger Joint Batch

↓

Lamella Batch

↓

Massive Panel Batch

↓

Finished Product Batch

The complete genealogy must always remain accessible.

---

# Relationships

Material

↓

Batch

↓

Inventory

↓

Stock Movement

↓

Goods Receipt

↓

Production Order

↓

Quality Inspection

↓

Shipment

↓

Customer

---

# Permissions

Batch.View

Batch.Create

Batch.Update

Batch.Archive

Batch.Trace

Batch.Export

Batch.Print

---

# API

GET /batches

GET /batches/{id}

GET /batches/search

GET /batches/history/{id}

GET /batches/genealogy/{id}

GET /batches/material/{materialId}

GET /batches/inventory/{id}

POST /batches

PUT /batches/{id}

DELETE /batches/{id}

---

# UI Pages

Batch List

Batch Detail

Batch Traceability

Batch Genealogy

Batch Inventory

Batch Dashboard

Batch History

Batch Label Printing

---

# UI Components

Search Box

Material Filter

Warehouse Filter

Status Filter

Date Filter

Batch Timeline

Genealogy Tree

Inventory Grid

Barcode

QR Code

Export Button

Print Label Button

---

# Database

Table

Batches

Columns

Id

BatchNumber

MaterialId

WarehouseId

LocationId

SupplierId

PurchaseOrderId

ProductionOrderId

ParentBatchId

Quantity

AvailableQuantity

ReservedQuantity

ManufacturingDate

ReceiptDate

ExpirationDate

QualityStatus

InventoryStatus

CountryOfOrigin

CertificateNumber

Notes

Status

CreatedAt

UpdatedAt

CreatedBy

UpdatedBy

---

# Events

BatchCreated

BatchUpdated

BatchReceived

BatchTransferred

BatchReserved

BatchReleased

BatchConsumed

BatchBlocked

BatchArchived

---

# Audit

Every Batch change must record:

User

Timestamp

Previous Value

New Value

Reason

Reference Document

---

# Performance Requirements

Support:

- Pagination
- Filtering
- Sorting
- Full Text Search

Genealogy queries must execute efficiently.

Traceability reports must support large production histories.

---

# Tests

Create Batch

Duplicate Batch Validation

Batch Traceability

Batch Genealogy

Batch Search

Batch Filtering

Batch Reservation

Blocked Batch Validation

Expired Batch Validation

Quality Hold Validation

Permission Validation

Performance Test

---

# Acceptance Criteria

✔ Batch CRUD completed

✔ Unique Batch Number enforced

✔ Parent / Child genealogy implemented

✔ Full traceability implemented

✔ Batch Inventory supported

✔ Search works

✔ Filtering works

✔ Barcode supported

✔ QR Code supported

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
- Batch Service
- Genealogy Service
- Traceability Service
- DTOs
- Validators
- REST API
- Database Migration
- React Pages
- Batch Dashboard
- Unit Tests
- Integration Tests
- Swagger Documentation

---

# Cursor Implementation Prompt

Read:

- Cursor_Rules.md
- Business_Rules.md
- Factory_Flow.md
- docs/13_Design/02_Inventory/Batch.md
- This TASK document

Implement the complete Batch Management module using:

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

- Every Batch must support complete forward and backward traceability.
- Support Parent / Child Batch genealogy.
- Batch inventory must always be synchronized with Inventory.
- Support barcode and QR code labels.
- Batch history must be immutable.
- Follow project architecture exactly.
- Do not implement unrelated modules.

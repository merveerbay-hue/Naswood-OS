# Material

## Purpose

Material Master stores every raw material, semi-finished product and finished product used within Naswood OS.

---

## Business Rules

- Every material has a unique code.
- Material code cannot be changed after creation.
- Every material belongs to one category.
- Every material has one base unit.
- Materials may be batch tracked.
- Materials may be serial tracked.
- Materials may be active or inactive.

---

## Fields

Material Code

Material Name

Category

Species

Grade

Thickness

Width

Length

Moisture

Unit

Status

Barcode

Description

---

## Relationships

Material

↓

Inventory

↓

Batch

↓

Production Order

↓

Purchase Order

↓

Sales Order

---

## Workflow

Create

↓

Approve

↓

Active

↓

Inactive

↓

Archived

---

## Permissions

View

Create

Update

Delete

Export

Import

---

## Validation

Material Code Required

Material Code Unique

Name Required

Unit Required

Category Required

---

## API

GET /materials

GET /materials/{id}

POST /materials

PUT /materials/{id}

DELETE /materials/{id}

---

## UI

Material List

Material Detail

Create Material

Edit Material

Material History

---

## Acceptance Criteria

CRUD

Search

Pagination

Filtering

Import Excel

Export Excel

Audit Log

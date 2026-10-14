# INV-008 — Location List

**Module:** Inventory  
**Workspace:** Master Data  
**Screen ID:** INV-008  
**Status:** Specified  

---

## Purpose

Locations within warehouses

---

## Primary users

Warehouse Operator

---

## Components

- Entity Grid
- Warehouse filter

---

## Filters

- Warehouse
- Code
- Type
- Status

---

## Actions

- Open Location Detail
- Create Location

---

## User flow

Land from Inventory Workspace → apply filters → act or drill to related screen.

---

## Data / domain dependencies

- Inventory module design (`docs/13_Design/02_Inventory/`)
- Permissions from Navigation Permissions layer
- Compose with Component Library (Entity Grid, Master Detail, Status Badge, Metric Card)

---

## Deferred (explicit)

- Full mobile scanner flows (see Inventory_Mobile.md)
- AI recommendation widgets beyond placeholder cards

---

## Notes

Part of Inventory Screen Architecture under `docs/15_UI/Inventory`.  
Do not redefine this screen as a generic TASK CRUD page.

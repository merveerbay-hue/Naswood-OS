# INV-004 — Material List

**Module:** Inventory  
**Workspace:** Master Data  
**Screen ID:** INV-004  
**Status:** Specified  

---

## Purpose

Browse and search materials for warehouse and planning use

---

## Primary users

Warehouse Operator, Inventory Controller, Planner

---

## Components

- Entity Grid
- Search
- Status filter
- New Material action

---

## Filters

- Code
- Name
- Category
- Status
- UoM

---

## Actions

- Open Material Detail
- Create Material
- Export (deferred)

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

# INV-006 — Warehouse List

**Module:** Inventory  
**Workspace:** Master Data  
**Screen ID:** INV-006  
**Status:** Specified  

---

## Purpose

Warehouse directory for the plant

---

## Primary users

Warehouse Manager

---

## Components

- Entity Grid
- Capacity hint column

---

## Filters

- Code
- Name
- Plant
- Status

---

## Actions

- Open Warehouse Detail
- Create Warehouse

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

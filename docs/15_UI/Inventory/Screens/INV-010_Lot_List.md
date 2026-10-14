# INV-010 — Lot List

**Module:** Inventory  
**Workspace:** Stock  
**Screen ID:** INV-010  
**Status:** Specified  

---

## Purpose

Batch/lot directory with quality and expiry signals

---

## Primary users

Warehouse Operator, Quality Inspector

---

## Components

- Entity Grid
- Status Badge
- Expiry column

---

## Filters

- Material
- Lot
- Warehouse
- Status

---

## Actions

- Open Lot Trace
- Filter blocked

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

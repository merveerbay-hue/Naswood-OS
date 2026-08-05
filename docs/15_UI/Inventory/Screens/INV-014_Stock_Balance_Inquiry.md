# INV-014 — Stock Balance Inquiry

**Module:** Inventory  
**Workspace:** Stock  
**Screen ID:** INV-014  
**Status:** Specified  

---

## Purpose

On-hand / reserved / available by material-location-lot

---

## Primary users

Warehouse Operator, Planner, Inventory Controller

---

## Components

- Entity Grid
- Available qty calc
- Warehouse and Material filters

---

## Filters

- Warehouse
- Material
- Location
- Lot
- Status

---

## Actions

- Open Lot Trace
- Drill to movements (deferred)

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

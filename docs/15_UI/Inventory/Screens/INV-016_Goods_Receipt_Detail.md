# INV-016 — Goods Receipt Detail

**Module:** Inventory  
**Workspace:** Operations  
**Screen ID:** INV-016  
**Status:** Specified  

---

## Purpose

Receipt header and lines; post increases stock

---

## Primary users

Warehouse Operator

---

## Components

- Master Detail
- Lines grid
- Post/Cancel actions
- Audit Timeline stub

---

## Filters

- n/a

---

## Actions

- Add line
- Post
- Cancel
- Attach documents (deferred)

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

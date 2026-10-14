# INV-021 — Cycle Count List

**Module:** Inventory  
**Workspace:** Counts  
**Screen ID:** INV-021  
**Status:** Specified  

---

## Purpose

Count sessions and accuracy follow-up

---

## Primary users

Inventory Controller

---

## Components

- Entity Grid
- Accuracy hint

---

## Filters

- Count No
- Warehouse
- Status
- Due

---

## Actions

- Open Count Detail
- Create

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

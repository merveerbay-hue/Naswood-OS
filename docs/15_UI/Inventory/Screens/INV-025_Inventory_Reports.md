# INV-025 — Inventory Reports

**Module:** Inventory  
**Workspace:** Reports  
**Screen ID:** INV-025  
**Status:** Specified  

---

## Purpose

Operational inventory report entry points

---

## Primary users

Warehouse Manager, Plant Manager

---

## Components

- Report launcher cards

---

## Filters

- Plant
- Warehouse
- Period

---

## Actions

- Run report
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

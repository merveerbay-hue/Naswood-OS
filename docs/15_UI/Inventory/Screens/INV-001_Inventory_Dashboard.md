# INV-001 — Inventory Dashboard

**Module:** Inventory  
**Workspace:** Dashboard  
**Screen ID:** INV-001  
**Status:** Specified  

---

## Purpose

Warehouse health cockpit with KPIs, operations queues, and health alerts

---

## Primary users

Plant Manager, Warehouse Manager, Warehouse Operator

---

## Components

- KPI cards
- Operations queue cards
- Health alerts
- Shortcuts to workspaces

---

## Filters

- Plant
- Warehouse
- Date range

---

## Actions

- Drill to Stock Balance
- Drill to Goods Receipt
- Refresh

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

# INV-005 — Material Detail

**Module:** Inventory  
**Workspace:** Master Data  
**Screen ID:** INV-005  
**Status:** Specified  

---

## Purpose

Material master header with classification, UoM, and status

---

## Primary users

Inventory Controller, Planner

---

## Components

- Master Detail header
- Status Badge
- Tabs (General
- Stock summary
- Documents)

---

## Filters

- n/a

---

## Actions

- Edit
- Activate/Deactivate
- View Stock Balance

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

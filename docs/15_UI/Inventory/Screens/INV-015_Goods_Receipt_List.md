# INV-015 — Goods Receipt List

**Module:** Inventory  
**Workspace:** Operations  
**Screen ID:** INV-015  
**Status:** Specified  

---

## Purpose

Inbound receipt documents awaiting or completed post

---

## Primary users

Warehouse Operator, Receiver

---

## Components

- Entity Grid
- Status chips (Draft/Posted)
- New Receipt

---

## Filters

- Document No
- Status
- Date
- Warehouse

---

## Actions

- Open Receipt Detail
- Create
- Post (on detail)

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

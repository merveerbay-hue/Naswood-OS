# INV-024 — Inventory Adjustment

**Module:** Inventory  
**Workspace:** Counts  
**Screen ID:** INV-024  
**Status:** Specified  

---

## Purpose

Adjustment documents for approved variances

---

## Primary users

Inventory Controller, Warehouse Manager

---

## Components

- Entity Grid + Detail actions

---

## Filters

- Document No
- Reason
- Status

---

## Actions

- Create
- Post (permission gated)

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

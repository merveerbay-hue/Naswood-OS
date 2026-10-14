# PRD-018 — Finished Goods

**Module:** Production  
**Workspace:** Execution  
**Screen ID:** PRD-018  
**Status:** Specified  
**Implementation TASKs (slices):** TASK-062

---

## Purpose

Receive finished goods into inventory from production.

---

## Primary users

Operator, Warehouse

---

## Components

- FG receipt list
- Detail with qty/lot
- Quality hold indicator

---

## Filters

- Order
- Warehouse
- Status

---

## Actions

- Receive
- Hold
- Release to stock

---

## User flow

- Packaging/Confirmation → FG → Inventory

---

## Data / domain dependencies

- Production domain aggregates and contracts relevant to this screen
- Plant / Company context from shell
- Permissions enforced per action

---

## Deferred (explicit)

- Auto GR from confirmation

---

## Notes

This screen is part of Production Screen Architecture under `docs/15_UI`.  
An Implementation TASK may deliver only a slice; it must not redefine this screen as a generic CRUD page.

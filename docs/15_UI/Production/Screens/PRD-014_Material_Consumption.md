# PRD-014 — Material Consumption

**Module:** Production  
**Workspace:** Execution  
**Screen ID:** PRD-014  
**Status:** Specified  
**Implementation TASKs (slices):** TASK-058

---

## Purpose

Issue / backflush components against production/work orders.

---

## Primary users

Operator, Warehouse, Supervisor

---

## Components

- Order selector
- Component demand vs issued
- Scan/lot entry
- Posting log

---

## Filters

- Production Order
- WO
- Material
- Status

---

## Actions

- Issue
- Reverse
- Backflush run

---

## User flow

- From Operator Panel or standalone → posts inventory movement

---

## Data / domain dependencies

- Production domain aggregates and contracts relevant to this screen
- Plant / Company context from shell
- Permissions enforced per action

---

## Deferred (explicit)

- Directed picking

---

## Notes

This screen is part of Production Screen Architecture under `docs/15_UI`.  
An Implementation TASK may deliver only a slice; it must not redefine this screen as a generic CRUD page.

# PRD-012 — Work Order

**Module:** Production  
**Workspace:** Planning  
**Screen ID:** PRD-012  
**Status:** Specified  
**Implementation TASKs (slices):** TASK-057

---

## Purpose

Shop-floor executable unit derived from production order operations.

---

## Primary users

Supervisor, Planner

---

## Components

- List
- Detail
- Assignment (machine/line/shift)
- Status

---

## Filters

- Status
- Machine
- Shift
- Production Order

---

## Actions

- Assign
- Release
- Start
- Complete
- Split

---

## User flow

- From PO Detail → WO → Dispatch List → Operator Panel

---

## Data / domain dependencies

- Production domain aggregates and contracts relevant to this screen
- Plant / Company context from shell
- Permissions enforced per action

---

## Deferred (explicit)

- Split/merge WO UI

---

## Notes

This screen is part of Production Screen Architecture under `docs/15_UI`.  
An Implementation TASK may deliver only a slice; it must not redefine this screen as a generic CRUD page.

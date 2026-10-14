# PRD-010 — Production Order List

**Module:** Production  
**Workspace:** Planning  
**Screen ID:** PRD-010  
**Status:** Specified  
**Implementation TASKs (slices):** TASK-056

---

## Purpose

Plan and control manufacturing orders prior to and during release.

---

## Primary users

Planner, Supervisor

---

## Components

- Grid
- Status pipeline tabs
- KPI strip
- Create

---

## Filters

- Status
- Product
- Line
- Date
- Priority

---

## Actions

- Open
- Create
- Release
- Hold
- Cancel
- Print

---

## User flow

- Create → Detail → Release → Dispatch List → Execution

---

## Data / domain dependencies

- Production domain aggregates and contracts relevant to this screen
- Plant / Company context from shell
- Permissions enforced per action

---

## Deferred (explicit)

- Gantt from list

---

## Notes

This screen is part of Production Screen Architecture under `docs/15_UI`.  
An Implementation TASK may deliver only a slice; it must not redefine this screen as a generic CRUD page.

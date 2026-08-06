# PRD-016 — WIP Tracking

**Module:** Production  
**Workspace:** Execution  
**Screen ID:** PRD-016  
**Status:** Specified  
**Implementation TASKs (slices):** TASK-060

---

## Purpose

Visibility of work-in-process quantities by order, operation, location.

---

## Primary users

Supervisor, Planner

---

## Components

- WIP board/grid
- Aging
- Bottleneck highlight

---

## Filters

- Line
- Order
- Operation
- Age

---

## Actions

- Drill to Order
- Drill to WO
- Move WIP (controlled)

---

## User flow

- Monitor → act on stalled WO

---

## Data / domain dependencies

- Production domain aggregates and contracts relevant to this screen
- Plant / Company context from shell
- Permissions enforced per action

---

## Deferred (explicit)

- Kanban board UI

---

## Notes

This screen is part of Production Screen Architecture under `docs/15_UI`.  
An Implementation TASK may deliver only a slice; it must not redefine this screen as a generic CRUD page.

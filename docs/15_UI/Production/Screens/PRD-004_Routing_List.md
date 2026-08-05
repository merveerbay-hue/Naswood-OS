# PRD-004 — Routing List

**Module:** Production  
**Workspace:** Master Data  
**Screen ID:** PRD-004  
**Status:** Specified  
**Implementation TASKs (slices):** TASK-047

---

## Purpose

Browse manufacturing routings (operation sequences).

---

## Primary users

Manufacturing Engineer, Planner

---

## Components

- Grid
- Status badges
- Create

---

## Filters

- Product
- Plant
- Work Center
- Status

---

## Actions

- Open
- Create
- Duplicate
- Export

---

## User flow

- List → Detail
- List → Create

---

## Data / domain dependencies

- Production domain aggregates and contracts relevant to this screen
- Plant / Company context from shell
- Permissions enforced per action

---

## Deferred (explicit)

- —

---

## Notes

This screen is part of Production Screen Architecture under `docs/15_UI`.  
An Implementation TASK may deliver only a slice; it must not redefine this screen as a generic CRUD page.

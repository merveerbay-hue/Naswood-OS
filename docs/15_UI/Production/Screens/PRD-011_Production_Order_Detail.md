# PRD-011 — Production Order Detail

**Module:** Production  
**Workspace:** Planning  
**Screen ID:** PRD-011  
**Status:** Specified  
**Implementation TASKs (slices):** TASK-056, TASK-057, TASK-059

---

## Purpose

Full order context: quantities, ops, components, related WOs, progress.

---

## Primary users

Planner, Supervisor

---

## Components

- Header
- Operations tab
- Components tab
- Work Orders tab
- Confirmations tab
- Scrap/Rework tab
- Timeline

---

## Filters

- —

---

## Actions

- Edit
- Release
- Schedule
- Hold
- Close
- Create WO
- Open Dispatch

---

## User flow

- Detail is hub into WO, Confirmation, Consumption

---

## Data / domain dependencies

- Production domain aggregates and contracts relevant to this screen
- Plant / Company context from shell
- Permissions enforced per action

---

## Deferred (explicit)

- Embedded scheduling widget

---

## Notes

This screen is part of Production Screen Architecture under `docs/15_UI`.  
An Implementation TASK may deliver only a slice; it must not redefine this screen as a generic CRUD page.

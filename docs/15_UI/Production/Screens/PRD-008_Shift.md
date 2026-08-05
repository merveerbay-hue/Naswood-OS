# PRD-008 — Shift

**Module:** Production  
**Workspace:** Master Data  
**Screen ID:** PRD-008  
**Status:** Specified  
**Implementation TASKs (slices):** TASK-051

---

## Purpose

Define shift patterns used by calendar and capacity.

---

## Primary users

Planner, Supervisor

---

## Components

- List
- Detail (start/end, breaks, crew size)

---

## Filters

- Plant
- Type

---

## Actions

- Create
- Edit
- Activate

---

## User flow

- Shift → Calendar → Capacity Planning

---

## Data / domain dependencies

- Production domain aggregates and contracts relevant to this screen
- Plant / Company context from shell
- Permissions enforced per action

---

## Deferred (explicit)

- Rotation patterns UI

---

## Notes

This screen is part of Production Screen Architecture under `docs/15_UI`.  
An Implementation TASK may deliver only a slice; it must not redefine this screen as a generic CRUD page.

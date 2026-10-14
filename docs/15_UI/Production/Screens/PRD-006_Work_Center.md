# PRD-006 — Work Center

**Module:** Production  
**Workspace:** Master Data  
**Screen ID:** PRD-006  
**Status:** Specified  
**Implementation TASKs (slices):** TASK-049

---

## Purpose

Maintain work center capacity and calendar assignment.

---

## Primary users

Manufacturing Engineer, Supervisor

---

## Components

- List+Detail pattern
- Capacity fields
- Linked machines
- Calendar ref

---

## Filters

- Plant
- Line
- Status

---

## Actions

- Create
- Edit
- Activate/Deactivate

---

## User flow

- Master setup → used in Routing and Scheduling

---

## Data / domain dependencies

- Production domain aggregates and contracts relevant to this screen
- Plant / Company context from shell
- Permissions enforced per action

---

## Deferred (explicit)

- Utilization chart on detail

---

## Notes

This screen is part of Production Screen Architecture under `docs/15_UI`.  
An Implementation TASK may deliver only a slice; it must not redefine this screen as a generic CRUD page.

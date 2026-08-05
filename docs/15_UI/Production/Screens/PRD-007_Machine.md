# PRD-007 — Machine

**Module:** Production  
**Workspace:** Master Data  
**Screen ID:** PRD-007  
**Status:** Specified  
**Implementation TASKs (slices):** TASK-048

---

## Purpose

Maintain machine master used by scheduling and machine panel.

---

## Primary users

Manufacturing Engineer, Maintenance bridge

---

## Components

- List
- Detail (status, work center, OEE target)
- Alarm config link

---

## Filters

- Work Center
- Status
- Line

---

## Actions

- Create
- Edit
- Set status

---

## User flow

- Master → Machine Panel / Maintenance Asset link

---

## Data / domain dependencies

- Production domain aggregates and contracts relevant to this screen
- Plant / Company context from shell
- Permissions enforced per action

---

## Deferred (explicit)

- Live telemetry pane

---

## Notes

This screen is part of Production Screen Architecture under `docs/15_UI`.  
An Implementation TASK may deliver only a slice; it must not redefine this screen as a generic CRUD page.

# PRD-005 — Routing Detail

**Module:** Production  
**Workspace:** Master Data  
**Screen ID:** PRD-005  
**Status:** Specified  
**Implementation TASKs (slices):** TASK-047, TASK-054

---

## Purpose

Define operation sequence, work centers, times and tooling refs.

---

## Primary users

Manufacturing Engineer

---

## Components

- Header
- Operation steps grid
- Time standards
- Work center assignment
- Linked BOM

---

## Filters

- —

---

## Actions

- Add operation
- Reorder
- Release
- Retire

---

## User flow

- Detail maintained → referenced by Production/Work Orders

---

## Data / domain dependencies

- Production domain aggregates and contracts relevant to this screen
- Plant / Company context from shell
- Permissions enforced per action

---

## Deferred (explicit)

- Graphical routing editor

---

## Notes

This screen is part of Production Screen Architecture under `docs/15_UI`.  
An Implementation TASK may deliver only a slice; it must not redefine this screen as a generic CRUD page.

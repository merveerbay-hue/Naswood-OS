# PRD-002 — BOM List

**Module:** Production  
**Workspace:** Master Data  
**Screen ID:** PRD-002  
**Status:** Specified  
**Implementation TASKs (slices):** TASK-046

---

## Purpose

Find and manage Bills of Material used for manufacturing.

---

## Primary users

Manufacturing Engineer, Planner

---

## Components

- Search bar
- Filter chips
- Data grid
- Bulk actions toolbar
- Create button

---

## Filters

- Product
- Plant
- Status
- Revision
- Effective date

---

## Actions

- Open
- Create
- Duplicate
- Export
- Obsolete

---

## User flow

- Search → open Detail
- Create → Create BOM

---

## Data / domain dependencies

- Production domain aggregates and contracts relevant to this screen
- Plant / Company context from shell
- Permissions enforced per action

---

## Deferred (explicit)

- Multi-select mass release

---

## Notes

This screen is part of Production Screen Architecture under `docs/15_UI`.  
An Implementation TASK may deliver only a slice; it must not redefine this screen as a generic CRUD page.

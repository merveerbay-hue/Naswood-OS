# PRD-009 — Production Calendar

**Module:** Production  
**Workspace:** Master Data  
**Screen ID:** PRD-009  
**Status:** Specified  
**Implementation TASKs (slices):** TASK-052

---

## Purpose

Official working calendar: working days, holidays, shutdowns.

---

## Primary users

Planner

---

## Components

- Calendar view
- Exception list
- Shift overlay

---

## Filters

- Plant
- Year
- Calendar type

---

## Actions

- Add holiday
- Add shutdown
- Publish
- Revise

---

## User flow

- Configure → publish → consumed by Scheduling

---

## Data / domain dependencies

- Production domain aggregates and contracts relevant to this screen
- Plant / Company context from shell
- Permissions enforced per action

---

## Deferred (explicit)

- Plant sync from HR calendar

---

## Notes

This screen is part of Production Screen Architecture under `docs/15_UI`.  
An Implementation TASK may deliver only a slice; it must not redefine this screen as a generic CRUD page.

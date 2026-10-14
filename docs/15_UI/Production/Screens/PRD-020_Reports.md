# PRD-020 — Production Reports

**Module:** Production  
**Workspace:** Reports  
**Screen ID:** PRD-020  
**Status:** Specified  
**Implementation TASKs (slices):** TASK-065

---

## Purpose

Standard operational/compliance reports hub.

---

## Primary users

Supervisor, Planner, Compliance

---

## Components

- Report catalog
- Parameter form
- Result viewer
- Schedule send

---

## Filters

- Report type
- Period
- Plant

---

## Actions

- Run
- Export PDF/XLSX
- Subscribe

---

## User flow

- Pick report → parameters → run

---

## Data / domain dependencies

- Production domain aggregates and contracts relevant to this screen
- Plant / Company context from shell
- Permissions enforced per action

---

## Deferred (explicit)

- Scheduled email delivery

---

## Notes

This screen is part of Production Screen Architecture under `docs/15_UI`.  
An Implementation TASK may deliver only a slice; it must not redefine this screen as a generic CRUD page.

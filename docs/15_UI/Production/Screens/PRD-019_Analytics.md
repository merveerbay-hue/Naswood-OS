# PRD-019 — Production Analytics

**Module:** Production  
**Workspace:** Analytics  
**Screen ID:** PRD-019  
**Status:** Specified  
**Implementation TASKs (slices):** TASK-065

---

## Purpose

Analytical views beyond operational dashboard (trends, comparisons).

---

## Primary users

Plant Manager, Analyst

---

## Components

- Trend charts
- OEE breakdown
- Scrap Pareto
- Export

---

## Filters

- Plant
- Line
- Period
- Product family

---

## Actions

- Export
- Save view
- Drill to order set

---

## User flow

- Dashboard → Analytics for deeper review

---

## Data / domain dependencies

- Production domain aggregates and contracts relevant to this screen
- Plant / Company context from shell
- Permissions enforced per action

---

## Deferred (explicit)

- Embedded BI semantic model

---

## Notes

This screen is part of Production Screen Architecture under `docs/15_UI`.  
An Implementation TASK may deliver only a slice; it must not redefine this screen as a generic CRUD page.

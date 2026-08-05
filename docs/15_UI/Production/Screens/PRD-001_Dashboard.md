# PRD-001 — Production Dashboard

**Module:** Production  
**Workspace:** Dashboard  
**Screen ID:** PRD-001  
**Status:** Specified  
**Implementation TASKs (slices):** TASK-065

---

## Purpose

Plant/line operational cockpit: open work, bottlenecks, quality/scrap signals, and deep-links into Planning and Execution.

---

## Primary users

Plant Manager, Supervisor, Planner

---

## Components

- KPI cards (Open POs, Active WOs, WIP qty, Scrap rate, OEE snapshot)
- Alert list
- Line status strip
- Shortcuts to Dispatch / Operator Panel / Order List

---

## Filters

- Company
- Plant
- Production Line
- Date range
- Shift

---

## Actions

- Open alert
- Drill to Production Order
- Drill to Work Order
- Refresh

---

## User flow

- Land from shell → review KPIs → drill to exception → resolve in Execution

---

## Data / domain dependencies

- Production domain aggregates and contracts relevant to this screen
- Plant / Company context from shell
- Permissions enforced per action

---

## Deferred (explicit)

- Real-time SignalR widgets
- Full OEE heatmap

---

## Notes

This screen is part of Production Screen Architecture under `docs/15_UI`.  
An Implementation TASK may deliver only a slice; it must not redefine this screen as a generic CRUD page.

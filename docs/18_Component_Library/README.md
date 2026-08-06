# 18 — Component Library

**Layer:** Enterprise UI building blocks  
**Status:** Active  
**Sits above:** Design System primitives (buttons, inputs, tokens)  
**Sits below:** Screen Architecture (`15_UI`) which composes these blocks

---

## Purpose

NOS screens are not assembled from raw HTML controls alone.

They compose **enterprise patterns** shared across modules:

| Component | Doc |
|-----------|-----|
| Entity Grid | [Entity_Grid.md](Entity_Grid.md) |
| Master Detail | [Master_Detail.md](Master_Detail.md) |
| Wizard | [Wizard.md](Wizard.md) |
| Timeline | [Timeline.md](Timeline.md) |
| Kanban | [Kanban.md](Kanban.md) |
| Tree | [Tree.md](Tree.md) |
| Scheduler | [Scheduler.md](Scheduler.md) |
| Dashboard Card | [Dashboard_Card.md](Dashboard_Card.md) |
| Metric Card | [Metric_Card.md](Metric_Card.md) |
| Chart | [Chart.md](Chart.md) |
| Status Badge | [Status_Badge.md](Status_Badge.md) |
| Approval Flow | [Approval_Flow.md](Approval_Flow.md) |
| Audit Timeline | [Audit_Timeline.md](Audit_Timeline.md) |

---

## Relationship to Design System

| Layer | Owns |
|-------|------|
| `16_Design_System` / `13_Design/.../Design_System` | Tokens, buttons, inputs, tables primitive, cards primitive |
| `18_Component_Library` | Domain-ready compositions (filterable entity grid, WO kanban, approval strip) |

Do not re-specify colors/spacing here — reference Design System.

---

## Agent rule

When a screen PRD lists “components”, prefer library names from this folder.
Do not invent a one-off CRUD form layout when Master Detail + Entity Grid apply.

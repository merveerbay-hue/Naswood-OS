# Production — UI Information Architecture

**Module:** Production  
**Status:** Exemplar (use as pattern for other modules)  
**Domain refs:** `docs/05_Modules/02_Production/`, `docs/13_Design/05_Production/`

---

## Module purpose

Production plans, releases, executes and monitors manufacturing work — from master definitions (BOM, routing, resources) through production/work orders to confirmations, WIP, scrap, packaging and finished goods.

---

## User roles (primary)

| Role | Jobs in this module |
|------|---------------------|
| Production Planner | Calendar, capacity, release orders |
| Supervisor | Dispatch, monitor WIP, exceptions |
| Operator | Terminal confirmations, consumption |
| Manufacturing Engineer | BOM, routing, parameters, tooling |
| Plant Manager | Dashboard, reports, bottlenecks |

---

## Workspaces

See [Workspaces.md](Workspaces.md).

```text
Production
├── Dashboard
├── Planning
├── Execution
├── Master Data
├── Monitoring
└── Reports
```

---

## Capability index

| Capability | Workspace | IA detail | Impl TASK (entry) |
|------------|-----------|-----------|-------------------|
| Production Dashboard | Dashboard | Workspaces.md | TASK-065 |
| Production Calendar / Shift | Planning | Workspaces.md | TASK-051, 052 |
| Production Order | Execution | [Production_Order.md](Production_Order.md) | TASK-056 |
| Work Order | Execution | Production_Order.md | TASK-057 |
| BOM | Master Data | [BOM.md](BOM.md) | TASK-046 |
| Routing / Operation | Master Data | Workspaces.md | TASK-047, 054 |
| Machine / Work Center / Line | Master Data | Workspaces.md | TASK-048–050 |
| Tooling / Parameters | Master Data | Workspaces.md | TASK-053, 055 |
| Confirmations / Consumption | Monitoring / Execution | Workspaces.md | TASK-058, 059 |
| WIP / Packaging / FG / Scrap / Rework | Monitoring | Workspaces.md | TASK-060–064 |

---

## Current gap (honest)

Implementation through Sprint 05 largely exposed **flat CRUD ResourcePages**.

That is **not** the Production product shape. Convergence target is this IA: workspaces, List/Detail families, terminals, and monitoring boards.

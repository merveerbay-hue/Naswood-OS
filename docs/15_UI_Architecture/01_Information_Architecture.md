# Global UI Information Architecture

**Status:** Active  
**Scope:** All business modules in Naswood OS

---

## Structural model

```text
Application Shell
    ↓
Module              (Production, Inventory, Quality, …)
    ↓
Workspace           (Planning, Execution, Master Data, Monitoring, Reports, …)
    ↓
Capability          (BOM, Production Order, NCR, Asset, …)
    ↓
Screen family       (List, Detail, Create, …)
    ↓
Components / panes
    ↓
Workflows
    ↓
Implementation TASK
```

This matches Design System layout hierarchy:

`Platform → Module → Workspace → Entity/Capability → Detail → Action`  
(`docs/13_Design/00_Platform/Design_System/03_Layout/Navigation.md`)

---

## Standard workspace types (reusable)

Most factory modules use some or all of:

| Workspace | User intent |
|-----------|-------------|
| **Dashboard / Monitoring** | See status, alerts, KPIs |
| **Planning** | Schedule, capacity, calendars |
| **Execution / Operations** | Run daily transactional work |
| **Master Data** | Stable definitions (BOM, routing, assets, materials) |
| **Analytics / Reports** | Historical and compliance views |
| **Administration** (module-local) | Module settings, parameters |

Not every module needs every workspace. Empty workspaces must not appear in navigation.

---

## Module → primary workspaces

| Module | Workspaces (product shape) |
|--------|----------------------------|
| Production | Dashboard, Planning, Execution, Master Data, Monitoring, Reports |
| Inventory | Overview, Operations, Master Data, Counts & Adjustments, Reports |
| Purchasing | Dashboard, Sourcing, Orders, Inbound, Master Data, Reports |
| Sales / CRM | Dashboard, Pipeline, Orders, Fulfillment, Master Data, Reports |
| Quality | Dashboard, Operations (Inspections/NCR), Plans, Lab, Reports |
| Maintenance | Dashboard, Assets, Work Management, Planning, Spare Parts, Reports |
| Finance | Dashboard, Master Data, Transactions, Period Close, Reports |
| Platform / Admin | Shell, Identity, Authorization, Files, Settings, Audit |

Detailed trees live under each module folder in `15_UI_Architecture/`.

---

## Capability vs screen vs TASK

| Term | Meaning |
|------|---------|
| **Capability** | Business ability (e.g. BOM, Non-Conformance) owned by a module |
| **Screen family** | Set of UIs that make the capability usable |
| **Screen** | One navigable surface (BOM List, NCR Detail) |
| **TASK** | Implementation work package delivering API/UI/workflow slices |

One capability → many screens.  
One screen → one or more TASKs over time.  
One TASK → never defines the whole module IA.

---

## Anti-patterns

1. **Entity = menu item = single ResourcePage** for every aggregate  
2. **Sprint TASK order** treated as navigation order  
3. **Skipping Detail / History / Workflow** panes forever without documenting deferral  
4. **Dashboard as empty KPI stubs** with no link into Execution workspaces  

---

## Relationship to `04_Application`

- `Screen_Catalog.md` — flat inventory of screen **names**  
- `UI_Flows.md` — process navigation between screens  
- `15_UI_Architecture` — **ownership tree** (who owns what workspace/screen family)

When they conflict, resolve in this order: Constitution → Domain module ownership → **UI Architecture** → Screen Catalog update → TASK update.

# 15 — UI Architecture

**Layer:** Product UI Information Architecture  
**Status:** Active  
**Authority:** Below Design System interaction standards; above Implementation TASKs

---

## Purpose

This folder defines **how Naswood OS is organized for users** — modules, workspaces, screens, and workflows — **before** any Implementation TASK is written or coded.

It exists to prevent the anti-pattern:

```text
TASK-046 → one CRUD screen
```

and replace it with:

```text
Module → Workspace → Screen family → Components → Workflow → Implementation TASK
```

---

## Governing hierarchy (NOS)

```text
Constitution
    ↓
Architecture
    ↓
Business Domain (05_Modules / domain docs)
    ↓
UI Information Architecture   ← this folder (15)
    ↓
Screen Architecture / Design specs (13_Design)
    ↓
User Flows (04_Application/UI_Flows + 17_User_Flows index)
    ↓
Navigation
    ↓
Design System (13_Design/.../Design_System + 16_Design_System index)
    ↓
Implementation TASK (14_Implementation)
    ↓
Source Code
```

**TASK is the lowest planning unit.** A TASK implements slices of already-defined screens and workflows. A TASK is never itself the product structure.

---

## Contents

| Path | Role |
|------|------|
| [00_Governing_Principles.md](00_Governing_Principles.md) | Anti-patterns, required thinking order, TASK rules |
| [01_Information_Architecture.md](01_Information_Architecture.md) | Global Module → Workspace model |
| [02_Navigation_Map.md](02_Navigation_Map.md) | Product navigation tree (workspaces, not entity CRUD lists) |
| [03_Screen_to_Task_Mapping.md](03_Screen_to_Task_Mapping.md) | How TASKs attach under screens |
| [Production/](Production/) | **Exemplar** — full Production IA |
| [Inventory/](Inventory/) | Workspace + screen-family map |
| [Quality/](Quality/) | Workspace + NCR family map |
| [Maintenance/](Maintenance/) | Workspace + Asset family map |
| [Purchasing/](Purchasing/) | Workspace map |
| [Sales/](Sales/) | Workspace map |

---

## Related sources of truth

| Concern | Canonical location |
|---------|-------------------|
| Screen registry (names) | `docs/04_Application/Screen_Catalog.md` |
| End-to-end UI flows | `docs/04_Application/UI_Flows.md` |
| Workspace / Navigation layout rules | `docs/13_Design/00_Platform/Design_System/03_Layout/` |
| Module business meaning | `docs/05_Modules/` |
| Implementation work packages | `docs/14_Implementation/` |

`15_UI_Architecture` **owns structure and ownership**. Screen Catalog remains the flat registry; UI Flows remain process paths; Design System owns visual/interaction standards.

---

## Agent / contributor rule

Before creating or coding any business UI TASK:

1. Identify the **Module** and **Workspace**.
2. Identify the **Screen family** (List / Detail / Create / related panes).
3. Identify the **User jobs** (not only CRUD).
4. Only then open or write an Implementation TASK that references those screens.

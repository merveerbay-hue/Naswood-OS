# NOS Product Documentation Layers

**Status:** Active roadmap  
**Replaces as delivery driver:** `14_Implementation` TASK chain

---

## Stack

```text
AI/NOS_CONSTITUTION/          Authority + AI execution protocol
docs/
├── 13_Design/                ✅ Backend / module design (Architecture, Workflow, API, Dashboard, Mobile)
├── 14_Implementation/        ✅ FROZEN — historical TASK archives only
├── 15_UI_Architecture/       ✅ Module → Workspace IA
├── 15_UI/                    ✅ Screen Architecture (PRD / QLT / MNT / INV / SAL / PUR / CRM)
├── 16_Design_System/         ← Visual & interaction standards (index + topics → canonical DS)
├── 17_User_Flows/            ← Cross-screen jobs
├── 18_Component_Library/     ← Enterprise building blocks (Entity Grid, Kanban, …)
├── 19_Navigation/            ← Menu, permissions, workspace chrome, breadcrumbs
└── 20_Frontend_Architecture/ ← App shell, routing, state, module FE boundaries
```

---

## Delivery chain (mandatory)

```text
Architecture
    ↓
Module
    ↓
Workspace
    ↓
Navigation
    ↓
Screens
    ↓
Components
    ↓
User Flow
    ↓
Frontend
```

**Not:**

```text
Architecture → TASK → TASK → TASK
```

---

## How to ask Cursor (examples)

| Avoid | Prefer |
|-------|--------|
| “TASK-078’i yap” | “Maintenance Workspace’i oluştur” |
| “TASK-056 CRUD” | “Production Orders List + Detail (PRD-010/011)” |
| “NCR ekranı ekle” | “Quality Non-Conformance screen family + CAPA flow” |

---

## AI entry

[`AI/NOS_CONSTITUTION/00_AI_EXECUTION.md`](../AI/NOS_CONSTITUTION/00_AI_EXECUTION.md)

# 15_UI — Screen Architecture

**Layer:** Screen Architecture (enterprise ERP/MES UX)  
**Status:** Active  
**Sits above:** Implementation TASKs (`14_Implementation`)  
**Sits beside:** UI Information Architecture (`15_UI_Architecture`)

---

## Purpose

Define **named screens** the way SAP / Dynamics / Infor / IFS / Opcenter do — not “one TASK = one CRUD page”.

```text
Module
  └── Workspace
        └── Screen (PRD-xxx / QLT-xxx / …)
              ├── Purpose
              ├── Components
              ├── Filters
              ├── Actions
              └── User flow
                    └── Implementation TASK (slice)
```

---

## Module screen libraries

| Module | Folder | Target scale |
|--------|--------|--------------|
| Production | [Production/](Production/) | ~35–40 screens |
| Quality | [Quality/](Quality/) | ~20 screens |
| Maintenance | [Maintenance/](Maintenance/) | ~25 screens |
| Inventory | [Inventory/](Inventory/) | ~30 screens |
| Sales | [Sales/](Sales/) | ~15–20 screens |
| Purchasing | [Purchasing/](Purchasing/) | ~15–20 screens |

Production includes full PRD specs for the core 20 screens plus planned stubs to reach MES breadth.

---

## Related layers

| Layer | Path |
|-------|------|
| Workspace / nav IA | `docs/15_UI_Architecture/` |
| Screen name registry | `docs/04_Application/Screen_Catalog.md` |
| Flows | `docs/17_User_Flows/` → `04_Application/UI_Flows.md` |
| Design System | `docs/16_Design_System/` |
| TASKs | `docs/14_Implementation/` |

---

## Agent rule

Before building UI for a capability:

1. Open the screen PRD (e.g. `PRD-011_Production_Order_Detail.md`)  
2. Implement components/actions listed there (or a declared MVP subset)  
3. Link the Implementation TASK as a **slice**, never as the screen definition

# 15_UI — Screen Architecture

**Layer:** Named enterprise screens  
**Status:** Active  
**Sits with:** UI Architecture (`15_UI_Architecture`), Navigation (`19`)  
**Above:** Frontend (`20`) · **Frozen below as driver:** `14_Implementation`

---

## Purpose

Define screens the way SAP / Dynamics / Infor / IFS / Opcenter do — not “one TASK = one CRUD page”.

```text
Module → Workspace → Screen (PRD-xxx / QLT-xxx / MNT-xxx / …)
              ├── Purpose
              ├── Components (see 18_Component_Library)
              ├── Filters
              ├── Actions
              └── User flow (see 17_User_Flows)
```

---

## Module screen libraries

| Module | Folder | Target scale |
|--------|--------|--------------|
| Production | [Production/](Production/) | ~35–40 screens |
| Quality | [Quality/](Quality/) | ~20 |
| Maintenance | [Maintenance/](Maintenance/) | ~25 |
| Inventory | [Inventory/](Inventory/) | ~30 |
| Sales | [Sales/](Sales/) | ~15–20 |
| Purchasing | [Purchasing/](Purchasing/) | ~15–20 |
| CRM | [CRM/](CRM/) | planned |

---

## Agent rule

When asked to build product UI, open the **workspace** then the **screen PRD**, then implement.  
Do not open work from a frozen TASK id.

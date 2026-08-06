# 15 — UI Architecture

**Layer:** Product UI Information Architecture  
**Status:** Active  
**Delivery driver:** Yes (with `15_UI`, `19_Navigation`, `17_User_Flows`)  
**Not a driver:** `14_Implementation` (frozen)

---

## Purpose

Organize NOS for users — modules, workspaces, screen families — **before** frontend work.

```text
Architecture → Module → Workspace → Navigation → Screens → Components → User Flow → Frontend
```

Anti-pattern ended:

```text
Architecture → TASK → TASK → TASK
```

---

## Contents

| Path | Role |
|------|------|
| [00_Governing_Principles.md](00_Governing_Principles.md) | Required thinking order |
| [01_Information_Architecture.md](01_Information_Architecture.md) | Global Module → Workspace model |
| [02_Navigation_Map.md](02_Navigation_Map.md) | IA tree (detail in `19_Navigation`) |
| [03_Screen_to_Task_Mapping.md](03_Screen_to_Task_Mapping.md) | Historical TASK mapping — archive rules |
| [Production/](Production/) … module IA folders | Workspace maps |

Screen specs → **[`docs/15_UI/`](../15_UI/)**  
Product stack → **[`docs/PRODUCT_LAYERS.md`](../PRODUCT_LAYERS.md)**

---

## Agent rule

Before coding business UI:

1. Identify **Module** and **Workspace**
2. Confirm **Navigation** (`19_Navigation`)
3. Open **Screen PRDs** (`15_UI`)
4. Attach **User Flow** (`17_User_Flows`)
5. Compose with **Component Library** + Design System
6. Place code per **Frontend Architecture** (`20`)

Do **not** create new TASK files under `14_Implementation`.

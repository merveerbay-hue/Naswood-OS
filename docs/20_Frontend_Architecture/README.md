# 20 — Frontend Architecture

**Layer:** How the web (and future client) apps realize product layers  
**Status:** Active (steering)  
**Depends on:** `15_UI*`, `16`, `17`, `18`, `19`  
**Does not redefine:** Business rules, workflows, or screen purpose

---

## Contents

| Document | Role |
|----------|------|
| [Overview.md](Overview.md) | FE boundaries and principles |
| [App_Shell.md](App_Shell.md) | Shell, auth gate, module loading |
| [Routing.md](Routing.md) | Route shape from Module → Workspace → Screen |
| [State_Data.md](State_Data.md) | Server state, forms, optimistic rules |
| [Module_Boundaries.md](Module_Boundaries.md) | FE folder / package boundaries per module |

---

## Delivery rule

Frontend work starts from **Workspace + Screen PRD + Component Library**, not from TASK files.

```text
15_UI_Architecture (workspace)
  → 19_Navigation (menu/route)
  → 15_UI (screen PRD)
  → 18_Component_Library (blocks)
  → 16_Design_System (tokens)
  → 20_Frontend_Architecture (how to place code)
  → apps/web source
```

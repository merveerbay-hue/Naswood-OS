# 17 — User Flows

**Layer:** End-to-end jobs across screens  
**Status:** Active product layer  
**Registry (legacy flat):** [`docs/04_Application/UI_Flows.md`](../04_Application/UI_Flows.md)

---

## Purpose

Flows answer: **how does a role complete a job** across workspaces and screens?

They reference Screen IDs from `15_UI`, not TASK ids.

```text
17_User_Flows/
  Production_Flow.md
  Planning_Flow.md
  Inventory_Flow.md
  Maintenance_Flow.md
  Quality_Flow.md
  Sales_Flow.md
  Purchasing_Flow.md
```

---

## Flow index

| Flow | File |
|------|------|
| Production (order → execution → FG) | [Production_Flow.md](Production_Flow.md) |
| Planning | [Planning_Flow.md](Planning_Flow.md) |
| Inventory | [Inventory_Flow.md](Inventory_Flow.md) |
| Maintenance | [Maintenance_Flow.md](Maintenance_Flow.md) |
| Quality | [Quality_Flow.md](Quality_Flow.md) |
| Sales | [Sales_Flow.md](Sales_Flow.md) |
| Purchasing | [Purchasing_Flow.md](Purchasing_Flow.md) |

Legacy narrative catalog: `04_Application/UI_Flows.md` (keep in sync when expanding).

---

## Authoring rules

1. Name actors and preconditions
2. List screens by ID (PRD-xxx / MNT-xxx / …)
3. Name workflow verbs at each step
4. Never use `TASK-0XX` as a flow step

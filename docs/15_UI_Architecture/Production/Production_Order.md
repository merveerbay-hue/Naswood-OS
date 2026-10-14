# Production Order — Screen Architecture

**Module:** Production  
**Workspace:** Execution  
**Capability:** Production Order  
**Status:** Target IA  
**Implementation entry TASK:** TASK-056  
**Related:** Work Order (TASK-057), Confirmations (TASK-059)

---

## Purpose

Plan and control a manufacturing run: what to make, how much, when, on which resources — and track execution through related work orders and confirmations.

---

## User jobs

1. Find orders by status / product / date / line  
2. Understand order progress (ops, qty, scrap)  
3. Create / release / hold / close orders  
4. Drill into work orders and confirmations  
5. Dispatch work (board)  
6. Execute from operator/machine terminals  

---

## Screen family

```text
Production Order
├── Production Order List
├── Production Order Detail
│     ├── Header
│     ├── Operations / routing snapshot
│     ├── Component demand
│     ├── Related Work Orders
│     ├── Confirmations
│     ├── Scrap / Rework
│     ├── Documents
│     └── Timeline / History
├── Create Production Order
├── Release / Schedule actions
├── Dispatch Board          (workspace-level, shared)
├── Operator Terminal       (workspace-level)
└── Machine Terminal        (workspace-level)
```

---

## MVP thinning

| Phase | Scope |
|-------|--------|
| MVP-1 | List, Detail (header + status), Create |
| MVP-2 | Related WO list, confirmation summary |
| MVP-3 | Dispatch Board |
| MVP-4 | Operator / Machine terminals |

---

## TASK mapping

| Slice | TASK |
|-------|------|
| Order API + List/Detail/Create | TASK-056 |
| Work Order family | TASK-057 |
| Consumption / Confirmation | TASK-058, TASK-059 |
| Boards / terminals | future TASKs (do not overload 056) |

---

## Anti-pattern

```text
TASK-056 → single ResourcePage CRUD
```

Replace with List → Detail → Actions as soon as re-baseline starts.

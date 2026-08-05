# Screen → TASK Mapping Rules

**Status:** Active

---

## Rule

Implementation TASKs **reference** screens defined in UI Architecture / Screen Catalog / Design.

They do **not** invent navigation or replace IA.

```text
UI Architecture (screen family)
        ↓
Design specs (13_Design / domain design TASK docs)
        ↓
Implementation TASK (14_Implementation)  ← delivery slice
        ↓
Code
```

---

## Mapping card template

Every Implementation TASK that touches UI must declare:

```markdown
## UI Architecture Mapping

- Module:
- Workspace:
- Capability:
- Screens in scope: (e.g. BOM List, BOM Detail — Create)
- Screens deferred: (e.g. Compare, Import, Export)
- Primary user jobs:
- Related UI Architecture doc:
```

---

## Examples

### TASK-046 — BOM

| Field | Value |
|-------|--------|
| Module | Production |
| Workspace | Master Data |
| Capability | BOM |
| Screens in scope (MVP OK) | BOM List, BOM Detail (header), Create BOM |
| Deferred | Revision compare, Import, Export, multi-level explosion UI |
| IA doc | `15_UI_Architecture/Production/BOM.md` |

### TASK-056 — Production Order

| Field | Value |
|-------|--------|
| Module | Production |
| Workspace | Execution |
| Capability | Production Order |
| Screens in scope (MVP OK) | Order List, Order Detail, Create Order |
| Deferred | Dispatch Board, Operator Terminal, scheduling Gantt |
| IA doc | `15_UI_Architecture/Production/Production_Order.md` |

### TASK-070 — Non-Conformance

| Field | Value |
|-------|--------|
| Module | Quality |
| Workspace | Operations |
| Capability | Non-Conformance |
| Screens in scope | NCR List, NCR Detail, workflow actions |
| Deferred | Full CAPA studio, advanced RCA tools |
| IA doc | `15_UI_Architecture/Quality/README.md` |

---

## Re-baselining existing MVP code

Sprint 01–05 delivered many **generic ResourcePage** UIs. That code is a **technical MVP**, not the product IA.

Re-baseline plan:

1. Keep APIs where domain-correct.  
2. Replace flat nav with workspace nav per `02_Navigation_Map.md`.  
3. Promote each capability from ResourcePage → List + Detail (+ actions) per module IA.  
4. Open Implementation TASKs only against named screens.

---

## Definition of Done (UI TASK)

A UI-facing TASK is not done when “CRUD works”.

It is done when:

- mapped screens exist or are explicitly deferred  
- navigation lands in the correct workspace  
- primary user job can be completed without inventing screens at runtime  
- Screen Catalog / UI Architecture links are updated if new screens were added

# Governing Principles — UI Information Architecture

**Status:** Approved for steering  
**Applies to:** All agents, designers, and implementers of Naswood OS UI

---

## 1. TASK is not the product

An Implementation TASK (`TASK-046`, `TASK-070`, …) is a **development work package**.

It is **not**:

- a module
- a workspace
- a screen architecture
- a user journey

It **is**:

- a bounded unit of delivery that implements part of an already defined screen family, API, or workflow.

---

## 2. Forbidden shortcut

```text
TASK-XXX → Library + Create + Edit + Delete
```

This is allowed only as a **temporary technical spike**, never as the product definition of a domain capability.

ERP / MES / WMS / QMS / CMMS capabilities require screen **families** and **workspaces**, not single generic Resource pages.

---

## 3. Required thinking order

For every capability (example: BOM, Non-Conformance, Asset):

1. **Module purpose** — Why does this module exist in the factory?
2. **User jobs** — What work do planners / operators / supervisors complete?
3. **Workspaces** — Which operational areas group those jobs?
4. **Screen families** — List, Detail, Create, Revision, Compare, Import, History, …
5. **Components & panes** — What appears on each screen?
6. **Workflows** — Draft → Approve → Release → …  
7. **Implementation TASKs** — Which work packages deliver which slices?

If step 7 is attempted before steps 1–6, stop and return to UI Architecture.

---

## 4. Correct hierarchy example — Production / BOM

```text
Production (Module)
  └── Master Data (Workspace)
        └── BOM (Capability)
              ├── BOM List
              ├── BOM Detail
              ├── Create BOM
              ├── Revision
              ├── Compare
              ├── Import
              └── Export
                    └── TASK-046 (implements slices of the above)
```

Not:

```text
TASK-046 → BOM CRUD page
```

---

## 5. Correct hierarchy example — Quality / NCR

```text
Quality (Module)
  └── Operations (Workspace)
        └── Non-Conformance (Capability)
              ├── NCR List
              ├── NCR Detail
              ├── Root Cause
              ├── CAPA
              ├── History
              ├── Attachments
              └── Workflow
                    └── TASK-070 (backend/UI slices of the above)
```

---

## 6. Correct hierarchy example — Maintenance / Asset

```text
Maintenance (Module)
  └── Assets (Workspace)
        └── Asset (Capability)
              ├── Asset Explorer
              ├── Hierarchy
              ├── Asset Detail
              ├── Maintenance History
              ├── Warranty
              ├── Spare Parts
              ├── Costs
              ├── Downtime
              ├── Documents
              ├── Sensors
              └── KPIs
                    └── TASK-076 (implements slices of the above)
```

---

## 7. MVP rule (honest thinning)

MVP may ship a **subset** of a screen family (e.g. BOM List + Detail + Create first).

MVP must **still**:

- name the full family in UI Architecture
- mark deferred screens explicitly
- avoid presenting generic CRUD as the finished product shape

---

## 8. Mapping to Constitution levels

| Constitution level | UI Architecture role |
|--------------------|----------------------|
| Level 5 Design — “What should be built?” | Screen families, navigation, UX jobs |
| Level 6 Implementation — TASK-* | Executable slices of those designs |

See `AI/NOS_CONSTITUTION/01_FOUNDATION.md` Architecture Hierarchy and Level 5–6.

---

## 9. AI Execution Constitution

Binding AI protocol (authority ladder + mandatory read order + module
reconstruction before any TASK):

**`AI/NOS_CONSTITUTION/00_AI_EXECUTION.md`**

Never generate a screen directly from a TASK. Reconstruct the module first.

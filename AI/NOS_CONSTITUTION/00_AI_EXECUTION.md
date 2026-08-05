# ==============================================================================
# NASWOOD OPERATING SYSTEM (NOS)
# AI EXECUTION CONSTITUTION
# ==============================================================================
#
# Document      : AI Execution Constitution
# Part          : 00 — Execution Authority
# Version       : 1.0.0
# Status        : Official
# Owner         : Naswood Technology
#
# This document is the binding execution protocol for every AI assistant
# working on NOS. It does not redefine product vision. It defines how AI
# must read, rank, and apply existing authority documents before writing
# any code, screen, API, or navigation change.
#
# If this document conflicts with a TASK, the TASK yields.
# If this document conflicts with Foundation / Engineering / Platform,
# the higher Constitution part governs product truth; this document
# still governs AI execution procedure.
#
# ==============================================================================

# 1. Purpose

Architecture and Design documents already exist in this repository.

What has been missing is not more product documentation.

What has been missing is an **execution constitution**: a protocol that
teaches AI which document has which authority, and forbids TASK documents
from becoming the de-facto product definition.

Without this protocol, even excellent Architecture and Design documents
remain in the shadow of TASK files.

---

# 2. Absolute Rules

## Rule A — TASK is only a work package

TASK documents (`TASK-046`, `TASK-070`, `TASK-078`, …) are **implementation
work packages**.

A TASK may define:

- Delivery objective
- Scope boundary for one increment
- Dependencies
- Acceptance criteria
- References to higher documents

A TASK may **never** define:

- Product architecture
- Business architecture
- Module architecture
- UI architecture
- Navigation
- Workflow
- User roles
- Workspace hierarchy
- Page hierarchy
- Dashboard model
- Mobile model
- Screen family shape (List / Detail / Terminal / …)

If a TASK appears to define any of the above, treat that content as a
**draft hint at best**, not as authority. Reconstruct from higher documents.

## Rule B — Never generate a screen directly from a TASK

```text
FORBIDDEN:
  TASK-078 → Asset CRUD screen
  TASK-046 → BOM ResourcePage
  TASK-070 → NCR create/edit page
```

```text
REQUIRED:
  Reconstruct Maintenance (or Production / Quality / …) module
    → Infer navigation, roles, workflows, dashboards, workspace & page hierarchy
    → Locate the screen family / PRD that the TASK slices
    → Implement only that TASK slice inside the reconstructed module
```

## Rule C — Reconstruct the complete module first

Before implementing any business TASK, the AI must reconstruct the target
module in working memory (and, when docs are incomplete, stop and report gaps).

Reconstruction must include:

1. Module purpose in the factory
2. User roles and jobs
3. Workspace hierarchy
4. Page / screen hierarchy (families, not one page per entity)
5. Workflows and state machines
6. Dashboards and operational views
7. Mobile surfaces where applicable
8. API / event boundaries
9. Relationships to sibling modules

Only after reconstruction may the AI implement the requested TASK slice.

### Exemplar — Maintenance

Do **not** treat `TASK-078` as “an Asset CRUD screen”.

First reconstruct Maintenance as a CMMS module:

- Asset
- Work Request
- Work Order
- Preventive
- Corrective
- Downtime
- Spare Parts
- Dashboard
- Reports

Infer how those objects relate, which roles act on them, which workspaces
group them, and which screens exist. Then implement only the TASK slice
(for example: Asset List columns + Asset Detail header pane) against that
model.

### Exemplar — Production

Do **not** treat `TASK-056` as “a Production Order CRUD page”.

First reconstruct Production Planning + Execution (orders, WOs, scheduling,
dispatch, operator/machine panels, consumption, confirmation, WIP, scrap,
genealogy, dashboards). Then implement the TASK against the named screen
PRDs under `docs/15_UI/Production/`.

---

# 3. Document Authority Ladder

Higher always wins. Lower may only refine, never contradict.

```text
L0  AI Execution Constitution          ← this file (procedure)
L1  NOS Constitution
      01_FOUNDATION.md
      02_ENGINEERING.md
      03_PLATFORM.md
L2  Architecture                       (system + module architecture)
L3  Engineering Rules / Standards
L4  Platform Rules
L5  Business Domain                    (docs/01_Business, docs/05_Modules, …)
L6  Module Design pack (per module)
      Architecture
      Workflow
      API
      Dashboard
      Mobile
L7  UI Architecture                    (docs/15_UI_Architecture)
L8  Screen Architecture                (docs/15_UI — PRD / QLT / MNT / INV …)
L9  Design System / User Flows         (docs/16_*, docs/17_*, docs/13_Design UX)
L10 Implementation TASK                (docs/14_Implementation — lowest planning unit)
L11 Source Code
L12 Tests / Deployment
```

### Conflict examples

| Conflict | Winner |
|----------|--------|
| TASK says CRUD; Screen Architecture says List + Detail + Terminal | Screen Architecture |
| TASK invents a nav item; UI Architecture omits it | UI Architecture |
| Code already has ResourcePage; Architecture requires workspaces | Architecture (code is debt) |
| Chat / prior agent habit says “one page per TASK” | This Execution Constitution |

---

# 4. Mandatory Read Order (before any TASK)

Before writing code for a TASK, read and comply in this order:

```text
1. AI Execution Constitution          (this document)
2. Constitution — Foundation
3. Constitution — Engineering Rules
4. Constitution — Platform Rules
5. Relevant ADRs / system architecture
6. Module Architecture
7. Module Workflow
8. Module API
9. Module Dashboard
10. Module Mobile
11. Module UI Architecture / Navigation Map
12. Relevant Screen Architecture (PRD / QLT / MNT / …)
13. THEN the TASK document
```

### Path map (Production example)

| Step | Typical path |
|------|----------------|
| Constitution | `AI/NOS_CONSTITUTION/01_FOUNDATION.md` … `03_PLATFORM.md` |
| Module Architecture | `docs/13_Design/05_Production/Production_Architecture.md` |
| Module Workflow | `docs/13_Design/05_Production/Production_Workflow.md` |
| Module API | `docs/13_Design/05_Production/Production_API.md` |
| Module Dashboard | `docs/13_Design/05_Production/Production_Dashboard.md` |
| Module Mobile | `docs/13_Design/05_Production/Production_Mobile.md` |
| UI Architecture | `docs/15_UI_Architecture/` |
| Screen Architecture | `docs/15_UI/Production/Screens/` |
| Domain module notes | `docs/05_Modules/02_Production/` |
| TASK | `docs/13_Design/05_Production/TASK-0XX_*.md` or `docs/14_Implementation/...` |

Other modules follow the same pattern under their Design folder
(Quality, Maintenance, Inventory, Sales, Purchasing, …).

If a required document is missing or empty:

1. **Stop** coding the product surface.
2. Report the gap.
3. Prefer authoring / completing the higher document over inventing UI from the TASK.
4. A temporary technical spike is allowed only when explicitly labeled as debt and must not be presented as the product shape.

---

# 5. Module Reconstruction Protocol

For every TASK targeting a business capability, produce (mentally or in the
PR description) this reconstruction before code:

```text
Module:              <name>
Purpose:             <one paragraph>
Primary roles:       <planner / operator / supervisor / …>
Workspaces:          <list>
Screen families:     <List / Detail / Create / Terminal / Dashboard / …>
Key workflows:       <states and transitions>
Dashboards:          <operational views>
Mobile:              <if applicable>
Sibling links:       <Inventory / Quality / Maintenance / …>
TASK slice:          <exactly what this TASK delivers inside the above>
Out of scope:        <what must NOT be invented as “done”>
```

If the AI cannot fill this block from repository documents, documentation is
incomplete — do not invent a CRUD screen to fill the void.

---

# 6. Implementation Shape Rules

1. Prefer named screens from Screen Architecture over generic `ResourcePage`.
2. Prefer workspace navigation from `docs/15_UI_Architecture/02_Navigation_Map.md`
   over flat “one menu item per entity” lists.
3. Prefer workflow actions (Release, Confirm, Scrap, Approve, …) over bare
   Create / Edit / Delete as the product story.
4. Existing flat CRUD is **technical MVP debt**, not the target architecture.
5. When a TASK’s acceptance criteria conflict with Screen Architecture, update
   the TASK mapping or escalate — do not silently shrink the product to CRUD.

---

# 7. Forbidden Patterns

- `TASK-XXX → one Library/Create/Edit/Delete page` as finished product
- Inventing navigation solely from TASK titles
- Skipping Module Workflow / Dashboard / Mobile because “this TASK is only API”
  when the TASK touches UI or operator experience
- Treating `docs/14_Implementation` as higher than `docs/15_UI*`
- Using prior sprint velocity (many CRUD modules) as precedent for product shape
- Claiming “done” when only entity CRUD exists but screen family is defined

---

# 8. Required Output Discipline

When an AI starts a TASK-bearing change, the first substantive artifact must
show module reconstruction (section 5), not a generated React CRUD page.

Pull requests and commit messages for business UI should name:

- Module
- Workspace
- Screen ID (e.g. `PRD-011`, `MNT-003`)
- TASK ID as the delivery slice

Never title work as “Add Asset CRUD” when the authority is Asset screen family
inside Maintenance.

---

# 9. Relationship to Other Constitution Parts

| Part | Role |
|------|------|
| `00_AI_EXECUTION.md` | How AI must execute (this file) |
| `01_FOUNDATION.md` | Identity, principles, documentation hierarchy |
| `02_ENGINEERING.md` | Engineering philosophy, SDLC, Clean/Hexagonal/DDD |
| `03_PLATFORM.md` | Platform capabilities (workflow, identity, files, …) |

`Cursor_Rules.md` at repository root must point here and must not weaken
these rules.

---

# 10. Final Statement

NOS will not become an enterprise manufacturing OS by completing TASKs as
isolated CRUD screens.

NOS becomes that platform when every TASK is executed **inside** a
reconstructed module — with navigation, roles, workflows, dashboards, and
page hierarchy already inferred from higher authority.

**Never generate a screen directly from a TASK.**  
**Always reconstruct the complete module first.**  
**Only then implement the requested TASK.**

This is the execution constitution.

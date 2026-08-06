# ==============================================================================
# NASWOOD OPERATING SYSTEM (NOS)
# AI EXECUTION CONSTITUTION
# ==============================================================================
#
# Document      : AI Execution Constitution
# Part          : 00 — Execution Authority
# Version       : 1.2.0
# Status        : Official
# Owner         : Naswood Technology
#
# This document is the binding execution protocol for every AI assistant
# working on NOS. It does not redefine product vision. It defines how AI
# must read, rank, and apply existing authority documents before writing
# any code, screen, API, or navigation change.
#
# Product thinking (how we design) lives in 04_PRODUCT_ARCHITECT.md.
# This file governs implementation after that design exists.
#
# If this document conflicts with a TASK, the TASK yields.
# If this document conflicts with Foundation / Engineering / Platform,
# the higher Constitution part governs product truth; this document
# still governs AI execution procedure.
#
# ==============================================================================

# 1. Purpose

Architecture, Module Design (Workflow / API / Dashboard / Mobile), and domain
documents already exist in this repository.

What has been missing is not more Implementation TASKs.

What has been missing is the **product layer** — how AI is forced to *think*
about NOS as a product (roles, workspaces, ERP comparison) before docs or code —
and an execution constitution that implements from that layer instead of TASKs.

**Product thinking protocol:** `04_PRODUCT_ARCHITECT.md`  
**Product map home:** `docs/00_Product/`  
**`docs/14_Implementation` is FROZEN.** Do not add new TASK files.

Delivery shape:

```text
NOS → Module → Workspace → Navigation → Screen → Component
  → Workflow → Permissions → Code
```

See `docs/PRODUCT_LAYERS.md`.

---

# 2. Absolute Rules

## Rule A — Implementation TASK layer is frozen

`docs/14_Implementation` is **frozen**.

- Do **not** create new `TASK-*.md` files.
- Do **not** plan delivery as Architecture → TASK → TASK → TASK.
- Existing TASK files are historical archives only.

Prefer prompts and PR titles like:

```text
Build Maintenance Workspace
Implement Production Orders (PRD-010 / PRD-011)
Author Quality NCR screen family
```

Not:

```text
Implement TASK-078
Do TASK-056
```

Historical TASK documents never define product, UI, workflow, navigation, or
business architecture. If they appear to, treat that as a draft hint at best.

## Rule B — Never generate a screen from a TASK (or TASK habit)

```text
FORBIDDEN:
  TASK-078 → Asset CRUD screen
  TASK-046 → BOM ResourcePage
  “next TASK” → another ResourcePage
```

```text
REQUIRED delivery chain:
  Architecture
    → Module
    → Workspace
    → Navigation
    → Screens (15_UI)
    → Components (18)
    → Workflow / User Flow (17)
    → Permissions (19)
    → Frontend (20 + apps/web)
```

Before designing a new module surface, run the Product Architect ladder in
`04_PRODUCT_ARCHITECT.md` (real life → roles → SAP/IFS/D365/Infor → NOS better
→ document → implement).

## Rule C — Reconstruct the complete module first

Before implementing any business UI or API surface, reconstruct the target
module (and, when docs are incomplete, stop and report gaps).

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
10. Navigation + permissions (`docs/19_Navigation`)

Only after reconstruction may the AI implement the requested **workspace /
screen / flow** slice.

### Exemplar — Maintenance

Prompt: **“Maintenance Workspace’i oluştur”** — not “TASK-078’i yap”.

Reconstruct Maintenance as a CMMS module:

- Asset, Work Request, Work Order, Preventive, Corrective, Downtime,
  Spare Parts, Dashboard, Reports

Then implement dashboard + list + detail + filters + actions + panels + flows
from `15_UI/Maintenance` and `17_User_Flows/Maintenance_Flow.md`.

### Exemplar — Production

Prompt: **“Production Planning Workspace”** — not “TASK-056 CRUD”.

Reconstruct Planning + Execution; implement against `15_UI/Production` PRDs
and `17_User_Flows/Production_Flow.md`.

---

# 3. Document Authority Ladder

Higher always wins. Lower may only refine, never contradict.

```text
L0  AI Execution Constitution          ← this file (implementation procedure)
L0b Product Architect Drive            04_PRODUCT_ARCHITECT.md (design thinking)
L1  NOS Constitution
      01_FOUNDATION.md
      02_ENGINEERING.md
      03_PLATFORM.md
L1b NOS Product Map                    docs/00_Product/
L2  Architecture                       (system + module architecture)
L3  Engineering Rules / Standards
L4  Platform Rules
L5  Business Domain                    (docs/01_Business, docs/05_Modules, …)
L6  Module Design pack (per module)
      Architecture · Workflow · API · Dashboard · Mobile
L7  UI Architecture                    (docs/15_UI_Architecture)
L8  Screen Architecture                (docs/15_UI)
L9  Navigation                         (docs/19_Navigation)
L10 User Flows                         (docs/17_User_Flows)
L11 Component Library                  (docs/18_Component_Library)
L12 Design System                      (docs/16_Design_System → 13_Design DS)
L13 Frontend Architecture              (docs/20_Frontend_Architecture)
L14 Source Code
L15 Tests / Deployment

FROZEN (not a delivery driver):
      docs/14_Implementation (historical TASK archives only)
```

### Conflict examples

| Conflict | Winner |
|----------|--------|
| Historical TASK says CRUD; Screen Architecture says List + Detail + Terminal | Screen Architecture |
| Code invents a nav item; Navigation layer omits it | `19_Navigation` / UI Architecture |
| Code already has ResourcePage; Architecture requires workspaces | Architecture (code is debt) |
| Habit says “one page per TASK” | This Execution Constitution |

---

# 4. Mandatory Read Order (before product UI / FE work)

```text
1. Product Architect Drive            (04 — how to think)
2. AI Execution Constitution          (this document — how to implement)
3. NOS Product Map                    (docs/00_Product)
4. Constitution — Foundation / Engineering / Platform
5. Relevant ADRs / system architecture
6. Module Architecture
7. Module Workflow
8. Module API
9. Module Dashboard
10. Module Mobile
11. UI Architecture (workspaces)
12. Navigation + Permissions (docs/19_Navigation)
13. Screen Architecture (PRD / QLT / MNT / …)
14. User Flows (docs/17_User_Flows)
15. Component Library + Design System
16. Frontend Architecture (docs/20_Frontend_Architecture)
17. Historical TASK only if tracing old acceptance — never as product definition
```

### Path map (Production example)

| Step | Typical path |
|------|----------------|
| Constitution | `AI/NOS_CONSTITUTION/01_FOUNDATION.md` … `03_PLATFORM.md` |
| Module Design pack | `docs/13_Design/05_Production/Production_{Architecture,Workflow,API,Dashboard,Mobile}.md` |
| UI Architecture | `docs/15_UI_Architecture/Production/` |
| Navigation | `docs/19_Navigation/` |
| Screens | `docs/15_UI/Production/Screens/` |
| Flows | `docs/17_User_Flows/Production_Flow.md` |
| Components / DS | `docs/18_Component_Library/` · `docs/16_Design_System/` |
| Frontend Arch | `docs/20_Frontend_Architecture/` |

If a required product document is missing:

1. **Stop** coding the product surface.
2. Report the gap.
3. **Author the product layer** (screen PRD, flow, navigation) — do not invent a TASK file.
4. A temporary technical spike is allowed only when explicitly labeled as debt.

---

# 5. Module Reconstruction Protocol

Before code, produce this reconstruction (PR description is fine):

```text
Module:              <name>
Purpose:             <one paragraph>
Primary roles:       <planner / operator / supervisor / …>
Workspaces:          <list>
Navigation entries:  <from 19_Navigation>
Screen families:     <List / Detail / Create / Terminal / Dashboard / …>
Key workflows:       <states and transitions>
User flows:          <17_User_Flows link>
Dashboards:          <operational views>
Components used:     <18_Component_Library>
Mobile:              <if applicable>
Sibling links:       <Inventory / Quality / Maintenance / …>
Delivery slice:      <workspace / screens / flow in this change>
Out of scope:        <what must NOT be invented as “done”>
```

If the AI cannot fill this block from repository documents, documentation is
incomplete — do not invent a CRUD screen to fill the void.

---

# 6. Implementation Shape Rules

1. Prefer named screens from Screen Architecture over generic `ResourcePage`.
2. Prefer workspace navigation from `docs/19_Navigation` + `15_UI_Architecture`.
3. Prefer workflow actions (Release, Confirm, Scrap, Approve, …) over bare CRUD.
4. Existing flat CRUD is **technical MVP debt**, not the target architecture.
5. Compose screens from `18_Component_Library` + Design System tokens.
6. Do not add files under `14_Implementation`.

---

# 7. Forbidden Patterns

- Creating new TASK documents
- `TASK-XXX → one Library/Create/Edit/Delete page` as finished product
- Inventing navigation solely from TASK titles or entity lists
- Skipping Module Workflow / Dashboard / Mobile when building operator UX
- Treating `docs/14_Implementation` as higher than `docs/15_UI*` / `19` / `17`
- Using prior sprint velocity (many CRUD modules) as precedent for product shape
- Claiming “done” when only entity CRUD exists but screen family is defined

---

# 8. Required Output Discipline

The first substantive artifact must show module reconstruction (section 5),
not a generated React CRUD page.

Pull requests and commit messages for business UI should name:

- Module
- Workspace
- Screen ID (e.g. `PRD-011`, `MNT-003`)
- Flow (optional)

Never title work as “Add Asset CRUD” or “TASK-078”.

---

# 9. Relationship to Other Constitution Parts

| Part | Role |
|------|------|
| `04_PRODUCT_ARCHITECT.md` | How humans + AI **design** the product |
| `00_AI_EXECUTION.md` | How AI **implements** (this file) |
| `01_FOUNDATION.md` | Identity, principles, documentation hierarchy |
| `02_ENGINEERING.md` | Engineering philosophy, SDLC, Clean/Hexagonal/DDD |
| `03_PLATFORM.md` | Platform capabilities (workflow, identity, files, …) |

Product layer map: `docs/PRODUCT_LAYERS.md`  
Product map home: `docs/00_Product/`  
`Cursor_Rules.md` must point here and must not weaken these rules.

---

# 10. Final Statement

NOS will not become an enterprise manufacturing OS by completing TASKs as
isolated CRUD screens.

NOS becomes that platform when we **design** with the Product Architect Drive
and **deliver** Module → Workspace → Navigation → Screen → Component →
Workflow → Permissions → Code.

**We design. Cursor applies.**  
**Never generate a screen directly from a TASK.**  
**Do not create new TASKs.**  
**Always reconstruct the complete module first.**  
**Deliver workspaces and screens, not work-package IDs.**

This is the execution constitution.

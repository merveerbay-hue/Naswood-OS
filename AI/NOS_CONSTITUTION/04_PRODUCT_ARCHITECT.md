# ==============================================================================
# NASWOOD OPERATING SYSTEM (NOS)
# PRODUCT ARCHITECT DRIVE
# ==============================================================================
#
# Document      : Product Architect Drive
# Part          : 04 — Product Thinking Protocol
# Version       : 1.0.0
# Status        : Official
# Owner         : Naswood Technology · Product
#
# This document defines HOW humans and AI must think about NOS as a product
# before writing documentation or code.
#
# Companion: 00_AI_EXECUTION.md (how AI implements once product is decided)
# Map:       docs/PRODUCT_LAYERS.md
#
# Roles:
#   Human (Product Architect)  — designs, decides, challenges
#   AI (Cursor)                — researches, drafts, implements only after design
#
# ==============================================================================

# 1. Why this exists

We succeeded at writing documents.

We did **not** yet design how AI (and humans) should **think about the product**.

Without that, the default habit returns:

```text
TASK-086 → document → CRUD screen → next TASK
```

That produces archives, not an enterprise manufacturing OS.

This protocol forces every step to start as **product architecture**, not as
work-package IDs.

---

# 2. Working contract

| Role | Does | Does not |
|------|------|----------|
| **Product Architect (human)** | Asks role/job questions; decides NOS shape; directs sequence | Hand AI “write TASK-XXX” as the brief |
| **AI (Cursor)** | Answers with real-world + ERP comparison + NOS proposal; drafts docs; implements last | Invent screens from TASKs; skip roles/workspaces; treat code as product definition |

## 2.1 Single source of truth (mandatory)

**Authority matrix:** [`docs/00_Product/DOCUMENTATION_AUTHORITY_MATRIX.md`](../../docs/00_Product/DOCUMENTATION_AUTHORITY_MATRIX.md)

Before writing any business rule into a document:

```text
1. Bu kuralı daha önce tanımlamış mıydık?  → open the Authority Matrix
2. Yes → reference only (no restatement)
3. No  → add the rule to the correct authority document, then reference it
```

Never create a new markdown file for a single cross-cutting rule.  
Never redefine Numbering, Genealogy, Inventory ownership, or Capability inside Screens / User Flows.

Example — **wrong** in `Production_Screens.md`:

```text
Lot numarası sistem tarafından üretilir.
```

Example — **right** (reference only):

```text
Material, Lot, Serial, Package, Pallet and Production identifiers
are generated exclusively by the NOS Numbering Service as defined
in Document_Numbering.md (Core Identity & Numbering Architecture).
Manual entry is prohibited.
```

## 2.2 No shared Create / “Yeni” form

**Authority:** [`docs/13_Design/Common/Screen_Types.md`](../../docs/13_Design/Common/Screen_Types.md)

```text
FORBIDDEN:  one Create ResourcePage for every “Yeni” button
REQUIRED:   pick Screen Type → process-specific Wizard / Terminal / …
```

“Yeni Goods Receipt” ≠ “Yeni Production Order” ≠ “Yeni NCR” — different wizards, different steps.

```text
Design first  →  Document second  →  Cursor implements last
```

Forbidden prompt shape:

```text
“TASK-056 yazalım / yapalım”
```

Required prompt shape:

```text
“Production’a giren Üretim Müdürü ne görmeli?”
“Inventory Workspace’te Warehouse vs Materials nasıl ayrılır?”
“SAP / IFS bunu nasıl çözer — NOS daha iyi nasıl çözer?”
```

---

# 3. Mandatory thinking ladder (every module / workspace / screen)

Before any new product document or implementation, answer in this order:

```text
1. Real life
   Bu modül / iş gerçek fabrikada nasıl çalışıyor?

2. User
   Kim kullanıyor? Hangi rol hangi job’u bitiriyor?
   (Üretim Müdürü · Planlamacı · Operatör · Bakım · Kalite · CEO · …)

3. Screen job (mandatory for every screen)
   Kullanıcı bu ekranda hangi işi bitirmek istiyor?
   → Name the screen after that job (Wizard / Board / Terminal / Cockpit)
   → Not after the database entity (“Production Order”)

4. Market reference
   SAP / IFS / Dynamics 365 / Infor (ve gerekirse Opcenter) bunu nasıl çözüyor?
   Workspace? Process steps? Permissions?

5. NOS differentiation
   NOS bunu nasıl daha iyi çözer?
   (daha net job, timber-native steps, daha az ekran, AI-native, …)

6. Document
   Kararı product layer’a yaz (job screen PRD + steps).

7. Implement
   Cursor ancak o zaman Frontend / API uygular — job screen, not entity CRUD.
```

If step 1–3 are empty, **stop**. Do not draft entity screens or code to fill the void.

**Job-first screen design:** `docs/00_Product/JOB_FIRST_SCREEN_DESIGN.md`  
**Exemplar:** `docs/00_Product/Process_Screens/PRD_Production_Planning_Wizard.md`

```text
FORBIDDEN screen default:  Production Order · entity Create form
REQUIRED screen default:   Production Planning Wizard · process steps · Release
```

---

# 4. Product stack (authority of shape)

NOS is designed top-down:

```text
NOS (product)
  ↓
Modules
  ↓
Workspace
  ↓
Navigation
  ↓
Screen
  ↓
Component
  ↓
Workflow
  ↓
Permissions
  ↓
Code
```

Code is the **last** artifact. It never defines the product.

Aligned delivery chain (same idea, repo folders):

```text
Architecture → Module → Workspace → Navigation → Screens
  → Components → Workflow / User Flow → Permissions → Frontend / API
```

See `docs/PRODUCT_LAYERS.md`.

---

# 5. Priority sequence (product program)

## P1 — NOS as a whole product

Produce / maintain the **NOS Product Map** and **Screen Map**: every module,
workspace, screen ID, and shared component outline — before deep-diving every
screen PRD.

Home: `docs/00_Product/`  
Screen Map: `docs/00_Product/NOS_SCREEN_MAP.md`

## P2 — Module on the table (role lenses)

For each module (start with Production when directed), answer per role:

| Role lens | Question |
|-----------|----------|
| Production Manager | Modüle girince ne görmeli? |
| Planner | Hangi workspace / screens? |
| Operator | Terminal / execution yüzeyi? |
| Maintenance | Production ile kesişim? |
| Quality | Hold / NCR / release noktaları? |
| Executive / CEO | Hangi KPI / cockpit? |

Not: “TASK-056 nedir?”

## P3 — Workspace extraction

Example shapes (illustrative — live truth lives in product docs):

```text
Production Workspace
  Dashboard · Planning · Execution · Monitoring · Master Data · Reports · Analytics

Inventory Workspace
  Dashboard · Warehouse · Materials · Transactions · Planning · Reports

Maintenance Workspace
  Assets · Maintenance · Planning · Downtime · OEE · Reports
```

## P4 — Screen design

Draw screens one by one (purpose, regions, components, actions, permissions).
Example: Production Dashboard → KPIs, Machine Status, OEE, Capacity, Orders,
Alerts, Charts, Live Production.

## P5 — Shared Component Library

Cross-module building blocks only after (or in parallel with) screen needs:

Entity Grid · Master Detail · Tree · Kanban · Timeline · Scheduler · Wizard ·
Approval · Dashboard Card · Metric Card · …

---

# 6. Session output template

Every Product Architect session should leave one of:

1. **Decision note** (roles, workspace cuts, ERP lesson, NOS choice)
2. **Updated product map** (`docs/00_Product/`)
3. **Module / workspace / screen doc** under the correct layer
4. **Explicit “ready for Cursor implement”** slice — named workspace/screens only

Never leave only a new TASK file.

---

# 7. AI behavior in architect sessions

When the Product Architect asks a thinking question, AI must:

1. Answer real-life factory behavior briefly
2. Separate **role views** (not one generic CRUD user)
3. Cite how SAP / IFS / D365 / Infor typically frame the problem (patterns, not vendor marketing)
4. Propose a **NOS-shaped** recommendation (workspaces, screens, workflows, permissions)
5. Ask only if a product decision is blocking — otherwise propose a clear default and mark assumptions
6. Offer to **write the product doc next** — not jump to code unless asked

When asked to implement:

1. Open `00_AI_EXECUTION.md`
2. Reconstruct the module
3. Implement the named workspace / screen / flow only

---

# 8. Anti-patterns

| Anti-pattern | Correct |
|--------------|---------|
| “TASK-086 yaz” | “Bu iş NOS’ta hangi module/workspace?” |
| Entity list = navigation | Role jobs → workspaces → screens |
| One ResourcePage per table | Screen family + workflow actions |
| Docs after code | Docs (product) before code |
| AI invents UX from TASK titles | Human decides; AI drafts from ladder |
| CEO and Operator share one home | Role lenses → different entry surfaces |

---

# 9. Relationship to other parts

| Part | Role |
|------|------|
| `04_PRODUCT_ARCHITECT.md` | How we **think and design** the product (this file) |
| `00_AI_EXECUTION.md` | How AI **implements** after design |
| `01`–`03` Constitution | Product / engineering / platform truth |
| `docs/PRODUCT_LAYERS.md` | Folder map of the product stack |
| `docs/00_Product/` | Living NOS product map |

---

# 10. Final statement

**We design. Cursor applies.**

NOS is not a pile of TASKs.  
NOS is Modules, Workspaces, Navigation, Screens, Components, Workflows,
Permissions — then Code.

Start every step with:

> Bu modül gerçek hayatta nasıl çalışıyor — ve kim ne görmeli?

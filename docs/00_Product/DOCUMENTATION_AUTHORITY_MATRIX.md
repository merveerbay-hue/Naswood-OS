# NOS Documentation Authority Matrix

**Status:** Official — Single Source of Truth (SSOT)  
**Version:** 1.0.0  
**Owner:** Product Architect + Architecture  
**Related:** [`04_PRODUCT_ARCHITECT.md`](../../AI/NOS_CONSTITUTION/04_PRODUCT_ARCHITECT.md)

---

## 1. Purpose

NOS must not redefine the same rule in many documents.

```text
One topic  →  one authority document  →  everywhere else only REFERENCES
```

When a rule comes to mind, **do not** open a new file first. Ask:

> **Bu kuralı daha önce tanımlamış mıydık?**

| Answer | Action |
|--------|--------|
| **Yes** | In the new/adjacent doc, **only reference** the authority (short pointer). Do not restate the rule. |
| **No** | Add the rule to the **correct authority document** in this matrix. Then reference it elsewhere. |

This keeps Cursor (and humans) from meeting contradictory truths.

---

## 2. Working contract

| Role | Does |
|------|------|
| Product Architect / human | Decides which row owns a topic; challenges duplicates |
| AI / Cursor | Before writing any rule: search matrix + authority doc; never invent a parallel rule in Screens / Flows / TASKs |

**Forbidden**

```text
Production_Screens.md:  "Lot numarası sistem tarafından üretilir."
Inventory_Workflow.md:   "Lot is auto-generated."
TASK-021:                "System creates lot number."
```

**Required**

```text
Identifiers are generated automatically according to the centralized
Numbering Architecture (docs/13_Design/99_Shared/Document_Numbering.md).

Manual entry is prohibited.
```

(Use that block as a **reference note** only — full rules live in the authority doc § System Generated Identifiers.)

---

## 3. Authority table

| Topic | Authority document | Consumers may only reference (examples) |
|-------|-------------------|----------------------------------------|
| Numbering / all system identifiers (codes, lots, WH, machine, docs) + **name-first UX** | [`docs/13_Design/99_Shared/Document_Numbering.md`](../13_Design/99_Shared/Document_Numbering.md) § System Generated Identifiers · Constitution § 2.3 | All Screens, Wizards, Builders, FE forms — **no editable Code *** |
| **Genealogy (parent–child material history)** | [`docs/05_Modules/02_Production/Material_Genealogy.md`](../05_Modules/02_Production/Material_Genealogy.md) + Production Architecture genealogy sections | Production Screens/Flows, Quality Traceability UI, Inventory |
| **Traceability (forward/backward inquiry UX & QMS view)** | Quality Architecture [`docs/13_Design/06_Quality/Quality_Architecture.md`](../13_Design/06_Quality/Quality_Architecture.md) **and** Inventory Architecture [`docs/13_Design/02_Inventory/Inventory_Architecture.md`](../13_Design/02_Inventory/Inventory_Architecture.md) — *joint; Genealogy owns the graph* | QLT Traceability screens, INV lot/serial inquiry |
| **Capability Profile** | Product / Master Data — [`docs/05_Modules/01_Master_Data/Products.md`](../05_Modules/01_Master_Data/Products.md) *(until dedicated Product Architecture exists)* | BOM, Production Order, Purchasing, Inventory validations |
| **Inventory ownership (stock truth)** | [`docs/13_Design/02_Inventory/Inventory_Architecture.md`](../13_Design/02_Inventory/Inventory_Architecture.md) | Production must not post stock except via Inventory transactions |
| **Production execution (how manufacturing runs)** | [`docs/13_Design/05_Production/Production_Workflow.md`](../13_Design/05_Production/Production_Workflow.md) | Screens, User Flows, API, Mobile — process truth |
| **Production module boundaries / aggregates** | [`docs/13_Design/05_Production/Production_Architecture.md`](../13_Design/05_Production/Production_Architecture.md) | All Production design pack peers |
| **UI Navigation (menu, workspace chrome, permissions map)** | [`docs/19_Navigation/`](../19_Navigation/) *(target; Platform Navigation until landed)* · [`docs/13_Design/00_Platform/Navigation.md`](../13_Design/00_Platform/Navigation.md) | Module Navigation docs, `nav-config` |
| **Screen types (Wizard / Builder / Designer / Configuration / Terminal / …)** | [`docs/13_Design/Common/Screen_Types.md`](../13_Design/Common/Screen_Types.md) | All module Screens, FE CTAs — **no shared Create**; **Master Data ≠ Create Form** |
| **UI patterns (pattern anatomy)** | [`docs/13_Design/Common/UI_Patterns.md`](../13_Design/Common/UI_Patterns.md) | Screen PRDs compose patterns; steps stay in module flows |
| **User experience (job screens & flows)** | Module **Screens** + **User Flows** (e.g. `Production_Screens.md`, `Inventory_Screens.md`) | Frontend — *not* numbering/genealogy; *not* a global Create form |
| **Job-first screen naming** | [`docs/00_Product/JOB_FIRST_SCREEN_DESIGN.md`](./JOB_FIRST_SCREEN_DESIGN.md) | All Screens / Screen Map |
| **NOS module → screen map** | [`docs/00_Product/NOS_SCREEN_MAP.md`](./NOS_SCREEN_MAP.md) | Navigation, FE routes |
| **Product thinking protocol** | [`AI/NOS_CONSTITUTION/04_PRODUCT_ARCHITECT.md`](../../AI/NOS_CONSTITUTION/04_PRODUCT_ARCHITECT.md) | All design sessions |
| **AI implementation procedure** | [`AI/NOS_CONSTITUTION/00_AI_EXECUTION.md`](../../AI/NOS_CONSTITUTION/00_AI_EXECUTION.md) | Code PRs |
| **Permissions / RBAC model** | [`docs/03_system/Permission_Model.md`](../03_system/Permission_Model.md) + [`docs/19_Navigation/Permissions.md`](../19_Navigation/Permissions.md) | Screens (role labels only), API |
| **Events** | [`docs/03_system/Event_Catalog.md`](../03_system/Event_Catalog.md) | Module Workflow/API event lists (reference + emit; don’t redefine payload elsewhere) |
| **Transformations** | [`docs/03_system/Transformation_Model.md`](../03_system/Transformation_Model.md) | Production/Inventory execution docs |
| **Production Order domain model** | [`docs/03_system/Production_Order_Model.md`](../03_system/Production_Order_Model.md) | Production Architecture/Workflow (must not contradict; escalate conflicts) |
| **Database schema** | [`docs/03_system/Database_Schema/`](../03_system/Database_Schema/) | Implementation; design pack must align or raise conflict |
| **Design System / visual** | [`docs/13_Design/00_Platform/Design_System/`](../13_Design/00_Platform/Design_System/) | Screens (layout tokens only) |
| **Component patterns** | [`docs/18_Component_Library/`](../18_Component_Library/) + module Components | Screens compose; don’t redefine grid behavior |
| **Historical TASKs** | **Not an authority** — [`docs/14_Implementation/`](../14_Implementation/) frozen archive | Acceptance archaeology only |

---

## 4. Layer roles (what each doc type is allowed to say)

| Doc type | May define | Must not redefine |
|----------|------------|-------------------|
| **Architecture** | Boundaries, ownership, invariants, integrations | Screen layouts, step-by-step UX copy |
| **Workflow** | Process phases, states, gates, who posts what | Numbering algorithms, stock ledger rules (→ Inventory) |
| **Screens / User Flows** | Jobs, steps, UI regions, role entry | Identifier generation, genealogy graph rules, inventory ownership |
| **API** | Endpoints, DTOs, permissions on calls | Alternate business rules not in Architecture/Workflow |
| **TASK (frozen)** | Historical delivery notes | Any live product truth |
| **ADR / Numbering / Schema** | Cross-cutting platform truth | Module-local exceptions without ADR update |

---

## 5. Conflict resolution

When two documents disagree:

```text
1. Open this matrix → which row owns the topic?
2. Authority document wins.
3. Consumer doc is updated to REFERENCE only (remove duplicate rule text).
4. If authority itself is wrong → Product Architect updates the authority doc
   (one place), then re-point consumers.
5. If two authority rows overlap (e.g. Traceability) → decide joint ownership
   in this matrix; never a third copy in Screens.
```

Constitution / Product Architect Drive outrank informal habit.  
Frozen TASKs never win against Architecture / Workflow / Screen Map.

---

## 6. Checklist before adding any rule

```text
[ ] Searched authority matrix for the topic
[ ] Searched authority document for existing rule
[ ] If exists → wrote a short reference only
[ ] If missing → edited the authority document
[ ] Did not paste the full rule into Screens / Flows / Workflow peers
[ ] Did not create a new markdown file “for this one rule”
```

---

## 7. Known gaps (authority exists but title differs)

| Desired name (Product language) | Current authority path | Action |
|---------------------------------|------------------------|--------|
| Core Identity & Numbering Architecture | `99_Shared/Document_Numbering.md` | Treat as that architecture; rename/split later only via ADR |
| Product Architecture (Capability) | `05_Modules/01_Master_Data/Products.md` | Prefer dedicated Product Architecture when authored; until then Products.md owns Capability |
| `docs/19_Navigation/*` | Present on UI-architecture branch; Platform `Navigation.md` on main | Converge; matrix points at both until merge |

---

## 8. Final statement

**No knowledge lives twice.**

Screens and flows describe **jobs**.  
Architecture and numbering describe **laws**.  
Cursor reads laws from the authority row — never from a repeated sentence in a screen PRD.

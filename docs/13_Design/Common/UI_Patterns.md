# NOS UI Patterns

**Document:** UI Patterns (reusable design patterns)  
**Status:** Official  
**Version:** 1.0.0  
**Companion authority:** [`Screen_Types.md`](./Screen_Types.md) — *which type to use*  
**This file:** *how each type is structured*

---

## 1. Purpose

Define reusable **interaction patterns** for NOS screen types.

Patterns standardize chrome and composition.  
They do **not** standardize a single Create form.

```text
Same pattern family (e.g. Wizard)
  ≠ same steps
  ≠ same fields
  ≠ same Post/Release rules
```

Process steps always come from module Workflow + User Flows + job screen PRDs.

---

## 2. Pattern: Wizard

### Purpose

Complete a transaction **step by step** with validation gates.

### Used for

Process PRDs: [`docs/00_Product/Process_Screens/`](../../00_Product/Process_Screens/)

- Production Planning  
- Goods Receipt (Receiving)  
- Issue / Transfer / Cycle Count  
- Maintenance Work Order  
- NCR  
- Purchase Order · Sales Order  

Never a shared Create ResourcePage — see `Screen_Types.md` § 3b.

### Anatomy

```text
┌ Context header (sticky: plant, ref, key identity)
├ Stepper (1…N) — completed / current / locked
├ Step body (inputs, panels, checks)
├ Gate messages (block / warn)
└ Footer: Back · Save draft (policy) · Next | Finish action (Post / Release / Submit)
```

### Behavior rules

- One primary finish action on the last step (Post / Release / Approve).  
- Hard gates block Next/Finish; soft gates warn.  
- Draft persistence between steps when policy allows.  
- New Lot/Serial/Package/Pallet/Production IDs → Numbering Service only (`Document_Numbering.md`).  
- Technical drawings / cross-sections attach via Platform File Upload — typed roles on the document (e.g. Production Planning Wizard step 4).  
- Finish step may offer **print/export** (shop packet) without a separate “Create print entity” screen.  
- No generic “Create entity” summary that skips process gates.

### Components (typical)

Stepper · Wizard shell · Approval Bar · Availability / Constraint panels · Attribute Panel · Scan Field · Document lines

### Anti-pattern

Single-page form titled “New {Entity}” with Save = done.

---

## 3. Pattern: Explorer

### Purpose

Inspect and navigate **master / stock / asset** data; open related jobs.

### Used for

- Warehouse / Location  
- Lot / Serial  
- Machine / Asset / Product  
- Plan / Order **Library** (secondary to Wizard)

### Anatomy

```text
┌ Filter Bar
├ Entity Grid | Tree | Map
├ Preview / Detail pane (optional split)
└ Actions: Open job… (Receive, Plan, Raise NCR) — not “Create clone of same form”
```

### Behavior rules

- Primary job CTAs launch the correct **Wizard / Terminal / Builder / Designer / Configuration**, not inline Create.  
- Engineering Library “Add…” opens the matching **Builder / Designer / Configuration** — never Code · Name · Save.  
- Master Data ≠ Create Form (`Screen_Types.md` § 1).

### Components

Entity Grid · Tree · Master Detail · Filter Bar · Status Badge · Warehouse Map

---

## 3b. Pattern: Builder

### Purpose

Construct a structured engineering object (tree / structure) with validation and **Release**.

### Used for

- BOM Builder  
- Nested package / kit structures

### Anatomy

```text
┌ Context (Product · Revision)
├ Structure canvas / tree
├ Property inspector
├ Validation panel
└ Actions: Validate · Submit · Release
```

### Behavior rules

- Not a header-only form.  
- Release publishes knowledge for Planning / Execution.

---

## 3c. Pattern: Designer

### Purpose

Design a process graph or layout with relations and optional simulation.

### Used for

- Routing Designer  
- Operation Designer  
- Work Center Layout Designer

### Anatomy

```text
┌ Canvas (nodes / stations / ops)
├ Palette (operations · machines · tools)
├ Inspector (cycle · labor · QC)
├ Simulation / conflict strip (optional)
└ Actions: Validate · Release
```

---

## 3d. Pattern: Configuration

### Purpose

Define a rich multi-facet resource (does not fit one page).

### Used for

- Machine Configuration  
- Line Configuration  
- Product Wizard (catalog)

### Anatomy

```text
┌ Identity header (sticky)
├ Facet navigation (Kimlik · Yerleşim · Teknik · …)
├ Facet body
├ Completeness / gate checklist
└ Actions: Validate · Release
```

### Behavior rules

- Facets required by policy before Release.  
- Documents via Platform File Upload (typed roles).  
- Never reduce to Code · Name · Save.  
- **No editable Code * field** — System Code shows “Automatically generated…” then read-only minted value (`Document_Numbering.md`).  
- Related-object pickers are **name-first** (product/material/warehouse by name).

---

## 4. Pattern: Terminal

### Purpose

Execute one focused operational job — often touch, scan, few fields, large controls.

### Used for

- Operator Terminal (shop floor)  
- Receiving / Shipping scan terminal  
- Barcode / QR stations

### Anatomy

```text
┌ Job context (WO / ASN / Shipment) — large type
├ Scan / primary input
├ Qty / result panel
└ Big actions: Start · Complete · Scrap · Post
```

### Behavior rules

- Minimal navigation; one job at a time.  
- Offline-tolerant when specified in Mobile docs.  
- Never a full master-data form.

### Components

Terminal Chrome · Scan Field · Confirmation Qty Panel · Status Badge · Metric strip

---

## 5. Pattern: Console

### Purpose

Operational **desk** for many short actions and a live queue (supervisor / warehouse lead).

### Used for

- Receiving Console  
- Shipping Console  
- Shop Floor supervisor console

### Anatomy

```text
┌ Queue / worklist
├ Detail / action pane
├ Side tools (print, hold, reassign)
└ Rapid actions (without full Wizard when exception path)
```

### vs Terminal

| Console | Terminal |
|---------|----------|
| Many documents / operators | One operator, one job |
| Desk / desktop | Touch / scan station |

### Components

Entity Grid · Split View · Action bar · Alert List · Scan Field

---

## 6. Pattern: Planner

### Purpose

Visual planning across time / capacity / resources.

### Used for

- Scheduling Board  
- Capacity Load Board  
- Dispatch Board  
- Preventive calendar

### Anatomy

```text
┌ Horizon / resource filters
├ Board (Gantt / Kanban / load chart)
├ Conflict / bottleneck panel
└ Actions: Reschedule · Open Wizard · Dispatch
```

### Behavior rules

- Drag-drop and conflicts are first-class.  
- Creating new demand opens **Wizard**, not Planner-embedded Create form.

### Components

Scheduler · Kanban · Capacity Chart · Risk Badge · Filter Bar

---

## 7. Pattern: Dashboard

### Purpose

Operational **command center** and management visibility — drill into jobs.  
**Not** a substitute for Reports / Analytics.

### Used for

- **Inventory Warehouse Command Center** (queues · exceptions · job CTAs)  
- Module dashboards  
- Executive / plant cockpits  

### Anatomy

```text
Action bar (job CTAs)
→ Live queues / boards
→ Exceptions / alerts
→ Thin status (optional)
→ Shortcuts into Wizard / Workbench / Console / Terminal / Planner
```

### Behavior rules

- Every queue / exception links to a job screen type.  
- Dashboard is never the place to “Create entity.”  
- Inventory: **not** a KPI page — see `Inventory_Dashboard.md`.  
- Charts and valuation trends belong in Reports / Analytics unless they directly unblock today’s work.

### Components

Action Chip / Button · Queue List · Exception List · Metric Strip (secondary) · Compact Grid · Alert List

---

## 8. Pattern: Workbench

### Purpose

Multi-pane session for **operational** warehouse desks or **engineering / quality** investigation — rich context, not a Create form.

### Used for

- **Receiving Workbench** (truck → OCR → verify → count → inspect → Post)  
- Shipping Workbench (future)  
- Quality engineering  
- BOM / routing engineering  
- CAPA effectiveness review  
- Cost analysis desks  

### Anatomy

```text
┌ Stage rail / timeline (or context object header)
├ Main surface: Document Viewer · Gallery · Split compare · Cards
├ Side panel: differences · suggestions · minted IDs (read-only)
└ Sticky action bar: Save draft · Next · Print · Raise NCR · Post / Release
```

### Operational Workbench rules

- Sticky action bar — never Save/Cancel-only form footer.  
- Prefer scan / photo / OCR over typing.  
- Identifiers from Numbering Service only (`Document_Numbering.md`).  
- Full receiving UX: `INV_Receiving_Workbench.md`.

### Components

Split View · Timeline · Document Viewer · Image Gallery · Attachment Panel · Progress Indicator · Sticky Action Bar · Genealogy Tracer (read) · Master Detail

---

## 9. Pattern: Approval Center

### Purpose

Decide pending approvals across document types.

### Used for

- PO / GR adjustment / Production Release / NCR disposition / Finance period steps

### Anatomy

```text
┌ Inbox filters (type, age, plant)
├ List of pending items
├ Diff / summary pane
└ Approve · Reject · Request changes
```

### Behavior rules

- Does not edit the full document — decides.  
- Deep link opens the owning Wizard/Detail read-only + decision.

### Components

Task Inbox · Approval Bar · Diff / Summary · Status Badge

---

## 10. Mapping cheat-sheet (Cursor)

| User says / CTA | Default screen type |
|-----------------|---------------------|
| Plan / Release production | Wizard |
| Receive / Post GR | **Workbench** (`INV_Receiving_Workbench.md`) — Terminal only for scan-count companion |
| Raise NCR | Wizard |
| Open maintenance WO | Wizard |
| Run operation on machine | Terminal |
| See warehouse / lots / assets | Explorer |
| Schedule / capacity | Planner |
| Approve backlog | Approval Center |
| Plant KPIs | Dashboard |
| Investigate quality / engineering | Workbench |
| Run full inbound truck acceptance | Receiving Workbench |

When unsure: ask *“Kullanıcı hangi işi bitiriyor?”* then pick type from [`Screen_Types.md`](./Screen_Types.md).

---

## 11. Related authorities

| Topic | Document |
|-------|----------|
| Which type | `Screen_Types.md` |
| Job naming | `docs/00_Product/JOB_FIRST_SCREEN_DESIGN.md` |
| Components | `docs/18_Component_Library/` · module `*_Components.md` |
| Numbering inside steps | `Document_Numbering.md` |
| Process steps | Module Workflow + User Flows + job PRDs |

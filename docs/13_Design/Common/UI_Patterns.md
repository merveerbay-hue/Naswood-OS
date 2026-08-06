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

- Production Planning (Production Order release path)  
- Goods Receipt (Receiving)  
- Maintenance Work Order  
- NCR  
- Purchase Order (sourcing → order)  
- Cycle Count Session (when multi-step)

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

- Primary job CTAs launch the correct **Wizard / Terminal**, not inline Create.  
- Master-data “Add…” allowed only for true catalog maintenance (still Explorer + Master Detail, not shared Create chrome).

### Components

Entity Grid · Tree · Master Detail · Filter Bar · Status Badge · Warehouse Map

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

Management visibility and drill-down into jobs.

### Used for

- Module dashboards  
- Executive / plant cockpits

### Anatomy

```text
KPI row → Queues / boards → Alerts → Shortcuts into Wizard / Console / Planner
```

### Behavior rules

- Every KPI/queue links to a job screen type.  
- Dashboard is never the place to “Create entity.”

### Components

Metric Card · Dashboard Card · Alert List · Chart · Compact Grid

---

## 8. Pattern: Workbench

### Purpose

Analysis / engineering / quality investigation with rich context.

### Used for

- Quality engineering  
- BOM / routing engineering  
- CAPA effectiveness review  
- Cost analysis desks

### Anatomy

```text
┌ Context object
├ Multi-panel tools (spec, history, genealogy view, attachments)
└ Actions: Edit master (policy) · Raise NCR · Open CAPA · Export
```

### Components

Split View · Timeline · Attachment Panel · Genealogy Tracer (read) · Master Detail

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
| Receive / Post GR | Wizard (or Terminal for scan-only post) |
| Raise NCR | Wizard |
| Open maintenance WO | Wizard |
| Run operation on machine | Terminal |
| See warehouse / lots / assets | Explorer |
| Schedule / capacity | Planner |
| Approve backlog | Approval Center |
| Plant KPIs | Dashboard |
| Investigate quality / engineering | Workbench |

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

# NOS Screen Types

**Document:** Screen Types (UX law)  
**Status:** Official  
**Version:** 1.0.0  
**Location:** `docs/13_Design/Common/Screen_Types.md`  
**Companion:** [`UI_Patterns.md`](./UI_Patterns.md)  
**SSOT matrix:** [`docs/00_Product/DOCUMENTATION_AUTHORITY_MATRIX.md`](../../00_Product/DOCUMENTATION_AUTHORITY_MATRIX.md)

---

## 1. Absolute rule

```text
NOS'ta "New" / "Create" diye tek tip ekran YOKTUR.
```

The label **Yeni** (or any “create” CTA) never means “open the shared entity form.”

**Yeni** means: start the **job** for that capability — and that job uses a **screen type**
(Wizard, Terminal, Console, …) whose flow is defined by the process.

| Wrong | Right |
|-------|--------|
| Shared `Create` ResourcePage for every entity | Screen type chosen per job |
| “Yeni” → same fields + Save | “Yeni” → Receiving Wizard / Planning Wizard / NCR Wizard / … |
| Cursor copies Inventory create into Production | Cursor reads Screen Type → then module process steps |

Reusable **components** (grid, filter, card, timeline) are shared.  
Reusable **CRUD create screens** are forbidden as the default.

---

## 2. Screen type catalog

| Screen type | Purpose | Typical jobs |
|-------------|---------|--------------|
| **Wizard** | Finish a multi-step business transaction with gates | Production Planning, Goods Receipt, Maintenance WO, NCR, Purchase Order |
| **Console** | Run a continuous operational desk (many short actions) | Receiving desk, Shipping desk, Shop-floor supervisor console |
| **Explorer** | Find, inspect, navigate hierarchical / catalog data | Warehouse, Lot, Machine, Asset, Product |
| **Planner** | Balance time / capacity / resources visually | Capacity, Scheduling, Dispatch, PM calendar |
| **Dashboard** | See status, KPIs, exceptions; drill into jobs | Module / executive cockpits |
| **Workbench** | Knowledge / engineering / quality analysis surface | Quality engineering, BOM engineering, CAPA analysis |
| **Approval Center** | Decide pending approvals across documents | PO / NCR / Release / Adjustment approvals |
| **Terminal** | Execute a single focused operational job (often touch / scan) | Operator Terminal, Receiving scan terminal, Shipping terminal |

---

## 3. “Yeni” means different jobs (examples)

### Inventory — not “New GoodsReceipt entity”

**CTA:** Receive goods / Mal kabul başlat  
**Type:** **Wizard** (Receiving Wizard)

```text
PO Seç
  → Bekleyen Satırlar
  → Teslim Miktarı
  → Depo seç             ※ kullanıcı seçer
  → Lokasyon             ※ seçilen depoya bağlı
  → Lot Oluştur          ※ malzeme cinsine göre Numbering Service — Document_Numbering.md
  → Kalite Kararı
  → Etiket
  → Post
```

Full: `docs/00_Product/Process_Screens/INV_Receiving_Wizard.md`

### Production — not “New ProductionOrder entity”

**CTA:** Plan production / Üretim planla  
**Type:** **Wizard** (Production Planning Wizard)

```text
Talep → Ürün → Revizyon → BOM → Routing
  → Ölçüler → Ağaç Türü → Hat → Kapasite → Termin → Release
```

Full: `docs/00_Product/Process_Screens/PRD_Production_Planning_Wizard.md`

### Maintenance — not “New WorkOrder entity”

**CTA:** Open work order / İş emri aç  
**Type:** **Wizard**

```text
Asset → Arıza → Öncelik → Teknisyen → Yedek Parça → Plan → Onay
```

### Quality — not “New NCR entity”

**CTA:** Raise NCR / NCR aç  
**Type:** **Wizard**

```text
Kaynak → Ürün → Lot → Problem → Fotoğraf → Root Cause → CAPA
```

---

## 4. CTA naming (replace Create/New)

| Avoid | Prefer (examples) |
|-------|-------------------|
| Yeni / Create / New | **Plan production** · **Receive goods** · **Raise NCR** · **Open work order** · **Start count** |
| Save (as the only outcome) | **Post** · **Release** · **Submit for approval** · **Complete** |

Entity **libraries** (Explorer) may have “Add master…” only for true master-data jobs — still not a shared Create chrome across modules.

---

## 5. How Cursor must choose a screen type

Before generating any UI for a “new / create” intent:

```text
1. What job is the user finishing?          (JOB_FIRST_SCREEN_DESIGN)
2. Which screen type fits that job?        (this document)
3. What process steps / gates apply?       (module Workflow + User Flows)
4. Which components compose the type?      (UI_Patterns + Component Library)
5. Which laws are reference-only?          (Authority Matrix — Numbering, Inventory, …)
```

If step 2 is skipped → defaulting to Entity Grid + Create form is a **defect**.

---

## 6. Type vs component vs entity

| Layer | Shared across modules? | Customized per process? |
|-------|------------------------|-------------------------|
| Screen **type** (Wizard, Terminal, …) | Pattern yes | Steps, gates, CTAs **always** process-specific |
| **Components** (Grid, Card, Stepper, Scan) | Yes | Configuration / slots |
| **Entity** (PO, GR, NCR) | Data only | Never defines the screen alone |

---

## 7. Authority

This file is the **authority for screen type selection**.  
Module Screens / User Flows **reference** a type and define process steps — they do not invent a parallel “Create” pattern.

Related:

- [`UI_Patterns.md`](./UI_Patterns.md) — pattern anatomy  
- [`JOB_FIRST_SCREEN_DESIGN.md`](../../00_Product/JOB_FIRST_SCREEN_DESIGN.md) — job question  
- Module Workflow / Screens — process steps  
- `Document_Numbering.md` — identity minting inside wizards (reference only)

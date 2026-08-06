# Inventory Screens

**Module:** Inventory  
**Version:** 1.0  
**Status:** Active  
**Job-first:** `docs/00_Product/JOB_FIRST_SCREEN_DESIGN.md`

---

# Authority references (do not redefine)

| Topic | Authority |
|-------|-----------|
| Numbering / system identifiers (Material, WH, **Material Identity**, Lot, Serial, Package, Pallet, GR…) + name-first UX | `docs/13_Design/99_Shared/Document_Numbering.md` § System Generated Identifiers — **reference only** |
| **Material Identity** (genealogy root · vs Lot) | `docs/13_Design/99_Shared/Material_Identity_Architecture.md` |
| Inventory ownership / stock truth | `Inventory_Architecture.md` |
| Genealogy | `docs/05_Modules/02_Production/Material_Genealogy.md` |
| Traceability views | Quality + Inventory Architecture |
| Process truth | `Inventory_Workflow.md` |
| Screen IDs | `docs/00_Product/NOS_SCREEN_MAP.md` § Inventory |
| Screen types | `docs/13_Design/Common/Screen_Types.md` |

Do **not** restate “lot/code is auto-generated” algorithms here — reference Numbering.

---

# Design rules (identifiers & names)

```text
Identifiers → Numbering Service only. No Code * input.
Users work with names (Malzeme, Depo) — codes are display-only after mint.
```

| Screen job | User enters | System shows / assigns |
|------------|-------------|------------------------|
| Malzeme tanımla | Ad · Tip · Grup · Ağaç · Ölçü · Birim · Capability · … | `MAT-…` after save (info badge) |
| Depo ekle / yapılandır | Depo adı · Tip · Fabrika · Sorumlu · … | `WH-…` after save |
| Mal kabul (Workbench) | Truck · docs · OCR confirm · count · inspect · **Depo (name)** · … | **Material Identity** (root, class-aware e.g. LOG) + optional Lot · Package / Pallet / GR / txn auto — never typed |
| Lot | operational party attrs (policy) | Minted / attached — **not** lifelong MI |

❌ Never: Warehouse Code ________ · Lot No ________ · Material Code *  
✅ System Code — Automatically generated after save

---

# Workspaces

Authority: [`Inventory_Workspaces.md`](./Inventory_Workspaces.md) · nav: [`Inventory_Navigation.md`](./Inventory_Navigation.md)

```text
Inventory
├── Dashboard
├── Operations
├── Stock
├── Counts & Adjustments
├── Master Data
├── Reports
├── Analytics
└── Settings
```

---

# Screen index (job-oriented)

| ID | Screen (job name) | Workspace | Job to be done |
|----|-------------------|-----------|----------------|
| INV-001 | Inventory Dashboard | Dashboard | See warehouse health & open queues |
| INV-RCV-001 / INV-015–016 | **Receiving Workbench** + receipt library | Operations | Finish **entire** inbound truck operation; mint **root Material Identity** (class-aware) + optional Lot; Depo → labels → **Post**. Type **Workbench** — **not** Create/CRUD. Spec: `INV_Receiving_Workbench.md` · `Material_Identity_Architecture.md` |
| INV-017/018 | **Issue Goods Wizard** | Operations | **Mal çıkışı / Issue goods** — Wizard; not “Yeni çıkış”. Spec: `Process_Screens/INV_Issue_Wizard.md` |
| INV-019/020 | **Transfer Stock Wizard** | Operations | **Stok transfer / Transfer stock** — Wizard; not “Yeni transfer”. Spec: `Process_Screens/INV_Transfer_Wizard.md` |
| INV-027 | Putaway / Picking *(future)* | Operations | Directed warehouse tasks |
| INV-028 | Package Management | Operations | Finish pack identity for move/ship |
| INV-014 | Stock Balance Inquiry | Stock | Answer “what/where/how much?” |
| INV-010/011 | Lot Library / Lot Trace | Stock | Find lot & open history |
| INV-012/013 | Serial Library / Detail | Stock | Find serial & status |
| INV-030 | Reservation Desk | Stock | Allocate / soft-reserve for demand |
| INV-021/022 | **Cycle Count Session** | Counts | **Sayım başlat / Start count** — not “Yeni sayım”. Spec: `Process_Screens/INV_Cycle_Count_Session.md` |
| INV-023 | Physical Inventory | Counts | Plant-wide count event |
| INV-024 | **Post Adjustment** | Counts | **Düzeltme onayla / Post adjustment** (Approval / Workbench) |
| INV-004/005 | Material Library / Define material | Master Data | **Malzeme tanımla** — business fields only; `MAT-…` auto (Numbering). Not Code* form |
| INV-006/007 | Warehouse Library / Configure warehouse | Master Data | **Depo adı · tip · fabrika…** → `WH-…` auto. Not Warehouse Code input |
| INV-008/009 | Location Library | Master Data | **Lokasyon ekle** — name-first; code auto |
| INV-002/003 | Product bridge | Master Data | Jump to Product (`PDT-*`) catalog |
| INV-025 | Inventory Reports | Reports | Run operational reports |
| INV-026 | Inventory Analytics | Analytics | Trends / accuracy |
| INV-029 | Inventory Settings | Settings | Module parameters |

---

# Design rules

- Screen types: `docs/13_Design/Common/Screen_Types.md` — **no shared “Yeni/Create” form**.  
- CTA **Mal kabul başlat / Receive goods** → **Receiving Workbench** (not “Yeni Goods Receipt”, not a Save/Cancel form).  
- Operational primary entries are **job screens** (Receive Workbench, Issue, Transfer, Count Session), not bare entity CRUD.  
- Libraries (Explorer) find & reopen work.  
- Compose from shared components; flow is process-specific (`UI_Patterns.md`).  
- CRUD-only ResourcePages are technical debt.

---

# Related

`Inventory_Workflow.md` · `Inventory_User_Flows.md` · `Inventory_Architecture.md` · `Inventory_Dashboard.md` · `Inventory_Workspaces.md` · `Inventory_Navigation.md` · `INV_Receiving_Workbench.md`

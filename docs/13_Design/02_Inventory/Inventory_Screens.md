# Inventory Screens

**Module:** Inventory  
**Version:** 1.0  
**Status:** Active  
**Job-first:** `docs/00_Product/JOB_FIRST_SCREEN_DESIGN.md`

---

# Authority references (do not redefine)

| Topic | Authority |
|-------|-----------|
| Numbering (Material, Lot, Serial, Package, Pallet) | `docs/13_Design/99_Shared/Document_Numbering.md` |
| Inventory ownership / stock truth | `Inventory_Architecture.md` |
| Genealogy | `docs/05_Modules/02_Production/Material_Genealogy.md` |
| Traceability views | Quality + Inventory Architecture |
| Process truth | `Inventory_Workflow.md` |
| Screen IDs | `docs/00_Product/NOS_SCREEN_MAP.md` § Inventory |

Do **not** write “lot number is system-generated” here — reference Numbering.

---

# Workspaces

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
| INV-015/016 | **Receive Goods** (list/detail) | Operations | Finish inbound receipt against reference |
| INV-017/018 | **Issue Goods** | Operations | Finish outbound issue |
| INV-019/020 | **Transfer Stock** | Operations | Move stock between WH/locations |
| INV-027 | Putaway / Picking *(future)* | Operations | Directed warehouse tasks |
| INV-028 | Package Management | Operations | Finish pack identity for move/ship |
| INV-014 | Stock Balance Inquiry | Stock | Answer “what/where/how much?” |
| INV-010/011 | Lot Library / Lot Trace | Stock | Find lot & open history |
| INV-012/013 | Serial Library / Detail | Stock | Find serial & status |
| INV-030 | Reservation Desk | Stock | Allocate / soft-reserve for demand |
| INV-021/022 | **Cycle Count Session** | Counts | Finish a count & differences |
| INV-023 | Physical Inventory | Counts | Plant-wide count event |
| INV-024 | **Post Adjustment** | Counts | Finish approved qty correction |
| INV-004/005 | Material Library | Master Data | Maintain material master |
| INV-006/007 | Warehouse Library / Map | Master Data | Maintain WH structure |
| INV-008/009 | Location Library | Master Data | Maintain bins/zones |
| INV-002/003 | Product bridge | Master Data | Jump to Product (`PDT-*`) catalog |
| INV-025 | Inventory Reports | Reports | Run operational reports |
| INV-026 | Inventory Analytics | Analytics | Trends / accuracy |
| INV-029 | Inventory Settings | Settings | Module parameters |

---

# Design rules

- Operational primary entries are **job screens** (Receive, Issue, Transfer, Count Session), not bare entity CRUD.  
- Libraries find & reopen work.  
- Compose from shared components (Entity Grid, Master Detail, Warehouse Map, Scan Field).  
- CRUD-only ResourcePages are technical debt.

---

# Related

`Inventory_Workflow.md` · `Inventory_User_Flows.md` · `Inventory_Architecture.md` · `Inventory_Dashboard.md`

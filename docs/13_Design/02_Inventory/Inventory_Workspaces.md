# Inventory Workspaces

**Module:** Inventory  
**Version:** 1.1  
**Status:** Active  
**Owns:** Inventory workspace tree and job grouping  
**Screen index:** `Inventory_Screens.md`  
**Navigation:** `Inventory_Navigation.md`  
**Design program:** [`Inventory_Design_Program.md`](./Inventory_Design_Program.md) — one process at a time

---

# Authority references

| Topic | Authority |
|-------|-----------|
| Screen IDs / jobs | `Inventory_Screens.md` |
| Design sequence | `Inventory_Design_Program.md` |
| Process truth | `Inventory_Workflow.md` |
| Ownership | `Inventory_Architecture.md` |
| Numbering | `Document_Numbering.md` (reference only) |
| Screen types | `Screen_Types.md` |

---

# Workspace tree

```text
Inventory
├── Dashboard          — Command Center (ops)
├── Operations         — receive → putaway → issue → transfer → ship (Design Program order)
├── Stock              — explorer · balances · lots · reservations · traceability
├── Counts & Adjustments
├── Master Data        — separate track (not in ops sequence)
├── Reports
├── Analytics
└── Settings
```

---

# Workspace → primary jobs

| Workspace | Primary CTAs (job verbs) | Not allowed |
|-----------|--------------------------|-------------|
| Dashboard | **Warehouse Command Center** — queues · exceptions · job CTAs | KPI wall / valuation charts (→ Reports) |
| **Operations** | **Mal kabul başlat** → Receiving · **Putaway** (next) · Issue · Transfer · Ship | “Yeni Goods Receipt” |
| Stock | Warehouse Explorer · Balance · Lot / MI · Reservation · Traceability | Manual Lot / MI create |
| Counts & Adjustments | Start count · Physical inventory · Post adjustment | Bare Create Adjustment |
| Master Data | Malzeme tanımla · Depo yapılandır · Lokasyon ekle | Code * Create forms |
| Reports / Analytics | Run / drill (item 12 in Design Program) | Replacing Command Center |
| Settings | Parameters | — |

---

# Operations — designed so far

| Job | Screen | Type | Spec |
|-----|--------|------|------|
| Run warehouse today | Command Center | Dashboard | `Inventory_Dashboard.md` |
| Receive goods | Receiving Workbench | Workbench | `INV_Receiving_Workbench.md` |
| Putaway | *(next)* | Terminal / Workbench | Design Program |

Purchasing owns PO / supplier; Inventory owns truck-to-stock → putaway chain.

---

# Related

`Inventory_Design_Program.md` · `Inventory_Screens.md` · `Inventory_Navigation.md` · `Inventory_User_Flows.md`

# Inventory Workspaces

**Module:** Inventory  
**Version:** 1.0  
**Status:** Active  
**Owns:** Inventory workspace tree and job grouping  
**Screen index:** `Inventory_Screens.md`  
**Navigation:** `Inventory_Navigation.md`

---

# Authority references

| Topic | Authority |
|-------|-----------|
| Screen IDs / jobs | `Inventory_Screens.md` |
| Process truth | `Inventory_Workflow.md` |
| Ownership | `Inventory_Architecture.md` |
| Numbering | `Document_Numbering.md` (reference only) |
| Screen types | `Screen_Types.md` |

---

# Workspace tree

```text
Inventory
├── Dashboard          — health, queues, exceptions
├── Operations         — receive · issue · transfer · putaway / pick (future)
├── Stock              — balances · lots · serials · reservations
├── Counts & Adjustments
├── Master Data        — material · warehouse · location libraries / define jobs
├── Reports
├── Analytics
└── Settings
```

---

# Workspace → primary jobs

| Workspace | Primary CTAs (job verbs) | Not allowed |
|-----------|--------------------------|-------------|
| Dashboard | **Warehouse Command Center** — queues · exceptions · job CTAs | KPI wall / valuation charts (→ Reports) |
| **Operations** | **Mal kabul başlat** → Receiving Workbench · Issue · Transfer | “Yeni Goods Receipt” |
| Stock | Balance inquiry · Lot / Serial library · Reservation desk | Manual Lot No create |
| Counts & Adjustments | Start count · Post adjustment | Bare Create Adjustment |
| Master Data | Malzeme tanımla · Depo yapılandır · Lokasyon ekle | Code * Create forms |
| Reports / Analytics | Run / drill | — |
| Settings | Parameters | — |

---

# Operations workspace — receiving

Inbound physical acceptance is owned by Inventory Operations:

| Job | Screen | Type | Spec |
|-----|--------|------|------|
| Receive goods / Mal kabul başlat | **Receiving Workbench** | Workbench | `docs/00_Product/Process_Screens/INV_Receiving_Workbench.md` |
| Find past receipts | Receipt Library | Explorer / Library | INV-015 — opens Workbench / detail, not Create |

Purchasing owns PO / supplier; Inventory owns truck-to-stock Workbench.

---

# Related

`Inventory_Screens.md` · `Inventory_Navigation.md` · `Inventory_User_Flows.md` · `Inventory_Dashboard.md`

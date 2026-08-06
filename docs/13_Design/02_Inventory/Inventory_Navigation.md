# Inventory Navigation

**Module:** Inventory  
**Version:** 1.0  
**Status:** Active  
**Owns:** Sidebar / module nav labels and deep-link targets for Inventory  
**Workspaces:** `Inventory_Workspaces.md`  
**Design program:** [`Inventory_Design_Program.md`](./Inventory_Design_Program.md)

---

# Sidebar (job-oriented labels)

Prefer **verbs / outcomes** over entity plurals for operations.  
Ops order follows Design Program: Command Center → Receiving → Putaway → … → Shipping → Traceability → Analytics.

| Topic | Authority |
|-------|-----------|
| Workspaces | `Inventory_Workspaces.md` |
| Job screens | `Inventory_Screens.md` |
| Screen types / no Create | `Screen_Types.md` |
| Receiving UX | `docs/00_Product/Process_Screens/INV_Receiving_Workbench.md` |

---

# Sidebar (job-oriented labels)

Prefer **verbs / outcomes** over entity plurals for operations.

```text
Inventory
├── Komuta Merkezi / Command Center   → INV-001 (not KPI page)
├── Operations
│   ├── Mal kabul başlat          → Receiving Workbench
│   ├── Mal çıkışı                → Issue Wizard
│   ├── Stok transfer             → Transfer Wizard
│   ├── Kabul kayıtları           → Receipt Library (find & reopen)
│   └── Putaway / Picking         → (future INV-027)
├── Stock
│   ├── Stok bakiyesi
│   ├── Lot kütüphanesi
│   ├── Seri kütüphanesi
│   └── Rezervasyon masası
├── Counts & Adjustments
│   ├── Sayım başlat
│   └── Düzeltme onayla
├── Master Data
│   ├── Malzeme kütüphanesi / Malzeme tanımla
│   ├── Depolar
│   └── Lokasyonlar
├── Reports
├── Analytics
└── Settings
```

---

# Deep links

| Path (logical) | Target |
|----------------|--------|
| `/inventory` | **Warehouse Command Center** (INV-001) |
| `/inventory/operations/receive` | **Receiving Workbench** (new session or resume draft) |
| `/inventory/operations/receipts` | Receipt Library |
| `/inventory/operations/issue` | Issue Wizard |
| `/inventory/operations/transfer` | Transfer Wizard |
| `/inventory/stock/balances` | Balance Inquiry |

Query `?receivingId=` resumes a Draft/InProgress Workbench session.

---

# Nav anti-patterns

| Wrong | Right |
|-------|--------|
| “Goods Receipts → + New” | **Mal kabul başlat** → Workbench |
| “Create Warehouse Code” | **Depo yapılandır** / library |
| Purchasing menu “Create GR” | Inventory **Mal kabul başlat** |

---

# Related

`Inventory_Workspaces.md` · `Inventory_Screens.md` · `INV_Receiving_Workbench.md` · `NOS_SCREEN_MAP.md` § Inventory

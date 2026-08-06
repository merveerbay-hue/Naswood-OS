# Inventory Navigation

**Module:** Inventory  
**Version:** 1.1  
**Status:** Active  
**Owns:** Sidebar / module nav labels and deep-link targets for Inventory  
**Workspaces:** `Inventory_Workspaces.md`  
**Screens:** `Inventory_Screens.md`  
**Design program:** [`Inventory_Design_Program.md`](./Inventory_Design_Program.md)

---

# Authority references

| Topic | Authority |
|-------|-----------|
| Workspaces | `Inventory_Workspaces.md` |
| Job screens | `Inventory_Screens.md` |
| Design sequence | `Inventory_Design_Program.md` |
| Screen types / no Create | `Screen_Types.md` |
| Receiving UX | `INV_Receiving_Workbench.md` |
| Command Center | `Inventory_Dashboard.md` |

---

# Sidebar (job-oriented labels)

Prefer **verbs / outcomes**. Ops order follows Design Program.

```text
Inventory
├── Komuta Merkezi                    → INV-001 Command Center ✓
├── Operations
│   ├── Mal kabul başlat              → Receiving Workbench ✓
│   ├── Yerleştir (Putaway)           → INV-027 (next)
│   ├── Kabul kayıtları               → Receipt Library
│   ├── Stok transfer                 → Transfer Wizard
│   ├── Mal çıkışı                    → Issue Wizard
│   └── Sevkiyat                      → Shipping (queued)
├── Stock
│   ├── Depo gezgini                  → Warehouse Explorer (queued)
│   ├── Stok bakiyesi
│   ├── Lot / Material Identity
│   ├── Rezervasyon masası
│   └── İzlenebilirlik                → Traceability (queued)
├── Counts & Adjustments
│   ├── Sayım başlat                  → Cycle Count
│   ├── Fiziksel envanter             → Physical Inventory
│   └── Düzeltme onayla
├── Master Data                       ← outside ops sequence
│   ├── Malzeme / Depo / Lokasyon
├── Reports                           → Analytics & Reports (item 12)
├── Analytics
└── Settings
```

---

# Deep links

| Path (logical) | Target |
|----------------|--------|
| `/inventory` | Warehouse Command Center |
| `/inventory/operations/receive` | Receiving Workbench |
| `/inventory/operations/putaway` | Putaway *(next)* |
| `/inventory/operations/receipts` | Receipt Library |
| `/inventory/operations/issue` | Issue Wizard |
| `/inventory/operations/transfer` | Transfer Wizard |
| `/inventory/stock/balances` | Balance Inquiry |

Query `?receivingId=` resumes Receiving; `?putawayId=` will resume Putaway.

---

# Nav anti-patterns

| Wrong | Right |
|-------|--------|
| “Goods Receipts → + New” | **Mal kabul başlat** → Workbench |
| KPI Dashboard as home | **Komuta Merkezi** |
| “Create Warehouse Code” | **Depo yapılandır** |
| Designing Shipping before Putaway | Follow Design Program order |

---

# Related

`Inventory_Design_Program.md` · `Inventory_Workspaces.md` · `Inventory_Screens.md` · `NOS_SCREEN_MAP.md`

# Inventory — Screen Architecture (~30 screens)

**Target:** WMS/ERP inventory UX  
**Status:** Active — core PRDs specified under `Screens/`

---

## Navigation (target)

```text
Inventory
├── Dashboard
├── Operations
│     ├── Goods Receipt
│     ├── Goods Issue
│     └── Transfer
├── Stock
│     ├── Stock Balance
│     ├── Lots
│     ├── Serials
│     └── Reservations
├── Counts & Adjustments
│     ├── Cycle Count
│     ├── Physical Inventory
│     └── Adjustment
├── Master Data
│     ├── Materials
│     ├── Warehouses
│     ├── Locations
│     └── Products
└── Reports
```

---

## Screen index

| ID | Screen | Workspace | Spec |
|----|--------|-----------|------|
| INV-001 | Inventory Dashboard | Dashboard | [Screens/](Screens/) |
| INV-002–003 | Product List / Detail | Master Data | planned |
| INV-004–005 | Material List / Detail | Master Data | Specified |
| INV-006–007 | Warehouse List / Detail | Master Data | Specified |
| INV-008–009 | Location List / Detail | Master Data | List specified |
| INV-010–011 | Lot List / Detail·Trace | Stock | List specified |
| INV-012–013 | Serial List / Detail | Stock | planned |
| INV-014 | Stock Balance Inquiry | Stock | Specified |
| INV-015–016 | Goods Receipt List / Detail | Operations | Specified |
| INV-017–018 | Goods Issue List / Detail | Operations | List specified |
| INV-019–020 | Transfer List / Detail | Operations | List specified |
| INV-021–022 | Cycle Count List / Detail | Counts | List specified |
| INV-023 | Physical Inventory | Counts | planned |
| INV-024 | Inventory Adjustment | Counts | Specified |
| INV-025–026 | Reports / Analytics | Reports | Reports specified |
| INV-027–030 | Putaway, Package, Settings, Reservation | — | planned / future |

---

## Agent rule

Build from **workspace + INV screen PRD**, not from frozen TASK-016–025 titles.

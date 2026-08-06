# Inventory — Screen Architecture (~30 screens)

**Target:** WMS/ERP inventory UX  
**Status:** Inventory specified

---

## Navigation (target)

```text
Inventory
├── Dashboard
├── Products
├── Materials
├── Warehouses
├── Locations
├── Lots
├── Serials
├── Goods Receipt
├── Goods Issue
├── Transfer
├── Cycle Count
├── Physical Inventory
├── Reports
└── Analytics
```

---

## Screen index

| ID | Screen | Workspace |
|----|--------|-----------|
| INV-001 | Inventory Dashboard | Dashboard |
| INV-002 | Product List | Master Data |
| INV-003 | Product Detail | Master Data |
| INV-004 | Material List | Master Data |
| INV-005 | Material Detail | Master Data |
| INV-006 | Warehouse List | Master Data |
| INV-007 | Warehouse Detail / Map | Master Data |
| INV-008 | Location List | Master Data |
| INV-009 | Location Detail | Master Data |
| INV-010 | Lot List | Stock |
| INV-011 | Lot Detail / Trace | Stock |
| INV-012 | Serial List | Stock |
| INV-013 | Serial Detail | Stock |
| INV-014 | Stock Balance Inquiry | Stock |
| INV-015 | Goods Receipt List | Operations |
| INV-016 | Goods Receipt Detail | Operations |
| INV-017 | Goods Issue List | Operations |
| INV-018 | Goods Issue Detail | Operations |
| INV-019 | Transfer List | Operations |
| INV-020 | Transfer Detail | Operations |
| INV-021 | Cycle Count List | Counts |
| INV-022 | Cycle Count Detail | Counts |
| INV-023 | Physical Inventory | Counts |
| INV-024 | Inventory Adjustment | Counts |
| INV-025 | Inventory Reports | Reports |
| INV-026 | Inventory Analytics | Analytics |
| INV-027 | Putaway / Picking *(future)* | Operations |
| INV-028 | Package Management | Operations |
| INV-029 | Inventory Settings | Settings |
| INV-030 | Reservation / Allocation | Stock |

Entry TASKs: TASK-016–025 (must map to INV screens, not replace them)

# Inventory — UI Information Architecture

**Module:** Inventory / WMS  
**Status:** Active  
**Screen specs:** [`docs/15_UI/Inventory/`](../../15_UI/Inventory/)

---

## Module purpose

Control stock identity, location, movements, counts and adjustments across warehouses.  
Stock changes only through posted documents — never silent quantity edits.

---

## Workspaces

```text
Inventory
├── Dashboard                 INV-001
├── Operations                Goods Receipt · Goods Issue · Transfer
├── Stock                     Balances · Lots · Serials · Reservations
├── Counts & Adjustments      Cycle Count · Physical · Adjustment
├── Master Data               Materials · Warehouses · Locations · Products
└── Reports                   Reports · Analytics
```

---

## Reconstruction (roles & jobs)

| Role | Primary workspaces |
|------|-------------------|
| Warehouse Operator | Operations, Stock |
| Warehouse Manager | Dashboard, Counts, Reports |
| Inventory Controller | Stock, Counts, Master Data |
| Planner / Production | Stock Balance inquiry |
| Quality Inspector | Lots (blocked / trace) |

---

## Screen families (MVP delivered in FE)

| Workspace | Screens in scope |
|-----------|------------------|
| Dashboard | INV-001 |
| Master Data | INV-004/005 Material, INV-006/007 Warehouse, INV-008 Location List |
| Stock | INV-014 Balance, INV-010 Lot List |
| Operations | INV-015/016 GR, INV-017 GI List, INV-019 Transfer List |
| Counts | INV-021 Cycle Count List, INV-024 Adjustment |
| Reports | INV-025 launcher |

Deferred: Products, Serials, Physical Inventory, Putaway/Picking, Package, Settings, full Analytics.

---

## Navigation

Canonical menu: `docs/19_Navigation/Menu.md`  
Routes: `/inventory/:workspace/...` per `docs/20_Frontend_Architecture/Routing.md`

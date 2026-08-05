# Inventory — UI Information Architecture

**Module:** Inventory / WMS  
**Status:** Draft workspace map (expand screen families before new CRUD TASKs)

---

## Module purpose

Control stock identity, location, movements, counts and adjustments across warehouses.

---

## Workspaces

```text
Inventory
├── Overview          (warehouse health, alerts)
├── Operations        (Goods Receipt, Goods Issue, Transfers)
├── Stock             (Balances, Batches, Location inquiry)
├── Counts & Adjustments
├── Master Data       (Materials, Warehouses, Locations)
└── Reports
```

---

## Capability examples (screen families required)

| Capability | Family (minimum) |
|------------|------------------|
| Material | Material List, Detail (UoM, classifications, status), Create |
| Warehouse / Location | Explorer or List+Detail, hierarchy |
| Goods Receipt / Issue | Document List, Document Detail (lines), Post/Cancel |
| Stock Transfer | List, Detail, Execute |
| Inventory Count | List, Detail (count lines), Post variance |
| Batch | List, Detail, trace links |

---

## TASK relationship

TASK-016–025 are **implementation slices**, not the IA.

Re-baseline: stop treating each TASK as one ResourcePage; attach them under the workspaces above.

**Entry TASKs:** TASK-016 Material … TASK-025 Adjustment  
**Screen registry:** `docs/04_Application/Screen_Catalog.md` (Warehouse & Inventory)

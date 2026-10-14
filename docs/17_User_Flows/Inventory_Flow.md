# Inventory Flow

**Actors:** Warehouse operator, Inventory controller, Warehouse manager  
**Module:** Inventory

---

## Happy path — Receipt to available stock

1. **Dashboard** — Review incoming / open receipts (`INV-001`)
2. **Create / open Goods Receipt** — List → Detail (`INV-015` → `INV-016`)
3. **Enter lines** — Material, qty, lot, destination location
4. **Post** — Stock on-hand increases (`INV-014` reflects balance)
5. **Optional Quality hold** — Lot status blocked until QI clears (`INV-010`)

---

## Issue / consumption path

Reservation or production request → Goods Issue List/Detail (`INV-017`/`INV-018`) → Post → Stock down → link to Production consumption when from WO

---

## Transfer / count / adjust

| Job | Screens |
|-----|---------|
| Move stock | INV-019 / INV-020 |
| Cycle count | INV-021 / INV-022 |
| Post variance | INV-024 (permission gated) |
| Inquire | INV-014 |

---

## Trace path

Lot List (`INV-010`) → Lot Detail / Trace (`INV-011`) → movements / genealogy

---

## Not a flow step

Any `TASK-*` id. Build from workspace + INV screens.

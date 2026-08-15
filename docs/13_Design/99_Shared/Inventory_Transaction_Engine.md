# Inventory Transaction Engine

**Document:** Inventory Transaction Engine  
**Status:** Skeleton — Foundation Aşama 1 #6 (Queued)  
**Version:** 0.1.0  
**Location:** `docs/13_Design/99_Shared/Inventory_Transaction_Engine.md`  
**Owns (when completed):** Single engine for all inventory movements · txn types · posting contract · reservation interaction · reverse/correction execution · immutability enforcement · cross-module Post API  
**Does not own:** Workbench UX · Material Definition · Conversion formulas · Compliance philosophy (→ `Compliance_Architecture.md`) · Numbering formats · Stock inquiry UI

---

## Foundation position

```text
Inventory_Foundation_Program.md #6 — the heart of the ERP stock ledger.
All movements pass through ONE engine.
```

Compose with: `Inventory_Architecture.md` · `Inventory_Workflow.md` · `Transactions.md` (shared primitives) · Compliance Architecture.

---

## Absolute laws (skeleton)

```text
1. Every stock change is an Inventory Transaction — no silent balance edits.
2. Posted transactions are immutable.
3. Mistakes → Reverse / Correction only (Compliance Architecture).
4. Receiving · Issue · Transfer · Adjustment · Production · Shipment · Scrap ·
   Cycle Count · Reservation effects · Reverse — same engine.
5. Each txn links Material Definition · MI · Package (as applicable) · demand doc · evidence.
6. Conversion snapshot sealed at Post (Measurement & Conversion Architecture).
```

---

## Transaction families (target)

| Family | Examples |
|--------|----------|
| Inbound | Goods Receipt |
| Outbound | Goods Issue · Shipment |
| Internal | Transfer · Production issue/receipt · Putaway |
| Quality / loss | Scrap · Quality Hold move |
| Count | Cycle Count · Physical Inventory adjustment |
| Control | Reservation / release (balance dimensions) |
| Correction | Reverse · Correction |

---

## Related

`Inventory_Foundation_Program.md` · `Inventory_Architecture.md` · `Compliance_Architecture.md` · `Material_Identity_Architecture.md` · `Measurement_Conversion_Architecture.md` · `Package_Architecture.md` · `Transactions.md`

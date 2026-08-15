# Package Allocation Workspace

**Document:** Package Allocation Workspace (cross-process pattern)  
**Status:** Official — Product Architect  
**Version:** 1.1.0  
**Location:** `docs/13_Design/99_Shared/Package_Allocation_Workspace.md`  
**Owns:** Reusable interactive package allocation UX law for all stock movements · live recalculation metrics · operator capabilities · **Take From Package disposition** (Good / Damaged / Quality Hold / Scrap / Rework) · Damage evidence · Scrap txn law · Package Closing Checklist  
**Does not own:** Process-specific Post artifacts · Evidence Archive permanence (→ `Document_Management_Evidence_and_Export.md`) · Material Identity meaning (→ `Material_Identity_Architecture.md`) · Identifier formats (→ `Document_Numbering.md`) · Stock ledger rules (→ `Inventory_Architecture.md`) · Quantity conversion pcs↔m³↔kg (→ `Measurement_Conversion_Engine.md`) · Quality disposition workflow detail (→ Quality) · Screen step copy (each Workbench / Wizard PRD)

---

## 1. Strategic intent

```text
Operators must see ONE working language for package selection
across every stock movement in NOS.

Learn once → use everywhere.
Reduce training time · raise speed · cut errors.
```

The Package Allocation Workspace is **not** a Goods Issue-only widget.  
It is the **standard operational surface** wherever packages (or equivalent handling units) are selected, partially consumed, verified, moved, counted, or loaded.

Goods Issue is the **first landed consumer** (`INV_Goods_Issue_Workbench.md` v2.0.4+ / **v3.0** Compliance by Design).  
Other processes **compose** this pattern — they do not invent a second grid language.  
Cross-cutting Workbench laws (audit · approvals · immutable txns): `Inventory_Workbench_Design_Standard.md`.

---

## 2. Absolute laws

```text
1. Package Allocation Workspace is NOT a simple table.
2. It is an interactive allocation workspace (warehouse planning class).
3. Same mental model across Receiving · Issue · Transfer · Production consumption · Shipping · Count.
4. Every modification recalculates live metrics immediately.
5. Workspace stays synchronized with warehouse inventory (Available).
6. Partial package: barcode / Package Identity unchanged unless company-policy split.
7. AI recommends · operator Accepts or Overrides · validation never silently off.
8. Process PRDs reference this law — they do not redefine grid capabilities.
```

---

## 3. Consumer processes (mandatory composition)

| Process | How the Workspace is used | Primary PRD / status |
|---------|---------------------------|----------------------|
| **Goods Issue** | Pick / allocate packages to demand (multi-package · partial) | `INV_Goods_Issue_Workbench.md` — **landed** |
| **Receiving** | Verify / match physical packages to documents · count confirmation | `INV_Receiving_Workbench.md` — compose on verify/count stages |
| **Warehouse Transfer** | Select packages to move WH/Location → WH/Location | Transfer Wizard / Workbench — **compose when designed** |
| **Production Material Consumption** | Select packages issued to PO / operation | Production issue / consumption Workbench — **compose** |
| **Shipping** | Loading plan — packages onto vehicle / shipment | Shipping Workbench — **compose** |
| **Inventory Count** | Count comparison — expected vs counted by package | Cycle Count / Physical Inventory — **compose** |

Future stock movements that select packages **shall** use this pattern unless Product Architect explicitly exempts them in this document.

---

## 4. Capabilities (shall support in every consumer)

| Capability | Rule |
|------------|------|
| Multi-package allocation | One transaction ← many packages |
| Partial package consumption | Selected ≤ Available; identity/barcode law |
| Real-time calculations | Immediate on every edit |
| Inline quantity editing | Selected (or Counted / Transfer Qty) cells |
| Barcode-driven row selection | Scan focuses / selects / adds row |
| Drag-and-drop allocation | From Explorer / pool → workspace |
| Keyboard shortcuts | Arrow / Tab / Enter / Del (Excel-like) |
| Excel-like navigation | Cell focus · safe keyboard flow |
| Sorting | Any column |
| Filtering | Package · lot · location · quality · moisture · WH |
| Grouping | By WH · Zone · Lot · Quality (optional) |
| Bulk selection | Multi-select → qty / remove / waive |
| AI recommendations | Seed · reset · highlight deltas |
| Manual override | Explorer / DnD / edit (audited) |
| Live validation | Mix + process rules as operator types |
| Inventory sync | Available qty live from stock truth |

Consumers may **hide** columns that do not apply (e.g. Counted vs Selected) but must not invent a parallel UX language.

---

## 5. Live recalculation (every modification)

Volume / weight / dimensional equivalents **shall** be computed via [`Measurement_Conversion_Engine.md`](./Measurement_Conversion_Engine.md) — never hard-coded thickness×width×length in the Workbench UI.

| Metric | Meaning |
|--------|---------|
| Required Quantity | Demand / plan / expected (process-specific source) |
| Selected Quantity | Σ operator allocation (or Counted / Transfer qty) |
| Remaining Quantity | Required − Selected **or** Available − Selected (row + totals — process defines) |
| Volume | Σ selected × unit volume · remaining volume |
| Weight | Σ selected × unit weight · remaining weight |
| Number of Packages | Count of rows with qty > 0 |

Process PRDs map “Required” to the correct source (SO line · PO component · transfer request · count book · shipment plan).

---

## 6. Column model (standard)

Each row = **one package** (handling unit).

| Column | Typical |
|--------|---------|
| Package Number | Display-only |
| Warehouse · Location | Name-first |
| Lot · Material Identity | Display-only |
| Species · Dimensions · Quality · Moisture | Spec |
| Available Quantity | Live inventory |
| **Working Quantity** | Inline — Selected / Counted / Transfer / Load qty |
| Remaining Quantity | Auto |
| Volume · Weight | Auto |

Process aliases for Working Quantity:

| Process | Working Quantity label |
|---------|------------------------|
| Goods Issue / Production consumption | Selected Quantity |
| Transfer | Transfer Quantity |
| Shipping | Load Quantity |
| Receiving verify | Verified / Counted Quantity |
| Inventory Count | Counted Quantity |

---

## 7. AI + Override (shared)

```text
AI recommends a minimum coherent package set (FIFO/FEFO · reservation ·
quality · lot/moisture/dimension/species consistency · WH optimize ·
customer / process rules as applicable).

Operator: Accept (Kabul Et)  OR  Manuel Paket Seç → Scan / Package Number Search + Smart Scan (Explorer only if explicitly requested).
AI Validation remains ON after Override and after grid edits.
Mix warnings (lots · quality · moisture · dims · customer) → continue only if authorized.
```

---

## 8. Partial package (shared)

```text
Physical package barcode / QR / Package Identity → unchanged by default.
Balances update only via Inventory Transaction(s).
Optional company-policy package split → Numbering mints linked child PKG
with complete parent–child traceability (see Goods Issue law detail).
```

**Wood-yard truth:** Opening a package does **not** mean every removed piece is usable for the demand. Disposition is mandatory (see § 8b).

---

## 8b. Damage & Scrap during picking (Take From Package)

When materials are removed from a package, the operator **shall** classify each removed quantity.

### Categories

| Category | Meaning |
|----------|---------|
| **Good** | Usable for the demand / shipment / consumption |
| **Damaged** | Physical damage — evidence required → Damage Hold / Quality Hold |
| **Quality Hold** | Suspect / needs QC decision (may overlap policy with Damaged) |
| **Scrap** | Never returns to the package — separate Scrap Inventory Transaction |
| **Rework** | Sent to rework path (policy) |

```text
Good + Damaged + Quality Hold + Scrap + Rework  =  Picked quantity
(always). Inventory integrity shall always be maintained.
```

### Take From Package panel (workspace right rail / detail)

```text
Take From Package
Requested     40
Picked        40
────────────────
Good          37
Damaged        2
Scrap          1
Quality Hold   0
Rework         0
────────────────
Remaining In Package   80
```

### Example (canonical)

```text
PKG-00254 · 120 pieces
Operator opens package · picks 40
  → Good 37 (shipment / issue)
  → Damaged 2 (photos + reason → Damage/Quality Hold)
  → Scrap 1 (photos + reason → Scrap txn)
Remaining in package 80
Total still balances: 37+2+1+80 = 120
```

### Damage evidence (mandatory for Damaged / Quality Hold qty > 0)

| Capture | Required |
|---------|----------|
| Photos | Yes |
| Damage type / reason | Yes |
| Notes | Optional |
| Voice / video | Optional |

**Damage reasons (standard set):** Broken · Cracked · Wet · Blue Stain · Warped · Forklift Damage · Transport Damage · Packaging Damage · Missing Material · Impact Damage · Other  

Every damage record is linked to the parent operational transaction (GI / Transfer / …) and to the Package + Material Identity. Permanence → `Document_Management_Evidence_and_Export.md`.

### Scrap handling

```text
Scrap shall NEVER return to the original package.
Scrap creates a separate Inventory Transaction.
```

Scrap preserves: Material Identity · Package Identity · Original Receiving · Operator · Date · Time · Reason · Evidence.

**Scrap reasons (standard set):** Machine Damage · Transport Damage · Broken Strap · Forklift Damage · Operator Damage · Unknown · (policy extensions)

### Quality Hold

Damaged / hold qty may move automatically to **Quality Hold** or **Quarantine** per company rules.  
Quality decides later: Release · Rework · Scrap · Supplier Return (Quality workflow — do not redefine here).

### Inventory outcome (integrity)

| Bucket | Example |
|--------|---------|
| Package remaining (Available) | 80 |
| Good → demand / shipment txn | 37 |
| Damaged → Damage Hold / Quality Hold txn | 2 |
| Scrap → Scrap txn | 1 |
| **Sum** | **120** (unchanged package genealogy total) |

### Genealogy / audit

```text
PKG-00254
  ├─ 37 → Sales / Production / … (Good)
  ├─ 2  → Quality Hold / Damage Hold
  ├─ 1  → Scrap
  └─ 80 → Remaining in package
```

Nothing overwritten. Every picked piece is fully traceable (shipped · scrapped · damaged · reworked · remained).

---

## 8c. Package Closing Checklist

After take-from-package (especially partial), present a short **Package Closing Checklist** before leaving the package:

| Check |
|-------|
| Remaining material restacked properly? |
| Package strap / binding re-applied (if required)? |
| Package label still readable? |
| Update package photo? (optional capture) |

Optional: operator takes a **closing photo** → stored as current physical state of the package (Evidence Archive / package gallery).  

**Why (wood):** Same package stays in the yard for days/weeks; next operator sees last known physical condition → safety + quality.

---

## 9. Design priority inside a Workbench

When a process uses package selection:

```text
1. Package Allocation Workspace   ← primary operational surface
2. Take From Package disposition + live totals
3. Damage / Scrap evidence · Package Closing Checklist
4. AI seed / Override Explorer
5. Evidence / destination / review / Post (process-specific)
```

Do not bury allocation in a secondary “lines” CRUD grid.

---

## 10. Ownership boundaries

| Concern | Authority |
|---------|-----------|
| Workspace capabilities · live metrics · shared UX · Take From Package disposition | **This document** |
| GI demand sources · Post · issue gates · worked GI scenarios | `INV_Goods_Issue_Workbench.md` |
| Receiving Evidence First · MI root mint | `INV_Receiving_Workbench.md` · `Material_Identity_Architecture.md` |
| Stock posting / balances | `Inventory_Architecture.md` · `Inventory_Workflow.md` |
| Quality release / rework / supplier return decisions | Quality Workflow / Architecture |
| Screen type Workbench shell | `UI_Patterns.md` · `Screen_Types.md` |
| Evidence permanence / export | `Document_Management_Evidence_and_Export.md` |

---

## 11. Implementation guidance (Cursor)

1. Build **one** FE workspace component family reused by consumers (props: columns · Required source · Working Quantity label · validation profile · disposition enabled).  
2. Do not fork a separate “Transfer grid” or “Count grid” look-and-feel.  
3. Process Workbenches supply stage rails + Post; Allocation Workspace owns the center + Take From Package rail.  
4. Post may emit **multiple** Inventory Transactions per package line (Good · Hold · Scrap) — sum conserved.  
5. Inventory sync: Available from stock service; never a stale local-only qty after Post of other sessions (refresh policy).  

---

## 12. Final statement

```text
One allocation language for every stock movement.
Picked ≠ all Good — classify Good / Damaged / Hold / Scrap / Rework.
Package identity stays; remaining Available decreases; scrap never returns.
Closing checklist keeps the yard honest for the next operator.
Goods Issue is first. Receiving · Transfer · Production · Shipping · Count follow.
```

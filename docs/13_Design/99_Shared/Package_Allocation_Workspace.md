# Package Allocation Workspace

**Document:** Package Allocation Workspace (cross-process pattern)  
**Status:** Official — Product Architect  
**Version:** 1.0.0  
**Location:** `docs/13_Design/99_Shared/Package_Allocation_Workspace.md`  
**Owns:** Reusable interactive package allocation UX law for all stock movements · live recalculation metrics · operator capabilities (inline edit, barcode, DnD, keyboard, Excel-like, sort/filter/group, bulk, AI seed, manual override, live validation, inventory sync)  
**Does not own:** Process-specific Post artifacts · Evidence Archive permanence (→ `Document_Management_Evidence_and_Export.md`) · Material Identity meaning (→ `Material_Identity_Architecture.md`) · Identifier formats (→ `Document_Numbering.md`) · Stock ledger rules (→ `Inventory_Architecture.md`) · Screen step copy (each Workbench / Wizard PRD)

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

Goods Issue is the **first landed consumer** (`INV_Goods_Issue_Workbench.md` v2.0.4+).  
Other processes **compose** this pattern — they do not invent a second grid language.

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

Operator: Accept (Kabul Et)  OR  Override / Yoksay → Explorer / DnD / edit.
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

---

## 9. Design priority inside a Workbench

When a process uses package selection:

```text
1. Package Allocation Workspace   ← primary operational surface
2. Live totals + validation
3. AI seed / Override Explorer
4. Evidence / destination / review / Post (process-specific)
```

Do not bury allocation in a secondary “lines” CRUD grid.

---

## 10. Ownership boundaries

| Concern | Authority |
|---------|-----------|
| Workspace capabilities · live metrics · shared UX language | **This document** |
| GI demand sources · Post · issue gates | `INV_Goods_Issue_Workbench.md` |
| Receiving Evidence First · MI root mint | `INV_Receiving_Workbench.md` · `Material_Identity_Architecture.md` |
| Stock posting / balances | `Inventory_Architecture.md` · `Inventory_Workflow.md` |
| Screen type Workbench shell | `UI_Patterns.md` · `Screen_Types.md` |
| Evidence permanence / export | `Document_Management_Evidence_and_Export.md` |

---

## 11. Implementation guidance (Cursor)

1. Build **one** FE workspace component family reused by consumers (props: columns · Required source · Working Quantity label · validation profile).  
2. Do not fork a separate “Transfer grid” or “Count grid” look-and-feel.  
3. Process Workbenches supply stage rails + Post; Allocation Workspace owns the center.  
4. Inventory sync: Available from stock service; never a stale local-only qty after Post of other sessions (refresh policy).  

---

## 12. Final statement

```text
One allocation language for every stock movement.
Goods Issue is first. Receiving · Transfer · Production · Shipping · Count follow.
Operators learn once. NOS stays consistent.
```

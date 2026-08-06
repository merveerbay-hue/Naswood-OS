# Measurement & Conversion Engine

**Document:** Measurement & Conversion Engine (centralized multi-UoM service)  
**Status:** Official — Product Architect  
**Version:** 1.0.0  
**Location:** `docs/13_Design/99_Shared/Measurement_Conversion_Engine.md`  
**Owns:** Material-centric multi-unit conversion service used by all NOS modules · enter-once / display-all quantity law · stock vs purchase / production / sales / costing UoM roles · dimensional · density · package · custom formula conversion composition · equivalent quantity display set (pcs · lm · m² · m³ · kg · t) · prohibition of module-local manual conversion math  
**Does not own:** SI unit catalog / precision defaults (→ `Measurement_System.md`) · generic unit-pair factor tables & packaging factor primitives (→ `Unit_Conversion.md`) · Material Definition composition / Designer UX (→ `Material_Definition_Architecture.md`) · Inventory stock ledger posting (→ `Inventory_Architecture.md`) · Costing valuation methods (→ Costing) · Package Allocation Workspace interaction (→ `Package_Allocation_Workspace.md`)

---

## 1. Strategic intent

```text
NOS shall include ONE centralized Measurement & Conversion Engine.

Every module uses the same engine.
Manual calculations are prohibited.
The operator enters the quantity only once.
The system displays all equivalent quantities.
```

A material may be **stored** in one unit of measure while being **planned, purchased, produced, sold, costed, or shipped** in another.

| Context (example) | Typical UoM |
|-------------------|-------------|
| Inventory (stock) | Piece |
| Production | Square Meter (m²) |
| Sales | Linear Meter (m / lm) |
| Costing | Cubic Meter (m³) |
| Shipping / weight | Kilogram / Ton |

The engine calculates **all equivalent quantities** automatically from material definition + dimensions + rules — never from operator spreadsheet math.

---

## 2. Absolute laws

```text
1. One Conversion Engine for the whole platform — no module-local converters.
2. Operator enters quantity once (in the transaction / context UoM).
3. System always computes and may display Pieces · Linear m · m² · m³ · kg · t
   (and other configured equivalents) from material definition.
4. Manual cross-UoM calculations by operators are prohibited.
5. Conversions are deterministic, auditable, and versioned with the material /
   formula revision used at calculation time.
6. Posted quantities store the entered value + UoM AND persist computed
   equivalents (or a reproducible conversion snapshot) for audit.
7. This engine EXTENDS Measurement_System + Unit_Conversion —
   it does not replace SI catalogs or generic factor tables.
```

---

## 3. Consumer modules (mandatory)

| Module | How it uses the engine |
|--------|------------------------|
| **Inventory** | Stock UoM · receiving / issue / transfer / count qty · package remaining pcs/m³/kg |
| **Production** | BOM / order / consumption / yield in production UoM ↔ stock UoM |
| **Purchasing** | PO / GR in purchase UoM ↔ stock UoM |
| **Sales** | Quotation / SO / delivery in sales UoM ↔ stock UoM |
| **Planning** | Demand / MRP / ATP in planning UoM ↔ stock availability |
| **Costing** | Cost drivers often m³ / kg — convert from stock / production qty |
| **Quality** | Sample sizes · moisture/density inputs that affect weight conversion |
| **Shipping** | Load plan weight / volume from issued packages |

Future modules that handle material quantities **shall** call this engine — they must not invent parallel formulas.

---

## 4. Authority composition

| Layer | Authority | Responsibility |
|-------|-----------|----------------|
| Units · precision · SI storage model | `Measurement_System.md` | What a measurable value is; canonical storage; display prefs |
| Unit-pair factors · packaging factors · wood factor primitives | `Unit_Conversion.md` | How mm↔m, pallet↔package, density↔mass primitives work |
| **Material multi-UoM business engine** | **This document** | Which UoM roles a material has; dimensional/package/custom formula path; enter-once UX; module API contract |
| Material dimensions / density / type / UoM roles | `Material_Definition_Architecture.md` (Material Definition) | Source attributes & conversion bindings the engine reads |
| Package contents rules | Packaging + Inventory | Pieces per package, remaining after partial issue |

```text
Screens and Workbenches REFERENCE this engine.
They never hardcode thickness × width × length × pieces in UI code.
```

---

## 5. Unit of measure roles (per material)

Every material (or product) definition SHALL declare UoM roles. Roles may share the same unit.

| Role | Purpose |
|------|---------|
| **Stock UoM** | Inventory balance unit (often Piece for packaged wood) |
| **Purchase UoM** | Ordering / supplier invoices |
| **Production UoM** | BOM / shop floor / yield |
| **Sales UoM** | Customer quotes & orders |
| **Planning UoM** | MRP / ATP (often = Stock or Sales) |
| **Costing UoM** | Cost rolls & valuation drivers (often m³) |
| **Shipping UoM** | Optional weight/volume for logistics |
| **Report UoM** | Default analytics display |

```text
Example (Thermowood decking):
  Stock      = Piece
  Production = m²
  Sales      = Linear meter
  Costing    = m³
```

Changing a role’s UoM is a **master-data revision** (audited) — not a silent screen toggle.

---

## 6. Conversion bases

The engine selects one or more bases (composed, not exclusive):

| Base | Inputs | Typical outputs |
|------|--------|-----------------|
| **Material Dimensions** | Thickness · Width · Length (per piece or per profile) | pcs ↔ lm ↔ m² ↔ m³ |
| **Thickness / Width / Length** | Explicit dimensional fields on material / package / MI | Area / volume |
| **Density** | Material density (+ moisture policy when required) | m³ ↔ kg ↔ t |
| **Material Type** | Softwood / hardwood / panel / log / chemical / spare — selects formula family | Correct formula set |
| **Package Rules** | Pieces per package · net/gross · layer rules | Package ↔ Piece ↔ m³ |
| **Custom Conversion Formula** | Versioned formula on material (approved) | Any configured pair |
| **Fixed factor** | From `Unit_Conversion.md` tables (e.g. package ↔ pallet) | Discrete packaging |

### Dimensional wood (default family)

For rectangular / profile pieces (typical NOS wood products):

```text
Linear meters (lm)  ≈  Pieces × Length_m
Square meters (m²)  ≈  Pieces × Width_m × Length_m
                      (or Pieces × face area per piece rule)
Cubic meters (m³)   ≈  Pieces × Thickness_m × Width_m × Length_m
                      (or Pieces × volume per piece)
Kilograms           ≈  m³ × Density_kg_per_m³  (moisture policy applied)
Tons                ≈  kg / 1000
```

Exact formulas are **material-type templates** + optional material overrides.  
Precision / rounding → `Measurement_System.md` · `Unit_Conversion.md`.

### Package rules

```text
Package Identity remains physical.
Engine updates Remaining Pieces AND Remaining m³ / kg using the same formulas.
Partial issue never requires the operator to recompute volume by hand.
```

Compose: `Package_Allocation_Workspace.md` live metrics call this engine.

---

## 7. Enter once — display all

### Operator rule

```text
The operator enters the quantity only once
(in the UoM required by the current job / document line).

The system automatically displays equivalents:

• Pieces
• Linear Meters
• Square Meters
• Cubic Meters
• Kilograms
• Tons

according to the material definition (show/hide by relevance).
```

### UX surface (all modules)

| Element | Behavior |
|---------|----------|
| Primary qty input | One field · context UoM labeled (name-first) |
| Equivalent strip | Live recalculation on every keystroke / scan confirm |
| Package Allocation | Selected / Remaining in pcs **and** m³ (and weight when configured) |
| Documents / exports | Print context UoM + key equivalents for audit |

```text
Forbidden: second manual field “also enter m³”.
Allowed:  read-only equivalent chips / columns fed by the engine.
```

---

## 8. Calculation contract (service)

### Request (logical)

| Field | Meaning |
|-------|---------|
| Material (or MI / Package ref) | Identity whose definition drives formulas |
| Quantity | Entered value |
| Source UoM | UoM of entered quantity |
| Target UoMs | Set requested (or “all configured equivalents”) |
| Context | Inventory / Production / Sales / … (for role defaults & rounding policy) |
| As-of | Material / formula revision timestamp (for replay) |

### Response (logical)

| Field | Meaning |
|-------|---------|
| Equivalents map | UoM → value (precision applied) |
| Formula id / revision | Which rule produced the result |
| Inputs used | Dimensions · density · package factors snapshot |
| Warnings | Missing density · incomplete dimensions · soft conflicts |

### Failure

Reject unknown UoM · incompatible dimensionlessness · missing required inputs for the formula family · division by zero / invalid density.  
Never silently invent dimensions.

---

## 9. Storage & audit

| Rule | Detail |
|------|--------|
| Stock ledger | Balances in **Stock UoM** (Inventory Architecture) |
| Documents | Line stores entered qty + UoM |
| Equivalents | Persist snapshot on Post (or recompute from sealed snapshot inputs) |
| Traceability | Auditor can see how m³ / kg were derived at Post time |
| Revisions | Material dimension/density/formula changes create master revisions; historical Posts keep their snapshot |

```text
Conversions must never silently rewrite posted history.
Replay uses the sealed conversion snapshot or reverse/correction flows.
```

---

## 10. Workbench & screen composition

| Surface | Obligation |
|---------|------------|
| Goods Issue / Receiving Workbenches | Live pcs · m³ · kg via engine — no hardcoding |
| Package Allocation Workspace | Available / Selected / Remaining volume & weight from engine |
| Sales Order / PO lines | Enter sales/purchase UoM once; show stock equivalent |
| Production Order / BOM | Production UoM ↔ stock UoM via engine |
| Costing sheets | Costing UoM derived, not retyped |
| Shipping | Weight / volume from issued packages via engine |
| Quality | Density/moisture inputs that feed weight conversion are evidence-backed when they change mass |

---

## 11. Configuration (Material Definition)

**Authority for the catalog object:** `Material_Definition_Architecture.md`.

Material Definition shall provide (minimum for dimensional wood):

| Attribute | Use |
|-----------|-----|
| UoM roles (§ 5) | Context defaults |
| Thickness · Width · Length | Dimensional conversion |
| Density (and moisture basis) | Mass conversion |
| Material Type / formula family | Template selection |
| Package rules | Package ↔ piece |
| Custom formula (optional) | Approved override |
| Display equivalent set | Which chips/columns to show |

Identifiers for formula versions follow Numbering / revision laws — operators do not invent formula codes.

---

## 12. AI assistance (optional)

AI may:

- Recommend Stock / Sales / Costing UoM roles for a new material  
- Detect missing dimensions before Post  
- Explain “why this m³” to the operator  
- Flag inconsistent package volume vs scan evidence  

AI **must** call the same engine — it must not invent a second calculator.

---

## 13. Compliance & audit readiness

Consistent multi-UoM conversion supports ISO / customer / FSC CoC quantity reconciliations (stock pcs vs shipped m³ vs invoiced lm).

```text
Same engine → same numbers in Inventory, Sales, Shipping, Costing, Quality.
Suitable for internal and external quantity audits.
```

---

## 14. Cursor implementation notes

1. Implement as a **shared service** (domain package) — not copy-pasted helpers per module.  
2. UI never multiplies thickness × width × length locally.  
3. Package Allocation Workspace and Inventory Workbenches inject the engine for live m³/kg.  
4. Persist conversion snapshot on Post.  
5. Compose `Measurement_System.md` + `Unit_Conversion.md` for factors/precision.  
6. Material Definition owns attributes & UoM role bindings; this engine owns the calculation contract.  
7. Prohibited: Excel-like manual dual entry of pcs and m³ as independent truths.

---

## 15. Worked example

```text
Material: Thermowood Deck 26×140×4000
Stock UoM: Piece
Sales UoM: Linear meter
Costing UoM: m³

Operator issues / sells: 40 Pieces  (entered once)

Engine displays:
  Pieces          40
  Linear meters   40 × 4.000 = 160 m
  Square meters   40 × 0.140 × 4.000 = 22.400 m²
  Cubic meters    40 × 0.026 × 0.140 × 4.000 = 0.5824 m³
  Kilograms       0.5824 × density_kg_m³ = …
  Tons            kg / 1000
```

Package PKG-00254 remaining after partial issue updates pcs **and** m³ through the same engine.

---

## Related

`Measurement_System.md` · `Unit_Conversion.md` · `Material_Definition_Architecture.md` · `Package_Allocation_Workspace.md` · `Inventory_Workbench_Design_Standard.md` · `Inventory_Architecture.md` · `Document_Numbering.md` · Materials / Products modules · Costing · Sales · Purchasing · Production · Quality · Shipping design packs

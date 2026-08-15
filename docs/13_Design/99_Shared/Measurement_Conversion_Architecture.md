# Measurement & Conversion Architecture

**Document:** Measurement & Conversion Architecture  
**Status:** Official — Product Architect  
**Version:** 1.0.0  
**Location:** `docs/13_Design/99_Shared/Measurement_Conversion_Architecture.md`  
**Owns:** Platform Measurement & Conversion **architecture** — Primary / Secondary / unlimited UoM · Formula Engine · automatic & dimension/density-based conversion · precision & rounding policy composition · wood-industry calculation families · cross-module contract for Inventory · Production · Purchasing · Sales · Planning · Costing · Quality · Shipping  
**Does not own:** SI unit catalog primitives (→ `Measurement_System.md`) · low-level unit-pair factor tables (→ `Unit_Conversion.md`) · Material Definition bindings (→ `Material_Definition_Architecture.md`) · runtime service call shape detail already in `Measurement_Conversion_Engine.md` (implementation contract — **composes under this architecture**) · Costing valuation methods · Package Allocation UX

---

## 1. Strategic intent

```text
One Measurement & Conversion Architecture for NOS.
Inventory, Production, Purchasing, Sales, Planning, Costing, Quality, Shipping
all use the same laws.

Operator enters quantity once.
System converts automatically.
Manual cross-UoM math is prohibited.
```

`Measurement_Conversion_Engine.md` = **service / enter-once contract** (consumer implementation).  
**This document** = architectural law (Primary/Secondary units, formula engine, wood families, precision/rounding composition).

---

## 2. Absolute laws

```text
1. Unlimited unit support — any configured UoM may participate if convertible.
2. Every Material Definition declares a Primary (Stock) Unit and Secondary roles.
3. Formula Engine is the only path for material-dimensional conversion.
4. Dimension-based and density-based conversions are first-class.
5. Precision and rounding are policy-driven — never ad-hoc in screens.
6. Wood-industry calculations (pcs ↔ lm ↔ m² ↔ m³ ↔ kg ↔ t) are template families.
7. Posted documents seal conversion snapshots (audit / Compliance Architecture).
```

---

## 3. Unit model

| Concept | Meaning |
|---------|---------|
| **Primary Unit** | Stock / inventory balance unit (often Piece for packaged wood) |
| **Secondary Units** | Purchase · Production · Sales · Planning · Costing · Shipping · Report |
| **Unlimited Unit Support** | Additional alternate UoMs may be registered if a conversion path exists |
| **Display set** | Equivalents shown live: Pieces · Linear m · m² · m³ · kg · Tons (configurable) |

Primary ≠ “only unit.” Primary = ledger unit. Secondaries are role UoMs on the Material Definition.

---

## 4. Formula Engine

| Mode | Description |
|------|-------------|
| **Fixed factor** | From `Unit_Conversion.md` (mm↔m, package↔pallet) |
| **Dimension-based** | Thickness · Width · Length → lm / m² / m³ |
| **Density-based** | m³ × density → kg / t (moisture basis applied) |
| **Package rules** | Pieces per package ↔ package count |
| **Custom formula** | Versioned formula on Material Definition (approved) |
| **Automatic conversion** | On every qty entry / scan confirm / Post |

```text
Screens NEVER hardcode thickness × width × length.
They call the Conversion Engine implementing this architecture.
```

### Dimensional wood (default family)

```text
lm  = pcs × Length_m
m²  = pcs × Width_m × Length_m   (or face-area rule)
m³  = pcs × Thickness_m × Width_m × Length_m
kg  = m³ × Density_kg_m³
t   = kg / 1000
```

Material Type selects the formula family (lumber · panel · log · chemical · spare).

---

## 5. Precision & rounding

| Concern | Authority composition |
|---------|----------------------|
| Default precision by quantity kind | `Measurement_System.md` |
| Rounding strategies | `Unit_Conversion.md` (half-up, banker’s, business rule) |
| Per-material override | Material Definition (Unit Precision · Rounding Rules packs) |
| Post-time seal | Compliance Architecture · Transaction Engine |

```text
Precision and rounding are part of the Definition revision
referenced by posted conversion snapshots.
```

---

## 6. Wood-industry specials

| Calculation | Notes |
|-------------|--------|
| Board foot ↔ m³ | Factor tables + species policy |
| Running meter ↔ piece | Length-driven |
| Log ↔ m³ | Log formula family (not rectangular default) |
| Package remaining | After partial issue — pcs and m³/kg via same engine |
| Moisture-adjusted density | When Quality / Definition requires |

---

## 7. Module consumption

| Module | Uses |
|--------|------|
| Inventory | Stock Primary · GI/GR remaining m³/kg |
| Production | Production UoM ↔ Primary |
| Purchasing | Purchase UoM ↔ Primary |
| Sales | Sales UoM ↔ Primary |
| Planning | Planning UoM · ATP |
| Costing | Costing UoM (often m³) |
| Quality | Density/moisture inputs to mass path |
| Shipping | Weight / volume |

---

## 8. Composition map

| Layer | Document |
|-------|----------|
| **Architecture (this)** | Primary/Secondary · Formula Engine · wood families · precision composition |
| Engine contract | `Measurement_Conversion_Engine.md` |
| SI / measurable value | `Measurement_System.md` |
| Unit-pair factors | `Unit_Conversion.md` |
| Bindings | `Material_Definition_Architecture.md` |
| Audit of sealed conversions | `Compliance_Architecture.md` |

---

## Related

`Measurement_Conversion_Engine.md` · `Measurement_System.md` · `Unit_Conversion.md` · `Material_Definition_Architecture.md` · `Compliance_Architecture.md` · `Inventory_Foundation_Program.md`

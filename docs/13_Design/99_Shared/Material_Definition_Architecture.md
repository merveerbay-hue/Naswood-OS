# Material Definition Architecture

**Document:** Material Definition Architecture  
**Status:** Official — Product Architect  
**Version:** 1.1.0  
**Location:** `docs/13_Design/99_Shared/Material_Definition_Architecture.md`  
**Owns:** Material Definition as the active, composite material knowledge object for NOS · distinction from passive Material Master card · **full attribute / rule-pack catalog** (Identity · Type · Family · Species · Grade · Moisture · Dimensions · Measurement & Conversion · Density · Packaging · Numbering · Traceability · Quality · Barcode · Default Warehouse · Storage · Unit Precision · Rounding · Costing Unit · Default Production Unit) · Material Definition vs Material Identity vs Commercial Product · Designer / Builder screen-type law · single definition consumed by all modules  
**Does not own:** Material Identity minting meaning (→ `Material_Identity_Architecture.md`) · Numbering format algorithms (→ `Document_Numbering.md`) · Conversion architecture / engine (→ `Measurement_Conversion_Architecture.md` · `Measurement_Conversion_Engine.md`) · SI units (→ `Measurement_System.md`) · Unit-pair factors (→ `Unit_Conversion.md`) · Warehouse hierarchy detail (→ `Warehouse_Architecture.md`) · Package domain depth (→ `Package_Architecture.md`) · Stock balances (→ Inventory Transaction Engine / Architecture) · Genealogy graph ops (→ `Material_Genealogy.md`) · Quality disposition workflows (→ Quality) · Costing valuation methods (→ Costing) · Commercial Product catalog (→ `Products.md`) · Legacy field lists (→ `Material.md` / `Materials.md` — consumers) · Compliance spine (→ `Compliance_Architecture.md`)

---

## Changelog

| Version | What landed |
|---------|-------------|
| 1.0.0 | Material Definition Architecture — active rule packs · three layers · Designer UX |
| 1.0.1 | Consumer docs updated: Inventory Architecture · Purchasing · Screen Map · Workspaces/Screens |
| **1.1.0** | Full attribute catalog (Foundation #1) · binds Measurement Conversion Architecture · Warehouse / Package / Compliance · Inventory Foundation Program |

---

## 1. Strategic intent

```text
In NOS, the material card is NOT a passive master-data form.

It is a Material Definition —
the living rule pack every module reads when it touches that material.
```

Traditional ERP **Material Master** = Code · Name · Base UoM · Save.  
NOS **Material Definition** = composed architecture that binds:

| Domain pack | What it defines |
|-------------|-----------------|
| **Identity** | How Material Identities are classified / minted for this type |
| **Measurement** | Dimensional & measurable attributes the engines read |
| **Conversion** | UoM roles · formula family · custom conversion rules |
| **Packaging** | Package / piece / pallet rules |
| **Numbering** | Which Numbering series / class applies (catalog + instance) |
| **Quality** | Specs · holds · sampling · grade rules |
| **Traceability** | Lot / MI / CoC / genealogy obligations |
| **Costing** | Costing UoM · cost drivers · valuation hooks |

```text
One Material Definition → all modules.
Inventory · Production · Purchasing · Sales · Planning ·
Costing · Quality · Shipping read the SAME definition.
This is the long-term scalability foundation of NOS.
```

---

## 2. Naming law

| Term | Use in NOS |
|------|------------|
| **Material Definition** | Preferred architecture name for the catalog knowledge object |
| **Material Master** | Legacy / synonym in older docs (`Material.md`) — **means Material Definition**; do not design passive CRUD cards under that name |
| **Material card** | Informal UI language — must open a **Designer / Builder**, never Code·Name·Save |

```text
Architecture language: Material Definition Architecture.
Screen language: Material Definition Designer (or Builder).
Forbidden product language: “Create Material” CRUD form.
```

Authority for screen types: `Screen_Types.md` — **Master Data ≠ Create Form**.

---

## 3. Three layers (do not collapse)

```text
Material Definition  →  catalog knowledge (type / template / rules)
Material Identity    →  physical instance state (genealogy node)
Commercial Product   →  sellable commercial offering (may link to Definition)
```

| Layer | Example | Owned by |
|-------|---------|----------|
| **Material Definition** | Thermowood Deck 26×140×4000 — rules, UoMs, packaging, quality, costing | **This architecture** |
| **Material Identity** | `FG-TWDECK-20260801-00012` at receiving / after transformation | `Material_Identity_Architecture.md` |
| **Product** | Customer-facing SKU / commercial name / price list | `Products.md` (until Product Architecture) |

```text
MAT-… (or equivalent catalog code) identifies a Material Definition.
It is NOT a physical instance. MI is.
Product may reference one or more Material Definitions for fulfillment.
```

MI Architecture already states: catalog material ≠ physical MI.  
This document owns the **catalog side** as an active Definition.

---

## 4. Absolute laws

```text
1. Material Definition is active knowledge — not a passive attribute dump.
2. Every module consumes the same Definition; no per-module shadow masters.
3. Operators never invent Material / MI / Package / Lot codes (Numbering).
4. Quantities convert only via Measurement & Conversion Engine
   using Definition-bound rules — no manual dual entry.
5. Definition changes are revisioned and audited — never silent overwrite
   of rules that posted history depended on.
6. Released Definitions are what operations may use; Draft is engineering only.
7. Extend existing Material.md / Materials.md field catalogs —
   do not fork a second material list.
```

---

## 5. Composition map (rule packs)

Each Material Definition **composes** the following packs.  
Detail algorithms stay in the cited authority; the Definition **binds** which rules apply.

### 5.1 Identity pack

| Binds | Authority |
|-------|-----------|
| Identity class / category for MI minting | `Material_Identity_Architecture.md` |
| When receiving creates root MI | MI Architecture |
| Lot vs MI policy for this type | MI Architecture |

### 5.2 Measurement pack

| Binds | Authority |
|-------|-----------|
| Thickness · Width · Length · density · moisture basis | Definition attributes + `Measurement_System.md` |
| Material type / formula family | Definition |

### 5.3 Conversion pack

| Binds | Authority |
|-------|-----------|
| Stock / Purchase / Production / Sales / Planning / Costing / Shipping UoM roles | `Measurement_Conversion_Architecture.md` |
| Custom conversion formula (versioned) | Conversion Architecture + Definition revision |
| Enter-once · display pcs / lm / m² / m³ / kg / t | Conversion Engine (implements Architecture) |
| Unit Precision · Rounding Rules | Definition + Measurement System / Unit Conversion |

### 5.4 Packaging pack

| Binds | Authority |
|-------|-----------|
| Default package structure · pieces per package · labeling | Packaging + `Unit_Conversion.md` packaging factors |
| Partial package behavior expectations | `Package_Allocation_Workspace.md` · GI Workbench |

### 5.5 Numbering pack

| Binds | Authority |
|-------|-----------|
| Catalog code series for the Definition | `Document_Numbering.md` |
| MI / Lot / Package series applicable to this class | Numbering + MI Architecture |

### 5.6 Quality pack

| Binds | Authority |
|-------|-----------|
| Grade · moisture limits · inspection plan hooks · hold behavior | Quality Architecture / QMS |
| Certificate / FSC-PEFC claim relevance | Quality / CoC docs |

### 5.7 Traceability pack

| Binds | Authority |
|-------|-----------|
| Genealogy obligation · CoC continuity · required evidence at movement | MI · Genealogy · Inventory / Quality Architecture |
| Supplier → Receiving → Package → Issue → FG → Customer reconstructability | Traceability joint authorities |

### 5.8 Costing pack

| Binds | Authority |
|-------|-----------|
| Costing UoM · cost drivers (often m³ / kg) · valuation class hooks | Costing module + Conversion Engine |

```text
The Definition does not re-implement these engines.
It SELECTS and VERSIONS the rule packs they execute.
```

---

## 5b. Full attribute catalog (Foundation v1.1)

Every Released Material Definition **shall be able to carry** (required vs optional by Material Type):

| Attribute / binding | Pack | Notes |
|---------------------|------|--------|
| **Material Identity** (class / mint rules) | Identity | Links to MI Architecture — not a physical MI |
| **Material Type** | General | LOG · PRS · Lumber · Panel · Chemical · Spare… |
| **Product Family** | General | Commercial / planning family |
| **Tree Species** | Measurement | e.g. Sarıçam · Oak |
| **Grade** | Quality | A / B / … |
| **Moisture** | Quality / Measurement | Spec band + basis for density |
| **Dimensions** | Measurement | Thickness · Width · Length |
| **Measurement & Conversion Engine** | Conversion | Role UoMs + formula family binding |
| **Density** | Measurement | kg/m³ (+ moisture basis) |
| **Packaging Rules** | Packaging | Pieces/package · label · close checklist expectations |
| **Numbering Rules** | Numbering | Which series for MAT / MI / PKG / Lot |
| **Traceability Rules** | Traceability | Genealogy · CoC · evidence required |
| **Quality Rules** | Quality | Inspection · hold · certificate relevance |
| **Barcode Rules** | Packaging / Numbering | Label template · scan expectations · no reuse |
| **Default Warehouse** | Storage | Default WH for putaway / planning |
| **Storage Rules** | Storage | Zone constraints · hazmat · quarantine affinity → Warehouse Architecture |
| **Unit Precision** | Conversion | Per-quantity precision overrides |
| **Rounding Rules** | Conversion | Business rounding for this material |
| **Costing Unit** | Costing | Often m³ |
| **Default Production Unit** | Conversion / Production | Production UoM role |

```text
This catalog is why Material Definition is the FIRST foundation file.
Production must not start until these bindings are Official.
```

---

## 6. Lifecycle

Align with existing Material lifecycle language (`Material.md`); architecture names:

| State | Meaning |
|-------|---------|
| Draft | Engineering may edit rule packs |
| Review | Cross-functional check (Quality · Costing · Warehouse) |
| Approved / Released | Eligible for operational use |
| Active | In use |
| Blocked | Temporarily not usable for new demand |
| Obsolete | No new use; history retained |
| Archived | Frozen knowledge |

```text
Released Definition revision is what Post-time conversion / quality / costing snapshots reference.
Changing dimensions or formulas creates a new Definition revision.
```

---

## 7. Screen type (UX)

```text
Material Definition Designer / Builder — NOT a Create Form.
Release (or Approve) — NOT bare Save as the job.
```

| Pattern | Use |
|---------|-----|
| **Designer** | Compose rule packs · dimensions · UoM roles · quality/costing hooks |
| **Builder** | When structure trees apply (e.g. package structure, alternate definitions) |
| **Library** | Find / filter Definitions → open Designer |
| **Configuration** | Company-wide formula templates / category defaults |

Compose: `Screen_Types.md` · `UI_Patterns.md` · `JOB_FIRST_SCREEN_DESIGN.md`.

CTA examples:

| Locale | Label |
|--------|--------|
| EN | **Define material** / Open Material Definition |
| TR | **Malzeme tanımla** / Malzeme Tanımı |

Never: “Yeni malzeme” as CRUD Create.

---

## 8. Module consumption

| Module | Reads from Definition |
|--------|------------------------|
| **Inventory** | Stock UoM · packaging · traceability · conversion for remaining m³/kg |
| **Production** | Production UoM · BOM compatibility · identity class on transform |
| **Purchasing** | Purchase UoM · quality inbound expectations |
| **Sales** | Sales UoM · commercial link via Product · grade/customer rules |
| **Planning** | Planning UoM · ATP dimensions |
| **Costing** | Costing UoM · drivers |
| **Quality** | Specs · sampling · hold |
| **Shipping** | Weight/volume via conversion · package rules |

```text
No module keeps a private copy of “base unit only” that disagrees
with the Material Definition.
```

---

## 9. Relationship to legacy Material Master docs

| Document | Role after this architecture |
|----------|------------------------------|
| `docs/13_Design/01_Master_Data/Material.md` | Field / category / lifecycle catalog — **consumer**; header points here; “Material Master” = Material Definition |
| `docs/05_Modules/01_Master_Data/Materials.md` | Module PRD — **consumer**; registration screens become Definition Designer |
| `docs/05_Modules/01_Master_Data/Products.md` | Commercial Product — remains separate; links to Definition(s) |
| Capability Profile (matrix) | Stays on Products until Product Architecture; **material rule packs** owned here |

This architecture **extends** those documents. It does not delete their attribute lists.

---

## 10. Worked mental model

```text
Material Definition: Thermowood Deck 26×140×4000
  Identity     → FG-TWDECK class · receiving roots MI
  Measurement  → 26 × 140 × 4000 · density · moisture basis
  Conversion   → Stock=Piece · Sales=lm · Production=m² · Costing=m³
  Packaging    → default package · pieces/package
  Numbering    → MAT / MI / PKG series for class
  Quality      → grade A defaults · moisture band
  Traceability → CoC + genealogy required
  Costing      → cost on m³

Operator never maintains eight separate cards —
one Definition feeds Measurement Engine, GI Workbench,
SO line, PO line, and costing sheet.
```

---

## 11. Cursor implementation notes

1. Name domain entity **MaterialDefinition** (API/docs); map legacy `Material` / Material Master routes to Definition Designer.  
2. Do **not** ship Code · Name · BaseUoM · Save as the material UX.  
3. Definition stores **bindings** to engines; engines execute (Conversion, Numbering, MI).  
4. Revision Definition when conversion/quality/costing inputs change; Post snapshots stay immutable.  
5. Products reference Definitions — do not duplicate dimensional/conversion rules on Product.  
6. FE Library → Material Definition Designer; Inventory/Sales/PO only **read** Released revisions.

---

## Related

`Inventory_Foundation_Program.md` · `Material_Identity_Architecture.md` · `Measurement_Conversion_Architecture.md` · `Measurement_Conversion_Engine.md` · `Compliance_Architecture.md` · `Warehouse_Architecture.md` · `Package_Architecture.md` · `Measurement_System.md` · `Unit_Conversion.md` · `Document_Numbering.md` · `Package_Allocation_Workspace.md` · `Material_Genealogy.md` · `Inventory_Architecture.md` · `Screen_Types.md` · `UI_Patterns.md` · `Material.md` · `Materials.md` · `Products.md`

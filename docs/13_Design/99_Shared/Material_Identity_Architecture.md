# Material Identity Architecture

**Document:** Material Identity Architecture  
**Status:** Official — Product Architect  
**Version:** 1.0.0  
**Location:** `docs/13_Design/99_Shared/Material_Identity_Architecture.md`  
**Owns:** Material Identity (lifecycle root), distinction from Lot/Batch, identity class chain, receiving as genealogy root  
**Does not own:** Numbering *format algorithms* (→ `Document_Numbering.md`) · Genealogy *graph operations / inquiry* (→ `Material_Genealogy.md`) · Stock balances (→ `Inventory_Architecture.md`) · Transformation execution (→ `Transformation_Model.md`)

---

## 1. Why this exists

NOS historically said **“Lot Number”** for inbound identity.

That undersells the job.

At receiving, the system must not only mint a unique party code — it must create the **first Material Identity**: the **root node** of the material’s complete genealogy, so origin is never lost from the first second of life in NOS.

```text
Lot / Batch     =  operational / logistics party attribute
Material Identity  =  lifelong traceable identity of a physical material state
```

Both exist. They are **not** the same concept.

---

## 2. Core definitions

### 2.1 Material Identity (MI)

**Material Identity** is the system identifier of a **specific physical material state** in the plant (or inbound).

| Property | Rule |
|----------|------|
| Lifetime | Bound to one physical state / form |
| On physical transformation | **New** MI is minted — never overwrite the old |
| Parent–child | Always recorded (Genealogy) |
| Origin | Reconstructable from receiving root → shipped FG |
| Manual entry | **Prohibited** — Numbering Service only |
| Generic sequence only | **Forbidden** — identity must reflect material category / class |

```text
Identifiers are generated automatically according to the centralized
Numbering Architecture (docs/13_Design/99_Shared/Document_Numbering.md).
```

**Format** (prefix, date segment, sequence pad, plant scope) is owned by Numbering.  
**Meaning** (which class · when to mint · parent link) is owned by **this** document.

### 2.2 Lot / Batch

**Lot / Batch** is an **operational and logistics** party attribute used for:

- Warehouse handling  
- Supplier / process party grouping  
- Inventory balance dimensions (where configured)  
- Quality sample grouping  

| Property | Rule |
|----------|------|
| May stay the same across some transformations | Policy (process-dependent) |
| May change | Policy |
| Does **not** replace Material Identity | Genealogy uses MI nodes |
| Manual Lot No | **Prohibited** — Numbering |

A single Material Identity may **carry** a Lot reference as an attribute.  
A Lot may contain many Material Identities (e.g. many logs in one supplier lot).

### 2.3 Related identifiers (not MI)

| Identifier | Role |
|------------|------|
| `MAT-…` | Catalog / master material definition (type, species, dims template) — **not** a physical instance |
| `GR-…` | Receiving **document** |
| Package / Pallet / Serial | Handling / unit identity — may link to MI. Partial GI: default = same Package Identity/barcode (remaining qty/status); optional company-policy **split** mints linked child PKG — see `INV_Goods_Issue_Workbench.md` |
| Production Order `PO-…` | Demand / plan — not material instance |

---

## 3. Absolute laws

```text
1. Receiving creates the FIRST Material Identity (genealogy root).
2. Every physical transformation mints a NEW Material Identity.
3. Existing Material Identities are never overwritten or reused.
4. Parent → child links are always preserved (Genealogy).
5. Genealogy is always reconstructable from first receiving to final shipped product.
6. Receiving shall never use generic sequential numbering devoid of material class.
7. Operators never type Material Identity or Lot numbers.
```

---

## 4. What drives Material Identity mint

Numbering Service selects series from **Identity Rules** using:

| Driver | Example |
|--------|---------|
| Material Category | Raw / WIP / FG |
| Material Family | Softwood lumber |
| Material Type / class | **LOG** · PRS · DRY · LAM · FJ · PAN · FG · … |
| Material Specification | Species, grade, moisture class (as configured) |
| Plant (and Company) | Scope of sequence |
| Identity Rules | Configured mapping Category+Type → series |

Illustrative composition (format **not** hard-coded here):

```text
Material Identity
        ↓
      LOG          ← material type / identity class
        ↓
      PINE         ← species / family segment (if rule says so)
        ↓
     20260806      ← date segment (if rule says so)
        ↓
      00045        ← sequence (Numbering Service)
```

Concrete string shape → `Document_Numbering.md` § Material Identity series.

What matters product-wise: identity **starts with the class of the physical state** (e.g. **LOG** for Tomruk), not a blank `ID-00001`.

---

## 5. Identity class chain (timber example)

Physical transformations mint a **new** class-prefixed Material Identity and link to parent(s):

```text
LOG   Tomruk (receiving root)
 ↓
PRS   Prizma
 ↓
DRY   Kurutulmuş prizma / lumber
 ↓
LAM   Lamel
 ↓
FJ    Finger Joint
 ↓
PAN   Panel
 ↓
FG    Finished Goods
```

Example (illustrative IDs — format from Numbering):

| Step | New MI | Parent MI |
|------|--------|-----------|
| Receive tomruk | `LOG-…-00045` | — (root) |
| Saw → prism | `PRS-…-00045` or new seq | `LOG-…-00045` |
| Kiln dry | `DRY-…` | `PRS-…` |
| Rip → lamella | `LAM-…` | `DRY-…` |
| Finger joint | `FJ-…` | `LAM-…` (merge: many parents) |
| Press panel | `PAN-…` | `FJ-…` / `LAM-…` |
| Finish / pack | `FG-…` | `PAN-…` |

**Split:** one parent → many children (each child = new MI).  
**Merge:** many parents → one child (one new MI; all parents linked).  
**Conversion:** one parent → one child (new MI; form/class change).

Authority for transformation shapes: `docs/03_system/Transformation_Model.md`.  
Authority for graph storage / inquiry: `Material_Genealogy.md`.

---

## 6. Receiving = genealogy root

```text
The Receiving Workbench is responsible for creating the first Material Identity.
This identity becomes the root of the complete material genealogy.
```

At **Post** (or line mint per policy), for each accepted physical line:

1. Resolve material type / identity class (e.g. LOG) from catalog + OCR / verification.  
2. Mint **Material Identity** via Numbering (class-aware — never generic-only).  
3. Optionally mint / attach **Lot / Batch** (operational party).  
4. Attach origin facts: supplier, truck, documents, photos, dims, species, qty, WH/location.  
5. Create Genealogy **root** node (no parent, or parent = standing-tree / harvest if already known).  
6. Create inventory transaction referencing **MI** (and Lot if used).

Receiving must not:

- Invent free-text Lot / MI  
- Use a single plant-wide sequential counter with no class  
- Treat GR document number as the material identity  

UX authority: `docs/00_Product/Process_Screens/INV_Receiving_Workbench.md`.

---

## 7. Attributes vs identity

Example — inbound Tomruk:

| Concern | Example | Concept |
|---------|---------|---------|
| Material Identity | `LOG-PINE-20260806-00045` | **MI** (root) |
| Lot / Batch | Supplier party / plant lot | **Lot** (attribute / party) |
| Catalog material | Tomruk · Scots Pine · template | `MAT-…` master |
| Species | Scots Pine | Spec attribute |
| Length / diameter | 4 m · 32 cm | Spec / measure |
| Supplier | ABC Forest | Origin attribute |
| Receiving | `GR-…` | Document |
| Photos / moisture | attachments / QI | Evidence attributes |

Production may keep the same Lot for logistics while minting a **new MI** on every transformation — Genealogy still holds.

---

## 8. Module responsibilities

| Module | Responsibility |
|--------|----------------|
| **Numbering** | Mint MI & Lot strings per series / Identity Rules |
| **Material Identity Architecture (this doc)** | When MI exists · class chain · vs Lot |
| **Genealogy** | Persist parent–child; never delete history |
| **Receiving Workbench** | Create **root** MI + origin context |
| **Production / Transformations** | Mint **child** MI on each physical transform |
| **Inventory** | Stock movements reference MI (and Lot as configured) |
| **Quality** | Holds / NCR / certificates hang on MI (and Lot) |
| **Traceability UX** | Forward / backward walk of MI tree |

---

## 9. Forbidden language (product)

| Avoid | Prefer |
|-------|--------|
| “Receiving only creates a Lot No” | “Receiving creates the **root Material Identity** (and may attach a Lot)” |
| “Update the lot code when sawn” | “Mint a **new MI**; link parent → child; Lot policy separate” |
| “Reuse the same identity after transform” | **Forbidden** for MI |
| Generic `ID-00001` with no class | Class-aware series (LOG / PRS / …) |

---

## 10. Cursor / implementation notes

1. Domain model: `MaterialIdentity` aggregate ≠ `Lot` / `Batch` entity.  
2. Genealogy edges: `ParentMaterialIdentityId` → `ChildMaterialIdentityId` + TransformationId.  
3. Receiving Post: mint MI first, then stock, then genealogy root event.  
4. FE: show MI as read-only badge (class-aware); never “Lot No ________”.  
5. Do not collapse MI into Lot in APIs — expose both when both exist.  
6. Screens/Flows **reference** this file — do not restate class tables elsewhere.

---

## 11. Related authorities

| Topic | Document |
|-------|----------|
| Number format / mint service | `Document_Numbering.md` |
| Genealogy graph | `docs/05_Modules/02_Production/Material_Genealogy.md` |
| Transformations | `docs/03_system/Transformation_Model.md` |
| Receiving UX | `INV_Receiving_Workbench.md` |
| Stock | `Inventory_Architecture.md` |
| Authority matrix | `docs/00_Product/DOCUMENTATION_AUTHORITY_MATRIX.md` |
| Constitution | `AI/NOS_CONSTITUTION/04_PRODUCT_ARCHITECT.md` § 2.3 |

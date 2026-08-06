# INV-RCV-001 — Receiving Workbench

**Module:** Inventory  
**Workspace:** Operations  
**Screen type:** **Workbench** (operational) — see `docs/13_Design/Common/Screen_Types.md`  
**Status:** Product Architect — authoritative receiving UX  
**Supersedes as primary UX:** phased-only “Receiving Wizard” shell  
**Companion (rules retained):** [`INV_Receiving_Wizard.md`](./INV_Receiving_Wizard.md) — Depo → Location → **Material Identity** (+ Lot) mint gates  
**Material Identity:** [`Material_Identity_Architecture.md`](../../13_Design/99_Shared/Material_Identity_Architecture.md) — receiving creates genealogy **root**
**Business capability source:** `docs/05_Modules/12_Purchasing/Receiving.md` (lifecycle & AI intents) · stock ownership: `Inventory_Architecture.md`

---

## Absolute rule

```text
This is NOT a Create / Edit form.
This is NOT a GoodsReceipt CRUD page.
This is a complete warehouse receiving operation executed by warehouse operators.
```

**Forbidden:** Save / Cancel form · `Code *` / Material Identity / Lot No / Package ID typed by hand · “Yeni Goods Receipt”.  
**Required:** Workbench session · scan / photo / OCR first · verify · **Post** → **root Material Identity**.

---

## Job to be done

> Depocu, kapıya gelen kamyonu **tek Receiving Workbench oturumunda** kayıt eder; evrakları yükler; OCR ve sayımla miktarı doğrular; hasarı belgeler; depo/lokasyon atar; etiket üretir; onaylar; **Post** ile stoğa işler ve malzemenin **kök Material Identity**’sini oluşturur (genealogy root — origin kaybolmaz).

**Not the job:** “Create a GoodsReceipt row” or type Warehouse / Material Identity / Lot / Package / Transaction numbers.


---

## CTA

| Locale | Label |
|--------|--------|
| EN | **Receive goods** / Open Receiving Workbench |
| TR | **Mal kabul başlat** / Mal Kabul Workbench |

Never: “Yeni” · “Create” · “Add Goods Receipt”.

Entry points: Inventory Dashboard queue · Operations nav · dock tablet · mobile terminal deep-link into the same Workbench session.

---

## Authority references (do not redefine)

| Topic | Authority |
|-------|-----------|
| **Material Identity** (genealogy root · MI vs Lot) | `docs/13_Design/99_Shared/Material_Identity_Architecture.md` |
| Identifiers / formats (WH, Location, MI, Lot, Serial, Package, Pallet, GR / inventory txn) | `docs/13_Design/99_Shared/Document_Numbering.md` § System Generated Identifiers · Material Identity series · Lot series |
| Genealogy graph | `docs/05_Modules/02_Production/Material_Genealogy.md` |
| Stock posting / immutability | `Inventory_Architecture.md` · `Inventory_Workflow.md` |
| Screen type / no Create form | `Screen_Types.md` · `UI_Patterns.md` § Workbench |
| Receiving capability catalog | `docs/05_Modules/12_Purchasing/Receiving.md` |
| Screen index | `Inventory_Screens.md` · `NOS_SCREEN_MAP.md` |

```text
Identifiers are generated automatically according to the centralized
Numbering Architecture (docs/13_Design/99_Shared/Document_Numbering.md).
Manual entry is prohibited. Users work with names; codes are display-only.
```

Operators **never** manually create: Warehouse codes · Location codes · **Material Identities** · Lot numbers · Serial numbers · Package IDs · Pallet IDs · Inventory transaction / GR numbers.

---

## MATERIAL IDENTITY (genealogy root)

```text
The Receiving Workbench is responsible for creating the first Material Identity.
This identity becomes the root of the complete material genealogy.
```

**Purpose is not** “mint any unique Lot No.”  
**Purpose is** to capture **origin** from the first second the material exists in NOS.

| Concept | At Receiving |
|---------|----------------|
| **Material Identity** | Minted — class-aware (e.g. **LOG** for Tomruk) — **root node** |
| **Lot / Batch** | Optional / parallel operational party — **not** a substitute for MI |
| **GR document** | `GR-…` — document only |

Authority: `Material_Identity_Architecture.md` · formats: `Document_Numbering.md`.

### Drivers (no generic-only sequence)

Material Identity shall be generated according to:

- Material Category  
- Material Family  
- Material Type / identity class (e.g. LOG)  
- Material Specification (species, dims, … as rules allow)  
- Plant  
- Identity Rules (Numbering)

Example inbound facts → class **LOG**:

| Fact | Example |
|------|---------|
| Type | Tomruk |
| Species | Scots Pine |
| Length | 4 m |
| Diameter | 32 cm |
| Supplier | ABC Forest |

Illustrative MI composition (exact format = Numbering):

```text
Material Identity → LOG → PINE → 20260806 → 00045
```

### After receiving — transformations mint NEW identities

```text
LOG → PRS → DRY → LAM → FJ → PAN → FG
```

Each arrow = physical transformation = **new** Material Identity + parent–child link.  
Existing MI is **never overwritten**. Genealogy remains reconstructable from this receiving root to final shipped product.

Lot may remain or change per logistics policy; **Material Identity chain** is the Digital Thread.

### Labels stage

Labels print **Material Identity** (primary) and Lot / Package / Pallet as applicable — all Numbering-minted, read-only.

---

## Screen type decision

| Candidate | Why not primary |
|-----------|-----------------|
| Wizard alone | Too linear; hides document viewer, gallery, split compare, sticky desk actions |
| Console alone | Continuous multi-truck desk — useful as **outer shell**; single truck session needs richer panes |
| Terminal alone | Scan-only post — complementary mobile surface, not full acceptance |
| Create form | Forbidden |

**Primary type = Operational Workbench:** multi-pane session for **one inbound truck / one Receiving record**, with **Wizard sections** as progress stages, **sticky action bar**, document viewer, galleries, and AI verify panels.

Optional companion: **Receiving Console** (queue of dock trucks) opens this Workbench per truck. Optional **scan Terminal** for counting / putaway lines.

---

## Workbench anatomy (enterprise WMS)

Inspired by SAP EWM / Infor WMS / Dynamics SCM / IFS — adapted to NOS laws.

```text
┌──────────────────────────────────────────────────────────────────────────┐
│ INV-RCV-001  Receiving Workbench          GR-… (system) · Draft/InProgress│
│ Truck 34 ABC 123 · Supplier · Gate 2 · Progress ●●●●○○○○○○  Stage 4/10   │
├──────────────┬───────────────────────────────────────────┬───────────────┤
│ STAGE RAIL   │  MAIN STAGE SURFACE                       │ CONTEXT PANEL │
│ (timeline)   │  Cards · Split · Viewer · Gallery         │ PO summary    │
│ 1 Truck      │                                           │ Differences   │
│ 2 Documents  │                                           │ Capacity hint │
│ 3 AI OCR     │                                           │ Attachments   │
│ 4 Verify     │                                           │ Audit peek    │
│ 5 Count      │                                           │               │
│ 6 Inspect    │                                           │               │
│ 7 Assign WH  │                                           │               │
│ 8 Labels     │                                           │               │
│ 9 Review     │                                           │               │
│ 10 Post      │                                           │               │
├──────────────┴───────────────────────────────────────────┴───────────────┤
│ STICKY ACTION BAR                                                        │
│  Save draft · Previous · Next stage · Print labels · Raise NCR · Post    │
└──────────────────────────────────────────────────────────────────────────┘
```

### Layout rules

| Pattern | Use |
|---------|-----|
| **Stage rail / Timeline** | 10 stages; jump allowed only to completed or unlocked stages |
| **Cards** | Only for interactive stage content (truck fields, verification rows) — not decorative hero cards |
| **Document Viewer** | Delivery note / packing list / PO / Excel / PDF side-by-side with OCR extract |
| **Image Gallery** | Truck photos · cargo · seal · damage · count sheets — unlimited |
| **Split View** | PO ∥ Delivery note ∥ OCR result (Material Verification) |
| **Side panel** | Live differences, suggested WH/location, minted IDs (read-only) |
| **Progress indicator** | Stage completion + blocking gates |
| **Sticky action bar** | Always visible — **Post** is primary when gates pass; never a lone Save/Cancel form footer |

Typing is minimized. Prefer: barcode / QR scan · camera capture · document upload · OCR · AI highlight → operator **confirm**.

---

## Session object

One Workbench session = one **Receiving record** (header + lines + attachments + inspections + labels).

| Field group | Operator enters / captures | System assigns |
|-------------|----------------------------|----------------|
| Identity | — | Receiving / GR number (`Document_Numbering.md`) |
| Truck | Plate, trailer, driver, supplier (name picker), arrival date/time, gate | — |
| Documents | Files / photos | Attachment IDs; linked to Receiving |
| Materials | Confirm OCR / scan / count | Material match to master (name-first) |
| Warehouse | Override suggestion if needed (name picker) | WH / Location codes display-only |
| Lots / packs | — | Lot / Serial / Package / Pallet via Numbering |
| Stock | Approve Post | Inventory transaction + balances |

---

## Stages (complete receiving operation)

Stages extend — do not replace — existing Depo → Location → Lot → QI → Post rules from the former Wizard spine.

```text
1  Truck Registration
2  Documents
3  AI OCR
4  Material Verification
5  Physical Counting
6  Material Inspection
7  Warehouse Assignment
8  Label Generation
9  Review
10 Posting
```

### 1 — Truck Registration

**Intent:** Register the arriving vehicle before unload.

| Capture | Notes |
|---------|--------|
| Truck plate | Required |
| Trailer plate | Optional |
| Driver | Name / ID as policy |
| Supplier | Name-first picker (Purchasing supplier) |
| Arrival date · time | Default now |
| Gate number | Dock / gate picker |

**Photos (multi):** Front · Rear · Side · Loaded cargo · Seal — gallery; camera preferred on tablet.

**Gate:** Truck plate + supplier + arrival time required before Documents.

---

### 2 — Documents

**Intent:** Attach inbound paperwork to the Receiving record (not orphan uploads).

**Accept:** Delivery Note · Packing List · Purchase Order · Excel · PDF · JPG · PNG · HEIC · camera photos.

| Rule | |
|------|--|
| Attach to Receiving | Every file belongs to this session |
| Link PO | Prefer PO from Purchasing when present |
| Viewer | Open in Document Viewer; multi-doc tabs |

**Gate:** At least one delivery document **or** explicit “manual inbound” policy exception.

---

### 3 — AI OCR

**Intent:** Extract structured line candidates so the operator does not retype the shipment.

After upload, system detects (best effort):

- Material (name / description)  
- Dimensions  
- Quantity  
- Unit  
- Bundle count  
- Supplier information  

Operator **reviews** and confirms / corrects fields. Never force full manual re-entry.

OCR confidence shown per field; low confidence highlighted.

**Gate:** Operator acknowledges OCR review (Accept extract / Edit extract) before Verification.

---

### 4 — Material Verification

**Intent:** Compare three sources in **Split View**.

```text
Purchase Order  ∥  Delivery Note  ∥  OCR Result
```

Highlight:

| Condition | UI |
|-----------|-----|
| Missing | Line on PO not on DN/OCR |
| Extra | Line on DN/OCR not on PO |
| Mismatch | Material / UoM conflict |
| Quantity difference | Ordered vs delivered vs OCR |
| Dimension difference | Spec / size conflict |

Operator resolves each row: Accept as received · Hold · Reject · Adjust qty (with reason).

**Gate:** No unresolved **blocking** mismatches (policy: Extra may require supervisor; Missing may allow partial receipt).

---

### 5 — Physical Counting

**Intent:** Capture physical qty without forcing keyboard entry.

Operator may:

1. **Scan** materials / packages (barcode / QR)  
2. **Upload** handwritten counting sheets  
3. **Photograph** handwritten notes → OCR → structured qty  

System converts handwriting to structured data; operator verifies.

```text
Prefer: Scan > Photo OCR > Upload sheet > Manual qty
```

**Gate:** Counted qty confirmed per line to be received (qty > 0).

---

### 6 — Material Inspection

**Intent:** Record visual condition before putaway (Inventory hold / QI link as needed).

| Condition flags | |
|-----------------|--|
| Visual OK | default |
| Broken · Wet · Blue stain · Crack · Mold · Damage | multi-select |

**Photos:** Unlimited damage / condition gallery.

Outcomes may set **Accept / Hold / Reject** (aligns with Wizard QI decision; Quality Incoming Inspection may trigger on Post).

**Gate:** Inspection decision recorded per line (or header policy for bulk OK).

---

### 7 — Warehouse Assignment

**Intent:** Choose destination stock target — **operator selects Depo** (existing law).

System **suggests**:

- Warehouse  
- Zone  
- Location  

based on Storage Rules · Capacity · Material Type.

Operator may change the suggestion (name-first warehouse / location pickers).

| Rule (retained) | |
|-----------------|--|
| Warehouse required before Location / Lot mint / Post | |
| Location filtered to selected warehouse | |
| Codes display-only after selection / mint | |

**Gate:** Warehouse selected; Location valid (unless WH-level balance policy).

---

### 8 — Label Generation

**Intent:** Produce physical identity labels — no manual numbering.

System generates (Numbering Architecture · Material Identity Architecture):

- **Material Identity** (class-aware root, e.g. LOG-…) — primary label  
- Lot / Batch (operational party — secondary)  
- Package number  
- Pallet number  
- QR · Barcode  

Operator prints labels (printer / Bluetooth / dock station). Preview is read-only IDs.

**Gate:** Material Identity mint succeeded; print optional but recommended before Post.

---

### 9 — Review

**Intent:** Single approval surface before stock mutation.

Display summary cards / read-only panes:

| Block | Content |
|-------|---------|
| Truck | Plates, driver, gate, photo count |
| Documents | Attached set |
| Materials | Lines, qty, UoM |
| Differences | Verification outcomes |
| Inspection | Flags + photo count |
| Warehouse | WH · zone · location (names) |
| Labels | Minted **Material Identity** · Lot / Package / Pallet (display-only) |
| Inventory summary | Expected balance delta |

Operator **Approves for Post** (explicit).

**Gate:** Approval checkbox / action; all prior stage gates green.

---

### 10 — Posting

**Intent:** Commit inventory truth **and** establish the genealogy root.

**Post** creates / updates (transaction-driven):

| Artifact | |
|----------|--|
| **Material Identity** (root) + optional Lot | |
| Genealogy root node (no parent, or harvest parent if known) | |
| Inventory transaction (immutable) — references MI | |
| Receiving / GR record (Posted) | |
| Warehouse stock / balance | |
| Audit trail | |
| Attachments retained on Receiving (origin evidence) | |

Traceability: truck → documents → lines → **Material Identity (root)** → Lot/package → transaction → balance → later child MIs.

On success: navigate to receipt library / putaway task (INV-027 future) / print confirmation.

**Finish action:** **Post** (not Save). **Save draft** allowed after Truck + Documents (and later stages) without mutating stock.

---

## Gates (summary)

1. Truck plate + supplier + arrival.  
2. Documents attached (or policy exception).  
3. OCR reviewed.  
4. Verification differences resolved per policy.  
5. Physical count confirmed.  
6. Inspection decision set.  
7. **Warehouse selected by operator**; location valid.  
8. Required **Material Identity** (+ Lot / Package / Serial as applicable) minted — manual entry prohibited.  
9. Operator Review approval.  
10. Post → stock + audit.

Serialized materials: Serial via Numbering Service. Quality Hold may block Available and create quarantine / hold balance per Inventory Workflow.

---

## Roles

| Role | In Workbench |
|------|----------------|
| Warehouse Operator | Primary — all stages except policy overrides |
| Inventory Controller | Resolve Extra / Missing · override WH suggestion |
| Quality Inspector | Inspection / Hold / Reject; may open from QI queue |
| Supervisor | Approve blocking mismatches · manual inbound exception |

---

## Mobile & scan

| Surface | Role |
|---------|------|
| Tablet / rugged Workbench | Full 10-stage session |
| Scan Terminal | Stage 5 count / Stage 8 label confirm — same Receiving ID |
| Camera | Truck · seal · damage · count sheets |

Offline: draft capture of photos/counts; sync before Post (`Inventory_Mobile.md`).

---

## What this is not

| Anti-pattern | |
|--------------|--|
| ResourcePage Create card for GoodsReceipt | Forbidden |
| Editable Lot No / Package ID / GR number fields | Forbidden |
| Save / Cancel as the only actions | Forbidden |
| Purchasing “Create GR” instead of Inventory Workbench | Forbidden — Purchasing owns PO; Inventory owns physical receipt |

---

## Cursor implementation notes

1. Screen type = **Workbench** — compose Stage rail · Document Viewer · Gallery · Split View · Sticky Action Bar.  
2. Do **not** generate a CRUD Create/Edit GoodsReceipt form.  
3. Reuse Numbering Service for all IDs; FE shows read-only badges.  
4. Warehouse = required name picker (Stage 7); Lot mint after material + WH known.  
5. Attachments API links files to Receiving ID.  
6. OCR / Vision = async jobs; UI shows progress + confidence.  
7. Receipt **Library** (INV-015) finds Posted/Draft sessions — CTA opens Workbench, never Create form.  
8. FE demo shortcuts may use Wizard shell until Workbench UI ships — product authority remains this document.

---

## Related

- [`INV_Receiving_Wizard.md`](./INV_Receiving_Wizard.md) — retained Depo / Lot rules (embedded here)  
- `Inventory_Screens.md` · `Inventory_Workflow.md` · `Inventory_User_Flows.md` FLOW-INV-001  
- `Inventory_Workspaces.md` · `Inventory_Navigation.md`  
- `Document_Numbering.md`  
- `docs/05_Modules/12_Purchasing/Receiving.md`  
- `UI_Patterns.md` § Workbench · `Screen_Types.md`

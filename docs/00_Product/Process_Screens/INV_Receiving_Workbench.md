# INV-RCV-001 — Receiving Workbench

**Module:** Inventory  
**Workspace:** Operations  
**Screen type:** **Workbench** (operational) — `Screen_Types.md` · `UI_Patterns.md`  
**Version:** 1.1 — Master Prompt aligned  
**Status:** Product Architect — authoritative receiving UX  
**Master intent:** Enterprise AI-powered Receiving Workbench (SAP EWM / Dynamics SCM / Infor WMS / IFS / Manhattan class + NOS AI)  
**Companion spine:** [`INV_Receiving_Wizard.md`](./INV_Receiving_Wizard.md) — Depo → Location → Material Identity (+ Lot)  
**Material Identity:** [`Material_Identity_Architecture.md`](../../13_Design/99_Shared/Material_Identity_Architecture.md)  
**Numbering:** [`Document_Numbering.md`](../../13_Design/99_Shared/Document_Numbering.md)  
**Capability catalog:** `docs/05_Modules/12_Purchasing/Receiving.md` · stock: `Inventory_Architecture.md`

---

## Absolute rules

```text
This is NOT a CRUD page.
This is NOT a Create / Edit form.
This is NOT a database editor.
This is an enterprise Receiving Workbench for warehouse operators
in real manufacturing environments.
```

```text
NOS does not begin with forms.
NOS begins with evidence.
AI converts evidence into structured business data.
Operators validate instead of typing.
```

**Forbidden:** Save/Cancel form · typed Material Identity / Lot / WH / Location / Package / Pallet / GR / txn numbers · “Yeni Goods Receipt”  
**Required:** Evidence-first Workbench · scan / photo / OCR / AI · verify · approve · **Post** → root **Material Identity** + stock + evidence archive

---

## Job to be done

> Depocu, kapıya gelen kamyonu **tek Receiving Workbench oturumunda** bitirir: kanıt toplar (foto · evrak · el yazısı); AI çıkarır ve karşılaştırır; sayım ve kaliteyi onaylar; depo atar; etiket basar; **Post** ile stok + **kök Material Identity** + audit + evidence archive oluşturur.

Operator time mix:

| Prefer | Minimize |
|--------|----------|
| Scanning · photographing · reviewing · verifying · approving | Typing |

**Not the job:** Create a GoodsReceipt row by filling a form.

---

## CTA

| Locale | Label |
|--------|--------|
| EN | **Receive goods** / Open Receiving Workbench |
| TR | **Mal kabul başlat** / Mal Kabul Workbench |

Entry: Warehouse Command Center · Operations · dock tablet · mobile · deep-link `?receivingId=`.

---

## Authority references (do not redefine)

| Topic | Authority |
|-------|-----------|
| Material Identity (root · vs Lot · class chain) | `Material_Identity_Architecture.md` |
| Identifier formats / mint | `Document_Numbering.md` |
| Genealogy graph | `Material_Genealogy.md` |
| Stock posting | `Inventory_Architecture.md` · `Inventory_Workflow.md` |
| Screen type | `Screen_Types.md` · `UI_Patterns.md` |
| Command Center entry | `Inventory_Dashboard.md` |
| Screens index | `Inventory_Screens.md` |

```text
Identifiers are generated automatically according to the centralized
Numbering Architecture (docs/13_Design/99_Shared/Document_Numbering.md).
Manual entry is prohibited. Users work with names; codes are display-only.
```

Operators **never** manually enter: Material Code · Warehouse Code · Location Code · Lot · Serial · Package · Pallet · Inventory Transaction Number · Receiving Number · Material Identity.

---

## EVIDENCE FIRST PRINCIPLE

Evidence is **not** “attachments on a form.”  
Evidence is the **primary input** of receiving.

| Evidence | Examples |
|----------|----------|
| Photos | Truck · seal · cargo · damage · labels · count sheets |
| Documents | Delivery note · packing list · PO · Excel · PDF · Word · certificates |
| Scans | Barcode / QR on packages |
| Handwriting | Paper lists · tablet ink · mobile ink |
| Voice / video | Optional future capture |

**Flow:** Capture evidence → AI structures data → Operator validates → System posts.

---

## Receiving flow (canonical)

```text
Truck Arrival
    ↓
1  Truck Registration
    ↓
2  Evidence Collection
    ↓
3  AI Document Understanding
    ↓
4  Document Comparison
    ↓
5  Physical Counting
    ↓
6  Photo Analysis
    ↓
7  Material Verification
    ↓
8  Quality Pre-Check
    ↓
9  Warehouse Assignment
    ↓
10 Identity & Numbering (system — continuous)
    ↓
11 Material Identity (root mint)
    ↓
12 Label Generation
    ↓
13 Review
    ↓
14 Posting → Inventory Transaction + Evidence Archive
```

Stages 10–11 are **system laws** surfaced in UI as read-only identity panels; operator does not “fill codes.”

---

## Workbench concept (UI)

AI-powered **operational workspace** — not a form.

Compose:

| Surface | Role |
|---------|------|
| Stage rail / Timeline | 14-step progress |
| Cards | Interactive stage content only |
| Split panels | PO ∥ DN ∥ Packing ∥ OCR ∥ Count |
| Document Viewer | Business evidence pages |
| Image Gallery | Truck · cargo · damage · sheets |
| OCR Viewer | Extracted fields + confidence |
| Warehouse Map | Suggest / override location |
| Material Preview | Name-first material + dims |
| AI Suggestions | Next action · mismatch tips |
| Sticky Action Bar | Draft · Next · Print · NCR · **Post** |

Enterprise refs: SAP EWM · Dynamics SCM · Infor WMS · IFS Cloud · Manhattan — adapted to NOS laws (Evidence First · Material Identity · no Create form).

```text
┌──────────────────────────────────────────────────────────────────────────┐
│ INV-RCV-001  Receiving Workbench     GR-… · MI preview · Draft/InProgress│
│ Truck · Supplier · Gate · Stage n/14                                     │
│ [Mal kabul] sticky progress                                              │
├────────────┬──────────────────────────────────────────┬──────────────────┤
│ TIMELINE   │ MAIN: Viewer · Gallery · Split · OCR     │ CONTEXT          │
│ 1–14       │ Material Preview · Warehouse Map         │ Diffs · AI hints │
│            │                                          │ Capacity · PO    │
├────────────┴──────────────────────────────────────────┴──────────────────┤
│ STICKY: Save draft · Back · Next · Print labels · Raise NCR · Post       │
└──────────────────────────────────────────────────────────────────────────┘
```

---

## Steps

### 1 — Truck Registration

Capture: Truck plate · Trailer plate · Driver · Supplier (name-first) · Arrival date/time · Security gate.

**Photos (multi):** Front · Rear · Side · Loaded cargo · Seal — camera preferred.

**Gate:** Plate + supplier + arrival required.

---

### 2 — Evidence Collection

Upload / capture **business evidence** (not orphan attachments):

Delivery Note · Packing List · Purchase Order · Excel · PDF · Word · JPG · PNG · HEIC · camera photos · Quality / FSC / Moisture certificates.

Support: Drag & drop · camera · mobile upload · multiple files.

Every file is linked to the Receiving session and later **Evidence Archive**.

**Gate:** ≥1 delivery evidence **or** policy “manual inbound” exception.

---

### 3 — AI Document Understanding

Immediately after upload, AI shall:

- Detect document type  
- Extract: Supplier · Material · Dimensions · Quantity · Species · Bundles · Packages · Moisture · Certificates · PO · Shipment info  

No manual re-entry of the full shipment. Operator reviews confidence + corrects sparse fields only.

**UI — Hataları düzelt:** Low-confidence / flagged fields highlight in the OCR Viewer. Primary action **Hataları düzelt** opens inline edit for those fields only (optional “apply suggested fixes”). Operator saves corrections, then **Accept extract**. Accept is blocked while flags remain or while edit mode is open.

**Gate:** Flagged OCR errors cleared · extract accepted.

---

### 4 — Document Comparison

Auto-compare:

```text
Purchase Order  ∥  Delivery Note  ∥  Packing List  ∥  Excel  ∥  OCR
```

Highlight: Missing / Extra material · Wrong qty · Wrong dimensions · Wrong species · Wrong package/bundle count · Missing documents.

Operator **validates** results — does not rebuild the grid from scratch.

**Gate:** Blocking mismatches resolved per policy.

---

### 5 — Physical Counting

Support:

- Handwritten paper (photo)  
- Photo of handwritten lists  
- Tablet / mobile handwriting  
- Barcode / QR scan  

AI OCR → structured qty/lines → operator verifies.

```text
Prefer: Scan > Handwriting OCR > Photo sheet > Manual qty
```

**Gate:** Counted qty confirmed (qty > 0) for lines to receive.

---

### 6 — Photo Analysis

AI analyses cargo / package photos:

Count bundles · Damaged packages · Broken straps · Wet · Mold · Blue stain · Cracks · Missing labels · Detect QR / barcodes.

AI proposes; **operator approves**.

**Gate:** Analysis reviewed (accept / override flags).

---

### 7 — Material Verification

Consolidate comparison across:

PO · Delivery Note · Packing List · Physical Count · OCR · Photos.

Highlight every remaining mismatch. Name-first material match to catalog.

**Gate:** Lines accepted for stock (partial receive allowed by policy).

---

### 8 — Quality Pre-Check

Record condition flags: Wet · Broken · Cracked · Blue stain · Mold · Rot · Warping · Mechanical damage · Visual OK.

Unlimited photos. May set Accept / Hold / Reject → QI / quarantine per Inventory + Quality workflow.

**Gate:** Decision per line (or bulk OK policy).

---

### 9 — Warehouse Assignment

System suggests Warehouse · Zone · Location · Bin from Storage Rules · Capacity · Material Type · Dimensions.

Operator may override (name-first pickers). **Warehouse required** before MI mint / Post (retained law).

**Gate:** WH selected; location valid (unless WH-level balance policy).

---

### 10 — Identity & Numbering (system law)

```text
Identifiers are generated automatically according to the centralized
Numbering Architecture (docs/13_Design/99_Shared/Document_Numbering.md).
```

UI shows read-only badges only. Never input fields for codes listed in Absolute rules.

---

### 11 — Material Identity (genealogy root)

```text
Receiving creates the first Material Identity.
This identity becomes the ROOT of the complete material genealogy.
```

Mint depends on: Material Category · Family · Type · Specification · Plant · Identity Rules  
(`Material_Identity_Architecture.md`).

Class-aware (e.g. **LOG** for Tomruk) — **never** generic sequential-only.

Later production: each physical transform → **new** MI + parent–child (LOG→PRS→DRY→LAM→FJ→PAN→FG…).

Optional **Lot/Batch** = operational party attribute — not a substitute for MI.

**Gate:** Root MI minted successfully for accepted lines.

---

### 12 — Label Generation

Auto-generate & print: Material Identity (primary) · Lot · Package · Pallet · QR · Barcode.

No manual numbering.

**Gate:** Mint OK; print recommended before Post.

---

### 13 — Review

Single approval surface:

Truck · Documents · Photos · AI / OCR results · Material list · Differences · Warehouse · Generated labels / MI · Inventory summary.

Operator **Approves for Post**.

**Gate:** Explicit approval; all prior gates green.

---

### 14 — Posting

**Post** (not Save) creates:

| Artifact |
|----------|
| Receiving / GR transaction (Posted) |
| Inventory transaction (immutable) |
| Warehouse stock / balance |
| **Material Identity** (root) + optional Lot |
| Genealogy root node |
| Audit trail |
| **Evidence Archive** (all photos, docs, OCR payloads, AI decisions) |

Everything traceable from truck → evidence → MI → stock → later child MIs.

---

## Gates (summary)

1. Truck + supplier + arrival  
2. Evidence collected  
3. AI extract reviewed  
4. Document diffs resolved  
5. Physical count confirmed  
6. Photo analysis reviewed  
7. Material lines accepted  
8. Quality pre-check decided  
9. Warehouse (+ location) set by operator  
10–11. Identifiers / root MI via Numbering — never typed  
12. Labels available  
13. Review approved  
14. Post → stock + MI + archive  

---

## Roles

| Role | Focus |
|------|--------|
| Warehouse Operator | Evidence · validate · Post |
| Supervisor | Override WH · unblock diffs |
| Quality | Pre-check / Hold / NCR |
| Inventory Controller | MI / Lot exceptions |

---

## Mobile

Tablet / rugged: full Workbench. Scan Terminal: count / label confirm on same Receiving ID. Offline: queue evidence; sync before Post (`Inventory_Mobile.md`).

---

## Cursor implementation notes

1. Screen type = **Workbench** — Evidence First; never CRUD Create form.  
2. Domain: `ReceivingEvidence` ≠ generic Attachment.  
3. AI jobs async: document understanding · compare · handwriting OCR · photo analysis.  
4. Post order: mint MI → stock txn → genealogy root → archive evidence → GR Posted.  
5. FE: stage rail 14 · Document Viewer · Gallery · Split · OCR Viewer · sticky **Post**.  
6. Demo may collapse 10–11 into identity panel; product authority remains this document.  
7. Command Center dock board opens this Workbench.

---

## Related

`INV_Receiving_Wizard.md` · `Material_Identity_Architecture.md` · `Document_Numbering.md`  
`Inventory_Workflow.md` · `Inventory_Screens.md` · `Inventory_Dashboard.md` · `Material_Genealogy.md`  
`docs/05_Modules/12_Purchasing/Receiving.md` · `UI_Patterns.md` § Workbench

# Document Management, Evidence Archive & Export

**Document:** Document Management · Evidence Archive · Export  
**Status:** Official — Product Architect  
**Version:** 1.0.0  
**Location:** `docs/13_Design/99_Shared/Document_Management_Evidence_and_Export.md`  
**Owns:** Complete digital file per Receiving / Goods Issue · Document Library · Evidence Archive capabilities · Document History chain · Ops search · Professional export (Excel / CSV / PDF) · Audit permanence for evidence  
**Does not own:** Object-storage plumbing (→ `File_Storage.md`) · Identifier formats (→ `Document_Numbering.md`) · Material Identity meaning (→ `Material_Identity_Architecture.md`) · Stock ledger (→ `Inventory_Architecture.md`) · Visual PDF branding (→ Design System `06_Documents/`) · Screen step UX (→ Receiving / Goods Issue Workbench PRDs)

---

## 1. Why this exists

Every Receiving and every Goods Issue must leave a **complete digital file**.

Operators must reopen that file at any time: preview, download, version, search, export — without losing OCR, AI analysis, corrections, or audit.

```text
File Storage     =  how bytes are stored (infrastructure)
This document    =  product law for evidence · library · history · export · audit
Workbench PRDs   =  capture UX (Evidence First jobs)
```

`ReceivingEvidence` / issue evidence is **business proof**, not an orphan attachment.

---

## 2. Absolute laws

```text
1. Every Receiving and Goods Issue operation creates a complete digital file.
2. Nothing is deleted automatically.
3. Nothing is overwritten — replace creates a new version; originals remain.
4. Every file remains permanently linked to its transaction.
5. Every export is reproducible from the same filters / snapshot rules.
6. Every document is traceable (upload → OCR → AI → corrections → versions → audit).
7. Operators access all uploaded evidence at any time (permission permitting).
```

---

## 3. Document Management — complete digital file

### 3.1 Scope (mandatory for INV Receiving & Goods Issue)

When a Receiving or Goods Issue session exists, the system **shall** permanently store (when captured) and keep linked to that transaction:

| Evidence class | Examples |
|----------------|----------|
| Truck photos | Front · rear · side · cargo · seal |
| Material photos | Packages · damage · wet · mold · labels |
| Delivery notes | İrsaliye / DN |
| Packing lists | Supplier packing |
| Purchase orders | PO PDF / print / photo |
| Spreadsheets | Excel / CSV uploads used as evidence |
| PDF files | Any business PDF attached to the session |
| Word documents | Supplier letters / specs |
| Handwritten counting sheets | Photo or scan of paper count |
| Quality certificates | Lab / supplier QC |
| Moisture certificates | Nem sertifikası |
| FSC certificates | Chain-of-custody |
| Videos | Dock / unload clips |
| Voice notes | Operator spoken notes |

Additional media allowed by `File_Storage.md` categories may attach under the same permanence rules.

### 3.2 Linkage

| Rule | Detail |
|------|--------|
| Owner | Transaction ID (Receiving / GR session · Goods Issue / GI) |
| Secondary links | Material Identity · Lot (display) · Warehouse · Supplier · PO · User · Date |
| Orphan files | Forbidden for ops evidence — every file has an owner transaction |
| Soft-delete of transaction | Evidence remains; access follows retention + permission (`Data_Retention.md`) |

### 3.3 Deletion policy

```text
Automatic purge of ops evidence = FORBIDDEN.
User “delete” of posted evidence = FORBIDDEN (or soft-hide with audit only if policy + permission).
Replace = new version; prior bytes stay.
```

---

## 4. Document Library (per transaction)

Each Receiving and each Goods Issue transaction **shall** include a **Document Library**.

### 4.1 Operator capabilities

| Capability | Rule |
|------------|------|
| **Preview** | In-app preview for supported types (image · PDF · common office via viewer) |
| **Download original** | Exact original bytes + original filename metadata |
| **Download all as ZIP** | Bulk package of current (or selected) library files |
| **Replace document** | Creates **new version**; never destroys prior version |
| **View versions** | Full version list with who / when / why |
| **Compare revisions** | Side-by-side or diff for text/PDF where feasible; image compare for photos |
| **Search documents** | Within the transaction library (name · type · uploader · date) |
| **Filter by document type** | Delivery note · packing · PO · certificate · photo · video · voice · sheet · other |
| **Upload history** | Chronological list of all uploads and replaces |

Library is reachable from: Workbench session · Receipt/Issue library detail · Evidence Archive entry for that txn.

### 4.2 Relationship to ADM-009 File Library

| Surface | Role |
|---------|------|
| **Transaction Document Library** | Ops digital file for one Receiving / GI (this law) |
| **ADM-009 File Library** | Platform-wide file finder (→ Platform File Upload / File Library screen) — must deep-link into transaction libraries, not duplicate permanence rules |

---

## 5. Evidence Archive

On **Post** (and progressively during the session), evidence enters the **Evidence Archive** bound to the inventory transaction.

### 5.1 Archive capabilities

| Capability | Description |
|------------|-------------|
| Preview | Same as Document Library |
| Download | Original + optionally derived (OCR text, AI JSON) |
| Bulk download | ZIP of archive contents |
| Print | Printable evidence pack / cover sheet (`Printing.md`) |
| OCR view | Structured OCR fields + confidence + flagged corrections |
| AI summary | Document understanding / photo analysis / pick validation summaries |
| Timeline | Capture → OCR → AI → user correction → approve → Post |
| Version history | Per-file versions |
| Audit history | Who uploaded / replaced / viewed / exported / printed |

### 5.2 Evidence First (product rule)

```text
NOS does not begin with forms.
NOS begins with evidence.
AI converts evidence into structured business data.
Operators validate instead of typing.
Evidence belongs to the transaction — not an orphan Attachment slot.
```

Capture UX lives in Workbench PRDs (`INV_Receiving_Workbench.md` · `INV_Goods_Issue_Workbench.md`).  
Permanence, library, history, and export laws live **here**.

---

## 6. Document History (chain)

Every uploaded file **shall** preserve the following chain (append-only; no overwrite of prior nodes):

```text
Original File
      ↓
OCR Result
      ↓
AI Analysis
      ↓
User Corrections
      ↓
Version History
      ↓
Audit Log
```

| Node | Content |
|------|---------|
| Original File | Immutable blob + checksum + original name |
| OCR Result | Extracted fields · raw text · confidence · engine/version |
| AI Analysis | Document type · comparisons · photo findings · recommendations |
| User Corrections | Field-level before/after · actor · timestamp (“Hataları düzelt”) |
| Version History | Replace / re-upload events |
| Audit Log | Access · export · print · permission-denied attempts (as policy) |

Derived artifacts (OCR JSON, AI summary) are **linked children** of the original — never a silent overwrite of the original.

---

## 7. Search (Receiving & Goods Issue operations)

Operators **shall** search prior Receiving and Goods Issue operations by:

| Dimension |
|-----------|
| Truck plate |
| Supplier |
| Purchase Order |
| Material (name-first; code display-only) |
| Material Identity |
| Lot |
| Warehouse |
| Date (range) |
| User (uploader / poster) |
| Document name |
| File type |

Search returns **transactions** (with evidence counts) and may deep-link into Document Library / Evidence Archive.  
Implementation filters follow `Search_Filtering.md` · pagination/export coverage `Pagination.md`.

---

## 8. Export

### 8.1 Formats

Professional export files **shall** support:

| Format | Extension |
|--------|-----------|
| Excel | `.xlsx` |
| CSV | `.csv` |
| PDF | `.pdf` |

### 8.2 Exportable inventories (Inventory ops)

Operators **shall** export:

| Export set |
|------------|
| Receiving List |
| Goods Issue List |
| Material List |
| Inventory Transactions |
| Counting Results |
| Difference Report |
| Inspection Results |

Exports respect permissions (`*.Export` / report permissions).  
Filtered export covers the **full filtered set** (async when large — `Pagination.md`).

### 8.3 Reproducibility

```text
Same filters + same as-of rules → same logical result.
Every export leaves an audit record (who · when · filter · format · row count).
Exports do not mutate business data.
```

---

## 9. Excel export content

Generated Excel files **shall** preserve columns (when applicable to the export set) so the file is **immediately usable without manual formatting**:

| Column group | Fields |
|--------------|--------|
| Material | Material information (name · catalog display) |
| Identity | Material Identity |
| Location | Warehouse · Location |
| Quantity | Quantity · Unit |
| Spec | Dimensions · Species · Package count |
| Logistics | Lot |
| State | Status · Approval status |
| Audit | User · Date · Time |
| Notes | Remarks |

Presentation rules:

- Header row frozen · typed columns (dates/numbers) · no decorative-only empty sheets required for use  
- Codes may appear as **display columns**; UX remains name-first in interactive screens  
- Character encoding UTF-8 (CSV) · Excel Unicode-safe  

Visual PDF styling → Design System documents; content columns → **this** law.

---

## 10. Audit

```text
Nothing shall be overwritten.
Every uploaded file shall remain permanently accessible (within retention + permission).
Every export shall be reproducible.
Every document shall be traceable.
```

| Event (minimum) | Logged |
|-----------------|--------|
| Upload / replace / version | Actor · txn · file id · checksum |
| OCR / AI run | Engine · input file · output artifact ids |
| User correction | Field diffs |
| Preview / download / ZIP / print / export | Actor · scope |
| Post archive seal | Txn · file set hash / manifest |

Audit export itself follows `Audit_Log.md`.

---

## 11. Permissions (types)

| Capability | Permission type (conceptual) |
|------------|------------------------------|
| Upload evidence | Upload |
| Preview / download | Read / Download |
| Replace / version | Upload + version permission |
| Bulk ZIP | Download / Export |
| Export lists / Excel | Export |
| Soft-hide (if ever allowed) | Archive + elevated |

Exact role grants → Permission model authorities. Screens only label roles.

---

## 12. Ownership boundaries

| Concern | Authority |
|---------|-----------|
| Bytes · buckets · MIME · infra versioning API | `File_Storage.md` |
| Evidence permanence · library · history chain · ops search · export sets | **This document** |
| Capture stages · Evidence First UX | `INV_Receiving_Workbench.md` · `INV_Goods_Issue_Workbench.md` |
| Grid/report chrome | Design System Reports / Data Grid |
| Retention windows | `Data_Retention.md` (must not contradict “no automatic delete” for posted ops evidence without PA change here) |

---

## 13. Screen map notes

| ID / surface | Role |
|--------------|------|
| INV-RCV-001 / INV-ISS-001 | Capture evidence into the digital file |
| INV-015 / INV-017 libraries | Open transaction → Document Library / Archive |
| ADM-009 File Library | Platform finder → deep-link to txn libraries |

No separate CRUD “Documents Create” form.

---

## 14. Final statement

```text
Receiving and Goods Issue leave a complete digital file.
Evidence is permanent, versioned, and transaction-linked.
Operators preview, download, ZIP, version, search, and export.
History is Original → OCR → AI → Corrections → Versions → Audit.
Exports are professional, column-complete, and reproducible.
```

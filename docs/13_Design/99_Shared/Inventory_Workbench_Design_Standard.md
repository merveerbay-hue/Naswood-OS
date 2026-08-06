# Inventory Workbench Design Standard

**Document:** Inventory Workbench Design Standard  
**Status:** Official — Product Architect  
**Version:** 3.0.0  
**Location:** `docs/13_Design/99_Shared/Inventory_Workbench_Design_Standard.md`  
**Owns:** COMPLIANCE BY DESIGN philosophy for Inventory / Warehouse Operations Workbenches · operator vs system roles · demand-backed movement law · AI recommend + IGNORE AI · audit / revision / electronic approval composition for ops · immutable posted transaction UX law · canonical package grid column set · package status vocabulary · Evidence First / Scan–Verify–Review–Approve interaction model  
**Does not own:** Stock ledger algorithms (→ `Inventory_Architecture.md` · `Inventory_Workflow.md`) · Package Allocation Workspace interaction capabilities (→ `Package_Allocation_Workspace.md`) · Evidence permanence / Document Library / Export columns (→ `Document_Management_Evidence_and_Export.md`) · Identifier formats (→ `Document_Numbering.md`) · Material Identity meaning (→ `Material_Identity_Architecture.md`) · Audit engine payloads (→ `Audit_Log.md`) · Approval engine routing (→ `Approval_Workflow.md`) · Process-specific Post artifacts (each Workbench PRD)

---

## 1. Design philosophy — COMPLIANCE BY DESIGN

```text
NOS Inventory Workbenches are built for real manufacturing companies.
Every output must be suitable for ISO · TSE · FSC · PEFC · customer audits.

COMPLIANCE BY DESIGN means:
• Controls are structural — not afterthoughts or optional checklists
• Evidence is captured in the flow — not reconstructed later
• Posted history is immutable — corrections create new records
• Identifiers are system-generated — operators never invent codes
• Genealogy is continuous — Supplier → Receiving → Package → Issue → Product → Customer
```

This standard applies to **all** Inventory / Warehouse Operations Workbenches.  
Goods Issue (`INV_Goods_Issue_Workbench.md`) is the **first full consumer** of Version 3.0.  
Receiving and subsequent ops Workbenches **compose** the same laws.

```text
Extend Architecture · Workflow · Screens · Shared authorities.
Never replace business rules defined there.
```

---

## 2. What this is — and is not

| This IS | This is NOT |
|---------|-------------|
| AI-powered Warehouse Operations Workbench | CRUD page |
| Scan · Verify · Review · Approve session | Create Form / Edit Form |
| Evidence First execution surface | Database editor |
| Compliance-grade digital file producer | Save / Cancel primary interaction |

```text
Warehouse operators should not spend their time entering data.

Operators:  Scan · Verify · Review · Approve
System:     Think · Compare · Recommend · Validate · Warn · Generate · Track
```

Primary interaction is **not** Save / Cancel.  
Primary interaction is **Accept AI** / **IGNORE AI** / **Post** (with sticky action bar).

---

## 3. Operator vs system

| Actor | Responsibility |
|-------|----------------|
| **Operator** | Scan packages · accept or override AI · classify take (Good/Damaged/…) · capture evidence when required · complete closing checklist · request / confirm approval · Post |
| **System** | Recommend WH / location / package / qty / route · validate FIFO/FEFO/reservation/quality/customer/WH rules · live recalculate · mint identifiers · write Inventory Transactions · seal audit · preserve Document Library · maintain genealogy |

Operators **never** manually enter:

Material Codes · Package Numbers · Lot Numbers · Warehouse Codes · Location Codes · Goods Issue / GR / Transfer Numbers · Inventory Transaction Numbers · Material Identity strings.

They work with **names**, **scans**, and **display-only** codes.  
Authority: `Document_Numbering.md`.

---

## 4. Business document law

Material **cannot leave** (or enter, where applicable) inventory without an **approved business document**, except explicit permission-controlled Manual exceptions with reason + audit.

### Goods Issue origins (mandatory)

| Source |
|--------|
| Production Order |
| Sales Order |
| Maintenance Order |
| Internal Request |
| Sample Request |
| R&D Request |
| Scrap Request |
| Warehouse Transfer |
| Manual Issue (**permission + reason + audit only**) |

Process PRDs list consumer-specific Manual rules.  
Stock truth and posting rules remain in `Inventory_Architecture.md` / `Inventory_Workflow.md`.

---

## 5. AI picking & IGNORE AI

### AI shall recommend

Warehouse · Location · Package · Quantity · Route  

according to:

FIFO · FEFO · Reservation · Customer Rules · Warehouse Rules · Quality Status · Material Availability  

(and process-specific constraints in the Workbench PRD).

### IGNORE AI (required control)

```text
Provide a clearly visible IGNORE AI control
(locale: EN “Ignore AI” · TR “Yoksay” — process PRD may pair with Accept / “Kabul Et”).

When selected:
→ Open Warehouse Explorer (or equivalent browse pool)
→ Allow manual package selection
→ All overrides SHALL be logged (who · when · AI proposal · operator choice · reason when required)
```

AI **Validation continues** after IGNORE AI.  
Operator may override **selection**; operator may **not** silently violate hard validation without authorized waiver.

---

## 6. Package Allocation Workspace (center)

The **center** of every package-selecting Inventory Workbench SHALL be the shared **Package Allocation Workspace**.

```text
Authority: Package_Allocation_Workspace.md
NOT a table. Interactive operational allocation workspace.
```

Shall support (detail → Shared PAW): multi-package · partial package · Excel-like editing · barcode scanning · drag & drop · keyboard shortcuts · sorting · filtering · grouping · AI suggestions · manual override · live calculations · Take From Package disposition · Package Closing Checklist.

Process PRDs **compose** — they do not fork a second grid language.

---

## 7. Canonical package grid columns

Every Inventory Package Allocation Workspace **shall be able to display** (show/hide by role/process config; defaults per Workbench PRD):

| Column |
|--------|
| Package Number |
| Material Identity |
| Warehouse |
| Location |
| Lot |
| Species |
| Dimensions |
| Quality Grade |
| Moisture |
| Available Quantity |
| Selected Quantity |
| Remaining Quantity |
| Available m³ |
| Selected m³ |
| Remaining m³ |
| Weight |
| Package Status |
| Production Date |
| Receiving Date |
| Supplier |
| Photos |
| Reservation Status |
| Customer Reservation |

Name-first display; codes read-only.  
Live Available syncs from inventory truth.

---

## 8. Multiple package picking

One operational document (e.g. one Goods Issue) **may** consume material from **multiple packages**.

System recommends the **minimum number of packages** while respecting:

FIFO · Reservation · Quality · Species · Dimensions · Moisture · Lot · Customer Rules  

Operator may: **Accept · Modify · Remove · Add · IGNORE AI**.

---

## 9. Partial package usage

```text
Packages are physical objects.
Barcode / Package Identity remain unchanged on partial use (default).
DO NOT generate a new package number.
DO NOT print a new barcode.
Update only: Available · Remaining · m³ · Weight · Status (via Inventory Transactions).

Example: PKG-00254 · 120 pcs → issue 40 → remaining 80 · same barcode.
```

Optional company-policy **package split** mints child IDs with full parent→child traceability — never silent rename.  
Detail: `Package_Allocation_Workspace.md` · process PRDs.

---

## 10. Package status vocabulary

| Status | Meaning |
|--------|---------|
| Available | Pickable / putaway-ready (process context) |
| Reserved | Bound to demand |
| Picking | Active pick / issue session |
| Partially Used | Remaining > 0 after partial consumption |
| Quality Hold | Blocked for Quality disposition |
| Damaged | Damaged classification / quarantine path |
| Consumed | Remaining = 0 |
| Closed | Business-closed (policy) |

Status changes are **transaction-driven** and audited — never free-hand master edits.

---

## 11. Damage, scrap & quality hold during picking

**Detail authority:** `Package_Allocation_Workspace.md` § Take From Package.

Categories when removing material: **Good · Damaged · Quality Hold · Rework · Scrap**.

```text
Σ categories = Picked.
Package conserved: Good + Damaged + Hold + Scrap + Rework + Remaining = Original.
```

| Path | Law |
|------|-----|
| **Damage** | Evidence mandatory (photos · notes · voice · video · damage type) |
| **Scrap** | NEVER returns to package · separate Inventory Transaction · preserves MI · Package · Receiving · Supplier · Operator · Date · Reason · Evidence |
| **Quality Hold** | May auto-move to Hold / Quarantine · only Quality may release or scrap |

Inventory integrity must always be preserved.

---

## 12. Document Library, management & export

**Authority:** `Document_Management_Evidence_and_Export.md`.  
**Infra:** `File_Storage.md` (storage only).

Every transaction permanently preserves (non-exhaustive): truck / material photos · delivery notes · packing lists · POs · Excel/PDF/Word · quality / FSC / moisture certificates · handwritten notes · videos · voice · OCR · AI analysis.

```text
Nothing important is auto-deleted.
Complete digital file per operational session.
```

Document management: Preview · Download · ZIP · Print · Search · Filtering · Version History · OCR View · AI Summary · Timeline.

Export (audit-ready): Excel · CSV · PDF — Goods Issue / Inventory Transaction / Material Consumption / Picking / Difference / Inventory History reports (process PRD names the set).

---

## 13. Audit trail

**Engine authority:** `Audit_Log.md`.  
**Workbench law:** every significant operator and system action is recorded; nothing is overwritten.

Examples (non-exhaustive):

| Event |
|-------|
| Created By / Created Date |
| Modified By / Modification Reason |
| AI recommendation accepted |
| IGNORE AI / override selection |
| Scan / verify outcomes |
| Disposition classification |
| Evidence attach |
| Approval / reject / return |
| Posting / Completion |
| Archive |

Override History panels on Workbenches are **operator-facing views** of the same append-only truth sealed into audit on Post.

---

## 14. Revision management

```text
Existing records shall never be edited silently.
Corrections create revisions (or reverse / correction transactions — § 16).
```

Each revision stores:

| Field |
|-------|
| Original Value |
| Corrected Value |
| Reason |
| User |
| Date |
| Approval (when required) |

Draft Workbench sessions may update working state; **posted** business documents and inventory transactions follow immutable / reverse laws below.

Compose with `Approval_Workflow.md` (Return for Revision) where electronic approval applies.

---

## 15. Electronic approvals

**Engine authority:** `Approval_Workflow.md`.

Inventory Workbenches support electronic approvals as configured per company / document type. Examples:

| Role |
|------|
| Warehouse Operator |
| Supervisor |
| Quality |
| Warehouse Manager |

Electronic approval history is **permanent** and audit-linked.  
Workbench surfaces show who approved, when, and outcome — never a silent “approved” flag without history.

---

## 16. Immutable transactions

```text
Posted Inventory Transactions shall never be editable.
If a mistake occurs → Reverse Transaction and/or Correction Transaction.
Never modify history.
```

Aligns with `Inventory_Architecture.md` (immutable inventory transactions).  
Workbench UX: Post seals the session; corrections open a **new** controlled flow, not an Edit form on the posted txn.

---

## 17. Traceability (genealogy)

The system shall always reconstruct:

| Node |
|------|
| Supplier |
| Receiving Operation |
| Warehouse |
| Location |
| Package |
| Production Order (when applicable) |
| Finished Product |
| Shipment |
| Customer |

```text
Complete genealogy is mandatory.
Traceability = Inventory Transactions + Material Genealogy.
Never by overwriting Package IDs or Material Identities.
```

Authorities: `Material_Identity_Architecture.md` · `Material_Genealogy.md` · Inventory / Quality Architecture (joint inquiry UX).

---

## 18. Numbering

**Authority:** `Document_Numbering.md`.

All identifiers are generated automatically according to the centralized NOS Numbering Architecture.  
Operators never invent codes. Selecting an **existing** Package / Lot / MI for consumption is allowed; minting new Package Identity is **not** part of default partial issue.

---

## 19. Compliance frameworks

Workbench design and records SHALL be compatible with:

| Framework |
|-----------|
| ISO 9001 |
| ISO 14001 |
| ISO 45001 |
| ISO 27001 |
| FSC Chain of Custody |
| PEFC |
| TSE |
| Customer Quality Audits |

```text
Every screen shall produce records suitable for internal and external audits.
Evidence First · append-only history · immutable posted txns · CoC-preserving MI/Package links.
```

FSC/PEFC claims and certificate attachment rules remain in Quality / CoC process docs; Inventory Workbenches **preserve** certificate evidence and material identity links without breaking the chain.

---

## 20. Evidence First UX model

```text
Design one of the best AI-powered warehouse Workbenches in manufacturing —
not a traditional ERP form.

Operator time: reviewing AI recommendations · scanning · verifying evidence.
System time: recommending · validating · generating · tracking.
```

| Principle | Practice |
|-----------|----------|
| Evidence First | Capture photos/docs/voice when the exception happens |
| Scan First | Barcode / QR drives row focus and confirmation |
| AI First pick | Default path is Accept recommendation |
| OVERRIDE visible | IGNORE AI is first-class, logged |
| Sticky actions | Draft · Next · Accept · Ignore AI · NCR · Post — not Save/Cancel form |
| Audit ready | Every Post produces digital file + txn + genealogy + approval history |

---

## 21. Consumer Workbenches

| Workbench | Composition status |
|-----------|-------------------|
| **Goods Issue** (`INV_Goods_Issue_Workbench.md`) | **Full consumer of v3.0** |
| **Receiving** (`INV_Receiving_Workbench.md`) | Compose — Evidence First already; align audit/revision/immutable language |
| Warehouse Transfer | Compose when designed |
| Production Material Consumption | Compose when designed |
| Shipping | Compose when designed |
| Inventory Count | Compose when designed |

Design Program sequence: `Inventory_Design_Program.md`.

---

## 22. Authority composition map

| Concern | Read from |
|---------|-----------|
| Stock / reservation / txn immutability | `Inventory_Architecture.md` · `Inventory_Workflow.md` |
| Package Allocation UX + disposition | `Package_Allocation_Workspace.md` |
| Document Library / Export | `Document_Management_Evidence_and_Export.md` |
| Audit engine | `Audit_Log.md` |
| Approval engine | `Approval_Workflow.md` |
| Numbering | `Document_Numbering.md` |
| Material Identity / Lot | `Material_Identity_Architecture.md` |
| Genealogy graph | `Material_Genealogy.md` |
| Screen type | `Screen_Types.md` · `UI_Patterns.md` |
| **This standard** | Compliance by Design · Workbench interaction laws · column/status vocabulary · audit/revision/approval composition for Inventory ops |

---

## 23. Cursor implementation notes

1. Do **not** implement Inventory ops as Create/Edit CRUD forms.  
2. Center package flows on Package Allocation Workspace.  
3. Wire IGNORE AI → Warehouse Explorer + override audit.  
4. Post → immutable Inventory Transaction(s); corrections = reverse/correction flows.  
5. Evidence / Export / permanence → Shared Document Management — reference only.  
6. Approvals / audit → shared engines — Workbench shows history, does not invent a second ledger.  
7. Process PRDs extend this standard; they do not restate Numbering / MI / PAW / Evidence algorithms.

---

## Related

`Package_Allocation_Workspace.md` · `Document_Management_Evidence_and_Export.md` · `Document_Numbering.md` · `Material_Identity_Architecture.md` · `Audit_Log.md` · `Approval_Workflow.md` · `Inventory_Architecture.md` · `Inventory_Workflow.md` · `Inventory_Design_Program.md` · `INV_Goods_Issue_Workbench.md` · `INV_Receiving_Workbench.md` · `UI_Patterns.md` · `Screen_Types.md`

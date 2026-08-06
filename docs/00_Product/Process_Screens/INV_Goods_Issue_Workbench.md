# INV-ISS-001 — Goods Issue Workbench

**Module:** Inventory  
**Workspace:** Operations  
**Screen type:** **Workbench** (warehouse execution) — `Screen_Types.md` · `UI_Patterns.md`  
**Version:** 2.0 — Master Prompt v2.0 aligned  
**Status:** Product Architect — authoritative issue UX  
**Supersedes as primary UX:** linear [`INV_Issue_Wizard.md`](./INV_Issue_Wizard.md) (retained as spine)  
**Stock truth:** `Inventory_Architecture.md` · `Inventory_Workflow.md`  
**Identity:** `Document_Numbering.md` · `Material_Identity_Architecture.md` · `Material_Genealogy.md`  
**Evidence / Document Library / Export:** [`Document_Management_Evidence_and_Export.md`](../../13_Design/99_Shared/Document_Management_Evidence_and_Export.md)  
**Package / barcode immutability (format refs):** `Barcode_QR_Model.md` · `Barcode_Strategy.md` · `Naming_Standards.md` · Packaging module  
**Design program:** `Inventory_Design_Program.md` § 7 (PA-directed ahead of Putaway)

---

## Changelog

| Version | What landed |
|---------|-------------|
| 1.0 | Demand-backed Workbench spine · AI pick · scan/verify · quality · thin loading · Post · Evidence First |
| **2.0** | Override Mode + Warehouse Explorer · Package Selection detail · **Partial Package Consumption** · Package Identity permanence · expanded Loading · Document Library / Export UX surfaces · Override History · gates table |

v2 **extends** Inventory Architecture / Workflow / Screens — it does not replace stock ledger or numbering algorithms.

---

## Absolute rules

```text
This is NOT a CRUD screen.
This is NOT a Create / Edit form.
This is NOT a database editor.
This is an AI-powered Warehouse Operations Workbench.
Inventory quantities are NEVER edited directly.
Every Goods Issue creates Inventory Transaction(s).
```

```text
Never allow issuing materials without a business document
(except permission-controlled Manual Issue).
AI prepares · Operator scans · reviews · verifies · confirms.
The system calculates · validates · warns · generates · tracks.
```

```text
PACKAGE IDENTITY LAW (wood manufacturing — Master Prompt v2.0)
─────────────────────────────────────────────────────────────
Packages are physical objects.
Package Identity never changes.
Package Barcode never changes.
Package QR Code never changes.
As long as the physical package exists, its identity is unchanged.
Partial consumption updates quantities / status only —
it does NOT mint a new package number and does NOT print a new barcode.
Traceability = Inventory Transactions + Material Genealogy —
never by changing Package IDs.
```

**Forbidden:** Bare Save form · typed Material / Lot / Package / Pallet / WH / Location / GI / txn / MI codes · orphan issue · inventing a new `PKG-…` on partial consume · reusing package barcodes  
**Required:** Business document → load demand → AI pick (default) → scan / verify → evidence → quality → destination → review (incl. overrides) → **Post**

---

## Question template answers

| # | Question | Answer |
|---|----------|--------|
| 1 | **Who is the user?** | Warehouse Operator (picker) · Warehouse Supervisor · Inventory Controller (exceptions) · optionally Production / Maintenance / Shipping coordinators as request owners |
| 2 | **Real-life job?** | Takes approved demand, goes to bin, picks correct package/lot/qty (often **partial package**), may override AI via Warehouse Explorer, proves exceptions, confirms issue so stock ↓ and reservation clears — **same physical barcode stays on the package** |
| 3 | **Documents?** | Source: Production Order · Maintenance WO · Sales Order · Sample / R&D / Scrap / Transfer / Internal · Manual GI. Output: GI · picking list · loading note (if ship) · digital file / Document Library |
| 4 | **Photos?** | Damage / missing / broken package · loading · optional video / voice — Evidence Panel → permanent archive |
| 5 | **AI support?** | FIFO/FEFO · reservation · quality · customer reqs · WH rules · location optimize · package integrity · availability · pick route · wrong material/dims/species/moisture/quality/package/lot detect |
| 6 | **Auto-generated?** | GI · inventory txn(s) · histories · audit · scan/validation/override history · genealogy · remaining package qty/volume/weight/status · suggested pick |
| 7 | **Never manual?** | Material Code · Lot · Package · Pallet · WH/Location codes · GI · txn · MI strings · free-hand stock balance edit · **new package number on partial issue** |
| 8 | **User decisions?** | Which demand · approve AI pick **or** Ignore AI + Explorer select · issue qty within package remaining · destination · evidence when needed · **Approve Post** · raise NCR |

---

## Job to be done

> Depocu, **iş belgesine** bağlı talebe karşı doğru paketi / lotu tarar; AI önerisini onaylar veya **Ignore AI Recommendation** ile Warehouse Explorer’dan seçer; **kısmi paket** çıkışında aynı barkodu koruyarak miktarı düşer; kalite ve rezervasyon kapılarını geçer; kanıt ekler; **Post** ile stok ve genealogy güncellenir.

**Not the job:** Create a GoodsIssue row · type inventory down · print a new package barcode for every partial pick.

## CTA

| Locale | Label |
|--------|--------|
| EN | **Issue goods** / Open Goods Issue Workbench |
| TR | **Mal çıkışı** / Mal Çıkış Workbench |

Never: “Yeni çıkış” / “Create Goods Issue.”

Entry: Command Center queues · Operations · Production / Maintenance / Sales deep-link · rugged Terminal into same session · INV-017 library reopen.

---

## Authority references

| Topic | Authority |
|-------|-----------|
| Identifiers | `Document_Numbering.md` |
| Material Identity / Lot (Package ≠ MI) | `Material_Identity_Architecture.md` |
| Genealogy | `Material_Genealogy.md` |
| Stock / reservations / txn immutability | `Inventory_Architecture.md` · `Inventory_Workflow.md` |
| Evidence · Document Library · Export | `Document_Management_Evidence_and_Export.md` |
| Package code immutability / QR | `Barcode_QR_Model.md` · `Barcode_Strategy.md` |
| Screen type | `Screen_Types.md` · `UI_Patterns.md` |
| Warehouse Explorer (browse alternate bins) | Design Program #4 — **consume when landed**; do not redefine Explorer here |
| Demand sources | Production / Maintenance / Sales / Quality process docs (reference only) |

```text
Identifiers are generated automatically according to the centralized
Numbering Architecture (docs/13_Design/99_Shared/Document_Numbering.md).
Manual entry is prohibited. Users work with names and scans; codes are display-only.
Selecting an existing Package / Lot / MI for consumption is allowed.
Minting a new Package Identity is NOT part of partial Goods Issue.
```

---

## Allowed business documents (issue source)

Goods Issue **can only** originate from:

| Source | Typical consumer |
|--------|------------------|
| Production Order | Production issue / backflush companion |
| Maintenance Work Order | Spare / consumable issue |
| Sales Order | Ship / stage for shipment |
| Sample Request | Quality / sales sample |
| Internal Consumption | Cost center / overhead |
| R&D Request | Lab / trial |
| Scrap Request | Controlled scrap issue |
| Warehouse Transfer | Issue leg toward transfer (policy) |
| **Manual Issue** | **Permission-controlled exception only** |

```text
Goods cannot leave inventory without a business document.
```

Manual requires elevated permission + reason code + audit.

---

## Workbench design (not a form)

Operational command center — compose:

| Surface | Role |
|---------|------|
| Cards | Interactive stage content only (not decorative KPI cards) |
| Timeline / stage rail | 10-step progress |
| Warehouse Map | AI route + current bin |
| Material Grid | Demand lines · reserved · available · remaining |
| AI Recommendation Panel | Default pick proposal |
| Evidence Panel | Photos · video · voice · docs |
| Warehouse Explorer | Override browse: WH → Zone → Rack → Shelf → Bin → Package |
| Package Preview | Full package card (see § Package Selection) |
| Document Viewer / Library | Session digital file |
| Sticky Action Bar | Draft · Back · Next · Ignore AI · NCR · **Post** |

Enterprise refs: SAP EWM · Dynamics SCM · Infor WMS · IFS Cloud · Manhattan — adapted to NOS laws (demand-backed · Package Identity · Evidence First · no Create form).

```text
┌──────────────────────────────────────────────────────────────────────────┐
│ INV-ISS-001  Goods Issue Workbench     GI-… (system) · Draft/InProgress  │
│ Ref: PO-… / WO-… / SO-… · Priority · Required date · Customer/Project    │
├────────────┬─────────────────────────────────────┬───────────────────────┤
│ TIMELINE   │ MAIN                                │ CONTEXT               │
│ 1 Document │ Task · Material Grid                │ AI Recommendations    │
│ 2 Materials│ Package Preview · Warehouse Map     │ Validation / Warnings │
│ 3 AI Pick  │ Explorer (override) · Evidence      │ Reservation / Quality │
│ 4 Picking  │ Document Library · Loading          │ Override History      │
│ 5 Verify   │                                     │                       │
│ 6 Evidence │                                     │                       │
│ 7 Quality  │                                     │                       │
│ 8 Loading  │                                     │                       │
│ 9 Review   │                                     │                       │
│ 10 Post    │                                     │                       │
├────────────┴─────────────────────────────────────┴───────────────────────┤
│ STICKY: Save draft · Back · Next · Ignore AI Recommendation · NCR · Post │
└──────────────────────────────────────────────────────────────────────────┘
```

---

## Flow (10 steps)

```text
1 Select business document
2 Load material requirements
3 AI picking recommendation  (+ Override Mode → Warehouse Explorer)
4 Picking / package selection (scan · partial qty)
5 Verify material & package
6 Evidence collection
7 Quality validation
8 Loading / production destination
9 Final review (+ Override History)
10 Posting → Inventory Transaction(s) + history + genealogy + Evidence Archive
```

### 1 — Select business document

Display eligible open demands:

Production Order · Sales Order · Maintenance Order · Transfer Request · Internal Request · Sample · R&D · Scrap · Manual (if permitted).

Show per demand:

| Field |
|-------|
| Owner |
| Priority |
| Required Date |
| Status |
| Customer (when sales) |
| Project (when applicable) |

**Gate:** One valid reference selected (or Manual + permission + reason).

---

### 2 — Load material requirements

Automatically retrieve (no manual material creation):

| Field |
|-------|
| Required Materials |
| Required Quantity |
| Reserved Quantity |
| Available Quantity |
| Alternative Materials |
| Required Package Type |
| Required Quality |
| Required Dimensions |
| Required Moisture |

Name-first display; codes secondary / display-only.

**Gate:** ≥1 open line with remaining qty.

---

### 3 — AI picking recommendation

Automatically recommend inventory using:

FIFO · FEFO · Reservation · Quality Status · Customer Requirements · Warehouse Rules · Location Optimization · Package Integrity · Material Availability.

Display (default option):

| Recommendation |
|----------------|
| Recommended Warehouse |
| Recommended Location |
| Recommended Package |
| Recommended Quantity |
| Recommended Route |

Operator **approves** the recommendation (default path) **or** enters Override Mode.

**Gate:** Each line has an approved pick proposal **or** an audited override selection.

---

### Override Mode

Clearly visible control:

```text
[ Ignore AI Recommendation ]
```

When selected:

1. Open **Warehouse Explorer** (browse — do not redefine Explorer UX; Design Program #4).  
2. Allow navigation:

```text
Warehouse → Zone → Rack → Shelf → Bin → Package
```

3. Operator manually selects another package (still subject to quality / reservation / authorization rules).  
4. **Every override is logged** (actor · timestamp · AI proposal · chosen package · reason if required by policy).

Override does **not** allow typing Package / Location / WH codes. Selection is scan or Explorer pick (name-first).

---

### 4 — Picking / package selection

Navigate along recommended route (or Explorer path). Support: Barcode · QR · RFID · Scanner · Voice (future).

Every scan validated against proposal / override / reservation.

Operator confirms **issue quantity** (may be less than package available — see Partial Package Consumption).

**Gate:** Scans recorded for required identity level; issue qty > 0 and ≤ available (and ≤ reserved when reservation-bound).

---

### 5 — Verify material

Check: Correct material · dimensions · species · moisture · quality · package · lot · qty · reservation · expiry.

Detect and **block until resolved** (or authorized waiver): Wrong material / dims / species / moisture / quality / package / lot · Blocked · Quarantine · Inspection hold · Expired · Mixed lots (policy).

**Gate:** All lines green or supervisor waiver (audited).

---

### 6 — Evidence collection

Every Goods Issue shall preserve (when captured), permanently linked to the GI transaction:

Photos · Videos · Voice Notes · Issue Documents · PDF · Excel · Operator Notes · Damage Photos · Loading Photos.

**Gate:** Required when exception/damage/loading policy says so; else optional.

Surfaces: Evidence Panel · Photo Gallery · Document Viewer — capabilities per Shared Document Management law (do not redefine).

---

### 7 — Quality validation

Validate automatically:

| Check |
|-------|
| Wrong Material |
| Wrong Dimensions |
| Wrong Species |
| Wrong Moisture |
| Wrong Quality |
| Wrong Package |
| Wrong Lot |
| Blocked Inventory |
| Quarantine |
| Inspection Hold |
| Expired Material |

Operator may override **package selection** (Override Mode) but **cannot violate business rules** without authorization (Quality / Supervisor release).

**Gate:** Quality clear for all issue lines (or authorized release recorded).

---

### 8 — Loading / destination

| If | Assign |
|----|--------|
| **Sales / shipment** | Vehicle · Loading Dock · Carrier · Shipment |
| **Production** | Production Line · Work Center · Operation |

Name-first pickers; no code typing. Optional loading photos → Evidence Panel.

**Gate:** Destination required by issue type policy.

---

### 9 — Final review

Display:

| Block |
|-------|
| Business Document |
| Requested Materials |
| Selected Packages (with issued / remaining) |
| Warehouse · Locations |
| Inventory Changes |
| Evidence |
| Warnings |
| AI Recommendations |
| **Override History** |

Operator confirms.

**Gate:** Explicit approve for Post.

---

### 10 — Posting

**Post** (not Save) automatically creates:

| Artifact |
|----------|
| Goods Issue document (Posted) — number via Numbering |
| Inventory Transaction(s) (immutable) — balance ↓ |
| Package quantity / volume / weight / status update (same Package ID) |
| Material movement · Warehouse history · Material history |
| Audit trail · Scan / validation / approval / **override** history |
| Evidence Archive + Document Library seal |
| Reservation clear (when applicable) |
| Genealogy update (MI consumed / linked to demand / FG path as rules allow) |

Nothing edited as a raw on-hand field outside the transaction. Reverse only via compensating transaction.

---

## Package Selection

Display complete package information in **Package Preview**:

| Field |
|-------|
| Package Number (display-only) |
| Material Identity |
| Species |
| Dimensions |
| Quality Grade |
| Moisture |
| Length · Width · Thickness |
| Available Pieces |
| Available Volume |
| Weight |
| Production Date |
| Current Warehouse |
| Current Location |
| Reservation Status |
| Customer Reservation |
| Photos |

Selection via scan of existing barcode/QR or Explorer pick — never typed Package Number.

---

## Partial Package Consumption

Warehouse operators **may consume only part of a package**.

### Example

```text
Package          PKG-00254
Contains         120 Pieces · 5.240 m³
Sales Order asks 40 Pieces

Operator selects 40 Pieces
```

### Absolute behavior

```text
The system SHALL NOT generate a new package number.
The system SHALL NOT print a new barcode / QR for the remainder.
The original barcode remains attached to the physical package.
```

Instead, update on the **same** Package Identity:

| Field | Example |
|-------|---------|
| Original Quantity | 120 |
| Issued (this GI) | 40 |
| Remaining Quantity | 80 |
| Remaining Volume | (recalculated) |
| Remaining Weight | (recalculated) |
| Package Status | → Partially Used (see statuses) |

### Inventory Transactions

Partial issue **shall** create Inventory Transaction(s):

```text
PKG-00254
    ↓
Goods Issue (40 Pieces)
    ↓
Production Order / Sales Order / …
    ↓
Inventory Transaction
    ↓
Audit Log
```

History is always preserved. Multiple Goods Issues may consume the same package over time until remaining reaches zero / Consumed / Closed.

### When is a new Package minted?

**Not** on ordinary partial issue.  
Only when a **physical handling split** policy explicitly requires a new handling unit (rare; Numbering Service mints a **new** Package; parent links via genealogy / package contents rules — never by renaming the original). Default wood-yard rule: **one physical package = one immutable barcode until fully consumed.**

---

## Package Identity

| Property | Rule |
|----------|------|
| Package Identity | Never changes |
| Package Barcode | Never changes |
| Package QR | Never changes |
| Lifetime | While physical package exists |
| May change | Available Quantity · Available Volume · Available Weight · Remaining Pieces · **Status** |

### Package Status

| Status | Meaning |
|--------|---------|
| Available | Pickable |
| Reserved | Bound to demand |
| In Picking | Active GI session |
| Partially Used | Remaining > 0 after one or more issues |
| Consumed | Remaining = 0 (fully issued) |
| Closed | Business-closed (policy) |

Package ≠ Material Identity. Package **links to** MI (and may contain / represent MI units). See `Material_Identity_Architecture.md`.

---

## Traceability

The system shall always reconstruct:

| Question | Answered by |
|----------|-------------|
| Which Receiving created this package / MI? | Receiving root + txn history |
| Which Supplier delivered it? | Receiving / PO link |
| Which Production Order consumed it? | GI → demand reference |
| Which Finished Product contains it? | Genealogy |
| Which Customer received it? | SO / shipment chain |

```text
Receiving root MI → Lot / Package → Reservation (if any) → Goods Issue txn(s) →
Production Order / WO / SO → (later) FG / Customer shipment
```

```text
Traceability relies on Inventory Transactions and Material Genealogy.
NOT by changing Package IDs.
```

---

## Document Library & Evidence Archive

Every Goods Issue includes (surfaces on Workbench + INV-017 detail):

Document Library · Photo Gallery · Evidence Viewer · AI Analysis · OCR Results · Audit History · Timeline.

Support (by Shared law — do not redefine): Preview · Download · ZIP · Print · Search · Version History.

**Authority:** [`Document_Management_Evidence_and_Export.md`](../../13_Design/99_Shared/Document_Management_Evidence_and_Export.md).

---

## Export

Support Excel · CSV · PDF.

Generate from library / reports (formats & columns → Shared law):

| Export |
|--------|
| Goods Issue Report |
| Picking List |
| Inventory Movement Report |
| Material Consumption Report |
| Transaction History |
| Difference Report |

---

## Override History

Append-only panel (Context + Final Review), sealed into audit on Post:

| Field |
|-------|
| When |
| Who |
| AI recommendation (WH · location · package · qty) |
| Operator selection |
| Reason (if policy requires) |
| Authorization (if waiver) |

---

## Gates summary

| Stage | Gate |
|-------|------|
| 1 Document | Valid demand (or Manual + permission + reason) |
| 2 Materials | ≥1 open line with remaining qty |
| 3 AI / Override | Approved proposal **or** logged Explorer override |
| 4 Picking | Scan OK · issue qty ≤ available (and reservation policy) |
| 5 Verify | All checks green or audited waiver |
| 6 Evidence | Policy-required evidence present |
| 7 Quality | No block / hold / quarantine / expired without release |
| 8 Destination | Required destination assigned |
| 9 Review | Explicit approve · Override History visible |
| 10 Post | Creates txn(s) · updates package remaining · archive |

---

## AI features (summary)

Recommend best package/location · FIFO/FEFO · Detect wrong material/dims/species/moisture/quality/package/lot/qty/mixed · Predict shortage · Suggest pick route · Detect repeated operator mistakes · Coach / supervisor alert on frequent overrides.

AI **prepares**; operator **approves** or **Ignore AI Recommendation** (logged).

---

## Identity & Numbering

Operator **never** enters:

Goods Issue Number · Inventory Transaction Number · Warehouse Code · Location Code · Package Number · Material Code · Lot Number · Material Identity strings.

All identifiers: Numbering Architecture.  
Existing Package / Lot / MI: **scan or pick only**.  
Partial issue: **no new Package number**.

---

## Roles

| Role | Focus |
|------|--------|
| Warehouse Operator | Scan · approve AI or Override · partial qty · evidence · Post |
| Supervisor | Authorize overrides / waivers · unblock |
| Inventory Controller | Reservation / shortage · Manual GI permission |
| Quality | Holds / release (blocks issue until clear) |

---

## Mobile / Terminal

Rugged Terminal: steps 4–5 (pick/verify/partial qty) inside the same GI session.  
Tablet / desk: document select · AI/Override · review · Document Library · Post.

---

## Cursor implementation notes

1. Screen type = **Workbench** — not Issue Create form.  
2. FE must support: AI default · **Ignore AI Recommendation** → Explorer · Package Preview · **partial qty on same PKG** · Override History · Document Library surfaces.  
3. Post → inventory txn + **package remaining update (same ID)** + reservation clear + evidence archive + genealogy.  
4. Never mint/print new package barcode on partial consume.  
5. Command Center “Open issues” opens this Workbench.  
6. Manual GI = permission + reason — audited.  
7. Export / permanence algorithms → Shared Document Management — reference only.

---

## Related

`INV_Issue_Wizard.md` · `Inventory_Workflow.md` · FLOW-INV-002 · `Inventory_Screens.md`  
`Inventory_Design_Program.md` · `Material_Identity_Architecture.md` · `Document_Numbering.md`  
`Document_Management_Evidence_and_Export.md` · `Inventory_Dashboard.md` · `Barcode_QR_Model.md`

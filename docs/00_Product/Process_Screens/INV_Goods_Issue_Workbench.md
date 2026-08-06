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
| **2.0.1** | **Accept / Override Recommendation** CTAs · AI Validation continues in Override · Partial pick auto-updates remaining · **optional company-policy package split** with full traceability |

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
Default partial pick: same Package Identity / Barcode / QR remain;
system updates Remaining Quantity · Volume · Weight · Pieces · Status.
Optional: if company policy requires automatic package split,
Numbering mints child package(s) while preserving complete traceability
(parent → child · Inventory Transactions · Genealogy).
Never silently rename or reuse a barcode.
Traceability = Inventory Transactions + Material Genealogy —
never by overwriting Package IDs.
```

```text
AI VALIDATION LAW
─────────────────────────────────────────────────────────────
Even in Override Mode, AI continues validating business rules.
Operator may override location / package selection.
Operator may NOT violate validation rules without explicit authorization.
```

**Forbidden:** Bare Save form · typed Material / Lot / Package / Pallet / WH / Location / GI / txn / MI codes · orphan issue · reusing package barcodes · bypassing AI validation without authorization  
**Required:** Business document → load demand → AI pick (default) → **Accept** or **Override** → scan / verify (AI validation always on) → evidence → quality → destination → review (incl. overrides) → **Post**

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
| 8 | **User decisions?** | Which demand · **Accept Recommendation** or **Override Recommendation** (Explorer) · issue qty within package remaining · destination · evidence when needed · **Approve Post** · raise NCR |

---

## Job to be done

> Depocu, **iş belgesine** bağlı talebe karşı doğru paketi / lotu tarar; AI önerisini **Accept** eder veya **Override** ile Warehouse Explorer’dan seçer; **kısmi paket** çıkışında kalan miktarı sistem günceller (politika varsa otomatik split + izlenebilirlik); Override’da bile AI validation çalışır; **Post** ile stok ve genealogy güncellenir.

**Not the job:** Create a GoodsIssue row · type inventory down · bypass validation · invent package codes.

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
│ STICKY: Draft · Back · Next · Accept Recommendation · Override · NCR · Post │
└──────────────────────────────────────────────────────────────────────────┘
```

---

## Flow (10 steps)

```text
1 Select business document
2 Load material requirements
3 AI picking recommendation → Accept Recommendation  OR  Override Recommendation
4 Picking / package selection (scan · partial qty)
5 Verify material & package  (AI Validation — also in Override)
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

The system **shall automatically recommend** inventory according to:

| Driver |
|--------|
| FIFO |
| FEFO |
| Reservation |
| Quality Status |
| Warehouse Rules |
| Location Optimization |
| Package Integrity |
| Customer Requirements |

The recommendation is the **default option**.

Display:

| Recommendation |
|----------------|
| Recommended Warehouse |
| Recommended Location |
| Recommended Package |
| Recommended Quantity |
| Recommended Route |

Operator chooses exactly one path:

| Control | Result |
|---------|--------|
| **✓ Accept Recommendation** | Proceed with AI default (scan/confirm along recommended route) |
| **✓ Override Recommendation** | Enter Override Mode (Warehouse Explorer) |

**Gate:** Each line has **Accept** recorded **or** an audited **Override** selection.

---

### Override Mode

When **Override Recommendation** is selected:

1. Open **Warehouse Explorer** (browse — do not redefine Explorer UX; Design Program #4).  
2. Operator browses inventory:

```text
Warehouse
    ↓
Zone
    ↓
Rack
    ↓
Shelf
    ↓
Bin
    ↓
Package
```

3. Operator may manually choose another package (scan or Explorer pick — name-first; **no typed codes**).  
4. **Every override shall be logged** in audit history (actor · timestamp · AI proposal · chosen WH/location/package · reason if policy requires).

**AI Validation continues in Override Mode** (see § AI Validation).  
Override changes **selection**, not the right to break business rules.

---

### 4 — Picking / package selection

Navigate along recommended route (or Explorer path). Support: Barcode · QR · RFID · Scanner · Voice (future).

Every scan runs through **AI Validation** against demand + proposal/override + reservation.

Operator confirms **issue quantity** (may be less than package available — see Partial Package Picking).

**Gate:** Scans recorded for required identity level; issue qty > 0 and ≤ available (and ≤ reserved when reservation-bound); AI Validation clear or authorized waiver.

---

### 5 — Verify material

Operator review surface for AI Validation results (Accept and Override paths).

**Gate:** All lines green or explicitly authorized waiver (audited).

---

### 6 — Evidence collection

Every Goods Issue shall preserve (when captured), permanently linked to the GI transaction:

Photos · Videos · Voice Notes · Issue Documents · PDF · Excel · Operator Notes · Damage Photos · Loading Photos.

**Gate:** Required when exception/damage/loading policy says so; else optional.

Surfaces: Evidence Panel · Photo Gallery · Document Viewer — capabilities per Shared Document Management law (do not redefine).

---

### 7 — Quality validation

Hard quality state checks (Blocked · Quarantine · Inspection Hold · Expired · Rejected) plus AI Validation rules.

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

## Partial Package Picking

Warehouse operators **shall be able to consume only part of a package**.

### Example

```text
Package
120 Pieces
    ↓
Issue
40 Pieces
    ↓
Remaining
80 Pieces
```

### Default behavior (same physical package)

The system **shall automatically update** on the **same** Package Identity:

| Field |
|-------|
| Remaining Quantity |
| Remaining Volume |
| Remaining Weight |
| Remaining Pieces |
| Package Status |

```text
Original barcode / QR stays on the physical package.
No new barcode is printed for the remainder (default policy).
```

| Field | Example |
|-------|---------|
| Original Quantity | 120 |
| Issued (this GI) | 40 |
| Remaining Pieces | 80 |
| Package Status | → Partially Used |

### Optional: company-policy package split

```text
If company policy requires,
the system may automatically split the package
while preserving complete traceability.
```

| Rule | Detail |
|------|--------|
| When | Configured company / plant policy only — not operator whim |
| How | Numbering Service mints **child** Package Identity for the remainder and/or issued handling unit (policy decides which side keeps the original barcode) |
| Traceability | Parent ↔ child package link · Inventory Transactions · Material Genealogy · audit — **complete chain** |
| Forbidden | Silent rename · barcode reuse · orphan remainder without link |

Default wood-yard policy: **no split** — one physical package keeps one barcode until fully consumed.  
Split policy is an explicit configuration, not the Workbench default.

### Inventory Transactions

Partial issue **shall** create Inventory Transaction(s):

```text
PKG-… (source)
    ↓
Goods Issue (40 Pieces)
    ↓
Production Order / Sales Order / …
    ↓
Inventory Transaction
    ↓
Audit Log
(+ child PKG link if policy split)
```

History is always preserved. Multiple Goods Issues may consume the same package over time until remaining reaches zero / Consumed / Closed (unless split moved remainder to a child package).

---

## AI Validation

**Even in Override Mode**, AI **shall continue validating**:

| Check |
|-------|
| Wrong Material |
| Wrong Dimension |
| Wrong Species |
| Wrong Quality |
| Wrong Customer Specification |
| Wrong Reservation |
| Wrong Lot |
| Wrong Moisture Class |
| Wrong Package |

```text
The operator may override location / package selection,
but may not violate business validation rules
unless explicitly authorized.
```

| Path | AI Validation |
|------|----------------|
| Accept Recommendation | On — confirms scan matches recommendation + demand |
| Override Recommendation | **Still on** — validates manually chosen package against demand / quality / reservation / customer spec |

Authorization to waive a validation = Supervisor / Quality / policy role · always audited · visible in Override History.

---

## Package Identity

| Property | Rule |
|----------|------|
| Package Identity | Immutable for that physical unit; child packages only via **policy split** (new ID, linked) |
| Package Barcode / QR | Never reused; default partial pick keeps original on physical package |
| Lifetime | While physical package exists |
| May change without new ID | Remaining Quantity · Volume · Weight · Pieces · **Status** |

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
| 3 AI | **Accept Recommendation** **or** logged **Override Recommendation** |
| 4 Picking | Scan OK · issue qty ≤ available · AI Validation clear (or authorized) |
| 5 Verify | AI Validation results green or audited waiver |
| 6 Evidence | Policy-required evidence present |
| 7 Quality | No block / hold / quarantine / expired without release |
| 8 Destination | Required destination assigned |
| 9 Review | Explicit approve · Override History visible |
| 10 Post | Creates txn(s) · updates package remaining · archive |

---

## AI features (summary)

Recommend (FIFO/FEFO/reservation/quality/WH rules/location/package integrity/customer) · **Accept** or **Override** · AI Validation always on (incl. Override) · shortage predict · pick route · repeated-override coaching.

AI **recommends and validates**; operator **Accepts** or **Overrides selection** — not business rules.

---

## Identity & Numbering

Operator **never** enters:

Goods Issue Number · Inventory Transaction Number · Warehouse Code · Location Code · Package Number · Material Code · Lot Number · Material Identity strings.

All identifiers: Numbering Architecture.  
Existing Package / Lot / MI: **scan or pick only**.  
Partial pick default: **same Package ID**; policy split: Numbering mints **linked child** package(s) only.

---

## Roles

| Role | Focus |
|------|--------|
| Warehouse Operator | Scan · **Accept** or **Override** · partial qty · evidence · Post |
| Supervisor | Authorize validation waivers · unblock |
| Inventory Controller | Reservation / shortage · Manual GI · package-split policy config (with admin) |
| Quality | Holds / release (blocks issue until clear) |

---

## Mobile / Terminal

Rugged Terminal: steps 4–5 (pick/verify/partial qty) inside the same GI session.  
Tablet / desk: document select · Accept/Override · review · Document Library · Post.

---

## Cursor implementation notes

1. Screen type = **Workbench** — not Issue Create form.  
2. FE CTAs: **Accept Recommendation** · **Override Recommendation** → Explorer.  
3. AI Validation runs on both paths (material/dims/species/quality/customer/reservation/lot/moisture/package).  
4. Partial pick: auto-update remaining qty/volume/weight/pieces/status; optional policy split with parent–child traceability.  
5. Post → inventory txn + package update (+ child PKG if policy) + reservation clear + evidence + genealogy.  
6. Command Center “Open issues” opens this Workbench.  
7. Manual GI = permission + reason — audited.  
8. Export / permanence → Shared Document Management — reference only.

---

## Related

`INV_Issue_Wizard.md` · `Inventory_Workflow.md` · FLOW-INV-002 · `Inventory_Screens.md`  
`Inventory_Design_Program.md` · `Material_Identity_Architecture.md` · `Document_Numbering.md`  
`Document_Management_Evidence_and_Export.md` · `Inventory_Dashboard.md` · `Barcode_QR_Model.md`

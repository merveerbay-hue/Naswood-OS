# Document Numbering

**Module:** Shared

**Category:** Numbering Strategy

**Version:** 1.1  
**Status:** Approved  

---

# Purpose

The Document Numbering standard defines how unique business document numbers
**and all system identifiers (codes)** are generated, formatted and managed
across Naswood OS.

A centralized numbering engine ensures consistency, uniqueness, traceability and compliance throughout the platform.

All business documents **and master / physical identifiers** must receive their
codes from the shared Numbering Engine — never from user-typed Code fields.

---

# Objectives

- Unique Document Identification
- Consistent Numbering
- Business Traceability
- Configurable Numbering Rules
- Multi-Company Support
- Regulatory Compliance

---

# Design Principles

Document numbers should be

Unique

Readable

Predictable

Configurable

Immutable

Sequential where required

Document numbers are business identifiers and are independent from internal UUIDs.

---

# Numbering Architecture

```
UUID

↓

Business Document Number

↓

Barcode

↓

QR Code

↓

Printed Document
```

---

# Numbering Engine

The Numbering Engine is responsible for

Sequence Generation

Prefix Management

Validation

Reservation

Duplicate Prevention

Reset Rules

Audit Logging

---

# Supported Documents

Purchase Request

Purchase Order

Sales Quotation

Sales Order

Invoice

Goods Receipt

Goods Issue

Inventory Adjustment

Production Order

Production Batch

Quality Inspection

Maintenance Work Order

Expense Claim

Shipment

Transfer Order

Document Revision

Project

Contract

Customer

Supplier

Employee

Asset

---

# Standard Format

```
PREFIX-YEAR-SEQUENCE
```

Example

```
PO-2026-000245
```

---

# Extended Format

```
COMPANY-PLANT-PREFIX-YEAR-MONTH-SEQUENCE
```

Example

```
NAS-BUC-PO-2026-08-000245
```

---

# Prefix Standards

Purchase Request

PR

Purchase Order

PO

Sales Quotation

SQ

Sales Order

SO

Goods Receipt

GR

Goods Issue

GI

Inventory Adjustment

IA

Production Order

PRO

Production Batch

BAT

Quality Inspection

QI

Maintenance Work Order

MWO

Invoice

INV

Shipment

SHP

Transfer Order

TRF

Supplier

SUP

Customer

CUS

Employee

EMP

Asset

AST

Project

PRJ

---

# Sequence Rules

Supports

Global Sequence

Module Sequence

Company Sequence

Plant Sequence

Warehouse Sequence

Project Sequence

Custom Sequence

---

# Reset Policies

Never Reset

Yearly

Monthly

Quarterly

Daily

Custom

Reset policy is configurable by document type.

---

# Reservation

Supports

Temporary Reservation

Automatic Release

Expiration

Reuse Prevention

---

# Number Generation

Supports

Automatic

Manual Override (Permission Required) — **business documents only** (see below)

External Integration

API

Import

---

# System Generated Identifiers (Constitution-level UX law)

**Constitution pointer:** `AI/NOS_CONSTITUTION/04_PRODUCT_ARCHITECT.md` § 2.3  
**This section is authoritative** for which IDs are auto-minted and how UI presents them.

```text
Business users shall never manually create or edit system identifiers.

All identifiers including codes, document numbers, lot numbers, serial numbers,
warehouse codes, package IDs, pallet IDs, machine codes and transaction numbers
shall be generated exclusively by the NOS Numbering Service.

Data entry forms shall capture business information only.

Technical identifiers are assigned automatically during creation or release,
depending on the business process.

Users work with names — not codes. Codes are display-only when shown.
```

## What users enter vs what the system assigns

| Object | User enters (business) | System assigns (never typed) | When minted |
|--------|------------------------|------------------------------|-------------|
| Material | Ad · Tip · Grup · Ağaç türü · Ölçüler · Birim · Capability · … | `MAT-…` | On save / first persist |
| Warehouse | Depo adı · Tip · Fabrika · Sorumlu · … | `WH-…` | On save |
| Location | Ad · Zone · … | `LOC-…` (policy) | On save |
| Machine | Makine adı · Üretici · Model · … (Configuration facets) | `MC-…` | On save / Release |
| Production Order | Ürün (by **name**) · qty · dates · … | `PO-2026-…` | On **Release** (draft may show “atanacak”) |
| Goods Receipt | PO · qty · Depo · … | `GR-…` | On Post / save per policy |
| Lot / Batch | *(nothing — category drives series)* | `LOT-…` | On GR line / process mint |
| Serial | *(nothing)* | `SN-…` | On mint |
| Package | business pack attrs | Package ID | On mint |
| Pallet | business pallet attrs | Pallet ID | On mint |
| Sales / Purchase Order | müşteri/tedarikçi (**name**) · lines | `SO-…` / `PO-…` (purchasing) | On Release / policy |

## UI presentation (mandatory)

### Forbidden

```text
❌  Code *
    _______________

❌  Warehouse Code
    _______________

❌  Production Order No
    _______________

❌  Lot No
    _______________

❌  Product Code   PRD-001245   (as the primary picker / typed field)
```

### Required

```text
✅  System Code / Identifier
    Automatically generated after save
    — or —
    Generated automatically according to numbering rules
    — or —
    (Release edilince atanır)   for documents minted on Release

✅  After persist: show read-only  MAT-000012548 · WH-0008 · PO-2026-000145
```

Corner badge / header may show the code **as information only** once known; never as an editable control.

## Name-first selection (mandatory)

Users select business objects by **human name / description**, not by typing codes.

| Wrong | Right |
|-------|--------|
| Product Code → `PRD-001245` | **Ürün** 🔍 `Thermowood Deck 26×140×3000` |
| Material Code → type `MAT-…` | **Malzeme** 🔍 name · species · dimension |
| Warehouse Code → type `WH-…` | **Depo** 🔍 `Ana Depo` |

The system stores and uses `ProductId`, `RevisionId`, `MaterialId`, etc. internally.  
Users do not see or enter those IDs during data entry. Codes may appear in Library grids as secondary columns for search/trace — pickers remain name-first.

## Scope of “manual entry prohibited”

Applies to **all** system identifiers listed above (and series configured in Numbering).  
Selecting an **existing** Lot/Serial/Package for consumption/issue is allowed.  
**Minting** a new identity is Numbering Service only — no fallback to free-text.

Admin configuration of **numbering series** (prefixes, pads) is Settings — not end-user forms.

---

# Material & Production Identifiers (authoritative)

This section is the **single source of truth** for physical and production
identity numbers. Other documents (Screens, User Flows, Workflows, TASKs)
must **reference** this section — they must not restate or weaken it.

```text
Material, Lot, Serial, Package, Pallet, Warehouse, Machine, and Production
identifiers are generated exclusively by the NOS Numbering Service as defined
in this Core Identity & Numbering Architecture
(docs/13_Design/99_Shared/Document_Numbering.md).

Manual entry is prohibited. Users work with names; codes are display-only.
```

Applies to (non-exhaustive):

- Material / Product catalog codes  
- Warehouse / Location codes  
- Machine / Work Center / Line codes (where numbered)  
- Lot / Batch numbers  
- Serial numbers  
- Package numbers  
- Pallet numbers  
- Production Order / Work Order / Operation execution numbers (business IDs)  
- Inventory / Purchasing / Sales transaction document numbers

UI and APIs may **display** these identifiers. They must not accept user-typed
values for creation. Selection of an *existing* Lot/Serial for consumption is
allowed; minting a new identity is Numbering Service only.

Business documents mint on **create or Release** per process (e.g. Production Order number on Release). Draft UI shows “Numara sistem tarafından atanacaktır” — never an empty Code input.

### Lot / Batch series by material category (authoritative)

When a **new Lot** (or Batch) is minted — e.g. during **Goods Receipt / Receiving Wizard** —
the Numbering Service selects the series from the material’s **category / type**
(and company · plant). The user does **not** type or pick a free-form lot number.

```text
Material.Category (or MaterialType / numbering class)
        →  Numbering series (prefix + sequence scope)
        →  Lot / Batch ID issued automatically
```

Examples (configurable per plant — not hard-coded in UI):

| Material category (example) | Series prefix (example) | Sample Lot ID |
|----------------------------|-------------------------|---------------|
| Raw / Tomruk               | LOT-RAW                 | LOT-RAW-2026-000118 |
| WIP / Lamelle              | LOT-WIP                 | LOT-WIP-2026-000042 |
| Finished / Profil          | LOT-FG                  | LOT-FG-2026-000077 |
| Chemical / Consumable      | LOT-CHM                 | LOT-CHM-2026-000009 |

Rules:

1. Series key = `Company + Plant + MaterialNumberingClass` (class derived from Material Category / Type).  
2. If class has no series configured → block mint; Admin must configure Numbering (do not fall back to manual entry).  
3. Goods Receipt **document** number remains `GR-…` (document series). Lot IDs use the material-class series above.  
4. Receiving UI shows the **proposed** Lot ID (read-only) after material is known; regenerate only if material line changes before Post.  
5. Screens/Flows must **reference** this section — they must not invent alternate lot formats.

Authority matrix: `docs/00_Product/DOCUMENTATION_AUTHORITY_MATRIX.md`.

---

# Validation Rules

Document Number is required.

Document Number must be unique.

Prefix must match document type.

Manual numbering requires authorization — **legacy / exception path only** for rare legal overrides of **business documents**.  
It does **not** apply to Material, Warehouse, Machine, Lot, Serial, Package, Pallet, or Production Order codes — those are **never** manually entered (see § System Generated Identifiers).

Material / Lot / Serial / Package / Pallet / Warehouse / Machine / Production identifiers: **no manual entry**.
Name-first pickers; codes display-only.

---

# Revision Handling

Revision numbers use

```
PO-2026-000245-R01
```

or

```
DOC-2026-001250-V03
```

Previous revisions remain accessible.

---

# Cancellation

Cancelled document numbers are never reused.

Status changes do not affect numbering.

---

# Multi-Company Support

Supports

Company Prefix

Business Unit Prefix

Plant Prefix

Country Prefix

Independent Sequences

---

# Localization

Supports

Country-specific numbering

Fiscal requirements

Legal formatting

Language-independent identifiers

---

# Barcode Integration

Document numbers may be encoded into

Barcode

QR Code

DataMatrix

Reference

Barcode_Strategy.md

---

# Printing

Displayed on

PDF

Labels

Reports

Invoices

Packing Lists

Certificates

Reference

Print.md

---

# Audit

Track

Number Generation

Manual Override

Reservation

Cancellation

Sequence Changes

Configuration Updates

Reference

Audit_Log.md

---

# Security

Supports

Role-Based Permissions

Approval for Manual Numbering

Immutable History

Sequence Locking

---

# API

Example Endpoints

```
GET /numbering/sequences

POST /numbering/generate

POST /numbering/reserve

POST /numbering/release

GET /numbering/history
```

---

# User Interface

Displays

Generated Number

Sequence

Prefix

Revision

Status

Generation Date

---

# Example Documents

Purchase Order

PO-2026-000245

Sales Order

SO-2026-000128

Production Order

PRO-2026-001542

Quality Inspection

QI-2026-000452

Shipment

SHP-2026-000087

---

# Performance

Supports

Sequence Caching

Distributed Locking

High-Concurrency Generation

Failover Recovery

Background Reservation Cleanup

---

# Best Practices

✓ Use centralized numbering.

✓ Keep business identifiers immutable.

✓ Never reuse document numbers.

✓ Separate UUIDs from business numbers.

✓ Use meaningful prefixes.

✓ Audit numbering changes.

---

# Do

✓ Generate numbers automatically

✓ Validate uniqueness

✓ Preserve cancelled numbers

✓ Support revisions

✓ Maintain audit history

---

# Don't

✗ Reuse document numbers

✗ Modify issued identifiers

✗ Hardcode prefixes

✗ Generate duplicate sequences

✗ Allow unauthorized overrides

---

# Acceptance Criteria

Document numbers are unique.

Numbering follows configured rules.

Business identifiers remain immutable.

Revision history is maintained.

Audit logging is enabled.

Performance supports concurrent generation.

---

# Related Documents

Barcode_Strategy.md

Audit_Log.md

API_Standards.md

Architecture.md

Approval_Workflow.md

Print.md

Labels.md

Security.md

Material.md

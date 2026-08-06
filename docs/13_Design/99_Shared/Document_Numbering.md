# Document Numbering

**Module:** Shared

**Category:** Numbering Strategy

**Version:** 1.0

**Status:** Approved

---

# Purpose

The Document Numbering standard defines how unique business document numbers are generated, formatted and managed across all modules within Naswood OS.

A centralized numbering engine ensures consistency, uniqueness, traceability and compliance throughout the platform.

All business documents must receive their identifiers from the shared Numbering Engine.

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

# Material & Production Identifiers (authoritative)

This section is the **single source of truth** for physical and production
identity numbers. Other documents (Screens, User Flows, Workflows, TASKs)
must **reference** this section — they must not restate or weaken it.

```text
Material, Lot, Serial, Package, Pallet and Production identifiers
are generated exclusively by the NOS Numbering Service as defined
in this Core Identity & Numbering Architecture
(docs/13_Design/99_Shared/Document_Numbering.md).

Manual entry is prohibited.
```

Applies to (non-exhaustive):

- Material / Product instance identifiers where numbered
- Lot / Batch numbers
- Serial numbers
- Package numbers
- Pallet numbers
- Production Order / Work Order / Operation execution numbers (business IDs)

UI and APIs may **display** these identifiers. They must not accept user-typed
values for creation. Selection of an *existing* Lot/Serial for consumption is
allowed; minting a new identity is Numbering Service only.

Business documents (PO, SO, GR, Invoice, …) remain under the general rules above
(automatic by default; manual override only with explicit permission).

Authority matrix: `docs/00_Product/DOCUMENTATION_AUTHORITY_MATRIX.md`.

---

# Validation Rules

Document Number is required.

Document Number must be unique.

Prefix must match document type.

Manual numbering requires authorization (business documents only).

Material / Lot / Serial / Package / Pallet / Production identifiers: **no manual entry**.

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

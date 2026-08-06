# Inventory Canonicalization Candidate

**Module:** Inventory

**Category:** Architecture Governance

**Version:** 1.0

**Status:** Candidate (Not Canonical)

---

# Purpose

Inventory is the next domain planned for Canonical freeze after Product
Management. This document records the gates that must close before Inventory
may be declared Canonical.

Inventory is **not** Canonical today. Implementation of Inventory must not
assume a frozen contract beyond the stable Product and negative-stock
invariants already approved.

---

# Current Status

| Attribute | Value |
|---|---|
| Domain Status | CANDIDATE |
| Breaking Changes | Not yet governed by Canonical freeze |
| Product Dependency | Product is Canonical; Inventory consumes Product contracts |
| Negative Stock | Constitutionally prohibited without exceptions |

---

# Ownership Already Approved

| Concern | Owner |
|---|---|
| Material Master | Inventory |
| Physical Material identity | Inventory |
| Physical quantity and stock state | Inventory |
| Inventory Ledger / stock movements | Inventory |
| Reservation | Inventory |
| Genealogy | Manufacturing |
| Product identity and capabilities | Product Management |

---

# Canonicalization Gates

Inventory may be declared Canonical only when all gates below are Closed.

## Gate 1 — Warehouse Hierarchy

Approve one warehouse and location hierarchy model:

- Plant → Warehouse → Zone → Bin (or approved alternative)
- Location identity rules
- Multi-warehouse transfer semantics
- Location status vs operational availability

## Gate 2 — Batch and Lot Taxonomy

Approve one physical identity taxonomy:

- Batch / Lot / Serial boundaries
- Relationship to Material and Product revision
- Recipe/batch taxonomy where manufacturing requires it
- Traceability identifiers used by Quality and Genealogy consumers

## Gate 3 — Stock Status Separation

Separate and freeze:

- Quantity states (On Hand, Reserved, Available, In Transit, Quarantine)
- Quality / hold statuses
- Accounting / valuation readiness (without inventing costing methods)
- Posting eligibility rules that never allow negative On Hand

## Gate 4 — Logistics Package Boundary

Approve whether Logistics owns shipment/receipt execution packages or Inventory
owns them as inventory documents with Logistics as a consumer/projection.

No duplicate shipment/receipt masters may remain after this gate.

## Gate 5 — Contract Alignment

Align and freeze:

- Inventory API DTOs under `/api/v1`
- Inventory ledger event names and payloads
- Reservation contracts
- Goods Receipt / Goods Issue / Transfer / Adjustment / Count commands
- Outbox publication and inbox consumption for Purchasing, Production, Sales,
  Quality and Finance reactions

## Gate 6 — Documentation Conformance

Every Inventory Domain, Design and Sprint_01 task document must:

- Match approved ownership
- Reference Product as business identity only
- Never auto-create Material from Product release
- Prohibit negative On Hand without exception configuration
- Remove duplicate reservation, material or stock models

---

# Explicit Non-Goals for This Candidate

The following remain business decisions and shall not be invented during
canonicalization:

- Costing and valuation methods
- Finance posting/reversal/period-lock details
- Approval matrices
- Recipe/batch taxonomy specifics still queued for business approval
- Retention, RPO and RTO

---

# Freeze Criteria

When all gates are Closed, Architecture shall publish an ADR declaring:

```
STATUS: CANONICAL
BREAKING CHANGES: FORBIDDEN
ADDITIVE EXTENSIONS: ALLOWED
BEHAVIOR CHANGES: ADR REQUIRED
SCHEMA CHANGES: ADR REQUIRED
```

Until that ADR exists, Inventory remains a Candidate domain.

---

# Related Documents

- `AI/NOS_CONSTITUTION/03_PLATFORM.md`
- `docs/00_Project_Governance/Architecture_Decisions.md` (ADR-009, ADR-011, ADR-013)
- `docs/13_Design/02_Inventory/Inventory_Architecture.md`
- `docs/13_Design/02_Inventory/Inventory_Ledger.md`
- `docs/13_Design/02_Inventory/Material_Master.md`
- `docs/13_Design/02_Inventory/Reservation.md`
- `docs/13_Design/99_Shared/Negative_Stock.md`

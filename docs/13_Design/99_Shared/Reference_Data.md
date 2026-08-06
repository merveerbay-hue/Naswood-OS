# Reference Data

**Module:** Shared

**Category:** Reference Data Management

**Version:** 1.0

**Status:** Approved

---

# Purpose

The Reference Data standard defines how stable, shared classifications and
controlled code lists are owned, versioned, consumed and retired throughout
Naswood Operating System.

Reference data examples include countries, currencies, units of measure, tax
codes, reason codes, status reasons, document types and standardized
classifications.

---

# Principles

- One authoritative owner
- Stable machine-readable code
- Localized display values
- Versioned lifecycle
- Effective dating
- Auditability
- Backward compatibility
- No duplicated module-local lists

---

# Reference Data Versus Business Entities

Reference data classifies business entities and transactions. It does not
replace entities with identity and behavior.

Examples:

| Reference Data | Business Entity |
|---|---|
| Country code | Customer |
| Currency code | Supplier Invoice |
| Unit code | Material |
| Adjustment reason | Inventory Adjustment |
| Inspection result code | Quality Inspection |

If a concept has independent ownership, relationships, workflow or material
business behavior, it shall be modeled as a business entity rather than a
reference-data row.

---

# Ownership

Every reference-data set has exactly one owner.

Platform owns universal technical and localization sets unless a business
module has authoritative domain responsibility.

Examples:

- ISO country and language codes → Platform
- Currency definitions → Finance
- Units and dimensions → Platform
- Inventory movement reasons → Inventory
- Quality defect classifications → Quality
- Maintenance failure codes → Maintenance

Modules consume reference data through contracts or local read-only
projections. They shall not create competing copies.

---

# Required Model

Every reference-data value includes:

- UUID
- Set Code
- Value Code
- Default Display Name
- Description
- Status
- Sort Order
- Effective From
- Effective To
- Is System
- Version
- Created At
- Created By
- Updated At
- Updated By

Localized labels are stored separately by locale and reference the stable
Value Code.

---

# Code Rules

- Codes are immutable after publication.
- Codes are uppercase ASCII unless an external standard mandates otherwise.
- Display names are never used as identifiers or business-logic inputs.
- External standard codes are preserved exactly and record their authority.
- A retired code is not reused.

---

# Lifecycle

```
Draft → Active → Deprecated → Retired
```

- Draft values cannot be used in posted business transactions.
- Active values may be selected.
- Deprecated values remain readable but cannot be selected for new records
  unless an approved transition permits it.
- Retired values remain resolvable for historical records.
- Published values are never hard deleted.

---

# Versioning and Effective Dating

Changing a display label does not change the stable code.

Changing business meaning requires a new code. Existing records retain the
original code and meaning.

Effective dates shall be used when a classification changes over time.
Consumers evaluating historical records use the value version effective at the
business event time.

---

# API

Canonical resources:

```
GET /api/v1/reference-data/{setCode}
GET /api/v1/reference-data/{setCode}/{valueCode}
```

Management APIs are restricted to the owning module and authorized data
stewards.

Responses use the canonical API envelope and may be cached. Cache invalidation
is driven by version changes and reference-data events.

---

# Events

Standard events:

- ReferenceDataSetCreated
- ReferenceDataValueActivated
- ReferenceDataValueDeprecated
- ReferenceDataValueRetired
- ReferenceDataLabelUpdated

Events use stable codes and versions. They shall not expose persistence
entities.

---

# Validation

- Set Code and Value Code are required.
- Value Code is unique inside a set.
- Effective periods shall not overlap for incompatible meanings.
- A value referenced by historical records cannot be hard deleted.
- Unauthorized modules cannot mutate an owned set.
- Business logic shall compare stable codes, never localized labels.

---

# Audit

Creation, activation, deprecation, retirement, effective-date changes and label
changes are audited with previous and new values.

---

# Acceptance Criteria

- Every set has one documented owner.
- No duplicate set exists in another module.
- Published codes are immutable.
- Localized labels do not affect business logic.
- Historical references remain resolvable.
- APIs and events are versioned.
- Authorization and audit are enforced.

---

# Related Documents

- `Entity_Rules.md`
- `Localization.md`
- `Caching.md`
- `Event_Model.md`
- `Versioning.md`
- `../../00_Project_Governance/Module_Boundaries_and_Ownership.md`

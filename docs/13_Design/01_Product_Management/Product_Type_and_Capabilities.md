# Product Type and Capabilities

**Module:** Product Management

**Domain:** Product Capability Model

**Version:** 1.0

**Status:** Approved

---

# Purpose

This document defines how one canonical Product Master controls Product
participation across NOS modules without creating duplicate Product, Material
or inventory models.

Product Type supplies defaults. A versioned Capability Profile records the
effective behavior of each Product.

---

# Principles

- One Product Master
- One Product Type per Product revision
- Explicit capabilities
- Type-based defaults
- Validated overrides
- Effective-dated versions
- No automatic Material creation
- No module-local Product copies

---

# Product Types

The approved Product Type catalog includes:

- Raw Material
- Semi Finished
- Finished Good
- Consumable
- Packaging
- Service
- Tool
- Spare Part

Codes are stable Reference Data. Display names may be localized.

Adding, renaming, deprecating or changing the meaning of a Product Type requires
Product Management governance and impact analysis.

---

# Capability Model

Every released Product revision has:

- Inventory Capability
- Production Capability
- Purchasing Capability
- Sales Capability
- Quality Capability
- Maintenance Capability
- Planning Capability

Capabilities are domain values, not UI-only flags.

---

# Capability Values

## Standard Capability Mode

Inventory, Purchasing, Sales, Quality, Maintenance and Planning use:

- `DISABLED`
- `OPTIONAL`
- `ENABLED`

Semantics:

- `DISABLED`: The module shall reject use of the Product.
- `OPTIONAL`: Participation is permitted but is not required or the default
  process; the calling transaction must select it explicitly.
- `ENABLED`: Participation is an approved standard process.

Inventory `OPTIONAL` or `ENABLED` never creates stock, Material, warehouse
assignment or opening balance.

## Production Capability Mode

- `NONE`
- `CONSUMPTION_ONLY`
- `OUTPUT_ONLY`
- `BOTH`

The directional enum is authoritative. No independent `ProductionEnabled`
boolean exists.

## Canonical Representation

```yaml
capabilities:
  inventory:
    mode: ENABLED
  production:
    mode: BOTH
  purchasing:
    mode: OPTIONAL
  sales:
    mode: ENABLED
  quality:
    mode: ENABLED
  maintenance:
    mode: DISABLED
  planning:
    mode: ENABLED
```

Booleans are not canonical capability fields.

---

# Approved Product Type Defaults

| Product Type | Inventory | Purchasing | Sales | Production | Quality | Planning |
|---|---|---|---|---|---|---|
| Raw Material | ENABLED | ENABLED | DISABLED | CONSUMPTION_ONLY | ENABLED | ENABLED |
| Semi Finished | ENABLED | OPTIONAL | DISABLED | BOTH | ENABLED | ENABLED |
| Finished Good | ENABLED | OPTIONAL | ENABLED | OUTPUT_ONLY | ENABLED | ENABLED |
| Consumable | ENABLED | ENABLED | DISABLED | CONSUMPTION_ONLY | OPTIONAL | DISABLED |
| Packaging | ENABLED | ENABLED | OPTIONAL | CONSUMPTION_ONLY | OPTIONAL | ENABLED |
| Spare Part | ENABLED | ENABLED | OPTIONAL | NONE | OPTIONAL | DISABLED |
| Tool | OPTIONAL | ENABLED | DISABLED | NONE | OPTIONAL | DISABLED |
| Service | DISABLED | ENABLED | ENABLED | NONE | DISABLED | DISABLED |

Maintenance defaults were not included in the approved matrix. Spare Part and
Tool are explicitly used by Maintenance/Tooling, but the capability mode for
every Product Type shall remain an explicit Product-level decision until a
complete Maintenance default matrix is approved.

Implementation shall not invent missing Maintenance defaults.

---

# Product-Level Configuration

A Product revision begins with its Product Type defaults.

Authorized users may propose capability overrides where the Product Type policy
permits them. Overrides require:

- Reason
- Effective date
- Impact analysis
- Validation
- Required workflow approval
- Audit

Changing capabilities creates a new Capability Profile revision. It does not
rewrite the Product or an existing profile. A separate Product revision is
required only when the Product definition itself changes.

Capabilities are not stored as columns on `Product`. They are owned by a
separate versioned `ProductCapabilityProfile` aggregate.

---

# Capability Profile

Aggregate Root: `ProductCapabilityProfile`

Contains:

- Capability Profile ID
- Product ID
- Product Revision ID
- Profile Revision
- Inventory Mode
- Purchasing Mode
- Sales Mode
- Production Mode
- Quality Mode
- Maintenance Mode
- Planning Mode
- Effective From
- Effective To
- Status
- Change Reason
- Approved By and At
- Activated By and At
- Superseded By Profile ID
- Version
- Audit metadata

`Product` stores `CurrentCapabilityProfileId` as a convenience pointer. The
profile remains the authoritative behavior contract.

Capability Profile lifecycle:

```
DRAFT → UNDER_REVIEW → APPROVED → ACTIVE → SUPERSEDED → RETIRED
```

- Draft may be edited.
- Under Review is locked for workflow review.
- Approved is not effective until activated.
- Active may govern new business transactions during its effective period.
- Superseded remains immutable and historically resolvable.
- Retired cannot govern new transactions.

Only one profile may be Active for a Product revision and effective instant.
Effective periods shall not overlap.

Activating, superseding or correcting a profile never rewrites an earlier
profile.

---

# Validation Rules

At minimum:

- Inventory `DISABLED` Products cannot be used in Inventory transactions.
- Production `CONSUMPTION_ONLY` Products cannot be production outputs.
- Production `OUTPUT_ONLY` Products cannot be consumed as BOM components.
- Production `NONE` Products cannot be used in production execution.
- Purchasing `DISABLED` Products cannot be placed on purchase documents.
- Sales `DISABLED` Products cannot be placed on quotations or sales orders.
- Quality `DISABLED` Products cannot silently bypass a quality requirement
  imposed by another approved rule.
- Maintenance `DISABLED` Products cannot be registered as maintenance-managed
  spare parts or tools.
- Planning `DISABLED` Products cannot independently generate planning demand.

Capabilities authorize participation; they do not guarantee that all other
business validations pass.

---

# Product and Material Creation

The following is prohibited:

```
Product Created
↓
Automatically Create Material
```

The valid model is:

```
Product
↓
Product Type
↓
Versioned Capabilities
↓
Authorized Business Transaction
↓
Inventory creates physical Material when physical stock exists
```

Physical Material creation triggers include only approved Inventory
transactions, such as:

- Goods Receipt
- Production Output
- Approved Opening Balance

Planning recommendations, purchase orders, sales orders and Product release do
not create physical Material.

---

# BOM Use

BOM is owned by Manufacturing.

BOM:

- references a released output Product revision
- references released component Product revisions
- defines quantity and unit
- may associate components with an operation
- does not own Product
- does not own physical Material

During production execution, Inventory allocates eligible physical Material
instances for component Product references.

---

# Future Manufacturing Extensions

The capability model permits future participation but does not encode every
manufacturing scenario.

- Co-Product and By-Product require Manufacturing-owned BOM output roles.
- Rework requires Production execution and Manufacturing genealogy rules.
- Phantom BOM requires a Manufacturing BOM type and explosion behavior.
- Outsourcing and Subcontracting require sourcing and execution strategies
  across Manufacturing, Purchasing and Production.

These capabilities shall be added through their owning domain models rather
than by overloading Product capability enums.

---

# Module Enforcement

| Module | Required check |
|---|---|
| Inventory | Inventory Capability |
| Production | Directional Production Capability |
| Purchasing | Purchasing Capability |
| Sales | Sales Capability |
| Quality | Quality Capability plus governing quality rules |
| Maintenance | Maintenance Capability |
| Planning | Planning Capability |

Each module evaluates the released Product revision referenced by the
transaction. UI visibility is not sufficient enforcement.

---

# Data Model

Canonical entities:

- `ProductType`
- `ProductCapabilityPolicy`
- `ProductCapabilityProfile`
- `ProductCapabilityProfileHistory`

Canonical tables:

- `product_capability_profiles`
- `product_capability_profile_history`

The Product aggregate stores the active profile pointer. Business transactions
store both:

- Product Revision ID
- Capability Profile ID

Capabilities are stored and exchanged as controlled enum values. Boolean
capability fields are prohibited because they cannot preserve `OPTIONAL` or
directional Production semantics.

Historical transactions never resolve capabilities through the Product's
current profile pointer. They use their pinned Capability Profile ID.

---

# API

```
GET  /api/v1/product-types
GET  /api/v1/product-types/{code}/capability-defaults
GET  /api/v1/products/{id}/capability-profiles
GET  /api/v1/product-capability-profiles/{profileId}
POST /api/v1/products/{id}/capability-profiles
POST /api/v1/product-capability-profiles/{profileId}/submit
POST /api/v1/product-capability-profiles/{profileId}/approve
POST /api/v1/product-capability-profiles/{profileId}/activate
POST /api/v1/product-capability-profiles/{profileId}/supersede
```

Responses identify Product Revision ID, Capability Profile ID and Profile
Revision.

---

# Events

- ProductTypeCreated
- ProductTypeDeprecated
- ProductCapabilityDefaultsChanged
- ProductCapabilityProfileCreated
- ProductCapabilityProfileApproved
- ProductCapabilityProfileActivated
- ProductCapabilityProfileSuperseded

Consumers refresh projections idempotently.

Planning may create a new plan version, Manufacturing may validate new
definitions, and Sales/Purchasing may refresh eligibility projections after
activation. Existing transactions retain their pinned profile.

---

# Authorization and Audit

Permissions distinguish viewing, proposing and approving Product Type defaults
and Product capability overrides.

Every change records previous/new values, reason, actor, workflow decision,
effective date and impacted Product revision.

AI may recommend capabilities but cannot activate or approve them.

---

# Acceptance Criteria

- One Product Master supports all approved Product Types.
- Product capabilities are explicit and versioned.
- Capability behavior is stored in a separate ProductCapabilityProfile.
- Product columns do not duplicate capability modes.
- Historical transactions pin Capability Profile ID.
- Activation and supersession publish canonical events.
- No capability is reduced to a canonical boolean.
- Product Type supplies defaults without hiding Product-level configuration.
- No Product action automatically creates Material or stock.
- Every module enforces its capability server-side.
- Unsupported combinations are rejected.
- Missing Maintenance defaults remain blocked rather than invented.
- Capability changes are authorized, approved and audited.

---

# Related Documents

- `Product_Management_Architecture.md`
- `../05_Production/BOM_Architecture.md`
- `../02_Inventory/Inventory_Ledger.md`
- `../99_Shared/Reference_Data.md`
- `../99_Shared/Versioning.md`
- `../../00_Project_Governance/Architecture_Decisions.md`

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

Product Type supplies defaults. A versioned capability set records the
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

## Inventory Capability

- Disabled
- Enabled

Enabled means the Product may participate in Inventory processes. It does not
create stock, Material, warehouse assignment or opening balance.

## Production Capability

- None
- Consumption Only
- Output Only
- Both

Production Enabled is a derived summary:

```
Production Enabled = Production Capability != None
```

The directional value is authoritative because a boolean cannot distinguish
raw-material consumption from producible output.

## Purchasing Capability

- Disabled
- Optional
- Enabled

Optional means purchasing is permitted but is not the default supply strategy.
Enabled means purchasing is an approved standard process. Neither value creates
a supplier, purchase request or purchase order.

## Sales Capability

- Disabled
- Enabled

## Quality Capability

- Disabled
- Enabled

## Maintenance Capability

- Disabled
- Enabled

## Planning Capability

- Disabled
- Enabled

---

# Approved Default Examples

These defaults implement the approved examples. Product-level overrides remain
subject to validation and authorization.

| Product Type | Inventory | Production | Purchasing | Sales |
|---|---|---|---|---|
| Finished Good | Enabled | Output Only | Optional | Enabled |
| Raw Material | Enabled | Consumption Only | Enabled | Disabled |
| Service | Disabled | None | Enabled | Enabled |

Quality, Maintenance and Planning defaults for these types, and all defaults
for Semi Finished, Consumable, Packaging, Tool and Spare Part, require separate
business approval.

No unspecified default may be invented by implementation.

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

Changing capabilities of a released Product creates a new Product revision.

---

# Validation Rules

At minimum:

- Inventory-disabled Products cannot be used in Inventory transactions.
- Production `Consumption Only` Products cannot be production outputs.
- Production `Output Only` Products cannot be consumed as BOM components.
- Production `None` Products cannot be used in production execution.
- Purchasing-disabled Products cannot be placed on purchase documents.
- Sales-disabled Products cannot be placed on quotations or sales orders.
- Quality-disabled Products cannot silently bypass a quality requirement
  imposed by another approved rule.
- Maintenance-disabled Products cannot be registered as maintenance-managed
  spare parts or tools.
- Planning-disabled Products cannot independently generate planning demand.

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
- `ProductCapabilitySet`
- `ProductCapabilityOverride`

`ProductCapabilitySet` contains:

- Product Revision ID
- Product Type Code
- Inventory Capability
- Production Capability
- Purchasing Capability
- Sales Capability
- Quality Capability
- Maintenance Capability
- Planning Capability
- Effective From and To
- Version
- Approval metadata
- Audit metadata

Capabilities are stored as controlled values. Derived boolean fields may be
exposed for UI convenience but are not independent sources of truth.

---

# API

```
GET  /api/v1/product-types
GET  /api/v1/product-types/{code}/capability-defaults
GET  /api/v1/products/{id}/capabilities
POST /api/v1/products/{id}/capability-revisions
POST /api/v1/products/{id}/capability-revisions/{revisionId}/submit
POST /api/v1/products/{id}/capability-revisions/{revisionId}/approve
```

Responses identify the Product revision and capability-set version.

---

# Events

- ProductTypeCreated
- ProductTypeDeprecated
- ProductCapabilityDefaultsChanged
- ProductCapabilitiesProposed
- ProductCapabilitiesApproved
- ProductCapabilitiesActivated

Consumers refresh projections idempotently.

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
- Production direction is not reduced to a boolean.
- Product Type supplies defaults without hiding Product-level configuration.
- No Product action automatically creates Material or stock.
- Every module enforces its capability server-side.
- Unsupported combinations are rejected.
- Unknown defaults remain blocked rather than invented.
- Capability changes are authorized, approved and audited.

---

# Related Documents

- `Product_Management_Architecture.md`
- `../05_Production/BOM_Architecture.md`
- `../02_Inventory/Inventory_Ledger.md`
- `../99_Shared/Reference_Data.md`
- `../99_Shared/Versioning.md`
- `../../00_Project_Governance/Architecture_Decisions.md`

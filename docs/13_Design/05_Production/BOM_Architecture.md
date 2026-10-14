# BOM Architecture

**Module:** Manufacturing

**Capability:** Production Master

**Domain:** Bill of Materials

**Version:** 1.0

**Status:** Approved

---

# Purpose

The Bill of Materials defines how a released Product is manufactured from
component Products, quantities, units and operation context.

BOM is owned by Manufacturing Production Master.

---

# Ownership

Manufacturing owns:

- BOM identity and business code
- BOM header
- BOM revision
- BOM lines
- BOM alternatives
- Effectivity
- Approval and release lifecycle
- BOM explosion
- Engineering change history

BOM does not own:

- Product definitions or revisions
- Material Master or physical Material
- Units of measure
- Operations, work centers, lines or tooling
- Inventory balances
- Purchase orders
- Sales orders
- Production execution
- Financial postings

It references these capabilities through stable contracts.

---

# Core Meaning

BOM answers:

> How is a Product manufactured?

It is not purchasing data, stock data or sales data.

---

# Aggregate

Aggregate Root: `BillOfMaterials`

Contains:

- BOM ID
- BOM Number
- Output Product ID and Revision ID
- Output Capability Profile ID
- BOM Type
- Company and plant applicability
- Base quantity and unit
- Status
- Effective dates
- Revision
- Lines
- Alternatives
- Version
- Approval and audit metadata

Released BOM revisions are immutable.

---

# BOM Line

Each BOM line contains:

- Line ID and sequence
- Component Product ID and Revision ID
- Component Capability Profile ID
- Quantity
- Unit
- Component role
- Operation reference where applicable
- Scrap or yield factor where approved
- Issue method
- Alternative group
- Effective dates
- Notes

A BOM line references Product Master. It never references a specific physical
Material, batch, serial number or inventory row.

Released BOM revisions pin the capability profiles validated at release.
Creating a new Product capability profile does not rewrite an existing BOM
revision.

Physical Material is selected and allocated by Inventory during authorized
production execution.

---

# Referenced Ownership

```
Product and Product Revision → Product Management
Material Master and Physical Material → Inventory
Unit → Platform Reference Data
Operation → Manufacturing Production Master
Work Center → Manufacturing Production Master
Production Line → Manufacturing Production Master
Tooling → Manufacturing Production Master
```

BOM stores identifiers and approved revision references. It does not copy or
mutate the referenced aggregates.

---

# Lifecycle

```
Draft → Under Engineering Review → Approved → Released → Superseded → Retired
```

- Draft may be edited.
- Under Engineering Review is controlled by Workflow.
- Approved has passed required reviews.
- Released may be used for Planning and Production.
- Superseded remains valid for historical execution references.
- Retired cannot be selected for new execution.

Changing a released BOM creates a new revision.

---

# BOM Types

Initial supported types may include:

- Manufacturing BOM
- Engineering BOM
- Phantom BOM
- Planning BOM
- Service BOM

The type catalog and conversion rules require Product/Manufacturing governance.
A Sales BOM is not automatically a Manufacturing BOM and shall not share
ownership merely because the names are similar.

---

# Product Capability Validation

Output Product:

- must have Production Capability `OUTPUT_ONLY` or `BOTH`
- must reference a released Product revision

Component Product:

- must have Production Capability `CONSUMPTION_ONLY` or `BOTH`
- must reference a released Product revision

Service components and non-stock costs require separately approved BOM-line
roles and shall not fabricate physical Material.

---

# Multi-Level BOM

A component Product may reference its own released BOM.

Rules:

- Cycles are prohibited.
- Explosion uses effective revisions for the planning or execution date.
- Historical orders retain the exact BOM revision used.
- Phantom expansion preserves traceability to the source BOM line.
- Maximum depth is an operational limit, not a business identity rule.

---

# Alternatives

Alternatives may be defined for:

- Component Product
- BOM revision
- Plant applicability
- Operation

Alternative selection policy must be documented and authorized. BOM does not
select suppliers or physical batches.

---

# Operations

A component may be associated with an Operation definition.

Operation association means the component is planned for consumption at that
step. It does not post consumption and does not assign a machine, worker or
physical Material.

---

# Planning Integration

Planning:

- reads released BOM revisions
- explodes component Product requirements
- applies approved factors and effectivity
- produces material requirements

Planning recommendations do not create purchase orders, production orders or
physical Material automatically.

---

# Production Integration

Production Order references:

- Output Product revision
- Output Capability Profile ID
- Released BOM revision
- Released routing revision

Component requirements retain the Component Capability Profile IDs pinned by
the released BOM revision.

Production creates execution requirements from the immutable referenced
revision. It does not modify BOM.

---

# Inventory Integration

Inventory:

- evaluates component Product capability
- checks availability
- reserves and allocates physical Material
- posts consumption
- posts output Material when production output physically exists

BOM never directly reserves or posts inventory.

---

# Costing Integration

BOM exposes component quantities and approved factors to Finance Costing.

Manufacturing does not own component prices, valuation layers or journal
entries. Cost rollup results do not modify BOM.

---

# Database

Canonical tables:

- `boms`
- `bom_revisions`
- `bom_lines`
- `bom_alternative_groups`
- `bom_alternatives`
- `bom_operation_links`
- `bom_history`
- `bom_outbox`
- `bom_inbox`

Product, unit and operation references are stable identifiers. Cross-module
foreign keys are prohibited.

---

# API

```
GET  /api/v1/boms
GET  /api/v1/boms/{id}
GET  /api/v1/boms/{id}/revisions
GET  /api/v1/boms/{id}/explode
POST /api/v1/boms
POST /api/v1/boms/{id}/revisions
POST /api/v1/boms/{id}/submit
POST /api/v1/boms/{id}/approve
POST /api/v1/boms/{id}/release
POST /api/v1/boms/{id}/retire
```

Requests and responses use DTO contracts, expected versions and idempotency
keys where applicable.

---

# Events

- BOMCreated
- BOMRevisionCreated
- BOMSubmittedForReview
- BOMApproved
- BOMReleased
- BOMSuperseded
- BOMRetired

`BOMExploded` is not a lifecycle event. Explosion is a query or Planning
calculation unless a separately documented business process requires an event.

---

# Authorization

Permissions distinguish:

- View
- Create
- Edit Draft
- Create Revision
- Review
- Approve
- Release
- Retire
- Manage Alternatives
- View Restricted Engineering Data

Company, plant, Product category and engineering scope are enforced
server-side.

---

# Audit

Audit includes:

- BOM creation
- Header and line changes
- Revision creation
- Alternative changes
- Operation-link changes
- Submission, approval and release
- Supersession and retirement

Historical released revisions are retained.

---

# Acceptance Criteria

- Manufacturing Production Master is the sole BOM owner.
- BOM references Product revisions without owning Product.
- BOM never references physical Material instances.
- Output and component capabilities are validated.
- Released revisions are immutable.
- Cycles are rejected.
- Planning and Production retain the exact BOM revision used.
- Inventory allocation and posting remain outside BOM.
- BOM APIs never expose persistence entities.
- Lifecycle actions are authorized, workflow-controlled and audited.

---

# Related Documents

- `../01_Product_Management/Product_Management_Architecture.md`
- `../01_Product_Management/Product_Type_and_Capabilities.md`
- `../02_Inventory/Inventory_Ledger.md`
- `../05_Planning/Planning_Architecture.md`
- `../99_Shared/Transactions.md`
- `../../00_Project_Governance/Architecture_Decisions.md`
- `../../14_Implementation/Sprint_04_Production_Master/TASK-046_BOM.md`

# Inventory Material Master

**Module:** Inventory

**Domain:** Physical Material Identity

**Version:** 1.0

**Status:** Approved

---

# Purpose

Inventory Material Master is the authoritative source for physical Material
identity in NOS.

A Material exists only when physical stock or a traceable physical unit exists.
It references a released Product revision but does not duplicate Product
definition or capabilities.

---

# Ownership

Inventory owns:

- Material identity
- Immutable Material business code
- Product and Product revision reference
- Physical classification and state
- Batch/serial association
- Current Inventory status and location references
- Material lifecycle
- Barcode and QR identity

Inventory does not own:

- Product definition, Product Type or capabilities
- BOM, routing or operation definitions
- Transformation genealogy
- Sales, purchase or production documents
- Quality inspection rules
- Financial valuation rules

Manufacturing owns transformation genealogy and references Inventory Material
identifiers.

---

# Creation Rule

The following is prohibited:

```
Product Created or Released
↓
Automatically Create Material
```

Material may be created only as part of an authorized posted Inventory
transaction:

- Goods Receipt
- Production Output
- Approved Opening Balance

Planning, quotation, sales order, purchase order, production order, BOM release
and Product release do not create physical Material.

---

# Product Reference

Every Material references:

- Product ID
- Released Product Revision ID

The referenced Product must have Inventory Capability `OPTIONAL` or `ENABLED`.
`OPTIONAL` requires explicit use by the posting transaction.

For Goods Receipt, Purchasing Capability must be `OPTIONAL` or `ENABLED`.

For Production Output, Production Capability must be `OUTPUT_ONLY` or `BOTH`.

Inventory stores the references used at creation. It does not copy Product
attributes as an independently editable master.

---

# Aggregate

Aggregate Root: `Material`

Contains:

- Material ID
- Immutable Material Code
- Product ID and Revision ID
- Material state
- Batch or serial reference where applicable
- Quantity and unit where the identity represents a quantified unit
- Inventory status
- Current warehouse and location references
- Origin transaction ID
- Created At and By
- Version

Whether quantity is held per Material, batch or ledger stock key is defined by
the approved Inventory tracking strategy. Duplicate quantity authorities are
prohibited.

---

# Lifecycle

```
Created → Available → Reserved or Allocated → Consumed or Shipped → Archived
                     ↓
                  Quality Hold or Blocked
```

Lifecycle transitions are driven by posted Inventory transactions and approved
Quality decisions.

Manufacturing transformations may consume input Materials and cause Inventory
to create output Materials. Manufacturing records genealogy links after
Inventory confirms the identifiers.

---

# Material State Versus Inventory Status

Material State describes the physical lifecycle.

Inventory Status describes availability for stock operations.

They are separate controlled values and shall not be collapsed into one enum.
Their exact catalogs require approved domain documentation.

---

# Tracking

Tracking modes are derived from Product/Inventory policy:

- Quantity
- Batch
- Serial
- Individually Identified Material

A released Product revision and Inventory policy determine the allowed mode.
Changing tracking mode after physical transactions exist requires a controlled
migration and architecture review.

---

# Database

Canonical tables:

- `materials`
- `material_identifiers`
- `material_status_history`
- `material_product_references`

Inventory Ledger owns quantity movements. Manufacturing owns genealogy edges.

No cross-module foreign keys are created to Product Management or
Manufacturing schemas.

---

# API

```
GET /api/v1/materials
GET /api/v1/materials/{id}
GET /api/v1/materials/{id}/timeline
GET /api/v1/materials/{id}/inventory
GET /api/v1/materials/{id}/genealogy
```

Material creation is not exposed as unrestricted CRUD. It occurs through the
authorized Inventory posting commands that represent physical transactions.

Manual correction uses approved Inventory adjustment or identity-correction
workflows and remains auditable.

---

# Events

- MaterialCreated
- MaterialStatusChanged
- MaterialBlocked
- MaterialReleased
- MaterialConsumed
- MaterialShipped
- MaterialArchived

Manufacturing publishes transformation and genealogy facts. Inventory shall
not publish those facts on behalf of Manufacturing.

---

# Authorization and Audit

Permissions distinguish viewing identity, viewing restricted attributes,
blocking/releasing, and controlled correction.

Creation, status changes, location changes, corrections and archival are
audited with source transaction and correlation identifiers.

---

# Acceptance Criteria

- Inventory is the sole Material Master owner.
- Every Material references a released Product revision.
- Product creation never creates Material.
- Material is created only with a posted physical Inventory transaction.
- Product attributes are not duplicated as editable Material master data.
- Material identity and Inventory Ledger remain consistent.
- Manufacturing genealogy references Material IDs without owning Material.
- Material creation cannot be invoked as unrestricted CRUD.

---

# Related Documents

- `../01_Product_Management/Product_Management_Architecture.md`
- `../01_Product_Management/Product_Type_and_Capabilities.md`
- `Inventory_Ledger.md`
- `Reservation.md`
- `../05_Production/BOM_Architecture.md`
- `../99_Shared/Transactions.md`
- `../../00_Project_Governance/Architecture_Decisions.md`

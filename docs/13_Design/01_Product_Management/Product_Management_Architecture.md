# Product Management Architecture

**Module:** Product Management

**Domain:** Product Definition

**Version:** 1.0

**Status:** Approved

---

# Purpose

Product Management is the authoritative source for Product identity,
definition, classification, revisions and lifecycle throughout Naswood
Operating System.

A Product describes what the enterprise offers, plans, manufactures, assures,
costs and sells. It is not a physical inventory instance.

---

# Ownership

Product Management owns:

- Product identity and business code
- Product name and description
- Product classification
- Product type
- Product capability set
- Product revision
- Technical and commercial classification attributes
- Lifecycle and release status
- Product document links
- Product substitution relationships
- Product configuration rules that are explicitly approved for this domain

Product Management does not own:

- Customer-specific prices, discounts or quotation terms
- Physical Material identity or genealogy
- Inventory quantity, location or reservation
- Production orders or execution
- Quality inspections
- Financial valuation or journals
- BOM or routing until their ownership is separately approved

---

# Module Relationships

| Consumer | Use of Product |
|---|---|
| Sales | Quotation and sales-order lines |
| CRM | Customer interest and opportunity context |
| Planning | Demand, MRP and scheduling reference |
| Manufacturing | Technical process and resource compatibility |
| Production | Execution output target |
| Inventory | Classification of finished or managed stock |
| Quality | Inspection requirements and acceptance context |
| Finance | Costing, revenue and reporting reference |
| Analytics | Product dimensions and KPIs |
| Digital Twin | Product model reference |
| AI Copilot | Governed Product knowledge |

Consumers retain the Product ID and released revision used by a business
transaction. They shall not copy or mutate the Product aggregate.

---

# Product Versus Material

Product:

- Defines an offered or manufactured item.
- Has revisions and release governance.
- May be referenced before physical production exists.
- Does not participate as a physical node in material genealogy.

Material:

- Is a physical traceable instance.
- Is owned by Inventory.
- Has identity, quantity and location managed by Inventory.
- Participates in transformation and genealogy.

Product creation or release never creates Material automatically. Inventory
creates Material only when an authorized physical transaction is posted, such
as goods receipt, production output or approved opening balance.

---

# Aggregate

Aggregate Root: `Product`

Contains:

- Product ID
- Immutable business code
- Product type
- Default name and description
- Classification references
- Current lifecycle status
- Active revision
- Revision history
- Substitution relationships
- Document references
- Version
- Audit metadata

Published revisions are immutable.

---

# Product Revision

A revision contains:

- Revision ID and code
- Effective dates
- Description
- Approved Product attributes
- Measurement and unit references
- Quality classification references
- Regulatory and sustainability references
- Document links
- Release metadata

Changing the meaning or technical definition of a released Product creates a
new revision. Historical business documents continue to reference their
original revision.

---

# Lifecycle

```
Draft → Under Review → Approved → Released → Deprecated → Retired
```

- Draft may be edited.
- Under Review is locked for controlled review.
- Approved has completed required decisions but is not yet available for
  operational use.
- Released may be used by consuming modules.
- Deprecated remains readable and may be restricted for new use.
- Retired cannot be selected for new transactions and remains historically
  resolvable.

Release, deprecation and retirement use the Workflow Engine according to
approved Product policies.

---

# Classification

Product classifications are governed Reference Data or Product Management
entities with one owner.

Classification codes are stable and localized labels do not drive business
logic.

Product classification shall not duplicate:

- Material physical state
- Inventory status
- Quality inspection result
- Sales-order status
- Production-order status

---

# Commercial Boundary

Sales owns:

- Customer
- Quotation
- Sales Order
- Customer-specific price
- Discount
- Delivery commitment
- Payment and delivery terms on sales documents

Product Management may expose commercial classification and sales eligibility,
but it does not calculate or own transaction prices.

---

# Manufacturing Boundary

Manufacturing owns:

- Material genealogy
- BOM
- Machine and process capability
- Work center, production line and tooling
- Process parameters
- Routing according to the approved ownership decision

Product Management provides the released Product revision. Manufacturing shall
not modify Product definitions.

BOM lines reference released Product revisions, quantity, unit and operation
context. Physical Material selection occurs during execution through Inventory.

---

# Database

Canonical tables:

- `products`
- `product_revisions`
- `product_classifications`
- `product_classification_assignments`
- `product_substitutions`
- `product_documents`
- `product_history`
- `product_outbox`
- `product_inbox`

Other modules do not write these tables or create cross-module foreign keys.

---

# API

Queries:

```
GET /api/v1/products
GET /api/v1/products/{id}
GET /api/v1/products/{id}/revisions
GET /api/v1/products/{id}/released-revision
```

Commands:

```
POST /api/v1/products
PATCH /api/v1/products/{id}
POST /api/v1/products/{id}/revisions
POST /api/v1/products/{id}/submit
POST /api/v1/products/{id}/approve
POST /api/v1/products/{id}/release
POST /api/v1/products/{id}/deprecate
POST /api/v1/products/{id}/retire
```

APIs expose Product contracts and DTOs, never persistence entities.

---

# Events

- ProductCreated
- ProductRevisionCreated
- ProductSubmittedForReview
- ProductApproved
- ProductReleased
- ProductDeprecated
- ProductRetired
- ProductClassificationChanged

Events identify the Product and revision. They do not include sales pricing,
inventory balance or physical genealogy.

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
- Deprecate
- Retire
- Manage Classification
- View Restricted Technical Data

Company, plant, Product category and confidentiality scope are enforced
server-side.

---

# Audit

Audit includes:

- Product creation
- Attribute and classification changes
- Revision creation
- Submission, approval and release
- Deprecation and retirement
- Substitution changes
- Document link changes

Released revision history is immutable.

---

# Acceptance Criteria

- Product has one authoritative owner.
- Sales does not own or mutate Product definitions.
- Product and physical Material remain distinct.
- Product creation never creates Material or stock automatically.
- Published revisions are immutable.
- Historical transactions retain their Product revision.
- Consumers use versioned contracts and identifiers.
- Product APIs never expose database entities.
- Product lifecycle actions are authorized, workflow-controlled and audited.
- BOM remains owned by Manufacturing and references Product contracts.

---

# Pending Business Decisions

- Product-type taxonomy
- Capability defaults for Product Types not explicitly approved
- Required approval chain
- Revision-effectivity rules
- Product substitution rules
- Global versus company-specific Product scope

---

# Related Documents

- `../../00_Project_Governance/Module_Boundaries_and_Ownership.md`
- `../../00_Project_Governance/Architecture_Decisions.md`
- `../99_Shared/Entity_Rules.md`
- `../99_Shared/Reference_Data.md`
- `../99_Shared/Versioning.md`
- `../00_Platform/Workflow_Engine.md`
- `Product_Type_and_Capabilities.md`
- `../05_Production/BOM_Architecture.md`

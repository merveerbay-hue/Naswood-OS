# Architecture Decisions

**Project:** Naswood OS

**Document:** Architecture Decisions

**Code:** GOV-003

**Version:** 1.0

**Status:** Active

---

# 1. Purpose

This document records the major architectural decisions made during the design and evolution of Naswood OS.

Each decision captures the rationale, expected benefits and impact on the overall platform architecture.

---

# 2. Objectives

- Document architectural decisions
- Preserve design rationale
- Ensure consistency
- Support future development
- Reduce architectural drift

---

# 3. Decision Process

Identify

↓

Evaluate

↓

Approve

↓

Implement

↓

Review

---

# 4. Decision Record

Each architectural decision shall include:

- Decision ID
- Title
- Status
- Context
- Decision
- Rationale
- Consequences
- Related Documents

---

# 5. Decision Status

- Proposed
- Approved
- Implemented
- Deprecated
- Superseded

---

# 6. Principles

- Business Driven
- Long-Term Maintainability
- Simplicity
- Scalability
- Reusability
- Traceability

---

# 7. Related Documents

- Vision.md
- Roadmap.md
- Business_Rules.md

---

# 8. Decision Register

## ADR-001 — Documentation Authority

**Status:** Approved

**Context:** The Constitution contains one AI decision hierarchy that places
Architecture above Constitution, while its documentation hierarchy and
architecture-development hierarchy place Constitution first.

**Decision:** The NOS Constitution is the highest authority. Architecture,
Standards, Domain, Design, Implementation, Code, Tests and Deployment follow
in that order.

**Rationale:** This is the repeated constitutional rule and the explicit
project instruction.

**Consequences:** Lower-level documents and implementation shall be corrected
when they conflict. Architecture shall not redefine the Constitution.

**Related Documents:** `Phase_0_Architecture_Resolution.md`

---

## ADR-002 — Mandatory Architecture Pattern

**Status:** Approved

**Context:** Documents describe Clean Architecture, DDD, Hexagonal Architecture,
CQRS and event-driven communication with inconsistent levels of obligation.

**Decision:** Clean Architecture, DDD, Hexagonal Architecture and CQRS are
mandatory for every business module. Cross-module reactions are event-driven;
synchronous collaboration uses versioned contracts.

**Rationale:** Constitution Part 02 declares these standards mandatory.

**Consequences:** CRUD-only services, direct cross-module orchestration and
database-entity APIs are not compliant.

**Related Documents:** `Phase_0_Architecture_Resolution.md`,
`docs/13_Design/99_Shared/Architecture.md`

---

## ADR-003 — Canonical Technology Baseline

**Status:** Approved

**Context:** Shared design lists SQL Server while system architecture, schema
documents and AI designs require PostgreSQL, JSONB and pgvector.

**Decision:** React and TypeScript are the frontend baseline, .NET is the
backend baseline and PostgreSQL is the canonical transactional database.

**Rationale:** This resolves the existing implementation dependencies with the
least architectural divergence. Domain and application layers remain
technology-independent.

**Consequences:** SQL Server references are non-canonical. Framework-specific
Java annotations in implementation tasks describe authorization intent only
and shall not dictate the backend framework.

**Related Documents:** `docs/13_Design/99_Shared/Architecture.md`

---

## ADR-004 — Module Data Ownership

**Status:** Approved

**Context:** Multiple documents define duplicate customer, supplier, warehouse,
reservation, shipment and receipt models.

**Decision:** Every entity and capability has one owning module. Other modules
use stable identifiers, contracts and local read-only projections. Cross-module
table writes and cross-module foreign keys are prohibited.

**Rationale:** This enforces bounded contexts, replaceability and single source
of truth.

**Consequences:** Duplicate models must be consolidated or explicitly
classified as projections before implementation.

**Related Documents:** `Module_Boundaries_and_Ownership.md`

---

## ADR-005 — Canonical API and Event Contracts

**Status:** Approved

**Context:** Response envelopes, error structures, sorting syntax, pagination
shapes and event names differ across approved documents.

**Decision:** `Phase_0_Canonical_Contracts.md` controls interpretation until
all affected standards and module designs are aligned.

**Rationale:** One contract is required for reusable clients, public APIs and
replaceable modules.

**Consequences:** New module contracts shall use the canonical envelope,
sorting syntax, event schema, versioning and idempotency rules.

**Related Documents:** `Phase_0_Canonical_Contracts.md`

---

## ADR-006 — Transaction and Messaging Reliability

**Status:** Approved

**Context:** Event-driven integration is mandatory, but transaction ownership,
atomic publication and duplicate delivery were undefined.

**Decision:** One local database transaction per owning module; transactional
outbox for publication; inbox/deduplication for consumption; saga/process
manager for multi-module processes.

**Rationale:** This preserves local consistency without coupling module
databases.

**Consequences:** Distributed database transactions are prohibited. Every
integration consumer must be idempotent.

**Related Documents:** `docs/13_Design/99_Shared/Transactions.md`

---

## ADR-007 — Workflow Ownership

**Status:** Approved

**Context:** Modules define approvals independently while a shared Workflow
Engine is constitutionally required.

**Decision:** Workflow Engine owns definitions, versions, instances, tasks,
delegation, escalation and execution history. Business modules own rules and
state changes. Workflows call module commands and never write module tables.

**Rationale:** Workflow configurability must not duplicate or extract business
logic from bounded contexts.

**Consequences:** Hardcoded module approval engines are prohibited.

**Related Documents:** `Phase_0_Canonical_Contracts.md`,
`docs/13_Design/99_Shared/Approval_Workflow.md`

---

## ADR-008 — Production, Manufacturing and CRM Boundaries

**Status:** Approved

**Context:** Production overlaps Manufacturing; Sales overlaps CRM. Existing
documents do not provide one approved ownership model.

**Decision:** Manufacturing owns Production Master, genealogy, reusable
resources and process definitions. Production owns production execution.
Inventory owns Material Master and physical Material identity. Sales owns
Customer, Quotation and Sales Order. CRM owns Lead, Opportunity, activities,
interactions and relationship history.

**Rationale:** The accountable business owner approved the recommended
boundaries during Phase 0.

**Consequences:** Module designs and implementation tasks shall align to these
owners. Existing duplicate customer, material, genealogy, resource and
execution models must become owned models or read-only projections.

**Related Documents:** `Module_Boundaries_and_Ownership.md`

---

## ADR-009 — Negative Inventory Policy

**Status:** Approved

**Context:** Documents conflict between prohibiting negative inventory and
allowing policy-based exceptions.

**Decision:** Posted physical inventory shall never become negative. No
company, plant, warehouse, location, material or user override is permitted.
Unfulfilled demand is represented as shortage, backorder, planning exception
or failed posting, never as negative stock.

**Rationale:** This preserves physical truth, auditability, costing integrity
and deterministic availability.

**Consequences:** Inventory posting rejects any transaction that would produce
a negative On Hand quantity. Configuration cannot weaken this invariant.

**Related Documents:** `docs/13_Design/99_Shared/Negative_Stock.md`,
`docs/13_Design/02_Inventory/Inventory_Ledger.md`

---

## ADR-010 — Product Management Ownership

**Status:** Approved

**Context:** Product is used as a commercial catalog, technical definition,
planning input and costing reference. The proposed Sales ownership was not
approved.

**Decision:** Product Management owns Product identity, definitions,
Product Types, capabilities, classifications, revisions and lifecycle. Sales
and operational modules consume released Product contracts.

**Rationale:** The accountable business owner approved Product Management as a
dedicated module during Phase 0.

**Consequences:** Sales shall not own or mutate Product definitions. Product
Management shall not own physical Material, inventory or production execution.
Product creation does not create Material or stock.

**Related Documents:** `Module_Boundaries_and_Ownership.md`,
`Phase_0_Issue_Register.md`

---

## ADR-011 — BOM and Product Capability Model

**Status:** Approved

**Context:** BOM ownership and the relationship between Product definitions and
physical Material were unresolved. A single boolean Production flag cannot
represent consumption-only behavior.

**Decision:** Manufacturing Production Master owns BOM. BOM references released
Product revisions, quantity, unit and operation context without owning Product
or Material. Product Management owns Product Type and versioned capabilities:
Inventory, Production Mode, Purchasing Mode, Sales, Quality, Maintenance and
Planning.

Product creation or release never creates Material automatically. Inventory
creates physical Material only through an authorized posted transaction such
as goods receipt, production output or approved opening balance.

**Rationale:** BOM answers how a Product is manufactured. Product Type and
capabilities express allowed module participation without duplicating Product
masters or fabricating physical stock.

**Consequences:** Production capability is directional (`None`,
`ConsumptionOnly`, `OutputOnly`, `Both`). Purchasing distinguishes Disabled,
Optional and Enabled. Product Type provides defaults; Product capability
overrides are validated, versioned and audited.

**Related Documents:**
`docs/13_Design/05_Production/BOM_Architecture.md`,
`docs/13_Design/01_Product_Management/Product_Type_and_Capabilities.md`

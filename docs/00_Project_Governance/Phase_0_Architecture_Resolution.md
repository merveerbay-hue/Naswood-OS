# Phase 0 Architecture Resolution

**Project:** Naswood Operating System (NOS)

**Document:** Phase 0 Architecture Resolution

**Code:** GOV-004

**Version:** 1.0

**Status:** Approved

---

# 1. Purpose

This document establishes the mandatory architecture-completion gate that
precedes implementation of Naswood Operating System.

Phase 0 resolves technical contradictions, identifies business decisions that
require accountable approval, and defines the evidence required before source
code may be created.

---

# 2. Authority

The governing hierarchy is:

```
NOS Constitution
↓
Architecture
↓
Standards
↓
Domain
↓
Design
↓
Implementation Tasks
↓
Source Code
↓
Tests
↓
Deployment
```

Lower levels may extend higher levels but may never contradict them.

The AI decision hierarchy in `01_FOUNDATION.md` that places Architecture above
the Constitution is superseded by the Constitution's documentation hierarchy,
the Architecture Development Hierarchy and the explicit project instruction
that the Constitution is the highest authority.

---

# 3. Platform Identity

NOS is a modular enterprise operating system. It is not an ERP.

ERP capabilities such as Sales, Purchasing, Inventory and Finance are modules
inside NOS. They do not define the identity or architectural boundary of the
platform.

NOS must behave as one product while preserving independently maintainable and
replaceable business modules.

---

# 4. Mandatory Architecture

Every business module shall use:

- Clean Architecture
- Domain-Driven Design
- Hexagonal Architecture
- CQRS
- Contract-based module boundaries
- Event-driven cross-module reactions
- Server-enforced authorization
- Immutable audit history
- Versioned API and event contracts

The canonical dependency direction is:

```
Presentation → Application → Domain
Infrastructure → Application and Domain ports
```

The Domain Layer shall not depend on UI, database, ORM, HTTP, messaging,
framework or cloud-provider implementations.

---

# 5. Resolved Technical Decisions

## 5.1 Technology Baseline

- Frontend: React, TypeScript and the approved NOS Design System
- Backend: .NET
- Transactional database: PostgreSQL
- External API style: versioned REST
- Real-time client delivery: SignalR where required
- Deployment: container-ready and infrastructure-portable

PostgreSQL is canonical because the system architecture, PostgreSQL schema
definitions, JSONB usage, event/audit requirements and pgvector-based AI design
already depend on PostgreSQL capabilities. References to SQL Server as the
canonical database are superseded.

Technology choices remain adapters. Domain and application layers shall remain
portable.

## 5.2 Module Communication

- A module owns its domain model and persistence.
- No module may write another module's tables.
- Synchronous needs use versioned module contracts.
- Cross-module reactions use versioned integration events.
- Distributed writes use idempotent consumers and the outbox/inbox pattern.
- Direct database synchronization between modules is prohibited.

## 5.3 Contract Standards

`Phase_0_Canonical_Contracts.md` is the controlling interpretation for API,
event, transaction, authentication and workflow contracts when existing
same-level design documents disagree.

## 5.4 Shared Infrastructure

Authentication, authorization, audit, configuration, notifications, workflow,
document management, numbering, localization, search, reporting, printing and
observability are Platform capabilities. Business modules shall consume them
through ports and contracts and shall not reimplement them.

## 5.5 Master Data

Master Data is a governance capability, not a database shared by all modules.
Each master entity has one owning bounded context. Other modules retain only
the owning identifier and approved local projections.

## 5.6 Replaceability

Replaceability means that a module can be substituted without exposing its
tables or internal domain model. It does not mean absence of dependencies.
Dependencies must be explicit, versioned, observable and migratable.

---

# 6. Business Decisions Requiring Approval

The following decisions may not be invented by architecture or implementation
teams:

| Decision | Required outcome | Status |
|---|---|---|
| Production versus Manufacturing | Manufacturing owns resources, Material and genealogy; Production owns execution | Approved |
| Sales versus CRM | Sales owns Customer and commercial transactions; CRM owns pre-sale relationships | Approved |
| Product ownership | Owning module and Product-to-Material transition | Pending |
| Material ownership | Manufacturing owns physical identity and genealogy | Approved |
| Planning scope | Demand, MRP, capacity and scheduling boundaries | Pending |
| Finance event policy | Events Finance may publish without becoming an operational source | Pending |
| Negative inventory | Prohibited without exceptions; shortages are represented separately | Approved |
| Costing policy | Permitted valuation and production-costing methods | Pending |
| Approval matrices | Roles, thresholds, escalation and segregation of duties | Pending |

Approved boundaries and remaining options are maintained in
`Module_Boundaries_and_Ownership.md`. Pending decisions are implementation stop
conditions for the affected capabilities.

---

# 7. Documentation Remediation Gate

Before a module enters implementation:

1. Its owner and bounded context are approved.
2. Entities, aggregates and state transitions are defined once.
3. Business rules and exception paths are documented.
4. API and event contracts are versioned.
5. Database ownership and transaction boundaries are defined.
6. Authentication and authorization requirements are defined.
7. Workflow and approval dependencies are defined.
8. Audit requirements are defined.
9. Reusable Platform services and UI components are identified.
10. Acceptance criteria and test obligations are traceable to design.
11. Contradicting lower-level documents are corrected or superseded.
12. No required document is empty or placeholder-only.

Any failed item stops implementation.

---

# 8. Phase 0 Deliverables

- Architecture decision records
- Canonical module catalog
- Entity ownership matrix
- Canonical contract standards
- Reference Data standard
- Transaction standard
- Documentation contradiction register
- Business decision queue
- Dependency-based implementation roadmap
- Architecture readiness checklist

---

# 9. Completion Rule

Phase 0 is complete only when all critical architectural decisions are Approved
and all affected Architecture, Standards, Domain, Design and Implementation
documents agree.

Completion of this document does not authorize source-code implementation.

---

# 10. Related Documents

- `AI/NOS_CONSTITUTION/01_FOUNDATION.md`
- `AI/NOS_CONSTITUTION/02_ENGINEERING.md`
- `AI/NOS_CONSTITUTION/03_PLATFORM.md`
- `Architecture_Decisions.md`
- `Module_Boundaries_and_Ownership.md`
- `Phase_0_Canonical_Contracts.md`
- `docs/13_Design/99_Shared/Architecture.md`

# Architecture Decision Records (ADR)

**Project:** Naswood OS
**Document:** Architecture Decisions
**Version:** 1.0
**Status:** Active

---

# Purpose

This document records the major architectural decisions made during the design and development of Naswood OS.

Each decision includes:

- Context
- Decision
- Reason
- Consequences
- Status

Architecture decisions shall never be deleted.

If a decision changes, a new ADR entry is created.

---

# ADR-001

## Title

Material-Centric Manufacturing Model

### Status

Accepted

### Date

2026-08

### Context

Traditional ERP systems are document-centric.

Naswood requires full material traceability throughout production.

### Decision

The system shall be designed around physical Materials rather than documents.

Every physical object inside the factory receives a unique Material identity.

### Consequences

- Complete genealogy
- Better traceability
- AI-ready structure
- Easier production analytics

---

# ADR-002

## Title

Transformation-Centric Production

### Status

Accepted

### Context

Production is the process of converting materials into new materials.

Traditional stock movement models cannot represent manufacturing accurately.

### Decision

Every manufacturing operation shall be represented by a Transformation entity.

Transformation becomes the central production object.

### Consequences

- Complete production history
- Accurate costing
- Waste tracking
- Recovery tracking
- Better reporting

---

# ADR-003

## Title

Material Identity Never Changes

### Status

Accepted

### Context

A physical object may change dimensions, moisture or quality.

Its identity should remain constant.

### Decision

Every Material receives a UUID that never changes.

Business properties may change.

Material identity never changes.

### Consequences

Reliable genealogy

Reliable inventory

Reliable traceability

---

# ADR-004

## Title

Business Code and UUID Separation

### Status

Accepted

### Context

Business users require readable codes.

Software requires immutable identifiers.

### Decision

Every entity contains:

UUID

Business Code

UUID is used internally.

Business Code is displayed to users.

---

# ADR-005

## Title

Attribute-Based Material Model

### Status

Accepted

### Context

Different material types require different technical properties.

Adding database columns for every new product is not scalable.

### Decision

Engineering properties shall be stored as configurable Material Attributes.

Material remains lightweight.

### Consequences

Unlimited product expansion

No schema changes

Future-proof architecture

---

# ADR-006

## Title

Measurement and Attribute Separation

### Status

Accepted

### Context

Measurements are observations.

Attributes describe the current state.

### Decision

Measurements shall be stored independently.

Material Attributes contain only the current engineering values.

Historical measurements remain immutable.

### Consequences

Better analytics

AI-ready measurements

High performance

---

# ADR-007

## Title

Event-Driven Architecture

### Status

Accepted

### Context

Modules should remain independent.

### Decision

Business modules communicate through Events.

Direct module-to-module data modification is prohibited.

### Consequences

Loose coupling

Scalability

Better integrations

Auditability

---

# ADR-008

## Title

Receiving Lot as Manufacturing Origin

### Status

Accepted

### Context

All materials entering the factory require traceable origin.

### Decision

Every material shall reference its Receiving Lot.

Receiving Lots represent truck-based or supplier-based intake.

### Consequences

Supplier traceability

FSC traceability

Recall support

---

# ADR-009

## Title

Dynamic Routing

### Status

Accepted

### Context

Wood production routes vary according to species, moisture, quality and production decisions.

### Decision

Routing shall remain configurable.

Production may follow different routes without changing database structure.

### Consequences

Flexible manufacturing

Reduced maintenance

Future product support

---

# ADR-010

## Title

Recovery is Production

### Status

Accepted

### Context

Recovered materials continue production.

They are not waste.

### Decision

Recovered materials create new Material identities while maintaining genealogy.

### Consequences

Maximum yield

True recovery reporting

Complete traceability

---

# ADR-011

## Title

Transformation Owns Production Cost

### Status

Accepted

### Context

Production costs originate during manufacturing operations.

### Decision

Costs shall be calculated at Transformation level.

Material stores only accumulated results.

### Consequences

True manufacturing costing

Operation-level profitability

Accurate KPI calculations

---

# ADR-012

## Title

Soft Delete Policy

### Status

Accepted

### Context

Manufacturing history must never be lost.

### Decision

Business records shall never be physically deleted.

Soft Delete is mandatory.

### Consequences

Audit compliance

Historical reporting

Recovery capability

---

# ADR-013

## Title

AI-First Architecture

### Status

Accepted

### Context

Naswood OS is designed to support AI-driven manufacturing.

### Decision

Every module shall expose structured, machine-readable data for AI services.

AI may recommend but never directly modify production data without user approval.

### Consequences

AI-ready platform

Predictive analytics

Future autonomous optimization

---

# ADR-014

## Title

Production Strategy Support

### Status

Accepted

### Context

Naswood manufactures both standard stock products and customer-specific products.

### Decision

The Planning Engine shall support:

- Make to Stock (MTS)
- Make to Order (MTO)
- Assemble to Order (ATO)
- Engineer to Order (ETO)

### Consequences

Flexible planning

Support for multiple business models

Scalable production planning

---

# ADR-015

## Title

Organization and Permissions Separation

### Status

Accepted

### Context

Organizational hierarchy and system permissions are different concepts.

### Decision

Reporting structure shall be managed independently from access permissions.

Permissions are controlled through the Permission Model.

### Consequences

Flexible authorization

Clear organizational model

Support for matrix organizations

---

# Future ADRs

Examples of future decisions:

- Database engine changes
- Microservice migration
- AI governance
- Cloud deployment strategy
- Multi-tenant architecture
- Digital Twin implementation
- PLC communication standards
- OPC-UA integration
- BIM integration
- Carbon accounting model

---

# ADR Rules

- Every architectural decision receives a unique ADR number.
- Accepted decisions are never edited; if a decision changes, create a new ADR that supersedes the previous one.
- Each ADR must include Context, Decision, Consequences and Status.
- Every significant architectural change must be documented before implementation.
- All developers, AI assistants and contributors must consult this document before changing the architecture.


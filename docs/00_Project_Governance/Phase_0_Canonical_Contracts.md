# Phase 0 Canonical Contracts

**Project:** Naswood Operating System (NOS)

**Document:** Canonical Contracts

**Code:** GOV-006

**Version:** 1.0

**Status:** Approved

---

# 1. Purpose

This document resolves conflicting same-level design examples and defines the
minimum contract shared by every NOS module.

Detailed standards may add constraints but may not change these shapes or
semantics without an approved versioned architecture decision.

---

# 2. API Contract

## 2.1 Resource Rules

- Base path: `/api/v1`
- Resource names: plural kebab-case nouns
- Database entities shall never be returned directly.
- Request, response, summary, detail, import and export DTOs are distinct
  contracts where their responsibilities differ.
- Every protected endpoint requires authentication and server-side
  authorization.
- Tenant, company and plant scope shall be derived from validated identity and
  explicit request context, never trusted from an unchecked header.

## 2.2 Success Envelope

```json
{
  "success": true,
  "data": {},
  "message": null,
  "metadata": {}
}
```

`data` contains the resource or collection. `metadata` contains transport
metadata such as pagination and contract warnings.

## 2.3 Error Envelope

```json
{
  "success": false,
  "data": null,
  "message": "Validation failed.",
  "errors": [
    {
      "code": "MAT-001",
      "category": "Validation",
      "field": "materialCode",
      "message": "Material Code already exists.",
      "details": {}
    }
  ],
  "metadata": {
    "correlationId": "9fd8b9ab-2e5e-47d3-b6cf-f61f0cbb2d9",
    "timestamp": "2026-08-05T10:15:22Z"
  }
}
```

Error codes use `{MODULE}-{NNN}`. Stack traces, secrets and internal
implementation details shall never be returned.

## 2.4 Pagination

Request:

```
GET /api/v1/materials?page=1&pageSize=25
```

Response:

```json
{
  "success": true,
  "data": [],
  "message": null,
  "metadata": {
    "pagination": {
      "page": 1,
      "pageSize": 25,
      "totalItems": 1240,
      "totalPages": 50,
      "hasNext": true,
      "hasPrevious": false
    }
  }
}
```

Cursor pagination uses `metadata.pagination.nextCursor`. Offset pagination is
for bounded interactive lists; keyset/cursor pagination is required for event,
audit, telemetry and other unbounded streams.

## 2.5 Sorting

Canonical syntax:

```
?sort=name,-createdAt
```

A leading `-` means descending. Absence means ascending. Servers shall append
`id` as a stable final sort key when it is not already present.

The separate `direction` parameter is superseded.

## 2.6 Concurrency and Idempotency

- Mutable resources expose a version or ETag.
- Writes use `If-Match` or the documented version field.
- Version conflicts return HTTP 409.
- Retryable commands accept an `Idempotency-Key`.
- Idempotency records are scoped by caller, operation and owning module.

---

# 3. Event Contract

```json
{
  "eventId": "uuid",
  "eventType": "PurchaseOrderApproved",
  "eventVersion": 1,
  "occurredAt": "2026-08-05T12:00:00Z",
  "correlationId": "uuid",
  "causationId": "uuid",
  "sourceModule": "Purchasing",
  "tenantId": "uuid",
  "actor": {
    "type": "User",
    "id": "uuid"
  },
  "subject": {
    "type": "PurchaseOrder",
    "id": "uuid"
  },
  "payload": {}
}
```

Rules:

- Event names are past-tense business facts.
- Events are immutable.
- Published event versions are never rewritten.
- Event payloads expose integration contracts, not persistence entities.
- Consumers are idempotent.
- Delivery is at least once; consumers shall not assume exactly once.
- Ordering is guaranteed only within the documented aggregate stream.
- Publication and business-state commit use an outbox.
- Consumption and side effects use an inbox or equivalent deduplication.
- Sensitive fields are excluded or protected according to classification.

Domain events remain inside the owning module. Integration events are the
versioned public facts consumed by other modules.

---

# 4. Database Contract

- PostgreSQL is the canonical transactional database.
- Each module owns a schema or otherwise enforceable persistence boundary.
- Cross-module foreign keys and cross-module table writes are prohibited.
- Cross-module references store the owner's stable identifier.
- Every mutable table includes created/updated audit fields and a concurrency
  version.
- Soft-delete fields are included for entities governed by the Soft Delete
  standard.
- Timestamps are UTC.
- Schema changes use version-controlled, forward-compatible migrations.
- Business rules do not execute in triggers or stored procedures.
- Read models may be denormalized but remain derived and rebuildable.
- Audit, event and financial journals are append-only.

---

# 5. Authentication and Authorization Contract

- Authentication is a single Platform capability.
- Authorization is deny-by-default.
- RBAC grants coarse capability; ABAC/policies apply company, plant, warehouse,
  record and field constraints.
- UI visibility is not authorization.
- Every command and protected query performs server-side authorization.
- Human, service, integration, device and AI identities are distinguishable.
- Tokens do not contain mutable authorization truth beyond a bounded cache
  lifetime; critical actions re-evaluate policy.
- Security actions are audited.

---

# 6. Workflow Contract

- Workflow definitions are versioned configuration, not hardcoded module logic.
- A running instance remains bound to its definition version.
- The Workflow Engine owns definitions, instances, tasks, delegation,
  escalation, timeout and execution history.
- Business modules own eligibility rules and resulting state changes.
- Workflow actions invoke module commands through contracts.
- Workflows never write module tables.
- Approval decisions include actor, authority, reason, timestamp and source
  state version.
- Automatic approval by AI is prohibited.
- Offline clients may prepare a decision but cannot finalize authorization-
  sensitive workflow actions without server validation.

---

# 7. Distributed Process Contract

Cross-module processes use a saga/process-manager pattern when multiple module
commits are required.

Every step defines:

- command or event contract
- idempotency boundary
- success event
- failure event
- timeout
- retry policy
- compensating action where valid
- terminal manual-intervention state where compensation is unsafe

Distributed database transactions across modules are prohibited.

---

# 8. Versioning

Contract evolution follows:

```
Add → Adopt → Deprecate → Retire
```

Breaking API changes require a new major API version. Breaking integration
event changes require a new event version and a transition plan. Transition
periods are defined per contract and recorded in its change history; no
undefined global period is assumed.

---

# 9. Related Standards

- `docs/13_Design/99_Shared/API_Standards.md`
- `docs/13_Design/99_Shared/Error_Handling.md`
- `docs/13_Design/99_Shared/Event_Model.md`
- `docs/13_Design/99_Shared/Integration_Events.md`
- `docs/13_Design/99_Shared/Pagination.md`
- `docs/13_Design/99_Shared/Sorting.md`
- `docs/13_Design/99_Shared/Concurrency.md`
- `docs/13_Design/99_Shared/Permission_Model.md`
- `docs/13_Design/99_Shared/Transactions.md`

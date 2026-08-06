# TASK-002 Authorization — Implementation Plan

**Task:** TASK-002 — Authorization  
**Module:** Platform  
**Status:** Approved for implementation  
**Date:** 2026-08-05

---

## 1. Authority Applied

1. Constitution (deny-by-default, server-side authorization)
2. ADR-002 / ADR-005 / Phase 0 Canonical Contracts
3. Design: `Authorization.md`, `Permission_Model.md`
4. Task: `TASK-002_Authorization.md`
5. P0-055: one API owner per Role/Permission surface

---

## 2. Conflict Resolution

| Topic | Decision | Rationale |
|---|---|---|
| Role CRUD POST/PUT/DELETE | Deferred to TASK-004 | P0-055 + TASK-004 owns Role Management |
| Permission CRUD POST/PUT/DELETE | Deferred to TASK-005 | P0-055 + TASK-005 owns Permission Management |
| API base path | `/api/v1` | Canonical Contracts |
| ABAC / Policy engine | Out of scope | Design Authorization Excluded / Future |
| Field-level UI matrix | Not invented | Design marks Field as Future; check API accepts optional field permission codes |
| Document-level | Evaluate via check request (company/plant/owner) | TASK acceptance; no invented approval matrices |

---

## 3. In Scope

- RBAC permission evaluation engine (deny-by-default)
- Company and plant access validation
- Document-level checks: company, plant, optional resource owner
- Field-level as permission-code evaluation when catalog contains field permissions
- In-memory permission cache with invalidation hook
- ASP.NET permission requirement handler (`RequirePermission`)
- Authorization denial audit via outbox + append-only authorization history
- Seed permission catalog + Administrator role
- Endpoints:
  - `GET /api/v1/permissions` (catalog read)
  - `GET /api/v1/roles` (role read)
  - `GET /api/v1/me/permissions`
  - `POST /api/v1/authorization/check`
  - `GET /api/v1/authorization/modules`
  - `GET /api/v1/authorization/menu`

---

## 4. Out of Scope

- Authentication / sessions (TASK-001)
- Role administration mutations (TASK-004)
- Permission definition mutations (TASK-005)
- User Management (TASK-003)
- ABAC policy engine, temporary/delegated access administration UIs
- Notification delivery

---

## 5. Error Codes

| Code | Meaning |
|---|---|
| AUTHZ-001 | ACCESS_DENIED |
| AUTHZ-002 | PERMISSION_REQUIRED |
| AUTHZ-003 | COMPANY_ACCESS_DENIED |
| AUTHZ-004 | PLANT_ACCESS_DENIED |
| AUTHZ-005 | ROLE_REQUIRED |
| AUTHZ-006 | SESSION_INVALID |

---

## 6. Completion Gate

Compile + tests green, docs updated, no placeholders.

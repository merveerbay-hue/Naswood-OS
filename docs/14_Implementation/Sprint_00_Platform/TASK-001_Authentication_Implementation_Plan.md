# TASK-001 Authentication — Implementation Plan

**Task:** TASK-001 — Authentication  
**Module:** Platform  
**Status:** Approved for implementation  
**Date:** 2026-08-05

---

## 1. Authority Order Applied

1. NOS Constitution (auth, security, CQRS, folder structure)
2. ADR-002 / ADR-003 / ADR-005 / ADR-006
3. Design: `Authentication.md`, `Security.md`, `Event_Model.md`, `Transactions.md`
4. Task: `TASK-001_Authentication.md`
5. Canonical Contracts: `/api/v1` envelopes

**Conflict resolution**

| Topic | Winner | Rationale |
|---|---|---|
| OAuth2 / OIDC / MFA | Design (Excluded / Future) | Design > Task |
| API base path | `/api/v1` | Phase 0 Canonical Contracts + Task |
| Response shape | Canonical envelope `data` | Phase 0 Canonical Contracts |
| User CRUD APIs | Out of scope | Task Out of Scope |

---

## 2. In Scope (this TASK only)

- Username + password authentication
- BCrypt password verification
- JWT access token (60 minutes, configurable)
- Refresh token (30 days) with rotation
- Session create / validate / revoke / expire
- Device metadata on session
- Account lockout after 5 failed attempts
- Login history persistence
- Endpoints:
  - `POST /api/v1/auth/login`
  - `POST /api/v1/auth/logout`
  - `POST /api/v1/auth/refresh`
  - `POST /api/v1/auth/revoke`
  - `GET /api/v1/auth/me`
  - `GET /api/v1/auth/session`
- Integration/outbox events for auth outcomes
- Login rate limiting (5 attempts → lock window)
- PostgreSQL persistence (EF Core)
- JWT bearer authentication for protected auth queries

---

## 3. Explicitly Out of Scope

- User CRUD (TASK-003)
- Role / Permission management APIs (TASK-004 / TASK-005)
- Authorization policies (TASK-002)
- Password reset
- MFA, OAuth, OIDC, LDAP, Entra ID
- Login UI (TASK-000)
- Notification delivery (TASK-011)
- Central Audit Log module UI/API (TASK-013) — Auth owns login history + publishes events

---

## 4. Identity Store Strategy (no User CRUD)

Authentication requires an Identity Store. User Management APIs are out of scope.

Platform owns a credential aggregate `AuthUser` used only for authentication:

- Identity, username, display name, email
- Password hash
- Active / locked / deleted flags
- Failed attempt count, lock timestamp
- Password expiration
- Company and plant assignments
- Role names for JWT claims (bounded cache; not authorization truth)

Bootstrap admin is created from configuration on startup when no users exist.  
TASK-003 will add User Management APIs against Platform identity later.

---

## 5. Domain Model

- `AuthUser` (aggregate) — credential and lockout behavior
- `AuthSession` (aggregate) — session lifecycle, refresh token hash, device
- `LoginHistoryEntry` — append-only login attempt record
- Value objects: `PasswordHash`, `DeviceInfo`
- Domain events → outbox integration events

---

## 6. Error Codes

| Code | Meaning |
|---|---|
| AUTH-001 | INVALID_CREDENTIALS |
| AUTH-002 | ACCOUNT_DISABLED |
| AUTH-003 | ACCOUNT_LOCKED |
| AUTH-004 | PASSWORD_EXPIRED |
| AUTH-005 | TOKEN_EXPIRED |
| AUTH-006 | TOKEN_INVALID |
| AUTH-007 | SESSION_EXPIRED |
| AUTH-008 | REFRESH_TOKEN_INVALID |
| AUTH-009 | COMPANY_OR_PLANT_REQUIRED |

---

## 7. Persistence

PostgreSQL schema `platform`:

- `auth_users`
- `auth_user_companies`
- `auth_user_plants`
- `auth_user_roles`
- `auth_sessions`
- `auth_login_history`
- `outbox_messages`

---

## 8. Test Plan

- Domain unit tests: lockout, credential rules, session revoke/rotate
- Application/integration tests: login success/failure, lockout, refresh rotation, logout, me/session with JWT
- Build must be green before completion

---

## 9. Completion Gate

TASK complete only when compile + tests pass, docs updated, no placeholders, production-ready.

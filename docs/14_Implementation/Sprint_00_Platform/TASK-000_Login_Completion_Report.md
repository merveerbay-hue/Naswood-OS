# TASK-000 Completion Report — Login

**Task:** TASK-000 — Login  
**Branch:** `cursor/task-000-login-ce37`  
**Date:** 2026-08-05  
**Result:** Completed (UI slice against existing TASK-001 auth API)

---

## Purpose

Deliver the Login screen and client session flow on the React foundation, reusing the completed backend authentication vertical slice (TASK-001).

---

## Implemented

### Routes
| Path | Behavior |
|---|---|
| `/login` | Login form; redirects to `/` when already authenticated |
| `/` | Authenticated shell; redirects to `/login` when anonymous |

### Form fields (TASK-000)
- Username, Password, Company, Plant, Remember Me
- Sign in action
- Forgot Password / Language / Help — present as disabled stubs (password reset, i18n, help center out of scope)

### Client session
- `POST /api/v1/auth/login` → store access + refresh tokens
- Remember Me → `localStorage`; otherwise `sessionStorage`
- `GET /api/v1/auth/me` hydrates identity
- `POST /api/v1/auth/logout` (or revoke) on sign-out
- Automatic refresh on `401` via `POST /api/v1/auth/refresh`
- Auth error codes mapped (AUTH-001/002/003/004/007/008/009)

### Files
```
apps/web/src/api/{types,client,auth}.ts
apps/web/src/auth/{session,auth-context,AuthProvider,useAuth}.ts(x)
apps/web/src/pages/LoginPage.tsx
apps/web/src/pages/FoundationHomePage.tsx  (session + logout)
apps/web/src/router.tsx
apps/web/src/lib/validation.ts
```

---

## Explicitly not done (per scope)

- Password reset / MFA
- Cookie-based auth (API returns Bearer JWT in JSON)
- `/api/v1/auth/validate` (not on backend)
- Dashboard layout / navigation / sidebar (TASK-006+)
- Company/plant catalog picker APIs
- Full localization / theme switcher / notification center

---

## Verification

| Check | Result |
|---|---|
| `pnpm --filter @naswood/web build` | Passed |
| `pnpm --filter @naswood/web lint` | Passed |
| `dotnet test src/Naswood.OS.sln` | Passed — 46 tests |

Dev credentials (bootstrap): `admin` / `Naswood!Admin1`

---

## Status

TASK-000 marked **Completed**.

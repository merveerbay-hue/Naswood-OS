# Naswood OS Source

This directory contains the Naswood Operating System (NOS) backend solution.

## Solution

```
src/Naswood.OS.sln
```

## Layout

| Path | Responsibility |
|---|---|
| `BuildingBlocks/` | Shared kernel: Entity, Result, CQRS abstractions, API envelopes |
| `Modules/Platform/` | Platform bounded context (Health first; Auth next) |
| `Hosts/Naswood.Api/` | Composition root / HTTP host |

## Architecture Rules

- Clean Architecture + DDD + Hexagonal + CQRS
- Domain has no infrastructure or framework dependencies
- Modules own their persistence; no cross-module table writes
- APIs never return database entities
- Business rules come from Constitution / Design — never invented in code

## Build

```bash
dotnet restore src/Naswood.OS.sln
dotnet build src/Naswood.OS.sln
dotnet test src/Naswood.OS.sln
```

## Run

```bash
dotnet run --project src/Hosts/Naswood.Api
```

Health endpoints:

- `GET /health/live`
- `GET /health/ready`
- `GET /health`

## Current Scope

Implemented:

- BuildingBlocks foundation
- Platform Health Check vertical slice (`GET /health`, `/health/live`, `/health/ready`)
- Platform Authentication (TASK-001)
- Platform Authorization (TASK-002):
  - RBAC evaluation engine + company/plant checks
  - `GET /api/v1/permissions|roles`
  - `GET /api/v1/me/permissions`
  - `POST /api/v1/authorization/check`
  - `GET /api/v1/authorization/modules|menu`
  - `RequirePermission` handler

Next (requires explicit approval):

- TASK-000 Login UI, TASK-003 User Management, TASK-004 Role Management, TASK-005 Permission Management
- React application shell

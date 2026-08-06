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
- Platform Authentication (TASK-001):
  - `POST /api/v1/auth/login|logout|refresh|revoke`
  - `GET /api/v1/auth/me|session`
  - JWT + refresh rotation, lockout, login history, outbox events

Next (requires explicit approval):

- TASK-000 Login UI and/or TASK-002 Authorization
- User / Role / Permission management
- React application shell

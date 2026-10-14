# Platform Engineering Foundation

**Module:** Platform

**Category:** Architecture / Engineering

**Version:** 1.0

**Status:** Approved

---

# Purpose

This document defines the first implementable engineering foundation for
Naswood OS source code.

It authorizes the Platform Kernel scaffold only. It does not authorize
Inventory, Sales, Purchasing, Manufacturing or other business-module
implementation while their Phase 0 gates remain open.

---

# Authority

```
Constitution
↓
Architecture Decisions
↓
This Design
↓
Sprint 00 Platform Tasks
↓
Source Code
```

---

# Decision

Source code begins as a modular .NET solution with Clean Architecture layers:

```
src/
  BuildingBlocks/     Shared kernel (Domain, Application, Infrastructure, AspNetCore)
  Modules/Platform/   Platform bounded context
  Hosts/Naswood.Api/  Composition root
tests/
```

Each module follows:

```
Domain → Application → Infrastructure
                     → Presentation
Contracts
```

Dependency direction remains inward. Domain has no framework dependencies.

---

# First Vertical Slice

Sprint 00 TASK-015 Health Check is the first runnable vertical slice because:

- It has no unresolved business-rule inventing
- It establishes host composition, CQRS dispatch, contracts and API envelope
- It provides DevOps probes required before Authentication wiring

Authentication (TASK-000 / TASK-001) follows next and shall consume the same
BuildingBlocks contracts without reimplementing Result, Error or envelopes.

---

# Explicit Non-Goals in This Slice

- PostgreSQL persistence (added when Auth/User aggregates are implemented)
- JWT issuance
- Authorization policies
- UI / React shell
- Inventory or Product module code
- Invented database/cache/queue health probes for services not yet configured

---

# Contracts

API responses use `Phase_0_Canonical_Contracts.md` envelopes.

Health endpoints:

- `GET /health`
- `GET /health/live`
- `GET /health/ready`

---

# Related Documents

- `AI/NOS_CONSTITUTION/02_ENGINEERING.md`
- `docs/00_Project_Governance/Architecture_Decisions.md` (ADR-003, ADR-014)
- `docs/13_Design/00_Platform/Health_Check.md`
- `docs/14_Implementation/Sprint_00_Platform/TASK-015_Health_Check.md`
- `src/README.md`

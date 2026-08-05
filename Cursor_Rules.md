# Cursor Rules

**Project:** Naswood OS

**Document:** Cursor Rules

**Version:** 1.1

---

# Purpose

These rules define how Cursor AI shall operate while developing Naswood OS.

The objective is to ensure consistency, maintainability and enterprise-grade software quality across the entire project.

---

# AI Execution Constitution (read first — mandatory)

**Canonical document:** [`AI/NOS_CONSTITUTION/00_AI_EXECUTION.md`](AI/NOS_CONSTITUTION/00_AI_EXECUTION.md)

TASK documents are **implementation work packages only**.  
They never define product, UI, workflow, navigation, or business architecture.

### Absolute rules

1. **Never generate a screen directly from a TASK.**
2. **Always reconstruct the complete module first** (navigation, roles, workflows, dashboards, workspace hierarchy, page hierarchy).
3. **Only then** implement the requested TASK slice.

### Mandatory read order before any TASK

```text
1. AI Execution Constitution     AI/NOS_CONSTITUTION/00_AI_EXECUTION.md
2. Constitution — Foundation     AI/NOS_CONSTITUTION/01_FOUNDATION.md
3. Engineering Rules             AI/NOS_CONSTITUTION/02_ENGINEERING.md
4. Platform Rules                AI/NOS_CONSTITUTION/03_PLATFORM.md
5. ADRs / system architecture
6. Module Architecture
7. Module Workflow
8. Module API
9. Module Dashboard
10. Module Mobile
11. UI Architecture              docs/15_UI_Architecture/
12. Screen Architecture          docs/15_UI/… (PRD / QLT / MNT / INV …)
13. THEN the TASK document       docs/14_Implementation/ or Design TASK-*
```

### Forbidden default

```text
TASK-078 → Asset CRUD screen
TASK-046 → BOM ResourcePage
```

If higher documents are missing, **stop and report** — do not invent product UI from the TASK.

---

# General Rules

- Follow the project architecture.
- Do not create duplicate functionality.
- Reuse existing modules whenever possible.
- Prefer simple and maintainable solutions.
- Preserve backward compatibility.
- Never modify unrelated files.

---

# Development Principles

- Modular Architecture
- Clean Code
- API First
- Event-Driven Design
- AI Native
- Secure by Design

---

# Documentation Rules

Every new feature shall include:

- Purpose
- Business Objective
- Technical Design
- API Changes
- Database Changes
- Related Documents

---

# Product UI Hierarchy (mandatory)

Follow Constitution authority. For UI work, use this order:

```text
AI Execution Constitution → Constitution → Architecture → Business Domain
    → Module Design (Architecture / Workflow / API / Dashboard / Mobile)
    → UI Architecture (docs/15_UI_Architecture) — Module / Workspace
    → Screen Architecture (docs/15_UI) — PRD-001, QLT-008, … named screens
    → Design specs (docs/13_Design)
    → User Flows (docs/17_User_Flows → 04_Application/UI_Flows.md)
    → Navigation
    → Design System (docs/16_Design_System → 13_Design/.../Design_System)
    → Implementation TASK (docs/14_Implementation)
    → Source Code
```

### Critical rules

- An Implementation **TASK is a work package**, not a product screen and not a module.
- **Forbidden default:** `TASK-XXX → one Library/Create/Edit/Delete ResourcePage` as the finished product shape.
- Before coding business UI, open the **screen PRD** (e.g. `docs/15_UI/Production/Screens/PRD-011_Production_Order_Detail.md`) and implement its components/actions (or a declared MVP subset).
- Production has ~29 indexed screens (target 35–40); Quality ~20; Maintenance ~25; Inventory ~30.
- Current flat CRUD nav/pages are **technical MVP debt**; converge toward `15_UI_Architecture/02_Navigation_Map.md` + `15_UI/`.

---

# Code Standards

- Use meaningful names.
- Keep functions small.
- Avoid duplicated logic.
- Follow existing project structure.
- Remove unused code.
- Write readable code before clever code.

---

# Module Rules

Before creating a new module:

- Verify that it does not already exist.
- Check related modules.
- Reuse shared components.
- Follow existing naming conventions.

---

# Database Rules

- Normalize data where appropriate.
- Never duplicate master data.
- Use audit fields.
- Preserve referential integrity.
- Support future scalability.

---

# API Rules

- API First
- Versioned APIs
- Consistent naming
- Standard error responses
- Secure endpoints

---

# AI Rules

- Use approved enterprise data.
- Never fabricate business information.
- Explain recommendations.
- Respect user permissions.

---

# Security Rules

- Validate all inputs.
- Apply role-based authorization.
- Never expose sensitive data.
- Log critical operations.

---

# Performance Rules

- Optimize database queries.
- Minimize unnecessary API calls.
- Avoid blocking operations.
- Design for scalability.

---

# Testing Rules

Every implementation should include:

- Functional validation
- API validation
- Error handling
- Permission checks
- Regression awareness

---

# Git Rules

- Keep commits focused.
- Write meaningful commit messages.
- Avoid unrelated changes.
- Preserve project history.

---

# Definition of Done

A task is complete when:

- Requirements are implemented.
- Code is reviewed.
- Tests pass.
- Documentation is updated.
- No critical issues remain.

---

# Guiding Principle

Build for long-term maintainability, not short-term convenience.

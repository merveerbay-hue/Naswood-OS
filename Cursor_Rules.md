# Cursor Rules

**Project:** Naswood OS

**Document:** Cursor Rules

**Version:** 1.2

---

# Purpose

These rules define how Cursor AI shall operate while developing Naswood OS.

The objective is to ensure consistency, maintainability and enterprise-grade software quality across the entire project.

---

# AI Execution Constitution (read first — mandatory)

**Canonical:** [`AI/NOS_CONSTITUTION/00_AI_EXECUTION.md`](AI/NOS_CONSTITUTION/00_AI_EXECUTION.md)  
**Product stack:** [`docs/PRODUCT_LAYERS.md`](docs/PRODUCT_LAYERS.md)

`docs/14_Implementation` is **FROZEN** — no new TASK files.  
Delivery driver is the product layer, not Architecture → TASK → TASK.

### Absolute rules

1. **Never generate a screen from a TASK** (or TASK habit).
2. **Always reconstruct the complete module first.**
3. Deliver **Module → Workspace → Navigation → Screens → Components → User Flow → Frontend.**
4. Prefer prompts like **“Maintenance Workspace’i oluştur”**, not **“TASK-078’i yap”**.

### Mandatory read order before product UI / FE work

```text
1. AI Execution Constitution
2. Foundation / Engineering / Platform
3. Module Architecture · Workflow · API · Dashboard · Mobile
4. UI Architecture          docs/15_UI_Architecture/
5. Navigation               docs/19_Navigation/
6. Screen Architecture      docs/15_UI/
7. User Flows               docs/17_User_Flows/
8. Component Library        docs/18_Component_Library/
9. Design System            docs/16_Design_System/
10. Frontend Architecture   docs/20_Frontend_Architecture/
```

### Forbidden default

```text
TASK-078 → Asset CRUD
next TASK → another ResourcePage
new file under 14_Implementation/
```

If product docs are missing, **author the product layer** (screen/flow/nav) — do not invent a TASK.

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

```text
AI Execution Constitution → Constitution → Architecture → Domain
    → Module Design (Architecture / Workflow / API / Dashboard / Mobile)
    → UI Architecture (15_UI_Architecture)
    → Navigation (19_Navigation)
    → Screen Architecture (15_UI)
    → User Flows (17_User_Flows)
    → Component Library (18) → Design System (16)
    → Frontend Architecture (20)
    → Source Code

14_Implementation = FROZEN (historical only)
```

### Critical rules

- Open work with **workspace / screen / flow**, never with a new TASK file.
- Before coding business UI, open the **screen PRD** and compose from Component Library.
- Current flat CRUD nav/pages are **technical MVP debt**; converge to `19_Navigation` + `15_UI`.

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

# Cursor Rules

**Project:** Naswood OS

**Document:** Cursor Rules

**Version:** 1.0

---

# Purpose

These rules define how Cursor AI shall operate while developing Naswood OS.

The objective is to ensure consistency, maintainability and enterprise-grade software quality across the entire project.

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
Constitution → Architecture → Business Domain
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

# Cursor Rules

**Project:** Naswood OS

**Document:** Cursor Rules

**Version:** 1.3

---

# Purpose

These rules define how Cursor AI shall operate while developing Naswood OS.

The objective is to ensure consistency, maintainability and enterprise-grade software quality across the entire project.

---

# Product Architect Drive (design first — mandatory)

**Canonical:** [`AI/NOS_CONSTITUTION/04_PRODUCT_ARCHITECT.md`](AI/NOS_CONSTITUTION/04_PRODUCT_ARCHITECT.md)  
**SSOT matrix:** [`docs/00_Product/DOCUMENTATION_AUTHORITY_MATRIX.md`](docs/00_Product/DOCUMENTATION_AUTHORITY_MATRIX.md)  
**Product map:** [`docs/00_Product/`](docs/00_Product/)  
**Implementation:** [`AI/NOS_CONSTITUTION/00_AI_EXECUTION.md`](AI/NOS_CONSTITUTION/00_AI_EXECUTION.md)  
**Layer map:** [`docs/PRODUCT_LAYERS.md`](docs/PRODUCT_LAYERS.md)

Before stating any cross-cutting rule in a doc or PR: check the Authority Matrix.  
**Yes, already defined → reference only. No → edit the authority document.**

### No shared Create screen — Master Data ≠ Create Form

**Authority:** [`docs/13_Design/Common/Screen_Types.md`](docs/13_Design/Common/Screen_Types.md) · [`UI_Patterns.md`](docs/13_Design/Common/UI_Patterns.md)

```text
NOS'ta "New" diye tek tip ekran yoktur.
NOS'ta Master Data ekranları "Create Form" değildir.
```

Never implement “Yeni” as the same entity Create form across modules.  
Choose **Wizard / Builder / Designer / Configuration / Terminal / Console / Explorer / Planner / Dashboard / Workbench / Approval Center / Library**, then process steps from module flows.

Engineering masters (BOM, Machine, Routing, …) use **Builder / Designer / Configuration** → **Release** — not Code · Name · Save.

```text
✘ Entity → Form
✔ Business Object → Business Workspace → Business Designer
```

### System generated identifiers + name-first UX

**Authority:** [`Document_Numbering.md`](docs/13_Design/99_Shared/Document_Numbering.md) § System Generated Identifiers · Constitution § 2.3

```text
✘  Code *  ________
✔  System Code — Automatically generated after save / on Release
✔  Pickers by name (Ürün 🔍 Thermowood Deck …) — not by typed product code
```

Never generate editable Code / Number / Lot / Warehouse Code fields on data-entry screens.


**We design. Cursor applies.**

```text
NOS → Module → Workspace → Navigation → Screen → Component → Workflow → Permissions → Code
```

### Thinking ladder (before docs or code)

1. Real life — factory behavior  
2. User / roles — who sees what?  
3. Market — SAP / IFS / Dynamics / Infor  
4. NOS better — our product choice  
5. Document — product layers  
6. Implement — named workspace / screens only  

### Absolute rules

1. **Never** start from “TASK-XXX yaz / yap”.
2. **Never** generate a screen from a TASK (or TASK habit).
3. **Always** reconstruct the complete module first (roles + workspaces).
4. Prefer **“Üretim Müdürü Production’da ne görmeli?”** / **“Maintenance Workspace’i oluştur”**, not **“TASK-078’i yap”**.

`docs/14_Implementation` is **FROZEN** — no new TASK files.

### Mandatory read order

```text
1. Product Architect Drive     AI/NOS_CONSTITUTION/04_PRODUCT_ARCHITECT.md
2. AI Execution Constitution   AI/NOS_CONSTITUTION/00_AI_EXECUTION.md
3. NOS Product Map             docs/00_Product/
4. Foundation / Engineering / Platform
5. Module Architecture · Workflow · API · Dashboard · Mobile
6. UI Architecture             docs/15_UI_Architecture/
7. Navigation / Permissions    docs/19_Navigation/
8. Screen Architecture         docs/15_UI/
9. User Flows                  docs/17_User_Flows/
10. Component Library          docs/18_Component_Library/
11. Design System              docs/16_Design_System/
12. Frontend Architecture      docs/20_Frontend_Architecture/
```

### Forbidden default

```text
TASK-078 → Asset CRUD
next TASK → another ResourcePage
new file under 14_Implementation/
“TASK yazalım” as a product brief
```

If product docs are missing, **run the thinking ladder and author the product layer** — do not invent a TASK.

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
Product Architect Drive → AI Execution Constitution → Constitution → Architecture
    → NOS Product Map (00_Product)
    → Module Design (Architecture / Workflow / API / Dashboard / Mobile)
    → UI Architecture (15_UI_Architecture)
    → Navigation + Permissions (19_Navigation)
    → Screen Architecture (15_UI)
    → Components (18) → Workflow / User Flows (17) → Design System (16)
    → Frontend Architecture (20)
    → Source Code

14_Implementation = FROZEN (historical only)
```

### Critical rules

- Open design with **roles and jobs**, not TASK IDs.
- Open implementation with **workspace / screen / flow**, never with a new TASK file.
- Before coding business UI, open the **screen PRD** and compose from Component Library.
- Current flat CRUD nav/pages are **technical MVP debt**; converge to product workspaces.

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

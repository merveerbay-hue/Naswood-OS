# Naswood OS

**Version:** 1.0

**Status:** In Development

---

# Overview

Naswood OS is an integrated Manufacturing Operating System (MOS) designed for the wood manufacturing industry.

The platform combines ERP, MES, WMS, QMS, CMMS, CRM, Analytics, AI and Digital Twin capabilities into a single architecture.

---

# Project Structure

```
00_Project_Governance
01_Business
02_Manufacturing
03_System
04_Application
05_Modules
06_AI
07_Digital_Twin
08_Integration
09_Deployment
10_Testing
11_Documentation
12_Reference
13_Design
14_Implementation
15_UI_Architecture
15_UI
16_Design_System
17_User_Flows
```

---

# Documentation Overview

| Folder | Description |
|--------|-------------|
| 00_Project_Governance | Vision, roadmap and project governance |
| 01_Business | Business processes and rules |
| 02_Manufacturing | Manufacturing concepts and standards |
| 03_System | Core system architecture |
| 04_Application | Screen catalog, dashboards, UI flow registry |
| 05_Modules | Functional business modules |
| 06_AI | Artificial Intelligence platform |
| 07_Digital_Twin | Digital Twin architecture |
| 08_Integration | APIs and external integrations |
| 09_Deployment | Infrastructure and deployment |
| 10_Testing | Testing strategy and quality assurance |
| 11_Documentation | Documentation standards |
| 12_Reference | Standards and reference materials |
| 13_Design | Design specifications (APIs, entities, UX specs) |
| 14_Implementation | Implementation TASKs (work packages — lowest planning unit) |
| 15_UI_Architecture | Module → Workspace IA (above TASKs) |
| 15_UI | **Screen Architecture** — PRD/QLT/MNT/INV… screen specs (enterprise UX) |
| 16_Design_System | Index to canonical Design System under `13_Design` |
| 17_User_Flows | Index to canonical UI Flows under `04_Application` |

### Product UI authority order

```text
AI Execution Constitution (AI/NOS_CONSTITUTION/00_AI_EXECUTION.md)
    → Constitution → Architecture → Domain
    → Module Design (Architecture / Workflow / API / Dashboard / Mobile)
    → UI Architecture (15_UI_Architecture) — workspaces
    → Screen Architecture (15_UI) — named screens (PRD-001…)
    → Design (13) → User Flows (17) → Design System (16)
    → Implementation TASK (14) → Code
```

A TASK never defines module navigation or screen architecture by itself.  
**Never generate a screen directly from a TASK** — reconstruct the module first.  
See `AI/NOS_CONSTITUTION/00_AI_EXECUTION.md`, `15_UI_Architecture/00_Governing_Principles.md`, and `15_UI/Production/`.
---

# Core Principles

- Modular Architecture
- API First
- Event-Driven Design
- AI Native
- Digital Twin Ready
- Secure by Design

---

# Scope

Naswood OS includes:

- Manufacturing
- Inventory
- Quality
- Maintenance
- Machines
- Tooling
- Sales
- Purchasing
- Logistics
- Finance
- Analytics
- Administration
- Artificial Intelligence
- Digital Twin

---

# Architecture

The platform is organized into five logical layers:

Business

↓

Platform

↓

Applications

↓

Modules

↓

Infrastructure

---

# Documentation Standards

All documents follow a common structure:

- Purpose
- Objectives
- Scope
- Principles
- Related Documents

---

# Versioning

Documentation is version controlled and maintained alongside platform development.

---

# License

Internal Project – Naswood

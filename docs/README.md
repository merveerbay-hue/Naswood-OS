# Naswood OS

**Version:** 1.1  
**Status:** In Development

---

# Overview

Naswood OS is an integrated Manufacturing Operating System (MOS) designed for the wood manufacturing industry.

The platform combines ERP, MES, WMS, QMS, CMMS, CRM, Analytics, AI and Digital Twin capabilities into a single architecture.

---

# Product documentation roadmap

```text
AI/NOS_CONSTITUTION/          Authority + AI execution
docs/
├── 13_Design/                ✅ Module design (Architecture, Workflow, API, Dashboard, Mobile)
├── 14_Implementation/        ✅ FROZEN — historical TASK archives only
├── 15_UI_Architecture/       ✅ Module → Workspace IA
├── 15_UI/                    ✅ Screen Architecture (PRD / QLT / MNT / … / CRM)
├── 16_Design_System/         ✅ Product DS index → canonical tokens/components
├── 17_User_Flows/            ✅ Cross-screen jobs
├── 18_Component_Library/     ✅ Enterprise UI blocks
├── 19_Navigation/            ✅ Menu, permissions, workspace, breadcrumbs
└── 20_Frontend_Architecture/ ✅ App shell, routing, FE boundaries
```

Full map: [`PRODUCT_LAYERS.md`](PRODUCT_LAYERS.md)  
AI protocol: [`../AI/NOS_CONSTITUTION/00_AI_EXECUTION.md`](../AI/NOS_CONSTITUTION/00_AI_EXECUTION.md)

### Delivery chain (mandatory)

```text
Architecture → Module → Workspace → Navigation → Screens → Components → User Flow → Frontend
```

**Not:** Architecture → TASK → TASK → TASK

---

# Project Structure (broader docs tree)

```
00_Project_Governance … 12_Reference
13_Design
14_Implementation          (frozen)
15_UI_Architecture
15_UI
16_Design_System
17_User_Flows
18_Component_Library
19_Navigation
20_Frontend_Architecture
```

---

# Documentation Overview

| Folder | Description |
|--------|-------------|
| 00–12 | Governance, business, manufacturing, system, application registry, modules, AI, twin, integration, deploy, test, docs standards, reference |
| 13_Design | Module design packs (Architecture / Workflow / API / Dashboard / Mobile) + platform design |
| 14_Implementation | **FROZEN** historical TASK work packages |
| 15_UI_Architecture | Module → Workspace information architecture |
| 15_UI | Named enterprise screens (Screen Architecture) |
| 16_Design_System | Colors, type, spacing, primitives (index → `13_Design/.../Design_System`) |
| 17_User_Flows | Production, Planning, Inventory, Maintenance, Quality, Sales, Purchasing flows |
| 18_Component_Library | Entity Grid, Master Detail, Kanban, Scheduler, … |
| 19_Navigation | Navigation, Menu, Permissions, Workspace, Breadcrumb |
| 20_Frontend_Architecture | Shell, routing, state, module FE boundaries |

---

# Core Principles

- Modular Architecture
- API First
- Event-Driven Design
- AI Native
- Digital Twin Ready
- Secure by Design

---

# How to ask Cursor

| Avoid | Prefer |
|-------|--------|
| TASK-078’i yap | Maintenance Workspace’i oluştur |
| TASK-056 CRUD | Production Orders (PRD-010 / PRD-011) |
| Yeni TASK yaz | Eksik screen PRD / flow / nav dokümanını yaz |

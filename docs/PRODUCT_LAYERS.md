# NOS Product Documentation Layers

**Status:** Active roadmap  
**Product thinking:** [`AI/NOS_CONSTITUTION/04_PRODUCT_ARCHITECT.md`](../AI/NOS_CONSTITUTION/04_PRODUCT_ARCHITECT.md)  
**AI implementation:** [`AI/NOS_CONSTITUTION/00_AI_EXECUTION.md`](../AI/NOS_CONSTITUTION/00_AI_EXECUTION.md)  
**Replaces as delivery driver:** `14_Implementation` TASK chain

---

## Stack

```text
AI/NOS_CONSTITUTION/
  04_PRODUCT_ARCHITECT.md     How we think & design (Product Architect Drive)
  00_AI_EXECUTION.md          How AI implements after design
  01–03                       Foundation / Engineering / Platform

docs/
├── 00_Product/               ✅ Living NOS product map (modules · roles · workspace outline)
├── 13_Design/                ✅ Module design packs (Architecture, Workflow, API, …)
├── 14_Implementation/        ✅ FROZEN — historical TASK archives only
├── 15_UI_Architecture/       Module → Workspace IA
├── 15_UI/                    Screen Architecture
├── 16_Design_System/         Visual & interaction standards
├── 17_User_Flows/            Cross-screen jobs / workflows
├── 18_Component_Library/     Enterprise building blocks
├── 19_Navigation/            Menu, permissions, workspace chrome
└── 20_Frontend_Architecture/ App shell, routing, state, FE boundaries
```

---

## Product shape (mandatory)

```text
NOS
  ↓
Modules
  ↓
Workspace
  ↓
Navigation
  ↓
Screen
  ↓
Component
  ↓
Workflow
  ↓
Permissions
  ↓
Code
```

**Not:**

```text
Architecture → TASK → TASK → TASK → CRUD
```

---

## Design → Document → Code

```text
1. Real life          How does the factory work?
2. User / roles       Who sees what?
3. Market reference   SAP / IFS / Dynamics / Infor
4. NOS better         Our product choice
5. Document           Product layers above
6. Cursor implement   Named workspace / screens only
```

---

## How to ask Cursor

| Avoid | Prefer |
|-------|--------|
| “TASK-078’i yap” | “Maintenance Workspace’i oluştur” |
| “TASK-056 CRUD” | “Production Orders List + Detail — Planning workspace” |
| “NCR ekranı ekle” | “Quality Non-Conformance screen family + CAPA flow” |
| “TASK yazalım” | “Üretim Müdürü Production’da ne görmeli?” |

---

## AI entry

1. Product thinking → [`04_PRODUCT_ARCHITECT.md`](../AI/NOS_CONSTITUTION/04_PRODUCT_ARCHITECT.md)  
2. Implementation → [`00_AI_EXECUTION.md`](../AI/NOS_CONSTITUTION/00_AI_EXECUTION.md)

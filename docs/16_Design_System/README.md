# 16 — Design System

**Layer:** Visual & interaction standards  
**Status:** Active product layer  
**Canonical specs:** [`docs/13_Design/00_Platform/Design_System/`](../13_Design/00_Platform/Design_System/)

---

## Purpose

This folder is the **product-facing Design System layer** in the delivery stack.
Token and component prose remain canonical under `13_Design`; here we expose the
topics agents must load before building UI.

```text
15_UI (what screen) → 18_Component_Library (which block)
    → 16_Design_System (how it looks/behaves) → Frontend
```

---

## Topic index

| Topic | Canonical path |
|-------|----------------|
| Colors | [01_Foundation/Colors.md](../13_Design/00_Platform/Design_System/01_Foundation/Colors.md) · [Color_Tokens.md](../13_Design/00_Platform/Design_System/01_Foundation/Color_Tokens.md) |
| Typography | [01_Foundation/Typography.md](../13_Design/00_Platform/Design_System/01_Foundation/Typography.md) |
| Spacing | [01_Foundation/Spacing.md](../13_Design/00_Platform/Design_System/01_Foundation/Spacing.md) |
| Cards | [02_Components/Cards.md](../13_Design/00_Platform/Design_System/02_Components/Cards.md) |
| Tables | [02_Components/Tables.md](../13_Design/00_Platform/Design_System/02_Components/Tables.md) |
| Forms | [02_Components/Forms.md](../13_Design/00_Platform/Design_System/02_Components/Forms.md) |
| Dialogs | [02_Components/Dialogs.md](../13_Design/00_Platform/Design_System/02_Components/Dialogs.md) |
| Buttons | [02_Components/Buttons.md](../13_Design/00_Platform/Design_System/02_Components/Buttons.md) |
| Icons | [01_Foundation/Icons.md](../13_Design/00_Platform/Design_System/01_Foundation/Icons.md) · [Iconography.md](../13_Design/00_Platform/Design_System/01_Foundation/Iconography.md) |
| Charts | [05_Charts/](../13_Design/00_Platform/Design_System/05_Charts/) |
| Navigation layout | [03_Layout/Navigation.md](../13_Design/00_Platform/Design_System/03_Layout/Navigation.md) |
| Workspace layout | [03_Layout/Workspace.md](../13_Design/00_Platform/Design_System/03_Layout/Workspace.md) |
| Full DS root | [Design_System/](../13_Design/00_Platform/Design_System/) |

Topic stubs in this folder point at the same files for discoverability:

- [Colors.md](Colors.md) · [Typography.md](Typography.md) · [Spacing.md](Spacing.md)
- [Cards.md](Cards.md) · [Tables.md](Tables.md) · [Forms.md](Forms.md)
- [Dialogs.md](Dialogs.md) · [Buttons.md](Buttons.md) · [Icons.md](Icons.md) · [Charts.md](Charts.md)

**Edit canonical files under `13_Design/.../Design_System/`, not duplicate specs.**

---

## Hierarchy reminder

| Layer | Question |
|-------|----------|
| UI Architecture / Screens | What exists for the user? |
| Component Library | Which enterprise block? |
| Design System | What tokens and primitives? |
| `14_Implementation` | Frozen — not a design driver |

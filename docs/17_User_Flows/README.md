# 17 — User Flows (Index)

**Role:** Stable entry point for end-to-end UI flows  
**Canonical content lives at:** `docs/04_Application/UI_Flows.md`

---

## Why this folder exists

Pairs with `15_UI_Architecture` and `16_Design_System` so the product stack is visible at docs root:

```text
15 UI Architecture  → structure (module / workspace / screen family)
16 Design System    → visual & interaction standards
17 User Flows       → how users move through screens to complete jobs
14 Implementation   → TASKs that build slices
```

**Do not fork a second flow catalog here** until a flow outgrows `UI_Flows.md`. Prefer adding module-specific flow files under this folder that link back to Screen Catalog + UI Architecture.

---

## Start here

| Topic | Path |
|-------|------|
| UI Flows (canonical) | [`../04_Application/UI_Flows.md`](../04_Application/UI_Flows.md) |
| Screen Catalog | [`../04_Application/Screen_Catalog.md`](../04_Application/Screen_Catalog.md) |
| Production IA | [`../15_UI_Architecture/Production/`](../15_UI_Architecture/Production/) |

---

## Future expansion (optional files)

When needed:

```text
17_User_Flows/
  Production_Order_Release.md
  NCR_to_CAPA.md
  Asset_Work_Order.md
```

Each flow doc must name screens from UI Architecture, not TASKs as steps.

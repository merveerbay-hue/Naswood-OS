# Screen → Historical TASK Mapping (Archive)

**Status:** Archive / migration aid  
**Note:** `docs/14_Implementation` is **FROZEN**. Do not create new TASK files.

---

## Rule

Product delivery is driven by:

```text
UI Architecture → Navigation → Screen Architecture → User Flow → Frontend
```

Historical Implementation TASKs may still be referenced to understand what an
old sprint intended. They do **not** invent navigation and do **not** authorize
new CRUD screens.

---

## If you find a TASK-* file

1. Map it to Module / Workspace / Screen IDs in `15_UI`
2. Implement the **screen / workspace**, naming those IDs in the PR
3. Do not write a successor TASK document

---

## Examples (historical)

### TASK-046 — BOM → screens

| Field | Value |
|-------|--------|
| Module | Production |
| Workspace | Master Data |
| Screens | PRD-002 BOM List, PRD-003 BOM Detail (+ create action) |
| IA doc | `15_UI_Architecture/Production/BOM.md` |

### TASK-056 — Production Order → screens

| Field | Value |
|-------|--------|
| Module | Production |
| Workspace | Planning |
| Screens | PRD-010 List, PRD-011 Detail |
| Flow | `17_User_Flows/Production_Flow.md` |

### TASK-078 era — Asset → screens

| Field | Value |
|-------|--------|
| Module | Maintenance |
| Workspace | Assets |
| Screens | MNT Asset Explorer / Detail family (`15_UI/Maintenance`) |
| Flow | `17_User_Flows/Maintenance_Flow.md` |

---

## Mapping card (for PRs — not for new TASK files)

```markdown
## Product Mapping

- Module:
- Workspace:
- Screens in scope:
- Screens deferred:
- User flow:
- Navigation entries:
- Components:
```

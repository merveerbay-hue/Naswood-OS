# Navigation Permissions

**Status:** Active  
**Depends on:** Platform Identity / RBAC (`03_PLATFORM`, Identity module)

---

## Model

```text
Permission
  → Menu visibility (can see nav item)
  → Screen open (can route to screen)
  → Actions on screen (Release, Confirm, Approve, …)
```

Menu visibility alone is insufficient. A role may see Production Orders List
but not Release; Detail actions must still be gated.

---

## Mapping rules

1. Permissions are named by **capability / screen action**, not by TASK id.
2. Prefer stable codes aligned with Screen Architecture, e.g.:
   - `production.orders.read`
   - `production.orders.release`
   - `maintenance.assets.write`
   - `quality.ncr.approve`
3. Administrator receives the full catalog; other roles receive workspace subsets.
4. Hidden nav ≠ unauthorized API — APIs must enforce the same codes.

---

## Workspace presets (examples)

| Role | Typical modules |
|------|-----------------|
| Production Planner | Production / Planning + Master Data (read) |
| Shop-floor Operator | Production / Execution terminals |
| Quality Inspector | Quality inspections + NCR create |
| Maintenance Technician | Maintenance Work Orders + Assets (read) |
| Warehouse Operator | Inventory movements + scanner flows |

---

## Agent rule

When adding a screen, declare required permission codes in the screen PRD and
register them in the permission catalog — do not invent ad-hoc menu flags in React only.

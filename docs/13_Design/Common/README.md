# Common Design — Cross-module UX laws

**Status:** Official  
**SSOT:** [`docs/00_Product/DOCUMENTATION_AUTHORITY_MATRIX.md`](../../00_Product/DOCUMENTATION_AUTHORITY_MATRIX.md)

| Document | Owns |
|----------|------|
| [`Screen_Types.md`](./Screen_Types.md) | Which screen type a job uses; **no shared Create** |
| [`UI_Patterns.md`](./UI_Patterns.md) | Anatomy / behavior of each pattern |

## Absolute UX law

```text
NOS'ta "New" diye tek tip ekran yoktur.
Her iş süreci kendi ekran tipini kullanır
(Wizard, Console, Explorer, Planner, Dashboard, Workbench, Approval Center, Terminal).
Bileşenler yeniden kullanılır; akış ve davranış sürece özgüdür.
```

Before any “Yeni” button in FE: open **Screen_Types**, then the module process Wizard/Terminal PRD.

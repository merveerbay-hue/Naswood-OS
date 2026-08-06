# INV-CNT-001 — Cycle Count Session

**Module:** Inventory · **Workspace:** Counts & Adjustments  
**Screen type:** Wizard (session) — may use Terminal for scan lines  
**Replaces:** “Yeni sayım” / Create Inventory Count form

## Job to be done

> Sayım görevlisi bir **sayım oturumunu** bitirir: sayılan miktarları kaydeder, farkı görür, politikaya göre onaylayıp kapatır.

**Not the job:** “Create an InventoryCount entity.”

## CTA

**Start count** / **Sayım başlat** — never “Yeni sayım.”

## Steps

```text
1. Scope seç (WH / zone / ABC / list)
2. Session aç (Numbering: count document series)
3. Satırları say (scan Terminal opsiyonel)
4. Farkları gözden geçir
5. Onay (policy) → Post adjustment / Close session
```

## Gates

- Scope non-empty.  
- Blind count policy when configured.  
- Variance above threshold → Approval Center.  
- Post updates balance only after approval gate.

## Related

`Inventory_Screens.md` · FLOW-INV-004 · `Document_Numbering.md`

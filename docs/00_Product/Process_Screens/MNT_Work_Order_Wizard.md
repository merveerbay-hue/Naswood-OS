# MNT-WO-001 — Maintenance Work Order Wizard

**Module:** Maintenance · **Workspace:** Work Management  
**Screen type:** Wizard — `Screen_Types.md`  
**Replaces:** “Yeni iş emri” / Create Maintenance Order form

## Job to be done

> Bakım planlayıcı / süpervizör, varlık için **iş emrini** açar: arıza/PM, öncelik, teknisyen, yedek parça, plan; onay sonrası sahaya verir.

**Not the job:** “Create a WorkOrder entity.”

## CTA

**Open work order** / **İş emri aç** — never “Yeni iş emri.”

## Steps

```text
1. Asset / makine seç
2. Tip (Breakdown · PM · Improvement)
3. Arıza / semptom (veya PM checklist)
4. Öncelik + hedef süre
5. Teknisyen / ekip
6. Yedek parça ihtiyacı (Inventory rezervasyon handoff)
7. Plan (tarih / vardiya)
8. Onay / Release to technician
```

## Gates

- Asset required.  
- Breakdown requires symptom/code.  
- Spare parts → Inventory reservation policy.  
- WO number via Numbering Service.

## Related

`Maintenance_Screens.md` · `NOS_SCREEN_MAP.md` § Maintenance · `Document_Numbering.md`

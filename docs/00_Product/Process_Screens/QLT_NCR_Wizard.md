# QLT-NCR-001 — NCR Wizard (Raise Non-Conformance)

**Module:** Quality · **Workspace:** Operations  
**Screen type:** Wizard — `Screen_Types.md`  
**Replaces:** “Yeni NCR” / Create NCR form

## Job to be done

> Kalite / operatör, uygunsuzluğu **kaynak → ürün/lot → problem → kanıt** ile kaydeder; disposition ve CAPA yolunu başlatır.

**Not the job:** “Create an NCR row.”

## CTA

**Raise NCR** / **NCR aç** — never “Yeni NCR.”

## Authority references

| Topic | Authority |
|-------|-----------|
| NCR number | `Document_Numbering.md` — mint only; manual entry prohibited |
| Process | `Quality_Workflow.md` |
| Screen type | `Screen_Types.md` |

## Steps

```text
1. Kaynak (Incoming · In-process · Final · Customer · Audit)
2. Ürün / malzeme
3. Lot / seri (mevcut kimlik — seçim; yeni mint yok)
4. Problem kodu + açıklama
5. Kanıt (foto / dosya — Platform File Upload)
6. Cidrigiyet / hold önerisi
7. Root cause (draft veya sonra)
8. CAPA bağla / oluştur (opsiyonel bu turda)
9. Submit → Approval / Disposition
```

## Gates

- Source + material + problem required.  
- Lot/serial when material controlled.  
- Inventory hold may auto-trigger on severity.  
- NCR ID via Numbering Service only.

## Related

`Quality_Screens.md` · `Quality_Workflow.md` · `NOS_SCREEN_MAP.md` § Quality

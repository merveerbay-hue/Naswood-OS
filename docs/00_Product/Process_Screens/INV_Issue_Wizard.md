# INV-ISS-001 — Issue Goods Wizard

**Module:** Inventory · **Workspace:** Operations  
**Screen type:** Wizard — `Screen_Types.md`  
**Replaces:** “Yeni çıkış” / Create Goods Issue form

## Job to be done

> Operatör, talebe (üretim / satış / manuel) karşı stoğu **seçilen lot/seri** ile çıkarır ve **Post** ile bakiyeyi düşer.

**Not the job:** “Create a GoodsIssue row.”

## CTA

**Issue goods** / **Mal çıkışı** — never “Yeni çıkış.”

## Steps

```text
1. Talep / referans seç (Production · Sales · Manual)
2. Satırlar & açık miktar
3. Kaynak depo / lokasyon
4. Lot / seri seç (mevcut — Numbering mint yok)
5. Miktar doğrula (≤ available / reserved)
6. Post
```

## Gates

- Reference or explicit manual policy.  
- Qty > 0 and ≤ available (policy).  
- Lot/serial required when material is lot/serial-controlled.  
- Reservation cleared on post when applicable.

## Related

`Inventory_Screens.md` · `Inventory_Workflow.md` · FLOW-INV-002 · `Document_Numbering.md` (identity rules only)

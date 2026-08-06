# INV-RCV-001 — Receiving Wizard (Goods Receipt)

**Module:** Inventory  
**Workspace:** Operations  
**Screen type:** Wizard — see `docs/13_Design/Common/Screen_Types.md`  
**Status:** Product Architect draft  
**Replaces:** “Yeni Goods Receipt” / shared Create form

---

## Job to be done

> Warehouse finishes inbound receipt so stock is posted (or held for quality) with correct lot, WH, location, and labels.

**Not the job:** “Create a GoodsReceipt row.”

---

## CTA

**Receive goods** / **Mal kabul başlat** — never generic “Yeni” / “Create.”

---

## Steps

```text
1. PO Seç
2. Bekleyen Satırlar
3. Teslim Miktarı
4. Lot Oluştur          → NOS Numbering Service (Document_Numbering.md) — manual entry prohibited
5. Kalite Kararı
6. Depo
7. Lokasyon
8. Etiket
9. Post
```

---

## Gates

- PO / reference required (or explicit manual inbound policy).  
- Qty > 0; serialized materials require serial mint/select.  
- Quality decision may force hold (Inventory hold via Inventory Architecture — not local stock edit).  
- Post requires WH + Location.

---

## Finish action

**Post** (not Save). Optional **Save draft** before Post.

---

## Related

- `Inventory_Screens.md` · `Inventory_Workflow.md` · `Inventory_User_Flows.md` FLOW-INV-001  
- Pattern anatomy: `docs/13_Design/Common/UI_Patterns.md` § Wizard  
- Production contrast: `PRD_Production_Planning_Wizard.md`

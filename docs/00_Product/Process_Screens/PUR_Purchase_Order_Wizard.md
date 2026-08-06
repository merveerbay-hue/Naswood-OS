# PUR-PO-001 — Purchase Order Wizard

**Module:** Purchasing · **Workspace:** Orders  
**Screen type:** Wizard — `Screen_Types.md`  
**Replaces:** “Create Purchase Order” / “Yeni PO” shared form

## Job to be done

> Satınalmacı, tedarikçiye **siparişi** satırları, teslim ve onay kapılarıyla verir; Release sonrası mal kabul bekler.

**Not the job:** “Create a PurchaseOrder row.”

## CTA

**Place purchase order** / **Sipariş ver** — never “Create PO” / “Yeni.”

## Steps

```text
1. Tedarikçi seç
2. Kaynak (PR / RFQ kazanan / manuel)
3. Satırlar (malzeme · miktar · fiyat · termin)
4. Teslim depo / plant
5. Koşullar / Incoterms
6. Submit for approval (policy)
7. Release to supplier
```

## Gates

- Supplier + ≥1 line.  
- Price / budget policy may force Approval Center.  
- PO number via Numbering Service.  
- Goods receipt later → Receiving Workbench (Inventory) — not this screen.

## Related

`Purchasing_Screens.md` · `Purchasing_Workflow.md` · `INV_Receiving_Workbench.md`

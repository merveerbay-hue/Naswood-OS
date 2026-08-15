# SAL-SO-001 — Sales Order Wizard

**Module:** Sales · **Workspace:** Orders  
**Screen type:** Wizard — `Screen_Types.md`  
**Replaces:** “+ New Sales Order” / Create Sales Order form

## Job to be done

> Satışçı, müşteri siparişini **satır → stok/termin kontrolü → rezervasyon → Release** ile bitirir.

**Not the job:** “Create a SalesOrder row.”

## CTA

**Enter sales order** / **Sipariş gir** — never “+ New Sales Order.”

## Steps

```text
1. Müşteri seç
2. Kaynak (Quotation · Manual · EDI)
3. Satırlar (ürün · miktar · fiyat)
4. Availability / ATP kontrolü
5. Rezervasyon (Inventory handoff)
6. Termin / sevkiyat adresi
7. Submit / Release
```

## Gates

- Customer + ≥1 line.  
- Credit / price policy may force Approval Center.  
- SO number via Numbering Service.  
- Shipment is a **separate job** (Plan shipment) — not this wizard’s finish.

## Related

`Sales_Screens.md` · `Sales_Workflow.md` · `NOS_SCREEN_MAP.md` § Sales

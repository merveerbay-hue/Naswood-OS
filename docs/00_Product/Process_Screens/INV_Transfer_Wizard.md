# INV-TRF-001 — Transfer Stock Wizard

**Module:** Inventory · **Workspace:** Operations  
**Screen type:** Wizard — `Screen_Types.md`  
**Replaces:** “Yeni transfer” / Create Stock Transfer form

## Job to be done

> Operatör, stoğu **kaynak → hedef** depo/lokasyona taşır; net kayıp yok; **Post** ile bakiyeler hareket eder.

**Not the job:** “Create a StockTransfer row.”

## CTA

**Transfer stock** / **Stok transfer** — never “Yeni transfer.”

## Steps

```text
1. Malzeme (+ lot/seri gerekirse)
2. Kaynak depo / lokasyon
3. Hedef depo / lokasyon
4. Miktar
5. Post
```

## Gates

- From ≠ To (WH or location).  
- Qty ≤ available at source.  
- Cross-plant only with policy / approval.  
- Lot/serial follow material rules.

## Related

`Inventory_Screens.md` · `Inventory_Workflow.md` · FLOW-INV-003

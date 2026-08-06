# Chain of Custody Architecture (FSC / PEFC)

**Document:** Chain of Custody Architecture  
**Status:** Official — Product Architect  
**Version:** 1.0.0  
**Location:** `docs/13_Design/99_Shared/Chain_of_Custody_Architecture.md`  
**Owns:** FSC / PEFC CoC continuity laws across Receiving → Inventory → Production → Shipping · certificate evidence obligations · claim category binding on Material Definition · prohibition of breaking MI/Package links that carry CoC  
**Does not own:** Certificate print layouts · Sales commercial claims copy · Quality inspection sampling algorithms · Numbering formats · Inventory stock ledger

---

## Strategic intent

```text
CoC is not a Quality-only checkbox.
Every movement that touches certified material must preserve
Supplier → Receiving → MI → Package → Process → FG → Customer
without silent identity overwrite.
```

Quality module is the **primary steward** of CoC controls; Inventory / Production / Shipping **execute** continuity.

---

## Absolute laws

```text
1. CoC-relevant Material Definitions bind claim category + certificate expectations.
2. Receiving captures certificate evidence into the digital file (Evidence Archive).
3. Material Identity genealogy is the CoC graph backbone (nodes = MI).
4. Package Identity changes only via policy split with parent→child — never silent rename.
5. Mixing incompatible claim categories requires controlled downgrade / segregation (policy).
6. Shipments that assert FSC/PEFC claims must prove unbroken chain + certificates on file.
7. Compliance Architecture covers audit trail / revision / exports for CoC records.
```

---

## Module roles

| Module | Role |
|--------|------|
| **Quality** | CoC rules · certificate records · claim validation · NCR on break |
| **Inventory** | Preserve MI/Package links on GR/GI/Transfer |
| **Production** | Transformation mints new MI with parent links |
| **Purchasing** | Supplier certificate intake |
| **Sales / Shipping** | Claim on order/shipment only if chain intact |

---

## Composition

| Topic | Authority |
|-------|-----------|
| Compliance spine | `Compliance_Architecture.md` |
| Material Definition bindings | `Material_Definition_Architecture.md` |
| Material Identity / genealogy | `Material_Identity_Architecture.md` · `Material_Genealogy.md` |
| Evidence archive | `Document_Management_Evidence_and_Export.md` |
| Quality module | `Quality_Architecture.md` · Foundation Program |

---

## Related

`Quality_Foundation_Program.md` · `Compliance_Architecture.md` · `Material_Definition_Architecture.md` · `Material_Identity_Architecture.md` · `INV_Receiving_Workbench.md`

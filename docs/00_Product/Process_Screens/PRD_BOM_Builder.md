# PRD-501 — BOM Builder

**Module:** Production  
**Workspace:** Engineering (Master Data)  
**Screen type:** Builder — `docs/13_Design/Common/Screen_Types.md` § 2b · § 3a  
**Status:** Product Architect draft  
**Replaces:** “Yeni BOM” / Code · Description · Save CRUD

---

## Job to be done

> Üretim mühendisi, ürün revizyonu için **malzeme ağacını** kurar; alternatifleri, fireyi ve operasyon bağlarını doğrular; **Onay → Release** ile yayınlar.

**Not the job:** Create a BOM header row.

---

## CTA

**Build BOM** / **BOM oluştur** — never “Yeni BOM.”

---

## Steps

```text
1. Ürün seç
2. Revizyon
3. Malzeme ağacı (tree builder)
4. Alternatif malzemeler
5. Fire / scrap factors
6. Operasyon bağlantıları (→ Routing / Operation Designer)
7. Onay
8. Release
```

---

## Gates

- Product + revision required.  
- Tree ≥1 component (policy).  
- Quantities / UoM valid.  
- Alternatives must resolve to active materials.  
- BOM number / revision via Numbering — manual entry prohibited.  
- Release may require Approval Center by policy.

---

## Related

- `Production_Screens.md` PRD-501  
- Planning Wizard consumes Released BOMs  
- `PRD_Machine_Configuration.md` · Routing Designer for capability checks  
- `Document_Numbering.md`

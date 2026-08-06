# PRD-501 — BOM Builder

**Module:** Production  
**Workspace:** Engineering  
**Screen type:** Builder — `Screen_Types.md` § 2b · § 3a  
**Status:** Product Architect draft  
**Replaces:** “Yeni BOM” / Code · Description · Save CRUD  
**Principle:** `Production_Screens.md` § ENGINEERING MASTER DATA PRINCIPLE

---

## Job to be done

> Üretim mühendisi, ürün revizyonu için **malzeme ağacını** kurar; alternatif, fire, operasyon bağları, **versiyon karşılaştırma** ve **etki analizi** ile doğrular; **Onay → Release** eder.

**Not the job:** Create a BOM header with a Code field.

---

## CTA

**Build BOM** / **BOM oluştur** — never “Yeni BOM.”

---

## Identifier

| | |
|--|--|
| User enters | Ürün (name-first) · Revizyon · tree / business data |
| System assigns | `BOM-…` via Numbering Service |
| UI | No `Code *` input — “System Code — automatically generated” then read-only `BOM-000845` |

Authority: `Document_Numbering.md` § System Generated Identifiers.

---

## Steps

```text
BOM Builder
  → Ürün Seç
  → Revizyon
  → Malzeme Ağacı (Tree)
  → Alternatif Malzemeler
  → Fire Oranları
  → Operasyon Bağlantıları
  → Versiyon Karşılaştırma
  → Etki Analizi
  → Onay
  → Release
```

| Step | Intent |
|------|--------|
| Ürün Seç | Name-first product picker (not Product Code) |
| Revizyon | Target engineering revision |
| Malzeme Ağacı | Tree builder — qty, UoM, scrap per node |
| Alternatifler | Substitute materials / preference |
| Fire | Scrap / yield factors |
| Operasyon bağları | Link components to operations / routing steps |
| Versiyon karşılaştırma | Diff vs previous released BOM |
| Etki analizi | Where-used: open orders, stock, cost impact |
| Onay → Release | Approval Center policy → publish for Planning |

---

## Gates

- Product + revision required.  
- Tree ≥1 component (policy).  
- Quantities / UoM valid; alternatives resolve to active materials.  
- Impact analysis acknowledged when open demand exists (policy).  
- `BOM-…` via Numbering only — manual entry prohibited.  
- Release may require Approval Center.

---

## Related

- `Production_Screens.md` PRD-501 · ENGINEERING MASTER DATA PRINCIPLE  
- Planning Wizard consumes **Released** BOMs  
- Routing Designer · Machine Configuration Studio  
- `Document_Numbering.md`

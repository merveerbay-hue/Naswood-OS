# PRD-503 — Machine Configuration

**Module:** Production  
**Workspace:** Engineering (Master Data)  
**Screen type:** Configuration — `docs/13_Design/Common/Screen_Types.md` § 2b · § 3a  
**Status:** Product Architect draft  
**Replaces:** “Yeni makine” / Machine Code · Name · Save CRUD

---

## Job to be done

> Üretim mühendisi, makineyi **kimlik · yerleşim · teknik · üretim yeteneği · bakım · doküman** ile tanımlar; doğrular ve **Release** eder — böylece planlama ve icra doğru makineyi kullanır.

**Not the job:** Create a Machine row with two fields and Save.

---

## CTA

**Configure machine** / **Makine yapılandır** — never “Yeni makine.”

Library (find & reopen) → opens this Configuration. Library is not a Create Form.

---

## Why not one page

A real machine definition does not fit Code · Name · Save. Facets:

| Facet | Content |
|-------|---------|
| **Kimlik** | Makine **adı** · tip · grup · üretici · model · seri no · asset kodu · (system) machine code via Numbering — **never a Code * input**; show “Otomatik atanacak” then `MC-…` |
| **Yerleşim** | Fabrika · bina · hat · work center · pozisyon |
| **Teknik** | Eksen sayısı · max en / boy / kalınlık · devir · güç · voltaj |
| **Üretim** | Yapabildiği operasyonlar · desteklenen ağaç türleri · ürünler · tool magazine · setup süresi · cycle time |
| **Bakım** | PM planı · yağlama · sensörler · sayaçlar (Maintenance handoff) |
| **Doküman** | PDF · manual · CAD · fotoğraf (Platform File Upload — typed roles) |

Finish: **Validate** → **Release** (not bare Save). Draft allowed; Released machines appear in Routing Designer / Planning.

---

## Gates

- Identity facet complete (type, model or equivalent policy).  
- Plant / placement required before Release.  
- Capability (≥1 operation or explicit “utility only”) before use in Routing.  
- Machine ID via Numbering Service — manual entry prohibited.  
- Documents optional but linked with roles when present.

---

## Related

- `Production_Screens.md` PRD-503  
- `Screen_Types.md` § Master Data ≠ Create Form  
- Asset link → Maintenance  
- Numbering: `Document_Numbering.md`

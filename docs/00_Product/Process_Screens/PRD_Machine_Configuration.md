# PRD-503 — Machine Configuration Studio

**Module:** Production  
**Workspace:** Engineering  
**Screen type:** Configuration (Studio) — `Screen_Types.md` § 2b · § 3a  
**Status:** Product Architect draft  
**Replaces:** “Yeni makine” / Machine Code · Name · Save CRUD  
**Principle:** `Production_Screens.md` § ENGINEERING MASTER DATA PRINCIPLE

---

## Job to be done

> Üretim mühendisi, makineyi **genel · teknik · kapasite · operasyon · tool magazine · bakım · sensör · doküman · IoT · devreye alma** ile yapılandırır; Validate → Release — planlama ve icra doğru makineyi kullanır.

**Not the job:** Type Machine Code and Save.

---

## CTA

**Configure machine** / **Makine yapılandır** — never “Yeni makine.”  
Library → opens this Studio (Library is not a Create Form).

---

## Identifier

| | |
|--|--|
| User enters | Makine **Adı** · Üretici · Model · tip · grup · seri · asset · … |
| System assigns | `MC-…` (e.g. `MC-000124`) via Numbering Service on save |
| UI | No `Code *` — badge “Otomatik atanacak” → read-only `MC-000124` |

Authority: `Document_Numbering.md` § System Generated Identifiers · Constitution § 2.3.

---

## Studio facets

```text
Machine Configuration Studio
  → Genel Bilgiler
  → Teknik Özellikler
  → Kapasiteler
  → Desteklenen Operasyonlar
  → Tool Magazine
  → Bakım
  → Sensörler
  → Dokümanlar
  → IoT
  → Devreye Alma
  → Validate → Release
```

| Facet | Content (examples) |
|-------|---------------------|
| **Genel Bilgiler** | Ad · tip · grup · üretici · model · seri no · asset — **no Code field** |
| **Teknik Özellikler** | Eksen · güç · voltaj · … |
| **Kapasiteler** | Max en / boy / kalınlık · devir · … |
| **Desteklenen Operasyonlar** | Ops · ağaç türleri · ürün aileleri · setup · cycle |
| **Tool Magazine** | Slots · compatible tools (`TL-…` from Tool Library) |
| **Bakım** | PM planı · yağlama (Maintenance handoff) |
| **Sensörler** | Counters · signal map |
| **Dokümanlar** | PDF · Manual · CAD · Foto (Platform File Upload) |
| **IoT** | Edge / tag bindings |
| **Devreye Alma** | Commission checklist → Active |

Finish: **Validate** → **Release** (not bare Save as the product outcome). Released machines appear in Routing Designer / Planning.

---

## Gates

- Genel Bilgiler: name + manufacturer/model (or policy equivalent).  
- Plant / placement before Release when required.  
- ≥1 supported operation or explicit utility-only flag.  
- `MC-…` via Numbering only.  
- Commission complete before shop-floor Active (policy).

---

## Related

- `Production_Screens.md` PRD-503 · ENGINEERING MASTER DATA PRINCIPLE  
- Asset / PM → Maintenance  
- Tool Library Manager · Routing Designer  
- `Document_Numbering.md`

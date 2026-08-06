# INV-RCV-001 — Receiving Wizard (Goods Receipt) — **rules spine**

**Module:** Inventory  
**Workspace:** Operations  
**Status:** Rules retained · **UX authority moved**  
**Primary UX:** [`INV_Receiving_Workbench.md`](./INV_Receiving_Workbench.md) — **Receiving Workbench** (not a Create form)  
**Material Identity:** [`Material_Identity_Architecture.md`](../../13_Design/99_Shared/Material_Identity_Architecture.md)

---

## Supersession notice

```text
Product UX for inbound receiving is the Receiving Workbench
(docs/00_Product/Process_Screens/INV_Receiving_Workbench.md).

This file keeps the Depo → Location → Material Identity (+ Lot) → QI → Post gates
so Workflow / Numbering consumers can still cite a short spine.
Do NOT implement a standalone CRUD “Create Goods Receipt” from this file.
```

**CTA:** Receive goods / Mal kabul başlat → opens **Receiving Workbench**.  
**Screen type:** Workbench (stages include former Wizard steps).

---

## Job to be done

> Depocu, gelen malı **seçtiği depoya / lokasyona** kabul eder; **kök Material Identity** (sınıf bilinçli, örn. LOG) otomatik oluşur; Lot operasyonel parti olarak bağlanabilir; kalite ve etiket adımlarından sonra **Post** ile stoğa işler.

**Not the job:** “Create a GoodsReceipt row” or type Material Identity / Lot by hand.

---

## Authority references

| Topic | Authority |
|-------|-----------|
| Full Workbench UX | [`INV_Receiving_Workbench.md`](./INV_Receiving_Workbench.md) |
| Material Identity (root · vs Lot) | `Material_Identity_Architecture.md` |
| MI / Lot / GR formats | `Document_Numbering.md` |
| Stock posting | `Inventory_Architecture.md` / `Inventory_Workflow.md` |
| Screen type | `Screen_Types.md` · `UI_Patterns.md` |

```text
Identifiers are generated automatically according to the centralized
Numbering Architecture (docs/13_Design/99_Shared/Document_Numbering.md).
Manual entry is prohibited.
```

---

## Spine steps (embedded in Workbench stages 4–10)

```text
Reference (PO) / lines / qty     → Workbench stages 2–5
Select warehouse (Depo)          → Stage 7 — required; operator chooses
Location in that warehouse       → Stage 7
Material Identity mint (root)    → Stage 8 — class-aware Numbering; read-only
Optional Lot (operational)       → Stage 8 — not a substitute for MI
Quality decision                 → Stage 6 / QI
Label                            → Stage 8
Review → Post                    → Stages 9–10 — genealogy root + stock
```

Depo, Material Identity’den **önce** seçilir — stok hedefi net olmadan kimlik basılmaz / post edilmez.

---

### Depo seç (retained)

| | |
|--|--|
| **Intent** | Malın gireceği **depoyu** kullanıcı seçsin. |
| **Inputs** | Warehouse list (plant-filtered; Active only) — **name-first** |
| **Defaults** | PO line default WH if any; else last-used WH; else system suggestion (Storage Rules) |
| **Gate** | Warehouse required before Location / MI / Post |
| **Not** | Hard-coded single warehouse; Warehouse Code typed by hand |

---

### Lokasyon seç (retained)

| | |
|--|--|
| **Intent** | Seçilen depo içinde göz / bölge. |
| **Inputs** | Locations **for selected warehouse only** |
| **Gate** | Location required (unless WH policy = WH-level balance only) |

---

### Material Identity oluştur (kök — mandatory)

| | |
|--|--|
| **Intent** | Her kabul satırı için **kök Material Identity** — malzeme sınıfına göre (örn. LOG). |
| **System** | Numbering Service + Identity Rules — `Document_Numbering.md` § Material Identity series · `Material_Identity_Architecture.md` |
| **UI** | **Read-only** Material Identity — never free-text |
| **Also** | Optional Lot (operational); GR document `GR-…` separately |
| **Not** | Generic sequential-only ID without material class; treating Lot as lifelong identity |

---

## Finish action

**Post** (not Save). Creates genealogy **root** + stock. Draft save allowed in Workbench without stock mutation.

---

## Related

- **[`INV_Receiving_Workbench.md`](./INV_Receiving_Workbench.md)** — authoritative receiving UX  
- `Material_Identity_Architecture.md` · `Document_Numbering.md` · `Material_Genealogy.md`  
- `Inventory_Screens.md` · `Inventory_Workflow.md` · `Inventory_User_Flows.md` FLOW-INV-001

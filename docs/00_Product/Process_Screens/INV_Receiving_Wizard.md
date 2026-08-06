# INV-RCV-001 — Receiving Wizard (Goods Receipt) — **rules spine**

**Module:** Inventory  
**Workspace:** Operations  
**Status:** Rules retained · **UX authority moved**  
**Primary UX:** [`INV_Receiving_Workbench.md`](./INV_Receiving_Workbench.md) — **Receiving Workbench** (not a Create form)

---

## Supersession notice

```text
Product UX for inbound receiving is the Receiving Workbench
(docs/00_Product/Process_Screens/INV_Receiving_Workbench.md).

This file keeps the Depo → Location → Lot mint → QI → Post gates
so Workflow / Numbering consumers can still cite a short spine.
Do NOT implement a standalone CRUD “Create Goods Receipt” from this file.
```

**CTA:** Receive goods / Mal kabul başlat → opens **Receiving Workbench**.  
**Screen type:** Workbench (stages include former Wizard steps).

---

## Job to be done

> Depocu, gelen malı **seçtiği depoya / lokasyona** kabul eder; lot numarası **malzeme cinsine göre otomatik** oluşur; kalite ve etiket adımlarından sonra **Post** ile stoğa işler.

**Not the job:** “Create a GoodsReceipt row” or type a lot number by hand.

---

## Authority references

| Topic | Authority |
|-------|-----------|
| Full Workbench UX (truck, OCR, verify, count, inspect, labels, review) | [`INV_Receiving_Workbench.md`](./INV_Receiving_Workbench.md) |
| Lot / GR numbers | `docs/13_Design/99_Shared/Document_Numbering.md` — *Lot series by material category*; manual lot entry **prohibited** |
| Stock posting | `Inventory_Architecture.md` / `Inventory_Workflow.md` |
| Screen type | `docs/13_Design/Common/Screen_Types.md` · `UI_Patterns.md` |

```text
Identifiers are generated automatically according to the centralized
Numbering Architecture (docs/13_Design/99_Shared/Document_Numbering.md).
Manual entry is prohibited.
```

---

## Spine steps (embedded in Workbench stages 4–10)

Former linear Wizard — now **gates inside Workbench**, not a Create form:

```text
Reference (PO) / lines / qty     → Workbench stages 2–5
Select warehouse (Depo)          → Stage 7 — required; operator chooses
Location in that warehouse       → Stage 7
Lot mint by material category    → Stage 8 — Numbering Service; read-only
Quality decision                 → Stage 6 / QI
Label                            → Stage 8
Review → Post                    → Stages 9–10
```

Depo, lot’tan **önce** seçilir — stok hedefi net olmadan kimlik basılmaz / post edilmez.

---

### Depo seç (retained)

| | |
|--|--|
| **Intent** | Malın gireceği **depoyu** kullanıcı seçsin. |
| **Inputs** | Warehouse list (plant-filtered; Active only) — **name-first** |
| **Defaults** | PO line default WH if any; else last-used WH; else system suggestion (Storage Rules) |
| **Gate** | Warehouse required before Location / Lot / Post |
| **Not** | Hard-coded single warehouse; Warehouse Code typed by hand |

---

### Lokasyon seç (retained)

| | |
|--|--|
| **Intent** | Seçilen depo içinde göz / bölge. |
| **Inputs** | Locations **for selected warehouse only** |
| **Gate** | Location required (unless WH policy = WH-level balance only) |

---

### Lot oluştur (otomatik — retained)

| | |
|--|--|
| **Intent** | Her kabul satırı için yeni lot kimliği **malzeme kategorisine** göre. |
| **System** | Numbering Service — see `Document_Numbering.md` § Lot series by material category |
| **UI** | **Read-only** Lot ID — never free-text Lot No |
| **Also** | GR document number minted as Goods Receipt series |

---

## Finish action

**Post** (not Save). Draft save allowed in Workbench without stock mutation.

---

## Related

- **[`INV_Receiving_Workbench.md`](./INV_Receiving_Workbench.md)** — authoritative receiving UX  
- `Inventory_Screens.md` · `Inventory_Workflow.md` · `Inventory_User_Flows.md` FLOW-INV-001  
- `Document_Numbering.md`

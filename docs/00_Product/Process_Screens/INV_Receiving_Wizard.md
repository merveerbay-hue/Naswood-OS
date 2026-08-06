# INV-RCV-001 — Receiving Wizard (Goods Receipt)

**Module:** Inventory  
**Workspace:** Operations  
**Screen type:** Wizard — see `docs/13_Design/Common/Screen_Types.md`  
**Status:** Product Architect draft  
**Replaces:** “Yeni Goods Receipt” / shared Create form

---

## Job to be done

> Depocu, gelen malı **seçtiği depoya / lokasyona** kabul eder; lot numarası **malzeme cinsine göre otomatik** oluşur; kalite ve etiket adımlarından sonra **Post** ile stoğa işler.

**Not the job:** “Create a GoodsReceipt row” or type a lot number by hand.

---

## CTA

**Receive goods** / **Mal kabul başlat** — never generic “Yeni” / “Create.”

---

## Authority references

| Topic | Authority |
|-------|-----------|
| Lot / GR numbers | `docs/13_Design/99_Shared/Document_Numbering.md` — *Lot series by material category*; manual lot entry **prohibited** |
| Stock posting | `Inventory_Architecture.md` / `Inventory_Workflow.md` |
| Screen type | `docs/13_Design/Common/Screen_Types.md` · `UI_Patterns.md` |

```text
Material, Lot, Serial, Package, Pallet and Production identifiers
are generated exclusively by the NOS Numbering Service as defined
in Document_Numbering.md. Manual entry is prohibited.
```

---

## Steps

```text
1. PO / referans seç
2. Bekleyen satırlar
3. Teslim miktarı
4. Depo seç                    ← kullanıcı seçer (zorunlu)
5. Lokasyon seç                ← seçilen depoya bağlı
6. Lot oluştur (otomatik)      ← malzeme cinsine göre Numbering Service
7. Kalite kararı
8. Etiket
9. Post
```

Depo, lot’tan **önce** seçilir — stok hedefi net olmadan kimlik basılmaz / post edilmez.

---

### Step 4 — Depo seç

| | |
|--|--|
| **Intent** | Malın gireceği **depoyu** kullanıcı seçsin. |
| **Inputs** | Warehouse list (plant-filtered; Active only) |
| **Defaults** | PO line default WH if any; else last-used WH for user/plant; else empty |
| **Gate** | Warehouse required before Location / Lot / Post |
| **UI** | Warehouse picker (code · name · type); show open capacity hint optional |
| **Not** | Hard-coded single warehouse; silent default without display |

---

### Step 5 — Lokasyon seç

| | |
|--|--|
| **Intent** | Seçilen depo içinde göz / bölge. |
| **Inputs** | Locations **for selected warehouse only** |
| **Gate** | Location required (unless WH policy = WH-level balance only) |
| **UI** | Location picker filtered by Step 4; clear location if WH changes |

---

### Step 6 — Lot oluştur (otomatik, malzeme cinsine göre)

| | |
|--|--|
| **Intent** | Her kabul satırı için yeni lot kimliği **malzeme kategorisine / numbering class’a** göre üretilsin. |
| **Inputs** | Material (from PO line) → Material Category / Numbering class; Company; Plant |
| **System** | Call Numbering Service → series from material class (see Document_Numbering § Lot/Batch series by material category) |
| **UI** | **Read-only** Lot ID (“Otomatik — malzeme cinsine göre”); never free-text Lot No. Material/Warehouse pickers **name-first**; codes display-only (`Document_Numbering.md` § System Generated Identifiers) |
| **Gate** | Mint succeeded; if series missing for category → block with Admin config message |
| **Also** | GR document number (`GR-…`) minted separately as Goods Receipt document series |

Example (illustrative — series config lives in Numbering):

| Malzeme | Cins | Otomatik Lot |
|---------|------|----------------|
| MAT-OAK-RAW | Raw | LOT-RAW-2026-000118 |
| MAT-PINE-LAM | WIP | LOT-WIP-2026-000042 |
| MAT-TW-FIN | Finished | LOT-FG-2026-000077 |

---

### Other steps (short)

| Step | Intent |
|------|--------|
| 1 PO / referans | Hangi sipariş / ASN / manuel giriş |
| 2 Bekleyen satırlar | Hangi satırlar kabul edilecek |
| 3 Teslim miktarı | Fiili miktar (≤ open qty policy) |
| 7 Kalite kararı | Accept / Hold / Reject → may Inventory-hold |
| 8 Etiket | Print labels (Barcode strategy — reference) |
| 9 Post | Stoğa işle — WH + Location + Lot zorunlu |

---

## Gates (summary)

- PO / reference required (or explicit manual inbound policy).  
- Qty > 0.  
- **Warehouse selected by user** (plant-valid).  
- Location valid for that warehouse.  
- **Lot ID auto-minted from material category** — no manual entry.  
- Quality decision may force hold.  
- Serialized materials: Serial also via Numbering Service.

---

## Finish action

**Post** (not Save). Optional **Save draft** after steps 1–5 (lot may mint on draft or at Post — policy; default mint at first save of line with material+WH).

---

## Cursor implementation note

1. Screen type = **Wizard** — not shared Create form.  
2. Warehouse = required select control (step 4).  
3. Lot field = read-only; `NumberingService.MintLot(materialId, company, plant)`.  
4. Do not hardcode prefixes in FE — load series from Numbering config.  
5. Changing material or cancelling line voids unused reserved lot numbers per Numbering reservation rules.

---

## Related

- `Inventory_Screens.md` · `Inventory_Workflow.md` · `Inventory_User_Flows.md` FLOW-INV-001  
- `Document_Numbering.md` § Material & Production Identifiers · Lot series by material category  
- `docs/13_Design/Common/UI_Patterns.md` § Wizard  

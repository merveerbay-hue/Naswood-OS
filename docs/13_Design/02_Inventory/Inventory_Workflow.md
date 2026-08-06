# Inventory Workflow

**Module:** Inventory  
**Version:** 1.0  
**Status:** Active  
**Owns:** Inventory process phases, transaction posting rules, state outcomes.

---

# Authority references

| Topic | Authority |
|-------|-----------|
| Numbering | `Document_Numbering.md` — Material/Lot/Serial/Package/Pallet IDs |
| Stock ownership | `Inventory_Architecture.md` |
| Genealogy | `Material_Genealogy.md` (created on transformations / receipts as applicable) |
| Quality holds | Quality Workflow + this module’s hold transactions |
| UX steps | `Inventory_Screens.md` · `Inventory_User_Flows.md` |

---

# High-level

```text
Master Data (Material, Warehouse, Location)
        │
        ▼
Inbound (GR) ──► Balance ↑
        │
Outbound (GI) ──► Balance ↓
        │
Transfer ──► Balance move
        │
Count / Adjust ──► Balance correct
        │
Reserve / Allocate ──► Soft commit
```

All balance changes are **transactions**. No silent updates.

---

# Goods receipt

**UX authority:** `docs/00_Product/Process_Screens/INV_Receiving_Workbench.md`  
**Spine gates:** `docs/00_Product/Process_Screens/INV_Receiving_Wizard.md` (Depo → Lot)

```text
Truck arrives
→ Receiving Workbench (not shared Create / not CRUD form)
→ Truck registration · Documents · AI OCR · Material verification
→ Physical count (scan / sheet OCR) · Inspection photos
→ Select warehouse (Depo) — required; operator chooses (system may suggest)
→ Location in that warehouse
→ Lot/Serial/Package/Pallet minted via Numbering Service (manual entry prohibited)
→ Labels · Review · Post
→ InventoryBalance update in chosen WH → Available (or Hold)
→ Optional: Quality Incoming Inspection trigger
→ Audit trail + attachments on Receiving record
```

---

# Goods issue

```text
Demand (Production / Sales / Manual)
→ Create GI (Draft)
→ Pick lot/serial (existing)
→ Post → Balance ↓ · Reservation clear
```

---

# Transfer

```text
From WH/Location → To WH/Location
→ Draft → Post → Balances move (same company/plant rules)
```

---

# Cycle count / adjustment

```text
Count Session → Record counted qty
→ Variance → Approval (policy)
→ Adjustment post → Balance correct
```

---

# Reservation

```text
Demand → Reserve qty on balance
→ Issue consumes reservation
→ Cancel releases reservation
```

---

# Document states (canonical)

Draft → Posted → Cancelled  
(Count: Open → InProgress → Review → Posted)

---

# Invariants

1. Posted transactions are immutable; reverse via compensating transaction.  
2. Serialized materials require serial on issue/receipt.  
3. New Lot/Serial/Package/Pallet IDs only from Numbering Service.  
4. Production/Quality never write balances except through Inventory transactions.

---

# Related

`Inventory_Architecture.md` · `Inventory_API.md` · Screen Map Inventory

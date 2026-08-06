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

**UX authority:** `docs/00_Product/Process_Screens/INV_Receiving_Workbench.md` (v1.1 — Evidence First · 14 steps)  
**Spine gates:** `INV_Receiving_Wizard.md` (Depo → Material Identity)  
**Material Identity:** `Material_Identity_Architecture.md`

```text
Truck arrives
→ Receiving Workbench (Evidence First — not Create / not CRUD)
→ Truck registration · Evidence collection (docs/photos = business evidence)
→ AI document understanding · Document comparison
→ Physical count (scan / handwriting OCR) · Photo analysis · Material verification
→ Quality pre-check
→ Warehouse assignment (Depo required; system suggests; operator confirms)
→ Material Identity root mint (class-aware) + optional Lot · Labels
→ Review · Post
→ Genealogy root + InventoryBalance + Evidence Archive + audit
→ Optional: Quality Incoming Inspection trigger
```

---

# Goods issue

**UX authority:** `docs/00_Product/Process_Screens/INV_Goods_Issue_Workbench.md`  
**Spine:** `INV_Issue_Wizard.md`

```text
Business document (Production / Maintenance / Sales / Sample / … / Manual+permission)
→ Goods Issue Workbench (not Create form)
→ Load required materials (no material create)
→ AI recommend WH / location / lot / package (FIFO/FEFO/reservation/quality)
→ Pick (scan) · Verify · Evidence · Quality gate
→ Destination (production line / loading dock)
→ Review · Post
→ InventoryTransaction · balance ↓ · reservation clear · genealogy link · evidence archive
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
3. New **Material Identity** / Lot / Serial / Package / Pallet IDs only from Numbering Service.  
   Material Identity = lifelong genealogy node (`Material_Identity_Architecture.md`); Lot = operational party.  
4. Production/Quality never write balances except through Inventory transactions.

---

# Related

`Inventory_Architecture.md` · `Inventory_API.md` · Screen Map Inventory

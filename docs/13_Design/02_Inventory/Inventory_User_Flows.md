# Inventory User Flows

**Module:** Inventory  
**Version:** 1.0  
**Status:** Active

---

# Authority references

| Topic | Authority |
|-------|-----------|
| Numbering | `Document_Numbering.md` |
| Process truth | `Inventory_Workflow.md` |
| Ownership | `Inventory_Architecture.md` |
| UX screen list | `Inventory_Screens.md` |

When a flow needs a new Lot / Serial / Package / Pallet ID:  
*“Identifier minted by NOS Numbering Service (Document_Numbering.md) — manual entry prohibited.”*

---

# Roles

- Warehouse Operator  
- Inventory Controller  
- Planner (inquiry / reservation)  
- Quality Inspector (hold visibility)  
- Production (issue / FG receipt consumers)

---

# FLOW-INV-001 — Receive goods

**Job:** Finish the **entire** inbound truck operation so stock is available (or held for QI).  
**Authority:** `INV_Receiving_Workbench.md` (Evidence First · 14 steps) · `Material_Identity_Architecture.md` · spine: `INV_Receiving_Wizard.md` · `Document_Numbering.md`

```text
Warehouse Command Center / Operations
→ Receive goods / Mal kabul başlat (Receiving Workbench — not Create form)
→ Truck registration + photos
→ Evidence collection (DN / packing / PO / certificates / camera) — not “attachments”
→ AI document understanding → document comparison (PO∥DN∥packing∥OCR)
→ Physical count (scan / handwriting OCR) · Photo analysis
→ Material verification · Quality pre-check
→ Warehouse assignment (Depo required)
→ Root Material Identity (+ optional Lot) · Labels
→ Review → Post
→ Genealogy root + stock + Evidence Archive · optional QI
```

---

# FLOW-INV-002 — Issue goods

**Job:** Issue materials against a business demand so stock decreases under full control.  
**Authority:** `INV_Goods_Issue_Workbench.md` · spine: `INV_Issue_Wizard.md`

```text
Command Center / Operations
→ Issue goods / Mal çıkışı (Goods Issue Workbench v2.0 — not Create form)
→ Select business document (Production / Maintenance / Sales / … / Manual+permission)
→ Load required materials (no material create)
→ AI recommend → Kabul Et OR Yoksay (Explorer)
→ Scan package · partial qty · AI Validation (also in Override) · Evidence · Quality
→ Destination (line / dock) · Review (+ Override History) · Post
→ Balance ↓ · package remaining updated (+ policy split if configured) · reservation clear · genealogy · evidence archive
```

---

# FLOW-INV-003 — Transfer between locations

**Job:** Move stock without net loss.

```text
Operations → Transfer Stock
→ From / To
→ Post
→ Balances moved
```

---

# FLOW-INV-004 — Cycle count session

**Job:** Finish a count and close differences.

```text
Counts → Cycle Count Session
→ Count lines
→ Review variance
→ Approve → Post Adjustment
→ Balance corrected
```

---

# FLOW-INV-005 — Stock inquiry

**Job:** Answer on-hand / reserved / available by material/lot/location.

```text
Stock → Balance Inquiry
→ Filter → Drill to Lot Trace / Reservations
```

---

# FLOW-INV-006 — Reserve for demand

**Job:** Soft-allocate stock for SO / Production.

```text
Stock → Reservation Desk
→ Create reservation
→ Later Issue consumes it
```

---

# Related

`Inventory_Screens.md` · `Inventory_Dashboard.md` · Screen Map Inventory

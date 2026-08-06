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
**Authority:** `docs/00_Product/Process_Screens/INV_Receiving_Workbench.md` · spine: `INV_Receiving_Wizard.md` · `Document_Numbering.md` § Lot series by material category

```text
Inventory Dashboard / Operations
→ Receive goods / Mal kabul başlat (Receiving Workbench — not Create form)
→ Truck registration + photos
→ Attach documents (DN / packing list / PO / photos)
→ AI OCR → operator review
→ Verify PO ∥ Delivery note ∥ OCR (missing / extra / mismatch)
→ Physical count (scan or handwritten OCR)
→ Material inspection + damage photos
→ Warehouse assignment (Depo required; system suggests; operator confirms)
→ Auto labels (Lot / Package / Pallet / QR) — no manual numbering
→ Review summary → Post
→ Balance updated in chosen WH → optional QI trigger · full audit + attachments
```

---

# FLOW-INV-002 — Issue to production

**Job:** Issue components to a released production demand.

```text
Operations → Issue Goods
→ Select Production reference
→ Pick lot/serial
→ Post GI
→ Reservation cleared · Production may consume
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

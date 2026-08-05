# Inventory Flow

**Actors:** Warehouse operator, Inventory controller  
**Module:** Inventory

---

## Core paths

### Receipt
Dashboard / Receipts screen → identify PO/ASN → count & putaway → stock updated → optional Quality hold handoff

### Issue / consumption
Reservation or production request → Issue screen → confirm pick → stock down → link to Production consumption when from WO

### Transfer / adjust / count
Transfers · Adjustments · Cycle Count screens — each with Entity Grid + confirmation, never silent stock edits

### Trace
Lot / serial inquiry → genealogy / movements

Screen IDs: `docs/15_UI/Inventory/` (INV-*)

# Maintenance Flow

**Actors:** Requester, Technician, Maintenance planner  
**Module:** Maintenance

---

## Corrective path

1. **Detect** — Asset Dashboard / Downtime / Request
2. **Request** — Work Request
3. **Plan** — Convert to Work Order; assign skills/parts
4. **Execute** — WO detail; consume Spare Parts; log time
5. **Close** — Confirm completion; update Asset history
6. **Review** — OEE / Reports / Asset KPIs

## Preventive path

Preventive calendar / Scheduler → generate WO → execute → close → next due update

## Asset-centric path

Asset Tree / Explorer → Asset Detail → History / Warranty / Parts / Downtime panes

Screen IDs: `docs/15_UI/Maintenance/` (MNT-*)

**Do not** collapse this flow into “Asset CRUD”.

# Production Flow

**Actors:** Planner, Supervisor, Operator  
**Module:** Production

---

## Happy path — Order to finished goods

1. **Plan** — Create/release Production Order (`PRD-010` → `PRD-011`)
2. **Explode** — Confirm BOM/Routing on order detail (`PRD-011`, master `PRD-002/005`)
3. **Schedule / dispatch** — Place on schedule / dispatch board (`PRD-021`, `PRD-023`)
4. **Execute** — Operator Terminal / Machine Panel (`PRD-013`, `PRD-024`)
5. **Consume** — Material consumption (`PRD-014`)
6. **Confirm** — Production confirmation (`PRD-015`)
7. **Track WIP** — WIP (`PRD-016`)
8. **Pack / FG** — Packaging → Finished Goods (`PRD-017`, `PRD-018`)
9. **Exceptions** — Scrap / Rework when needed (`PRD-027`, `PRD-028`)
10. **Review** — Dashboard / Reports (`PRD-001`, `PRD-020`)

---

## Screens involved

PRD-001, 010–018, 021, 023, 024, 027, 028, 020

## Not a flow step

Any `TASK-*` id.

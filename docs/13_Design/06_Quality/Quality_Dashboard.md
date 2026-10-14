# Quality Dashboard

**Screen:** QLT-001  
**Workspace:** Dashboard  
**Version:** 1.0

---

# Job to be done

> Quality Manager / Inspector sees open risk and drills into Inspection Queue, NCR, CAPA.

---

# Authority references

KPIs are views over Quality aggregates. Numbering / genealogy / stock laws → authority matrix.

---

# Primary users

- Quality Manager  
- Quality Inspector  
- Plant Manager (read)

---

# Layout

```text
Header (plant, refresh)
KPI row
Open work (Inspections · NCRs · CAPAs)
Risk panels (holds, overdue CAPA, fail rate)
Shortcuts (Queue, Incoming, NCR, Traceability, Lab)
```

---

# KPI cards

| KPI | Meaning |
|-----|---------|
| Open inspections | Queue depth |
| Overdue inspections | Past due |
| Open NCRs | Active incidents |
| Open CAPAs | Actions in flight |
| First-pass yield (period) | Pass / total |
| Supplier reject rate | Incoming fails |
| Hold lots | Inventory holds linked to quality |

---

# Panels

- Inspection Queue snapshot (top N) → QLT-004  
- NCR list snapshot → QLT-008  
- CAPA due this week → QLT-011  
- Moisture / lab alerts (timber) → QLT-017  
- Traceability quick search → QLT-014  

---

# Components

Metric Card · Dashboard Card · Entity Grid (compact) · Alert List · Status Badge

---

# Related

`Quality_Architecture.md` · `Quality_Workflow.md` · Screen Map `QLT-001`

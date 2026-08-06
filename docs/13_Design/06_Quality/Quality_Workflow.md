# Quality Workflow

**Module:** Quality  
**Version:** 1.0  
**Status:** Active  
**Owns:** Quality process phases and gates (not UX layout, not numbering).

---

# Authority references

| Topic | Authority |
|-------|-----------|
| Numbering | `Document_Numbering.md` |
| Genealogy | `Material_Genealogy.md` |
| Stock / holds | `Inventory_Architecture.md` |
| Production ops | `Production_Workflow.md` |
| UX jobs | Screen Map `QLT-*` + future Quality Screens / User Flows |

---

# High-level flow

```text
Inspection Plan
      │
      ▼
Trigger (GR / Op complete / Final / Manual)
      │
      ▼
Inspection Queue → Execute Inspection
      │
      ├── Pass → Continue (release gate)
      │
      └── Fail / Conditional → NCR
                │
                ▼
         Containment → Investigation → Root Cause
                │
                ▼
              CAPA ←→ Disposition (Rework / Scrap / Use-as-is / Return)
                │
                ▼
         Verify → Close NCR / CAPA
                │
                ▼
         Certificate / Traceability views (as needed)
```

---

# Incoming inspection

```text
PO / ASN → Goods Receipt (Inventory) → Incoming Inspection
→ Sample / Measure → Accept | Reject | Conditional
→ On reject: NCR + Inventory hold
```

Domain: `docs/05_Modules/07_Quality/Incoming_Inspection.md`

---

# In-process / final

```text
Production operation / FG ready
→ In-Process or Final Inspection
→ Pass → continue / FG release path
→ Fail → NCR → Production hold / rework / scrap (Production + Inventory)
```

Domain: `Process_Inspection.md` · `Final_Inspection.md`

---

# NCR → CAPA

```text
Detect → Register NCR → Classify → Contain
→ Investigate → Root Cause
→ Corrective Action → Preventive Action
→ Verify effectiveness → Close
```

Domain: `Non_Conformance.md`

---

# Laboratory (timber)

Moisture / lab results attach to lot or inspection; may gate kiln/thermowood release per plant policy.  
Domain: `Moisture.md`

---

# States (canonical for Quality docs)

**Inspection:** Planned → InProgress → Passed | Failed | Conditional | Cancelled  

**NCR:** Open → Contained → Investigating → DispositionPending → CAPALinked → Closed | Cancelled  

**CAPA:** Open → InProgress → PendingVerification → Effective | Ineffective → Closed  

---

# Gates

- Critical fail blocks Production/Inventory progression until disposition.  
- Certificate issue requires closed inspections / genealogy available for lot.  
- Approvals use Quality Approval Inbox when dual-control policy is on.

---

# Related

`Quality_Architecture.md` · Screen Map Quality section · Production FLOW-009

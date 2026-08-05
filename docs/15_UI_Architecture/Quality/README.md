# Quality — UI Information Architecture

**Module:** Quality (QMS)  
**Status:** Draft — NCR family is the exemplar for this module  
**Domain:** `docs/05_Modules/07_Quality/`

---

## Module purpose

Assure product and process quality through plans, inspections, non-conformance handling, CAPA and traceability.

---

## Workspaces

```text
Quality
├── Dashboard
├── Operations
│     ├── Inspection Queue
│     ├── Incoming Inspection
│     ├── In-Process Inspection
│     ├── Final Inspection
│     └── Non-Conformance
├── Plans & Specs        (Inspection Plans, characteristics)
├── Laboratory
└── Reports / Traceability
```

---

## Capability exemplar — Non-Conformance (NCR)

**Wrong:** `TASK-070 → Non Conformance CRUD screen`

**Right:**

```text
Quality
  └── Operations
        └── Non-Conformance
              ├── NCR List
              ├── NCR Detail
              ├── Root Cause
              ├── CAPA
              ├── History
              ├── Attachments
              └── Workflow
                    └── TASK-070 (+ follow-ons) implement slices
```

### Screen family

| Screen | Intent |
|--------|--------|
| NCR List | Queue by severity, status, product, line |
| NCR Detail | Header, defect, disposition, links to inspection/order |
| Root Cause | Structured RCA capture |
| CAPA | Corrective / preventive actions and owners |
| History | Status and assignment timeline |
| Attachments | Evidence files |
| Workflow | Submit, approve, close, reopen |

### MVP thinning

1. List + Detail + basic workflow  
2. Attachments + History  
3. Root Cause + CAPA desks  

**Implementation entry:** TASK-070 (must map to this family, not replace it)

---

## Other capabilities (index only)

| Capability | Workspace | Entry TASK |
|------------|-----------|------------|
| Inspection Plan | Plans & Specs | TASK-066 |
| Incoming / In-Process / Final Inspection | Operations | TASK-067–069 |
| CAPA (may share NCR workspace) | Operations | TASK-071 |
| Certificates / Traceability | Reports | TASK-072, 075 |
| Quality Dashboard | Dashboard | TASK-073 |

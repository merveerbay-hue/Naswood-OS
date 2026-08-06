# Maintenance — UI Information Architecture

**Module:** Maintenance (CMMS)  
**Status:** Draft — Asset family is the exemplar  
**Domain:** `docs/05_Modules/08_Maintenance/`

---

## Module purpose

Keep assets reliable: structure the asset park, plan/prevent/correct failures, manage spare parts and measure downtime/OEE.

---

## Workspaces

```text
Maintenance
├── Dashboard
├── Assets
├── Work Management      (Requests, Maintenance Orders)
├── Planning             (Preventive calendar)
├── Spare Parts
└── Reports / OEE
```

---

## Capability exemplar — Asset

**Wrong:** `Asset → CRUD page`

**Right:**

```text
Maintenance
  └── Assets
        └── Asset
              ├── Asset Explorer
              ├── Hierarchy
              ├── Asset Detail
              ├── Maintenance History
              ├── Warranty
              ├── Spare Parts
              ├── Costs
              ├── Downtime
              ├── Documents
              ├── Sensors
              └── KPIs
                    └── TASK-076 (+ follow-ons) implement slices
```

### Screen family

| Screen / pane | Intent |
|---------------|--------|
| Asset Explorer | Searchable park view |
| Hierarchy | Plant → area → line → machine tree |
| Asset Detail | Identity, class, criticality, location |
| Maintenance History | Past orders and failures |
| Warranty | Coverage window and vendor |
| Spare Parts | BOM of parts for the asset |
| Costs | Labor/material cost rollup |
| Downtime | Events affecting availability |
| Documents | Manuals, certificates |
| Sensors | IoT / signal links *(later)* |
| KPIs | MTBF, MTTR, availability |

### MVP thinning

1. Explorer + Detail + Hierarchy  
2. History + Documents  
3. Spare Parts + Downtime + Costs  
4. Sensors / advanced KPIs  

**Implementation entry:** TASK-076

---

## Other capabilities (index)

| Capability | Workspace | Entry TASK |
|------------|-----------|------------|
| Work Request | Work Management | TASK-077 |
| Maintenance Order | Work Management | TASK-078 |
| Preventive / Corrective | Planning / Work Mgmt | TASK-079–080 |
| Downtime / Spare Parts | Assets / Spare Parts | TASK-081–082 |
| Dashboards / OEE | Dashboard / Reports | TASK-083–085 |

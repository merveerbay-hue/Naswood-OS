# Production Workspaces

**Module:** Production  
**Status:** Active target IA

---

## 1. Dashboard

**Purpose:** At-a-glance plant / line health and entry points into work.

**Screens**

| Screen | Intent |
|--------|--------|
| Production Dashboard | Open orders, active WOs, WIP, scrap rate, alerts |
| Alert Center (module) | Exceptions requiring supervisor action |

**Not a dump of every entity KPI.** Cards must deep-link into Execution / Monitoring.

**TASK entry:** TASK-065 (MVP metrics only until boards exist)

---

## 2. Planning

**Purpose:** Time and capacity before release.

**Screens**

| Screen | Intent |
|--------|--------|
| Production Calendar | Working time, holidays, maintenance windows |
| Shift Planning | Shift definitions and assignments |
| Capacity Planning | Load vs available capacity |
| Planned Order Board *(future)* | Firming / releasing into Production Orders |

**TASK entry:** TASK-051 Shift, TASK-052 Calendar (family to be expanded beyond CRUD)

---

## 3. Execution

**Purpose:** Run manufacturing work.

**Screens**

| Screen | Intent |
|--------|--------|
| Production Order List | Find / filter / release / hold |
| Production Order Detail | Header, lines/ops, status, related WOs |
| Create / Edit Production Order | Structured create (not generic 4-field form) |
| Work Order List / Detail | Shop-floor executable unit |
| Dispatch Board | Assign WOs to lines/machines/shifts |
| Operator Terminal | Simplified confirm / consume / scrap |
| Machine Terminal | Machine-centric queue |

**TASK entry:** TASK-056, TASK-057 (+ future tasks for boards/terminals)

Detail: [Production_Order.md](Production_Order.md)

---

## 4. Master Data

**Purpose:** Stable manufacturing definitions.

**Capabilities & screen families**

| Capability | Minimum family |
|------------|----------------|
| BOM | List, Detail, Create, Revision, Compare, Import, Export — see [BOM.md](BOM.md) |
| Routing | List, Detail, Create, Operation steps |
| Operation | List, Detail (used by routing) |
| Work Center / Production Line / Machine | List, Detail, status |
| Tooling | List, Detail, assignments |
| Production Parameters | List, Detail by product/machine |

**TASK entry:** TASK-046–055 as **slices**, not as “one screen each equals done”.

---

## 5. Monitoring

**Purpose:** In-flight and exception visibility.

**Screens**

| Screen | Intent |
|--------|--------|
| WIP Board | Quantities by order/op/location |
| Confirmation Log | Posted confirmations |
| Material Consumption | Issues against orders |
| Scrap / Rework desks | Capture and disposition |
| Packaging / Finished Goods | FG receipt readiness |

**TASK entry:** TASK-058–064

---

## 6. Reports

**Purpose:** Historical / compliance / management views.

**Screens**

| Screen | Intent |
|--------|--------|
| Production Reports hub | Standard report list |
| Order cycle time / OEE views *(future)* | Analytics |

Prefer reuse of Analytics module where cross-cutting.

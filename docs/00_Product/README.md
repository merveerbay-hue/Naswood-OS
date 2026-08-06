# NOS Product Map (living)

**Status:** Active — Product Architect Drive  
**Authority:** [`AI/NOS_CONSTITUTION/04_PRODUCT_ARCHITECT.md`](../../AI/NOS_CONSTITUTION/04_PRODUCT_ARCHITECT.md)

This folder holds NOS **as a product**, not as a TASK backlog.

```text
NOS → Modules → Workspace → Navigation → Screen → Component → Workflow → Permissions → Code
```

---

## Program order

1. NOS whole-product map (this folder)
2. Module deep-dives (role lenses)
3. Workspace extraction
4. Screen design
5. Shared component library
6. Cursor implementation of named slices

---

## Module index (outline — refine in sessions)

| Module | Purpose (one line) | Primary roles | Workspaces (target) | Product docs |
|--------|-------------------|---------------|---------------------|--------------|
| Production | Plan and execute shop-floor manufacturing | Prod. Manager, Planner, Operator, Quality, Maintenance | Dashboard, Planning, Execution, Monitoring, Master Data, Reports, Analytics | `13_Design/05_Production/`, `15_UI_Architecture/Production/` |
| Inventory | Warehouse health, stock, movements | Warehouse, Materials, Planner | Dashboard, Warehouse, Materials, Transactions, Planning, Reports | `15_UI_Architecture/Inventory/` |
| Purchasing | Source and receive supply | Buyer, Approver | *(session)* | Purchasing IA |
| Sales | Order-to-cash commercial | Sales, CSR | *(session)* | Sales IA |
| Quality | Inspect, NCR, CAPA, release | Quality, Operator | *(session)* | Quality IA |
| Maintenance | Assets, WO, PM, downtime, OEE | Maintenance, Supervisor | Assets, Maintenance, Planning, Downtime, OEE, Reports | Maintenance IA |
| Finance | *(session)* | *(session)* | *(session)* | — |
| Analytics | Cross-module insight | Manager, Executive | *(session)* | — |
| Platform / Admin | Identity, files, settings | Admin | *(session)* | Constitution Platform |

---

## Role lens checklist (per module session)

Copy into the module decision note:

- [ ] Üretim / modül müdürü ne görür?
- [ ] Planlamacı ne görür?
- [ ] Operatör / saha kullanıcısı ne görür?
- [ ] Bakım kesişimi?
- [ ] Kalite kesişimi?
- [ ] Yönetici / CEO KPI yüzeyi?
- [ ] SAP / IFS / D365 / Infor referans notu
- [ ] NOS farklılaştırma kararı
- [ ] Workspace listesi kilitlendi
- [ ] Document layer güncellendi
- [ ] Cursor implement slice adlandırıldı (veya henüz değil)

---

## Thinking ladder (mandatory)

1. Real life  
2. User / roles  
3. Market reference (SAP · IFS · Dynamics · Infor)  
4. NOS better  
5. Document  
6. Implement  

---

## Next session

Directed by Product Architect. Default first deep-dive when requested: **Production** — “Üretim Müdürü Production’a girince ne görmeli?”

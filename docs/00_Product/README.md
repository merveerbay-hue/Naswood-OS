# NOS Product Map (living)

**Status:** Active — Phase 2 Product Architecture  
**Authority:** [`AI/NOS_CONSTITUTION/04_PRODUCT_ARCHITECT.md`](../../AI/NOS_CONSTITUTION/04_PRODUCT_ARCHITECT.md)

---

## Phase 2 anchor

**SSOT / who owns which rule:** [`DOCUMENTATION_AUTHORITY_MATRIX.md`](./DOCUMENTATION_AUTHORITY_MATRIX.md)  
**Screen types (no shared Create):** [`docs/13_Design/Common/Screen_Types.md`](../13_Design/Common/Screen_Types.md)  
**UI patterns:** [`docs/13_Design/Common/UI_Patterns.md`](../13_Design/Common/UI_Patterns.md)  
**Canonical Screen Map:** [`NOS_SCREEN_MAP.md`](./NOS_SCREEN_MAP.md)  
**Job-first design:** [`JOB_FIRST_SCREEN_DESIGN.md`](./JOB_FIRST_SCREEN_DESIGN.md)  
**Process screens (job Create CTAs):** [`Process_Screens/`](./Process_Screens/) — Planning · Receiving · Issue · Transfer · Count · NCR · Maint WO · PO · SO  
**Create → Job matrix:** [`Screen_Types.md`](../13_Design/Common/Screen_Types.md) § 3b

Before any new rule: *Bu kuralı daha önce tanımlamış mıydık?* → Matrix → reference or extend authority.

```text
NOS → Module → Workspace → Job Screen → Component → Workflow → Permissions → Code
```

**Every screen:** *Kullanıcı bu ekranda hangi işi bitirmek istiyor?*  
Not: *Hangi entity’yi CRUD’larız?*

---

## Top-level modules (locked in Screen Map)

```text
NOS
├── Dashboard
├── Product
├── Sales
├── CRM
├── Purchasing
├── Inventory
├── Production
├── Quality
├── Maintenance
├── Finance
├── HR
├── Administration
└── Settings
```

---

## Program order

1. **NOS Screen Map** ← current  
2. Module deep-dives (role lenses)  
3. Workspace refinement  
4. Screen PRDs (`docs/15_UI/`)  
5. Shared component library (`docs/18_Component_Library/`)  
6. Cursor implementation of named slices  

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

Directed by Product Architect. Recommended:

- **Production** — “Üretim Müdürü Production’a girince ne görmeli?”  
- or **Dashboard** — “CEO / Plant Manager ne görmeli?”

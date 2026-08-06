# INV-ISS-001 — Issue Goods — **rules spine**

**Module:** Inventory · **Workspace:** Operations  
**Status:** Rules retained · **UX authority:** [`INV_Goods_Issue_Workbench.md`](./INV_Goods_Issue_Workbench.md)  
**Replaces as UI:** “Yeni çıkış” / Create Goods Issue form

---

## Supersession

Full UX = **Goods Issue Workbench v2.0** (AI pick · Override/Explorer · package preview · **partial package** · evidence · Post).  
This file keeps demand → pick existing Lot/MI/Package → qty → Post gates.

---

## Job

> Operatör, talebe karşı stoğu **tarayarak / doğrulayarak** çıkarır; **Post** ile bakiye düşer. Create formu değil.

## CTA

**Issue goods** / **Mal çıkışı**

## Spine (Workbench)

```text
1 Business document (PO / WO / SO / … / Manual+permission)
2 Load required materials (no material create)
3 AI recommend WH / location / lot / package  (+ Ignore AI → Warehouse Explorer)
4 Pick / package select (scan · partial qty allowed)
5 Verify · Quality gate
6 Evidence (as needed)
7 Destination (line / dock)
8 Review (+ Override History) → Post
```

## Gates

- Business reason / reference required (Manual = permission + reason).  
- Qty > 0 and ≤ available / reserved (policy).  
- Existing Lot / Serial / MI / Package — select/scan, never type codes.  
- **Partial package:** same Package Identity / barcode; update remaining qty only — **no new PKG number**.  
- Quality not blocking (without authorization).  
- Overrides logged.  
- Reservation cleared on Post when applicable.  
- GI + inventory txn numbers via Numbering only.

## Related

`INV_Goods_Issue_Workbench.md` · `Inventory_Workflow.md` · FLOW-INV-002 · `Document_Numbering.md`

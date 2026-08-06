# INV-ISS-001 — Issue Goods — **rules spine**

**Module:** Inventory · **Workspace:** Operations  
**Status:** Rules retained · **UX authority:** [`INV_Goods_Issue_Workbench.md`](./INV_Goods_Issue_Workbench.md)  
**Replaces as UI:** “Yeni çıkış” / Create Goods Issue form

---

## Supersession

Full UX = **Goods Issue Workbench v2.0** (AI pick · **Accept / Override** · Explorer · package preview · **partial package** · AI Validation always on · evidence · Post).  
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
3 AI recommend → Kabul Et  OR  Yoksay (Explorer)
4 Pick / package select (scan · partial qty allowed)
5 Verify · AI Validation (also in Override) · Quality gate
6 Evidence (as needed)
7 Destination (line / dock)
8 Review (+ Override History) → Post
```

## Gates

- Business reason / reference required (Manual = permission + reason).  
- Qty > 0 and ≤ available / reserved (policy).  
- Existing Lot / Serial / MI / Package — select/scan, never type codes.  
- **Partial package:** auto-update remaining qty/volume/weight/pieces/status; optional **company-policy split** with parent–child traceability.  
- AI Validation always on (Override does not disable rules).  
- Quality not blocking (without authorization).  
- Overrides logged in audit history.  
- Reservation cleared on Post when applicable.  
- GI + inventory txn numbers via Numbering only.

## Related

`INV_Goods_Issue_Workbench.md` · `Inventory_Workflow.md` · FLOW-INV-002 · `Document_Numbering.md`

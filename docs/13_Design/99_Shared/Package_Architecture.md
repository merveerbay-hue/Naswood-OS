# Package Architecture

**Document:** Package Architecture  
**Status:** Skeleton — Foundation Aşama 1 #7 (Queued)  
**Version:** 0.1.0  
**Location:** `docs/13_Design/99_Shared/Package_Architecture.md`  
**Owns (when completed):** What a Package is · lifecycle (open → picking → partially used → closed / consumed) · package status · package history · package photos / evidence · barcode / QR immutability · parent–child split policy · relationship to Material Identity and Lot  
**Does not own:** Package Allocation Workspace UX (→ `Package_Allocation_Workspace.md`) · Numbering formats · Conversion math · Goods Issue step copy

---

## Foundation position

```text
Inventory_Foundation_Program.md #7
PAW and GI Workbenches already consume package laws — this doc becomes the
single Package domain authority those PRDs reference.
```

---

## Absolute laws (skeleton)

```text
1. Package is a physical handling unit with immutable Package Identity / barcode
   (default partial use keeps same ID).
2. Package ≠ Material Identity — Package links to MI (and may hold many units).
3. Status is transaction-driven: Available · Reserved · Picking · Partially Used ·
   Quality Hold · Damaged · Consumed · Closed.
4. Closing checklist after partial take (restack · strap · label · optional photo).
5. Package history and photos are permanent (Compliance · Evidence).
6. Optional company-policy split mints child packages with full parent→child trace.
```

---

## Lifecycle questions (to deepen)

| Question |
|----------|
| When does a package open / close? |
| What evidence is required at close? |
| How do package photos attach to Document Library? |
| How does Package relate to Bundle / Pallet? |

---

## Related

`Inventory_Foundation_Program.md` · `Package_Allocation_Workspace.md` · `Material_Identity_Architecture.md` · `Material_Definition_Architecture.md` · `Compliance_Architecture.md` · `INV_Goods_Issue_Workbench.md`

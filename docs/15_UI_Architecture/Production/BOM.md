# BOM — Screen Architecture

**Module:** Production  
**Workspace:** Master Data  
**Capability:** Bill of Materials  
**Status:** Target IA (exemplar)  
**Domain design:** `docs/13_Design/05_Production/BOM_Architecture.md`  
**Implementation entry TASK:** TASK-046

---

## Purpose

Enable manufacturing engineering to define **how a product is built**, maintain revisions, and support planning/execution consumers.

---

## User jobs

1. Find an existing BOM  
2. Open and understand structure (header + components)  
3. Create a new BOM  
4. Revise without losing history  
5. Compare revisions  
6. Import / export definitions  
7. Release for use in Production Orders  

---

## Screen family (required shape)

```text
BOM
├── BOM List
├── BOM Detail
│     ├── Header
│     ├── Components (lines)
│     ├── Operations context (refs)
│     ├── Effectivity
│     ├── Status / lifecycle
│     └── Attachments
├── Create BOM
├── Revision
├── Compare
├── Import
└── Export
```

This is the product shape. **TASK-046 does not equal “BOM screen”.**  
TASK-046 (and follow-ons) deliver slices of this family.

---

## Screen responsibilities

### BOM List

- Search / filter by product, plant, status, revision  
- Columns: code, product, revision, status, effective dates  
- Actions: Open, Create, Duplicate, Export  

### BOM Detail

- Read/update header within permission rules  
- Line grid with quantities, units, scrap factors, valid-from/to  
- Lifecycle actions: Submit, Approve, Release, Obsolete  
- Links to Routing / Product  

### Create BOM

- Guided create (product, plant, base qty, type)  
- Not a four-field generic form pretending to be complete  

### Revision

- Create new revision from released BOM  
- Preserve prior revision immutability rules  

### Compare

- Side-by-side revision diff (lines added/removed/changed)  

### Import / Export

- File-based exchange (format per Integration/Design specs)  

---

## MVP thinning (allowed)

| Phase | Screens |
|-------|---------|
| MVP-1 | BOM List, BOM Detail (header + lines basic), Create |
| MVP-2 | Revision, lifecycle actions |
| MVP-3 | Compare, Import, Export |

Deferred screens must remain **named** here so agents do not declare the capability “done” at MVP-1.

---

## Implementation TASK mapping

| Slice | Suggested TASK / follow-on |
|-------|----------------------------|
| API + List/Detail/Create MVP | TASK-046 |
| Revision + approve/release UI | follow-on TASK |
| Compare | follow-on TASK |
| Import/Export | follow-on TASK |

Each TASK must include the **UI Architecture Mapping** block from `../03_Screen_to_Task_Mapping.md`.

---

## Anti-pattern (current Sprint debt)

```text
/production/boms → ResourcePage (Code, Name, Status, Notes)
```

Acceptable only as a temporary scaffold. Convergence target is this screen family.

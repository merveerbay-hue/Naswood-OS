# PRD-017 — Packaging

**Module:** Production  
**Workspace:** Execution  
**Screen ID:** PRD-017  
**Status:** Specified  
**Implementation TASKs (slices):** TASK-061

---

## Purpose

Pack manufactured output into packages/units for FG receipt.

---

## Primary users

Operator, Packaging lead

---

## Components

- Pack list
- Package build
- Label print action
- Link to order

---

## Filters

- Order
- Status
- Package type

---

## Actions

- Create package
- Close package
- Print label
- Post

---

## User flow

- Confirmation → Packaging → Finished Goods

---

## Data / domain dependencies

- Production domain aggregates and contracts relevant to this screen
- Plant / Company context from shell
- Permissions enforced per action

---

## Deferred (explicit)

- SSCC / advanced labeling

---

## Notes

This screen is part of Production Screen Architecture under `docs/15_UI`.  
An Implementation TASK may deliver only a slice; it must not redefine this screen as a generic CRUD page.

# PRD-003 — BOM Detail

**Module:** Production  
**Workspace:** Master Data  
**Screen ID:** PRD-003  
**Status:** Specified  
**Implementation TASKs (slices):** TASK-046

---

## Purpose

Maintain BOM header, component lines, effectivity and lifecycle.

---

## Primary users

Manufacturing Engineer

---

## Components

- Header form
- Component line grid
- Effectivity panel
- Lifecycle status
- Attachments
- Related Routing link

---

## Filters

- Line search within BOM

---

## Actions

- Edit header
- Add/edit/remove lines
- Submit
- Approve
- Release
- Create Revision
- Compare
- Export

---

## User flow

- From List → Detail → Revise → Release → used by Production Order

---

## Data / domain dependencies

- Production domain aggregates and contracts relevant to this screen
- Plant / Company context from shell
- Permissions enforced per action

---

## Deferred (explicit)

- Visual multilevel explosion tree

---

## Notes

This screen is part of Production Screen Architecture under `docs/15_UI`.  
An Implementation TASK may deliver only a slice; it must not redefine this screen as a generic CRUD page.

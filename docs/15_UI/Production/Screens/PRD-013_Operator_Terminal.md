# PRD-013 — Operator Panel

**Module:** Production  
**Workspace:** Execution  
**Screen ID:** PRD-013  
**Status:** Specified  
**Implementation TASKs (slices):** TASK-059, TASK-058, TASK-063

---

## Purpose

Simplified execution UI for operators: start/complete ops, report qty, scrap.

---

## Primary users

Operator

---

## Components

- WO queue
- Current job card
- Qty pad
- Scrap reason
- Material quick-issue
- Large touch targets

---

## Filters

- Line
- Machine
- Shift
- Assigned to me

---

## Actions

- Start
- Partial confirm
- Final confirm
- Report scrap
- Call supervisor

---

## User flow

- Login context → queue → execute → confirmation posts

---

## Data / domain dependencies

- Production domain aggregates and contracts relevant to this screen
- Plant / Company context from shell
- Permissions enforced per action

---

## Deferred (explicit)

- Badge/QR login

---

## Notes

This screen is part of Production Screen Architecture under `docs/15_UI`.  
An Implementation TASK may deliver only a slice; it must not redefine this screen as a generic CRUD page.

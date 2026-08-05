# PRD-015 — Production Confirmation

**Module:** Production  
**Workspace:** Execution  
**Screen ID:** PRD-015  
**Status:** Specified  
**Implementation TASKs (slices):** TASK-059

---

## Purpose

Post yield, time, and completion against operations.

---

## Primary users

Operator, Supervisor

---

## Components

- Confirmation form
- History grid
- Variance indicators

---

## Filters

- Order
- WO
- Operation
- Date

---

## Actions

- Confirm
- Cancel confirmation
- Adjust

---

## User flow

- Operator Panel / Machine Panel → Confirmation → WIP/FG update

---

## Data / domain dependencies

- Production domain aggregates and contracts relevant to this screen
- Plant / Company context from shell
- Permissions enforced per action

---

## Deferred (explicit)

- Auto-confirm from machine signals

---

## Notes

This screen is part of Production Screen Architecture under `docs/15_UI`.  
An Implementation TASK may deliver only a slice; it must not redefine this screen as a generic CRUD page.

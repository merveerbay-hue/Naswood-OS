# Wizard

**Use for:** Multi-step create / guided processes (receipt, NCR intake, PO creation)

## Anatomy

- Step indicator
- Step body (form or selection)
- Footer: Back / Next / Cancel / Finish
- Validation per step; summary step before commit

## Rules

- Prefer wizard only when cognitive load exceeds a single form
- Final step posts once; intermediate steps are draft-safe when backend supports drafts

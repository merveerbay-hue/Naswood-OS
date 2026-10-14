# App Shell

## Responsibilities

- Authentication gate
- Global header (plant, user, search, notifications, AI entry)
- Module navigation rail / drawer (from `19_Navigation`)
- Workspace sub-nav host
- Theme / density / locale

## Rules

- Shell does not embed business CRUD
- Module areas lazy-load when possible
- Focused shells (Operator Terminal) reuse auth but may hide global nav

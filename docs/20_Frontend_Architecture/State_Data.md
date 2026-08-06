# State & Data

## Defaults

- **Server state** (lists, details, dashboards): query library / RTK Query / equivalent — cache by screen key
- **Form state**: local to screen or wizard; submit via Module API commands
- **Workflow actions**: explicit mutations; refresh detail + audit after success
- **Optimistic UI**: only for non-workflow toggles; never optimistic Approve/Release

## Rules

- API contracts from Module API design docs
- Errors surface workflow/business messages, not only HTTP codes
- Filters/saved views persist per user when platform supports preferences

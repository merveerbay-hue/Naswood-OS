# Entity Grid

**Use for:** Primary list screens (orders, assets, NCRs, stock docs)

## Anatomy

- Toolbar: search, filters, column chooser, primary actions (Create / Export)
- Filter bar: saved views, status chips, date range, plant/warehouse
- Grid: sortable columns, row selection, bulk actions
- Pagination / virtual scroll
- Row click → Master Detail or full Detail screen

## Rules

- Not a naked DataTable dump of every DB column
- Columns follow Screen Architecture field lists
- Status via Status Badge; never raw enums as plain text without semantic color
- Empty / loading / error states mandatory

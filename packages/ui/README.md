# @naswood/ui — Naswood OS Design System

Shared UI foundation for Naswood OS web applications.

## Scope (this package)

- Design tokens (CSS variables from approved Design System)
- Base primitives styled in shadcn/ui patterns: `Button`, `Input`, `Label`, `Card`
- `cn()` utility for Tailwind class merging

## Out of scope (for later tasks)

- Business pages (Login, Dashboard, Inventory, …)
- Full component catalog (DataGrid, Charts, Documents)

## Usage

```tsx
import { Button, Card } from '@naswood/ui';
import '@naswood/ui/styles.css';
```

Tokens follow `docs/13_Design/00_Platform/Design_System/01_Foundation/`.

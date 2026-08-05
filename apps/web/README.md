# Naswood OS Web

Platform frontend foundation (`apps/web`).

## Stack

- React 19 + Vite + TypeScript
- Tailwind CSS 4
- `@naswood/ui` Design System (shadcn-style primitives)
- TanStack Router
- TanStack Query
- React Hook Form + Zod
- ESLint + Prettier

## Commands

```bash
pnpm install
pnpm --filter @naswood/web dev
pnpm --filter @naswood/web build
```

## Notes

- No business pages yet (Login/Dashboard/modules come after foundation).
- API proxy targets `http://localhost:5080` for `/api` and `/health`.

# Naswood OS Web

Platform frontend (`apps/web`).

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

## Auth (TASK-000)

- `/login` — Login form (username, password, company, plant, remember me)
- `/` — Authenticated shell (requires session)
- Tokens stored in `sessionStorage` (or `localStorage` when Remember me is on)
- API via Vite proxy → `http://localhost:5080` (`/api`, `/health`)

Dev credentials (bootstrap): `admin` / `Naswood!Admin1`

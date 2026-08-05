import { useRouterState } from '@tanstack/react-router';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@naswood/ui';
import { findNavTrail } from '@/navigation/nav-config';

/** Placeholder until the owning business module is implemented. */
export function ModulePlaceholderPage() {
  const pathname = useRouterState({ select: (s) => s.location.pathname });
  const trail = findNavTrail(pathname);
  const title = trail.at(-1)?.label ?? 'Module';

  return (
    <section className="space-y-4">
      <div>
        <h1 className="text-2xl font-semibold tracking-tight">{title}</h1>
        <p className="mt-1 text-[var(--text-secondary)]">
          This route is reserved for a future module screen. Navigation and shell are wired.
        </p>
      </div>
      <Card>
        <CardHeader>
          <CardTitle>Placeholder</CardTitle>
          <CardDescription>{pathname}</CardDescription>
        </CardHeader>
        <CardContent className="text-sm text-[var(--text-secondary)]">
          No business UI yet — intentional for Sprint 00 platform shell.
        </CardContent>
      </Card>
    </section>
  );
}

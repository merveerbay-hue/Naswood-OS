import { Button, Card, CardContent, CardDescription, CardHeader, CardTitle } from '@naswood/ui';

/**
 * Platform foundation shell only — no business pages yet.
 * Login / Dashboard / modules land in later tasks on this stack.
 */
export function FoundationHomePage() {
  return (
    <main className="mx-auto flex min-h-screen max-w-3xl flex-col justify-center gap-6 px-6 py-12">
      <div>
        <p className="text-sm font-medium uppercase tracking-wide text-[var(--color-primary)]">
          Naswood OS
        </p>
        <h1 className="mt-2 text-3xl font-semibold tracking-tight">Platform Web Foundation</h1>
        <p className="mt-2 text-[var(--text-secondary)]">
          React + Vite + TypeScript shell with Design System, TanStack Router/Query, React Hook Form
          and Zod. Business pages are intentionally not implemented yet.
        </p>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Ready stack</CardTitle>
          <CardDescription>
            Shared package <code>@naswood/ui</code> and app routing are wired for upcoming Login and
            module work.
          </CardDescription>
        </CardHeader>
        <CardContent className="flex flex-wrap gap-3">
          <Button type="button">Primary</Button>
          <Button type="button" variant="secondary">
            Secondary
          </Button>
          <Button type="button" variant="outline">
            Outline
          </Button>
        </CardContent>
      </Card>
    </main>
  );
}

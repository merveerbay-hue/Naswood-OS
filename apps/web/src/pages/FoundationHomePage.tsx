import { Button, Card, CardContent, CardDescription, CardHeader, CardTitle } from '@naswood/ui';
import { useNavigate } from '@tanstack/react-router';
import { useState } from 'react';
import { useAuth } from '@/auth/useAuth';

/**
 * Authenticated landing shell after Login.
 * Dashboard layout / modules land in later tasks.
 */
export function FoundationHomePage() {
  const { user, logout, isBootstrapping } = useAuth();
  const navigate = useNavigate();
  const [isLoggingOut, setIsLoggingOut] = useState(false);

  const onLogout = async () => {
    setIsLoggingOut(true);
    try {
      await logout();
      await navigate({ to: '/login' });
    } finally {
      setIsLoggingOut(false);
    }
  };

  if (isBootstrapping) {
    return (
      <main className="mx-auto flex min-h-screen max-w-3xl items-center justify-center px-6">
        <p className="text-[var(--text-secondary)]">Loading session…</p>
      </main>
    );
  }

  return (
    <main className="mx-auto flex min-h-screen max-w-3xl flex-col justify-center gap-6 px-6 py-12">
      <div>
        <p className="text-sm font-medium uppercase tracking-wide text-[var(--color-primary)]">
          Naswood OS
        </p>
        <h1 className="mt-2 text-3xl font-semibold tracking-tight">
          Welcome{user?.name ? `, ${user.name}` : ''}
        </h1>
        <p className="mt-2 text-[var(--text-secondary)]">
          You are signed in. Dashboard layout and business modules will build on this foundation.
        </p>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Session</CardTitle>
          <CardDescription>Current identity from authentication.</CardDescription>
        </CardHeader>
        <CardContent className="space-y-3 text-sm">
          <dl className="grid gap-2 sm:grid-cols-2">
            <div>
              <dt className="text-[var(--text-muted)]">Username</dt>
              <dd className="font-medium">{user?.username ?? '—'}</dd>
            </div>
            <div>
              <dt className="text-[var(--text-muted)]">Roles</dt>
              <dd className="font-medium">{user?.roles?.join(', ') || '—'}</dd>
            </div>
            <div>
              <dt className="text-[var(--text-muted)]">Company</dt>
              <dd className="font-medium">{user?.companyId ?? '—'}</dd>
            </div>
            <div>
              <dt className="text-[var(--text-muted)]">Plant</dt>
              <dd className="font-medium">{user?.plantId ?? '—'}</dd>
            </div>
          </dl>
          <Button type="button" variant="outline" onClick={onLogout} disabled={isLoggingOut}>
            {isLoggingOut ? 'Signing out…' : 'Sign out'}
          </Button>
        </CardContent>
      </Card>
    </main>
  );
}

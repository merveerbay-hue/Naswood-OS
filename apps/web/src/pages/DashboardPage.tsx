import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@naswood/ui';
import { useAuth } from '@/auth/useAuth';

export function DashboardPage() {
  const { user } = useAuth();

  return (
    <section className="space-y-6">
      <div>
        <h1 className="text-2xl font-semibold tracking-tight">
          Welcome{user?.name ? `, ${user.name}` : ''}
        </h1>
        <p className="mt-1 text-[var(--text-secondary)]">
          Platform dashboard shell is ready. Widgets and module KPIs land in later slices.
        </p>
      </div>

      <div className="grid gap-4 md:grid-cols-3">
        <Card>
          <CardHeader>
            <CardTitle>Session</CardTitle>
            <CardDescription>Current identity</CardDescription>
          </CardHeader>
          <CardContent className="space-y-2 text-sm">
            <p>
              <span className="text-[var(--text-muted)]">User · </span>
              {user?.username}
            </p>
            <p>
              <span className="text-[var(--text-muted)]">Roles · </span>
              {user?.roles?.join(', ') || '—'}
            </p>
          </CardContent>
        </Card>
        <Card>
          <CardHeader>
            <CardTitle>Company</CardTitle>
            <CardDescription>Active context</CardDescription>
          </CardHeader>
          <CardContent className="space-y-2 text-sm">
            <p>
              <span className="text-[var(--text-muted)]">Company · </span>
              {user?.companyId}
            </p>
            <p>
              <span className="text-[var(--text-muted)]">Plant · </span>
              {user?.plantId}
            </p>
          </CardContent>
        </Card>
        <Card>
          <CardHeader>
            <CardTitle>Shell</CardTitle>
            <CardDescription>TASK-006–009</CardDescription>
          </CardHeader>
          <CardContent className="text-sm text-[var(--text-secondary)]">
            Layout, navigation, sidebar and header chrome are active. Business pages remain
            placeholders.
          </CardContent>
        </Card>
      </div>
    </section>
  );
}

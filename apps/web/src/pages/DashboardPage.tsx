import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@naswood/ui';
import { useAuth } from '@/auth/useAuth';
import { useI18n } from '@/i18n';

export function DashboardPage() {
  const { user } = useAuth();
  const { t } = useI18n();

  return (
    <section className="space-y-6">
      <div>
        <h1 className="text-2xl font-semibold tracking-tight">
          {t('home.welcome')}
          {user?.name ? `, ${user.name}` : ''}
        </h1>
        <p className="mt-1 text-[var(--text-secondary)]">{t('home.subtitle')}</p>
      </div>

      <div className="grid gap-4 md:grid-cols-3">
        <Card>
          <CardHeader>
            <CardTitle>{t('home.session')}</CardTitle>
            <CardDescription>{t('home.currentIdentity')}</CardDescription>
          </CardHeader>
          <CardContent className="space-y-2 text-sm">
            <p>
              <span className="text-[var(--text-muted)]">{t('home.user')} · </span>
              {user?.username}
            </p>
            <p>
              <span className="text-[var(--text-muted)]">{t('home.roles')} · </span>
              {user?.roles?.join(', ') || '—'}
            </p>
          </CardContent>
        </Card>
        <Card>
          <CardHeader>
            <CardTitle>{t('company')}</CardTitle>
            <CardDescription>{t('home.activeContext')}</CardDescription>
          </CardHeader>
          <CardContent className="space-y-2 text-sm">
            <p>
              <span className="text-[var(--text-muted)]">{t('company')} · </span>
              {user?.companyId}
            </p>
            <p>
              <span className="text-[var(--text-muted)]">{t('plant')} · </span>
              {user?.plantId}
            </p>
          </CardContent>
        </Card>
        <Card>
          <CardHeader>
            <CardTitle>{t('home.shell')}</CardTitle>
            <CardDescription>{t('platform')}</CardDescription>
          </CardHeader>
          <CardContent className="text-sm text-[var(--text-secondary)]">{t('home.shellHint')}</CardContent>
        </Card>
      </div>
    </section>
  );
}

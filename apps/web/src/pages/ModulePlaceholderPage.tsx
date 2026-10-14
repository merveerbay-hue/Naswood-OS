import { useRouterState } from '@tanstack/react-router';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@naswood/ui';
import { useI18n } from '@/i18n';
import { findNavTrail } from '@/navigation/nav-config';

/** Placeholder until the owning business module is implemented. */
export function ModulePlaceholderPage() {
  const { t } = useI18n();
  const pathname = useRouterState({ select: (s) => s.location.pathname });
  const trail = findNavTrail(pathname);
  const title = trail.at(-1)?.label ?? t('placeholder.module');

  return (
    <section className="space-y-4">
      <div>
        <h1 className="text-2xl font-semibold tracking-tight">{title}</h1>
        <p className="mt-1 text-[var(--text-secondary)]">{t('placeholder.comingSoonBody')}</p>
      </div>
      <Card>
        <CardHeader>
          <CardTitle>{t('placeholder.comingSoonTitle')}</CardTitle>
          <CardDescription>{pathname}</CardDescription>
        </CardHeader>
        <CardContent className="text-sm text-[var(--text-secondary)]">
          {t('placeholder.comingSoonBody')}
        </CardContent>
      </Card>
    </section>
  );
}

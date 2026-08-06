import { Link } from '@tanstack/react-router';
import { Card, CardContent, CardHeader, CardTitle } from '@naswood/ui';
import { useI18n } from '@/i18n';

/** QLT-001 — Quality Command Center (queues · jobs — not vanity KPI wall). */
export function QualityDashboardPage() {
  const { t } = useI18n();

  const queues = [
    { label: t('quality.qOpenInsp'), value: '12', path: '/quality/operations/inspect', tone: 'default' as const },
    { label: t('quality.qOpenHold'), value: '4', path: '/quality/operations/hold-desk', tone: 'warn' as const },
    { label: t('quality.qOpenNcr'), value: '7', path: '/quality/operations/ncrs', tone: 'danger' as const },
    { label: t('quality.qOpenCapa'), value: '3', path: '/quality/operations/capa', tone: 'default' as const },
  ];

  const jobs = [
    { label: t('quality.jobInspect'), path: '/quality/operations/inspect', primary: true },
    { label: t('quality.jobHold'), path: '/quality/operations/hold-desk', primary: false },
    { label: t('quality.jobNcr'), path: '/quality/operations/ncr', primary: false },
    { label: t('quality.jobTrace'), path: '/quality/compliance/traceability', primary: false },
  ];

  return (
    <div className="space-y-6">
      <div>
        <p className="text-xs font-medium text-[var(--text-muted)]">QLT-001</p>
        <h2 className="text-xl font-semibold tracking-tight">{t('quality.dashTitle')}</h2>
        <p className="mt-1 max-w-2xl text-sm text-[var(--text-secondary)]">{t('quality.dashDesc')}</p>
      </div>

      <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
        {queues.map((q) => (
          <Link key={q.path} to={q.path} className="block transition hover:-translate-y-0.5">
            <Card
              className={
                q.tone === 'danger'
                  ? 'border-[var(--color-danger)]/40'
                  : q.tone === 'warn'
                    ? 'border-amber-500/40'
                    : undefined
              }
            >
              <CardHeader>
                <CardTitle className="text-base">{q.label}</CardTitle>
              </CardHeader>
              <CardContent className="text-2xl font-semibold tabular-nums">{q.value}</CardContent>
            </Card>
          </Link>
        ))}
      </div>

      <div>
        <h3 className="mb-2 text-sm font-semibold">{t('quality.jobCtas')}</h3>
        <div className="flex flex-wrap gap-2">
          {jobs.map((j) => (
            <Link
              key={j.path}
              to={j.path}
              className={`rounded-md px-3 py-2 text-sm font-medium ${
                j.primary
                  ? 'bg-[var(--color-primary)] text-white'
                  : 'border border-[var(--border-default)] bg-[var(--color-surface)]'
              }`}
            >
              {j.label}
            </Link>
          ))}
        </div>
      </div>

      <Card>
        <CardHeader>
          <CardTitle className="text-base">{t('quality.foundationTitle')}</CardTitle>
        </CardHeader>
        <CardContent className="space-y-1 text-sm text-[var(--text-secondary)]">
          <p>{t('quality.foundationLine1')}</p>
          <p>{t('quality.foundationLine2')}</p>
          <p>{t('quality.foundationLine3')}</p>
        </CardContent>
      </Card>
    </div>
  );
}

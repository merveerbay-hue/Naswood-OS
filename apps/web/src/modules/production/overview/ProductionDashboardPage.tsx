import { useQuery } from '@tanstack/react-query';
import { Link } from '@tanstack/react-router';
import { Button, Card, CardContent, CardHeader, CardTitle } from '@naswood/ui';
import { getDashboard } from '@/api/business';
import { useI18n } from '@/i18n';

interface ProductionDashboardDto {
  openProductionOrders?: number;
  OpenProductionOrders?: number;
  activeWorkOrders?: number;
  ActiveWorkOrders?: number;
  wipQuantity?: number;
  WipQuantity?: number;
  scrapRate?: number;
  ScrapRate?: number;
}

function metric(data: ProductionDashboardDto | undefined, camel: keyof ProductionDashboardDto, pascal: keyof ProductionDashboardDto) {
  return data?.[camel] ?? data?.[pascal] ?? 0;
}

export function ProductionDashboardPage() {
  const { t } = useI18n();
  const query = useQuery({
    queryKey: ['business', 'production/dashboard'],
    queryFn: () => getDashboard<ProductionDashboardDto>('production/dashboard'),
  });
  const data = query.data;

  const kpis = [
    { camel: 'openProductionOrders' as const, pascal: 'OpenProductionOrders' as const, label: t('production.openOrders'), path: '/production/planning/orders' },
    { camel: 'activeWorkOrders' as const, pascal: 'ActiveWorkOrders' as const, label: t('production.activeWos'), path: '/production/planning/work-orders' },
    { camel: 'wipQuantity' as const, pascal: 'WipQuantity' as const, label: t('production.wipQty'), path: '/production/execution/wip' },
    { camel: 'scrapRate' as const, pascal: 'ScrapRate' as const, label: t('production.scrapRate'), path: '/production/execution/scrap' },
  ];

  const planning = [
    [t('production.productionOrders'), '/production/planning/orders'],
    [t('production.workOrders'), '/production/planning/work-orders'],
    [t('production.dispatch'), '/production/planning/dispatch'],
    [t('production.scheduling'), '/production/planning/scheduling'],
  ] as const;

  const execution = [
    [t('production.operatorTerminal'), '/production/execution/operator-terminal'],
    [t('production.machinePanel'), '/production/execution/machine-panel'],
    [t('production.confirmation'), '/production/execution/confirmation'],
    [t('production.wip'), '/production/execution/wip'],
  ] as const;

  return (
    <div className="space-y-6">
      <div className="flex flex-wrap items-start justify-between gap-3">
        <div>
          <p className="text-xs font-medium text-[var(--text-muted)]">PRD-001</p>
          <h2 className="text-xl font-semibold tracking-tight">{t('production.dashTitle')}</h2>
          <p className="mt-1 text-sm text-[var(--text-secondary)]">{t('production.dashDesc')}</p>
        </div>
        <Button type="button" variant="secondary" onClick={() => void query.refetch()} disabled={query.isFetching}>
          {t('refresh')}
        </Button>
      </div>

      {query.isError ? <p className="text-sm text-[var(--color-danger)]">{(query.error as Error).message}</p> : null}

      <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
        {kpis.map((item) => (
          <Link key={item.pascal} to={item.path} className="block transition hover:-translate-y-0.5">
            <Card>
              <CardHeader>
                <CardTitle className="text-base">{item.label}</CardTitle>
              </CardHeader>
              <CardContent className="text-2xl font-semibold tabular-nums">
                {query.isLoading ? '…' : metric(data, item.camel, item.pascal)}
              </CardContent>
            </Card>
          </Link>
        ))}
      </div>

      <div className="grid gap-4 lg:grid-cols-2">
        <Card>
          <CardHeader>
            <CardTitle>{t('production.planning')}</CardTitle>
          </CardHeader>
          <CardContent className="flex flex-wrap gap-2">
            {planning.map(([label, path]) => (
              <Link
                key={path}
                to={path}
                className="rounded-full bg-[var(--color-surface)] px-3 py-1.5 text-sm ring-1 ring-[var(--border-default)] hover:text-[var(--color-primary)]"
              >
                {label}
              </Link>
            ))}
          </CardContent>
        </Card>
        <Card>
          <CardHeader>
            <CardTitle>{t('production.execution')}</CardTitle>
          </CardHeader>
          <CardContent className="flex flex-wrap gap-2">
            {execution.map(([label, path]) => (
              <Link
                key={path}
                to={path}
                className="rounded-full bg-[var(--color-surface)] px-3 py-1.5 text-sm ring-1 ring-[var(--border-default)] hover:text-[var(--color-primary)]"
              >
                {label}
              </Link>
            ))}
          </CardContent>
        </Card>
      </div>
    </div>
  );
}

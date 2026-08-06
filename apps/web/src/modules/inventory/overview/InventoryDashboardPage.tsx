import { useQuery } from '@tanstack/react-query';
import { Link } from '@tanstack/react-router';
import { Button, Card, CardContent, CardHeader, CardTitle } from '@naswood/ui';
import { getDashboard } from '@/api/business';
import { useI18n } from '@/i18n';

interface InventoryDashboardDto {
  materialCount?: number;
  MaterialCount?: number;
  warehouseCount?: number;
  WarehouseCount?: number;
  locationCount?: number;
  LocationCount?: number;
  balanceRows?: number;
  BalanceRows?: number;
  quantityOnHand?: number;
  QuantityOnHand?: number;
  quantityReserved?: number;
  QuantityReserved?: number;
  quantityAvailable?: number;
  QuantityAvailable?: number;
  openGoodsReceipts?: number;
  OpenGoodsReceipts?: number;
  openGoodsIssues?: number;
  OpenGoodsIssues?: number;
  openTransfers?: number;
  OpenTransfers?: number;
  openCounts?: number;
  OpenCounts?: number;
}

function metric(data: InventoryDashboardDto | undefined, camel: keyof InventoryDashboardDto, pascal: keyof InventoryDashboardDto) {
  return data?.[camel] ?? data?.[pascal] ?? 0;
}

export function InventoryDashboardPage() {
  const { t } = useI18n();
  const query = useQuery({
    queryKey: ['business', 'inventory/dashboard'],
    queryFn: () => getDashboard<InventoryDashboardDto>('inventory/dashboard'),
  });

  const data = query.data;

  const kpi = [
    { camel: 'quantityOnHand' as const, pascal: 'QuantityOnHand' as const, label: t('inventory.onHand'), path: '/inventory/stock/balances' },
    { camel: 'quantityReserved' as const, pascal: 'QuantityReserved' as const, label: t('inventory.reserved'), path: '/inventory/stock/balances' },
    { camel: 'quantityAvailable' as const, pascal: 'QuantityAvailable' as const, label: t('inventory.available'), path: '/inventory/stock/balances' },
    { camel: 'balanceRows' as const, pascal: 'BalanceRows' as const, label: t('inventory.balanceRows'), path: '/inventory/stock/balances' },
  ];

  const queues = [
    { camel: 'openGoodsReceipts' as const, pascal: 'OpenGoodsReceipts' as const, label: t('inventory.openGoodsReceipts'), path: '/inventory/operations/goods-receipts' },
    { camel: 'openGoodsIssues' as const, pascal: 'OpenGoodsIssues' as const, label: t('inventory.openGoodsIssues'), path: '/inventory/operations/goods-issues' },
    { camel: 'openTransfers' as const, pascal: 'OpenTransfers' as const, label: t('inventory.openTransfers'), path: '/inventory/operations/transfers' },
    { camel: 'openCounts' as const, pascal: 'OpenCounts' as const, label: t('inventory.openCounts'), path: '/inventory/counts/cycle-counts' },
  ];

  const shortcuts = [
    [t('inventory.stockBalance'), '/inventory/stock/balances'],
    [t('inventory.lots'), '/inventory/stock/lots'],
    [t('inventory.goodsReceipt'), '/inventory/operations/goods-receipts'],
    [t('inventory.cycleCount'), '/inventory/counts/cycle-counts'],
    [t('inventory.reports'), '/inventory/reports'],
  ] as const;

  return (
    <div className="space-y-6">
      <div className="flex flex-wrap items-start justify-between gap-3">
        <div>
          <p className="text-xs font-medium text-[var(--text-muted)]">INV-001</p>
          <h2 className="text-xl font-semibold tracking-tight">{t('inventory.dashTitle')}</h2>
          <p className="mt-1 text-sm text-[var(--text-secondary)]">{t('inventory.dashDesc')}</p>
        </div>
        <Button type="button" variant="secondary" onClick={() => void query.refetch()} disabled={query.isFetching}>
          {t('refresh')}
        </Button>
      </div>

      {query.isError ? (
        <p className="text-sm text-[var(--color-danger)]">{(query.error as Error).message}</p>
      ) : null}

      <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
        {kpi.map((item) => (
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
            <CardTitle>{t('inventory.operationsQueues')}</CardTitle>
          </CardHeader>
          <CardContent className="space-y-2">
            {queues.map((item) => (
              <Link
                key={item.pascal}
                to={item.path}
                className="flex items-center justify-between rounded-[var(--radius-md)] border border-[var(--border-default)] px-3 py-2 text-sm hover:bg-[var(--color-surface-hover)]"
              >
                <span>{item.label}</span>
                <span className="font-semibold tabular-nums">
                  {query.isLoading ? '…' : metric(data, item.camel, item.pascal)}
                </span>
              </Link>
            ))}
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>{t('inventory.masterFootprint')}</CardTitle>
          </CardHeader>
          <CardContent className="grid gap-3 sm:grid-cols-3">
            <MetricLink label={t('inventory.materials')} value={metric(data, 'materialCount', 'MaterialCount')} path="/inventory/master-data/materials" loading={query.isLoading} />
            <MetricLink label={t('inventory.warehouses')} value={metric(data, 'warehouseCount', 'WarehouseCount')} path="/inventory/master-data/warehouses" loading={query.isLoading} />
            <MetricLink label={t('inventory.locations')} value={metric(data, 'locationCount', 'LocationCount')} path="/inventory/master-data/locations" loading={query.isLoading} />
          </CardContent>
        </Card>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>{t('inventory.shortcuts')}</CardTitle>
        </CardHeader>
        <CardContent className="flex flex-wrap gap-2">
          {shortcuts.map(([label, path]) => (
            <Link
              key={path}
              to={path}
              className="rounded-full bg-[var(--color-surface)] px-3 py-1.5 text-sm text-[var(--text-secondary)] ring-1 ring-[var(--border-default)] hover:text-[var(--text-primary)]"
            >
              {label}
            </Link>
          ))}
        </CardContent>
      </Card>
    </div>
  );
}

function MetricLink({
  label,
  value,
  path,
  loading,
}: {
  label: string;
  value?: number;
  path: string;
  loading: boolean;
}) {
  return (
    <Link to={path} className="rounded-[var(--radius-md)] border border-[var(--border-default)] p-3 hover:bg-[var(--color-surface-hover)]">
      <p className="text-xs text-[var(--text-muted)]">{label}</p>
      <p className="mt-1 text-xl font-semibold tabular-nums">{loading ? '…' : (value ?? 0)}</p>
    </Link>
  );
}

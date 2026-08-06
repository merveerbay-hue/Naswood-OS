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

/** INV-001 — Warehouse Command Center (not a KPI page). */
export function InventoryDashboardPage() {
  const { t } = useI18n();
  const query = useQuery({
    queryKey: ['business', 'inventory/dashboard'],
    queryFn: () => getDashboard<InventoryDashboardDto>('inventory/dashboard'),
  });

  const data = query.data;

  const actions = [
    { label: t('inventory.cmdReceive'), path: '/inventory/operations/receive', primary: true },
    { label: t('inventory.cmdIssue'), path: '/inventory/operations/issue', primary: false },
    { label: t('inventory.cmdTransfer'), path: '/inventory/operations/transfer', primary: false },
    { label: t('inventory.cmdCount'), path: '/inventory/counts/start', primary: false },
  ] as const;

  const queues = [
    {
      camel: 'openGoodsReceipts' as const,
      pascal: 'OpenGoodsReceipts' as const,
      label: t('inventory.queueReceiving'),
      hint: t('inventory.queueReceivingHint'),
      path: '/inventory/operations/goods-receipts',
      actionPath: '/inventory/operations/receive',
      actionLabel: t('inventory.cmdReceive'),
    },
    {
      camel: 'openGoodsIssues' as const,
      pascal: 'OpenGoodsIssues' as const,
      label: t('inventory.queueIssues'),
      hint: t('inventory.queueIssuesHint'),
      path: '/inventory/operations/goods-issues',
      actionPath: '/inventory/operations/issue',
      actionLabel: t('inventory.cmdIssue'),
    },
    {
      camel: 'openTransfers' as const,
      pascal: 'OpenTransfers' as const,
      label: t('inventory.queueTransfers'),
      hint: t('inventory.queueTransfersHint'),
      path: '/inventory/operations/transfers',
      actionPath: '/inventory/operations/transfer',
      actionLabel: t('inventory.cmdTransfer'),
    },
    {
      camel: 'openCounts' as const,
      pascal: 'OpenCounts' as const,
      label: t('inventory.queueCounts'),
      hint: t('inventory.queueCountsHint'),
      path: '/inventory/counts/cycle-counts',
      actionPath: '/inventory/counts/start',
      actionLabel: t('inventory.cmdCount'),
    },
  ];

  const exceptions = [
    {
      label: t('inventory.exNegative'),
      detail: t('inventory.exNegativeHint'),
      path: '/inventory/stock/balances',
    },
    {
      label: t('inventory.exHold'),
      detail: t('inventory.exHoldHint'),
      path: '/inventory/stock/lots',
    },
    {
      label: t('inventory.exCapacity'),
      detail: t('inventory.exCapacityHint'),
      path: '/inventory/master-data/warehouses',
    },
  ];

  const statusStrip = [
    { camel: 'quantityAvailable' as const, pascal: 'QuantityAvailable' as const, label: t('inventory.available'), path: '/inventory/stock/balances' },
    { camel: 'quantityReserved' as const, pascal: 'QuantityReserved' as const, label: t('inventory.reserved'), path: '/inventory/stock/balances' },
    { camel: 'quantityOnHand' as const, pascal: 'QuantityOnHand' as const, label: t('inventory.onHand'), path: '/inventory/stock/balances' },
    { camel: 'openGoodsReceipts' as const, pascal: 'OpenGoodsReceipts' as const, label: t('inventory.inboundOpen'), path: '/inventory/operations/goods-receipts' },
  ];

  return (
    <div className="space-y-5">
      <div className="flex flex-wrap items-start justify-between gap-3 border-b border-[var(--border-default)] pb-3">
        <div>
          <p className="text-xs font-medium text-[var(--text-muted)]">INV-001 · {t('inventory.cmdScreenType')}</p>
          <h2 className="text-xl font-semibold tracking-tight">{t('inventory.dashTitle')}</h2>
          <p className="mt-1 max-w-2xl text-sm text-[var(--text-secondary)]">{t('inventory.dashDesc')}</p>
        </div>
        <Button type="button" variant="secondary" onClick={() => void query.refetch()} disabled={query.isFetching}>
          {t('refresh')}
        </Button>
      </div>

      {query.isError ? (
        <p className="text-sm text-[var(--color-danger)]">{(query.error as Error).message}</p>
      ) : null}

      {/* Action bar — primary */}
      <div className="flex flex-wrap gap-2">
        {actions.map((a) => (
          <Link key={a.path} to={a.path}>
            <Button type="button" variant={a.primary ? 'default' : 'secondary'}>
              {a.label}
            </Button>
          </Link>
        ))}
      </div>

      <div className="grid gap-4 lg:grid-cols-[minmax(0,1.4fr)_minmax(0,1fr)]">
        {/* Live queues */}
        <Card>
          <CardHeader>
            <CardTitle>{t('inventory.liveQueues')}</CardTitle>
            <p className="text-xs text-[var(--text-muted)]">{t('inventory.liveQueuesHint')}</p>
          </CardHeader>
          <CardContent className="space-y-2">
            {queues.map((item) => {
              const n = metric(data, item.camel, item.pascal);
              return (
                <div
                  key={item.pascal}
                  className="flex flex-wrap items-center justify-between gap-2 rounded-[var(--radius-md)] border border-[var(--border-default)] px-3 py-2.5"
                >
                  <div className="min-w-0">
                    <Link to={item.path} className="text-sm font-medium text-[var(--text-primary)] hover:underline">
                      {item.label}
                    </Link>
                    <p className="text-xs text-[var(--text-muted)]">{item.hint}</p>
                  </div>
                  <div className="flex items-center gap-3">
                    <span className="text-lg font-semibold tabular-nums">{query.isLoading ? '…' : n}</span>
                    <Link to={item.actionPath} className="text-xs font-medium text-[var(--color-primary)] hover:underline">
                      {item.actionLabel}
                    </Link>
                  </div>
                </div>
              );
            })}
          </CardContent>
        </Card>

        {/* Exceptions */}
        <Card>
          <CardHeader>
            <CardTitle>{t('inventory.exceptions')}</CardTitle>
            <p className="text-xs text-[var(--text-muted)]">{t('inventory.exceptionsHint')}</p>
          </CardHeader>
          <CardContent className="space-y-2">
            {exceptions.map((ex) => (
              <Link
                key={ex.label}
                to={ex.path}
                className="block rounded-[var(--radius-md)] border border-[var(--border-default)] px-3 py-2.5 hover:bg-[var(--color-surface-hover)]"
              >
                <p className="text-sm font-medium text-[var(--color-danger)]">{ex.label}</p>
                <p className="text-xs text-[var(--text-muted)]">{ex.detail}</p>
              </Link>
            ))}
          </CardContent>
        </Card>
      </div>

      {/* Thin status — secondary, not hero KPIs */}
      <div>
        <p className="mb-2 text-[10px] font-semibold uppercase tracking-wide text-[var(--text-muted)]">
          {t('inventory.statusStrip')}
        </p>
        <div className="grid gap-2 sm:grid-cols-2 xl:grid-cols-4">
          {statusStrip.map((item) => (
            <Link
              key={item.pascal}
              to={item.path}
              className="rounded-md border border-[var(--border-default)] bg-[var(--color-surface)] px-3 py-2 hover:bg-[var(--color-surface-hover)]"
            >
              <p className="text-xs text-[var(--text-muted)]">{item.label}</p>
              <p className="text-lg font-semibold tabular-nums">
                {query.isLoading ? '…' : metric(data, item.camel, item.pascal)}
              </p>
            </Link>
          ))}
        </div>
      </div>

      {/* Dock board (demo operational surface) */}
      <Card>
        <CardHeader>
          <CardTitle>{t('inventory.dockBoard')}</CardTitle>
          <p className="text-xs text-[var(--text-muted)]">{t('inventory.dockBoardHint')}</p>
        </CardHeader>
        <CardContent>
          <div className="overflow-x-auto">
            <table className="w-full min-w-[560px] text-left text-sm">
              <thead className="text-xs text-[var(--text-muted)]">
                <tr>
                  <th className="pb-2 font-medium">{t('inventory.dockGate')}</th>
                  <th className="pb-2 font-medium">{t('inventory.dockTruck')}</th>
                  <th className="pb-2 font-medium">{t('inventory.dockSupplier')}</th>
                  <th className="pb-2 font-medium">{t('inventory.dockStage')}</th>
                  <th className="pb-2 font-medium" />
                </tr>
              </thead>
              <tbody className="divide-y divide-[var(--border-default)]">
                <tr>
                  <td className="py-2">2</td>
                  <td className="py-2 font-mono text-xs">34 ABC 123</td>
                  <td className="py-2">Nordic Timber Oy</td>
                  <td className="py-2">{t('inventory.dockStageDocs')}</td>
                  <td className="py-2 text-right">
                    <Link to="/inventory/operations/receive" className="text-xs font-medium text-[var(--color-primary)] hover:underline">
                      {t('inventory.openWorkbench')}
                    </Link>
                  </td>
                </tr>
                <tr>
                  <td className="py-2">1</td>
                  <td className="py-2 font-mono text-xs">06 XYZ 778</td>
                  <td className="py-2">ABC Forest</td>
                  <td className="py-2">{t('inventory.dockStageGate')}</td>
                  <td className="py-2 text-right">
                    <Link to="/inventory/operations/receive" className="text-xs font-medium text-[var(--color-primary)] hover:underline">
                      {t('inventory.openWorkbench')}
                    </Link>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </CardContent>
      </Card>

      <p className="text-xs text-[var(--text-muted)]">
        {t('inventory.cmdReportsNote')}{' '}
        <Link to="/inventory/reports" className="text-[var(--color-primary)] hover:underline">
          {t('inventory.reports')}
        </Link>
      </p>
    </div>
  );
}

import { useQuery } from '@tanstack/react-query';
import { Link } from '@tanstack/react-router';
import { Button, Card, CardContent, CardHeader, CardTitle } from '@naswood/ui';
import { getDashboard } from '@/api/business';

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

const KPI = [
  { camel: 'quantityOnHand', pascal: 'QuantityOnHand', label: 'On hand', path: '/inventory/stock/balances' },
  { camel: 'quantityReserved', pascal: 'QuantityReserved', label: 'Reserved', path: '/inventory/stock/balances' },
  { camel: 'quantityAvailable', pascal: 'QuantityAvailable', label: 'Available', path: '/inventory/stock/balances' },
  { camel: 'balanceRows', pascal: 'BalanceRows', label: 'Balance rows', path: '/inventory/stock/balances' },
] as const;

const QUEUES = [
  { camel: 'openGoodsReceipts', pascal: 'OpenGoodsReceipts', label: 'Open goods receipts', path: '/inventory/operations/goods-receipts' },
  { camel: 'openGoodsIssues', pascal: 'OpenGoodsIssues', label: 'Open goods issues', path: '/inventory/operations/goods-issues' },
  { camel: 'openTransfers', pascal: 'OpenTransfers', label: 'Open transfers', path: '/inventory/operations/transfers' },
  { camel: 'openCounts', pascal: 'OpenCounts', label: 'Open counts', path: '/inventory/counts/cycle-counts' },
] as const;

export function InventoryDashboardPage() {
  const query = useQuery({
    queryKey: ['business', 'inventory/dashboard'],
    queryFn: () => getDashboard<InventoryDashboardDto>('inventory/dashboard'),
  });

  const data = query.data;

  return (
    <div className="space-y-6">
      <div className="flex flex-wrap items-start justify-between gap-3">
        <div>
          <p className="text-xs font-medium text-[var(--text-muted)]">INV-001</p>
          <h2 className="text-xl font-semibold tracking-tight">Inventory Dashboard</h2>
          <p className="mt-1 text-sm text-[var(--text-secondary)]">
            Warehouse health cockpit — KPIs, operations queues, shortcuts into workspaces.
          </p>
        </div>
        <Button type="button" variant="secondary" onClick={() => void query.refetch()} disabled={query.isFetching}>
          Refresh
        </Button>
      </div>

      {query.isError ? (
        <p className="text-sm text-[var(--color-danger)]">{(query.error as Error).message}</p>
      ) : null}

      <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
        {KPI.map((item) => (
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
            <CardTitle>Operations queues</CardTitle>
          </CardHeader>
          <CardContent className="space-y-2">
            {QUEUES.map((item) => (
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
            <CardTitle>Master data footprint</CardTitle>
          </CardHeader>
          <CardContent className="grid gap-3 sm:grid-cols-3">
            <MetricLink label="Materials" value={metric(data, 'materialCount', 'MaterialCount')} path="/inventory/master-data/materials" loading={query.isLoading} />
            <MetricLink label="Warehouses" value={metric(data, 'warehouseCount', 'WarehouseCount')} path="/inventory/master-data/warehouses" loading={query.isLoading} />
            <MetricLink label="Locations" value={metric(data, 'locationCount', 'LocationCount')} path="/inventory/master-data/locations" loading={query.isLoading} />
          </CardContent>
        </Card>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Workspace shortcuts</CardTitle>
        </CardHeader>
        <CardContent className="flex flex-wrap gap-2">
          {[
            ['Stock Balance', '/inventory/stock/balances'],
            ['Lots', '/inventory/stock/lots'],
            ['Goods Receipt', '/inventory/operations/goods-receipts'],
            ['Cycle Count', '/inventory/counts/cycle-counts'],
            ['Reports', '/inventory/reports'],
          ].map(([label, path]) => (
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

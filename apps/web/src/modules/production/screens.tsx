import { useQuery } from '@tanstack/react-query';
import { Link, useParams } from '@tanstack/react-router';
import { Button, Card, CardContent, CardHeader, CardTitle } from '@naswood/ui';
import { searchResource } from '@/api/business';
import { EntityDetailScreen } from '@/modules/shared/entity/EntityDetailScreen';
import { EntityListScreen, type EntityField } from '@/modules/shared/entity/EntityListScreen';
import { StatusBadge } from '@/modules/shared/entity/StatusBadge';

const codeNameStatus: EntityField[] = [
  { key: 'Code', label: 'Code' },
  { key: 'Name', label: 'Name' },
  { key: 'Status', label: 'Status', status: true },
  { key: 'Notes', label: 'Notes' },
];

const bomFields: EntityField[] = [
  { key: 'Number', label: 'Number' },
  { key: 'MaterialCode', label: 'Material' },
  { key: 'Version', label: 'Version', type: 'number' },
  { key: 'Status', label: 'Status', status: true },
  { key: 'Notes', label: 'Notes' },
];

const routingFields = bomFields;

const machineFields: EntityField[] = [
  { key: 'Code', label: 'Code' },
  { key: 'Name', label: 'Name' },
  { key: 'WorkCenterCode', label: 'Work center' },
  { key: 'Status', label: 'Status', status: true },
  { key: 'OeeTarget', label: 'OEE target', type: 'number' },
];

const workCenterFields: EntityField[] = [
  { key: 'Code', label: 'Code' },
  { key: 'Name', label: 'Name' },
  { key: 'CapacityPerHour', label: 'Capacity/h', type: 'number' },
  { key: 'Status', label: 'Status', status: true },
];

function PlaceholderBoard({
  screenId,
  title,
  description,
  linkPath,
  linkLabel,
}: {
  screenId: string;
  title: string;
  description: string;
  linkPath: string;
  linkLabel: string;
}) {
  return (
    <div className="space-y-4">
      <div>
        <p className="text-xs font-medium text-[var(--text-muted)]">{screenId}</p>
        <h2 className="text-xl font-semibold tracking-tight">{title}</h2>
        <p className="mt-1 text-sm text-[var(--text-secondary)]">{description}</p>
      </div>
      <Card>
        <CardHeader>
          <CardTitle>Board surface</CardTitle>
        </CardHeader>
        <CardContent className="space-y-3 text-sm text-[var(--text-secondary)]">
          <p>Scheduler / capacity / dispatch board chrome lands here (Component Library · Scheduler / Kanban).</p>
          <Link to={linkPath} className="font-medium text-[var(--color-primary)] hover:underline">
            {linkLabel}
          </Link>
        </CardContent>
      </Card>
    </div>
  );
}

export function ProductionOrderListPage() {
  return (
    <EntityListScreen
      screenId="PRD-010"
      title="Production Orders"
      description="Planning · order library — release and schedule from Detail."
      route="production-orders"
      fields={codeNameStatus}
      detailPath={(id) => `/production/planning/orders/${id}`}
      createLabel="New order"
    />
  );
}

export function ProductionOrderDetailPage() {
  const { id } = useParams({ strict: false }) as { id: string };
  return (
    <EntityDetailScreen
      screenId="PRD-011"
      title="Production Order Detail"
      route="production-orders"
      id={id}
      listPath="/production/planning/orders"
      fields={codeNameStatus}
    />
  );
}

export function WorkOrderListPage() {
  return (
    <EntityListScreen
      screenId="PRD-012"
      title="Work Orders"
      description="Planning · executable WO queue."
      route="work-orders"
      fields={codeNameStatus}
      createLabel="New work order"
    />
  );
}

export function SchedulingPage() {
  return (
    <PlaceholderBoard
      screenId="PRD-021"
      title="Scheduling"
      description="Place orders on the time / resource axis."
      linkPath="/production/planning/orders"
      linkLabel="Open production orders"
    />
  );
}

export function CapacityPage() {
  return (
    <PlaceholderBoard
      screenId="PRD-022"
      title="Capacity Planning"
      description="Load vs capacity by work center / line."
      linkPath="/production/master-data/work-centers"
      linkLabel="Open work centers"
    />
  );
}

export function DispatchPage() {
  return (
    <PlaceholderBoard
      screenId="PRD-023"
      title="Dispatch List"
      description="Released work ready for the floor — Kanban/dispatch board."
      linkPath="/production/planning/work-orders"
      linkLabel="Open work orders"
    />
  );
}

export function OperatorTerminalPage() {
  const woQuery = useQuery({
    queryKey: ['business', 'work-orders', 'terminal'],
    queryFn: () => searchResource<Record<string, unknown>>('work-orders'),
  });

  return (
    <div className="space-y-4">
      <div className="flex flex-wrap items-start justify-between gap-3">
        <div>
          <p className="text-xs font-medium text-[var(--text-muted)]">PRD-013</p>
          <h2 className="text-xl font-semibold tracking-tight">Operator Terminal</h2>
          <p className="mt-1 text-sm text-[var(--text-secondary)]">
            Focused shop-floor surface — select WO, confirm, consume, scrap.
          </p>
        </div>
        <div className="flex flex-wrap gap-2">
          <Link to="/production/execution/confirmation">
            <Button type="button">Confirm</Button>
          </Link>
          <Link to="/production/execution/consumption">
            <Button type="button" variant="secondary">
              Consume
            </Button>
          </Link>
          <Link to="/production/execution/scrap">
            <Button type="button" variant="danger">
              Scrap
            </Button>
          </Link>
        </div>
      </div>

      <div className="grid gap-4 lg:grid-cols-3">
        <Card className="lg:col-span-2">
          <CardHeader>
            <CardTitle>Active work orders</CardTitle>
          </CardHeader>
          <CardContent className="space-y-2">
            {woQuery.isLoading ? (
              <p className="text-sm text-[var(--text-secondary)]">Loading…</p>
            ) : (woQuery.data?.items?.length ?? 0) === 0 ? (
              <p className="text-sm text-[var(--text-secondary)]">No work orders yet. Create from Planning.</p>
            ) : (
              (woQuery.data?.items ?? []).slice(0, 8).map((row) => {
                const id = String(row.id ?? row.Id ?? '');
                const code = String(row.code ?? row.Code ?? '—');
                const name = String(row.name ?? row.Name ?? '');
                const status = String(row.status ?? row.Status ?? '');
                return (
                  <div
                    key={id}
                    className="flex items-center justify-between rounded-[var(--radius-md)] border border-[var(--border-default)] px-3 py-3"
                  >
                    <div>
                      <p className="font-medium">{code}</p>
                      <p className="text-sm text-[var(--text-secondary)]">{name}</p>
                    </div>
                    <StatusBadge status={status} />
                  </div>
                );
              })
            )}
          </CardContent>
        </Card>
        <Card>
          <CardHeader>
            <CardTitle>Station context</CardTitle>
          </CardHeader>
          <CardContent className="space-y-2 text-sm text-[var(--text-secondary)]">
            <p>Plant · Line · Machine selection (shell context) — MVP shows module-wide WO list.</p>
            <Link to="/production/execution/machine-panel" className="text-[var(--color-primary)] hover:underline">
              Open machine panel
            </Link>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}

export function MachinePanelPage() {
  return (
    <EntityListScreen
      screenId="PRD-024"
      title="Machine Panel"
      description="Execution · machine status and assigned work."
      route="machines"
      fields={machineFields}
      createLabel="New machine"
    />
  );
}

export function ConsumptionPage() {
  return (
    <EntityListScreen
      screenId="PRD-014"
      title="Material Consumption"
      description="Execution · post material issues against WO."
      route="material-consumptions"
      fields={codeNameStatus}
      createLabel="New consumption"
    />
  );
}

export function ConfirmationPage() {
  return (
    <EntityListScreen
      screenId="PRD-015"
      title="Production Confirmation"
      description="Execution · quantity / time confirmations."
      route="production-confirmations"
      fields={codeNameStatus}
      createLabel="New confirmation"
    />
  );
}

export function WipPage() {
  return (
    <EntityListScreen
      screenId="PRD-016"
      title="WIP Tracking"
      description="Execution · work-in-process visibility."
      route="wips"
      fields={codeNameStatus}
      createLabel="New WIP row"
    />
  );
}

export function PackagingPage() {
  return (
    <EntityListScreen
      screenId="PRD-017"
      title="Packaging"
      description="Execution · pack finished output."
      route="packagings"
      fields={codeNameStatus}
      createLabel="New packaging"
    />
  );
}

export function FinishedGoodsPage() {
  return (
    <EntityListScreen
      screenId="PRD-018"
      title="Finished Goods"
      description="Execution · FG receipt into inventory."
      route="finished-goods"
      fields={codeNameStatus}
      createLabel="New FG"
    />
  );
}

export function ScrapPage() {
  return (
    <EntityListScreen
      screenId="PRD-027"
      title="Scrap"
      description="Execution · scrap posting and reasons."
      route="scraps"
      fields={codeNameStatus}
      createLabel="New scrap"
    />
  );
}

export function ReworkPage() {
  return (
    <EntityListScreen
      screenId="PRD-028"
      title="Rework"
      description="Execution · rework loops."
      route="reworks"
      fields={codeNameStatus}
      createLabel="New rework"
    />
  );
}

export function BomListPage() {
  return (
    <EntityListScreen
      screenId="PRD-002"
      title="BOM"
      description="Master data · bill of materials."
      route="boms"
      fields={bomFields}
      detailPath={(id) => `/production/master-data/boms/${id}`}
      createLabel="New BOM"
    />
  );
}

export function BomDetailPage() {
  const { id } = useParams({ strict: false }) as { id: string };
  return (
    <EntityDetailScreen
      screenId="PRD-003"
      title="BOM Detail"
      route="boms"
      id={id}
      listPath="/production/master-data/boms"
      fields={bomFields}
    />
  );
}

export function RoutingListPage() {
  return (
    <EntityListScreen
      screenId="PRD-004"
      title="Routing"
      description="Master data · process routes."
      route="routings"
      fields={routingFields}
      createLabel="New routing"
    />
  );
}

export function OperationsMasterPage() {
  return (
    <EntityListScreen
      screenId="PRD-025"
      title="Operations"
      description="Master data · standard operations."
      route="operations"
      fields={codeNameStatus}
      createLabel="New operation"
    />
  );
}

export function MachinesMasterPage() {
  return (
    <EntityListScreen
      screenId="PRD-007"
      title="Machines"
      description="Master data · machine register."
      route="machines"
      fields={machineFields}
      createLabel="New machine"
    />
  );
}

export function WorkCentersPage() {
  return (
    <EntityListScreen
      screenId="PRD-006"
      title="Work Centers"
      description="Master data · capacity centers."
      route="work-centers"
      fields={workCenterFields}
      createLabel="New work center"
    />
  );
}

export function ProductionLinesPage() {
  return (
    <EntityListScreen
      screenId="PRD-026"
      title="Production Lines"
      description="Master data · lines."
      route="production-lines"
      fields={codeNameStatus}
      createLabel="New line"
    />
  );
}

export function ShiftsPage() {
  return (
    <EntityListScreen
      screenId="PRD-008"
      title="Shifts"
      description="Master data · shift definitions."
      route="shifts"
      fields={codeNameStatus}
      createLabel="New shift"
    />
  );
}

export function CalendarsPage() {
  return (
    <EntityListScreen
      screenId="PRD-009"
      title="Calendars"
      description="Master data · working calendars."
      route="calendars"
      fields={codeNameStatus}
      createLabel="New calendar"
    />
  );
}

export function ToolingsPage() {
  return (
    <EntityListScreen
      screenId="PRD-Tooling"
      title="Tooling"
      description="Master data · tools and assemblies."
      route="toolings"
      fields={codeNameStatus}
      createLabel="New tooling"
    />
  );
}

export function ProductionReportsPage() {
  return (
    <div className="space-y-4">
      <div>
        <p className="text-xs font-medium text-[var(--text-muted)]">PRD-020</p>
        <h2 className="text-xl font-semibold tracking-tight">Production Reports</h2>
      </div>
      <div className="grid gap-4 md:grid-cols-3">
        {['Order status', 'WO throughput', 'Scrap by reason'].map((title) => (
          <Card key={title}>
            <CardHeader>
              <CardTitle className="text-base">{title}</CardTitle>
            </CardHeader>
            <CardContent className="text-sm text-[var(--text-secondary)]">Report launcher — wire to report engine next.</CardContent>
          </Card>
        ))}
      </div>
    </div>
  );
}

export function ProductionAnalyticsPage() {
  return (
    <div className="space-y-4">
      <div>
        <p className="text-xs font-medium text-[var(--text-muted)]">PRD-019</p>
        <h2 className="text-xl font-semibold tracking-tight">Production Analytics</h2>
        <p className="mt-1 text-sm text-[var(--text-secondary)]">OEE and trend visuals — Design System charts.</p>
      </div>
      <Card>
        <CardContent className="py-8 text-sm text-[var(--text-secondary)]">Analytics canvas placeholder for this slice.</CardContent>
      </Card>
    </div>
  );
}

export function ProductionSettingsPage() {
  return (
    <div className="space-y-4">
      <div>
        <p className="text-xs font-medium text-[var(--text-muted)]">PRD-029</p>
        <h2 className="text-xl font-semibold tracking-tight">Production Settings</h2>
      </div>
      <Card>
        <CardContent className="py-8 text-sm text-[var(--text-secondary)]">
          Module settings (default plant, posting rules) — deferred behind Platform Settings patterns.
        </CardContent>
      </Card>
    </div>
  );
}

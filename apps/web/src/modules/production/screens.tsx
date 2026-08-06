import { useQuery } from '@tanstack/react-query';
import { Link, useParams } from '@tanstack/react-router';
import { Button, Card, CardContent, CardHeader, CardTitle } from '@naswood/ui';
import { searchResource } from '@/api/business';
import { useI18n } from '@/i18n';
import { EntityDetailScreen } from '@/modules/shared/entity/EntityDetailScreen';
import { EntityListScreen, type EntityField } from '@/modules/shared/entity/EntityListScreen';
import { StatusBadge } from '@/modules/shared/entity/StatusBadge';

function usePrdFields() {
  const { t } = useI18n();
  const codeNameStatus: EntityField[] = [
    { key: 'Code', label: t('production.fields.code') },
    { key: 'Name', label: t('production.fields.name') },
    { key: 'Status', label: t('production.fields.status'), status: true },
    { key: 'Notes', label: t('production.fields.notes') },
  ];
  const bomFields: EntityField[] = [
    { key: 'Number', label: t('production.fields.number') },
    { key: 'MaterialCode', label: t('production.fields.material') },
    { key: 'Version', label: t('production.fields.version'), type: 'number' },
    { key: 'Status', label: t('production.fields.status'), status: true },
    { key: 'Notes', label: t('production.fields.notes') },
  ];
  const machineFields: EntityField[] = [
    { key: 'Code', label: t('production.fields.code') },
    { key: 'Name', label: t('production.fields.name') },
    { key: 'WorkCenterCode', label: t('production.fields.workCenter') },
    { key: 'Status', label: t('production.fields.status'), status: true },
    { key: 'OeeTarget', label: t('production.fields.oeeTarget'), type: 'number' },
  ];
  const workCenterFields: EntityField[] = [
    { key: 'Code', label: t('production.fields.code') },
    { key: 'Name', label: t('production.fields.name') },
    { key: 'CapacityPerHour', label: t('production.fields.capacityPerHour'), type: 'number' },
    { key: 'Status', label: t('production.fields.status'), status: true },
  ];
  return { codeNameStatus, bomFields, machineFields, workCenterFields };
}

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
  const { t } = useI18n();
  return (
    <div className="space-y-4">
      <div>
        <p className="text-xs font-medium text-[var(--text-muted)]">{screenId}</p>
        <h2 className="text-xl font-semibold tracking-tight">{title}</h2>
        <p className="mt-1 text-sm text-[var(--text-secondary)]">{description}</p>
      </div>
      <Card>
        <CardHeader>
          <CardTitle>{t('production.boardSurface')}</CardTitle>
        </CardHeader>
        <CardContent className="space-y-3 text-sm text-[var(--text-secondary)]">
          <p>{t('production.boardHint')}</p>
          <Link to={linkPath} className="font-medium text-[var(--color-primary)] hover:underline">
            {linkLabel}
          </Link>
        </CardContent>
      </Card>
    </div>
  );
}

export function ProductionOrderListPage() {
  const { t } = useI18n();
  const { codeNameStatus } = usePrdFields();
  return (
    <EntityListScreen
      screenId="PRD-010"
      title={t('production.poTitle')}
      description={t('production.poDesc')}
      route="production-orders"
      fields={codeNameStatus}
      detailPath={(id) => `/production/planning/orders/${id}`}
      createLabel={t('production.newOrder')}
    />
  );
}

export function ProductionOrderDetailPage() {
  const { t } = useI18n();
  const { codeNameStatus } = usePrdFields();
  const { id } = useParams({ strict: false }) as { id: string };
  return (
    <EntityDetailScreen
      screenId="PRD-011"
      title={t('production.poDetail')}
      route="production-orders"
      id={id}
      listPath="/production/planning/orders"
      fields={codeNameStatus}
    />
  );
}

export function WorkOrderListPage() {
  const { t } = useI18n();
  const { codeNameStatus } = usePrdFields();
  return (
    <EntityListScreen
      screenId="PRD-012"
      title={t('production.woTitle')}
      description={t('production.woDesc')}
      route="work-orders"
      fields={codeNameStatus}
      createLabel={t('production.newWo')}
    />
  );
}

export function SchedulingPage() {
  const { t } = useI18n();
  return (
    <PlaceholderBoard
      screenId="PRD-021"
      title={t('production.schedulingTitle')}
      description={t('production.schedulingDesc')}
      linkPath="/production/planning/orders"
      linkLabel={t('production.openOrdersLink')}
    />
  );
}

export function CapacityPage() {
  const { t } = useI18n();
  return (
    <PlaceholderBoard
      screenId="PRD-022"
      title={t('production.capacityTitle')}
      description={t('production.capacityDesc')}
      linkPath="/production/master-data/work-centers"
      linkLabel={t('production.openWcLink')}
    />
  );
}

export function DispatchPage() {
  const { t } = useI18n();
  return (
    <PlaceholderBoard
      screenId="PRD-023"
      title={t('production.dispatchTitle')}
      description={t('production.dispatchDesc')}
      linkPath="/production/planning/work-orders"
      linkLabel={t('production.openWoLink')}
    />
  );
}

export function OperatorTerminalPage() {
  const { t } = useI18n();
  const woQuery = useQuery({
    queryKey: ['business', 'work-orders', 'terminal'],
    queryFn: () => searchResource<Record<string, unknown>>('work-orders'),
  });

  return (
    <div className="space-y-4">
      <div className="flex flex-wrap items-start justify-between gap-3">
        <div>
          <p className="text-xs font-medium text-[var(--text-muted)]">PRD-013</p>
          <h2 className="text-xl font-semibold tracking-tight">{t('production.operatorTitle')}</h2>
          <p className="mt-1 text-sm text-[var(--text-secondary)]">{t('production.operatorDesc')}</p>
        </div>
        <div className="flex flex-wrap gap-2">
          <Link to="/production/execution/confirmation">
            <Button type="button">{t('production.confirm')}</Button>
          </Link>
          <Link to="/production/execution/consumption">
            <Button type="button" variant="secondary">
              {t('production.consume')}
            </Button>
          </Link>
          <Link to="/production/execution/scrap">
            <Button type="button" variant="danger">
              {t('production.scrap')}
            </Button>
          </Link>
        </div>
      </div>

      <div className="grid gap-4 lg:grid-cols-3">
        <Card className="lg:col-span-2">
          <CardHeader>
            <CardTitle>{t('production.activeWosTitle')}</CardTitle>
          </CardHeader>
          <CardContent className="space-y-2">
            {woQuery.isLoading ? (
              <p className="text-sm text-[var(--text-secondary)]">{t('loading')}</p>
            ) : (woQuery.data?.items?.length ?? 0) === 0 ? (
              <p className="text-sm text-[var(--text-secondary)]">{t('production.noWos')}</p>
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
            <CardTitle>{t('production.stationContext')}</CardTitle>
          </CardHeader>
          <CardContent className="space-y-2 text-sm text-[var(--text-secondary)]">
            <p>{t('production.stationHint')}</p>
            <Link to="/production/execution/machine-panel" className="text-[var(--color-primary)] hover:underline">
              {t('production.openMachinePanel')}
            </Link>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}

export function MachinePanelPage() {
  const { t } = useI18n();
  const { machineFields } = usePrdFields();
  return (
    <EntityListScreen
      screenId="PRD-024"
      title={t('production.machinePanel')}
      description={t('production.machinePanelDesc')}
      route="machines"
      fields={machineFields}
      createLabel={t('production.newMachine')}
    />
  );
}

export function ConsumptionPage() {
  const { t } = useI18n();
  const { codeNameStatus } = usePrdFields();
  return (
    <EntityListScreen
      screenId="PRD-014"
      title={t('production.consumption')}
      description={t('production.consumptionDesc')}
      route="material-consumptions"
      fields={codeNameStatus}
      createLabel={t('production.newConsumption')}
    />
  );
}

export function ConfirmationPage() {
  const { t } = useI18n();
  const { codeNameStatus } = usePrdFields();
  return (
    <EntityListScreen
      screenId="PRD-015"
      title={t('production.confirmation')}
      description={t('production.confirmationDesc')}
      route="production-confirmations"
      fields={codeNameStatus}
      createLabel={t('production.newConfirmation')}
    />
  );
}

export function WipPage() {
  const { t } = useI18n();
  const { codeNameStatus } = usePrdFields();
  return (
    <EntityListScreen
      screenId="PRD-016"
      title={t('production.wip')}
      description={t('production.wipDesc')}
      route="wips"
      fields={codeNameStatus}
      createLabel={t('production.newWip')}
    />
  );
}

export function PackagingPage() {
  const { t } = useI18n();
  const { codeNameStatus } = usePrdFields();
  return (
    <EntityListScreen
      screenId="PRD-017"
      title={t('production.packaging')}
      description={t('production.packagingDesc')}
      route="packagings"
      fields={codeNameStatus}
      createLabel={t('production.newPackaging')}
    />
  );
}

export function FinishedGoodsPage() {
  const { t } = useI18n();
  const { codeNameStatus } = usePrdFields();
  return (
    <EntityListScreen
      screenId="PRD-018"
      title={t('production.finishedGoods')}
      description={t('production.fgDesc')}
      route="finished-goods"
      fields={codeNameStatus}
      createLabel={t('production.newFg')}
    />
  );
}

export function ScrapPage() {
  const { t } = useI18n();
  const { codeNameStatus } = usePrdFields();
  return (
    <EntityListScreen
      screenId="PRD-027"
      title={t('production.scrap')}
      description={t('production.scrapDesc')}
      route="scraps"
      fields={codeNameStatus}
      createLabel={t('production.newScrap')}
    />
  );
}

export function ReworkPage() {
  const { t } = useI18n();
  const { codeNameStatus } = usePrdFields();
  return (
    <EntityListScreen
      screenId="PRD-028"
      title={t('production.rework')}
      description={t('production.reworkDesc')}
      route="reworks"
      fields={codeNameStatus}
      createLabel={t('production.newRework')}
    />
  );
}

export function BomListPage() {
  const { t } = useI18n();
  const { bomFields } = usePrdFields();
  return (
    <EntityListScreen
      screenId="PRD-002"
      title={t('production.bom')}
      description={t('production.bomDesc')}
      route="boms"
      fields={bomFields}
      detailPath={(id) => `/production/master-data/boms/${id}`}
      createLabel={t('production.newBom')}
    />
  );
}

export function BomDetailPage() {
  const { t } = useI18n();
  const { bomFields } = usePrdFields();
  const { id } = useParams({ strict: false }) as { id: string };
  return (
    <EntityDetailScreen
      screenId="PRD-003"
      title={t('production.bomDetail')}
      route="boms"
      id={id}
      listPath="/production/master-data/boms"
      fields={bomFields}
    />
  );
}

export function RoutingListPage() {
  const { t } = useI18n();
  const { bomFields } = usePrdFields();
  return (
    <EntityListScreen
      screenId="PRD-004"
      title={t('production.routing')}
      description={t('production.routingDesc')}
      route="routings"
      fields={bomFields}
      createLabel={t('production.newRouting')}
    />
  );
}

export function OperationsMasterPage() {
  const { t } = useI18n();
  const { codeNameStatus } = usePrdFields();
  return (
    <EntityListScreen
      screenId="PRD-025"
      title={t('production.operations')}
      description={t('production.operationsDesc')}
      route="operations"
      fields={codeNameStatus}
      createLabel={t('production.newOperation')}
    />
  );
}

export function MachinesMasterPage() {
  const { t } = useI18n();
  const { machineFields } = usePrdFields();
  return (
    <EntityListScreen
      screenId="PRD-007"
      title={t('production.machines')}
      description={t('production.machinesDesc')}
      route="machines"
      fields={machineFields}
      createLabel={t('production.newMachine')}
    />
  );
}

export function WorkCentersPage() {
  const { t } = useI18n();
  const { workCenterFields } = usePrdFields();
  return (
    <EntityListScreen
      screenId="PRD-006"
      title={t('production.workCenters')}
      description={t('production.workCentersDesc')}
      route="work-centers"
      fields={workCenterFields}
      createLabel={t('production.newWorkCenter')}
    />
  );
}

export function ProductionLinesPage() {
  const { t } = useI18n();
  const { codeNameStatus } = usePrdFields();
  return (
    <EntityListScreen
      screenId="PRD-026"
      title={t('production.lines')}
      description={t('production.linesDesc')}
      route="production-lines"
      fields={codeNameStatus}
      createLabel={t('production.newLine')}
    />
  );
}

export function ShiftsPage() {
  const { t } = useI18n();
  const { codeNameStatus } = usePrdFields();
  return (
    <EntityListScreen
      screenId="PRD-008"
      title={t('production.shifts')}
      description={t('production.shiftsDesc')}
      route="shifts"
      fields={codeNameStatus}
      createLabel={t('production.newShift')}
    />
  );
}

export function CalendarsPage() {
  const { t } = useI18n();
  const { codeNameStatus } = usePrdFields();
  return (
    <EntityListScreen
      screenId="PRD-009"
      title={t('production.calendars')}
      description={t('production.calendarsDesc')}
      route="calendars"
      fields={codeNameStatus}
      createLabel={t('production.newCalendar')}
    />
  );
}

export function ToolingsPage() {
  const { t } = useI18n();
  const { codeNameStatus } = usePrdFields();
  return (
    <EntityListScreen
      screenId="PRD-Tooling"
      title={t('production.tooling')}
      description={t('production.toolingDesc')}
      route="toolings"
      fields={codeNameStatus}
      createLabel={t('production.newTooling')}
    />
  );
}

export function ProductionReportsPage() {
  const { t } = useI18n();
  const reports = [
    t('production.reportOrderStatus'),
    t('production.reportThroughput'),
    t('production.reportScrapReason'),
  ];
  return (
    <div className="space-y-4">
      <div>
        <p className="text-xs font-medium text-[var(--text-muted)]">PRD-020</p>
        <h2 className="text-xl font-semibold tracking-tight">{t('production.reportsTitle')}</h2>
      </div>
      <div className="grid gap-4 md:grid-cols-3">
        {reports.map((title) => (
          <Card key={title}>
            <CardHeader>
              <CardTitle className="text-base">{title}</CardTitle>
            </CardHeader>
            <CardContent className="text-sm text-[var(--text-secondary)]">
              {t('production.reportLauncherHint')}
            </CardContent>
          </Card>
        ))}
      </div>
    </div>
  );
}

export function ProductionAnalyticsPage() {
  const { t } = useI18n();
  return (
    <div className="space-y-4">
      <div>
        <p className="text-xs font-medium text-[var(--text-muted)]">PRD-019</p>
        <h2 className="text-xl font-semibold tracking-tight">{t('production.analyticsTitle')}</h2>
        <p className="mt-1 text-sm text-[var(--text-secondary)]">{t('production.analyticsDesc')}</p>
      </div>
      <Card>
        <CardContent className="py-8 text-sm text-[var(--text-secondary)]">
          {t('production.analyticsPlaceholder')}
        </CardContent>
      </Card>
    </div>
  );
}

export function ProductionSettingsPage() {
  const { t } = useI18n();
  return (
    <div className="space-y-4">
      <div>
        <p className="text-xs font-medium text-[var(--text-muted)]">PRD-029</p>
        <h2 className="text-xl font-semibold tracking-tight">{t('production.settingsTitle')}</h2>
      </div>
      <Card>
        <CardContent className="py-8 text-sm text-[var(--text-secondary)]">
          {t('production.settingsHint')}
        </CardContent>
      </Card>
    </div>
  );
}

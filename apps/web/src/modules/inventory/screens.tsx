import { Link, useParams } from '@tanstack/react-router';
import { Card, CardContent, CardHeader, CardTitle } from '@naswood/ui';
import { useI18n } from '@/i18n';
import { EntityDetailScreen } from '@/modules/shared/entity/EntityDetailScreen';
import { EntityListScreen, type EntityField } from '@/modules/shared/entity/EntityListScreen';

function useInvFields() {
  const { t } = useI18n();
  const f = {
    material: [
      { key: 'Code', label: t('inventory.fields.code') },
      { key: 'Name', label: t('inventory.fields.name') },
      { key: 'Description', label: t('inventory.fields.description') },
      { key: 'Category', label: t('inventory.fields.category') },
      { key: 'UnitOfMeasure', label: t('inventory.fields.uom') },
      { key: 'Status', label: t('inventory.fields.status'), status: true },
    ] as EntityField[],
    warehouse: [
      { key: 'Code', label: t('inventory.fields.code') },
      { key: 'Name', label: t('inventory.fields.name') },
      { key: 'WarehouseType', label: t('inventory.fields.type') },
      { key: 'Status', label: t('inventory.fields.status'), status: true },
    ] as EntityField[],
    location: [
      { key: 'WarehouseCode', label: t('inventory.fields.warehouse') },
      { key: 'Code', label: t('inventory.fields.code') },
      { key: 'Name', label: t('inventory.fields.name') },
      { key: 'LocationType', label: t('inventory.fields.type') },
      { key: 'Status', label: t('inventory.fields.status'), status: true },
    ] as EntityField[],
    balance: [
      { key: 'MaterialCode', label: t('inventory.fields.material') },
      { key: 'WarehouseCode', label: t('inventory.fields.warehouse') },
      { key: 'LocationCode', label: t('inventory.fields.location') },
      { key: 'BatchNumber', label: t('inventory.fields.lot') },
      { key: 'QuantityOnHand', label: t('inventory.fields.onHand'), type: 'number' as const },
      { key: 'QuantityReserved', label: t('inventory.fields.reserved'), type: 'number' as const },
      { key: 'Status', label: t('inventory.fields.status'), status: true },
    ] as EntityField[],
    batch: [
      { key: 'BatchNumber', label: t('inventory.fields.lot') },
      { key: 'MaterialCode', label: t('inventory.fields.material') },
      { key: 'Quantity', label: t('inventory.fields.qty'), type: 'number' as const },
      { key: 'ExpiryDate', label: t('inventory.fields.expiry'), type: 'date' as const },
      { key: 'Status', label: t('inventory.fields.status'), status: true },
    ] as EntityField[],
    receipt: [
      { key: 'Number', label: t('inventory.fields.number') },
      { key: 'WarehouseCode', label: t('inventory.fields.warehouse') },
      { key: 'Reference', label: t('inventory.fields.reference') },
      { key: 'Status', label: t('inventory.fields.status'), status: true },
      { key: 'Notes', label: t('inventory.fields.notes') },
    ] as EntityField[],
    issue: [
      { key: 'Number', label: t('inventory.fields.number') },
      { key: 'WarehouseCode', label: t('inventory.fields.warehouse') },
      { key: 'Reference', label: t('inventory.fields.reference') },
      { key: 'Status', label: t('inventory.fields.status'), status: true },
      { key: 'Notes', label: t('inventory.fields.notes') },
    ] as EntityField[],
    transfer: [
      { key: 'Number', label: t('inventory.fields.number') },
      { key: 'FromWarehouseCode', label: t('inventory.fields.fromWh') },
      { key: 'ToWarehouseCode', label: t('inventory.fields.toWh') },
      { key: 'Status', label: t('inventory.fields.status'), status: true },
      { key: 'Notes', label: t('inventory.fields.notes') },
    ] as EntityField[],
    count: [
      { key: 'Number', label: t('inventory.fields.number') },
      { key: 'WarehouseCode', label: t('inventory.fields.warehouse') },
      { key: 'Status', label: t('inventory.fields.status'), status: true },
      { key: 'Notes', label: t('inventory.fields.notes') },
    ] as EntityField[],
    adjustment: [
      { key: 'Number', label: t('inventory.fields.number') },
      { key: 'WarehouseCode', label: t('inventory.fields.warehouse') },
      { key: 'Reason', label: t('inventory.fields.reason') },
      { key: 'Status', label: t('inventory.fields.status'), status: true },
      { key: 'Notes', label: t('inventory.fields.notes') },
    ] as EntityField[],
  };
  return f;
}

export function MaterialListPage() {
  const { t } = useI18n();
  const fields = useInvFields().material;
  return (
    <EntityListScreen
      screenId="INV-004"
      title={t('inventory.materialsTitle')}
      description={t('inventory.materialsDesc')}
      route="materials"
      fields={fields}
      detailPath={(id) => `/inventory/master-data/materials/${id}`}
      createLabel={t('inventory.newMaterial')}
      jobPath="/inventory/master-data/define-material"
    />
  );
}

export function MaterialDetailPage() {
  const { t } = useI18n();
  const { id } = useParams({ strict: false }) as { id: string };
  return (
    <div className="space-y-3">
      <EntityDetailScreen
        screenId="INV-005"
        title={t('inventory.materialDetail')}
        route="materials"
        id={id}
        listPath="/inventory/master-data/materials"
        fields={useInvFields().material}
      />
      <p className="text-sm text-[var(--text-secondary)]">
        {t('md.detailHint')}{' '}
        <Link
          to="/inventory/master-data/define-material"
          className="font-medium text-[var(--color-primary)] hover:underline"
        >
          {t('md.openDesigner')}
        </Link>
      </p>
    </div>
  );
}

export function WarehouseListPage() {
  const { t } = useI18n();
  return (
    <EntityListScreen
      screenId="INV-006"
      title={t('inventory.warehousesTitle')}
      description={t('inventory.warehousesDesc')}
      route="warehouses"
      fields={useInvFields().warehouse}
      detailPath={(id) => `/inventory/master-data/warehouses/${id}`}
      createLabel={t('inventory.newWarehouse')}
      jobPath="/inventory/master-data/define-warehouse"
    />
  );
}

export function WarehouseDetailPage() {
  const { t } = useI18n();
  const { id } = useParams({ strict: false }) as { id: string };
  return (
    <EntityDetailScreen
      screenId="INV-007"
      title={t('inventory.warehouseDetail')}
      route="warehouses"
      id={id}
      listPath="/inventory/master-data/warehouses"
      fields={useInvFields().warehouse}
    />
  );
}

export function LocationListPage() {
  const { t } = useI18n();
  return (
    <EntityListScreen
      screenId="INV-008"
      title={t('inventory.locationsTitle')}
      description={t('inventory.locationsDesc')}
      route="locations"
      fields={useInvFields().location}
      createLabel={t('inventory.newLocation')}
    />
  );
}

export function StockBalancePage() {
  const { t } = useI18n();
  return (
    <EntityListScreen
      screenId="INV-014"
      title={t('inventory.balanceTitle')}
      description={t('inventory.balanceDesc')}
      route="inventory"
      fields={useInvFields().balance}
      createLabel={t('inventory.addBalance')}
    />
  );
}

export function LotListPage() {
  const { t } = useI18n();
  return (
    <EntityListScreen
      screenId="INV-010"
      title={t('inventory.lotsTitle')}
      description={t('inventory.lotsDesc')}
      route="batches"
      fields={useInvFields().batch}
      createLabel={t('inventory.newLot')}
    />
  );
}

export function GoodsReceiptListPage() {
  const { t } = useI18n();
  return (
    <EntityListScreen
      screenId="INV-015"
      title={t('inventory.grTitle')}
      description={t('inventory.grDesc')}
      route="goods-receipts"
      fields={useInvFields().receipt}
      detailPath={(id) => `/inventory/operations/goods-receipts/${id}`}
      createLabel={t('inventory.newReceipt')}
      jobPath="/inventory/operations/receive"
    />
  );
}

export function GoodsReceiptDetailPage() {
  const { t } = useI18n();
  const { id } = useParams({ strict: false }) as { id: string };
  return (
    <EntityDetailScreen
      screenId="INV-016"
      title={t('inventory.grDetail')}
      route="goods-receipts"
      id={id}
      listPath="/inventory/operations/goods-receipts"
      fields={useInvFields().receipt}
    />
  );
}

export function GoodsIssueListPage() {
  const { t } = useI18n();
  return (
    <EntityListScreen
      screenId="INV-017"
      title={t('inventory.giTitle')}
      description={t('inventory.giDesc')}
      route="goods-issues"
      fields={useInvFields().issue}
      createLabel={t('inventory.newIssue')}
      jobPath="/inventory/operations/issue"
    />
  );
}

export function TransferListPage() {
  const { t } = useI18n();
  return (
    <EntityListScreen
      screenId="INV-019"
      title={t('inventory.trTitle')}
      description={t('inventory.trDesc')}
      route="transfers"
      fields={useInvFields().transfer}
      createLabel={t('inventory.newTransfer')}
      jobPath="/inventory/operations/transfer"
    />
  );
}

export function CycleCountListPage() {
  const { t } = useI18n();
  return (
    <EntityListScreen
      screenId="INV-021"
      title={t('inventory.ccTitle')}
      description={t('inventory.ccDesc')}
      route="inventory-counts"
      fields={useInvFields().count}
      createLabel={t('inventory.newCount')}
      jobPath="/inventory/counts/start"
    />
  );
}

export function AdjustmentListPage() {
  const { t } = useI18n();
  return (
    <EntityListScreen
      screenId="INV-024"
      title={t('inventory.adjTitle')}
      description={t('inventory.adjDesc')}
      route="inventory-adjustments"
      fields={useInvFields().adjustment}
      createLabel={t('inventory.newAdjustment')}
    />
  );
}

export function InventoryReportsPage() {
  const { t } = useI18n();
  return (
    <div className="space-y-4">
      <div>
        <p className="text-xs font-medium text-[var(--text-muted)]">INV-025</p>
        <h2 className="text-xl font-semibold tracking-tight">{t('inventory.reportsTitle')}</h2>
        <p className="mt-1 text-sm text-[var(--text-secondary)]">{t('inventory.reportsDesc')}</p>
      </div>
      <div className="grid gap-4 md:grid-cols-3">
        {[
          [t('inventory.reportStockByWh'), t('inventory.reportStockByWhDesc')],
          [t('inventory.reportOpenMoves'), t('inventory.reportOpenMovesDesc')],
          [t('inventory.reportCountAccuracy'), t('inventory.reportCountAccuracyDesc')],
        ].map(([title, body]) => (
          <Card key={title}>
            <CardHeader>
              <CardTitle className="text-base">{title}</CardTitle>
            </CardHeader>
            <CardContent className="text-sm text-[var(--text-secondary)]">{body}</CardContent>
          </Card>
        ))}
      </div>
    </div>
  );
}

import { Link, useParams } from '@tanstack/react-router';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { useMemo, useState } from 'react';
import { Button, Card, CardContent, CardHeader, CardTitle } from '@naswood/ui';
import { createResource, searchAllResource, searchResource } from '@/api/business';
import { usePlantContext } from '@/auth/usePlantContext';
import { useI18n } from '@/i18n';
import { EntityDetailScreen } from '@/modules/shared/entity/EntityDetailScreen';
import { EntityListScreen, type EntityField } from '@/modules/shared/entity/EntityListScreen';
import {
  formatMaterialCodeWithDims,
  formatNominalDims,
  readNominalDims,
  seedItemToCreateBody,
  type MasterSeedItem,
} from '@/modules/inventory/materials/materialNominalDims';
import masterSeed from '@/modules/inventory/materials/masterMaterialSeed.json';
import { LocationListPage as LocationListPageImpl } from '@/modules/inventory/locations/LocationListPage';
import { MasterStockPage } from '@/modules/inventory/stock/MasterStockPage';

function materialDimsSource(row: Record<string, unknown>) {
  return {
    code: String(row.code ?? row.Code ?? ''),
    name: String(row.name ?? row.Name ?? ''),
    description: String(row.description ?? row.Description ?? ''),
    definitionJson: String(row.definitionJson ?? row.DefinitionJson ?? ''),
  };
}

function useInvFields() {
  const { t } = useI18n();
  const f = {
    material: [
      {
        key: 'Code',
        label: t('inventory.fields.code'),
        formatValue: (row) => formatMaterialCodeWithDims(materialDimsSource(row)),
      },
      {
        key: 'NominalDims',
        label: t('inventory.fields.nominalDims'),
        formatValue: (row) => formatNominalDims(readNominalDims(materialDimsSource(row))),
      },
      { key: 'Name', label: t('inventory.fields.name') },
      { key: 'Category', label: t('inventory.fields.category') },
      { key: 'UnitOfMeasure', label: t('inventory.fields.uom') },
      { key: 'Status', label: t('inventory.fields.status'), status: true },
    ] as EntityField[],
    warehouse: [
      { key: 'Code', label: t('inventory.fields.code') },
      { key: 'Name', label: t('inventory.fields.name') },
      { key: 'WarehouseType', label: t('inventory.fields.type') },
      { key: 'Description', label: t('inventory.fields.description') },
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
  const queryClient = useQueryClient();
  const [importMsg, setImportMsg] = useState<string | null>(null);
  const [importErr, setImportErr] = useState<string | null>(null);

  const importMutation = useMutation({
    mutationFn: async () => {
      const all = await searchAllResource<{ code: string }>('materials');
      const existing = new Set(all.map((m) => String(m.code ?? '').toUpperCase()));
      const items = masterSeed.items as MasterSeedItem[];
      let created = 0;
      let skipped = 0;
      for (const item of items) {
        if (existing.has(item.materialCode.toUpperCase())) {
          skipped += 1;
          continue;
        }
        const body = seedItemToCreateBody(item);
        await createResource('materials', body);
        existing.add(item.materialCode.toUpperCase());
        created += 1;
      }
      return { created, skipped, total: items.length };
    },
    onSuccess: async (r) => {
      setImportErr(null);
      setImportMsg(
        t('inventory.masterImportDone')
          .replace('{created}', String(r.created))
          .replace('{skipped}', String(r.skipped))
          .replace('{total}', String(r.total)),
      );
      await queryClient.invalidateQueries({ queryKey: ['business', 'materials'] });
    },
    onError: (e: Error) => setImportErr(e.message),
  });

  const seedCount = useMemo(() => (masterSeed.items as MasterSeedItem[]).length, []);

  return (
    <div className="space-y-3">
      <EntityListScreen
        screenId="INV-004"
        title={t('inventory.materialsTitle')}
        description={t('inventory.materialsDescDims')}
        route="materials"
        fields={fields}
        detailPath={(id) => `/inventory/master-data/materials/${id}`}
        createLabel={t('inventory.newMaterial')}
        jobPath="/inventory/master-data/define-material"
      />
      <Card>
        <CardHeader className="pb-2">
          <CardTitle className="text-base">{t('inventory.masterImportTitle')}</CardTitle>
        </CardHeader>
        <CardContent className="space-y-2">
          <p className="text-sm text-[var(--text-secondary)]">
            {t('inventory.masterImportHint').replace('{n}', String(seedCount))}
          </p>
          <Button
            type="button"
            variant="secondary"
            disabled={importMutation.isPending}
            onClick={() => {
              setImportMsg(null);
              setImportErr(null);
              importMutation.mutate();
            }}
          >
            {importMutation.isPending ? t('saving') : t('inventory.masterImportCta')}
          </Button>
          {importMsg ? <p className="text-sm text-[var(--color-primary)]">{importMsg}</p> : null}
          {importErr ? <p className="text-sm text-[var(--color-danger)]">{importErr}</p> : null}
        </CardContent>
      </Card>
    </div>
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
  return <LocationListPageImpl />;
}

export function StockBalancePage() {
  return <MasterStockPage />;
}

export function LotListPage() {
  const { t } = useI18n();
  const { plantId } = usePlantContext();
  return (
    <EntityListScreen
      screenId="INV-010"
      title={t('inventory.lotsTitle')}
      description={t('inventory.lotsDesc')}
      route="batches"
      fields={useInvFields().batch}
      plantId={plantId}
      readOnly
    />
  );
}

export function PackageListPage() {
  const { plantId } = usePlantContext();
  return (
    <div className="space-y-3">
      <div className="flex flex-wrap gap-2 text-sm">
        <Link className="underline" to="/inventory/stock/scan">
          Barkod tara
        </Link>
      </div>
      <EntityListScreen
        screenId="INV-PKG"
        title="Paketler"
        description="Package ve barcode fiziksel stok kimliğidir. Miktar InventoryBalance’dan gelir; paket ana satırı çoğaltmaz."
        route="packages"
        fields={[
          { key: 'Barcode', label: 'Barkod' },
          { key: 'PackageNumber', label: 'Paket' },
          { key: 'MaterialCode', label: 'Malzeme' },
          { key: 'LotNumber', label: 'Lot' },
          { key: 'WarehouseCode', label: 'Depo' },
          { key: 'LocationCode', label: 'Lokasyon' },
          { key: 'Quantity', label: 'Miktar', type: 'number' },
          { key: 'Status', label: 'Durum', status: true },
        ]}
        detailPath={(id) => `/inventory/stock/packages/${id}`}
        plantId={plantId}
        readOnly
      />
    </div>
  );
}

export function MaterialIdentityListPage() {
  const { plantId } = usePlantContext();
  return (
    <EntityListScreen
      screenId="INV-MI"
      title="Material Identity (sistem)"
      description="Arka plan teknik kimlik — operatör mal kabulde MI seçmez. Ana menüde gösterilmez."
      route="material-identities"
      fields={[
        { key: 'IdentityNumber', label: 'MI' },
        { key: 'MaterialCode', label: 'Malzeme' },
        { key: 'LotNumber', label: 'Lot' },
        { key: 'WarehouseCode', label: 'Depo' },
        { key: 'LocationCode', label: 'Lokasyon' },
        { key: 'Quantity', label: 'Miktar', type: 'number' },
        { key: 'RootGoodsReceiptNumber', label: 'GR' },
        { key: 'Status', label: 'Durum', status: true },
      ]}
      plantId={plantId}
      readOnly
    />
  );
}

export function InventoryMovementListPage() {
  const { plantId } = usePlantContext();
  return (
    <EntityListScreen
      screenId="INV-MV"
      title="Stok Hareketleri"
      description="Material · Lot · Paket · Depo — MI teknik kimlik arka planda."
      route="inventory-movements"
      fields={[
        { key: 'MovementNumber', label: 'Hareket' },
        { key: 'MovementType', label: 'Tip' },
        { key: 'Direction', label: 'Yön' },
        { key: 'DocumentNumber', label: 'Belge' },
        { key: 'MaterialCode', label: 'Malzeme' },
        { key: 'LotNumber', label: 'Lot' },
        { key: 'PackageNumber', label: 'Paket' },
        { key: 'Quantity', label: 'Miktar', type: 'number' },
        { key: 'WarehouseCode', label: 'Depo' },
        { key: 'Status', label: 'Durum', status: true },
      ]}
      plantId={plantId}
      readOnly
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
  const { plantId } = usePlantContext();
  return (
    <EntityListScreen
      screenId="INV-019"
      title={t('inventory.trTitle')}
      description={t('inventory.trDesc')}
      route="transfers"
      fields={useInvFields().transfer}
      createLabel={t('inventory.newTransfer')}
      jobPath="/inventory/operations/transfer"
      plantId={plantId}
    />
  );
}

export function CycleCountListPage() {
  const { t } = useI18n();
  const { plantId } = usePlantContext();
  return (
    <EntityListScreen
      screenId="INV-021"
      title={t('inventory.ccTitle')}
      description={t('inventory.ccDesc')}
      route="inventory-counts"
      fields={useInvFields().count}
      createLabel={t('inventory.newCount')}
      jobPath="/inventory/counts/start"
      plantId={plantId}
    />
  );
}

export function AdjustmentListPage() {
  const { t } = useI18n();
  const { plantId } = usePlantContext();
  return (
    <EntityListScreen
      screenId="INV-024"
      title={t('inventory.adjTitle')}
      description={t('inventory.adjDesc')}
      route="inventory-adjustments"
      fields={useInvFields().adjustment}
      createLabel={t('inventory.newAdjustment')}
      plantId={plantId}
    />
  );
}

export function InventoryReportsPage() {
  const { t } = useI18n();
  const { plantId } = usePlantContext();
  const balances = useQuery({
    queryKey: ['business', 'inventory', 'report', plantId],
    queryFn: () => searchResource<Record<string, unknown>>('inventory', undefined, { plantId }),
  });
  const movements = useQuery({
    queryKey: ['business', 'inventory-movements', 'report', plantId],
    queryFn: () => searchResource<Record<string, unknown>>('inventory-movements', undefined, { plantId }),
  });
  const packages = useQuery({
    queryKey: ['business', 'packages', 'report', plantId],
    queryFn: () => searchResource<Record<string, unknown>>('packages', undefined, { plantId }),
  });

  const byWh = new Map<string, number>();
  for (const row of balances.data?.items ?? []) {
    const wh = String(row.warehouseCode ?? row.WarehouseCode ?? '—');
    const qty = Number(row.quantityOnHand ?? row.QuantityOnHand ?? 0);
    byWh.set(wh, (byWh.get(wh) ?? 0) + qty);
  }

  const inQty = (movements.data?.items ?? [])
    .filter((m) => String(m.direction ?? m.Direction ?? '') === 'In')
    .reduce((s, m) => s + Number(m.quantity ?? m.Quantity ?? 0), 0);
  const outQty = (movements.data?.items ?? [])
    .filter((m) => String(m.direction ?? m.Direction ?? '') === 'Out')
    .reduce((s, m) => s + Number(m.quantity ?? m.Quantity ?? 0), 0);
  const availablePkgs = (packages.data?.items ?? []).filter(
    (p) => String(p.status ?? p.Status ?? '').toLowerCase() === 'available',
  ).length;

  return (
    <div className="space-y-4">
      <div>
        <p className="text-xs font-medium text-[var(--text-muted)]">INV-025</p>
        <h2 className="text-xl font-semibold tracking-tight">{t('inventory.reportsTitle')}</h2>
        <p className="mt-1 text-sm text-[var(--text-secondary)]">{t('inventory.reportsDesc')}</p>
      </div>
      <div className="grid gap-4 md:grid-cols-3">
        <Card>
          <CardHeader>
            <CardTitle className="text-base">{t('inventory.reportStockByWh')}</CardTitle>
          </CardHeader>
          <CardContent className="space-y-2 text-sm text-[var(--text-secondary)]">
            {balances.isLoading ? (
              <p>{t('loading')}</p>
            ) : byWh.size === 0 ? (
              <p>Bakiye yok — önce mal kabul execute edin.</p>
            ) : (
              [...byWh.entries()].map(([wh, qty]) => (
                <div key={wh} className="flex justify-between gap-2 border-b border-[var(--border-default)] py-1">
                  <span>{wh}</span>
                  <span className="font-semibold tabular-nums text-[var(--text-primary)]">{qty}</span>
                </div>
              ))
            )}
            <Link to="/inventory/stock/balances" className="text-xs text-[var(--color-primary)] hover:underline">
              Bakiyeleri aç
            </Link>
          </CardContent>
        </Card>
        <Card>
          <CardHeader>
            <CardTitle className="text-base">{t('inventory.reportOpenMoves')}</CardTitle>
          </CardHeader>
          <CardContent className="space-y-2 text-sm text-[var(--text-secondary)]">
            {movements.isLoading ? (
              <p>{t('loading')}</p>
            ) : (
              <>
                <p>
                  Toplam hareket: <strong className="text-[var(--text-primary)]">{movements.data?.totalCount ?? 0}</strong>
                </p>
                <p>
                  In: <strong className="text-[var(--text-primary)]">{inQty}</strong> · Out:{' '}
                  <strong className="text-[var(--text-primary)]">{outQty}</strong>
                </p>
              </>
            )}
            <Link to="/inventory/stock/movements" className="text-xs text-[var(--color-primary)] hover:underline">
              Hareketleri aç
            </Link>
          </CardContent>
        </Card>
        <Card>
          <CardHeader>
            <CardTitle className="text-base">{t('inventory.reportCountAccuracy')}</CardTitle>
          </CardHeader>
          <CardContent className="space-y-2 text-sm text-[var(--text-secondary)]">
            {packages.isLoading ? (
              <p>{t('loading')}</p>
            ) : (
              <p>
                Available paket: <strong className="text-[var(--text-primary)]">{availablePkgs}</strong> /{' '}
                {packages.data?.totalCount ?? 0}
              </p>
            )}
            <Link to="/inventory/stock/packages" className="text-xs text-[var(--color-primary)] hover:underline">
              Paketleri aç
            </Link>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}

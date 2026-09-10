import { Fragment, useEffect, useMemo, useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { Button, Card, CardContent, Input } from '@naswood/ui';
import { searchAllResource, searchResource } from '@/api/business';
import { useAuth } from '@/auth/useAuth';
import { usePlantContext } from '@/auth/usePlantContext';
import { plantDisplayName } from '@/modules/inventory/locations/locationCatalog';
import {
  exportMasterStock,
  getMasterStockRowPackages,
  searchMasterStock,
  searchMasterStockPackages,
  type MasterStockPackageRow,
  type MasterStockRow,
} from './masterStockApi';
import { buildMasterStockXlsxBytes, downloadMasterStockXlsx } from './masterStockExcel';

type WarehouseRow = { code?: string; Code?: string; name?: string; Name?: string; status?: string };
type LocationRow = { code?: string; Code?: string; name?: string; Name?: string; warehouseCode?: string; status?: string };

function num(v: number | null | undefined): string {
  if (v == null || Number.isNaN(Number(v))) return '—';
  return String(v);
}

export function MasterStockPage() {
  const { user } = useAuth();
  const { homePlantId, plantId: workingPlantId, visiblePlantIds, canSwitchPlant } = usePlantContext();
  const plantIds = visiblePlantIds.length ? visiblePlantIds : [homePlantId || 'PLANT-001'];
  const otherPlants = canSwitchPlant
    ? plantIds.filter((p) => p.toUpperCase() !== (homePlantId || '').toUpperCase())
    : [];

  const [viewPlantId, setViewPlantId] = useState(homePlantId || workingPlantId);
  const [tab, setTab] = useState<'stock' | 'packages'>('stock');
  const [warehouseCode, setWarehouseCode] = useState('');
  const [locationCode, setLocationCode] = useState('');
  const [materialCode, setMaterialCode] = useState('');
  const [lotNumber, setLotNumber] = useState('');
  const [stockStatus, setStockStatus] = useState('');
  const [page, setPage] = useState(1);
  const [sortBy, setSortBy] = useState('materialCode');
  const [expanded, setExpanded] = useState<string | null>(null);
  const [childRows, setChildRows] = useState<MasterStockPackageRow[] | null>(null);
  const [exporting, setExporting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    setViewPlantId(homePlantId || workingPlantId);
  }, [homePlantId, workingPlantId]);

  const effectiveView = useMemo(() => {
    if (plantIds.some((p) => p.toUpperCase() === (viewPlantId || '').toUpperCase())) return viewPlantId;
    return homePlantId;
  }, [viewPlantId, homePlantId, plantIds]);

  const isHomeView = (effectiveView || '').toUpperCase() === (homePlantId || '').toUpperCase();

  const filters = {
    plantId: effectiveView,
    warehouseCode: warehouseCode || undefined,
    locationCode: locationCode || undefined,
    materialCode: materialCode.trim() || undefined,
    lotNumber: lotNumber.trim() || undefined,
    stockStatus: stockStatus || undefined,
    page,
    pageSize: 20,
    sortBy,
  };

  const warehousesQuery = useQuery({
    queryKey: ['business', 'warehouses', 'by-plant', effectiveView],
    queryFn: () => searchAllResource<WarehouseRow>('warehouses', undefined, { plantId: effectiveView }),
  });

  const locationsQuery = useQuery({
    queryKey: ['business', 'locations', effectiveView, warehouseCode],
    enabled: Boolean(warehouseCode),
    queryFn: () =>
      searchResource<LocationRow>('locations', undefined, {
        pageSize: 100,
        plantId: effectiveView,
        warehouseCode,
      }),
  });

  const stockQuery = useQuery({
    queryKey: ['master-stock', filters, tab],
    enabled: tab === 'stock',
    queryFn: () => searchMasterStock(filters),
  });

  const pkgQuery = useQuery({
    queryKey: ['master-stock-packages', filters, tab],
    enabled: tab === 'packages',
    queryFn: () => searchMasterStockPackages(filters),
  });

  useEffect(() => {
    setPage(1);
    setExpanded(null);
    setChildRows(null);
  }, [effectiveView, warehouseCode, locationCode, materialCode, lotNumber, stockStatus, tab]);

  async function toggleExpand(row: MasterStockRow) {
    if (expanded === row.balanceId) {
      setExpanded(null);
      setChildRows(null);
      return;
    }
    setExpanded(row.balanceId);
    setChildRows(null);
    try {
      const rows = await getMasterStockRowPackages(row.balanceId);
      setChildRows(rows);
    } catch (e) {
      setError(e instanceof Error ? e.message : String(e));
    }
  }

  async function onExcel() {
    setError(null);
    setExporting(true);
    try {
      const data = await exportMasterStock({
        plantId: effectiveView,
        warehouseCode: warehouseCode || undefined,
        locationCode: locationCode || undefined,
        materialCode: materialCode.trim() || undefined,
        lotNumber: lotNumber.trim() || undefined,
        stockStatus: stockStatus || undefined,
      });
      const bytes = buildMasterStockXlsxBytes({
        stockRows: data.stockRows,
        packageRows: data.packageRows,
        report: data.report,
      });
      downloadMasterStockXlsx(bytes, data.fileName);
    } catch (e) {
      setError(e instanceof Error ? e.message : String(e));
    } finally {
      setExporting(false);
    }
  }

  const totals = stockQuery.data?.totals;
  const locations = (locationsQuery.data?.items ?? []).filter(
    (l) => String(l.status ?? 'Active').toLowerCase() === 'active',
  );

  return (
    <div className="space-y-4">
      <div className="flex flex-wrap items-start justify-between gap-3">
        <div>
          <p className="text-xs font-medium text-[var(--text-muted)]">INV-014 · MASTER STOK</p>
          <h2 className="text-xl font-semibold tracking-tight">Master Stok</h2>
          <p className="mt-1 max-w-3xl text-sm text-[var(--text-secondary)]">
            Güncel fiziksel bakiye InventoryBalance üzerinden okunur. Paketler satırı çoğaltmaz. Excel okuma
            raporudur; sayım şablonu değildir.
          </p>
        </div>
        <Button disabled={exporting} onClick={() => void onExcel()}>
          {exporting ? 'Hazırlanıyor…' : 'Excel indir'}
        </Button>
      </div>

      {canSwitchPlant && otherPlants.length > 0 ? (
        <div className="flex flex-wrap gap-2 text-sm">
          <button
            type="button"
            className={`rounded-md border px-3 py-1.5 ${isHomeView ? 'border-[var(--color-primary)] bg-[var(--color-primary)]/10' : ''}`}
            onClick={() => setViewPlantId(homePlantId)}
          >
            Ana Üs · {plantDisplayName(homePlantId)}
          </button>
          {otherPlants.map((p) => (
            <button
              key={p}
              type="button"
              className={`rounded-md border px-3 py-1.5 ${viewPlantId === p ? 'border-[var(--color-primary)] bg-[var(--color-primary)]/10' : ''}`}
              onClick={() => setViewPlantId(p)}
            >
              Diğer Tesis · {plantDisplayName(p)}
            </button>
          ))}
        </div>
      ) : (
        <p className="text-sm text-[var(--text-secondary)]">
          Ana Üs: <span className="font-medium text-[var(--text-primary)]">{plantDisplayName(effectiveView)}</span>
        </p>
      )}

      <Card>
        <CardContent className="grid gap-3 pt-4 md:grid-cols-3 lg:grid-cols-6">
          <label className="text-xs">
            Ana Üs
            <div className="mt-1 text-sm font-medium">{plantDisplayName(effectiveView)}</div>
          </label>
          <label className="text-xs">
            Depo
            <select
              className="mt-1 h-9 w-full rounded-md border bg-transparent px-2 text-sm"
              value={warehouseCode}
              onChange={(e) => {
                setWarehouseCode(e.target.value);
                setLocationCode('');
              }}
            >
              <option value="">Tümü</option>
              {(warehousesQuery.data ?? []).map((w) => {
                const code = w.code ?? w.Code ?? '';
                return (
                  <option key={code} value={code}>
                    {w.name || w.Name || code}
                  </option>
                );
              })}
            </select>
          </label>
          <label className="text-xs">
            Lokasyon
            <select
              className="mt-1 h-9 w-full rounded-md border bg-transparent px-2 text-sm"
              value={locationCode}
              disabled={!warehouseCode}
              onChange={(e) => setLocationCode(e.target.value)}
            >
              <option value="">Tümü</option>
              {locations.map((l) => {
                const code = l.code ?? l.Code ?? '';
                return (
                  <option key={code} value={code}>
                    {l.name || l.Name || code}
                  </option>
                );
              })}
            </select>
          </label>
          <label className="text-xs">
            Material
            <Input className="mt-1" value={materialCode} onChange={(e) => setMaterialCode(e.target.value)} placeholder="Kod" />
          </label>
          <label className="text-xs">
            Lot
            <Input className="mt-1" value={lotNumber} onChange={(e) => setLotNumber(e.target.value)} placeholder="Lot" />
          </label>
          <label className="text-xs">
            Stok Durumu
            <select
              className="mt-1 h-9 w-full rounded-md border bg-transparent px-2 text-sm"
              value={stockStatus}
              onChange={(e) => setStockStatus(e.target.value)}
            >
              <option value="">Tümü</option>
              <option value="Available">Available</option>
              <option value="Quarantine">Quarantine</option>
              <option value="Hold">Hold</option>
            </select>
          </label>
        </CardContent>
      </Card>

      {error ? <p className="text-sm text-red-600">{error}</p> : null}

      <div className="flex gap-2">
        <Button variant={tab === 'stock' ? 'default' : 'secondary'} onClick={() => setTab('stock')}>
          Stok
        </Button>
        <Button variant={tab === 'packages' ? 'default' : 'secondary'} onClick={() => setTab('packages')}>
          Paketler
        </Button>
      </div>

      {tab === 'stock' && totals ? (
        <div className="flex flex-wrap gap-4 text-sm text-[var(--text-secondary)]">
          <span>
            Toplam material satırı:{' '}
            <strong className="text-[var(--text-primary)]">{totals.materialRowCount}</strong>
          </span>
          <span>
            Toplam paket: <strong className="text-[var(--text-primary)]">{totals.packageCount}</strong>
          </span>
          {(totals.byUnit ?? []).map((u) => (
            <span key={u.unit}>
              Toplam {u.unit}: <strong className="text-[var(--text-primary)]">{u.quantity}</strong>
            </span>
          ))}
        </div>
      ) : null}

      {tab === 'stock' ? (
        <div className="overflow-x-auto rounded-md border">
          <table className="min-w-full text-left text-sm">
            <thead className="bg-[var(--surface-muted)] text-xs">
              <tr>
                {[
                  ['materialCode', 'MaterialCode'],
                  ['materialName', 'MaterialName'],
                  [null, 'WoodSpecies'],
                  [null, 'ActualMeasurement'],
                  ['lot', 'Lot'],
                  ['warehouse', 'Warehouse'],
                  ['location', 'Location'],
                  [null, 'PieceCount'],
                  [null, 'PackageCount'],
                  [null, 'StockUnit'],
                  ['qty', 'StockQuantity'],
                  ['status', 'StockStatus'],
                ].map(([key, label]) => (
                  <th key={label} className="px-2 py-2">
                    {key ? (
                      <button type="button" onClick={() => setSortBy(key)}>
                        {label}
                      </button>
                    ) : (
                      label
                    )}
                  </th>
                ))}
              </tr>
            </thead>
            <tbody>
              {stockQuery.isLoading ? (
                <tr>
                  <td className="px-2 py-4" colSpan={12}>
                    Yükleniyor…
                  </td>
                </tr>
              ) : (stockQuery.data?.items ?? []).length === 0 ? (
                <tr>
                  <td className="px-2 py-4" colSpan={12}>
                    Kayıt yok.
                  </td>
                </tr>
              ) : (
                (stockQuery.data?.items ?? []).map((row) => (
                  <Fragment key={row.balanceId}>
                    <tr
                      className="cursor-pointer border-t hover:bg-[var(--surface-muted)]/60"
                      onClick={() => void toggleExpand(row)}
                    >
                      <td className="px-2 py-2 font-mono text-xs">{row.materialCode}</td>
                      <td className="px-2 py-2">{row.materialName || '—'}</td>
                      <td className="px-2 py-2">{row.woodSpecies || '—'}</td>
                      <td className="px-2 py-2">{row.actualMeasurement || '—'}</td>
                      <td className="px-2 py-2">{row.lot || '—'}</td>
                      <td className="px-2 py-2">{row.warehouse}</td>
                      <td className="px-2 py-2">{row.location}</td>
                      <td className="px-2 py-2 tabular-nums">{num(row.pieceCount)}</td>
                      <td className="px-2 py-2 tabular-nums">{row.packageCount}</td>
                      <td className="px-2 py-2">{row.stockUnit}</td>
                      <td className="px-2 py-2 tabular-nums">{row.stockQuantity}</td>
                      <td className="px-2 py-2">
                        {row.stockStatus}
                        {row.packageBalanceMismatch ? (
                          <div className="text-[11px] text-amber-700">PACKAGE/BALANCE UYUMSUZLUĞU</div>
                        ) : null}
                      </td>
                    </tr>
                    {expanded === row.balanceId ? (
                      <tr className="bg-[var(--surface-muted)]/40">
                        <td colSpan={12} className="px-4 py-3">
                          {childRows == null ? (
                            <p className="text-xs">Paketler yükleniyor…</p>
                          ) : childRows.length === 0 ? (
                            <p className="text-xs">Paket kaydı yok (stok satırı korunur).</p>
                          ) : (
                            <table className="w-full text-xs">
                              <thead>
                                <tr>
                                  <th className="py-1 text-left">PackageNo</th>
                                  <th className="text-left">PhysicalGroupLabel</th>
                                  <th className="text-left">ActualMeasurement</th>
                                  <th className="text-left">PieceCount</th>
                                  <th className="text-left">StockQuantity</th>
                                  <th className="text-left">Status</th>
                                </tr>
                              </thead>
                              <tbody>
                                {childRows.map((p) => (
                                  <tr key={p.id}>
                                    <td className="py-1 font-mono">{p.packageNo}</td>
                                    <td>{p.physicalGroupLabel || '—'}</td>
                                    <td>{p.actualMeasurement || '—'}</td>
                                    <td>{num(p.pieceCount)}</td>
                                    <td>
                                      {p.stockQuantity} {p.stockUnit}
                                    </td>
                                    <td>{p.status}</td>
                                  </tr>
                                ))}
                              </tbody>
                            </table>
                          )}
                        </td>
                      </tr>
                    ) : null}
                  </Fragment>
                ))
              )}
            </tbody>
          </table>
        </div>
      ) : (
        <div className="overflow-x-auto rounded-md border">
          <table className="min-w-full text-left text-sm">
            <thead className="bg-[var(--surface-muted)] text-xs">
              <tr>
                <th className="px-2 py-2">PackageNo</th>
                <th className="px-2 py-2">PhysicalGroupLabel</th>
                <th className="px-2 py-2">Material</th>
                <th className="px-2 py-2">ActualMeasurement</th>
                <th className="px-2 py-2">Lot</th>
                <th className="px-2 py-2">Warehouse</th>
                <th className="px-2 py-2">Location</th>
                <th className="px-2 py-2">PieceCount</th>
                <th className="px-2 py-2">StockQuantity</th>
                <th className="px-2 py-2">StockUnit</th>
                <th className="px-2 py-2">Status</th>
              </tr>
            </thead>
            <tbody>
              {pkgQuery.isLoading ? (
                <tr>
                  <td className="px-2 py-4" colSpan={11}>
                    Yükleniyor…
                  </td>
                </tr>
              ) : (pkgQuery.data?.items ?? []).length === 0 ? (
                <tr>
                  <td className="px-2 py-4" colSpan={11}>
                    Paket yok.
                  </td>
                </tr>
              ) : (
                (pkgQuery.data?.items ?? []).map((p) => (
                  <tr key={p.id} className="border-t">
                    <td className="px-2 py-2 font-mono text-xs">{p.packageNo}</td>
                    <td className="px-2 py-2">{p.physicalGroupLabel || '—'}</td>
                    <td className="px-2 py-2">
                      {p.materialCode}
                      {p.materialName ? ` · ${p.materialName}` : ''}
                    </td>
                    <td className="px-2 py-2">{p.actualMeasurement || '—'}</td>
                    <td className="px-2 py-2">{p.lot}</td>
                    <td className="px-2 py-2">{p.warehouse}</td>
                    <td className="px-2 py-2">{p.location}</td>
                    <td className="px-2 py-2">{num(p.pieceCount)}</td>
                    <td className="px-2 py-2">{p.stockQuantity}</td>
                    <td className="px-2 py-2">{p.stockUnit}</td>
                    <td className="px-2 py-2">{p.status}</td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      )}

      <div className="flex items-center gap-2 text-sm">
        <Button
          variant="secondary"
          disabled={page <= 1}
          onClick={() => setPage((p) => Math.max(1, p - 1))}
        >
          Önceki
        </Button>
        <span>
          Sayfa {page}
          {tab === 'stock' && stockQuery.data?.totalPages ? ` / ${stockQuery.data.totalPages}` : ''}
          {tab === 'packages' && pkgQuery.data?.totalPages ? ` / ${pkgQuery.data.totalPages}` : ''}
        </span>
        <Button
          variant="secondary"
          disabled={
            tab === 'stock'
              ? page >= (stockQuery.data?.totalPages || 1)
              : page >= (pkgQuery.data?.totalPages || 1)
          }
          onClick={() => setPage((p) => p + 1)}
        >
          Sonraki
        </Button>
        <span className="text-[var(--text-muted)]">Hazırlayan: {user?.username ?? user?.name ?? '—'}</span>
      </div>
    </div>
  );
}

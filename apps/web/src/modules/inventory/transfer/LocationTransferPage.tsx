import { Link } from '@tanstack/react-router';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { useEffect, useMemo, useState } from 'react';
import { Button, Card, CardContent, CardDescription, CardHeader, CardTitle, Input } from '@naswood/ui';
import { executeStockDocument, searchAllResource, searchResource } from '@/api/business';
import { usePlantContext } from '@/auth/usePlantContext';
import { plantDisplayName } from '@/modules/inventory/locations/locationCatalog';
import { validateLocationTransfer } from './locationTransfer';

type WarehouseOpt = { code?: string; name?: string; status?: string; plantId?: string };
type LocationOpt = { code?: string; name?: string; warehouseCode?: string; status?: string };
type BalanceRow = {
  id?: string;
  materialCode?: string;
  warehouseCode?: string;
  locationCode?: string;
  batchNumber?: string;
  quantityOnHand?: number;
  quantityReserved?: number;
  status?: string;
  plantId?: string;
};

type ExecuteResult = {
  documentId: string;
  documentNumber: string;
  status: string;
  plantId: string;
  materialCode: string;
  lotNumber: string;
  fromWarehouseCode: string;
  fromLocationCode: string;
  toWarehouseCode: string;
  toLocationCode: string;
  quantity: number;
  outMovementNumber: string;
  inMovementNumber: string;
};

/**
 * INV-TRF-001 — Stok lokasyon transferi (aynı fabrika).
 * Ana Üs varsayılan; kaynak/hedef aynı seçili tesiste kalır.
 */
export function LocationTransferPage() {
  const { homePlantId, plantId: sessionPlantId, visiblePlantIds, canSwitchPlant } = usePlantContext();
  const queryClient = useQueryClient();
  const plantIds = visiblePlantIds.length ? visiblePlantIds : [homePlantId || 'PLANT-001'];
  const [plantId, setPlantId] = useState(sessionPlantId || homePlantId);

  useEffect(() => {
    setPlantId(sessionPlantId || homePlantId);
  }, [sessionPlantId, homePlantId]);

  const [materialCode, setMaterialCode] = useState('');
  const [lotNumber, setLotNumber] = useState('');
  const [fromWarehouseCode, setFromWarehouseCode] = useState('');
  const [fromLocationCode, setFromLocationCode] = useState('');
  const [toWarehouseCode, setToWarehouseCode] = useState('');
  const [toLocationCode, setToLocationCode] = useState('');
  const [qty, setQty] = useState('');
  const [notes, setNotes] = useState('');
  const [result, setResult] = useState<ExecuteResult | null>(null);
  const [error, setError] = useState<string | null>(null);

  const warehousesQuery = useQuery({
    queryKey: ['business', 'warehouses', 'trf', plantId],
    queryFn: () => searchAllResource<WarehouseOpt>('warehouses', undefined, { plantId }),
  });
  const locationsQuery = useQuery({
    queryKey: ['business', 'locations', 'trf', plantId],
    queryFn: () => searchAllResource<LocationOpt>('locations', undefined, { plantId }),
  });
  const balancesQuery = useQuery({
    queryKey: ['business', 'inventory', 'trf', plantId, materialCode],
    queryFn: () =>
      searchResource<BalanceRow>('inventory', materialCode.trim() || undefined, {
        page: 1,
        pageSize: 100,
        plantId,
      }),
  });

  const activeWarehouses = useMemo(
    () =>
      (warehousesQuery.data ?? []).filter(
        (w) => String(w.status ?? 'Active').toLowerCase() === 'active' && String(w.code ?? '').trim(),
      ),
    [warehousesQuery.data],
  );

  const locationsByWh = useMemo(() => {
    const map = new Map<string, LocationOpt[]>();
    for (const loc of locationsQuery.data ?? []) {
      if (String(loc.status ?? 'Active').toLowerCase() !== 'active') continue;
      const wh = String(loc.warehouseCode ?? '').toUpperCase();
      if (!map.has(wh)) map.set(wh, []);
      map.get(wh)!.push(loc);
    }
    return map;
  }, [locationsQuery.data]);

  const plantBalances = useMemo(() => {
    return (balancesQuery.data?.items ?? []).filter((b) => {
      const p = String(b.plantId ?? '').trim();
      if (p && p.toUpperCase() !== plantId.toUpperCase()) return false;
      const onHand = Number(b.quantityOnHand ?? 0);
      return onHand > 0;
    });
  }, [balancesQuery.data, plantId]);

  const sourceBalance = useMemo(() => {
    return plantBalances.find(
      (b) =>
        String(b.materialCode ?? '').toUpperCase() === materialCode.trim().toUpperCase() &&
        String(b.warehouseCode ?? '').toUpperCase() === fromWarehouseCode.trim().toUpperCase() &&
        String(b.locationCode ?? '').toUpperCase() === fromLocationCode.trim().toUpperCase() &&
        String(b.batchNumber ?? '').toUpperCase() === lotNumber.trim().toUpperCase(),
    );
  }, [plantBalances, materialCode, fromWarehouseCode, fromLocationCode, lotNumber]);

  const availableQty = Math.max(
    0,
    Number(sourceBalance?.quantityOnHand ?? 0) - Number(sourceBalance?.quantityReserved ?? 0),
  );

  const allowlistWarehouses = useMemo(
    () => new Set(activeWarehouses.map((w) => String(w.code).trim().toUpperCase())),
    [activeWarehouses],
  );
  const allowlistLocs = useMemo(() => {
    const map = new Map<string, ReadonlySet<string>>();
    for (const [wh, locs] of locationsByWh) {
      map.set(wh, new Set(locs.map((l) => String(l.code).trim().toUpperCase())));
    }
    return map;
  }, [locationsByWh]);

  const transferQty = Number(String(qty).replace(',', '.'));
  const validation = validateLocationTransfer({
    plantId,
    materialCode,
    lotNumber,
    fromWarehouseCode,
    fromLocationCode,
    toWarehouseCode,
    toLocationCode,
    quantity: Number.isFinite(transferQty) ? transferQty : 0,
    availableQty,
    activeWarehouses: allowlistWarehouses,
    locationsByWarehouse: allowlistLocs,
  });

  const fromLocOptions = locationsByWh.get(fromWarehouseCode.trim().toUpperCase()) ?? [];
  const toLocOptions = locationsByWh.get(toWarehouseCode.trim().toUpperCase()) ?? [];

  function pickSource(row: BalanceRow) {
    setMaterialCode(String(row.materialCode ?? ''));
    setLotNumber(String(row.batchNumber ?? ''));
    setFromWarehouseCode(String(row.warehouseCode ?? ''));
    setFromLocationCode(String(row.locationCode ?? ''));
    setToWarehouseCode(String(row.warehouseCode ?? ''));
    setToLocationCode('');
    setQty('');
    setResult(null);
    setError(null);
  }

  const mutate = useMutation({
    mutationFn: async () => {
      if (!validation.ok) throw new Error(validation.message);
      return executeStockDocument<ExecuteResult>('transfers/execute', {
        plantId,
        materialCode: materialCode.trim(),
        lotNumber: lotNumber.trim(),
        fromWarehouseCode: fromWarehouseCode.trim(),
        fromLocationCode: fromLocationCode.trim(),
        toWarehouseCode: toWarehouseCode.trim(),
        toLocationCode: toLocationCode.trim(),
        quantity: transferQty,
        unitOfMeasure: 'Piece',
        notes: notes.trim(),
      });
    },
    onSuccess: (data) => {
      setResult(data);
      setError(null);
      void queryClient.invalidateQueries({ queryKey: ['business', 'inventory'] });
      void queryClient.invalidateQueries({ queryKey: ['business', 'transfers'] });
    },
    onError: (err: unknown) => {
      setResult(null);
      setError(err instanceof Error ? err.message : 'Transfer başarısız.');
    },
  });

  return (
    <div className="mx-auto max-w-3xl space-y-6 p-4 md:p-6">
      <div className="flex flex-wrap items-start justify-between gap-3">
        <div>
          <p className="text-[10px] font-semibold uppercase tracking-wide text-[var(--text-muted)]">
            INV-TRF-001
          </p>
          <h1 className="text-xl font-semibold tracking-tight">Stok Lokasyon Transferi</h1>
          <p className="mt-1 text-sm text-[var(--text-secondary)]">
            Aynı fabrika içinde depo/lokasyon taşıma. Fabrikalar arası transfer ayrı modüldür.
          </p>
        </div>
        <Link
          to="/inventory/operations/transfers"
          className="text-sm text-[var(--color-primary)] hover:underline"
        >
          Transfer listesi
        </Link>
      </div>

      <Card>
        <CardHeader className="pb-3">
          <CardTitle className="text-base">Ana Fabrika</CardTitle>
          <CardDescription>
            {canSwitchPlant
              ? `Varsayılan Ana Üs: ${plantDisplayName(homePlantId)} (${homePlantId}). Kaynak ve hedef aynı seçili tesiste kalır.`
              : `Yalnızca Ana Fabrika: ${plantDisplayName(homePlantId)} (${homePlantId}).`}
          </CardDescription>
        </CardHeader>
        <CardContent>
          {canSwitchPlant ? (
            <select
              className="h-10 w-full max-w-md rounded-md border border-[var(--border-default)] bg-transparent px-3 text-sm"
              value={plantId}
              disabled={plantIds.length <= 1}
              onChange={(e) => {
                setPlantId(e.target.value);
                setFromWarehouseCode('');
                setFromLocationCode('');
                setToWarehouseCode('');
                setToLocationCode('');
                setResult(null);
              }}
            >
              {plantIds.map((p) => (
                <option key={p} value={p}>
                  {plantDisplayName(p)} ({p})
                  {p.toUpperCase() === homePlantId.toUpperCase() ? ' · Ana Üs' : ''}
                </option>
              ))}
            </select>
          ) : (
            <Input
              value={`${plantDisplayName(homePlantId)} (${homePlantId})`}
              readOnly
              disabled
              className="max-w-md"
            />
          )}
        </CardContent>
      </Card>

      <Card>
        <CardHeader className="pb-3">
          <CardTitle className="text-base">Kaynak stok seç</CardTitle>
          <CardDescription>
            Malzeme kodu ile ara; satıra tıklayınca kaynak alanları dolar.
          </CardDescription>
        </CardHeader>
        <CardContent className="space-y-3">
          <Input
            value={materialCode}
            onChange={(e) => setMaterialCode(e.target.value)}
            placeholder="Malzeme kodu (ör. HM-KR-PIN-001)"
          />
          <div className="max-h-48 overflow-auto rounded-md border border-[var(--border-default)]">
            <table className="w-full text-left text-xs">
              <thead className="sticky top-0 bg-[var(--color-surface)] text-[10px] uppercase text-[var(--text-muted)]">
                <tr>
                  <th className="px-2 py-1">Malzeme</th>
                  <th className="px-2 py-1">Lot</th>
                  <th className="px-2 py-1">Depo</th>
                  <th className="px-2 py-1">Lokasyon</th>
                  <th className="px-2 py-1">Miktar</th>
                </tr>
              </thead>
              <tbody>
                {plantBalances.length === 0 ? (
                  <tr>
                    <td colSpan={5} className="px-2 py-3 text-[var(--text-muted)]">
                      Bu tesiste eşleşen stok yok.
                    </td>
                  </tr>
                ) : (
                  plantBalances.map((row) => (
                    <tr
                      key={String(row.id)}
                      className="cursor-pointer border-t border-[var(--border-default)] hover:bg-[var(--color-surface-hover)]"
                      onClick={() => pickSource(row)}
                    >
                      <td className="px-2 py-1.5 font-mono">{row.materialCode}</td>
                      <td className="px-2 py-1.5 font-mono">{row.batchNumber || '—'}</td>
                      <td className="px-2 py-1.5">{row.warehouseCode}</td>
                      <td className="px-2 py-1.5">{row.locationCode}</td>
                      <td className="px-2 py-1.5 tabular-nums">{row.quantityOnHand}</td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </div>
        </CardContent>
      </Card>

      <Card>
        <CardHeader className="pb-3">
          <CardTitle className="text-base">Transfer</CardTitle>
        </CardHeader>
        <CardContent className="grid gap-4 sm:grid-cols-2">
          <label className="block space-y-1 sm:col-span-2">
            <span className="text-xs font-medium text-[var(--text-muted)]">Malzeme</span>
            <Input value={materialCode} onChange={(e) => setMaterialCode(e.target.value)} />
          </label>
          <label className="block space-y-1">
            <span className="text-xs font-medium text-[var(--text-muted)]">Lot</span>
            <Input value={lotNumber} onChange={(e) => setLotNumber(e.target.value)} />
          </label>
          <div className="space-y-1">
            <span className="text-xs font-medium text-[var(--text-muted)]">Mevcut miktar</span>
            <p className="font-mono text-sm tabular-nums">
              {sourceBalance ? `${availableQty} PCS` : '—'}
            </p>
          </div>

          <label className="block space-y-1">
            <span className="text-xs font-medium text-[var(--text-muted)]">Kaynak depo</span>
            <select
              className="h-10 w-full rounded-md border border-[var(--border-default)] bg-transparent px-2 text-sm"
              value={fromWarehouseCode}
              onChange={(e) => {
                setFromWarehouseCode(e.target.value);
                setFromLocationCode('');
              }}
            >
              <option value="">—</option>
              {activeWarehouses.map((w) => (
                <option key={String(w.code)} value={String(w.code)}>
                  {w.code}
                  {w.name ? ` · ${w.name}` : ''}
                </option>
              ))}
            </select>
          </label>
          <label className="block space-y-1">
            <span className="text-xs font-medium text-[var(--text-muted)]">Kaynak lokasyon</span>
            <select
              className="h-10 w-full rounded-md border border-[var(--border-default)] bg-transparent px-2 text-sm"
              value={fromLocationCode}
              onChange={(e) => setFromLocationCode(e.target.value)}
              disabled={!fromWarehouseCode}
            >
              <option value="">—</option>
              {fromLocOptions.map((l) => (
                <option key={String(l.code)} value={String(l.code)}>
                  {l.code}
                  {l.name ? ` · ${l.name}` : ''}
                </option>
              ))}
            </select>
          </label>

          <label className="block space-y-1">
            <span className="text-xs font-medium text-[var(--text-muted)]">Hedef depo</span>
            <select
              className="h-10 w-full rounded-md border border-[var(--border-default)] bg-transparent px-2 text-sm"
              value={toWarehouseCode}
              onChange={(e) => {
                setToWarehouseCode(e.target.value);
                setToLocationCode('');
              }}
            >
              <option value="">—</option>
              {activeWarehouses.map((w) => (
                <option key={String(w.code)} value={String(w.code)}>
                  {w.code}
                  {w.name ? ` · ${w.name}` : ''}
                </option>
              ))}
            </select>
          </label>
          <label className="block space-y-1">
            <span className="text-xs font-medium text-[var(--text-muted)]">Hedef lokasyon</span>
            <select
              className="h-10 w-full rounded-md border border-[var(--border-default)] bg-transparent px-2 text-sm"
              value={toLocationCode}
              onChange={(e) => setToLocationCode(e.target.value)}
              disabled={!toWarehouseCode}
            >
              <option value="">—</option>
              {toLocOptions.map((l) => (
                <option key={String(l.code)} value={String(l.code)}>
                  {l.code}
                  {l.name ? ` · ${l.name}` : ''}
                </option>
              ))}
            </select>
          </label>

          <label className="block space-y-1">
            <span className="text-xs font-medium text-[var(--text-muted)]">Transfer miktarı</span>
            <Input
              value={qty}
              onChange={(e) => setQty(e.target.value)}
              placeholder="PCS"
              inputMode="decimal"
            />
          </label>
          <label className="block space-y-1 sm:col-span-2">
            <span className="text-xs font-medium text-[var(--text-muted)]">Açıklama (opsiyonel)</span>
            <Input value={notes} onChange={(e) => setNotes(e.target.value)} />
          </label>

          {!validation.ok ? (
            <p className="sm:col-span-2 text-sm text-[var(--color-danger)]">{validation.message}</p>
          ) : null}
          {error ? (
            <p className="sm:col-span-2 text-sm text-[var(--color-danger)]">{error}</p>
          ) : null}
          {result ? (
            <div className="sm:col-span-2 rounded-md border border-[var(--color-primary)]/40 bg-[var(--color-primary)]/5 px-3 py-2 text-sm">
              <p className="font-medium text-[var(--color-primary)]">
                Transfer kaydedildi · {result.documentNumber}
              </p>
              <p className="mt-1 font-mono text-xs text-[var(--text-muted)]">
                {result.fromWarehouseCode}/{result.fromLocationCode} → {result.toWarehouseCode}/
                {result.toLocationCode} · {result.quantity} PCS
              </p>
              <p className="mt-1 font-mono text-xs text-[var(--text-muted)]">
                OUT {result.outMovementNumber} · IN {result.inMovementNumber}
              </p>
            </div>
          ) : null}

          <div className="sm:col-span-2">
            <Button
              type="button"
              disabled={!validation.ok || mutate.isPending}
              onClick={() => mutate.mutate()}
            >
              {mutate.isPending ? 'Kaydediliyor…' : 'Transferi Kaydet'}
            </Button>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}

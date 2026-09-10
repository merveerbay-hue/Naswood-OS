import { useMemo, useState } from 'react';
import { Link } from '@tanstack/react-router';
import { useQuery } from '@tanstack/react-query';
import { Button, Card, CardContent, CardHeader, CardTitle, Input } from '@naswood/ui';
import { searchResource } from '@/api/business';
import { usePlantContext } from '@/auth/usePlantContext';
import { getPackagePassport } from '@/modules/inventory/stock/packagePassportApi';
import { printPackageLabels } from '@/modules/inventory/stock/packageLabelPrint';
import {
  postProductionOutput,
  previewProductionOutput,
  type ProductionOutputPreview,
  type ProductionOutputResult,
} from './productionOutputApi';

type OrderRow = { id?: string; Id?: string; code?: string; Code?: string; name?: string; Name?: string; plantId?: string };
type MaterialRow = { id?: string; Id?: string; code?: string; Code?: string; name?: string; Name?: string };
type WarehouseRow = { code?: string; Code?: string; name?: string; Name?: string };
type LocationRow = { code?: string; Code?: string; warehouseCode?: string };
type BatchRow = { id?: string; Id?: string; batchNumber?: string; BatchNumber?: string; materialCode?: string };

type Line = {
  key: string;
  physicalGroupLabel: string;
  thicknessMm: string;
  widthMm: string;
  lengthMm: string;
  pieceCount: string;
};

function idOf(row: { id?: string; Id?: string }) {
  return row.id || row.Id || '';
}

export function ProductionOutputPage() {
  const { homePlantId, plantId } = usePlantContext();
  const effectivePlant = plantId || homePlantId;
  const [orderId, setOrderId] = useState('');
  const [materialId, setMaterialId] = useState('');
  const [warehouseCode, setWarehouseCode] = useState('');
  const [locationCode, setLocationCode] = useState('');
  const [workCenterCode, setWorkCenterCode] = useState('');
  const [stockStatus, setStockStatus] = useState('Available');
  const [lines, setLines] = useState<Line[]>([
    { key: '1', physicalGroupLabel: 'İstif A', thicknessMm: '', widthMm: '', lengthMm: '', pieceCount: '' },
  ]);
  const [sourceLotId, setSourceLotId] = useState('');
  const [sourceWh, setSourceWh] = useState('');
  const [sourceLoc, setSourceLoc] = useState('');
  const [sourceQty, setSourceQty] = useState('');
  const [preview, setPreview] = useState<ProductionOutputPreview | null>(null);
  const [result, setResult] = useState<ProductionOutputResult | null>(null);
  const [err, setErr] = useState<string | null>(null);
  const [busy, setBusy] = useState(false);

  const orders = useQuery({
    queryKey: ['production-orders', effectivePlant],
    queryFn: () => searchResource<OrderRow>('production-orders', undefined, { pageSize: 100, plantId: effectivePlant }),
  });
  const materials = useQuery({
    queryKey: ['materials-output'],
    queryFn: () => searchResource<MaterialRow>('materials', undefined, { pageSize: 100 }),
  });
  const warehouses = useQuery({
    queryKey: ['warehouses-output', effectivePlant],
    queryFn: () => searchResource<WarehouseRow>('warehouses', undefined, { pageSize: 100, plantId: effectivePlant }),
  });
  const locations = useQuery({
    queryKey: ['locations-output', effectivePlant, warehouseCode],
    enabled: Boolean(warehouseCode),
    queryFn: () =>
      searchResource<LocationRow>('locations', undefined, {
        pageSize: 100,
        plantId: effectivePlant,
        warehouseCode,
      }),
  });
  const lots = useQuery({
    queryKey: ['batches-output', effectivePlant],
    queryFn: () => searchResource<BatchRow>('batches', undefined, { pageSize: 100, plantId: effectivePlant }),
  });

  const body = useMemo(
    () => ({
      productionOrderId: orderId,
      outputMaterialId: materialId,
      warehouseCode,
      locationCode,
      workCenterCode,
      stockStatus,
      plantId: effectivePlant,
      lines: lines.map((l) => ({
        physicalGroupLabel: l.physicalGroupLabel,
        thicknessMm: l.thicknessMm ? Number(l.thicknessMm) : null,
        widthMm: l.widthMm ? Number(l.widthMm) : null,
        lengthMm: l.lengthMm ? Number(l.lengthMm) : null,
        pieceCount: l.pieceCount ? Number(l.pieceCount) : null,
      })),
      sources: sourceLotId
        ? [
            {
              sourceLotId,
              sourceWarehouseCode: sourceWh,
              sourceLocationCode: sourceLoc,
              consumedQuantity: Number(sourceQty || 0),
              unit: '',
            },
          ]
        : [],
    }),
    [orderId, materialId, warehouseCode, locationCode, workCenterCode, stockStatus, effectivePlant, lines, sourceLotId, sourceWh, sourceLoc, sourceQty],
  );

  async function onPreview() {
    setErr(null);
    setBusy(true);
    try {
      setPreview(await previewProductionOutput(body));
      setResult(null);
    } catch (e) {
      setErr(e instanceof Error ? e.message : String(e));
    } finally {
      setBusy(false);
    }
  }

  async function onPost() {
    setErr(null);
    setBusy(true);
    try {
      setResult(await postProductionOutput(body));
    } catch (e) {
      setErr(e instanceof Error ? e.message : String(e));
    } finally {
      setBusy(false);
    }
  }

  async function printCreated() {
    if (!result) return;
    const docs = await Promise.all(result.packages.map((p) => getPackagePassport(p.packageId)));
    printPackageLabels(docs);
  }

  return (
    <div className="space-y-4">
      <div>
        <p className="text-xs text-[var(--text-muted)]">PRD-OUT · üretim çıkışı</p>
        <h1 className="text-2xl font-semibold">Üretim Çıkışı</h1>
        <p className="text-sm text-[var(--text-muted)]">
          Lot, paket ve barkod final işlemede üretilir. Work center stok lokasyonu değildir.
        </p>
      </div>
      {err ? <p className="text-sm text-red-600">{err}</p> : null}

      <Card>
        <CardHeader>
          <CardTitle>Emir ve hedef</CardTitle>
        </CardHeader>
        <CardContent className="grid gap-2 md:grid-cols-2">
          <select className="rounded-md border px-2 py-2 text-sm" value={orderId} onChange={(e) => setOrderId(e.target.value)}>
            <option value="">Üretim emri</option>
            {(orders.data?.items ?? []).map((o) => (
              <option key={idOf(o)} value={idOf(o)}>
                {o.code || o.Code} {o.name || o.Name}
              </option>
            ))}
          </select>
          <select className="rounded-md border px-2 py-2 text-sm" value={materialId} onChange={(e) => setMaterialId(e.target.value)}>
            <option value="">Çıktı malzemesi</option>
            {(materials.data?.items ?? []).map((m) => (
              <option key={idOf(m)} value={idOf(m)}>
                {m.code || m.Code} {m.name || m.Name}
              </option>
            ))}
          </select>
          <select className="rounded-md border px-2 py-2 text-sm" value={warehouseCode} onChange={(e) => setWarehouseCode(e.target.value)}>
            <option value="">Hedef depo</option>
            {(warehouses.data?.items ?? []).map((w) => (
              <option key={w.code || w.Code} value={w.code || w.Code}>
                {w.code || w.Code} {w.name || w.Name}
              </option>
            ))}
          </select>
          <select className="rounded-md border px-2 py-2 text-sm" value={locationCode} onChange={(e) => setLocationCode(e.target.value)}>
            <option value="">Hedef lokasyon</option>
            {(locations.data?.items ?? []).map((l) => (
              <option key={l.code || l.Code} value={l.code || l.Code}>
                {l.code || l.Code}
              </option>
            ))}
          </select>
          <Input placeholder="İş merkezi (bilgi)" value={workCenterCode} onChange={(e) => setWorkCenterCode(e.target.value)} />
          <select className="rounded-md border px-2 py-2 text-sm" value={stockStatus} onChange={(e) => setStockStatus(e.target.value)}>
            <option value="Available">Available</option>
            <option value="Quarantine">Quarantine</option>
          </select>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>Fiziksel çıkış</CardTitle>
        </CardHeader>
        <CardContent className="space-y-2">
          {lines.map((line, i) => (
            <div key={line.key} className="grid gap-2 md:grid-cols-6">
              <Input
                placeholder="İstif"
                value={line.physicalGroupLabel}
                onChange={(e) =>
                  setLines((rows) => rows.map((r, idx) => (idx === i ? { ...r, physicalGroupLabel: e.target.value } : r)))
                }
              />
              <Input placeholder="Kalınlık" value={line.thicknessMm} onChange={(e) => setLines((rows) => rows.map((r, idx) => (idx === i ? { ...r, thicknessMm: e.target.value } : r)))} />
              <Input placeholder="Genişlik" value={line.widthMm} onChange={(e) => setLines((rows) => rows.map((r, idx) => (idx === i ? { ...r, widthMm: e.target.value } : r)))} />
              <Input placeholder="Boy" value={line.lengthMm} onChange={(e) => setLines((rows) => rows.map((r, idx) => (idx === i ? { ...r, lengthMm: e.target.value } : r)))} />
              <Input placeholder="Adet" value={line.pieceCount} onChange={(e) => setLines((rows) => rows.map((r, idx) => (idx === i ? { ...r, pieceCount: e.target.value } : r)))} />
              <Button variant="secondary" onClick={() => setLines((rows) => rows.filter((_, idx) => idx !== i))}>
                Sil
              </Button>
            </div>
          ))}
          <Button
            variant="secondary"
            onClick={() =>
              setLines((rows) => [
                ...rows,
                { key: String(Date.now()), physicalGroupLabel: '', thicknessMm: '', widthMm: '', lengthMm: '', pieceCount: '' },
              ])
            }
          >
            + Çıktı satırı
          </Button>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>Kaynak lot</CardTitle>
        </CardHeader>
        <CardContent className="grid gap-2 md:grid-cols-4">
          <select className="rounded-md border px-2 py-2 text-sm" value={sourceLotId} onChange={(e) => setSourceLotId(e.target.value)}>
            <option value="">Kaynak lot (opsiyonel)</option>
            {(lots.data?.items ?? []).map((b) => (
              <option key={idOf(b)} value={idOf(b)}>
                {b.batchNumber || b.BatchNumber} {b.materialCode}
              </option>
            ))}
          </select>
          <Input placeholder="Kaynak depo" value={sourceWh} onChange={(e) => setSourceWh(e.target.value)} />
          <Input placeholder="Kaynak lokasyon" value={sourceLoc} onChange={(e) => setSourceLoc(e.target.value)} />
          <Input placeholder="Tüketim miktarı" value={sourceQty} onChange={(e) => setSourceQty(e.target.value)} />
        </CardContent>
      </Card>

      <div className="flex flex-wrap gap-2">
        <Button disabled={busy || !orderId || !materialId || !warehouseCode || !locationCode} onClick={() => void onPreview()}>
          Önizle
        </Button>
        <Button disabled={busy || !preview} onClick={() => void onPost()}>
          Üretim çıkışını işle
        </Button>
      </div>

      {preview ? (
        <Card>
          <CardHeader>
            <CardTitle>Üretim çıkışı önizlemesi</CardTitle>
          </CardHeader>
          <CardContent className="space-y-1 text-sm">
            <div>Emir: {preview.productionOrderNumber}</div>
            <div>
              Çıktı: {preview.outputMaterialName} ({preview.outputMaterialCode})
            </div>
            <div>Üretim lotu: {preview.lotHint}</div>
            <div>
              Hedef: {preview.destinationWarehouse} / {preview.destinationLocation}
            </div>
            <div>
              Paket: {preview.packageCount} · Stok: {preview.outputQuantity} {preview.unit} · Kaynak: {preview.sourceLotCount} lot
            </div>
            {preview.packages.map((p, i) => (
              <div key={i}>
                {p.physicalGroupLabel || 'satır paketi'}: {p.measurements.join(' · ')}
              </div>
            ))}
          </CardContent>
        </Card>
      ) : null}

      {result ? (
        <Card>
          <CardHeader>
            <CardTitle>Üretim çıkışı işlendi</CardTitle>
          </CardHeader>
          <CardContent className="space-y-2 text-sm">
            <div>Üretim lotu: {result.productionLotNumber}</div>
            <div>
              Paket: {result.packageCount} · Barkod: {result.packageCount} · Çıktı: {result.outputQuantity} {result.unit}
            </div>
            <div>Kaynak lot: {result.sourceLotNumbers.join(', ') || result.sourceLotCount}</div>
            <div className="flex flex-wrap gap-2">
              <Link className="rounded-md border px-3 py-2" to="/inventory/stock/packages">
                Paketleri gör
              </Link>
              <Button variant="secondary" onClick={() => void printCreated()}>
                Etiketleri yazdır
              </Button>
              <Link className="rounded-md border px-3 py-2" to="/inventory/stock/lots">
                Üretim lotunu gör
              </Link>
              <Link className="rounded-md border px-3 py-2" to="/inventory/stock/balances">
                Stok görünümü
              </Link>
            </div>
          </CardContent>
        </Card>
      ) : null}
    </div>
  );
}

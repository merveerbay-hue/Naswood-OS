import { FormEvent, useEffect, useRef, useState } from 'react';
import { Link, useNavigate, useParams } from '@tanstack/react-router';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Button, Card, CardContent, CardHeader, CardTitle, Input } from '@naswood/ui';
import { searchResource } from '@/api/business';
import { usePlantContext } from '@/auth/usePlantContext';
import {
  addScrap,
  completeExecution,
  consumeExecution,
  endDowntime,
  getExecution,
  pauseExecution,
  resumeExecution,
  scanExecution,
  startDowntime,
  type ExecutionScan,
} from './shopFloorApi';
import { ShopFloorReportButton } from './ShopFloorReportButton';

function istanbul(iso?: string) {
  if (!iso) return '—';
  return new Date(iso).toLocaleString('tr-TR', { timeZone: 'Europe/Istanbul' });
}

function clock(minutes: number) {
  const total = Math.max(0, Math.round(minutes * 60));
  const h = Math.floor(total / 3600);
  const m = Math.floor((total % 3600) / 60);
  const s = total % 60;
  return `${String(h).padStart(2, '0')}:${String(m).padStart(2, '0')}:${String(s).padStart(2, '0')}`;
}

export function ShopFloorExecutionPage() {
  const { id } = useParams({ strict: false }) as { id: string };
  const nav = useNavigate();
  const { plantId, homePlantId } = usePlantContext();
  const plant = plantId || homePlantId;
  const qc = useQueryClient();
  const scanRef = useRef<HTMLInputElement>(null);
  const qtyRef = useRef<HTMLInputElement>(null);
  const [barcode, setBarcode] = useState('');
  const [scan, setScan] = useState<ExecutionScan | null>(null);
  const [qty, setQty] = useState('');
  const [contentId, setContentId] = useState('');
  const [error, setError] = useState('');
  const [scrapQty, setScrapQty] = useState('');
  const [scrapReason, setScrapReason] = useState('OTHER');
  const [dtReason, setDtReason] = useState('MACHINE');
  const [outMat, setOutMat] = useState('');
  const [wh, setWh] = useState('');
  const [loc, setLoc] = useState('');
  const [pcs, setPcs] = useState('');
  const [t, setT] = useState('');
  const [w, setW] = useState('');
  const [l, setL] = useState('');

  const exec = useQuery({ queryKey: ['shop-exec', id], queryFn: () => getExecution(id), refetchInterval: 4000 });
  const materials = useQuery({
    queryKey: ['sf-mat', plant],
    queryFn: () => searchResource('materials', { page: 1, pageSize: 80, plantId: plant }),
  });
  const warehouses = useQuery({
    queryKey: ['sf-wh', plant],
    queryFn: () => searchResource('warehouses', { page: 1, pageSize: 40, plantId: plant }),
  });
  const locations = useQuery({
    queryKey: ['sf-loc', plant],
    queryFn: () => searchResource('locations', { page: 1, pageSize: 80, plantId: plant }),
  });

  useEffect(() => { scanRef.current?.focus(); }, [id]);

  const refresh = () => qc.invalidateQueries({ queryKey: ['shop-exec', id] });
  const run = useMutation({
    mutationFn: async (fn: () => Promise<unknown>) => fn(),
    onSuccess: () => { setError(''); refresh(); },
    onError: (e: Error) => setError(e.message),
  });

  const data = exec.data;
  const can = (a: string) => data?.allowedActions.includes(a);

  useEffect(() => {
    if (data?.expectedMaterialId && !outMat) setOutMat(data.expectedMaterialId);
  }, [data, outMat]);

  async function onScan(e: FormEvent) {
    e.preventDefault();
    setError('');
    try {
      const row = await scanExecution(id, barcode.trim());
      setScan(row);
      setQty(row.availableQuantity ? String(row.availableQuantity) : '');
      setContentId(row.contents[0]?.id ?? '');
      if (!wh) setWh(row.warehouseCode);
      if (!loc) setLoc(row.locationCode);
      if (!row.canConsume) setError(row.inactiveReason || 'Paket tüketilemez.');
      requestAnimationFrame(() => qtyRef.current?.focus());
    } catch (err) {
      setScan(null);
      setError(err instanceof Error ? err.message : 'Tarama hatası');
    }
  }

  async function onConsume() {
    if (!scan) return;
    await run.mutateAsync(() => consumeExecution(id, {
      packageId: scan.packageId,
      barcode: scan.barcode,
      quantity: Number(qty),
      packageContentId: contentId || undefined,
      idempotencyKey: `${id}-${scan.packageId}-${qty}`,
    }));
    setScan(null);
    setBarcode('');
    scanRef.current?.focus();
  }

  async function onComplete() {
    await run.mutateAsync(() => completeExecution(id, data?.outputType === 'NONE' ? {} : {
      outputMaterialId: outMat,
      warehouseCode: wh,
      locationCode: loc,
      lines: [{
        physicalGroupLabel: 'İstif',
        thicknessMm: t ? Number(t) : undefined,
        widthMm: w ? Number(w) : undefined,
        lengthMm: l ? Number(l) : undefined,
        pieceCount: pcs ? Number(pcs) : undefined,
      }],
    }));
    nav({ to: `/production/shop-floor/work-centers/${data?.workCenterCode ? id : id}` });
  }

  return (
    <div className="mx-auto max-w-3xl space-y-4 p-4 pb-28">
      <div className="flex items-start justify-between gap-3">
        <Link to="/production/shop-floor" className="text-sm underline">← Saha</Link>
        <ShopFloorReportButton
          workCenterId={data?.workCenterId}
          workCenterCode={data?.workCenterCode}
          executionId={data?.id}
          executionNumber={data?.number}
          productionOrderNumber={data?.productionOrderNumber}
        />
      </div>
      <header className="space-y-1">
        <p className="text-sm text-muted-foreground">{data?.productionOrderNumber}</p>
        <h1 className="text-2xl font-semibold">{data?.operationName}</h1>
        <p>{data?.workCenterName} · {data?.status}</p>
        <p className="font-mono text-3xl">{clock(data?.totalRunMinutes ?? 0)}</p>
      </header>
      {error ? <p className="rounded bg-red-50 p-3 text-red-800">{error}</p> : null}

      {can('SCAN') ? (
        <Card>
          <CardHeader><CardTitle>Barkod okut</CardTitle></CardHeader>
          <CardContent>
            <form className="space-y-2" onSubmit={onScan}>
              <Input ref={scanRef} autoFocus value={barcode} onChange={(e) => setBarcode(e.target.value)} placeholder="Barkod okutun" className="h-14 text-lg" />
              <p className="text-xs text-muted-foreground">Okuyucu Enter ile arar. Malzeme, lot, depo ve lokasyon otomatik gelir.</p>
              <button type="submit" className="text-sm underline">Elle ara</button>
            </form>
            {scan ? (
              <div className="mt-4 space-y-2 rounded border p-3">
                <p className="font-medium">{scan.packageNo}</p>
                <p>{scan.materialName} · Lot {scan.sourceLotNumber}</p>
                <p>{scan.warehouseCode} / {scan.locationCode} · kullanılabilir {scan.availableQuantity} {scan.unit}</p>
                {scan.contents.length > 1 ? (
                  <select className="h-12 w-full rounded border px-2" value={contentId} onChange={(e) => setContentId(e.target.value)}>
                    {scan.contents.map((c) => (
                      <option key={c.id} value={c.id}>{c.measurement || 'ölçü'} · {c.quantity} {c.unit}</option>
                    ))}
                  </select>
                ) : null}
                <label className="text-sm">Miktar ({scan.unit})</label>
                <Input ref={qtyRef} value={qty} onChange={(e) => setQty(e.target.value)} className="h-12" inputMode="decimal" />
                <Button className="h-12 w-full" disabled={!scan.canConsume || run.isPending} onClick={() => void onConsume()}>Onayla</Button>
              </div>
            ) : null}
          </CardContent>
        </Card>
      ) : null}

      <Card>
        <CardHeader><CardTitle>Tüketilen girdiler</CardTitle></CardHeader>
        <CardContent className="space-y-2">
          {(data?.inputs ?? []).map((row) => (
            <div key={row.id} className="rounded border p-2 text-sm">
              <div>{row.barcode} · {row.materialCode}</div>
              <div>Lot {row.lotNumber || '—'} · {row.physicalMeasure} · {row.consumedQuantity} {row.unit}</div>
              <div>Kalan paket: {row.remainingPackageQuantity}</div>
            </div>
          ))}
          {data?.inputs.length === 0 ? <p className="text-sm text-muted-foreground">Henüz tüketim yok</p> : null}
        </CardContent>
      </Card>

      <Card>
        <CardHeader><CardTitle>Fire</CardTitle></CardHeader>
        <CardContent className="space-y-2">
          {(data?.scraps ?? []).map((s) => (
            <p key={s.id} className="text-sm">{s.quantity} {s.unit} · {s.reasonCode} · {istanbul(s.recordedAt)}</p>
          ))}
          {can('SCRAP') ? (
            <div className="grid gap-2">
              <Input value={scrapQty} onChange={(e) => setScrapQty(e.target.value)} placeholder="Miktar" className="h-12" />
              <select className="h-12 rounded border px-2" value={scrapReason} onChange={(e) => setScrapReason(e.target.value)}>
                {['CUTTING', 'DEFECT', 'CRACK', 'MOISTURE', 'QUALITY', 'MACHINE', 'SETUP', 'TRIM', 'OTHER'].map((r) => <option key={r}>{r}</option>)}
              </select>
              <Button className="h-12" onClick={() => run.mutate(() => addScrap(id, Number(scrapQty), scrapReason, data?.unit ?? 'M3'))}>Fire ekle</Button>
            </div>
          ) : null}
        </CardContent>
      </Card>

      {data && data.outputType !== 'NONE' ? (
        <Card>
          <CardHeader><CardTitle>Çıktı</CardTitle></CardHeader>
          <CardContent className="grid gap-2">
            <select className="h-12 rounded border px-2" value={outMat} onChange={(e) => setOutMat(e.target.value)}>
              <option value="">Malzeme</option>
              {(materials.data?.items ?? materials.data?.Items ?? []).map((m: { id?: string; Id?: string; code?: string; Code?: string; name?: string }) => (
                <option key={m.id || m.Id} value={m.id || m.Id}>{m.code || m.Code} {m.name}</option>
              ))}
            </select>
            <select className="h-12 rounded border px-2" value={wh} onChange={(e) => setWh(e.target.value)}>
              <option value="">Depo</option>
              {(warehouses.data?.items ?? warehouses.data?.Items ?? []).map((m: { code?: string; Code?: string }) => (
                <option key={m.code || m.Code} value={m.code || m.Code}>{m.code || m.Code}</option>
              ))}
            </select>
            <select className="h-12 rounded border px-2" value={loc} onChange={(e) => setLoc(e.target.value)}>
              <option value="">Lokasyon</option>
              {(locations.data?.items ?? locations.data?.Items ?? []).map((m: { code?: string; Code?: string; warehouseCode?: string }) => (
                <option key={m.code || m.Code} value={m.code || m.Code}>{m.code || m.Code}</option>
              ))}
            </select>
            <div className="grid grid-cols-2 gap-2">
              <Input placeholder="Kalınlık mm" value={t} onChange={(e) => setT(e.target.value)} className="h-12" />
              <Input placeholder="En mm" value={w} onChange={(e) => setW(e.target.value)} className="h-12" />
              <Input placeholder="Boy mm" value={l} onChange={(e) => setL(e.target.value)} className="h-12" />
              <Input placeholder="Adet" value={pcs} onChange={(e) => setPcs(e.target.value)} className="h-12" />
            </div>
            {data.productionLotNumber ? <p>Lot {data.productionLotNumber} · QC {data.qcStatus}</p> : null}
          </CardContent>
        </Card>
      ) : null}

      <Card>
        <CardHeader><CardTitle>Geçmiş</CardTitle></CardHeader>
        <CardContent>
          {(data?.events ?? []).map((e, i) => (
            <p key={i} className="text-sm">{istanbul(e.occurredAt)} · {e.eventType}{e.reasonCode ? ` — ${e.reasonCode}` : ''}</p>
          ))}
        </CardContent>
      </Card>

      <footer className="fixed inset-x-0 bottom-0 grid grid-cols-2 gap-2 bg-background p-3 shadow">
        {can('PAUSE') ? <Button className="h-14" variant="secondary" onClick={() => run.mutate(() => pauseExecution(id))}>Duraklat</Button> : null}
        {can('RESUME') ? <Button className="h-14" onClick={() => run.mutate(() => resumeExecution(id))}>Sürdür</Button> : null}
        {can('DOWNTIME') && !data?.events.some((e, _, all) => e.eventType === 'DOWNTIME_START' && !all.slice(all.indexOf(e) + 1).some((x) => x.eventType === 'DOWNTIME_END')) ? (
          <div className="col-span-2 flex gap-2">
            <select className="h-14 flex-1 rounded border px-2" value={dtReason} onChange={(e) => setDtReason(e.target.value)}>
              {['MACHINE', 'MATERIAL', 'QUALITY', 'SETUP', 'MAINTENANCE', 'OTHER'].map((r) => <option key={r}>{r}</option>)}
            </select>
            <Button className="h-14 flex-1" variant="secondary" onClick={() => run.mutate(() => startDowntime(id, dtReason))}>Duruş</Button>
          </div>
        ) : null}
        {data?.events.length && data.events.filter((e) => e.eventType === 'DOWNTIME_START').length > data.events.filter((e) => e.eventType === 'DOWNTIME_END').length ? (
          <Button className="h-14 col-span-2" onClick={() => run.mutate(() => endDowntime(id))}>Duruşu bitir</Button>
        ) : null}
        {can('COMPLETE') ? <Button className="h-14 col-span-2" onClick={() => void onComplete()}>Operasyonu tamamla</Button> : null}
      </footer>
    </div>
  );
}

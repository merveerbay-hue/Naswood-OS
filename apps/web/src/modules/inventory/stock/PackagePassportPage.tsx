import { Link, useNavigate, useParams, useSearch } from '@tanstack/react-router';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { useMemo, useState } from 'react';
import { Button, Card, CardContent, CardHeader, CardTitle, Input } from '@naswood/ui';
import { plantDisplayName } from '@/modules/inventory/locations/locationCatalog';
import {
  getPackageByPublicId,
  getPackagePassport,
  mergePackages,
  partialMovePackage,
  recordLabelPrint,
  relocatePackage,
  repackPackage,
  splitPackage,
  type PackagePassport,
} from './packagePassportApi';
import { printPackageLabels } from './packageLabelPrint';

function can(p: PackagePassport, action: string) {
  return (p.allowedActions ?? []).includes(action);
}

export function PackagePassportPage() {
  const params = useParams({ strict: false }) as { id?: string; publicId?: string };
  const search = useSearch({ strict: false }) as { pk?: string };
  const navigate = useNavigate();
  const qc = useQueryClient();
  const [wh, setWh] = useState('');
  const [loc, setLoc] = useState('');
  const [moveMode, setMoveMode] = useState<'full' | 'partial'>('full');
  const [childQtys, setChildQtys] = useState<Record<string, string>>({});
  const [mergeIds, setMergeIds] = useState('');
  const [group, setGroup] = useState('');
  const [err, setErr] = useState<string | null>(null);
  const [ok, setOk] = useState<string | null>(null);
  const lookupKey = params.publicId || params.id || '';

  const q = useQuery({
    queryKey: ['package-passport', lookupKey, search?.pk],
    queryFn: () => (params.publicId ? getPackageByPublicId(params.publicId) : getPackagePassport(params.id!)),
    enabled: Boolean(lookupKey),
  });
  const p = q.data;

  const preview = useMemo(() => {
    if (!p) return { child: 0, original: 0 };
    const child =
      (p.contents ?? []).length === 0
        ? Number(childQtys.total || 0) || 0
        : (p.contents ?? []).reduce((sum, c) => sum + (Number(childQtys[c.id ?? String(c.lineNo)] || 0) || 0), 0);
    return { child, original: Math.max(0, p.quantity - child) };
  }, [p, childQtys]);

  const lines = () => {
    if (!p) return [];
    if ((p.contents ?? []).length === 0) {
      const quantity = Number(childQtys.total || 0);
      return quantity > 0 ? [{ quantity }] : [];
    }
    return (p.contents ?? [])
      .map((c) => ({
        sourceContentId: c.id,
        thicknessMm: c.thicknessMm,
        widthMm: c.widthMm,
        lengthMm: c.lengthMm,
        quantity: Number(childQtys[c.id ?? String(c.lineNo)] || 0),
        pieceCount: c.pieceCount != null ? Number(childQtys[c.id ?? String(c.lineNo)] || 0) : null,
      }))
      .filter((l) => l.quantity > 0);
  };

  const printDocs = async (docs: PackagePassport[]) => {
    printPackageLabels(docs);
    for (const doc of docs) await recordLabelPrint(doc.id);
  };

  const printMut = useMutation({
    mutationFn: async () => {
      if (!p) throw new Error('Paket yok');
      await printDocs([p]);
      return getPackagePassport(p.id);
    },
    onSuccess: (doc) => {
      void qc.setQueryData(['package-passport', lookupKey, search?.pk], doc);
    },
    onError: (e: Error) => setErr(e.message),
  });

  const moveMut = useMutation({
    mutationFn: async () => {
      if (!p) throw new Error('Paket yok');
      if (moveMode === 'partial') return partialMovePackage(p.id, wh, loc, lines(), undefined, group);
      return relocatePackage(p.id, wh, loc);
    },
    onSuccess: (doc) => {
      if ('operationType' in doc) {
        setOk(doc.message);
        if (doc.source) void qc.setQueryData(['package-passport', lookupKey, search?.pk], doc.source);
      } else {
        void qc.setQueryData(['package-passport', lookupKey, search?.pk], doc);
        setOk('Paket lokasyonu güncellendi.');
      }
      setErr(null);
    },
    onError: (e: Error) => setErr(e.message),
  });

  const splitMut = useMutation({
    mutationFn: () => splitPackage(p!.id, lines(), undefined, group),
    onSuccess: (doc) => {
      setOk(doc.message);
      setErr(null);
      if (doc.source) void qc.setQueryData(['package-passport', lookupKey, search?.pk], doc.source);
    },
    onError: (e: Error) => setErr(e.message),
  });

  const repackMut = useMutation({
    mutationFn: () => repackPackage(p!.id, undefined, group),
    onSuccess: (doc) => {
      setOk(doc.message);
      setErr(null);
      if (doc.target) navigate({ to: '/inventory/stock/packages/$id', params: { id: doc.target.id } });
    },
    onError: (e: Error) => setErr(e.message),
  });

  const mergeMut = useMutation({
    mutationFn: () => {
      const extra = mergeIds.split(/[\s,;]+/).map((x) => x.trim()).filter(Boolean);
      return mergePackages([p!.id, ...extra], undefined, group);
    },
    onSuccess: (doc) => {
      setOk(doc.message);
      setErr(null);
      if (doc.target) navigate({ to: '/inventory/stock/packages/$id', params: { id: doc.target.id } });
    },
    onError: (e: Error) => setErr(e.message),
  });

  if (q.isError) {
    return (
      <div className="space-y-2">
        <p className="text-sm text-red-600">{(q.error as Error).message || 'Barkod sistemde bulunamadı.'}</p>
        <Button variant="secondary" onClick={() => navigate({ to: '/inventory/stock/scan' })}>
          Barkod tara
        </Button>
      </div>
    );
  }
  if (!p) return <p className="text-sm text-[var(--text-muted)]">Yükleniyor…</p>;

  return (
    <div className="space-y-4">
      <div>
        <p className="text-xs text-[var(--text-muted)]">INV-PKG · paket pasaportu</p>
        <h1 className="text-2xl font-semibold uppercase">{p.materialName}</h1>
        <p className="font-mono text-lg">{p.materialCode}</p>
      </div>
      {p.inactiveReason ? (
        <p className="rounded-md border border-amber-500/40 bg-amber-500/10 px-3 py-2 text-sm font-medium">{p.inactiveReason}</p>
      ) : null}
      {p.labelHint ? <p className="rounded-md border px-3 py-2 text-sm">{p.labelHint}</p> : null}
      {p.packageBalanceMismatch ? (
        <p className="rounded-md border border-amber-500/40 bg-amber-500/10 px-3 py-2 text-sm">
          PAKET / STOK UYUMSUZLUĞU — InventoryBalance paketten türetilmez; bakiye otomatik değiştirilmedi.
        </p>
      ) : null}
      {err ? <p className="text-sm text-red-600">{err}</p> : null}
      {ok ? <p className="text-sm text-emerald-700">{ok}</p> : null}

      <Card>
        <CardHeader>
          <CardTitle>Fiziksel içerik</CardTitle>
        </CardHeader>
        <CardContent>
          <table className="w-full text-sm">
            <thead>
              <tr className="text-left text-[var(--text-muted)]">
                <th>Ölçü</th>
                <th>Adet</th>
                <th>Miktar</th>
              </tr>
            </thead>
            <tbody>
              {(p.contents ?? []).map((c) => (
                <tr key={c.id ?? c.lineNo} className="border-t border-[var(--border)]">
                  <td>{c.measurement || '—'}</td>
                  <td>{c.pieceCount ?? '—'}</td>
                  <td>
                    {c.quantity} {c.unitOfMeasure}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
          <p className="mt-2 text-sm">
            Toplam {p.totalPieceCount != null ? `${p.totalPieceCount} PCS · ` : ''}
            {p.quantity} {p.stockUnit || p.unitOfMeasure}
          </p>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>İzlenebilirlik</CardTitle>
        </CardHeader>
        <CardContent className="grid gap-1 text-sm md:grid-cols-2">
          <div>Lot: {p.lotNumber}</div>
          <div>Paket: {p.packageNo}</div>
          <div>Barkod: {p.barcode}</div>
          <div>Kaynak: {p.sourceType || '—'}</div>
          <div>Referans: {p.sourceReferenceNo || '—'}</div>
          {p.sourceType === 'PRODUCTION' ? (
            <>
              <div>Üretim emri: {p.productionOrderNumber || '—'}</div>
              <div>Kaynak lot: {(p.sourceLotNumbers ?? []).join(', ') || `${p.sourceLotCount ?? 0} lot`}</div>
            </>
          ) : null}
          <div>
            Konum: {plantDisplayName(p.factory)} · {p.warehouseCode} / {p.locationCode} · {p.status}
          </div>
          <div>Oluşturma: {new Date(p.createdAt).toLocaleString('tr-TR')}</div>
          <div>Son hareket: {p.lastMovementAt ? new Date(p.lastMovementAt).toLocaleString('tr-TR') : '—'}</div>
        </CardContent>
      </Card>

      {(p.relations ?? []).length > 0 ? (
        <Card>
          <CardHeader>
            <CardTitle>Kaynak / yeni paketler</CardTitle>
          </CardHeader>
          <CardContent className="space-y-1 text-sm">
            {(p.relations ?? []).map((r) => (
              <div key={`${r.sourcePackageId}-${r.targetPackageId}-${r.relationType}`}>
                {r.relationType === 'SPLIT' && r.direction === 'IN' ? `Split From: ${r.sourcePackageNo}` : null}
                {r.relationType === 'SPLIT' && r.direction === 'OUT' ? `Split Into: ${r.targetPackageNo} (${r.quantity} ${r.unit})` : null}
                {r.relationType === 'MERGE' && r.direction === 'IN' ? `Merged From: ${r.sourcePackageNo}` : null}
                {r.relationType === 'MERGE' && r.direction === 'OUT' ? `Merged Into: ${r.targetPackageNo}` : null}
                {r.relationType === 'REPACK' && r.direction === 'IN' ? `Repacked From: ${r.sourcePackageNo}` : null}
                {r.relationType === 'REPACK' && r.direction === 'OUT' ? `Repacked Into: ${r.targetPackageNo}` : null}
                {r.relationType === 'PARTIAL_MOVE' && r.direction === 'OUT' ? `Partial move → ${r.targetPackageNo}` : null}
                {r.relationType === 'PARTIAL_MOVE' && r.direction === 'IN' ? `Partial move from ${r.sourcePackageNo}` : null}
              </div>
            ))}
          </CardContent>
        </Card>
      ) : null}

      <Card>
        <CardHeader>
          <CardTitle>Paket yaşam döngüsü</CardTitle>
        </CardHeader>
        <CardContent>
          <table className="w-full text-sm">
            <thead>
              <tr className="text-left text-[var(--text-muted)]">
                <th>Zaman</th>
                <th>İşlem</th>
                <th>Nereden</th>
                <th>Nereye</th>
                <th>Miktar</th>
                <th>Referans</th>
              </tr>
            </thead>
            <tbody>
              {(p.movements ?? []).map((m, i) => (
                <tr key={`${m.reference}-${i}`} className="border-t border-[var(--border)]">
                  <td>{new Date(m.at).toLocaleString('tr-TR')}</td>
                  <td>{m.action}</td>
                  <td>{m.fromLocation || '—'}</td>
                  <td>{m.toLocation || '—'}</td>
                  <td>
                    {m.quantity} {m.unit}
                  </td>
                  <td>{m.reference}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </CardContent>
      </Card>

      <div className="flex flex-wrap gap-2">
        {can(p, 'PRINT_LABEL') ? <Button onClick={() => printMut.mutate()}>Etiket yazdır</Button> : null}
        <Link className="rounded-md border px-3 py-2 text-sm" to="/inventory/stock/movements">
          Hareketler
        </Link>
      </div>

      {can(p, 'MOVE') ? (
        <Card>
          <CardHeader>
            <CardTitle>Lokasyon değiştir</CardTitle>
          </CardHeader>
          <CardContent className="space-y-2">
            <div className="flex flex-wrap gap-3 text-sm">
              <label>
                <input type="radio" checked={moveMode === 'full'} onChange={() => setMoveMode('full')} /> Tüm paketi taşı
              </label>
              <label>
                <input type="radio" checked={moveMode === 'partial'} onChange={() => setMoveMode('partial')} /> Paketin bir kısmını taşı
              </label>
            </div>
            {moveMode === 'partial' ? (
              <p className="text-xs text-[var(--text-muted)]">Taşınan kısım yeni barkod alır. Orijinal barkod aynı lokasyonda kalır.</p>
            ) : (
              <p className="text-xs text-[var(--text-muted)]">Aynı paket no ve barkod korunur.</p>
            )}
            <div className="flex flex-wrap gap-2">
              <Input placeholder="Depo" value={wh} onChange={(e) => setWh(e.target.value)} />
              <Input placeholder="Lokasyon" value={loc} onChange={(e) => setLoc(e.target.value)} />
              <Button disabled={!wh || !loc || moveMut.isPending} onClick={() => moveMut.mutate()}>
                Taşı
              </Button>
            </div>
            {moveMode === 'partial' ? <ContentQtyEditor passport={p} childQtys={childQtys} setChildQtys={setChildQtys} preview={preview} /> : null}
          </CardContent>
        </Card>
      ) : null}

      {can(p, 'SPLIT') ? (
        <Card>
          <CardHeader>
            <CardTitle>Paketi böl</CardTitle>
          </CardHeader>
          <CardContent className="space-y-2">
            <Input placeholder="PhysicalGroupLabel (opsiyonel)" value={group} onChange={(e) => setGroup(e.target.value)} />
            <ContentQtyEditor passport={p} childQtys={childQtys} setChildQtys={setChildQtys} preview={preview} />
            <Button disabled={preview.child <= 0 || splitMut.isPending} onClick={() => splitMut.mutate()}>
              Onayla ve yeni paket oluştur
            </Button>
          </CardContent>
        </Card>
      ) : null}

      {can(p, 'REPACK') ? (
        <Card>
          <CardHeader>
            <CardTitle>Yeniden paketle</CardTitle>
          </CardHeader>
          <CardContent className="space-y-2">
            <p className="text-sm">Yeni fiziksel paket kimliği ve barkodu oluşturulacaktır.</p>
            <Button disabled={repackMut.isPending} onClick={() => repackMut.mutate()}>
              Yeniden paketle
            </Button>
          </CardContent>
        </Card>
      ) : null}

      {can(p, 'MERGE') ? (
        <Card>
          <CardHeader>
            <CardTitle>Birleştir</CardTitle>
          </CardHeader>
          <CardContent className="space-y-2">
            <p className="text-sm">Aynı malzeme / lot / lokasyon / status paket ID’lerini ekleyin. Yeni hedef barkod üretilir.</p>
            <Input placeholder="Diğer paket ID (virgülle)" value={mergeIds} onChange={(e) => setMergeIds(e.target.value)} />
            <Button disabled={!mergeIds.trim() || mergeMut.isPending} onClick={() => mergeMut.mutate()}>
              Birleştir
            </Button>
          </CardContent>
        </Card>
      ) : null}
    </div>
  );
}

function ContentQtyEditor({
  passport,
  childQtys,
  setChildQtys,
  preview,
}: {
  passport: PackagePassport;
  childQtys: Record<string, string>;
  setChildQtys: (v: Record<string, string>) => void;
  preview: { child: number; original: number };
}) {
  return (
    <div className="space-y-2 text-sm">
      {(passport.contents ?? []).map((c) => {
        const key = c.id ?? String(c.lineNo);
        return (
          <div key={key} className="flex flex-wrap items-center gap-2">
            <span className="min-w-40">
              {c.measurement || '—'} · {c.quantity} {c.unitOfMeasure}
            </span>
            <Input
              className="w-28"
              placeholder="Child qty"
              value={childQtys[key] ?? ''}
              onChange={(e) => setChildQtys({ ...childQtys, [key]: e.target.value })}
            />
          </div>
        );
      })}
      {(passport.contents ?? []).length === 0 ? (
        <Input
          className="w-28"
          placeholder="Child qty"
          value={childQtys.total ?? ''}
          onChange={(e) => setChildQtys({ ...childQtys, total: e.target.value })}
        />
      ) : null}
      <p>
        Orijinal sonra: {preview.original} {passport.unitOfMeasure} · Yeni paket: {preview.child} {passport.unitOfMeasure}
      </p>
    </div>
  );
}

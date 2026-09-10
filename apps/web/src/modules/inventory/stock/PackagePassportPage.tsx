import { Link, useNavigate, useParams, useSearch } from '@tanstack/react-router';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { useState } from 'react';
import { Button, Card, CardContent, CardHeader, CardTitle, Input } from '@naswood/ui';
import { plantDisplayName } from '@/modules/inventory/locations/locationCatalog';
import { getPackagePassport, recordLabelPrint, relocatePackage } from './packagePassportApi';
import { printPackageLabels } from './packageLabelPrint';

export function PackagePassportPage() {
  const { id: packageId } = useParams({ strict: false }) as { id: string };
  const search = useSearch({ strict: false }) as { pk?: string };
  const navigate = useNavigate();
  const qc = useQueryClient();
  const [wh, setWh] = useState('');
  const [loc, setLoc] = useState('');
  const [err, setErr] = useState<string | null>(null);

  const q = useQuery({
    queryKey: ['package-passport', packageId, search?.pk],
    queryFn: () => getPackagePassport(packageId),
    enabled: Boolean(packageId),
  });
  const p = q.data;

  const printMut = useMutation({
    mutationFn: async () => {
      if (!p) throw new Error('Paket yok');
      printPackageLabels([p]);
      return recordLabelPrint(p.id);
    },
    onSuccess: (doc) => {
      void qc.setQueryData(['package-passport', packageId, search?.pk], doc);
    },
    onError: (e: Error) => setErr(e.message),
  });

  const moveMut = useMutation({
    mutationFn: () => relocatePackage(packageId, wh, loc),
    onSuccess: (doc) => {
      void qc.setQueryData(['package-passport', packageId, search?.pk], doc);
      setErr(null);
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
      {p.packageBalanceMismatch ? (
        <p className="rounded-md border border-amber-500/40 bg-amber-500/10 px-3 py-2 text-sm">
          PAKET / STOK UYUMSUZLUĞU — InventoryBalance paketten türetilmez; bakiye otomatik değiştirilmedi.
        </p>
      ) : null}
      {err ? <p className="text-sm text-red-600">{err}</p> : null}

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
                <tr key={c.lineNo} className="border-t border-[var(--border)]">
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
          <div>
            Konum: {plantDisplayName(p.factory)} · {p.warehouseCode} / {p.locationCode} · {p.status}
          </div>
          <div>Oluşturma: {new Date(p.createdAt).toLocaleString('tr-TR')}</div>
          <div>Son hareket: {p.lastMovementAt ? new Date(p.lastMovementAt).toLocaleString('tr-TR') : '—'}</div>
        </CardContent>
      </Card>

      <div className="flex flex-wrap gap-2">
        <Button onClick={() => printMut.mutate()}>Etiket yazdır</Button>
        <Button variant="secondary" onClick={() => printMut.mutate()}>
          Etiketi yeniden yazdır
        </Button>
        <Link className="rounded-md border px-3 py-2 text-sm" to="/inventory/stock/movements">
          Hareketler
        </Link>
        <Link className="rounded-md border px-3 py-2 text-sm" to="/inventory/stock/balances">
          Stok detayı
        </Link>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Lokasyon değiştir</CardTitle>
        </CardHeader>
        <CardContent className="flex flex-wrap gap-2">
          <Input placeholder="Depo" value={wh} onChange={(e) => setWh(e.target.value)} />
          <Input placeholder="Lokasyon" value={loc} onChange={(e) => setLoc(e.target.value)} />
          <Button disabled={!wh || !loc || moveMut.isPending} onClick={() => moveMut.mutate()}>
            Taşı
          </Button>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>Hareket geçmişi</CardTitle>
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
    </div>
  );
}

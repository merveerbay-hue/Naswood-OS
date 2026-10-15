import { useNavigate } from '@tanstack/react-router';
import { useState } from 'react';
import { Button, Card, CardContent, CardHeader, CardTitle, Input } from '@naswood/ui';
import { resolveScan } from './identityCode';
import { getPackageByBarcode } from './packagePassportApi';

export function PackageScanPage() {
  const navigate = useNavigate();
  const [code, setCode] = useState('');
  const [err, setErr] = useState<string | null>(null);
  const [busy, setBusy] = useState(false);

  async function lookup(raw: string) {
    if (!raw.trim()) return;
    setErr(null);
    const scan = resolveScan(raw);
    if (scan.kind === 'invalid' || scan.kind === 'foreign' || scan.kind === 'unsupported') {
      setErr(scan.kind === 'invalid' ? scan.message : `${scan.code}: ${scan.message}`);
      return;
    }
    if (scan.kind === 'lot') {
      navigate({ to: '/inventory/stock/lots' });
      return;
    }
    if (scan.kind === 'transfer') {
      navigate({ to: '/inventory/operations/transfers' });
      return;
    }
    setBusy(true);
    try {
      const doc = await getPackageByBarcode(scan.code);
      navigate({ to: '/inventory/packages/p/$publicId', params: { publicId: doc.publicId } });
    } catch (e) {
      setErr(e instanceof Error ? e.message : 'Barkod sistemde bulunamadı.');
    } finally {
      setBusy(false);
    }
  }

  return (
    <Card>
      <CardHeader>
        <CardTitle>Barkod tara</CardTitle>
      </CardHeader>
      <CardContent className="space-y-3">
        <p className="text-sm text-[var(--text-muted)]">
          Paket kodu (PKG-BUC-000001) okutun veya yazın + Enter. Ön eksiz ya da ANT kodlar Antalya'ya aittir.
        </p>
        <Input
          autoFocus
          placeholder="PKG-BUC-000001"
          value={code}
          onChange={(e) => setCode(e.target.value)}
          onKeyDown={(e) => {
            if (e.key === 'Enter') void lookup(code);
          }}
        />
        <Button disabled={busy || !code.trim()} onClick={() => void lookup(code)}>
          Paketi aç
        </Button>
        {err ? <p className="text-sm text-red-600">{err}</p> : null}
      </CardContent>
    </Card>
  );
}

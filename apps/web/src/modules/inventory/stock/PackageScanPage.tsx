import { useNavigate } from '@tanstack/react-router';
import { useState } from 'react';
import { Button, Card, CardContent, CardHeader, CardTitle, Input } from '@naswood/ui';
import { getPackageByBarcode } from './packagePassportApi';

export function PackageScanPage() {
  const navigate = useNavigate();
  const [code, setCode] = useState('');
  const [err, setErr] = useState<string | null>(null);
  const [busy, setBusy] = useState(false);

  async function lookup(raw: string) {
    const barcode = raw.trim();
    if (!barcode) return;
    setBusy(true);
    setErr(null);
    try {
      const doc = await getPackageByBarcode(barcode);
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
          Tam barkod + Enter. Malzeme/lot aramayın — sistem paketin allowedActions listesine göre taşı, böl, paketle veya üretime ver der.
        </p>
        <Input
          autoFocus
          placeholder="NWPKG-F01-26-000001"
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

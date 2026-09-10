import { Link } from '@tanstack/react-router';
import { useQuery } from '@tanstack/react-query';
import { Card, CardContent, CardHeader, CardTitle } from '@naswood/ui';
import { listShopFloorFeedback } from './shopFloorApi';

const LABELS: Record<string, string> = {
  BARCODE_FAIL: 'Barkod okumadı',
  FIELD_UNNECESSARY: 'Alan gereksiz',
  TOO_SLOW: 'İşlem uzun',
  WRONG_STOCK: 'Yanlış stok',
  OTHER: 'Diğer',
};

export function ShopFloorFeedbackPage() {
  const q = useQuery({ queryKey: ['shop-floor-feedback'], queryFn: listShopFloorFeedback });
  return (
    <div className="mx-auto max-w-3xl space-y-4 p-4">
      <Link to="/production/shop-floor" className="text-sm underline">← Saha</Link>
      <h1 className="text-2xl font-semibold">Saha geri bildirimleri</h1>
      <p className="text-sm text-muted-foreground">Pilot notları. WhatsApp yerine buradan toplanır.</p>
      {(q.data ?? []).map((row) => (
        <Card key={row.id}>
          <CardHeader className="pb-2">
            <CardTitle className="text-lg">{LABELS[row.topic] ?? row.topic}</CardTitle>
            <p className="text-sm text-muted-foreground">
              {row.impact === 'BLOCKING' ? 'İşi durduruyor' : 'Devam edebiliyorum'}
              {' · '}
              {new Date(row.occurredAt).toLocaleString('tr-TR', { timeZone: 'Europe/Istanbul' })} · {row.userId}
            </p>
          </CardHeader>
          <CardContent className="space-y-1 text-sm">
            {row.note ? <p>{row.note}</p> : null}
            <p>{row.screen}</p>
            <p>{row.workCenterCode || '—'} · {row.executionNumber || '—'} · {row.productionOrderNumber || '—'}</p>
            <p className="font-mono text-xs">{row.appVersion || '—'} {row.gitSha ? `· ${row.gitSha.slice(0, 12)}` : ''}</p>
          </CardContent>
        </Card>
      ))}
      {q.data?.length === 0 ? <p>Henüz bildirim yok.</p> : null}
    </div>
  );
}

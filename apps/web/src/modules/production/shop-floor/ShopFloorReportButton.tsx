import { useState } from 'react';
import { useMutation } from '@tanstack/react-query';
import { Button, Input } from '@naswood/ui';
import { submitShopFloorFeedback } from './shopFloorApi';

const TOPICS = [
  { id: 'BARCODE_FAIL', label: 'Barkod okumadı' },
  { id: 'FIELD_UNNECESSARY', label: 'Bu alan gereksiz' },
  { id: 'TOO_SLOW', label: 'Bu işlem çok uzun' },
  { id: 'WRONG_STOCK', label: 'Yanlış stok gösteriyor' },
  { id: 'OTHER', label: 'Diğer' },
] as const;

type Props = {
  workCenterId?: string;
  workCenterCode?: string;
  executionId?: string;
  executionNumber?: string;
  productionOrderNumber?: string;
};

export function ShopFloorReportButton(props: Props) {
  const [open, setOpen] = useState(false);
  const [topic, setTopic] = useState<string>('BARCODE_FAIL');
  const [note, setNote] = useState('');
  const [done, setDone] = useState('');

  const send = useMutation({
    mutationFn: () => submitShopFloorFeedback({
      topic,
      note,
      screen: typeof window !== 'undefined' ? window.location.pathname : '',
      workCenterId: props.workCenterId,
      workCenterCode: props.workCenterCode ?? '',
      executionId: props.executionId,
      executionNumber: props.executionNumber ?? '',
      productionOrderNumber: props.productionOrderNumber ?? '',
    }),
    onSuccess: () => {
      setDone('Kaydedildi. Teşekkürler.');
      setNote('');
      setTimeout(() => { setOpen(false); setDone(''); }, 1200);
    },
  });

  return (
    <div className="text-right">
      <button type="button" className="text-sm underline" onClick={() => setOpen((v) => !v)}>
        Sorun bildir
      </button>
      {open ? (
        <div className="mt-2 rounded border bg-background p-3 text-left shadow">
          <p className="mb-2 text-sm font-medium">Ne oldu?</p>
          <div className="grid gap-2">
            {TOPICS.map((t) => (
              <Button key={t.id} type="button" variant={topic === t.id ? 'default' : 'secondary'} className="h-11 justify-start" onClick={() => setTopic(t.id)}>
                {t.label}
              </Button>
            ))}
            <Input value={note} onChange={(e) => setNote(e.target.value)} placeholder="Kısa not (isteğe bağlı)" className="h-11" />
            {done ? <p className="text-sm text-green-700">{done}</p> : null}
            {send.isError ? <p className="text-sm text-red-700">{send.error.message}</p> : null}
            <Button className="h-12" disabled={send.isPending} onClick={() => send.mutate()}>Gönder</Button>
          </div>
        </div>
      ) : null}
    </div>
  );
}

import { Button, Input } from '@naswood/ui';
import { useI18n } from '@/i18n';

export type TruckInfo = {
  plate: string;
  trailer: string;
  driver: string;
  supplier: string;
  arrivalDate: string;
  arrivalTime: string;
  gate: string;
};

export type EvidenceDocKind =
  | 'deliveryNote'
  | 'packingList'
  | 'materialList'
  | 'excel'
  | 'pdf'
  | 'invoice'
  | 'photo'
  | 'scan';

export type PhotoSlot =
  | 'truckOverview'
  | 'cargo'
  | 'package'
  | 'packageLabel'
  | 'material'
  | 'damage';

const DOC_KINDS: EvidenceDocKind[] = [
  'deliveryNote',
  'packingList',
  'materialList',
  'excel',
  'pdf',
  'invoice',
  'photo',
  'scan',
];

const PHOTO_SLOTS: PhotoSlot[] = [
  'truckOverview',
  'cargo',
  'package',
  'packageLabel',
  'material',
  'damage',
];

type Props = {
  truck: TruckInfo;
  onTruckChange: (next: TruckInfo) => void;
  docs: EvidenceDocKind[];
  onToggleDoc: (kind: EvidenceDocKind) => void;
  photos: Partial<Record<PhotoSlot, boolean>>;
  onTogglePhoto: (slot: PhotoSlot) => void;
  disabled?: boolean;
};

/** Stage 1 — Kamyon & Kanıt: collect truck + photos + documents only (no compare). */
export function TruckEvidenceStep({
  truck,
  onTruckChange,
  docs,
  onToggleDoc,
  photos,
  onTogglePhoto,
  disabled,
}: Props) {
  const { t } = useI18n();

  function setField<K extends keyof TruckInfo>(key: K, value: TruckInfo[K]) {
    onTruckChange({ ...truck, [key]: value });
  }

  const photoCount = PHOTO_SLOTS.filter((s) => photos[s]).length;

  return (
    <div className="space-y-6">
      <p className="text-sm text-[var(--text-secondary)]">{t('wb.rcv.ops.stage1Intro')}</p>

      <section className="space-y-3">
        <h3 className="text-sm font-semibold tracking-tight">{t('wb.rcv.ops.truckSection')}</h3>
        <div className="grid gap-2 sm:grid-cols-2 lg:grid-cols-3">
          {(
            [
              ['plate', t('wb.rcv.truckPlate')],
              ['trailer', t('wb.rcv.trailer')],
              ['driver', t('wb.rcv.driver')],
              ['supplier', t('wb.rcv.supplier')],
              ['arrivalDate', t('wb.rcv.arrivalDate')],
              ['arrivalTime', t('wb.rcv.arrivalTime')],
              ['gate', t('wb.rcv.gate')],
            ] as const
          ).map(([key, label]) => (
            <label key={key} className="block space-y-1 text-sm">
              <span className="text-[var(--text-secondary)]">{label}</span>
              <Input
                type={key === 'arrivalDate' ? 'date' : key === 'arrivalTime' ? 'time' : 'text'}
                value={truck[key]}
                disabled={disabled}
                onChange={(e) => setField(key, e.target.value)}
              />
            </label>
          ))}
        </div>
      </section>

      <section className="space-y-3">
        <div className="flex flex-wrap items-baseline justify-between gap-2">
          <h3 className="text-sm font-semibold tracking-tight">{t('wb.rcv.ops.photoSection')}</h3>
          <p className="text-xs text-[var(--text-muted)]">
            {photoCount}/{PHOTO_SLOTS.length} · {t('wb.rcv.ops.photoHint')}
          </p>
        </div>
        <div className="grid gap-2 sm:grid-cols-2 lg:grid-cols-3">
          {PHOTO_SLOTS.map((slot) => {
            const on = !!photos[slot];
            return (
              <button
                key={slot}
                type="button"
                disabled={disabled}
                onClick={() => onTogglePhoto(slot)}
                className={`rounded-md border px-3 py-3 text-left text-sm transition-colors ${
                  on
                    ? 'border-[var(--color-primary)] bg-[var(--color-primary)]/5'
                    : 'border-[var(--border-default)] hover:bg-[var(--color-surface-hover)]'
                }`}
              >
                <p className="font-medium">{t(`wb.rcv.ops.photo.${slot}`)}</p>
                <p className="mt-1 text-xs text-[var(--text-muted)]">
                  {on ? t('wb.rcv.ops.photoCaptured') : t('wb.rcv.ops.photoCapture')}
                </p>
              </button>
            );
          })}
        </div>
        <p className="text-xs text-[var(--text-muted)]">{t('wb.rcv.ops.photoNoAiYet')}</p>
      </section>

      <section className="space-y-3">
        <h3 className="text-sm font-semibold tracking-tight">{t('wb.rcv.ops.docSection')}</h3>
        <p className="text-xs text-[var(--text-muted)]">{t('wb.rcv.ops.docHint')}</p>
        <div className="flex flex-wrap gap-2">
          {DOC_KINDS.map((kind) => {
            const on = docs.includes(kind);
            return (
              <Button
                key={kind}
                type="button"
                size="sm"
                variant={on ? 'default' : 'secondary'}
                disabled={disabled}
                onClick={() => onToggleDoc(kind)}
              >
                {t(`wb.rcv.ops.docKind.${kind}`)}
                {on ? ` · ${t('wb.rcv.attached')}` : ''}
              </Button>
            );
          })}
        </div>
        {docs.length === 0 ? (
          <p className="text-sm text-[var(--text-muted)]">{t('wb.rcv.ops.docEmpty')}</p>
        ) : (
          <ul className="space-y-1 text-sm">
            {docs.map((d) => (
              <li key={d} className="text-[var(--text-primary)]">
                {t(`wb.rcv.ops.docKind.${d}`)} — {t('wb.rcv.attached')}
              </li>
            ))}
          </ul>
        )}
        <p className="text-xs text-[var(--text-muted)]">{t('wb.rcv.ops.noCompareYet')}</p>
      </section>
    </div>
  );
}

export { DOC_KINDS, PHOTO_SLOTS };

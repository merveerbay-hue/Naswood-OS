import { Link, useNavigate } from '@tanstack/react-router';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { useMemo, useState } from 'react';
import { Button, Card, CardContent, CardDescription, CardHeader, CardTitle, Input } from '@naswood/ui';
import { createResource } from '@/api/business';
import { useI18n } from '@/i18n';

type StageId =
  | 'truck'
  | 'documents'
  | 'ocr'
  | 'verify'
  | 'count'
  | 'inspect'
  | 'warehouse'
  | 'labels'
  | 'review'
  | 'post';

interface StageDef {
  id: StageId;
  titleKey: string;
  hintKey: string;
}

const STAGES: StageDef[] = [
  { id: 'truck', titleKey: 'wb.rcv.truck', hintKey: 'wb.rcv.truckHint' },
  { id: 'documents', titleKey: 'wb.rcv.documents', hintKey: 'wb.rcv.documentsHint' },
  { id: 'ocr', titleKey: 'wb.rcv.ocr', hintKey: 'wb.rcv.ocrHint' },
  { id: 'verify', titleKey: 'wb.rcv.verify', hintKey: 'wb.rcv.verifyHint' },
  { id: 'count', titleKey: 'wb.rcv.count', hintKey: 'wb.rcv.countHint' },
  { id: 'inspect', titleKey: 'wb.rcv.inspect', hintKey: 'wb.rcv.inspectHint' },
  { id: 'warehouse', titleKey: 'wb.rcv.warehouse', hintKey: 'wb.rcv.warehouseHint' },
  { id: 'labels', titleKey: 'wb.rcv.labels', hintKey: 'wb.rcv.labelsHint' },
  { id: 'review', titleKey: 'wb.rcv.review', hintKey: 'wb.rcv.reviewHint' },
  { id: 'post', titleKey: 'wb.rcv.post', hintKey: 'wb.rcv.postHint' },
];

const PHOTO_SLOTS = ['front', 'rear', 'side', 'cargo', 'seal'] as const;
const DAMAGE_FLAGS = ['broken', 'wet', 'blueStain', 'crack', 'mold', 'damage'] as const;

const OCR_DEMO = {
  material: 'Thermowood Deck 26×140×3000',
  dimensions: '26 × 140 × 3000 mm',
  quantity: '48',
  unit: 'adet',
  bundles: '4',
  supplier: 'Nordic Timber Oy',
};

const VERIFY_ROWS = [
  { key: 'mat', po: 'Thermowood Deck 26×140×3000', dn: 'Thermowood Deck 26×140×3000', ocr: 'Thermowood Deck 26×140×3000', status: 'ok' as const },
  { key: 'qty', po: '50 adet', dn: '48 adet', ocr: '48 adet', status: 'qtyDiff' as const },
  { key: 'dim', po: '26×140×3000', dn: '26×140×3000', ocr: '26×140×3000', status: 'ok' as const },
  { key: 'extra', po: '—', dn: 'Ambalaj bandı ×2', ocr: '—', status: 'extra' as const },
];

function mintPreview(prefix: string) {
  const seq = new Date().toISOString().slice(2, 10).replace(/-/g, '') + '-' + String(Math.floor(100000 + Math.random() * 900000));
  return `${prefix}-${seq}`;
}

/** INV-RCV-001 Receiving Workbench — not a Create/CRUD form. */
export function ReceivingWorkbench() {
  const { t } = useI18n();
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const [stageIdx, setStageIdx] = useState(0);
  const [maxReached, setMaxReached] = useState(0);
  const [posted, setPosted] = useState(false);
  const [minted, setMinted] = useState<{ gr?: string; lot?: string; pkg?: string; pallet?: string }>({});
  const [error, setError] = useState<string | null>(null);
  const [approved, setApproved] = useState(false);

  const [truck, setTruck] = useState({
    plate: '',
    trailer: '',
    driver: '',
    supplier: 'Nordic Timber Oy',
    arrivalDate: new Date().toISOString().slice(0, 10),
    arrivalTime: new Date().toTimeString().slice(0, 5),
    gate: '2',
  });
  const [photos, setPhotos] = useState<Record<string, boolean>>({});
  const [docs, setDocs] = useState<string[]>([]);
  const [ocrAccepted, setOcrAccepted] = useState(false);
  const [verifyResolved, setVerifyResolved] = useState(false);
  const [countQty, setCountQty] = useState('48');
  const [countMode, setCountMode] = useState<'scan' | 'sheet' | 'photo'>('scan');
  const [flags, setFlags] = useState<Record<string, boolean>>({});
  const [inspectOk, setInspectOk] = useState(true);
  const [warehouse, setWarehouse] = useState('Ana Hammadde Deposu');
  const [location, setLocation] = useState('Rampa A / Bölge 1');
  const [labelsPrinted, setLabelsPrinted] = useState(false);

  const stage = STAGES[stageIdx];
  const progress = Math.round(((stageIdx + (posted ? 1 : 0)) / STAGES.length) * 100);

  const canAdvance = useMemo(() => {
    switch (stage.id) {
      case 'truck':
        return Boolean(truck.plate.trim() && truck.supplier.trim());
      case 'documents':
        return docs.length > 0;
      case 'ocr':
        return ocrAccepted;
      case 'verify':
        return verifyResolved;
      case 'count':
        return Number(countQty) > 0;
      case 'inspect':
        return true;
      case 'warehouse':
        return Boolean(warehouse.trim() && location.trim());
      case 'labels':
        return Boolean(minted.lot) || labelsPrinted;
      case 'review':
        return approved;
      case 'post':
        return !posted;
      default:
        return true;
    }
  }, [stage.id, truck, docs, ocrAccepted, verifyResolved, countQty, warehouse, location, minted.lot, labelsPrinted, approved, posted]);

  const persistMutation = useMutation({
    mutationFn: async () => {
      const whCode = warehouse.toLowerCase().includes('hammadde') || warehouse.toLowerCase().includes('ana') ? 'WH-RM' : 'WH-FG';
      return createResource<Record<string, unknown>>('goods-receipts', {
        warehouseCode: whCode,
        reference: truck.plate || 'MANUAL',
        status: 'Posted',
        notes: [
          `supplier=${truck.supplier}`,
          `gate=${truck.gate}`,
          `qty=${countQty}`,
          `loc=${location}`,
          `material=${OCR_DEMO.material}`,
          `inspect=${inspectOk ? 'OK' : 'HOLD'}`,
          `flags=${DAMAGE_FLAGS.filter((f) => flags[f]).join(',') || 'none'}`,
          `docs=${docs.length}`,
          `photos=${Object.values(photos).filter(Boolean).length}`,
        ].join('; '),
        number: '',
      });
    },
    onSuccess: async (created) => {
      setError(null);
      const gr =
        (typeof created.number === 'string' && created.number) ||
        (typeof created.code === 'string' && created.code) ||
        mintPreview('GR');
      setMinted((m) => ({
        ...m,
        gr,
        lot: m.lot ?? mintPreview('LOT'),
        pkg: m.pkg ?? mintPreview('PKG'),
        pallet: m.pallet ?? mintPreview('PAL'),
      }));
      setPosted(true);
      await queryClient.invalidateQueries({ queryKey: ['business', 'goods-receipts'] });
    },
    onError: (e: Error) => setError(e.message),
  });

  function goNext() {
    if (stage.id === 'labels' && !minted.lot) {
      setMinted({
        lot: mintPreview('LOT'),
        pkg: mintPreview('PKG'),
        pallet: mintPreview('PAL'),
      });
      setLabelsPrinted(true);
    }
    if (stage.id === 'post') {
      persistMutation.mutate();
      return;
    }
    const next = Math.min(stageIdx + 1, STAGES.length - 1);
    setStageIdx(next);
    setMaxReached((m) => Math.max(m, next));
  }

  function toggleFlag(key: string) {
    setFlags((f) => ({ ...f, [key]: !f[key] }));
    if (key !== 'ok') setInspectOk(false);
  }

  function addDemoDoc(kind: string) {
    setDocs((d) => (d.includes(kind) ? d : [...d, kind]));
  }

  return (
    <div className="flex min-h-[calc(100vh-7rem)] flex-col gap-3">
      {/* Header */}
      <div className="flex flex-wrap items-start justify-between gap-3 border-b border-[var(--border-default)] pb-3">
        <div>
          <p className="text-xs font-medium text-[var(--text-muted)]">INV-RCV-001 · {t('wb.rcv.screenType')}</p>
          <h2 className="text-xl font-semibold tracking-tight">{t('wb.rcv.title')}</h2>
          <p className="mt-1 max-w-2xl text-sm text-[var(--text-secondary)]">{t('wb.rcv.desc')}</p>
          <p className="mt-2 text-xs text-[var(--text-muted)]">
            {truck.plate ? `${t('wb.rcv.truckPlate')}: ${truck.plate}` : t('wb.rcv.noTruckYet')}
            {truck.supplier ? ` · ${truck.supplier}` : ''}
            {truck.gate ? ` · ${t('wb.rcv.gate')} ${truck.gate}` : ''}
            {' · '}
            {t('wb.rcv.stageOf').replace('{n}', String(stageIdx + 1)).replace('{total}', String(STAGES.length))}
          </p>
        </div>
        <div className="flex flex-col items-end gap-2">
          <div className="rounded-md border border-[var(--border-default)] bg-[var(--color-surface)] px-3 py-2 text-right">
            <p className="text-[10px] uppercase tracking-wide text-[var(--text-muted)]">{t('wizard.systemCode')}</p>
            <p className="font-mono text-sm font-medium">{minted.gr ?? t('wizard.autoGenerated')}</p>
            <p className="text-[10px] text-[var(--text-muted)]">GR-… · LOT-… · PKG-…</p>
          </div>
          <Link
            to="/inventory/operations/goods-receipts"
            className="text-sm font-medium text-[var(--color-primary)] hover:underline"
          >
            {t('wizard.backToLibrary')}
          </Link>
        </div>
      </div>

      {/* Progress */}
      <div className="h-1.5 w-full overflow-hidden rounded-full bg-[var(--color-surface-hover)]">
        <div
          className="h-full rounded-full bg-[var(--color-primary)] transition-all duration-300"
          style={{ width: `${posted ? 100 : progress}%` }}
        />
      </div>

      <div className="grid flex-1 gap-3 lg:grid-cols-[220px_minmax(0,1fr)_260px]">
        {/* Stage rail */}
        <nav className="rounded-lg border border-[var(--border-default)] bg-[var(--color-surface)] p-2">
          <p className="mb-2 px-2 text-[10px] font-semibold uppercase tracking-wide text-[var(--text-muted)]">
            {t('wb.rcv.timeline')}
          </p>
          <ol className="space-y-0.5">
            {STAGES.map((s, i) => {
              const locked = i > maxReached && i !== stageIdx;
              const done = i < stageIdx || (posted && i <= stageIdx);
              const active = i === stageIdx;
              return (
                <li key={s.id}>
                  <button
                    type="button"
                    disabled={locked}
                    onClick={() => !locked && setStageIdx(i)}
                    className={`flex w-full items-center gap-2 rounded-md px-2 py-1.5 text-left text-xs transition-colors ${
                      active
                        ? 'bg-[var(--color-primary)] text-white'
                        : done
                          ? 'bg-[var(--color-surface-hover)] text-[var(--text-primary)]'
                          : locked
                            ? 'cursor-not-allowed text-[var(--text-muted)] opacity-50'
                            : 'text-[var(--text-secondary)] hover:bg-[var(--color-surface-hover)]'
                    }`}
                  >
                    <span className="font-mono tabular-nums opacity-80">{i + 1}</span>
                    <span className="leading-snug">{t(s.titleKey)}</span>
                  </button>
                </li>
              );
            })}
          </ol>
        </nav>

        {/* Main surface */}
        <main className="min-w-0 space-y-3">
          <Card>
            <CardHeader>
              <CardTitle>
                {stageIdx + 1}. {t(stage.titleKey)}
              </CardTitle>
              <CardDescription>{t(stage.hintKey)}</CardDescription>
            </CardHeader>
            <CardContent className="space-y-4">
              {stage.id === 'truck' ? (
                <div className="space-y-4">
                  <div className="grid gap-3 md:grid-cols-2">
                    {(
                      [
                        ['plate', 'wb.rcv.truckPlate'],
                        ['trailer', 'wb.rcv.trailer'],
                        ['driver', 'wb.rcv.driver'],
                        ['supplier', 'wb.rcv.supplier'],
                        ['arrivalDate', 'wb.rcv.arrivalDate'],
                        ['arrivalTime', 'wb.rcv.arrivalTime'],
                        ['gate', 'wb.rcv.gate'],
                      ] as const
                    ).map(([key, label]) => (
                      <label key={key} className="space-y-1 text-sm">
                        <span className="text-[var(--text-secondary)]">{t(label)}</span>
                        <Input
                          type={key === 'arrivalDate' ? 'date' : key === 'arrivalTime' ? 'time' : 'text'}
                          value={truck[key]}
                          onChange={(e) => setTruck((prev) => ({ ...prev, [key]: e.target.value }))}
                          placeholder={key === 'plate' ? '34 ABC 123' : undefined}
                        />
                      </label>
                    ))}
                  </div>
                  <div>
                    <p className="mb-2 text-sm text-[var(--text-secondary)]">{t('wb.rcv.truckPhotos')}</p>
                    <div className="flex flex-wrap gap-2">
                      {PHOTO_SLOTS.map((slot) => (
                        <button
                          key={slot}
                          type="button"
                          onClick={() => setPhotos((p) => ({ ...p, [slot]: !p[slot] }))}
                          className={`rounded-md border px-3 py-2 text-xs font-medium ${
                            photos[slot]
                              ? 'border-[var(--color-primary)] bg-[var(--color-primary)]/10 text-[var(--color-primary)]'
                              : 'border-dashed border-[var(--border-default)] text-[var(--text-muted)]'
                          }`}
                        >
                          {photos[slot] ? '✓ ' : '+ '}
                          {t(`wb.rcv.photo.${slot}`)}
                        </button>
                      ))}
                    </div>
                    <p className="mt-2 text-xs text-[var(--text-muted)]">{t('wb.rcv.cameraPrefer')}</p>
                  </div>
                </div>
              ) : null}

              {stage.id === 'documents' ? (
                <div className="space-y-3">
                  <p className="text-sm text-[var(--text-secondary)]">{t('wb.rcv.docAttach')}</p>
                  <div className="flex flex-wrap gap-2">
                    {['deliveryNote', 'packingList', 'purchaseOrder', 'excel', 'photo'].map((kind) => (
                      <Button key={kind} type="button" variant="secondary" onClick={() => addDemoDoc(kind)}>
                        {docs.includes(kind) ? '✓ ' : '+ '}
                        {t(`wb.rcv.doc.${kind}`)}
                      </Button>
                    ))}
                  </div>
                  <div className="rounded-md border border-[var(--border-default)] bg-[var(--color-surface-hover)]/40 p-4">
                    <p className="text-xs font-semibold uppercase tracking-wide text-[var(--text-muted)]">
                      {t('wb.rcv.docViewer')}
                    </p>
                    {docs.length === 0 ? (
                      <p className="mt-2 text-sm text-[var(--text-muted)]">{t('wb.rcv.docEmpty')}</p>
                    ) : (
                      <ul className="mt-2 space-y-1 text-sm">
                        {docs.map((d) => (
                          <li key={d} className="font-medium text-[var(--text-primary)]">
                            {t(`wb.rcv.doc.${d}`)} — {t('wb.rcv.attached')}
                          </li>
                        ))}
                      </ul>
                    )}
                  </div>
                </div>
              ) : null}

              {stage.id === 'ocr' ? (
                <div className="space-y-3">
                  <p className="text-sm text-[var(--text-secondary)]">{t('wb.rcv.ocrIntro')}</p>
                  <div className="grid gap-2 md:grid-cols-2">
                    {Object.entries(OCR_DEMO).map(([k, v]) => (
                      <div key={k} className="rounded-md border border-[var(--border-default)] px-3 py-2">
                        <p className="text-[10px] uppercase text-[var(--text-muted)]">{t(`wb.rcv.ocrField.${k}`)}</p>
                        <p className="font-medium">{v}</p>
                        <p className="text-[10px] text-[var(--color-primary)]">{t('wb.rcv.ocrConfidence')} 92%</p>
                      </div>
                    ))}
                  </div>
                  <label className="flex items-center gap-2 text-sm">
                    <input type="checkbox" checked={ocrAccepted} onChange={(e) => setOcrAccepted(e.target.checked)} />
                    {t('wb.rcv.ocrAccept')}
                  </label>
                </div>
              ) : null}

              {stage.id === 'verify' ? (
                <div className="space-y-3">
                  <p className="text-sm text-[var(--text-secondary)]">{t('wb.rcv.verifyIntro')}</p>
                  <div className="overflow-x-auto rounded-md border border-[var(--border-default)]">
                    <table className="w-full min-w-[640px] text-left text-xs">
                      <thead className="bg-[var(--color-surface-hover)] text-[var(--text-muted)]">
                        <tr>
                          <th className="px-3 py-2">{t('wb.rcv.colField')}</th>
                          <th className="px-3 py-2">PO</th>
                          <th className="px-3 py-2">{t('wb.rcv.deliveryNote')}</th>
                          <th className="px-3 py-2">OCR</th>
                          <th className="px-3 py-2">{t('wb.rcv.diff')}</th>
                        </tr>
                      </thead>
                      <tbody>
                        {VERIFY_ROWS.map((row) => (
                          <tr key={row.key} className="border-t border-[var(--border-default)]">
                            <td className="px-3 py-2 font-medium">{t(`wb.rcv.verifyRow.${row.key}`)}</td>
                            <td className="px-3 py-2">{row.po}</td>
                            <td className="px-3 py-2">{row.dn}</td>
                            <td className="px-3 py-2">{row.ocr}</td>
                            <td className="px-3 py-2">
                              <span
                                className={
                                  row.status === 'ok'
                                    ? 'text-[var(--color-success,var(--color-primary))]'
                                    : 'font-semibold text-[var(--color-danger)]'
                                }
                              >
                                {t(`wb.rcv.status.${row.status}`)}
                              </span>
                            </td>
                          </tr>
                        ))}
                      </tbody>
                    </table>
                  </div>
                  <label className="flex items-center gap-2 text-sm">
                    <input
                      type="checkbox"
                      checked={verifyResolved}
                      onChange={(e) => setVerifyResolved(e.target.checked)}
                    />
                    {t('wb.rcv.verifyResolve')}
                  </label>
                </div>
              ) : null}

              {stage.id === 'count' ? (
                <div className="space-y-3">
                  <p className="text-sm text-[var(--text-secondary)]">{t('wb.rcv.countIntro')}</p>
                  <div className="flex flex-wrap gap-2">
                    {(['scan', 'sheet', 'photo'] as const).map((m) => (
                      <Button
                        key={m}
                        type="button"
                        variant={countMode === m ? 'default' : 'secondary'}
                        onClick={() => setCountMode(m)}
                      >
                        {t(`wb.rcv.countMode.${m}`)}
                      </Button>
                    ))}
                  </div>
                  <label className="block max-w-xs space-y-1 text-sm">
                    <span className="text-[var(--text-secondary)]">{t('wb.rcv.countedQty')}</span>
                    <Input type="number" value={countQty} onChange={(e) => setCountQty(e.target.value)} />
                  </label>
                  <p className="text-xs text-[var(--text-muted)]">{t('wb.rcv.countPrefer')}</p>
                </div>
              ) : null}

              {stage.id === 'inspect' ? (
                <div className="space-y-3">
                  <label className="flex items-center gap-2 text-sm font-medium">
                    <input
                      type="checkbox"
                      checked={inspectOk}
                      onChange={(e) => {
                        setInspectOk(e.target.checked);
                        if (e.target.checked) setFlags({});
                      }}
                    />
                    {t('wb.rcv.visualOk')}
                  </label>
                  <div className="flex flex-wrap gap-2">
                    {DAMAGE_FLAGS.map((f) => (
                      <button
                        key={f}
                        type="button"
                        onClick={() => toggleFlag(f)}
                        className={`rounded-md border px-3 py-1.5 text-xs font-medium ${
                          flags[f]
                            ? 'border-[var(--color-danger)] bg-[var(--color-danger)]/10 text-[var(--color-danger)]'
                            : 'border-[var(--border-default)] text-[var(--text-secondary)]'
                        }`}
                      >
                        {t(`wb.rcv.flag.${f}`)}
                      </button>
                    ))}
                  </div>
                  <Button
                    type="button"
                    variant="secondary"
                    onClick={() => setPhotos((p) => ({ ...p, damage: !p.damage }))}
                  >
                    {photos.damage ? '✓ ' : '+ '}
                    {t('wb.rcv.addDamagePhoto')}
                  </Button>
                </div>
              ) : null}

              {stage.id === 'warehouse' ? (
                <div className="space-y-3">
                  <div className="rounded-md border border-[var(--color-primary)]/30 bg-[var(--color-primary)]/5 px-3 py-2 text-sm">
                    <p className="text-xs font-semibold uppercase text-[var(--text-muted)]">{t('wb.rcv.suggestion')}</p>
                    <p>
                      {t('wb.rcv.suggestedWh')}: <strong>Ana Hammadde Deposu</strong> · Zone A · Rampa A / Bölge 1
                    </p>
                    <p className="text-xs text-[var(--text-muted)]">{t('wb.rcv.suggestionWhy')}</p>
                  </div>
                  <div className="grid gap-3 md:grid-cols-2">
                    <label className="space-y-1 text-sm">
                      <span className="text-[var(--text-secondary)]">{t('wb.rcv.whPick')}</span>
                      <Input value={warehouse} onChange={(e) => setWarehouse(e.target.value)} placeholder={t('wizard.nameFirstHint')} />
                    </label>
                    <label className="space-y-1 text-sm">
                      <span className="text-[var(--text-secondary)]">{t('wb.rcv.locPick')}</span>
                      <Input value={location} onChange={(e) => setLocation(e.target.value)} />
                    </label>
                  </div>
                  <p className="text-xs text-[var(--text-muted)]">{t('wb.rcv.noCodeInput')}</p>
                </div>
              ) : null}

              {stage.id === 'labels' ? (
                <div className="space-y-3">
                  <p className="text-sm text-[var(--text-secondary)]">{t('wb.rcv.labelsIntro')}</p>
                  <div className="grid gap-2 sm:grid-cols-3">
                    {[
                      ['lot', minted.lot ?? 'LOT-…'],
                      ['pkg', minted.pkg ?? 'PKG-…'],
                      ['pallet', minted.pallet ?? 'PAL-…'],
                    ].map(([k, v]) => (
                      <div key={k} className="rounded-md border border-[var(--border-default)] px-3 py-3 text-center">
                        <p className="text-[10px] uppercase text-[var(--text-muted)]">{t(`wb.rcv.id.${k}`)}</p>
                        <p className="font-mono text-sm font-semibold">{v}</p>
                        <p className="mt-1 text-[10px] text-[var(--text-muted)]">QR · Barcode</p>
                      </div>
                    ))}
                  </div>
                  <Button
                    type="button"
                    variant="secondary"
                    onClick={() => {
                      setMinted({
                        lot: mintPreview('LOT'),
                        pkg: mintPreview('PKG'),
                        pallet: mintPreview('PAL'),
                      });
                      setLabelsPrinted(true);
                    }}
                  >
                    {t('wb.rcv.mintAndPrint')}
                  </Button>
                </div>
              ) : null}

              {stage.id === 'review' ? (
                <div className="space-y-3">
                  <div className="grid gap-2 md:grid-cols-2">
                    {[
                      [t('wb.rcv.truck'), `${truck.plate || '—'} · ${truck.supplier} · Gate ${truck.gate}`],
                      [t('wb.rcv.documents'), `${docs.length} ${t('wb.rcv.files')}`],
                      [t('wb.rcv.materials'), `${OCR_DEMO.material} · ${countQty} ${OCR_DEMO.unit}`],
                      [t('wb.rcv.diff'), verifyResolved ? t('wb.rcv.resolved') : t('wb.rcv.openDiffs')],
                      [t('wb.rcv.inspect'), inspectOk ? t('wb.rcv.visualOk') : DAMAGE_FLAGS.filter((f) => flags[f]).map((f) => t(`wb.rcv.flag.${f}`)).join(', ')],
                      [t('wb.rcv.warehouse'), `${warehouse} · ${location}`],
                      [t('wb.rcv.labels'), minted.lot ? `${minted.lot} / ${minted.pkg}` : t('wizard.autoGenerated')],
                    ].map(([label, value]) => (
                      <div key={label} className="rounded-md border border-[var(--border-default)] px-3 py-2">
                        <p className="text-[10px] uppercase text-[var(--text-muted)]">{label}</p>
                        <p className="text-sm font-medium">{value}</p>
                      </div>
                    ))}
                  </div>
                  <label className="flex items-center gap-2 text-sm font-medium">
                    <input type="checkbox" checked={approved} onChange={(e) => setApproved(e.target.checked)} />
                    {t('wb.rcv.approvePost')}
                  </label>
                </div>
              ) : null}

              {stage.id === 'post' ? (
                <div className="space-y-3">
                  <p className="text-sm text-[var(--text-secondary)]">{t('wb.rcv.postIntro')}</p>
                  <ul className="list-inside list-disc text-sm text-[var(--text-primary)]">
                    <li>{t('wb.rcv.postItem.txn')}</li>
                    <li>{t('wb.rcv.postItem.receiving')}</li>
                    <li>{t('wb.rcv.postItem.stock')}</li>
                    <li>{t('wb.rcv.postItem.audit')}</li>
                    <li>{t('wb.rcv.postItem.attachments')}</li>
                  </ul>
                  {posted ? (
                    <p className="text-sm font-medium text-[var(--color-primary)]">
                      {t('wb.rcv.postedBanner')}
                      {minted.gr ? ` · ${minted.gr}` : ''}
                      {minted.lot ? ` · ${minted.lot}` : ''}
                      {' — '}
                      {t('wizard.inLibrary')}
                    </p>
                  ) : null}
                </div>
              ) : null}

              {error ? <p className="text-sm text-[var(--color-danger)]">{error}</p> : null}
            </CardContent>
          </Card>
        </main>

        {/* Context panel */}
        <aside className="space-y-3">
          <div className="rounded-lg border border-[var(--border-default)] bg-[var(--color-surface)] p-3">
            <p className="text-[10px] font-semibold uppercase tracking-wide text-[var(--text-muted)]">{t('wb.rcv.context')}</p>
            <dl className="mt-2 space-y-2 text-xs">
              <div>
                <dt className="text-[var(--text-muted)]">PO</dt>
                <dd className="font-medium">PO-2026-0142</dd>
              </div>
              <div>
                <dt className="text-[var(--text-muted)]">{t('wb.rcv.supplier')}</dt>
                <dd className="font-medium">{truck.supplier || '—'}</dd>
              </div>
              <div>
                <dt className="text-[var(--text-muted)]">{t('wb.rcv.openDiffs')}</dt>
                <dd className="font-medium text-[var(--color-danger)]">{verifyResolved ? '0' : '2'}</dd>
              </div>
              <div>
                <dt className="text-[var(--text-muted)]">{t('wb.rcv.suggestion')}</dt>
                <dd className="font-medium">Ana Hammadde · Zone A</dd>
              </div>
              <div>
                <dt className="text-[var(--text-muted)]">{t('wb.rcv.attachments')}</dt>
                <dd className="font-medium">
                  {docs.length} {t('wb.rcv.files')} · {Object.values(photos).filter(Boolean).length} {t('wb.rcv.photos')}
                </dd>
              </div>
            </dl>
          </div>
          <div className="rounded-lg border border-dashed border-[var(--border-default)] p-3 text-xs text-[var(--text-muted)]">
            {t('wb.rcv.noManualIds')}
          </div>
        </aside>
      </div>

      {/* Sticky action bar */}
      <div className="sticky bottom-0 z-10 -mx-1 flex flex-wrap items-center justify-between gap-2 border-t border-[var(--border-default)] bg-[var(--color-bg,var(--color-surface))] px-2 py-3">
        <div className="flex flex-wrap gap-2">
          <Button type="button" variant="secondary" disabled={stageIdx === 0 || posted} onClick={() => setStageIdx((s) => s - 1)}>
            {t('wizard.back')}
          </Button>
          <Button type="button" variant="secondary" disabled={posted}>
            {t('wb.rcv.saveDraft')}
          </Button>
        </div>
        <div className="flex flex-wrap gap-2">
          {stage.id !== 'post' ? (
            <Button type="button" disabled={!canAdvance || posted} onClick={goNext}>
              {t('wb.rcv.nextStage')}
            </Button>
          ) : (
            <>
              <Button
                type="button"
                disabled={posted || persistMutation.isPending || !approved}
                onClick={() => persistMutation.mutate()}
              >
                {persistMutation.isPending ? t('saving') : t('wizard.post')}
              </Button>
              {posted ? (
                <Button type="button" variant="secondary" onClick={() => void navigate({ to: '/inventory/operations/goods-receipts' })}>
                  {t('wizard.backToLibrary')}
                </Button>
              ) : null}
            </>
          )}
        </div>
      </div>
    </div>
  );
}

import { Link, useNavigate } from '@tanstack/react-router';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { useMemo, useState } from 'react';
import { Button, Card, CardContent, CardDescription, CardHeader, CardTitle, Input } from '@naswood/ui';
import { executeStockDocument, searchResource } from '@/api/business';
import { useI18n } from '@/i18n';
import {
  formatDims,
  parseDefinitionDims,
  parseDimensions,
  rankMaterialMatches,
  type MaterialCandidate,
  type MaterialMatchResult,
} from './materialMatch';

/**
 * Phase-1 receiving pipeline (no real OCR engine yet):
 * İrsaliye → OCR (demo extract) → Kontrol → Malzeme eşleştirme → Miktar doğrulama → Onay → Stok
 *
 * Demo OCR is for operator control only. Stock posts only matched master code + verified qty
 * with extractSource=manual (never extractSource=demo).
 */
type StageId = 'deliveryNote' | 'ocr' | 'control' | 'materialMatch' | 'qtyVerify' | 'approve' | 'stock';

interface StageDef {
  id: StageId;
  titleKey: string;
  hintKey: string;
}

const STAGES: StageDef[] = [
  { id: 'deliveryNote', titleKey: 'wb.rcv.step.deliveryNote', hintKey: 'wb.rcv.step.deliveryNoteHint' },
  { id: 'ocr', titleKey: 'wb.rcv.step.ocr', hintKey: 'wb.rcv.step.ocrHint' },
  { id: 'control', titleKey: 'wb.rcv.step.control', hintKey: 'wb.rcv.step.controlHint' },
  { id: 'materialMatch', titleKey: 'wb.rcv.step.materialMatch', hintKey: 'wb.rcv.step.materialMatchHint' },
  { id: 'qtyVerify', titleKey: 'wb.rcv.step.qtyVerify', hintKey: 'wb.rcv.step.qtyVerifyHint' },
  { id: 'approve', titleKey: 'wb.rcv.step.approve', hintKey: 'wb.rcv.step.approveHint' },
  { id: 'stock', titleKey: 'wb.rcv.step.stock', hintKey: 'wb.rcv.step.stockHint' },
];

type OcrFieldKey = 'material' | 'dimensions' | 'quantity' | 'unit' | 'bundles' | 'supplier';

const OCR_FIELD_KEYS: OcrFieldKey[] = ['material', 'dimensions', 'quantity', 'unit', 'bundles', 'supplier'];

/** Placeholder extract — not a real OCR engine. */
const OCR_DEMO_INITIAL: Record<OcrFieldKey, { value: string; confidence: number; flagged: boolean }> = {
  material: { value: 'Thermowood Deck 26×140×3000', confidence: 94, flagged: false },
  dimensions: { value: '26 × 14O × 3000 mm', confidence: 61, flagged: true },
  quantity: { value: '4B', confidence: 58, flagged: true },
  unit: { value: 'adet', confidence: 96, flagged: false },
  bundles: { value: '4', confidence: 91, flagged: false },
  supplier: { value: 'Nordlc Timber Oy', confidence: 72, flagged: true },
};

const OCR_DEMO_CORRECTED: Record<OcrFieldKey, string> = {
  material: 'Thermowood Deck 26×140×3000',
  dimensions: '26 × 140 × 3000 mm',
  quantity: '48',
  unit: 'adet',
  bundles: '4',
  supplier: 'Nordic Timber Oy',
};

function mintPreview(prefix: string) {
  const seq = new Date().toISOString().slice(2, 10).replace(/-/g, '') + '-' + String(Math.floor(100000 + Math.random() * 900000));
  return `${prefix}-${seq}`;
}

/** INV-RCV-001 — phase-1 safe stock path (demo OCR → operator confirm → ledger). */
export function ReceivingWorkbench() {
  const { t } = useI18n();
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const [stageIdx, setStageIdx] = useState(0);
  const [maxReached, setMaxReached] = useState(0);
  const [posted, setPosted] = useState(false);
  const [minted, setMinted] = useState<{ gr?: string; lot?: string; pkg?: string; mi?: string }>({});
  const [error, setError] = useState<string | null>(null);
  const [approved, setApproved] = useState(false);

  const [truck, setTruck] = useState({
    plate: '34 ABC 123',
    supplier: 'Nordic Timber Oy',
    gate: '2',
  });
  const [docs, setDocs] = useState<string[]>([]);
  const [ocrFields, setOcrFields] = useState(OCR_DEMO_INITIAL);
  const [ocrEditing, setOcrEditing] = useState(false);
  const [ocrAccepted, setOcrAccepted] = useState(false);
  const [controlAccepted, setControlAccepted] = useState(false);
  const [countQty, setCountQty] = useState('48');
  const [quantityVerified, setQuantityVerified] = useState(false);
  const [matchedMaterialCode, setMatchedMaterialCode] = useState('');
  const [matchedMaterialId, setMatchedMaterialId] = useState('');
  const [matchConfirmed, setMatchConfirmed] = useState(false);
  const [confirmedMatchScore, setConfirmedMatchScore] = useState<number | null>(null);
  const [showMaterialPicker, setShowMaterialPicker] = useState(false);
  const [materialSearch, setMaterialSearch] = useState('');
  const [warehouse, setWarehouse] = useState('Ana Hammadde Deposu');
  const [location, setLocation] = useState('Rampa A / Bölge 1');

  const ocrFlaggedCount = OCR_FIELD_KEYS.filter((k) => ocrFields[k].flagged).length;
  const ocrConfidence = ocrFields.material.confidence;

  const materialsQuery = useQuery({
    queryKey: ['business', 'materials', 'receiving-match'],
    queryFn: () =>
      searchResource<{
        id: string;
        code: string;
        name: string;
        description?: string;
        category?: string;
        unitOfMeasure?: string;
        status: string;
        definitionJson?: string;
      }>('materials'),
  });

  const materialCandidates: MaterialCandidate[] = useMemo(
    () =>
      (materialsQuery.data?.items ?? [])
        .filter((m) => !m.status || m.status.toLowerCase() === 'active')
        .map((m) => ({
          id: String(m.id),
          code: m.code,
          name: m.name,
          description: m.description,
          category: m.category,
          unitOfMeasure: m.unitOfMeasure,
          status: m.status,
          definitionJson: m.definitionJson,
        })),
    [materialsQuery.data],
  );

  const rankedMatches = useMemo(
    () =>
      rankMaterialMatches(ocrFields.material.value, materialCandidates, ocrFields.dimensions.value, 8),
    [ocrFields.material.value, ocrFields.dimensions.value, materialCandidates],
  );

  const suggestedMatch: MaterialMatchResult | null = rankedMatches[0] ?? null;

  const pickerList = useMemo(() => {
    const q = materialSearch.trim().toLowerCase();
    if (!q) return materialCandidates;
    return materialCandidates.filter(
      (m) => m.code.toLowerCase().includes(q) || m.name.toLowerCase().includes(q),
    );
  }, [materialCandidates, materialSearch]);

  const stage = STAGES[stageIdx];
  const progress = Math.round(((stageIdx + (posted ? 1 : 0)) / STAGES.length) * 100);

  /** Stock posts operator-confirmed master material only — never raw OCR text. */
  const postBlockedReason = useMemo(() => {
    if (!matchConfirmed || !matchedMaterialCode.trim() || !matchedMaterialId.trim()) {
      return t('wb.rcv.gateNeedMaterialConfirm');
    }
    if (!quantityVerified) return t('wb.rcv.gateNeedQtyVerify');
    if (Number(countQty) <= 0) return t('wb.rcv.gateNeedCount');
    if (!approved) return t('wb.rcv.gateNeedApprove');
    if (!warehouse.trim() || !location.trim()) return t('wb.rcv.gateNeedWh');
    return null;
  }, [
    matchConfirmed,
    matchedMaterialCode,
    matchedMaterialId,
    quantityVerified,
    countQty,
    approved,
    warehouse,
    location,
    t,
  ]);

  const gateMessage = useMemo(() => {
    switch (stage.id) {
      case 'deliveryNote':
        if (!truck.plate.trim()) return t('wb.rcv.gateNeedPlate');
        if (!docs.includes('deliveryNote')) return t('wb.rcv.gateNeedDeliveryNote');
        return null;
      case 'ocr':
        if (ocrEditing) return t('wb.rcv.gateNeedOcrSave');
        if (ocrFlaggedCount > 0) return t('wb.rcv.gateNeedOcrFix');
        return !ocrAccepted ? t('wb.rcv.gateNeedOcr') : null;
      case 'control':
        return !controlAccepted ? t('wb.rcv.gateNeedControl') : null;
      case 'materialMatch':
        if (!matchConfirmed || !matchedMaterialCode.trim()) return t('wb.rcv.gateNeedMaterialConfirm');
        return null;
      case 'qtyVerify':
        if (Number(countQty) <= 0) return t('wb.rcv.gateNeedCount');
        if (!quantityVerified) return t('wb.rcv.gateNeedQtyVerify');
        return null;
      case 'approve':
        if (!warehouse.trim() || !location.trim()) return t('wb.rcv.gateNeedWh');
        return !approved ? t('wb.rcv.gateNeedApprove') : null;
      case 'stock':
        return postBlockedReason;
      default:
        return null;
    }
  }, [
    stage.id,
    truck.plate,
    docs,
    ocrEditing,
    ocrFlaggedCount,
    ocrAccepted,
    controlAccepted,
    matchConfirmed,
    matchedMaterialCode,
    countQty,
    quantityVerified,
    warehouse,
    location,
    approved,
    postBlockedReason,
    t,
  ]);

  const canAdvance = !gateMessage && !posted;

  function clearMaterialMatch() {
    setMatchedMaterialCode('');
    setMatchedMaterialId('');
    setMatchConfirmed(false);
    setConfirmedMatchScore(null);
    setApproved(false);
  }

  function confirmMatch(result: MaterialMatchResult) {
    if (result.status === 'NO_MATCH') return;
    setMatchedMaterialCode(result.material.code);
    setMatchedMaterialId(result.material.id);
    setConfirmedMatchScore(result.score);
    setMatchConfirmed(true);
    setShowMaterialPicker(false);
    setApproved(false);
  }

  function selectMasterMaterial(m: MaterialCandidate) {
    const scored = rankMaterialMatches(ocrFields.material.value, [m], ocrFields.dimensions.value, 1)[0];
    setMatchedMaterialCode(m.code);
    setMatchedMaterialId(m.id);
    setConfirmedMatchScore(scored?.score ?? 0);
    setMatchConfirmed(true);
    setShowMaterialPicker(false);
    setApproved(false);
  }

  const persistMutation = useMutation({
    mutationFn: async () => {
      if (!matchConfirmed || !quantityVerified || !approved) {
        throw new Error(t('wb.rcv.gateNeedMaterialConfirm'));
      }
      const materialCode = matchedMaterialCode.trim();
      const materialId = matchedMaterialId.trim();
      if (!materialCode || !materialId) throw new Error(t('wb.rcv.gateNeedMaterialConfirm'));
      // Hard block: never post OCR free-text as materialCode
      if (materialCode === ocrFields.material.value.trim() && !materialCandidates.some((c) => c.code === materialCode)) {
        throw new Error(t('wb.rcv.gateNeedMaterialConfirm'));
      }
      const qty = Number(countQty) || 0;
      if (qty <= 0) throw new Error(t('wb.rcv.gateNeedCount'));

      const whCode =
        warehouse.toLowerCase().includes('hammadde') || warehouse.toLowerCase().includes('ana') ? 'WH-RM' : 'WH-FG';
      const locCode = location.trim() || 'RECV';
      const mi = mintPreview('MI');
      const lot = mintPreview('LOT');
      const pkg = mintPreview('PKG');

      return executeStockDocument<{
        documentId: string;
        documentNumber: string;
        status: string;
        lines: Array<{
          materialCode: string;
          materialIdentityNumber: string;
          packageNumber: string;
          lotNumber: string;
          movementNumber: string;
          quantity: number;
        }>;
      }>('goods-receipts/execute', {
        warehouseCode: whCode,
        reference: truck.plate || 'MANUAL',
        quantityVerified: true,
        extractSource: 'manual',
        notes: [
          `pipeline=deliveryNote>ocrDemo>control>materialMatch>qtyVerify>approve>stock`,
          `supplier=${truck.supplier}`,
          `gate=${truck.gate}`,
          `qty=${qty}`,
          `loc=${locCode}`,
          `materialId=${materialId}`,
          `material=${materialCode}`,
          `matchScore=${confirmedMatchScore ?? ''}`,
          `ocrConfidence=${ocrConfidence}`,
          `ocrLabel=${ocrFields.material.value}`,
          `docs=${docs.join(',')}`,
        ].join('; '),
        number: '',
        lines: [
          {
            materialCode,
            materialId,
            locationCode: locCode,
            lotNumber: lot,
            packageNumber: pkg,
            materialIdentityNumber: mi,
            quantity: qty,
            unitOfMeasure: 'Piece',
            barcode: pkg,
          },
        ],
      });
    },
    onSuccess: async (created) => {
      setError(null);
      const line = created.lines?.[0];
      setMinted((m) => ({
        ...m,
        gr: created.documentNumber || mintPreview('GR'),
        mi: line?.materialIdentityNumber || m.mi,
        lot: line?.lotNumber || m.lot,
        pkg: line?.packageNumber || m.pkg,
      }));
      setPosted(true);
      await queryClient.invalidateQueries({ queryKey: ['business', 'goods-receipts'] });
      await queryClient.invalidateQueries({ queryKey: ['business', 'inventory'] });
      await queryClient.invalidateQueries({ queryKey: ['business', 'packages'] });
    },
    onError: (e: Error) => setError(e.message),
  });

  function goNext() {
    if (stage.id === 'stock') {
      if (postBlockedReason) {
        setError(postBlockedReason);
        return;
      }
      // Mint identity numbers at post time (system-owned).
      setMinted((m) => ({
        ...m,
        mi: m.mi ?? mintPreview('MI'),
        lot: m.lot ?? mintPreview('LOT'),
        pkg: m.pkg ?? mintPreview('PKG'),
      }));
      persistMutation.mutate();
      return;
    }
    const next = Math.min(stageIdx + 1, STAGES.length - 1);
    setStageIdx(next);
    setMaxReached((m) => Math.max(m, next));
  }

  function addDoc(kind: string) {
    setDocs((d) => (d.includes(kind) ? d : [...d, kind]));
  }

  return (
    <div className="flex min-h-[calc(100vh-7rem)] flex-col gap-3">
      <div className="flex flex-wrap items-start justify-between gap-3 border-b border-[var(--border-default)] pb-3">
        <div>
          <p className="text-xs font-medium text-[var(--text-muted)]">INV-RCV-001 · {t('wb.rcv.screenType')}</p>
          <h2 className="text-xl font-semibold tracking-tight">{t('wb.rcv.title')}</h2>
          <p className="mt-1 max-w-2xl text-sm text-[var(--text-secondary)]">{t('wb.rcv.phase1Desc')}</p>
          <p className="mt-1 text-xs font-medium text-[var(--color-primary)]">{t('wb.rcv.phase1Pipeline')}</p>
          <p className="mt-2 text-xs text-[var(--text-muted)]">
            {truck.plate ? `${t('wb.rcv.truckPlate')}: ${truck.plate}` : t('wb.rcv.noTruckYet')}
            {truck.supplier ? ` · ${truck.supplier}` : ''}
            {' · '}
            {t('wb.rcv.stageOf').replace('{n}', String(stageIdx + 1)).replace('{total}', String(STAGES.length))}
          </p>
        </div>
        <div className="flex flex-col items-end gap-2">
          <div className="rounded-md border border-[var(--border-default)] bg-[var(--color-surface)] px-3 py-2 text-right">
            <p className="text-[10px] uppercase tracking-wide text-[var(--text-muted)]">{t('wizard.systemCode')}</p>
            <p className="font-mono text-sm font-medium">{minted.gr ?? minted.mi ?? t('wizard.autoGenerated')}</p>
            <p className="text-[10px] text-[var(--text-muted)]">GR · MI · LOT</p>
          </div>
          <Link
            to="/inventory/operations/goods-receipts"
            className="text-sm font-medium text-[var(--color-primary)] hover:underline"
          >
            {t('wizard.backToLibrary')}
          </Link>
        </div>
      </div>

      <div className="h-1.5 w-full overflow-hidden rounded-full bg-[var(--color-surface-hover)]">
        <div
          className="h-full rounded-full bg-[var(--color-primary)] transition-all duration-300"
          style={{ width: `${posted ? 100 : progress}%` }}
        />
      </div>

      <div className="grid flex-1 gap-3 lg:grid-cols-[220px_minmax(0,1fr)_260px]">
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

        <main className="min-w-0 space-y-3">
          <Card>
            <CardHeader>
              <CardTitle>
                {stageIdx + 1}. {t(stage.titleKey)}
              </CardTitle>
              <CardDescription>{t(stage.hintKey)}</CardDescription>
            </CardHeader>
            <CardContent className="space-y-4">
              {stage.id === 'deliveryNote' ? (
                <div className="space-y-3">
                  <p className="text-sm text-[var(--text-secondary)]">{t('wb.rcv.deliveryNoteIntro')}</p>
                  <div className="grid gap-2 sm:grid-cols-3">
                    <label className="block space-y-1 text-sm">
                      <span className="text-[var(--text-secondary)]">{t('wb.rcv.truckPlate')}</span>
                      <Input
                        value={truck.plate}
                        disabled={posted}
                        onChange={(e) => setTruck((tr) => ({ ...tr, plate: e.target.value }))}
                      />
                    </label>
                    <label className="block space-y-1 text-sm">
                      <span className="text-[var(--text-secondary)]">{t('wb.rcv.supplier')}</span>
                      <Input
                        value={truck.supplier}
                        disabled={posted}
                        onChange={(e) => setTruck((tr) => ({ ...tr, supplier: e.target.value }))}
                      />
                    </label>
                    <label className="block space-y-1 text-sm">
                      <span className="text-[var(--text-secondary)]">{t('wb.rcv.gate')}</span>
                      <Input
                        value={truck.gate}
                        disabled={posted}
                        onChange={(e) => setTruck((tr) => ({ ...tr, gate: e.target.value }))}
                      />
                    </label>
                  </div>
                  <div className="flex flex-wrap gap-2">
                    {(['deliveryNote', 'packingList', 'purchaseOrder'] as const).map((kind) => (
                      <Button
                        key={kind}
                        type="button"
                        variant={docs.includes(kind) ? 'default' : 'secondary'}
                        disabled={posted}
                        onClick={() => addDoc(kind)}
                      >
                        {t(`wb.rcv.doc.${kind}`)}
                        {docs.includes(kind) ? ` · ${t('wb.rcv.attached')}` : ''}
                      </Button>
                    ))}
                  </div>
                  {docs.length === 0 ? (
                    <p className="text-sm text-[var(--text-muted)]">{t('wb.rcv.docEmpty')}</p>
                  ) : null}
                </div>
              ) : null}

              {stage.id === 'ocr' ? (
                <div className="space-y-3">
                  <p className="rounded-md border border-[var(--border-default)] bg-[var(--color-surface-hover)] px-3 py-2 text-sm text-[var(--text-secondary)]">
                    {t('wb.rcv.demoOcrBanner')}
                  </p>
                  <div className="flex flex-wrap items-start justify-between gap-2">
                    <p className="text-sm text-[var(--text-secondary)]">{t('wb.rcv.ocrIntro')}</p>
                    <div className="flex flex-wrap gap-2">
                      {!ocrEditing ? (
                        <Button
                          type="button"
                          variant={ocrFlaggedCount > 0 ? 'default' : 'secondary'}
                          disabled={posted}
                          onClick={() => {
                            setOcrEditing(true);
                            setOcrAccepted(false);
                          }}
                        >
                          {t('wb.rcv.ocrFixErrors')}
                          {ocrFlaggedCount > 0 ? ` (${ocrFlaggedCount})` : ''}
                        </Button>
                      ) : (
                        <>
                          <Button
                            type="button"
                            variant="secondary"
                            onClick={() => {
                              setOcrFields((prev) => {
                                const next = { ...prev };
                                for (const k of OCR_FIELD_KEYS) {
                                  next[k] = { value: OCR_DEMO_CORRECTED[k], confidence: 99, flagged: false };
                                }
                                return next;
                              });
                              setCountQty(OCR_DEMO_CORRECTED.quantity);
                              setQuantityVerified(false);
                              clearMaterialMatch();
                            }}
                          >
                            {t('wb.rcv.ocrApplySuggested')}
                          </Button>
                          <Button
                            type="button"
                            onClick={() => {
                              setOcrFields((prev) => {
                                const next = { ...prev };
                                for (const k of OCR_FIELD_KEYS) {
                                  next[k] = { ...next[k], flagged: false, confidence: Math.max(next[k].confidence, 95) };
                                }
                                return next;
                              });
                              setCountQty(ocrFields.quantity.value.replace(/\D/g, '') || countQty);
                              setQuantityVerified(false);
                              clearMaterialMatch();
                              setOcrEditing(false);
                            }}
                          >
                            {t('wb.rcv.ocrSaveFixes')}
                          </Button>
                        </>
                      )}
                    </div>
                  </div>
                  {ocrFlaggedCount > 0 && !ocrEditing ? (
                    <p className="text-sm text-[var(--color-danger)]">{t('wb.rcv.ocrErrorsHint')}</p>
                  ) : null}
                  <div className="grid gap-2 md:grid-cols-2">
                    {OCR_FIELD_KEYS.map((k) => {
                      const field = ocrFields[k];
                      const bad = field.flagged || field.confidence < 80;
                      return (
                        <div
                          key={k}
                          className={`rounded-md border px-3 py-2 ${
                            bad ? 'border-[var(--color-danger)] bg-[var(--color-danger)]/5' : 'border-[var(--border-default)]'
                          }`}
                        >
                          <div className="flex items-center justify-between gap-2">
                            <p className="text-[10px] uppercase text-[var(--text-muted)]">{t(`wb.rcv.ocrField.${k}`)}</p>
                            {bad ? (
                              <span className="text-[10px] font-semibold text-[var(--color-danger)]">{t('wb.rcv.ocrNeedsFix')}</span>
                            ) : null}
                          </div>
                          {ocrEditing ? (
                            <Input
                              className="mt-1"
                              value={field.value}
                              onChange={(e) =>
                                setOcrFields((prev) => ({
                                  ...prev,
                                  [k]: { ...prev[k], value: e.target.value, flagged: false },
                                }))
                              }
                            />
                          ) : (
                            <p className="font-medium">{field.value}</p>
                          )}
                          <p className={`text-[10px] ${bad ? 'text-[var(--color-danger)]' : 'text-[var(--text-muted)]'}`}>
                            {t('wb.rcv.ocrConfidence')} {field.confidence}% · {t('wb.rcv.demoConfidenceNote')}
                          </p>
                        </div>
                      );
                    })}
                  </div>
                  <label className="flex items-center gap-2 text-sm">
                    <input
                      type="checkbox"
                      checked={ocrAccepted}
                      disabled={ocrEditing || ocrFlaggedCount > 0 || posted}
                      onChange={(e) => setOcrAccepted(e.target.checked)}
                    />
                    {t('wb.rcv.ocrAccept')}
                  </label>
                </div>
              ) : null}

              {stage.id === 'control' ? (
                <div className="space-y-3">
                  <p className="text-sm text-[var(--text-secondary)]">{t('wb.rcv.controlIntro')}</p>
                  <dl className="grid gap-2 sm:grid-cols-2">
                    {OCR_FIELD_KEYS.map((k) => (
                      <div key={k} className="rounded-md border border-[var(--border-default)] px-3 py-2">
                        <dt className="text-[10px] uppercase text-[var(--text-muted)]">{t(`wb.rcv.ocrField.${k}`)}</dt>
                        <dd className="text-sm font-medium">{ocrFields[k].value}</dd>
                      </div>
                    ))}
                  </dl>
                  <p className="text-xs text-[var(--text-muted)]">{t('wb.rcv.controlNotMaster')}</p>
                  <label className="flex items-center gap-2 text-sm font-medium">
                    <input
                      type="checkbox"
                      checked={controlAccepted}
                      disabled={posted}
                      onChange={(e) => setControlAccepted(e.target.checked)}
                    />
                    {t('wb.rcv.controlAccept')}
                  </label>
                </div>
              ) : null}

              {stage.id === 'materialMatch' ? (
                <div className="space-y-3">
                  <p className="text-sm text-[var(--text-secondary)]">{t('wb.rcv.materialMatchHint')}</p>

                  <div className="rounded-md border border-[var(--border-default)] px-3 py-3">
                    <p className="text-[10px] uppercase text-[var(--text-muted)]">{t('wb.rcv.ocrReadLabel')}</p>
                    <p className="font-medium">{ocrFields.material.value}</p>
                    <p className="mt-1 text-xs text-[var(--text-muted)]">
                      {t('wb.rcv.ocrConfidence')}: %{ocrConfidence}
                      {' · '}
                      {t('wb.rcv.parsedDims')}: {formatDims(parseDimensions(ocrFields.material.value) ?? parseDimensions(ocrFields.dimensions.value))}
                    </p>
                  </div>

                  {suggestedMatch ? (
                    <div
                      className={`rounded-md border px-3 py-3 ${
                        suggestedMatch.status === 'STRONG_MATCH'
                          ? 'border-[var(--color-primary)]/40 bg-[var(--color-primary)]/5'
                          : suggestedMatch.status === 'REVIEW_REQUIRED'
                            ? 'border-[var(--border-default)]'
                            : 'border-[var(--color-danger)]/40 bg-[var(--color-danger)]/5'
                      }`}
                    >
                      <p className="text-[10px] uppercase text-[var(--text-muted)]">{t('wb.rcv.suggestedMaterial')}</p>
                      <p className="font-mono text-sm font-semibold">{suggestedMatch.material.code}</p>
                      <p className="text-sm">{suggestedMatch.material.name}</p>
                      <p className="text-xs text-[var(--text-muted)]">
                        {formatDims(
                          parseDefinitionDims(suggestedMatch.material.definitionJson) ??
                            parseDimensions(suggestedMatch.material.name),
                        )}
                      </p>
                      <div className="mt-2 grid gap-1 text-xs sm:grid-cols-2">
                        <p>
                          {t('wb.rcv.ocrConfidence')}: <span className="font-medium">%{ocrConfidence}</span>
                        </p>
                        <p>
                          {t('wb.rcv.materialMatchScore')}:{' '}
                          <span className="font-medium">%{suggestedMatch.score}</span>
                        </p>
                        <p className="sm:col-span-2 font-medium">
                          {t(`wb.rcv.matchStatus.${suggestedMatch.status}`)}
                        </p>
                      </div>
                      {matchConfirmed && matchedMaterialCode === suggestedMatch.material.code ? (
                        <p className="mt-2 text-sm font-medium text-[var(--color-primary)]">
                          {t('wb.rcv.matchConfirmedBanner')} · {matchedMaterialCode}
                        </p>
                      ) : null}
                      <div className="mt-3 flex flex-wrap gap-2">
                        <Button
                          type="button"
                          disabled={posted || suggestedMatch.status === 'NO_MATCH'}
                          onClick={() => confirmMatch(suggestedMatch)}
                        >
                          {t('wb.rcv.confirmMatch')}
                        </Button>
                        <Button
                          type="button"
                          variant="secondary"
                          disabled={posted}
                          onClick={() => {
                            setShowMaterialPicker(true);
                            clearMaterialMatch();
                          }}
                        >
                          {t('wb.rcv.pickOtherMaterial')}
                        </Button>
                      </div>
                      {suggestedMatch.status === 'NO_MATCH' ? (
                        <p className="mt-2 text-xs text-[var(--color-danger)]">{t('wb.rcv.noMatchHint')}</p>
                      ) : null}
                    </div>
                  ) : (
                    <p className="text-sm text-[var(--color-danger)]">{t('wb.rcv.noMaterialsInMaster')}</p>
                  )}

                  {showMaterialPicker ? (
                    <div className="space-y-2 rounded-md border border-[var(--border-default)] px-3 py-3">
                      <p className="text-sm font-medium">{t('wb.rcv.materialPickerTitle')}</p>
                      <Input
                        value={materialSearch}
                        disabled={posted}
                        placeholder={t('wb.rcv.materialSearchPlaceholder')}
                        onChange={(e) => setMaterialSearch(e.target.value)}
                      />
                      <ul className="max-h-48 space-y-1 overflow-y-auto text-sm">
                        {pickerList.map((m) => (
                          <li key={m.id}>
                            <button
                              type="button"
                              disabled={posted}
                              className="flex w-full flex-col rounded-md px-2 py-1.5 text-left hover:bg-[var(--color-surface-hover)]"
                              onClick={() => selectMasterMaterial(m)}
                            >
                              <span className="font-mono text-xs font-semibold">{m.code}</span>
                              <span>{m.name}</span>
                            </button>
                          </li>
                        ))}
                        {pickerList.length === 0 ? (
                          <li className="text-xs text-[var(--text-muted)]">{t('wb.rcv.noMaterialsInMaster')}</li>
                        ) : null}
                      </ul>
                    </div>
                  ) : null}

                  {materialsQuery.isError ? (
                    <p className="text-xs text-[var(--color-danger)]">{t('wb.rcv.materialLoadError')}</p>
                  ) : null}
                </div>
              ) : null}

              {stage.id === 'qtyVerify' ? (
                <div className="space-y-3">
                  <p className="text-sm text-[var(--text-secondary)]">{t('wb.rcv.qtyVerifyIntro')}</p>
                  <p className="text-xs text-[var(--text-muted)]">
                    OCR miktar: <span className="font-medium">{ocrFields.quantity.value}</span> — {t('wb.rcv.qtyOcrNotTrusted')}
                  </p>
                  <label className="block max-w-xs space-y-1 text-sm">
                    <span className="text-[var(--text-secondary)]">{t('wb.rcv.countedQty')}</span>
                    <Input
                      type="number"
                      value={countQty}
                      disabled={posted}
                      onChange={(e) => {
                        setCountQty(e.target.value);
                        setQuantityVerified(false);
                        setApproved(false);
                      }}
                    />
                  </label>
                  <label className="flex items-center gap-2 text-sm font-medium">
                    <input
                      type="checkbox"
                      checked={quantityVerified}
                      disabled={posted || Number(countQty) <= 0}
                      onChange={(e) => {
                        setQuantityVerified(e.target.checked);
                        if (!e.target.checked) setApproved(false);
                      }}
                    />
                    {t('wb.rcv.qtyVerified')}
                  </label>
                </div>
              ) : null}

              {stage.id === 'approve' ? (
                <div className="space-y-3">
                  <div className="grid gap-2 md:grid-cols-2">
                    {[
                      [t('wb.rcv.doc.deliveryNote'), docs.includes('deliveryNote') ? t('wb.rcv.attached') : '—'],
                      [t('wb.rcv.truck'), `${truck.plate} · ${truck.supplier}`],
                      [t('wb.rcv.matchedMaterial'), matchConfirmed ? `${matchedMaterialCode} · id ${matchedMaterialId.slice(0, 8)}…` : t('wb.rcv.noMaterialMatched')],
                      [t('wb.rcv.countedQty'), `${countQty} · ${quantityVerified ? t('wb.rcv.qtyVerifiedShort') : '—'}`],
                      [t('wb.rcv.ocrField.material'), ocrFields.material.value],
                    ].map(([label, value]) => (
                      <div key={label} className="rounded-md border border-[var(--border-default)] px-3 py-2">
                        <p className="text-[10px] uppercase text-[var(--text-muted)]">{label}</p>
                        <p className="text-sm font-medium">{value}</p>
                      </div>
                    ))}
                  </div>
                  <div className="grid gap-2 sm:grid-cols-2">
                    <label className="block space-y-1 text-sm">
                      <span className="text-[var(--text-secondary)]">{t('wb.rcv.whPick')}</span>
                      <Input value={warehouse} disabled={posted} onChange={(e) => setWarehouse(e.target.value)} />
                    </label>
                    <label className="block space-y-1 text-sm">
                      <span className="text-[var(--text-secondary)]">{t('wb.rcv.locPick')}</span>
                      <Input value={location} disabled={posted} onChange={(e) => setLocation(e.target.value)} />
                    </label>
                  </div>
                  <p className="text-xs text-[var(--text-muted)]">{t('wb.rcv.approveStockNote')}</p>
                  <label className="flex items-center gap-2 text-sm font-medium">
                    <input
                      type="checkbox"
                      checked={approved}
                      disabled={posted || !matchedMaterialCode || !quantityVerified}
                      onChange={(e) => setApproved(e.target.checked)}
                    />
                    {t('wb.rcv.approvePost')}
                  </label>
                </div>
              ) : null}

              {stage.id === 'stock' ? (
                <div className="space-y-3">
                  <p className="text-sm text-[var(--text-secondary)]">{t('wb.rcv.stockIntro')}</p>
                  {postBlockedReason ? (
                    <p className="rounded-md border border-[var(--color-danger)]/40 bg-[var(--color-danger)]/5 px-3 py-2 text-sm text-[var(--color-danger)]">
                      {postBlockedReason}
                    </p>
                  ) : (
                    <p className="rounded-md border border-[var(--border-default)] bg-[var(--color-surface-hover)] px-3 py-2 text-sm">
                      {t('wb.rcv.stockReady')
                        .replace('{material}', matchedMaterialCode)
                        .replace('{qty}', countQty)}
                    </p>
                  )}
                  <ul className="list-inside list-disc text-sm text-[var(--text-primary)]">
                    <li>{t('wb.rcv.postItem.txn')}</li>
                    <li>{t('wb.rcv.postItem.receiving')}</li>
                    <li>{t('wb.rcv.postItem.stock')}</li>
                    <li>{t('wb.rcv.postItem.mi')}</li>
                  </ul>
                  {posted ? (
                    <p className="text-sm font-medium text-[var(--color-primary)]">
                      {t('wb.rcv.postedBanner')}
                      {minted.gr ? ` · ${minted.gr}` : ''}
                      {minted.mi ? ` · ${minted.mi}` : ''}
                      {' — '}
                      {t('wizard.inLibrary')}
                    </p>
                  ) : null}
                </div>
              ) : null}

              {gateMessage ? <p className="text-sm text-[var(--color-danger)]">{gateMessage}</p> : null}
              {error ? <p className="text-sm text-[var(--color-danger)]">{error}</p> : null}
            </CardContent>
          </Card>
        </main>

        <aside className="space-y-3">
          <div className="rounded-lg border border-[var(--border-default)] bg-[var(--color-surface)] p-3">
            <p className="text-[10px] font-semibold uppercase tracking-wide text-[var(--text-muted)]">{t('wb.rcv.context')}</p>
            <dl className="mt-2 space-y-2 text-xs">
              <div>
                <dt className="text-[var(--text-muted)]">{t('wb.rcv.doc.deliveryNote')}</dt>
                <dd className="font-medium">{docs.includes('deliveryNote') ? t('wb.rcv.attached') : '—'}</dd>
              </div>
              <div>
                <dt className="text-[var(--text-muted)]">{t('wb.rcv.matchedMaterial')}</dt>
                <dd className="font-medium">
                  {matchConfirmed ? matchedMaterialCode : t('wb.rcv.noMaterialMatched')}
                  {confirmedMatchScore != null && matchConfirmed ? ` · %${confirmedMatchScore}` : ''}
                </dd>
              </div>
              <div>
                <dt className="text-[var(--text-muted)]">{t('wb.rcv.ocrConfidence')}</dt>
                <dd className="font-medium">%{ocrConfidence}</dd>
              </div>
              <div>
                <dt className="text-[var(--text-muted)]">{t('wb.rcv.countedQty')}</dt>
                <dd className="font-medium">
                  {countQty}
                  {quantityVerified ? ` · ${t('wb.rcv.qtyVerifiedShort')}` : ''}
                </dd>
              </div>
              <div>
                <dt className="text-[var(--text-muted)]">{t('wb.rcv.supplier')}</dt>
                <dd className="font-medium">{truck.supplier || '—'}</dd>
              </div>
            </dl>
          </div>
          <div className="rounded-lg border border-dashed border-[var(--border-default)] p-3 text-xs text-[var(--text-muted)]">
            {t('wb.rcv.phase1GuardNote')}
          </div>
        </aside>
      </div>

      <div className="sticky bottom-0 z-10 -mx-1 flex flex-wrap items-center justify-between gap-2 border-t border-[var(--border-default)] bg-[var(--color-bg,var(--color-surface))] px-2 py-3">
        <Button type="button" variant="secondary" disabled={stageIdx === 0 || posted} onClick={() => setStageIdx((s) => s - 1)}>
          {t('wizard.back')}
        </Button>
        <div className="flex flex-wrap gap-2">
          {stage.id !== 'stock' ? (
            <Button type="button" disabled={!canAdvance || posted} onClick={goNext}>
              {t('wb.rcv.nextStage')}
            </Button>
          ) : (
            <>
              <Button
                type="button"
                disabled={posted || persistMutation.isPending || !!postBlockedReason}
                onClick={() => {
                  if (postBlockedReason) {
                    setError(postBlockedReason);
                    return;
                  }
                  setMinted((m) => ({
                    ...m,
                    mi: m.mi ?? mintPreview('MI'),
                    lot: m.lot ?? mintPreview('LOT'),
                    pkg: m.pkg ?? mintPreview('PKG'),
                  }));
                  persistMutation.mutate();
                }}
              >
                {persistMutation.isPending ? t('saving') : t('wb.rcv.postToStock')}
              </Button>
              {posted ? (
                <Button
                  type="button"
                  variant="secondary"
                  onClick={() => void navigate({ to: '/inventory/operations/goods-receipts' })}
                >
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

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
import { DocumentControlPanel } from './DocumentControlPanel';
import {
  TruckEvidenceStep,
  type EvidenceDocKind,
  type PhotoSlot,
  type TruckInfo,
} from './TruckEvidenceStep';

/**
 * Real ops receiving rail (6 stages):
 * 1 Kamyon & Kanıt → 2 Malzeme kontrolü → 3 Fiziksel sayım → 4 Karşılaştırma → 5 Sonuç → 6 Stok
 *
 * This change implements Stage 1 fully. Stages 2–6 keep the existing safe stock path
 * (material master match + verified qty) until each stage is built next.
 */
type StageId = 'truckEvidence' | 'materialCheck' | 'physicalCount' | 'compare' | 'result' | 'stock';

interface StageDef {
  id: StageId;
  titleKey: string;
  hintKey: string;
}

const STAGES: StageDef[] = [
  { id: 'truckEvidence', titleKey: 'wb.rcv.ops.step.truckEvidence', hintKey: 'wb.rcv.ops.step.truckEvidenceHint' },
  { id: 'materialCheck', titleKey: 'wb.rcv.ops.step.materialCheck', hintKey: 'wb.rcv.ops.step.materialCheckHint' },
  { id: 'physicalCount', titleKey: 'wb.rcv.ops.step.physicalCount', hintKey: 'wb.rcv.ops.step.physicalCountHint' },
  { id: 'compare', titleKey: 'wb.rcv.ops.step.compare', hintKey: 'wb.rcv.ops.step.compareHint' },
  { id: 'result', titleKey: 'wb.rcv.ops.step.result', hintKey: 'wb.rcv.ops.step.resultHint' },
  { id: 'stock', titleKey: 'wb.rcv.ops.step.stock', hintKey: 'wb.rcv.ops.step.stockHint' },
];

/** Placeholder label used until Stage 3 document reading extracts lines. */
const PLACEHOLDER_MATERIAL_LABEL = 'Thermowood Deck 26×140×3000';

function mintPreview(prefix: string) {
  const seq =
    new Date().toISOString().slice(2, 10).replace(/-/g, '') +
    '-' +
    String(Math.floor(100000 + Math.random() * 900000));
  return `${prefix}-${seq}`;
}

function nowDate() {
  return new Date().toISOString().slice(0, 10);
}

function nowTime() {
  return new Date().toTimeString().slice(0, 5);
}

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

  const [truck, setTruck] = useState<TruckInfo>({
    plate: '34 ABC 123',
    trailer: '34 DEF 456',
    driver: 'Ahmet Yılmaz',
    supplier: 'Nordic Timber',
    arrivalDate: nowDate(),
    arrivalTime: nowTime(),
    gate: '2',
  });
  const [docs, setDocs] = useState<EvidenceDocKind[]>([]);
  const [photos, setPhotos] = useState<Partial<Record<PhotoSlot, boolean>>>({});

  const [materialLabel, setMaterialLabel] = useState(PLACEHOLDER_MATERIAL_LABEL);
  const [preAccept, setPreAccept] = useState<'none' | 'ok' | 'conditional' | 'reject'>('none');
  const [countQty, setCountQty] = useState('48');
  const [quantityVerified, setQuantityVerified] = useState(false);
  const [compareResolved, setCompareResolved] = useState(false);
  const [matchedMaterialCode, setMatchedMaterialCode] = useState('');
  const [matchedMaterialId, setMatchedMaterialId] = useState('');
  const [matchConfirmed, setMatchConfirmed] = useState(false);
  const [confirmedMatchScore, setConfirmedMatchScore] = useState<number | null>(null);
  const [showMaterialPicker, setShowMaterialPicker] = useState(false);
  const [materialSearch, setMaterialSearch] = useState('');
  const [warehouse, setWarehouse] = useState('WH-RM');
  const [location, setLocation] = useState('A-03-02');

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
    () => rankMaterialMatches(materialLabel, materialCandidates, undefined, 8),
    [materialLabel, materialCandidates],
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
  const photoCount = Object.values(photos).filter(Boolean).length;

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
      case 'truckEvidence':
        if (!truck.plate.trim()) return t('wb.rcv.gateNeedPlate');
        if (!truck.supplier.trim()) return t('wb.rcv.gateNeedSupplier');
        if (docs.length === 0 && photoCount === 0) return t('wb.rcv.ops.gateNeedEvidence');
        return null;
      case 'materialCheck':
        if (preAccept === 'none') return t('wb.rcv.ops.gateNeedPreAccept');
        if (preAccept === 'reject') return t('wb.rcv.ops.gatePreAcceptReject');
        if (!matchConfirmed || !matchedMaterialCode.trim()) return t('wb.rcv.gateNeedMaterialConfirm');
        return null;
      case 'physicalCount':
        if (Number(countQty) <= 0) return t('wb.rcv.gateNeedCount');
        if (!quantityVerified) return t('wb.rcv.gateNeedQtyVerify');
        return null;
      case 'compare':
        return !compareResolved ? t('wb.rcv.gateNeedControl') : null;
      case 'result':
        return !approved ? t('wb.rcv.gateNeedApprove') : null;
      case 'stock':
        return postBlockedReason;
      default:
        return null;
    }
  }, [
    stage.id,
    truck.plate,
    truck.supplier,
    docs.length,
    photoCount,
    preAccept,
    matchConfirmed,
    matchedMaterialCode,
    countQty,
    quantityVerified,
    compareResolved,
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
    const scored = rankMaterialMatches(materialLabel, [m], undefined, 1)[0];
    setMatchedMaterialCode(m.code);
    setMatchedMaterialId(m.id);
    setConfirmedMatchScore(scored?.score ?? 0);
    setMatchConfirmed(true);
    setShowMaterialPicker(false);
    setApproved(false);
  }

  function toggleDoc(kind: EvidenceDocKind) {
    setDocs((d) => (d.includes(kind) ? d.filter((x) => x !== kind) : [...d, kind]));
  }

  function togglePhoto(slot: PhotoSlot) {
    setPhotos((p) => ({ ...p, [slot]: !p[slot] }));
  }

  const persistMutation = useMutation({
    mutationFn: async () => {
      if (!matchConfirmed || !quantityVerified || !approved) {
        throw new Error(t('wb.rcv.gateNeedMaterialConfirm'));
      }
      const materialCode = matchedMaterialCode.trim();
      const materialId = matchedMaterialId.trim();
      if (!materialCode || !materialId) throw new Error(t('wb.rcv.gateNeedMaterialConfirm'));
      const qty = Number(countQty) || 0;
      if (qty <= 0) throw new Error(t('wb.rcv.gateNeedCount'));

      const whCode = warehouse.trim() || 'WH-RM';
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
          `pipeline=truckEvidence>materialCheck>physicalCount>compare>result>stock`,
          `supplier=${truck.supplier}`,
          `trailer=${truck.trailer}`,
          `driver=${truck.driver}`,
          `arrival=${truck.arrivalDate}T${truck.arrivalTime}`,
          `gate=${truck.gate}`,
          `preAccept=${preAccept}`,
          `photos=${photoCount}`,
          `docs=${docs.join(',')}`,
          `qty=${qty}`,
          `loc=${locCode}`,
          `materialId=${materialId}`,
          `material=${materialCode}`,
          `matchScore=${confirmedMatchScore ?? ''}`,
          `label=${materialLabel}`,
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
      persistMutation.mutate();
      return;
    }
    const next = Math.min(stageIdx + 1, STAGES.length - 1);
    setStageIdx(next);
    setMaxReached((m) => Math.max(m, next));
  }

  return (
    <div className="flex min-h-[calc(100vh-7rem)] flex-col gap-3">
      <div className="flex flex-wrap items-start justify-between gap-3 border-b border-[var(--border-default)] pb-3">
        <div>
          <p className="text-xs font-medium text-[var(--text-muted)]">INV-RCV-001 · {t('wb.rcv.ops.screenType')}</p>
          <h2 className="text-xl font-semibold tracking-tight">{t('wb.rcv.title')}</h2>
          <p className="mt-1 max-w-2xl text-sm text-[var(--text-secondary)]">{t('wb.rcv.ops.desc')}</p>
          <p className="mt-1 text-xs font-medium text-[var(--color-primary)]">{t('wb.rcv.ops.pipeline')}</p>
          <p className="mt-2 text-xs text-[var(--text-muted)]">
            {truck.plate ? `${t('wb.rcv.truckPlate')}: ${truck.plate}` : t('wb.rcv.noTruckYet')}
            {truck.supplier ? ` · ${truck.supplier}` : ''}
            {` · ${docs.length} ${t('wb.rcv.files')} · ${photoCount} ${t('wb.rcv.photos')}`}
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

      <div className="grid flex-1 gap-3 lg:grid-cols-[200px_minmax(0,1fr)_240px]">
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
              {stage.id === 'truckEvidence' ? (
                <TruckEvidenceStep
                  truck={truck}
                  onTruckChange={setTruck}
                  docs={docs}
                  onToggleDoc={toggleDoc}
                  photos={photos}
                  onTogglePhoto={togglePhoto}
                  disabled={posted}
                />
              ) : null}

              {stage.id === 'materialCheck' ? (
                <div className="space-y-4">
                  <p className="rounded-md border border-[var(--border-default)] bg-[var(--color-surface-hover)] px-3 py-2 text-sm text-[var(--text-secondary)]">
                    {t('wb.rcv.ops.stage2Interim')}
                  </p>

                  <div className="space-y-2">
                    <p className="text-sm font-medium">{t('wb.rcv.ops.preAcceptTitle')}</p>
                    <div className="flex flex-wrap gap-2">
                      {(
                        [
                          ['ok', t('wb.rcv.ops.preAccept.ok')],
                          ['conditional', t('wb.rcv.ops.preAccept.conditional')],
                          ['reject', t('wb.rcv.ops.preAccept.reject')],
                        ] as const
                      ).map(([id, label]) => (
                        <Button
                          key={id}
                          type="button"
                          size="sm"
                          variant={preAccept === id ? 'default' : 'secondary'}
                          disabled={posted}
                          onClick={() => setPreAccept(id)}
                        >
                          {label}
                        </Button>
                      ))}
                    </div>
                  </div>

                  <div className="rounded-md border border-[var(--border-default)] px-3 py-3">
                    <p className="text-[10px] uppercase text-[var(--text-muted)]">{t('wb.rcv.ops.incomingLabel')}</p>
                    <Input
                      className="mt-1"
                      value={materialLabel}
                      disabled={posted}
                      onChange={(e) => {
                        setMaterialLabel(e.target.value);
                        clearMaterialMatch();
                      }}
                    />
                  </div>

                  {suggestedMatch ? (
                    <div className="rounded-md border border-[var(--border-default)] px-3 py-3">
                      <p className="text-[10px] uppercase text-[var(--text-muted)]">{t('wb.rcv.suggestedMaterial')}</p>
                      <p className="font-mono text-sm font-semibold">{suggestedMatch.material.code}</p>
                      <p className="text-sm">{suggestedMatch.material.name}</p>
                      <p className="text-xs text-[var(--text-muted)]">
                        {formatDims(
                          parseDefinitionDims(suggestedMatch.material.definitionJson) ??
                            parseDimensions(suggestedMatch.material.name),
                        )}
                      </p>
                      <p className="mt-1 text-xs">
                        {t('wb.rcv.materialMatchScore')}: %{suggestedMatch.score} ·{' '}
                        {t(`wb.rcv.matchStatus.${suggestedMatch.status}`)}
                      </p>
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
                      {matchConfirmed ? (
                        <p className="mt-2 text-sm font-medium text-[var(--color-primary)]">
                          {t('wb.rcv.matchConfirmedBanner')} · {matchedMaterialCode}
                        </p>
                      ) : null}
                    </div>
                  ) : (
                    <p className="text-sm text-[var(--color-danger)]">{t('wb.rcv.noMaterialsInMaster')}</p>
                  )}

                  {showMaterialPicker ? (
                    <div className="space-y-2 rounded-md border border-[var(--border-default)] px-3 py-3">
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
                      </ul>
                    </div>
                  ) : null}
                </div>
              ) : null}

              {stage.id === 'physicalCount' ? (
                <div className="space-y-3">
                  <p className="text-sm text-[var(--text-secondary)]">{t('wb.rcv.ops.stage3Interim')}</p>
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

              {stage.id === 'compare' ? (
                <DocumentControlPanel
                  disabled={posted}
                  onResolvedChange={setCompareResolved}
                  onRequestMaterialMatch={(line) => {
                    setMaterialLabel(line.product);
                    clearMaterialMatch();
                    const idx = STAGES.findIndex((s) => s.id === 'materialCheck');
                    if (idx >= 0) {
                      setStageIdx(idx);
                      setMaxReached((m) => Math.max(m, idx));
                    }
                  }}
                />
              ) : null}

              {stage.id === 'result' ? (
                <div className="space-y-3">
                  <div className="grid gap-2 md:grid-cols-2">
                    {[
                      [t('wb.rcv.truck'), `${truck.plate} · ${truck.trailer} · ${truck.driver}`],
                      [t('wb.rcv.supplier'), truck.supplier],
                      [t('wb.rcv.ops.evidenceSummary'), `${docs.length} belge · ${photoCount} foto`],
                      [t('wb.rcv.ops.preAcceptTitle'), preAccept === 'none' ? '—' : t(`wb.rcv.ops.preAccept.${preAccept}`)],
                      [t('wb.rcv.matchedMaterial'), matchConfirmed ? matchedMaterialCode : t('wb.rcv.noMaterialMatched')],
                      [t('wb.rcv.countedQty'), `${countQty} · ${quantityVerified ? t('wb.rcv.qtyVerifiedShort') : '—'}`],
                      [t('wb.rcv.warehouse'), `${warehouse} · ${location}`],
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
                  <label className="flex items-center gap-2 text-sm font-medium">
                    <input
                      type="checkbox"
                      checked={approved}
                      disabled={posted || !matchConfirmed || !quantityVerified}
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
                  {posted ? (
                    <p className="text-sm font-medium text-[var(--color-primary)]">
                      {t('wb.rcv.postedBanner')}
                      {minted.gr ? ` · ${minted.gr}` : ''}
                      {minted.mi ? ` · ${minted.mi}` : ''}
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
                <dt className="text-[var(--text-muted)]">{t('wb.rcv.truckPlate')}</dt>
                <dd className="font-medium">{truck.plate || '—'}</dd>
              </div>
              <div>
                <dt className="text-[var(--text-muted)]">{t('wb.rcv.ops.evidenceSummary')}</dt>
                <dd className="font-medium">
                  {docs.length} {t('wb.rcv.files')} · {photoCount} {t('wb.rcv.photos')}
                </dd>
              </div>
              <div>
                <dt className="text-[var(--text-muted)]">{t('wb.rcv.matchedMaterial')}</dt>
                <dd className="font-medium">{matchConfirmed ? matchedMaterialCode : t('wb.rcv.noMaterialMatched')}</dd>
              </div>
              <div>
                <dt className="text-[var(--text-muted)]">{t('wb.rcv.countedQty')}</dt>
                <dd className="font-medium">
                  {countQty}
                  {quantityVerified ? ` · ${t('wb.rcv.qtyVerifiedShort')}` : ''}
                </dd>
              </div>
            </dl>
          </div>
          <div className="rounded-lg border border-dashed border-[var(--border-default)] p-3 text-xs text-[var(--text-muted)]">
            {t('wb.rcv.ops.stage1GuardNote')}
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

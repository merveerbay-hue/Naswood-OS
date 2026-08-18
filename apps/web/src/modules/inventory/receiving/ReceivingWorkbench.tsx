import { Link, useNavigate } from '@tanstack/react-router';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { useEffect, useMemo, useState } from 'react';
import { Button, Card, CardContent, CardDescription, CardHeader, CardTitle } from '@naswood/ui';
import { executeStockDocument, searchAllResource, searchResource } from '@/api/business';
import { useAuth } from '@/auth/useAuth';
import { useI18n } from '@/i18n';
import { type MaterialCandidate } from './materialMatch';
import {
  TruckEvidenceStep,
  type EvidenceDocKind,
  type PhotoSlot,
  type TruckInfo,
} from './TruckEvidenceStep';
import {
  MaterialCheckStep,
  createDefaultMaterialCheck,
  type MaterialCheckState,
} from './MaterialCheckStep';
import { PhysicalCountStep, finalPhysicalQtyFromLines } from './PhysicalCountStep';
import { IncomingLineCheckPanel } from './IncomingLineCheckPanel';
import { CompareStep } from './CompareStep';
import { ResultStep } from './ResultStep';
import { StockStep } from './StockStep';
import {
  allCountableCounted,
  allLinesReadyForCount,
  countableLines,
  finalLineQty,
  formatLineDims,
  formatPhysicalDims,
  lineMaterialMatched,
  linePhysicalVolumeM3,
  seedIncomingFromDocuments,
  stockBasisQty,
  stockBasisVolumeM3,
  syncBatchPreAccept,
  type IncomingLine,
} from './incomingLines';
import {
  acceptedStockLines,
  buildDefaultDistributions,
  buildExecuteLines,
  mintLotNumber,
  validateDistributions,
  type DistributionAllowlist,
  type StockDistributionRow,
} from './stockDistribution';

/**
 * Real ops receiving rail (6 stages):
 * 1 Kamyon & Kanıt → 2 Malzeme kontrolü → 3 Fiziksel sayım → 4 Karşılaştırma → 5 Sonuç → 6 Stoklaştırma
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
  const { user } = useAuth();
  // Rule 1 — default posting context is HomeFactory (homePlantId).
  const homePlantId = user?.homePlantId || user?.plantId || 'PLANT-001';
  const postingPlantId = user?.plantId || homePlantId;
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

  const [materialCheck, setMaterialCheck] = useState<MaterialCheckState>(() => createDefaultMaterialCheck());
  const [incomingLines, setIncomingLines] = useState<IncomingLine[]>([]);
  const [compareResolved, setCompareResolved] = useState(false);
  const [warehouse, setWarehouse] = useState('');
  const [location, setLocation] = useState('');
  const [distributions, setDistributions] = useState<StockDistributionRow[]>([]);
  const [grNumber] = useState(() => mintPreview('GR'));
  const [sharedLot] = useState(() => mintLotNumber());
  const [lotNote, setLotNote] = useState('');
  const [lotTracking, setLotTracking] = useState(true);

  const warehousesQuery = useQuery({
    queryKey: ['business', 'warehouses', 'rcv-wb', postingPlantId],
    queryFn: () =>
      searchAllResource<{ code?: string; status?: string }>('warehouses', undefined, {
        plantId: postingPlantId,
      }),
  });
  const locationsQuery = useQuery({
    queryKey: ['business', 'locations', 'rcv-wb', postingPlantId],
    queryFn: () =>
      searchAllResource<{ code?: string; warehouseCode?: string; status?: string }>(
        'locations',
        undefined,
        { plantId: postingPlantId },
      ),
  });

  const postingAllowlist: DistributionAllowlist | null = useMemo(() => {
    if (warehousesQuery.isLoading || locationsQuery.isLoading) return null;
    const warehouseCodes = new Set(
      (warehousesQuery.data ?? [])
        .filter((w) => String(w.status ?? 'Active').toLowerCase() === 'active')
        .map((w) => String(w.code ?? '').trim().toUpperCase())
        .filter(Boolean),
    );
    const locationsByWarehouse = new Map<string, ReadonlySet<string>>();
    for (const loc of locationsQuery.data ?? []) {
      if (String(loc.status ?? 'Active').toLowerCase() !== 'active') continue;
      const wh = String(loc.warehouseCode ?? '').trim().toUpperCase();
      const code = String(loc.code ?? '').trim().toUpperCase();
      if (!wh || !code) continue;
      const set = new Set(locationsByWarehouse.get(wh) ?? []);
      set.add(code);
      locationsByWarehouse.set(wh, set);
    }
    return { warehouseCodes, locationsByWarehouse };
  }, [warehousesQuery.data, warehousesQuery.isLoading, locationsQuery.data, locationsQuery.isLoading]);

  useEffect(() => {
    if (warehouse.trim() && location.trim()) return;
    const whs = (warehousesQuery.data ?? []).filter(
      (w) => String(w.status ?? 'Active').toLowerCase() === 'active' && String(w.code ?? '').trim(),
    );
    if (whs.length === 0) return;
    const preferred =
      whs.find((w) => String(w.code).toUpperCase() === 'WH-RM') ?? whs[0]!;
    const whCode = String(preferred.code);
    const locs = (locationsQuery.data ?? []).filter(
      (l) =>
        String(l.status ?? 'Active').toLowerCase() === 'active' &&
        String(l.warehouseCode ?? '').toUpperCase() === whCode.toUpperCase() &&
        String(l.code ?? '').trim(),
    );
    if (!warehouse.trim()) setWarehouse(whCode);
    if (!location.trim() && locs[0]?.code) setLocation(String(locs[0].code));
  }, [warehousesQuery.data, locationsQuery.data, warehouse, location]);

  const preAccept = syncBatchPreAccept(incomingLines);
  const countable = useMemo(() => countableLines(incomingLines), [incomingLines]);
  const quantityVerified = allCountableCounted(incomingLines);
  const stockAcceptedLines = useMemo(() => acceptedStockLines(incomingLines), [incomingLines]);
  const matchConfirmed = stockAcceptedLines.length > 0 && stockAcceptedLines.every(lineMaterialMatched);
  const primaryStockLine = useMemo(() => {
    return stockAcceptedLines.find((l) => l.kind === 'lumber') ?? stockAcceptedLines[0] ?? null;
  }, [stockAcceptedLines]);
  const matchedMaterialCode = primaryStockLine?.matchedMaterialCode ?? '';
  const confirmedMatchScore = primaryStockLine?.matchScore ?? null;
  const stockQty = stockAcceptedLines.reduce((s, l) => s + stockBasisQty(l), 0);
  const stockVol = stockAcceptedLines.reduce((s, l) => s + (stockBasisVolumeM3(l) ?? 0), 0);
  const countQty = String(stockQty || finalPhysicalQtyFromLines(incomingLines) || '');
  const distValidation = useMemo(
    () => validateDistributions(incomingLines, distributions, postingAllowlist),
    [incomingLines, distributions, postingAllowlist],
  );

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

  const stage = STAGES[stageIdx];
  const progress = Math.round(((stageIdx + (posted ? 1 : 0)) / STAGES.length) * 100);
  const photoCount = Object.values(photos).filter(Boolean).length;

  const postBlockedReason = useMemo(() => {
    if (posted) return null;
    if (!matchConfirmed || stockAcceptedLines.length === 0) {
      return t('wb.rcv.gateNeedMaterialConfirm');
    }
    if (!quantityVerified) return t('wb.rcv.gateNeedQtyVerify');
    if (Number(countQty) <= 0) return t('wb.rcv.gateNeedCount');
    if (!approved) return t('wb.rcv.gateNeedApprove');
    if (!distValidation.ok) {
      if (distValidation.code === 'over') return t('wb.rcv.stockStep.errOver');
      if (distValidation.code === 'under') return t('wb.rcv.stockStep.errUnder');
      if (distValidation.code === 'missingWh') return t('wb.rcv.stockStep.errWh');
      if (distValidation.code === 'invalidWh') {
        return 'Seçilen depo bu tesisin aktif depoları arasında değil.';
      }
      if (distValidation.code === 'invalidLoc') {
        return 'Seçilen lokasyon, seçili deponun aktif lokasyonları arasında değil.';
      }
      return t('wb.rcv.gateNeedWh');
    }
    return null;
  }, [
    posted,
    matchConfirmed,
    stockAcceptedLines.length,
    quantityVerified,
    countQty,
    approved,
    distValidation,
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
        if (incomingLines.length === 0) return t('wb.rcv.ops.check.noIncomingYet');
        if (!allLinesReadyForCount(incomingLines)) return t('wb.rcv.ops.check.gateNeedPerLineMaterial');
        if (materialCheck.qualityVerdict === 'none') return t('wb.rcv.ops.check.gateNeedQuality');
        if (preAccept === 'reject') return t('wb.rcv.ops.gatePreAcceptReject');
        if (countable.length === 0) return t('wb.rcv.ops.gateAllRejected');
        return null;
      case 'physicalCount':
        if (countable.length === 0) return t('wb.rcv.ops.count.noCountable');
        if (!allCountableCounted(incomingLines)) return t('wb.rcv.ops.count.needAllCounted');
        return null;
      case 'compare':
        return !compareResolved ? t('wb.rcv.compare.gateNeedResolve') : null;
      case 'result':
        if (!matchConfirmed || !quantityVerified) return t('wb.rcv.gateNeedMaterialConfirm');
        if (!approved) return t('wb.rcv.gateNeedApprove');
        return null;
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
    incomingLines,
    materialCheck.qualityVerdict,
    preAccept,
    countable.length,
    compareResolved,
    approved,
    matchConfirmed,
    quantityVerified,
    postBlockedReason,
    t,
  ]);

  const canAdvance = !gateMessage && !posted;

  function updateIncomingLines(next: IncomingLine[]) {
    setIncomingLines(next);
    setApproved(false);
    setDistributions([]);
  }

  function ensureDistributions() {
    setDistributions((prev) => {
      if (prev.length > 0) return prev;
      return buildDefaultDistributions(incomingLines, warehouse, location);
    });
  }

  useEffect(() => {
    if (stage.id === 'stock' && !posted && distributions.length === 0 && stockAcceptedLines.length > 0) {
      setDistributions(buildDefaultDistributions(incomingLines, warehouse, location));
    }
  }, [stage.id, posted, distributions.length, stockAcceptedLines.length, incomingLines, warehouse, location]);

  function toggleDoc(kind: EvidenceDocKind) {
    setDocs((d) => (d.includes(kind) ? d.filter((x) => x !== kind) : [...d, kind]));
  }

  function togglePhoto(slot: PhotoSlot) {
    setPhotos((p) => ({ ...p, [slot]: !p[slot] }));
  }

  const persistMutation = useMutation({
    mutationFn: async () => {
      // RULE 8 / 23 — already posted: do not create a second stock movement
      if (posted) {
        return {
          documentId: '',
          documentNumber: minted.gr || grNumber,
          status: 'Posted',
          lines: [] as Array<{
            materialCode: string;
            materialIdentityNumber: string;
            packageNumber: string;
            lotNumber: string;
            movementNumber: string;
            quantity: number;
          }>,
          idempotent: true,
        };
      }
      if (!matchConfirmed || !quantityVerified || !approved) {
        throw new Error(t('wb.rcv.gateNeedMaterialConfirm'));
      }
      const validation = validateDistributions(incomingLines, distributions, postingAllowlist);
      if (!validation.ok) {
        if (validation.code === 'over') throw new Error(t('wb.rcv.stockStep.errOver'));
        if (validation.code === 'under') throw new Error(t('wb.rcv.stockStep.errUnder'));
        if (validation.code === 'missingWh') throw new Error(t('wb.rcv.stockStep.errWh'));
        if (validation.code === 'invalidWh') {
          throw new Error('Seçilen depo bu tesisin aktif depoları arasında değil.');
        }
        if (validation.code === 'invalidLoc') {
          throw new Error('Seçilen lokasyon, seçili deponun aktif lokasyonları arasında değil.');
        }
        throw new Error(t('wb.rcv.gateNeedWh'));
      }

      const postLines = buildExecuteLines(incomingLines, distributions, sharedLot);
      if (postLines.length === 0) throw new Error(t('wb.rcv.stockStep.noAccepted'));
      const headerWh = postLines[0]!.warehouseCode;

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
        idempotent?: boolean;
      }>('goods-receipts/execute', {
        warehouseCode: headerWh,
        reference: truck.plate || 'MANUAL',
        quantityVerified: true,
        extractSource: 'manual',
        plantId: postingPlantId,
        notes: [
          `lot=${sharedLot}`,
          `lotTracking=${lotTracking}`,
          lotNote ? `lotNote=${lotNote}` : null,
          `pipeline=truckEvidence>materialCheck>physicalCount>compare>result>stock`,
          `compareResolved=${compareResolved}`,
          `supplier=${truck.supplier}`,
          `plate=${truck.plate}`,
          `preAccept=${preAccept}`,
          `stockBasisQtyTotal=${stockQty}`,
          `stockBasisVolumeM3Total=${stockVol > 0 ? stockVol.toFixed(4) : ''}`,
          `dist=${distributions
            .map(
              (d) =>
                `${d.lineId}|g=${d.groupId ?? '-'}|q=${d.qty}|wh=${d.warehouseCode}|loc=${d.locationCode}|b=${d.bucket}`,
            )
            .join(',')}`,
          `countedLines=${stockAcceptedLines
            .map(
              (l) =>
                `${l.name}|card=${l.matchedMaterialCode}|doc=${formatLineDims(l)}|phys=${formatPhysicalDims(l)}|q=${finalLineQty(l)}|m3=${linePhysicalVolumeM3(l)?.toFixed(3) ?? ''}|sa=${l.stockAccept}`,
            )
            .join(',')}`,
          `rejected=${incomingLines
            .filter((l) => l.preAccept === 'reject' || l.stockAccept === 'reject')
            .map((l) => l.name)
            .join(',')}`,
          `quality=${materialCheck.qualityVerdict}`,
          `matchScore=${confirmedMatchScore ?? ''}`,
        ]
          .filter(Boolean)
          .join('; '),
        number: grNumber,
        lines: postLines.map((p) => ({
          materialCode: p.materialCode,
          materialId: p.materialId,
          warehouseCode: p.warehouseCode,
          locationCode: p.locationCode,
          lotNumber: p.lotNumber,
          packageNumber: p.packageNumber,
          materialIdentityNumber: p.materialIdentityNumber,
          quantity: p.quantity,
          unitOfMeasure: p.unitOfMeasure,
          barcode: p.barcode,
          stockStatus: p.stockStatus,
          actualThicknessMm: p.actualThicknessMm ?? undefined,
          actualWidthMm: p.actualWidthMm ?? undefined,
          actualLengthMm: p.actualLengthMm ?? undefined,
        })),
      });
    },
    onSuccess: async (created) => {
      setError(null);
      const line = created.lines?.[0];
      setMinted((m) => ({
        ...m,
        gr: created.documentNumber || grNumber,
        mi: line?.materialIdentityNumber || m.mi,
        lot: line?.lotNumber || sharedLot || m.lot,
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
      if (posted) return;
      if (postBlockedReason) {
        setError(postBlockedReason);
        return;
      }
      persistMutation.mutate();
      return;
    }
    if (stage.id === 'truckEvidence' && incomingLines.length === 0) {
      setIncomingLines(seedIncomingFromDocuments());
    }
    if (stage.id === 'result') {
      ensureDistributions();
    }
    const next = Math.min(stageIdx + 1, STAGES.length - 1);
    setStageIdx(next);
    setMaxReached((m) => Math.max(m, next));
    if (STAGES[next]?.id === 'stock') {
      ensureDistributions();
    }
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
            <p className="font-mono text-sm font-medium">{minted.gr ?? grNumber}</p>
            <p className="font-mono text-xs text-[var(--text-secondary)]">
              {lotTracking ? minted.lot || sharedLot : t('wb.rcv.lotOff')}
            </p>
            <p className="text-[10px] text-[var(--text-muted)]">GR · LOT</p>
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
                <div className="space-y-5">
                  <IncomingLineCheckPanel
                    lines={incomingLines}
                    onChange={updateIncomingLines}
                    materialCandidates={materialCandidates}
                    disabled={posted}
                    onRefreshMaterials={() => {
                      void materialsQuery.refetch();
                    }}
                  />
                  <MaterialCheckStep
                    value={materialCheck}
                    onChange={setMaterialCheck}
                    disabled={posted}
                    hidePreAccept
                    hideMoistureAndDims
                  />
                </div>
              ) : null}

              {stage.id === 'physicalCount' ? (
                <PhysicalCountStep
                  lines={incomingLines}
                  onChange={updateIncomingLines}
                  disabled={posted}
                />
              ) : null}

              {stage.id === 'compare' ? (
                <CompareStep
                  lines={incomingLines}
                  onChange={updateIncomingLines}
                  supplier={truck.supplier}
                  disabled={posted}
                  onResolvedChange={setCompareResolved}
                  onRequestMaterialMatch={(lineId) => {
                    const idx = STAGES.findIndex((s) => s.id === 'materialCheck');
                    if (idx >= 0) {
                      setStageIdx(idx);
                      setMaxReached((m) => Math.max(m, idx));
                    }
                    void lineId;
                  }}
                />
              ) : null}

              {stage.id === 'result' ? (
                <ResultStep
                  truck={truck}
                  docsCount={docs.length}
                  photoCount={photoCount}
                  lines={incomingLines}
                  materialCheck={materialCheck}
                  warehouse={warehouse}
                  location={location}
                  onWarehouseChange={setWarehouse}
                  onLocationChange={setLocation}
                  approved={approved}
                  onApprovedChange={(v) => {
                    setApproved(v);
                    if (v) {
                      setDistributions(buildDefaultDistributions(incomingLines, warehouse, location));
                    }
                  }}
                  canApprove={matchConfirmed && quantityVerified}
                  disabled={posted}
                />
              ) : null}

              {stage.id === 'stock' ? (
                <StockStep
                  lines={incomingLines}
                  distributions={distributions}
                  onDistributionsChange={setDistributions}
                  posted={posted}
                  postedDocumentNumber={minted.gr || grNumber}
                  postBlockedReason={postBlockedReason}
                  disabled={posted}
                  lotNumber={sharedLot}
                  lotTracking={lotTracking}
                  onLotTrackingChange={setLotTracking}
                  lotNote={lotNote}
                  onLotNoteChange={setLotNote}
                />
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
                <dd className="font-medium">
                  {matchConfirmed
                    ? `${stockAcceptedLines.length} · ${matchedMaterialCode || '—'}`
                    : t('wb.rcv.noMaterialMatched')}
                </dd>
              </div>
              <div>
                <dt className="text-[var(--text-muted)]">{t('wb.rcv.countedQty')}</dt>
                <dd className="font-medium">
                  {countQty}
                  {quantityVerified ? ` · ${t('wb.rcv.qtyVerifiedShort')}` : ''}
                  {stockVol > 0 ? ` · ${stockVol.toFixed(3)} m³` : ''}
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
                  if (posted) return;
                  if (distributions.length === 0) {
                    setDistributions(buildDefaultDistributions(incomingLines, warehouse, location));
                  }
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

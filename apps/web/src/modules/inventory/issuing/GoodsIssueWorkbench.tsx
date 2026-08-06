import { Link, useNavigate } from '@tanstack/react-router';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { Fragment, useMemo, useState, type DragEvent } from 'react';
import { Button, Card, CardContent, CardDescription, CardHeader, CardTitle, Input } from '@naswood/ui';
import { createResource } from '@/api/business';
import { useI18n } from '@/i18n';

type StageId =
  | 'document'
  | 'materials'
  | 'aiPick'
  | 'picking'
  | 'verify'
  | 'evidence'
  | 'quality'
  | 'destination'
  | 'review'
  | 'post';

interface StageDef {
  id: StageId;
  titleKey: string;
  hintKey: string;
}

const STAGES: StageDef[] = [
  { id: 'document', titleKey: 'wb.iss.document', hintKey: 'wb.iss.documentHint' },
  { id: 'materials', titleKey: 'wb.iss.materials', hintKey: 'wb.iss.materialsHint' },
  { id: 'aiPick', titleKey: 'wb.iss.aiPick', hintKey: 'wb.iss.aiPickHint' },
  { id: 'picking', titleKey: 'wb.iss.allocWorkspace', hintKey: 'wb.iss.allocWorkspaceHint' },
  { id: 'verify', titleKey: 'wb.iss.verify', hintKey: 'wb.iss.verifyHint' },
  { id: 'evidence', titleKey: 'wb.iss.evidence', hintKey: 'wb.iss.evidenceHint' },
  { id: 'quality', titleKey: 'wb.iss.quality', hintKey: 'wb.iss.qualityHint' },
  { id: 'destination', titleKey: 'wb.iss.destination', hintKey: 'wb.iss.destinationHint' },
  { id: 'review', titleKey: 'wb.iss.review', hintKey: 'wb.iss.reviewHint' },
  { id: 'post', titleKey: 'wb.iss.post', hintKey: 'wb.iss.postHint' },
];

type DocType = 'production' | 'maintenance' | 'sales' | 'sample' | 'internal' | 'rnd' | 'scrap' | 'transfer' | 'manual';

const DOC_OPTIONS: { id: DocType; ref: string; owner: string; priority: string; due: string; customer?: string }[] = [
  { id: 'sales', ref: 'SO-250001', owner: 'Satış', priority: 'Yüksek', due: '2026-08-06', customer: 'Nordic Deck AS' },
  { id: 'production', ref: 'PO-2026-0142', owner: 'Planlama', priority: 'Yüksek', due: '2026-08-06' },
  { id: 'maintenance', ref: 'WO-2026-0088', owner: 'Bakım', priority: 'Normal', due: '2026-08-07' },
  { id: 'sample', ref: 'SMP-2026-0012', owner: 'Kalite', priority: 'Düşük', due: '2026-08-08' },
  { id: 'manual', ref: 'MANUAL', owner: 'Depo', priority: '—', due: '—' },
];

const MATERIAL_LINES = [
  {
    name: 'Thermowood Deck 26×140×4000',
    required: 50,
    reserved: 50,
    available: 86,
    alt: 'Thermowood Deck 26×140×3000',
    unit: 'Paket',
  },
];

type AllocRow = {
  code: string;
  warehouse: string;
  location: string;
  lot: string;
  mi: string;
  species: string;
  dimensions: string;
  quality: string;
  moisture: string;
  available: number;
  selected: number;
  /** m³ per piece */
  unitVolume: number;
  /** kg per piece */
  unitWeight: number;
};

type SortKey = 'code' | 'location' | 'lot' | 'quality' | 'moisture' | 'available' | 'selected';
type GroupKey = 'none' | 'warehouse' | 'lot' | 'quality';

const REQUIRED_QTY = 50;

const AI_RULES = [
  'FIFO/FEFO',
  'Müşteri',
  'Rezervasyon',
  'Kalite',
  'Lot tutarlılığı',
  'Nem tutarlılığı',
  'Ölçü tutarlılığı',
  'Tür tutarlılığı',
  'Depo optimizasyonu',
];

/** Catalog of pickable packages (Explorer / Add). */
const UV = 0.0437; // m³ / piece for 26×140×4000 approx
const UW = 18.5; // kg / piece demo

const PACKAGE_CATALOG: AllocRow[] = [
  {
    code: 'PKG-A-2026-000120',
    warehouse: 'Ana Mamul Deposu',
    location: 'A-01-02',
    lot: 'LOT-TW-2026-000042',
    mi: 'FG-TWDECK-20260801-00012',
    species: 'Pine Thermowood',
    dimensions: '26×140×4000',
    quality: 'A',
    moisture: '%6',
    available: 20,
    selected: 20,
    unitVolume: UV,
    unitWeight: UW,
  },
  {
    code: 'PKG-B-2026-000088',
    warehouse: 'Ana Mamul Deposu',
    location: 'A-01-04',
    lot: 'LOT-TW-2026-000042',
    mi: 'FG-TWDECK-20260801-00018',
    species: 'Pine Thermowood',
    dimensions: '26×140×4000',
    quality: 'A',
    moisture: '%6',
    available: 15,
    selected: 15,
    unitVolume: UV,
    unitWeight: UW,
  },
  {
    code: 'PKG-C-2026-000091',
    warehouse: 'Ana Mamul Deposu',
    location: 'A-02-01',
    lot: 'LOT-TW-2026-000042',
    mi: 'FG-TWDECK-20260801-00022',
    species: 'Pine Thermowood',
    dimensions: '26×140×4000',
    quality: 'A',
    moisture: '%6',
    available: 15,
    selected: 15,
    unitVolume: UV,
    unitWeight: UW,
  },
  {
    code: 'PKG-00254',
    warehouse: 'Ana Mamul Deposu',
    location: 'A-03-02',
    lot: 'LOT-TW-2026-000042',
    mi: 'FG-TWDECK-20260801-00030',
    species: 'Pine Thermowood',
    dimensions: '26×140×4000',
    quality: 'A',
    moisture: '%6',
    available: 120,
    selected: 40,
    unitVolume: UV,
    unitWeight: UW,
  },
  {
    code: 'PKG-D-2026-000210',
    warehouse: 'Ana Mamul Deposu',
    location: 'B-03-01',
    lot: 'LOT-TW-2026-000099',
    mi: 'FG-TWDECK-20260715-00008',
    species: 'Pine Thermowood',
    dimensions: '26×140×3000',
    quality: 'B',
    moisture: '%8',
    available: 20,
    selected: 20,
    unitVolume: 0.0328,
    unitWeight: 14.2,
  },
];

/** Minimum package set for SO-250001 (50). */
const AI_ALLOCATION: AllocRow[] = PACKAGE_CATALOG.slice(0, 3).map((p) => ({ ...p }));

function remainingOf(row: AllocRow) {
  return Math.max(0, row.available - row.selected);
}

function mixWarnings(rows: AllocRow[]): string[] {
  if (rows.length === 0) return [];
  const warns: string[] = [];
  const lots = new Set(rows.map((r) => r.lot));
  const qualities = new Set(rows.map((r) => r.quality));
  const moistures = new Set(rows.map((r) => r.moisture));
  const dims = new Set(rows.map((r) => r.dimensions));
  if (lots.size > 1) warns.push('mixLots');
  if (qualities.size > 1) warns.push('mixQuality');
  if (moistures.size > 1) warns.push('mixMoisture');
  if (dims.size > 1) warns.push('mixDimensions');
  if (rows.some((r) => r.code.startsWith('PKG-D'))) warns.push('mixCustomer');
  return warns;
}

function mintPreview(prefix: string) {
  const seq = new Date().toISOString().slice(2, 10).replace(/-/g, '') + '-' + String(Math.floor(100000 + Math.random() * 900000));
  return `${prefix}-${seq}`;
}

/** INV-ISS-001 Goods Issue Workbench — not a Create/CRUD form. */
export function GoodsIssueWorkbench() {
  const { t } = useI18n();
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const [stageIdx, setStageIdx] = useState(0);
  const [maxReached, setMaxReached] = useState(0);
  const [posted, setPosted] = useState(false);
  const [mintedGi, setMintedGi] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [approved, setApproved] = useState(false);

  const [docType, setDocType] = useState<DocType | null>('sales');
  const [manualReason, setManualReason] = useState('');
  const [aiDecision, setAiDecision] = useState<'none' | 'accept' | 'ignore'>('none');
  const [overrideOpen, setOverrideOpen] = useState(false);
  const [allocation, setAllocation] = useState<AllocRow[]>([]);
  const [overrideHistory, setOverrideHistory] = useState<string[]>([]);
  const [mixWaived, setMixWaived] = useState(false);
  const [scanValue, setScanValue] = useState('');
  const [scannedOk, setScannedOk] = useState(false);
  const [focusedCode, setFocusedCode] = useState<string | null>(null);
  const [bulkSelected, setBulkSelected] = useState<string[]>([]);
  const [filterText, setFilterText] = useState('');
  const [sortKey, setSortKey] = useState<SortKey>('location');
  const [groupKey, setGroupKey] = useState<GroupKey>('none');
  const [verifyOk, setVerifyOk] = useState(false);
  const [evidence, setEvidence] = useState<string[]>([]);
  const [qualityOk, setQualityOk] = useState(false);
  const [destType, setDestType] = useState<'production' | 'shipping'>('shipping');
  const [destination, setDestination] = useState('Rampa 2 · Araç 34 ABC 123');

  const stage = STAGES[stageIdx];
  const selectedDoc = DOC_OPTIONS.find((d) => d.id === docType) ?? null;
  const progress = Math.round(((stageIdx + (posted ? 1 : 0)) / STAGES.length) * 100);
  const selectedTotal = useMemo(() => allocation.reduce((s, r) => s + r.selected, 0), [allocation]);
  const remainingTotal = useMemo(() => allocation.reduce((s, r) => s + remainingOf(r), 0), [allocation]);
  const selectedVolume = useMemo(
    () => Number(allocation.reduce((s, r) => s + r.selected * r.unitVolume, 0).toFixed(3)),
    [allocation],
  );
  const selectedWeight = useMemo(
    () => Number(allocation.reduce((s, r) => s + r.selected * r.unitWeight, 0).toFixed(1)),
    [allocation],
  );
  const remainingVolume = useMemo(
    () => Number(allocation.reduce((s, r) => s + remainingOf(r) * r.unitVolume, 0).toFixed(3)),
    [allocation],
  );
  const packageCount = useMemo(() => allocation.filter((r) => r.selected > 0).length, [allocation]);
  const warnings = useMemo(() => mixWarnings(allocation), [allocation]);
  const addablePackages = useMemo(
    () => PACKAGE_CATALOG.filter((p) => !allocation.some((a) => a.code === p.code)),
    [allocation],
  );
  const viewRows = useMemo(() => {
    const q = filterText.trim().toLowerCase();
    let rows = allocation.filter((r) => {
      if (!q) return true;
      return (
        r.code.toLowerCase().includes(q) ||
        r.lot.toLowerCase().includes(q) ||
        r.location.toLowerCase().includes(q) ||
        r.quality.toLowerCase().includes(q) ||
        r.moisture.toLowerCase().includes(q) ||
        r.warehouse.toLowerCase().includes(q)
      );
    });
    rows = [...rows].sort((a, b) => {
      const av = a[sortKey];
      const bv = b[sortKey];
      if (typeof av === 'number' && typeof bv === 'number') return av - bv;
      return String(av).localeCompare(String(bv), 'tr');
    });
    return rows;
  }, [allocation, filterText, sortKey]);

  const gateMessage = useMemo(() => {
    switch (stage.id) {
      case 'document':
        if (!docType) return t('wb.iss.gateNeedDoc');
        if (docType === 'manual' && !manualReason.trim()) return t('wb.iss.gateNeedManualReason');
        return null;
      case 'materials':
        return null;
      case 'aiPick':
        if (aiDecision === 'none') return t('wb.iss.gateNeedAi');
        if (allocation.length === 0) return t('wb.iss.gateNeedAlloc');
        return null;
      case 'picking':
        if (allocation.length === 0) return t('wb.iss.gateNeedAlloc');
        if (allocation.some((r) => r.selected <= 0 || r.selected > r.available)) return t('wb.iss.gateNeedPartialQty');
        if (selectedTotal !== REQUIRED_QTY) return t('wb.iss.gateNeedTotal');
        if (warnings.length > 0 && !mixWaived) return t('wb.iss.gateNeedMixAuth');
        return !scannedOk ? t('wb.iss.gateNeedScan') : null;
      case 'verify':
        return !verifyOk ? t('wb.iss.gateNeedVerify') : null;
      case 'evidence':
        return null;
      case 'quality':
        return !qualityOk ? t('wb.iss.gateNeedQuality') : null;
      case 'destination':
        return !destination.trim() ? t('wb.iss.gateNeedDest') : null;
      case 'review':
        return !approved ? t('wb.iss.gateNeedApprove') : null;
      default:
        return null;
    }
  }, [
    stage.id,
    docType,
    manualReason,
    aiDecision,
    allocation,
    selectedTotal,
    warnings,
    mixWaived,
    scannedOk,
    verifyOk,
    qualityOk,
    destination,
    approved,
    t,
  ]);

  const canAdvance = !gateMessage && !posted;

  const persistMutation = useMutation({
    mutationFn: async () => {
      return createResource<Record<string, unknown>>('goods-issues', {
        warehouseCode: 'WH-RM',
        reference: selectedDoc?.ref || 'MANUAL',
        status: 'Posted',
        notes: [
          `docType=${docType}`,
          `ai=${aiDecision}`,
          `alloc=${allocation.map((r) => `${r.code}:${r.selected}/${r.available}`).join(',')}`,
          `selectedTotal=${selectedTotal}`,
          `mix=${warnings.join(',') || 'ok'}`,
          `mixWaived=${mixWaived}`,
          `dest=${destination}`,
          `scan=${scanValue || 'OK'}`,
          `evidence=${evidence.length}`,
          overrideHistory.length ? `overrides=${overrideHistory.join('|')}` : '',
          manualReason ? `manualReason=${manualReason}` : '',
        ]
          .filter(Boolean)
          .join('; '),
        number: '',
      });
    },
    onSuccess: async (created) => {
      setError(null);
      const gi =
        (typeof created.number === 'string' && created.number) ||
        (typeof created.code === 'string' && created.code) ||
        mintPreview('GI');
      setMintedGi(gi);
      setPosted(true);
      await queryClient.invalidateQueries({ queryKey: ['business', 'goods-issues'] });
    },
    onError: (e: Error) => setError(e.message),
  });

  function goNext() {
    if (stage.id === 'post') {
      persistMutation.mutate();
      return;
    }
    const next = Math.min(stageIdx + 1, STAGES.length - 1);
    setStageIdx(next);
    setMaxReached((m) => Math.max(m, next));
  }

  function addEvidence(kind: string) {
    setEvidence((e) => (e.includes(kind) ? e : [...e, kind]));
  }

  function acceptAi() {
    setAiDecision('accept');
    setOverrideOpen(false);
    setAllocation(AI_ALLOCATION.map((p) => ({ ...p })));
    setMixWaived(false);
    setError(null);
  }

  function ignoreAi() {
    setAiDecision('ignore');
    setOverrideOpen(true);
    setAllocation([]);
    setMixWaived(false);
    setError(null);
  }

  function addPackage(code: string) {
    const src = PACKAGE_CATALOG.find((p) => p.code === code);
    if (!src || allocation.some((a) => a.code === code)) return;
    setAllocation((rows) => [...rows, { ...src }]);
    setOverrideHistory((h) => [...h, `${new Date().toISOString().slice(11, 19)} Add → ${code}`]);
    setMixWaived(false);
    setError(null);
  }

  function removePackage(code: string) {
    setAllocation((rows) => rows.filter((r) => r.code !== code));
    setOverrideHistory((h) => [...h, `${new Date().toISOString().slice(11, 19)} Remove → ${code}`]);
    setMixWaived(false);
  }

  function setSelectedQty(code: string, selected: number) {
    setAllocation((rows) =>
      rows.map((r) => (r.code === code ? { ...r, selected: Math.min(Math.max(0, selected), r.available) } : r)),
    );
    setMixWaived(false);
  }

  function resetToAi() {
    setAiDecision('accept');
    setAllocation(AI_ALLOCATION.map((p) => ({ ...p })));
    setOverrideOpen(false);
    setMixWaived(false);
  }

  function tryScan() {
    const v = scanValue.trim().toUpperCase();
    if (!v) {
      setScannedOk(false);
      return;
    }
    const match = allocation.find(
      (r) => r.code.toUpperCase() === v || r.code.toUpperCase().includes(v) || v.includes(r.code.toUpperCase().slice(0, 10)),
    );
    const inCatalog = PACKAGE_CATALOG.find(
      (r) => r.code.toUpperCase() === v || r.code.toUpperCase().includes(v),
    );
    if (match) {
      setFocusedCode(match.code);
      setScannedOk(true);
      setError(null);
      return;
    }
    if (inCatalog) {
      addPackage(inCatalog.code);
      setFocusedCode(inCatalog.code);
      setScannedOk(true);
      setError(null);
      return;
    }
    setScannedOk(false);
    setError(t('wb.iss.scanMismatch'));
  }

  function toggleBulk(code: string) {
    setBulkSelected((s) => (s.includes(code) ? s.filter((c) => c !== code) : [...s, code]));
  }

  function bulkRemove() {
    bulkSelected.forEach(removePackage);
    setBulkSelected([]);
  }

  function bulkZero() {
    bulkSelected.forEach((c) => setSelectedQty(c, 0));
  }

  function onDropPackage(e: DragEvent) {
    e.preventDefault();
    const code = e.dataTransfer.getData('text/pkg');
    if (code) addPackage(code);
  }

  function LiveTotalsStrip() {
    const deficit = REQUIRED_QTY - selectedTotal;
    return (
      <div className="grid grid-cols-2 gap-2 sm:grid-cols-3 lg:grid-cols-6">
        {[
          [t('wb.iss.required'), String(REQUIRED_QTY)],
          [t('wb.iss.colSelected'), String(selectedTotal)],
          [t('wb.iss.remaining'), String(Math.max(0, deficit))],
          [t('wb.iss.metricVolume'), `${selectedVolume} m³`],
          [t('wb.iss.metricWeight'), `${selectedWeight} kg`],
          [t('wb.iss.metricPkgCount'), String(packageCount)],
        ].map(([label, value]) => (
          <div key={label} className="rounded-md border border-[var(--border-default)] bg-[var(--color-surface)] px-2.5 py-2">
            <p className="text-[10px] uppercase tracking-wide text-[var(--text-muted)]">{label}</p>
            <p
              className={`font-mono text-sm font-semibold tabular-nums ${
                label === t('wb.iss.colSelected')
                  ? selectedTotal === REQUIRED_QTY
                    ? 'text-[var(--color-primary)]'
                    : 'text-[var(--color-danger)]'
                  : ''
              }`}
            >
              {value}
            </p>
          </div>
        ))}
        <p className="col-span-full text-[10px] text-[var(--text-muted)]">
          {t('wb.iss.remainVolShort')}: {remainingVolume} m³ · {t('wb.iss.inventorySync')}
        </p>
      </div>
    );
  }

  function PackageAllocationWorkspace({ editable }: { editable: boolean }) {
    const grouped =
      groupKey === 'none'
        ? [{ key: '', rows: viewRows }]
        : Object.entries(
            viewRows.reduce<Record<string, AllocRow[]>>((acc, r) => {
              const k = groupKey === 'warehouse' ? r.warehouse : groupKey === 'lot' ? r.lot : r.quality;
              (acc[k] ??= []).push(r);
              return acc;
            }, {}),
          ).map(([key, rows]) => ({ key, rows }));

    return (
      <div
        className="space-y-3 rounded-lg border-2 border-[var(--color-primary)]/35 bg-[var(--color-surface)] p-3 shadow-sm"
        onDragOver={(e) => e.preventDefault()}
        onDrop={onDropPackage}
      >
        <div className="flex flex-wrap items-start justify-between gap-2">
          <div>
            <p className="text-[10px] font-semibold uppercase tracking-wide text-[var(--color-primary)]">
              ★ {t('wb.iss.allocWorkspace')}
            </p>
            <p className="text-xs text-[var(--text-secondary)]">{t('wb.iss.allocWorkspaceHint')}</p>
          </div>
          <p className="text-[10px] text-[var(--text-muted)]">{t('wb.iss.keyboardHints')}</p>
        </div>

        <LiveTotalsStrip />

        {editable ? (
          <div className="flex flex-wrap items-end gap-2 rounded-md border border-[var(--border-default)] bg-[var(--color-surface-hover)]/40 px-2 py-2">
            <label className="space-y-0.5 text-[11px]">
              <span className="text-[var(--text-muted)]">{t('wb.iss.filter')}</span>
              <Input
                className="h-8 w-40"
                value={filterText}
                onChange={(e) => setFilterText(e.target.value)}
                placeholder={t('wb.iss.filterPh')}
              />
            </label>
            <label className="space-y-0.5 text-[11px]">
              <span className="text-[var(--text-muted)]">{t('wb.iss.sort')}</span>
              <select
                className="h-8 rounded-md border border-[var(--border-default)] bg-[var(--color-surface)] px-2 text-xs"
                value={sortKey}
                onChange={(e) => setSortKey(e.target.value as SortKey)}
              >
                <option value="location">{t('wb.iss.loc')}</option>
                <option value="code">{t('wb.iss.colPkgNo')}</option>
                <option value="lot">{t('wb.iss.colLot')}</option>
                <option value="quality">{t('wb.iss.colQuality')}</option>
                <option value="moisture">{t('wb.iss.colMoisture')}</option>
                <option value="available">{t('wb.iss.colAvail')}</option>
                <option value="selected">{t('wb.iss.colSelected')}</option>
              </select>
            </label>
            <label className="space-y-0.5 text-[11px]">
              <span className="text-[var(--text-muted)]">{t('wb.iss.group')}</span>
              <select
                className="h-8 rounded-md border border-[var(--border-default)] bg-[var(--color-surface)] px-2 text-xs"
                value={groupKey}
                onChange={(e) => setGroupKey(e.target.value as GroupKey)}
              >
                <option value="none">{t('wb.iss.groupNone')}</option>
                <option value="warehouse">{t('wb.iss.wh')}</option>
                <option value="lot">{t('wb.iss.colLot')}</option>
                <option value="quality">{t('wb.iss.colQuality')}</option>
              </select>
            </label>
            <div className="flex flex-wrap gap-1">
              <Button type="button" variant="secondary" className="h-8" onClick={resetToAi}>
                {t('wb.iss.resetAi')}
              </Button>
              {bulkSelected.length > 0 ? (
                <>
                  <Button type="button" variant="secondary" className="h-8" onClick={bulkZero}>
                    {t('wb.iss.bulkZero')} ({bulkSelected.length})
                  </Button>
                  <Button type="button" variant="secondary" className="h-8" onClick={bulkRemove}>
                    {t('wb.iss.bulkRemove')}
                  </Button>
                </>
              ) : null}
            </div>
          </div>
        ) : null}

        <div className="overflow-x-auto rounded-md border border-[var(--border-default)]">
          <table className="w-full min-w-[1100px] text-left text-[11px]">
            <thead className="sticky top-0 bg-[var(--color-surface-hover)] text-[var(--text-muted)]">
              <tr>
                {editable ? <th className="px-2 py-2 w-8" /> : null}
                <th className="px-2 py-2">{t('wb.iss.colPkgNo')}</th>
                <th className="px-2 py-2">{t('wb.iss.wh')}</th>
                <th className="px-2 py-2">{t('wb.iss.loc')}</th>
                <th className="px-2 py-2">{t('wb.iss.colLot')}</th>
                <th className="px-2 py-2">{t('wb.iss.colMi')}</th>
                <th className="px-2 py-2">{t('wb.iss.colSpecies')}</th>
                <th className="px-2 py-2">{t('wb.iss.colDims')}</th>
                <th className="px-2 py-2">{t('wb.iss.colQuality')}</th>
                <th className="px-2 py-2">{t('wb.iss.colMoisture')}</th>
                <th className="px-2 py-2">{t('wb.iss.colAvail')}</th>
                <th className="px-2 py-2">{t('wb.iss.colSelected')}</th>
                <th className="px-2 py-2">{t('wb.iss.colRemain')}</th>
                <th className="px-2 py-2">{t('wb.iss.metricVolume')}</th>
                <th className="px-2 py-2">{t('wb.iss.metricWeight')}</th>
                {editable ? <th className="px-2 py-2" /> : null}
              </tr>
            </thead>
            <tbody>
              {grouped.map((g) => (
                <Fragment key={g.key || 'all'}>
                  {g.key ? (
                    <tr className="bg-[var(--color-primary)]/5">
                      <td colSpan={editable ? 16 : 14} className="px-2 py-1 text-[10px] font-semibold uppercase text-[var(--text-muted)]">
                        {g.key}
                      </td>
                    </tr>
                  ) : null}
                  {g.rows.map((r) => {
                    const idx = viewRows.findIndex((x) => x.code === r.code);
                    return (
                    <tr
                      key={r.code}
                      className={`border-t border-[var(--border-default)] ${
                        focusedCode === r.code ? 'bg-[var(--color-primary)]/15 ring-1 ring-inset ring-[var(--color-primary)]' : ''
                      } ${bulkSelected.includes(r.code) ? 'bg-[var(--color-primary)]/5' : ''}`}
                      onClick={() => setFocusedCode(r.code)}
                    >
                      {editable ? (
                        <td className="px-2 py-1">
                          <input type="checkbox" checked={bulkSelected.includes(r.code)} onChange={() => toggleBulk(r.code)} />
                        </td>
                      ) : null}
                      <td className="px-2 py-1.5 font-mono font-medium">{r.code}</td>
                      <td className="px-2 py-1.5">{r.warehouse}</td>
                      <td className="px-2 py-1.5 font-mono">{r.location}</td>
                      <td className="px-2 py-1.5 font-mono">{r.lot}</td>
                      <td className="px-2 py-1.5 font-mono text-[10px]">{r.mi}</td>
                      <td className="px-2 py-1.5">{r.species}</td>
                      <td className="px-2 py-1.5">{r.dimensions}</td>
                      <td className="px-2 py-1.5">{r.quality}</td>
                      <td className="px-2 py-1.5">{r.moisture}</td>
                      <td className="px-2 py-1.5 tabular-nums">{r.available}</td>
                      <td className="px-2 py-1.5">
                        {editable ? (
                          <Input
                            type="number"
                            className="h-7 w-16"
                            min={0}
                            max={r.available}
                            value={r.selected}
                            autoFocus={focusedCode === r.code}
                            onChange={(e) => setSelectedQty(r.code, Number(e.target.value) || 0)}
                            onKeyDown={(e) => {
                              if (e.key === 'ArrowDown' || e.key === 'Enter') {
                                e.preventDefault();
                                const next = viewRows[idx + 1] ?? viewRows[0];
                                if (next) setFocusedCode(next.code);
                              }
                              if (e.key === 'ArrowUp') {
                                e.preventDefault();
                                const prev = viewRows[idx - 1] ?? viewRows[viewRows.length - 1];
                                if (prev) setFocusedCode(prev.code);
                              }
                              if (e.key === 'Delete') {
                                e.preventDefault();
                                removePackage(r.code);
                              }
                            }}
                          />
                        ) : (
                          <span className="tabular-nums font-medium">{r.selected}</span>
                        )}
                      </td>
                      <td className="px-2 py-1.5 tabular-nums text-[var(--color-primary)]">{remainingOf(r)}</td>
                      <td className="px-2 py-1.5 tabular-nums">{(r.selected * r.unitVolume).toFixed(3)}</td>
                      <td className="px-2 py-1.5 tabular-nums">{(r.selected * r.unitWeight).toFixed(1)}</td>
                      {editable ? (
                        <td className="px-2 py-1.5">
                          <Button type="button" variant="secondary" className="h-7 px-2 text-[10px]" onClick={() => removePackage(r.code)}>
                            {t('wb.iss.removePkg')}
                          </Button>
                        </td>
                      ) : null}
                    </tr>
                    );
                  })}
                </Fragment>
              ))}
              <tr className="border-t border-[var(--border-default)] bg-[var(--color-surface-hover)] font-medium">
                <td className="px-2 py-2" colSpan={editable ? 10 : 9}>
                  {t('wb.iss.total')} · {t('wb.iss.required')}: {REQUIRED_QTY}
                </td>
                <td className="px-2 py-2 tabular-nums">{allocation.reduce((s, r) => s + r.available, 0)}</td>
                <td
                  className={`px-2 py-2 tabular-nums ${selectedTotal === REQUIRED_QTY ? 'text-[var(--color-primary)]' : 'text-[var(--color-danger)]'}`}
                >
                  {selectedTotal}
                </td>
                <td className="px-2 py-2 tabular-nums">{remainingTotal}</td>
                <td className="px-2 py-2 tabular-nums">{selectedVolume}</td>
                <td className="px-2 py-2 tabular-nums">{selectedWeight}</td>
                {editable ? <td /> : null}
              </tr>
            </tbody>
          </table>
        </div>
        <p className="text-xs text-[var(--text-muted)]">{t('wb.iss.partialAuto')}</p>
        {editable ? (
          <p className="text-[10px] text-[var(--text-muted)]">{t('wb.iss.dndHint')}</p>
        ) : null}
      </div>
    );
  }

  return (
    <div className="flex min-h-[calc(100vh-7rem)] flex-col gap-3">
      <div className="flex flex-wrap items-start justify-between gap-3 border-b border-[var(--border-default)] pb-3">
        <div>
          <p className="text-xs font-medium text-[var(--text-muted)]">INV-ISS-001 · {t('wb.iss.screenType')}</p>
          <h2 className="text-xl font-semibold tracking-tight">{t('wb.iss.title')}</h2>
          <p className="mt-1 max-w-2xl text-sm text-[var(--text-secondary)]">{t('wb.iss.desc')}</p>
          <p className="mt-2 text-xs text-[var(--text-muted)]">
            {selectedDoc
              ? `${t(`wb.iss.docType.${docType!}`)} · ${selectedDoc.ref} · ${selectedDoc.priority}`
              : t('wb.iss.noDocYet')}
            {' · '}
            {t('wb.iss.stageOf').replace('{n}', String(stageIdx + 1)).replace('{total}', String(STAGES.length))}
          </p>
        </div>
        <div className="flex flex-col items-end gap-2">
          <div className="rounded-md border border-[var(--border-default)] bg-[var(--color-surface)] px-3 py-2 text-right">
            <p className="text-[10px] uppercase tracking-wide text-[var(--text-muted)]">{t('wizard.systemCode')}</p>
            <p className="font-mono text-sm font-medium">{mintedGi ?? t('wizard.autoGenerated')}</p>
            <p className="text-[10px] text-[var(--text-muted)]">GI-… · txn oto</p>
          </div>
          <Link
            to="/inventory/operations/goods-issues"
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
            {t('wb.iss.timeline')}
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
              {stage.id === 'document' ? (
                <div className="space-y-3">
                  <p className="text-sm text-[var(--text-secondary)]">{t('wb.iss.documentIntro')}</p>
                  <div className="grid gap-2 sm:grid-cols-2">
                    {DOC_OPTIONS.map((d) => (
                      <button
                        key={d.id}
                        type="button"
                        onClick={() => setDocType(d.id)}
                        className={`rounded-md border px-3 py-2 text-left text-sm ${
                          docType === d.id
                            ? 'border-[var(--color-primary)] bg-[var(--color-primary)]/10'
                            : 'border-[var(--border-default)]'
                        }`}
                      >
                        <p className="font-medium">{t(`wb.iss.docType.${d.id}`)}</p>
                        <p className="font-mono text-xs text-[var(--text-muted)]">{d.ref}</p>
                        <p className="text-xs text-[var(--text-secondary)]">
                          {d.owner} · {d.priority} · {d.due}
                          {d.customer ? ` · ${d.customer}` : ''}
                        </p>
                      </button>
                    ))}
                  </div>
                  {docType === 'manual' ? (
                    <label className="block space-y-1 text-sm">
                      <span className="text-[var(--text-secondary)]">{t('wb.iss.manualReason')}</span>
                      <Input
                        value={manualReason}
                        onChange={(e) => setManualReason(e.target.value)}
                        placeholder={t('wb.iss.manualReasonPh')}
                      />
                      <p className="text-xs text-[var(--color-danger)]">{t('wb.iss.manualPermission')}</p>
                    </label>
                  ) : null}
                </div>
              ) : null}

              {stage.id === 'materials' ? (
                <div className="space-y-3">
                  <p className="text-sm text-[var(--text-secondary)]">{t('wb.iss.materialsIntro')}</p>
                  <div className="overflow-x-auto rounded-md border border-[var(--border-default)]">
                    <table className="w-full min-w-[560px] text-left text-xs">
                      <thead className="bg-[var(--color-surface-hover)] text-[var(--text-muted)]">
                        <tr>
                          <th className="px-3 py-2">{t('wb.iss.colMaterial')}</th>
                          <th className="px-3 py-2">{t('wb.iss.colRequired')}</th>
                          <th className="px-3 py-2">{t('wb.iss.colReserved')}</th>
                          <th className="px-3 py-2">{t('wb.iss.colAvailable')}</th>
                          <th className="px-3 py-2">{t('wb.iss.colAlt')}</th>
                        </tr>
                      </thead>
                      <tbody>
                        {MATERIAL_LINES.map((m) => (
                          <tr key={m.name} className="border-t border-[var(--border-default)]">
                            <td className="px-3 py-2 font-medium">{m.name}</td>
                            <td className="px-3 py-2 tabular-nums">
                              {m.required} {m.unit}
                            </td>
                            <td className="px-3 py-2 tabular-nums">
                              {m.reserved} {m.unit}
                            </td>
                            <td className="px-3 py-2 tabular-nums">
                              {m.available} {m.unit}
                            </td>
                            <td className="px-3 py-2 text-[var(--text-muted)]">{m.alt}</td>
                          </tr>
                        ))}
                      </tbody>
                    </table>
                  </div>
                  <p className="text-xs text-[var(--text-muted)]">{t('wb.iss.noMaterialCreate')}</p>
                </div>
              ) : null}

              {stage.id === 'aiPick' ? (
                <div className="space-y-3">
                  <p className="text-sm text-[var(--text-secondary)]">{t('wb.iss.aiPickIntro')}</p>
                  <p className="text-xs text-[var(--text-muted)]">
                    {selectedDoc?.ref ?? 'SO-250001'} · {MATERIAL_LINES[0].name} · {REQUIRED_QTY} ·{' '}
                    {t('wb.iss.minPackages')}
                  </p>
                  <div className="flex flex-wrap gap-1.5">
                    {AI_RULES.map((r) => (
                      <span
                        key={r}
                        className="rounded-md border border-[var(--border-default)] bg-[var(--color-surface-hover)] px-2 py-0.5 text-[10px] text-[var(--text-secondary)]"
                      >
                        {r}
                      </span>
                    ))}
                  </div>
                  <p className="text-xs font-semibold uppercase text-[var(--text-muted)]">{t('wb.iss.aiSuggestion')}</p>
                  <div className="overflow-x-auto rounded-md border border-[var(--color-primary)]/30">
                    <table className="w-full min-w-[520px] text-left text-xs">
                      <thead className="bg-[var(--color-primary)]/10 text-[var(--text-muted)]">
                        <tr>
                          <th className="px-3 py-2">{t('wb.iss.colPkgNo')}</th>
                          <th className="px-3 py-2">{t('wb.iss.loc')}</th>
                          <th className="px-3 py-2">{t('wb.iss.colSelected')}</th>
                          <th className="px-3 py-2">{t('wb.iss.colLot')}</th>
                        </tr>
                      </thead>
                      <tbody>
                        {AI_ALLOCATION.map((p) => (
                          <tr key={p.code} className="border-t border-[var(--border-default)]">
                            <td className="px-3 py-2 font-mono font-medium">{p.code}</td>
                            <td className="px-3 py-2 font-mono">{p.location}</td>
                            <td className="px-3 py-2 tabular-nums">{p.selected}</td>
                            <td className="px-3 py-2 font-mono text-[10px]">{p.lot}</td>
                          </tr>
                        ))}
                        <tr className="border-t border-[var(--border-default)] bg-[var(--color-surface-hover)] font-medium">
                          <td className="px-3 py-2">{t('wb.iss.total')}</td>
                          <td />
                          <td className="px-3 py-2 tabular-nums">{REQUIRED_QTY}</td>
                          <td />
                        </tr>
                      </tbody>
                    </table>
                  </div>
                  <div className="flex flex-wrap gap-2">
                    <Button type="button" variant={aiDecision === 'accept' ? 'default' : 'secondary'} onClick={acceptAi}>
                      ✓ {t('wb.iss.acceptAi')}
                    </Button>
                    <Button type="button" variant={aiDecision === 'ignore' ? 'default' : 'secondary'} onClick={ignoreAi}>
                      {t('wb.iss.ignoreAi')}
                    </Button>
                  </div>
                  {aiDecision === 'accept' ? (
                    <p className="text-sm text-[var(--color-primary)]">{t('wb.iss.acceptOk')}</p>
                  ) : null}
                  {overrideOpen || aiDecision === 'ignore' ? (
                    <div className="space-y-2 rounded-md border border-[var(--border-default)] px-3 py-3">
                      <p className="text-xs font-semibold uppercase text-[var(--text-muted)]">{t('wb.iss.explorer')}</p>
                      <p className="font-mono text-[11px] text-[var(--text-muted)]">{t('wb.iss.explorerPath')}</p>
                      <p className="text-sm text-[var(--text-secondary)]">{t('wb.iss.overrideHint')}</p>
                      <div className="grid gap-2">
                        {addablePackages.map((p) => (
                          <button
                            key={p.code}
                            type="button"
                            draggable
                            onDragStart={(e) => e.dataTransfer.setData('text/pkg', p.code)}
                            onClick={() => addPackage(p.code)}
                            className="cursor-grab rounded-md border border-[var(--border-default)] px-3 py-2 text-left text-sm hover:border-[var(--color-primary)] active:cursor-grabbing"
                          >
                            <p className="font-mono text-xs font-medium">{p.code}</p>
                            <p className="text-xs text-[var(--text-muted)]">
                              {p.location} · {p.available} {t('wb.iss.colAvail').toLowerCase()} · {p.lot} · {p.quality} ·{' '}
                              {p.moisture} · {t('wb.iss.dragMe')}
                            </p>
                          </button>
                        ))}
                      </div>
                      {allocation.length > 0 ? <PackageAllocationWorkspace editable={false} /> : null}
                      <p className="text-xs text-[var(--text-muted)]">{t('wb.iss.aiValidationOverride')}</p>
                    </div>
                  ) : null}
                </div>
              ) : null}

              {stage.id === 'picking' ? (
                <div className="space-y-3">
                  <p className="text-sm text-[var(--text-secondary)]">{t('wb.iss.allocIntro')}</p>
                  <div className="flex flex-wrap gap-2">
                    <Button type="button" variant="secondary" onClick={resetToAi}>
                      {t('wb.iss.resetAi')}
                    </Button>
                    <Button type="button" variant="secondary" onClick={() => setOverrideOpen(true)}>
                      {t('wb.iss.addPkg')}
                    </Button>
                  </div>
                  {overrideOpen ? (
                    <div className="flex flex-wrap gap-2 rounded-md border border-[var(--border-default)] px-3 py-2">
                      {addablePackages.length === 0 ? (
                        <p className="text-xs text-[var(--text-muted)]">{t('wb.iss.noMorePkg')}</p>
                      ) : (
                        addablePackages.map((p) => (
                          <Button
                            key={p.code}
                            type="button"
                            variant="secondary"
                            draggable
                            onDragStart={(e) =>
                              ((e as unknown as DragEvent).dataTransfer.setData('text/pkg', p.code))
                            }
                            onClick={() => addPackage(p.code)}
                          >
                            + {p.code}
                          </Button>
                        ))
                      )}
                    </div>
                  ) : null}
                  <PackageAllocationWorkspace editable />
                  {warnings.length > 0 ? (
                    <div className="space-y-2 rounded-md border border-[var(--color-danger)]/40 bg-[var(--color-danger)]/5 px-3 py-2">
                      <p className="text-xs font-semibold text-[var(--color-danger)]">{t('wb.iss.mixWarnings')}</p>
                      <ul className="list-inside list-disc text-xs text-[var(--text-secondary)]">
                        {warnings.map((w) => (
                          <li key={w}>{t(`wb.iss.warn.${w}`)}</li>
                        ))}
                      </ul>
                      <label className="flex items-center gap-2 text-sm font-medium">
                        <input type="checkbox" checked={mixWaived} onChange={(e) => setMixWaived(e.target.checked)} />
                        {t('wb.iss.mixWaive')}
                      </label>
                    </div>
                  ) : (
                    <p className="text-xs text-[var(--color-primary)]">{t('wb.iss.mixOk')}</p>
                  )}
                  <label className="block max-w-md space-y-1 text-sm">
                    <span className="text-[var(--text-secondary)]">{t('wb.iss.scanLabel')}</span>
                    <div className="flex gap-2">
                      <Input
                        value={scanValue}
                        onChange={(e) => setScanValue(e.target.value)}
                        placeholder={allocation[0]?.code ?? 'PKG-…'}
                      />
                      <Button type="button" variant="secondary" onClick={tryScan}>
                        {t('wb.iss.scanValidate')}
                      </Button>
                    </div>
                  </label>
                  <Button
                    type="button"
                    variant="secondary"
                    onClick={() => {
                      setScanValue(allocation.map((r) => r.code).join('+'));
                      setScannedOk(true);
                      setError(null);
                    }}
                  >
                    {t('wb.iss.demoScan')}
                  </Button>
                  {scannedOk ? <p className="text-sm text-[var(--color-primary)]">{t('wb.iss.scanOk')}</p> : null}
                </div>
              ) : null}

              {stage.id === 'verify' ? (
                <div className="space-y-3">
                  <p className="text-sm text-[var(--text-secondary)]">{t('wb.iss.verifyIntro')}</p>
                  <ul className="list-inside list-disc text-sm">
                    <li>{t('wb.iss.checkMaterial')}</li>
                    <li>{t('wb.iss.checkLot')}</li>
                    <li>{t('wb.iss.checkQty')}</li>
                    <li>{t('wb.iss.checkQuality')}</li>
                    <li>{t('wb.iss.checkReservation')}</li>
                  </ul>
                  <label className="flex items-center gap-2 text-sm font-medium">
                    <input type="checkbox" checked={verifyOk} onChange={(e) => setVerifyOk(e.target.checked)} />
                    {t('wb.iss.verifyOk')}
                  </label>
                </div>
              ) : null}

              {stage.id === 'evidence' ? (
                <div className="space-y-3">
                  <p className="text-sm text-[var(--text-secondary)]">{t('wb.iss.evidenceIntro')}</p>
                  <div className="flex flex-wrap gap-2">
                    {['damage', 'missing', 'brokenPkg', 'note'].map((k) => (
                      <Button key={k} type="button" variant="secondary" onClick={() => addEvidence(k)}>
                        {evidence.includes(k) ? '✓ ' : '+ '}
                        {t(`wb.iss.ev.${k}`)}
                      </Button>
                    ))}
                  </div>
                  <p className="text-xs text-[var(--text-muted)]">
                    {evidence.length} {t('wb.iss.evidenceCount')} — {t('wb.iss.evidenceBelongs')}
                  </p>
                </div>
              ) : null}

              {stage.id === 'quality' ? (
                <div className="space-y-3">
                  <p className="text-sm text-[var(--text-secondary)]">{t('wb.iss.qualityIntro')}</p>
                  <div className="rounded-md border border-[var(--border-default)] px-3 py-2 text-sm">
                    <p>✓ {t('wb.iss.qBlocked')}</p>
                    <p>✓ {t('wb.iss.qHold')}</p>
                    <p>✓ {t('wb.iss.qQuarantine')}</p>
                    <p>✓ {t('wb.iss.qExpired')}</p>
                  </div>
                  <label className="flex items-center gap-2 text-sm font-medium">
                    <input type="checkbox" checked={qualityOk} onChange={(e) => setQualityOk(e.target.checked)} />
                    {t('wb.iss.qualityClear')}
                  </label>
                </div>
              ) : null}

              {stage.id === 'destination' ? (
                <div className="space-y-3">
                  <div className="flex flex-wrap gap-2">
                    <Button
                      type="button"
                      variant={destType === 'production' ? 'default' : 'secondary'}
                      onClick={() => {
                        setDestType('production');
                        setDestination('Hat 1 · WC-SAW · OP-10');
                      }}
                    >
                      {t('wb.iss.destProduction')}
                    </Button>
                    <Button
                      type="button"
                      variant={destType === 'shipping' ? 'default' : 'secondary'}
                      onClick={() => {
                        setDestType('shipping');
                        setDestination('Rampa 3 · Araç 34 ABC 123');
                      }}
                    >
                      {t('wb.iss.destShipping')}
                    </Button>
                  </div>
                  <label className="block space-y-1 text-sm">
                    <span className="text-[var(--text-secondary)]">{t('wb.iss.destLabel')}</span>
                    <Input value={destination} onChange={(e) => setDestination(e.target.value)} />
                  </label>
                  <p className="text-xs text-[var(--text-muted)]">{t('wb.iss.noCodeInput')}</p>
                </div>
              ) : null}

              {stage.id === 'review' ? (
                <div className="space-y-3">
                  <div className="grid gap-2 md:grid-cols-2">
                    {[
                      [t('wb.iss.document'), selectedDoc ? `${t(`wb.iss.docType.${docType!}`)} · ${selectedDoc.ref}` : '—'],
                      [t('wb.iss.colMaterial'), `${MATERIAL_LINES[0].name} · ${REQUIRED_QTY}`],
                      [
                        t('wb.iss.aiDecision'),
                        aiDecision === 'accept'
                          ? t('wb.iss.acceptAi')
                          : aiDecision === 'ignore'
                            ? t('wb.iss.ignoreAi')
                            : '—',
                      ],
                      [
                        t('wb.iss.allocGrid'),
                        allocation.map((r) => `${r.code}:${r.selected}`).join(' · ') || '—',
                      ],
                      [t('wb.iss.total'), `${selectedTotal} / ${REQUIRED_QTY}`],
                      [t('wb.iss.evidence'), `${evidence.length} ${t('wb.iss.evidenceCount')}`],
                      [t('wb.iss.destination'), destination],
                      [
                        t('wb.iss.overrideHistory'),
                        overrideHistory.length ? overrideHistory.join(' · ') : t('wb.iss.noOverrides'),
                      ],
                    ].map(([label, value]) => (
                      <div key={label} className="rounded-md border border-[var(--border-default)] px-3 py-2">
                        <p className="text-[10px] uppercase text-[var(--text-muted)]">{label}</p>
                        <p className="text-sm font-medium">{value}</p>
                      </div>
                    ))}
                  </div>
                  <label className="flex items-center gap-2 text-sm font-medium">
                    <input type="checkbox" checked={approved} onChange={(e) => setApproved(e.target.checked)} />
                    {t('wb.iss.approvePost')}
                  </label>
                </div>
              ) : null}

              {stage.id === 'post' ? (
                <div className="space-y-3">
                  <p className="text-sm text-[var(--text-secondary)]">{t('wb.iss.postIntro')}</p>
                  <ul className="list-inside list-disc text-sm">
                    <li>{t('wb.iss.postItem.gi')}</li>
                    <li>{t('wb.iss.postItem.txn')}</li>
                    <li>{t('wb.iss.postItem.history')}</li>
                    <li>{t('wb.iss.postItem.reservation')}</li>
                    <li>{t('wb.iss.postItem.genealogy')}</li>
                    <li>{t('wb.iss.postItem.evidence')}</li>
                  </ul>
                  {posted ? (
                    <p className="text-sm font-medium text-[var(--color-primary)]">
                      {t('wb.iss.postedBanner')}
                      {mintedGi ? ` · ${mintedGi}` : ''}
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
            <p className="text-[10px] font-semibold uppercase tracking-wide text-[var(--text-muted)]">{t('wb.iss.context')}</p>
            <dl className="mt-2 space-y-2 text-xs">
              <div>
                <dt className="text-[var(--text-muted)]">{t('wb.iss.aiDecision')}</dt>
                <dd className="font-medium">
                  {aiDecision === 'accept'
                    ? t('wb.iss.acceptAi')
                    : aiDecision === 'ignore'
                      ? t('wb.iss.ignoreAi')
                      : '—'}
                </dd>
              </div>
              <div>
                <dt className="text-[var(--text-muted)]">{t('wb.iss.allocGrid')}</dt>
                <dd className="font-medium tabular-nums">
                  {allocation.length} pkg · Σ {selectedTotal}
                </dd>
              </div>
              <div>
                <dt className="text-[var(--text-muted)]">{t('wb.iss.validation')}</dt>
                <dd className="font-medium">
                  {warnings.length && !mixWaived
                    ? t('wb.iss.mixWarnings')
                    : verifyOk && qualityOk
                      ? t('wb.iss.validationOk')
                      : t('wb.iss.validationOpen')}
                </dd>
              </div>
              <div>
                <dt className="text-[var(--text-muted)]">{t('wb.iss.noManualIds')}</dt>
                <dd className="text-[var(--text-secondary)]">GI · Lot · MI · WH kodu elle yok</dd>
              </div>
            </dl>
          </div>
        </aside>
      </div>

      <div className="sticky bottom-0 z-10 -mx-1 flex flex-wrap items-center justify-between gap-2 border-t border-[var(--border-default)] bg-[var(--color-bg,var(--color-surface))] px-2 py-3">
        <div className="flex flex-wrap gap-2">
          <Button type="button" variant="secondary" disabled={stageIdx === 0 || posted} onClick={() => setStageIdx((s) => s - 1)}>
            {t('wizard.back')}
          </Button>
          <Button type="button" variant="secondary" disabled={posted}>
            {t('wb.iss.saveDraft')}
          </Button>
        </div>
        <div className="flex flex-wrap gap-2">
          {stage.id !== 'post' ? (
            <Button type="button" disabled={!canAdvance || posted} onClick={goNext}>
              {t('wb.iss.nextStage')}
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
                <Button
                  type="button"
                  variant="secondary"
                  onClick={() => void navigate({ to: '/inventory/operations/goods-issues' })}
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

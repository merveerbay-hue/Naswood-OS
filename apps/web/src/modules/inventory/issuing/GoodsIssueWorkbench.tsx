import { Link, useNavigate } from '@tanstack/react-router';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { useMemo, useState } from 'react';
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
  { id: 'picking', titleKey: 'wb.iss.picking', hintKey: 'wb.iss.pickingHint' },
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

/** Senaryo 1 — AI multi-package recommendation for SO-250001 */
const AI_PACKAGES = [
  { id: 'A', code: 'PKG-A-2026-000120', qty: 20, loc: 'A-01-02', note: 'FIFO · aynı lot' },
  { id: 'B', code: 'PKG-B-2026-000088', qty: 15, loc: 'A-01-04', note: 'üste yakın' },
  { id: 'C', code: 'PKG-C-2026-000091', qty: 15, loc: 'A-02-01', note: 'aynı nem · kalite' },
];

const AI_RULES = ['FIFO', 'FEFO', 'Rezervasyon', 'Kalite', 'Lokasyon', 'Aynı Lot', 'Aynı Nem', 'Aynı Kalite', 'Aynı Üretim Tarihi'];

const OVERRIDE_PACKAGES = [
  { code: 'PKG-B-2026-000088', label: 'Paket B — üste yakın / forklift', loc: 'A-01-04', qty: 15 },
  { code: 'PKG-D-2026-000210', label: 'Paket D — müşteriye daha uygun', loc: 'B-03-01', qty: 20 },
  { code: 'PKG-A-2026-000120', label: 'Paket A', loc: 'A-01-02', qty: 20 },
];

/** Senaryo 3 — partial package */
const PARTIAL_PKG = {
  code: 'PKG-00254',
  original: 120,
  volume: 5.24,
  issueDefault: 40,
  unit: 'adet',
};

const AI_PICK = {
  warehouse: 'Ana Mamul Deposu',
  location: 'A-01-02',
  lot: 'LOT-TW-2026-000042',
  mi: 'FG-TWDECK-20260801-00012',
  package: AI_PACKAGES[0].code,
  rule: AI_RULES.join(' · '),
};

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
  const [selectedPackage, setSelectedPackage] = useState<string | null>(null);
  const [overrideHistory, setOverrideHistory] = useState<string[]>([]);
  const [scanValue, setScanValue] = useState('');
  const [scannedOk, setScannedOk] = useState(false);
  const [verifyOk, setVerifyOk] = useState(false);
  const [evidence, setEvidence] = useState<string[]>([]);
  const [qualityOk, setQualityOk] = useState(false);
  const [destType, setDestType] = useState<'production' | 'shipping'>('shipping');
  const [destination, setDestination] = useState('Rampa 2 · Araç 34 ABC 123');
  const [issueQty, setIssueQty] = useState(PARTIAL_PKG.issueDefault);
  const remainingQty = Math.max(0, PARTIAL_PKG.original - issueQty);
  const remainingVolume = Number(((PARTIAL_PKG.volume * remainingQty) / PARTIAL_PKG.original).toFixed(2));

  const stage = STAGES[stageIdx];
  const selectedDoc = DOC_OPTIONS.find((d) => d.id === docType) ?? null;
  const progress = Math.round(((stageIdx + (posted ? 1 : 0)) / STAGES.length) * 100);

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
        if (aiDecision === 'ignore' && !selectedPackage) return t('wb.iss.gateNeedOverridePkg');
        return null;
      case 'picking':
        if (issueQty <= 0 || issueQty > PARTIAL_PKG.original) return t('wb.iss.gateNeedPartialQty');
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
    selectedPackage,
    issueQty,
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
          `pkg=${selectedPackage || AI_PACKAGES.map((p) => p.code).join(',')}`,
          `partial=${PARTIAL_PKG.code}:${issueQty}/${PARTIAL_PKG.original}`,
          `remaining=${remainingQty}`,
          `lot=${AI_PICK.lot}`,
          `mi=${AI_PICK.mi}`,
          `loc=${AI_PICK.location}`,
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
    setSelectedPackage(null);
    setError(null);
  }

  function ignoreAi() {
    setAiDecision('ignore');
    setOverrideOpen(true);
    setSelectedPackage(null);
    setError(null);
  }

  function pickOverridePackage(code: string) {
    setSelectedPackage(code);
    setOverrideHistory((h) => [
      ...h,
      `${new Date().toISOString().slice(11, 19)} Yoksay → ${code} (AI: ${AI_PACKAGES.map((p) => p.id).join('+')})`,
    ]);
    setError(null);
  }

  function tryScan() {
    const v = scanValue.trim().toUpperCase();
    const target = (selectedPackage || PARTIAL_PKG.code || AI_PICK.package).toUpperCase();
    const ok =
      !v ||
      v.includes('PKG') ||
      v.includes('LOT') ||
      v === target ||
      v === PARTIAL_PKG.code.toUpperCase() ||
      v === AI_PICK.lot.toUpperCase();
    setScannedOk(ok);
    if (!ok) setError(t('wb.iss.scanMismatch'));
    else setError(null);
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
                    {selectedDoc?.ref ?? 'SO-250001'} · {MATERIAL_LINES[0].name} · {MATERIAL_LINES[0].required}{' '}
                    {MATERIAL_LINES[0].unit}
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
                  <div className="overflow-x-auto rounded-md border border-[var(--color-primary)]/30">
                    <table className="w-full min-w-[420px] text-left text-xs">
                      <thead className="bg-[var(--color-primary)]/10 text-[var(--text-muted)]">
                        <tr>
                          <th className="px-3 py-2">{t('wb.iss.colPackage')}</th>
                          <th className="px-3 py-2">{t('wb.iss.colRequired')}</th>
                          <th className="px-3 py-2">{t('wb.iss.loc')}</th>
                          <th className="px-3 py-2">{t('wb.iss.aiSuggestion')}</th>
                        </tr>
                      </thead>
                      <tbody>
                        {AI_PACKAGES.map((p) => (
                          <tr key={p.id} className="border-t border-[var(--border-default)]">
                            <td className="px-3 py-2 font-medium">
                              {t('wb.iss.packageLabel')} {p.id}{' '}
                              <span className="font-mono text-[10px] text-[var(--text-muted)]">{p.code}</span>
                            </td>
                            <td className="px-3 py-2 tabular-nums">
                              {p.qty} {MATERIAL_LINES[0].unit}
                            </td>
                            <td className="px-3 py-2 font-mono">{p.loc}</td>
                            <td className="px-3 py-2 text-[var(--text-muted)]">{p.note}</td>
                          </tr>
                        ))}
                        <tr className="border-t border-[var(--border-default)] bg-[var(--color-surface-hover)] font-medium">
                          <td className="px-3 py-2">{t('wb.iss.total')}</td>
                          <td className="px-3 py-2 tabular-nums">
                            {AI_PACKAGES.reduce((s, p) => s + p.qty, 0)} {MATERIAL_LINES[0].unit}
                          </td>
                          <td colSpan={2} />
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
                      <div className="grid gap-2 sm:grid-cols-1">
                        {OVERRIDE_PACKAGES.map((p) => (
                          <button
                            key={p.code}
                            type="button"
                            onClick={() => pickOverridePackage(p.code)}
                            className={`rounded-md border px-3 py-2 text-left text-sm ${
                              selectedPackage === p.code
                                ? 'border-[var(--color-primary)] bg-[var(--color-primary)]/10'
                                : 'border-[var(--border-default)]'
                            }`}
                          >
                            <p className="font-medium">{p.label}</p>
                            <p className="font-mono text-xs text-[var(--text-muted)]">
                              {p.code} · {p.loc} · {p.qty} {MATERIAL_LINES[0].unit}
                            </p>
                          </button>
                        ))}
                      </div>
                      <p className="text-xs text-[var(--text-muted)]">{t('wb.iss.aiValidationOverride')}</p>
                    </div>
                  ) : null}
                </div>
              ) : null}

              {stage.id === 'picking' ? (
                <div className="space-y-3">
                  <p className="text-sm text-[var(--text-secondary)]">{t('wb.iss.pickingIntro')}</p>
                  <p className="font-mono text-xs text-[var(--text-muted)]">
                    {AI_PICK.warehouse} → Zone A → Rack → Shelf →{' '}
                    {selectedPackage || AI_PACKAGES.map((p) => p.code).join(' + ')}
                  </p>

                  <div className="rounded-md border border-[var(--border-default)] px-3 py-3 space-y-2">
                    <p className="text-xs font-semibold uppercase text-[var(--text-muted)]">{t('wb.iss.partialTitle')}</p>
                    <p className="text-sm">
                      <span className="font-mono font-medium">{PARTIAL_PKG.code}</span>
                      {' · '}
                      {PARTIAL_PKG.original} {PARTIAL_PKG.unit} · {PARTIAL_PKG.volume} m³
                    </p>
                    <label className="block max-w-xs space-y-1 text-sm">
                      <span className="text-[var(--text-secondary)]">{t('wb.iss.issueQty')}</span>
                      <Input
                        type="number"
                        min={1}
                        max={PARTIAL_PKG.original}
                        value={issueQty}
                        onChange={(e) => setIssueQty(Number(e.target.value) || 0)}
                      />
                    </label>
                    <dl className="grid grid-cols-2 gap-2 text-xs sm:grid-cols-4">
                      <div className="rounded-md bg-[var(--color-surface-hover)] px-2 py-1.5">
                        <dt className="text-[var(--text-muted)]">{t('wb.iss.issued')}</dt>
                        <dd className="font-medium tabular-nums">
                          {issueQty} {PARTIAL_PKG.unit}
                        </dd>
                      </div>
                      <div className="rounded-md bg-[var(--color-surface-hover)] px-2 py-1.5">
                        <dt className="text-[var(--text-muted)]">{t('wb.iss.remaining')}</dt>
                        <dd className="font-medium tabular-nums text-[var(--color-primary)]">
                          {remainingQty} {PARTIAL_PKG.unit}
                        </dd>
                      </div>
                      <div className="rounded-md bg-[var(--color-surface-hover)] px-2 py-1.5">
                        <dt className="text-[var(--text-muted)]">{t('wb.iss.remainingVol')}</dt>
                        <dd className="font-medium tabular-nums">{remainingVolume} m³</dd>
                      </div>
                      <div className="rounded-md bg-[var(--color-surface-hover)] px-2 py-1.5">
                        <dt className="text-[var(--text-muted)]">{t('wb.iss.pkgStatus')}</dt>
                        <dd className="font-medium">{t('wb.iss.statusPartial')}</dd>
                      </div>
                    </dl>
                    <p className="text-xs text-[var(--text-muted)]">{t('wb.iss.partialAuto')}</p>
                  </div>

                  <label className="block max-w-md space-y-1 text-sm">
                    <span className="text-[var(--text-secondary)]">{t('wb.iss.scanLabel')}</span>
                    <div className="flex gap-2">
                      <Input
                        value={scanValue}
                        onChange={(e) => setScanValue(e.target.value)}
                        placeholder={PARTIAL_PKG.code}
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
                      setScanValue(PARTIAL_PKG.code);
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
                      [t('wb.iss.colMaterial'), `${MATERIAL_LINES[0].name} · ${MATERIAL_LINES[0].required} ${MATERIAL_LINES[0].unit}`],
                      [
                        t('wb.iss.aiDecision'),
                        aiDecision === 'accept'
                          ? t('wb.iss.acceptAi')
                          : aiDecision === 'ignore'
                            ? `${t('wb.iss.ignoreAi')} → ${selectedPackage ?? '—'}`
                            : '—',
                      ],
                      [
                        t('wb.iss.partialTitle'),
                        `${PARTIAL_PKG.code}: ${issueQty} ↓ · ${remainingQty} ${t('wb.iss.remaining').toLowerCase()}`,
                      ],
                      [t('wb.iss.wh'), `${AI_PICK.warehouse} · ${AI_PICK.location}`],
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
                <dt className="text-[var(--text-muted)]">{t('wb.iss.partialTitle')}</dt>
                <dd className="font-medium tabular-nums">
                  {PARTIAL_PKG.code}: {remainingQty}/{PARTIAL_PKG.original}
                </dd>
              </div>
              <div>
                <dt className="text-[var(--text-muted)]">{t('wb.iss.validation')}</dt>
                <dd className="font-medium">{verifyOk && qualityOk ? t('wb.iss.validationOk') : t('wb.iss.validationOpen')}</dd>
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

import { useMemo, useState } from 'react';
import { Button, Input } from '@naswood/ui';
import { useI18n } from '@/i18n';
import {
  allCountableCounted,
  countableLines,
  finalLineQty,
  formatLineDims,
  lumberVolumeM3,
  matchIncomingByLabel,
  packageTotal,
  type CountStatus,
  type IncomingLine,
  type MaterialKind,
  type PackageRow,
} from './incomingLines';

export type CountMethod = 'photo' | 'handwriting' | 'excel' | 'manual';

/** @deprecated Session qty helpers — stock now uses incoming line finals. */
export type PhysicalCountState = {
  method: CountMethod;
  documentQty: string;
  aiQty: string;
  aiNote: string;
  handwritingPhoto: boolean;
  handwritingParsed: boolean;
  excelUploaded: boolean;
  excelParsed: boolean;
  packages: string;
  perPackage: string;
  operatorQty: string;
  lines: { id: string; product: string; qty: string }[];
  verified: boolean;
};

export function createDefaultPhysicalCount(): PhysicalCountState {
  return {
    method: 'manual',
    documentQty: '',
    aiQty: '',
    aiNote: '',
    handwritingPhoto: false,
    handwritingParsed: false,
    excelUploaded: false,
    excelParsed: false,
    packages: '',
    perPackage: '',
    operatorQty: '',
    lines: [],
    verified: false,
  };
}

/** Aggregate final qty for single-line stock post (primary lumber, else first counted). */
export function finalPhysicalQtyFromLines(lines: IncomingLine[]): number {
  const counted = countableLines(lines).filter((l) => l.countStatus === 'counted' && finalLineQty(l) > 0);
  const primary = counted.find((l) => l.kind === 'lumber') ?? counted[0];
  return primary ? finalLineQty(primary) : 0;
}

/** @deprecated Prefer finalPhysicalQtyFromLines */
export function finalPhysicalQty(state: PhysicalCountState): number {
  const op = Number(String(state.operatorQty).replace(',', '.'));
  if (Number.isFinite(op) && op > 0) return op;
  const ai = Number(String(state.aiQty).replace(',', '.'));
  if (Number.isFinite(ai) && ai > 0) return ai;
  return 0;
}

const KIND_LABEL: Record<MaterialKind, string> = {
  lumber: 'Kereste',
  log: 'Tomruk',
  lamella: 'Lamel',
  thermowood: 'Thermowood',
  panel: 'Masif Panel',
  packaged: 'Paketli',
  other: 'Diğer',
};

const STATUS_KEY: Record<CountStatus, string> = {
  pending: 'wb.rcv.ops.count.status.pending',
  counting: 'wb.rcv.ops.count.status.counting',
  counted: 'wb.rcv.ops.count.status.counted',
  recheck: 'wb.rcv.ops.count.status.recheck',
};

const DEMO_HANDWRITING: { label: string; qty: string }[] = [
  { label: 'Çam Kereste 26×140×3000', qty: '498' },
  { label: 'Çam Kereste 26×92×3000', qty: '296' },
  { label: 'Çam Tomruk', qty: '42' },
];

const DEMO_EXCEL: { label: string; qty: string }[] = [
  { label: 'Çam Kereste 26×140×3000', qty: '498' },
  { label: 'Çam Kereste 26×92×3000', qty: '300' },
  { label: 'Thermowood Deck 26×140×3000', qty: '98' },
];

type Props = {
  lines: IncomingLine[];
  onChange: (next: IncomingLine[]) => void;
  disabled?: boolean;
};

function updateLine(lines: IncomingLine[], id: string, patch: Partial<IncomingLine>): IncomingLine[] {
  return lines.map((l) => (l.id === id ? { ...l, ...patch } : l));
}

/** Stage 3 — Fiziksel sayım: gelen ürün satırlarından (yeni malzeme yok). */
export function PhysicalCountStep({ lines, onChange, disabled }: Props) {
  const { t } = useI18n();
  const [method, setMethod] = useState<CountMethod>('manual');
  const [selectedId, setSelectedId] = useState<string | null>(null);
  const [photoCaptured, setPhotoCaptured] = useState(false);
  const [handwritingPhoto, setHandwritingPhoto] = useState(false);
  const [excelUploaded, setExcelUploaded] = useState(false);
  const [bulkNote, setBulkNote] = useState('');

  const countable = useMemo(() => countableLines(lines), [lines]);
  const selected = countable.find((l) => l.id === selectedId) ?? countable[0] ?? null;

  const totals = useMemo(() => {
    const counted = countable.filter((l) => l.countStatus === 'counted').length;
    const recheck = countable.filter((l) => l.countStatus === 'recheck').length;
    const pending = countable.length - counted - recheck;
    return { total: countable.length, counted, pending: Math.max(0, pending), recheck };
  }, [countable]);

  const allDone = allCountableCounted(lines);

  function selectLine(id: string) {
    setSelectedId(id);
    const line = lines.find((l) => l.id === id);
    if (line && line.countStatus === 'pending') {
      onChange(updateLine(lines, id, { countStatus: 'counting' }));
    }
  }

  function patchSelected(patch: Partial<IncomingLine>) {
    if (!selected) return;
    onChange(updateLine(lines, selected.id, { ...patch, countStatus: patch.countStatus ?? 'counting' }));
  }

  function confirmSelected() {
    if (!selected) return;
    const qty = finalLineQty({ ...selected, operatorQty: selected.operatorQty || selected.aiQty });
    if (qty <= 0) return;
    onChange(
      updateLine(lines, selected.id, {
        operatorQty: selected.operatorQty || selected.aiQty,
        countStatus: 'counted',
      }),
    );
  }

  function markRecheck() {
    if (!selected) return;
    onChange(updateLine(lines, selected.id, { countStatus: 'recheck' }));
  }

  function applyPackageToOperator() {
    if (!selected) return;
    const total = packageTotal(selected);
    if (total == null) return;
    patchSelected({ operatorQty: String(total) });
  }

  function addPackageRow() {
    if (!selected) return;
    const row: PackageRow = { id: `pkg-${Date.now()}`, qty: '' };
    patchSelected({ packageRows: [...selected.packageRows, row] });
  }

  function runPhotoAi() {
    if (!selected) return;
    const suggestion =
      selected.kind === 'log'
        ? String(selected.documentQty)
        : String(Math.max(1, selected.documentQty - 4));
    onChange(
      updateLine(lines, selected.id, {
        aiQty: suggestion,
        countStatus: 'counting',
        note: t('wb.rcv.ops.count.aiPhotoNote'),
      }),
    );
    setBulkNote(t('wb.rcv.ops.count.aiPhotoNote'));
  }

  function applyMatchedSuggestions(pairs: { label: string; qty: string }[], note: string) {
    let next = [...lines];
    for (const pair of pairs) {
      const hit = matchIncomingByLabel(next, pair.label);
      if (!hit) continue;
      next = updateLine(next, hit.id, {
        aiQty: pair.qty,
        countStatus: hit.countStatus === 'counted' ? 'counted' : 'counting',
        note,
      });
    }
    onChange(next);
    setBulkNote(note);
  }

  function runHandwritingAi() {
    applyMatchedSuggestions(DEMO_HANDWRITING, t('wb.rcv.ops.count.aiHandwritingNote'));
  }

  function runExcelParse() {
    applyMatchedSuggestions(DEMO_EXCEL, t('wb.rcv.ops.count.aiExcelNote'));
  }

  function acceptAiAsOperator() {
    if (!selected?.aiQty) return;
    patchSelected({ operatorQty: selected.aiQty });
  }

  const vol =
    selected && (selected.kind === 'lumber' || selected.kind === 'thermowood' || selected.kind === 'lamella')
      ? lumberVolumeM3(selected, finalLineQty(selected) || Number(selected.operatorQty) || 0)
      : null;
  const pkgTot = selected ? packageTotal(selected) : null;

  return (
    <div className="space-y-5">
      <p className="text-sm text-[var(--text-secondary)]">{t('wb.rcv.ops.count.introLines')}</p>
      <p className="text-xs text-[var(--text-muted)]">{t('wb.rcv.ops.count.noNewMaterial')}</p>

      <div className="flex flex-wrap gap-3 text-sm">
        <span className="rounded-md border border-[var(--border-default)] px-3 py-1.5">
          {t('wb.rcv.ops.count.summaryIncoming')}: <strong className="tabular-nums">{totals.total}</strong>
        </span>
        <span className="rounded-md border border-[var(--border-default)] px-3 py-1.5">
          {t('wb.rcv.ops.count.summaryCounted')}: <strong className="tabular-nums">{totals.counted}</strong>
        </span>
        <span className="rounded-md border border-[var(--border-default)] px-3 py-1.5">
          {t('wb.rcv.ops.count.summaryPending')}: <strong className="tabular-nums">{totals.pending}</strong>
        </span>
        {totals.recheck > 0 ? (
          <span className="rounded-md border border-[var(--color-danger)]/40 px-3 py-1.5 text-[var(--color-danger)]">
            {t('wb.rcv.ops.count.summaryRecheck')}: <strong className="tabular-nums">{totals.recheck}</strong>
          </span>
        ) : null}
      </div>

      <div className="flex flex-wrap gap-2">
        {(
          [
            ['photo', t('wb.rcv.ops.count.method.photo')],
            ['handwriting', t('wb.rcv.ops.count.method.handwriting')],
            ['excel', t('wb.rcv.ops.count.method.excel')],
            ['manual', t('wb.rcv.ops.count.method.manual')],
          ] as const
        ).map(([id, label]) => (
          <Button
            key={id}
            type="button"
            size="sm"
            variant={method === id ? 'default' : 'secondary'}
            disabled={disabled}
            onClick={() => setMethod(id)}
          >
            {label}
          </Button>
        ))}
      </div>

      {method === 'photo' ? (
        <section className="space-y-3 rounded-md border border-[var(--border-default)] px-3 py-3">
          <p className="text-sm font-medium">{t('wb.rcv.ops.count.photoTitle')}</p>
          <p className="text-xs text-[var(--text-muted)]">{t('wb.rcv.ops.count.photoHint')}</p>
          <label className="flex items-center gap-2 text-sm">
            <input
              type="checkbox"
              checked={photoCaptured}
              disabled={disabled}
              onChange={(e) => setPhotoCaptured(e.target.checked)}
            />
            {t('wb.rcv.ops.count.photoCaptured')}
          </label>
          <Button type="button" size="sm" variant="secondary" disabled={disabled || !photoCaptured || !selected} onClick={runPhotoAi}>
            {t('wb.rcv.ops.count.runPhotoAi')}
          </Button>
          <p className="text-xs text-[var(--text-muted)]">{t('wb.rcv.ops.count.aiNotFinal')}</p>
        </section>
      ) : null}

      {method === 'handwriting' ? (
        <section className="space-y-3 rounded-md border border-[var(--border-default)] px-3 py-3">
          <p className="text-sm font-medium">{t('wb.rcv.ops.count.handwritingTitle')}</p>
          <p className="text-xs text-[var(--text-muted)]">{t('wb.rcv.ops.count.handwritingMatchHint')}</p>
          <label className="flex items-center gap-2 text-sm">
            <input
              type="checkbox"
              checked={handwritingPhoto}
              disabled={disabled}
              onChange={(e) => setHandwritingPhoto(e.target.checked)}
            />
            {t('wb.rcv.ops.count.handwritingPhoto')}
          </label>
          <Button type="button" size="sm" variant="secondary" disabled={disabled || !handwritingPhoto} onClick={runHandwritingAi}>
            {t('wb.rcv.ops.count.runAiRead')}
          </Button>
          <p className="text-xs text-[var(--text-muted)]">{t('wb.rcv.ops.count.aiNotFinal')}</p>
        </section>
      ) : null}

      {method === 'excel' ? (
        <section className="space-y-3 rounded-md border border-[var(--border-default)] px-3 py-3">
          <p className="text-sm font-medium">{t('wb.rcv.ops.count.excelTitle')}</p>
          <p className="text-xs text-[var(--text-muted)]">{t('wb.rcv.ops.count.excelMatchHint')}</p>
          <label className="flex items-center gap-2 text-sm">
            <input
              type="checkbox"
              checked={excelUploaded}
              disabled={disabled}
              onChange={(e) => setExcelUploaded(e.target.checked)}
            />
            {t('wb.rcv.ops.count.excelUploaded')}
          </label>
          <Button type="button" size="sm" variant="secondary" disabled={disabled || !excelUploaded} onClick={runExcelParse}>
            {t('wb.rcv.ops.count.parseExcel')}
          </Button>
        </section>
      ) : null}

      {bulkNote ? <p className="text-xs text-[var(--text-muted)]">{bulkNote}</p> : null}

      {countable.length === 0 ? (
        <p className="rounded-md border border-[var(--color-danger)]/40 bg-[var(--color-danger)]/5 px-3 py-2 text-sm text-[var(--color-danger)]">
          {t('wb.rcv.ops.count.noCountable')}
        </p>
      ) : (
        <div className="overflow-x-auto rounded-md border border-[var(--border-default)]">
          <table className="w-full min-w-[560px] text-left text-sm">
            <thead className="bg-[var(--color-surface-hover)] text-[10px] uppercase text-[var(--text-muted)]">
              <tr>
                <th className="px-3 py-2">{t('wb.rcv.ops.count.colName')}</th>
                <th className="px-3 py-2">{t('wb.rcv.ops.count.colDims')}</th>
                <th className="px-3 py-2">{t('wb.rcv.ops.count.colKind')}</th>
                <th className="px-3 py-2">{t('wb.rcv.ops.count.colDocQty')}</th>
                <th className="px-3 py-2">{t('wb.rcv.ops.count.colStatus')}</th>
              </tr>
            </thead>
            <tbody>
              {countable.map((line) => {
                const active = (selected?.id ?? countable[0]?.id) === line.id;
                return (
                  <tr
                    key={line.id}
                    className={`cursor-pointer border-t border-[var(--border-default)] ${active ? 'bg-[var(--color-primary)]/5' : ''}`}
                    onClick={() => !disabled && selectLine(line.id)}
                  >
                    <td className="px-3 py-2 font-medium">{line.name}</td>
                    <td className="px-3 py-2 tabular-nums text-[var(--text-secondary)]">{formatLineDims(line)}</td>
                    <td className="px-3 py-2">{KIND_LABEL[line.kind]}</td>
                    <td className="px-3 py-2 tabular-nums">
                      {line.documentQty} {line.unit}
                    </td>
                    <td className="px-3 py-2">{t(STATUS_KEY[line.countStatus])}</td>
                  </tr>
                );
              })}
            </tbody>
          </table>
        </div>
      )}

      {selected ? (
        <section className="space-y-4 rounded-md border border-[var(--border-default)] px-3 py-3">
          <div>
            <h3 className="text-sm font-semibold tracking-tight">
              {selected.name} · {formatLineDims(selected)}
            </h3>
            <p className="text-xs text-[var(--text-muted)]">
              {KIND_LABEL[selected.kind]} · {t('wb.rcv.ops.count.documentQty')}: {selected.documentQty} {selected.unit}
              {selected.preAccept === 'conditional' ? ` · ${t('wb.rcv.ops.preAccept.conditional')}` : ''}
            </p>
          </div>

          {(selected.kind === 'lumber' ||
            selected.kind === 'packaged' ||
            selected.kind === 'thermowood' ||
            selected.kind === 'lamella') && (
            <div className="space-y-2">
              <h4 className="text-xs font-semibold uppercase text-[var(--text-muted)]">{t('wb.rcv.ops.count.packageMath')}</h4>
              <div className="flex flex-wrap items-end gap-2">
                <label className="block space-y-1 text-xs text-[var(--text-muted)]">
                  {t('wb.rcv.ops.count.packages')}
                  <Input
                    className="w-24"
                    value={selected.packages}
                    disabled={disabled}
                    onChange={(e) => patchSelected({ packages: e.target.value })}
                  />
                </label>
                <span className="pb-2 text-sm">×</span>
                <label className="block space-y-1 text-xs text-[var(--text-muted)]">
                  {t('wb.rcv.ops.count.perPackage')}
                  <Input
                    className="w-24"
                    value={selected.perPackage}
                    disabled={disabled}
                    onChange={(e) => patchSelected({ perPackage: e.target.value })}
                  />
                </label>
                <span className="pb-2 text-sm">=</span>
                <p className="pb-2 text-sm font-medium tabular-nums">{pkgTot ?? '—'}</p>
                <Button type="button" size="sm" variant="secondary" disabled={disabled || pkgTot == null} onClick={applyPackageToOperator}>
                  {t('wb.rcv.ops.count.applyPackage')}
                </Button>
              </div>
              <div className="space-y-2">
                <div className="flex items-center justify-between gap-2">
                  <p className="text-xs text-[var(--text-muted)]">{t('wb.rcv.ops.count.unevenPackages')}</p>
                  <Button type="button" size="sm" variant="secondary" disabled={disabled} onClick={addPackageRow}>
                    {t('wb.rcv.ops.count.addPackageRow')}
                  </Button>
                </div>
                {selected.packageRows.map((row, idx) => (
                  <div key={row.id} className="flex items-center gap-2">
                    <span className="text-xs text-[var(--text-muted)]">
                      {t('wb.rcv.ops.count.packageN').replace('{n}', String(idx + 1))}
                    </span>
                    <Input
                      className="w-24"
                      value={row.qty}
                      disabled={disabled}
                      onChange={(e) => {
                        const packageRows = selected.packageRows.map((r) =>
                          r.id === row.id ? { ...r, qty: e.target.value } : r,
                        );
                        patchSelected({ packageRows });
                      }}
                    />
                  </div>
                ))}
                {selected.packageRows.length > 0 ? (
                  <Button
                    type="button"
                    size="sm"
                    variant="secondary"
                    disabled={disabled || pkgTot == null}
                    onClick={applyPackageToOperator}
                  >
                    {t('wb.rcv.ops.count.applyPackageRows')}
                  </Button>
                ) : null}
              </div>
            </div>
          )}

          {selected.kind === 'log' ? (
            <div className="space-y-3">
              <h4 className="text-xs font-semibold uppercase text-[var(--text-muted)]">{t('wb.rcv.ops.count.logTitle')}</h4>
              <p className="text-xs text-[var(--text-muted)]">{t('wb.rcv.ops.count.logHint')}</p>
              <div className="flex flex-wrap gap-3">
                <label className="block space-y-1 text-xs text-[var(--text-muted)]">
                  {t('wb.rcv.ops.count.logCount')}
                  <Input
                    className="w-28"
                    value={selected.logCount}
                    disabled={disabled}
                    onChange={(e) =>
                      patchSelected({
                        logCount: e.target.value,
                        operatorQty: e.target.value,
                      })
                    }
                  />
                </label>
                <label className="block space-y-1 text-xs text-[var(--text-muted)]">
                  {t('wb.rcv.ops.count.logTotalM3')}
                  <Input
                    className="w-28"
                    value={selected.logTotalM3}
                    disabled={disabled}
                    onChange={(e) => patchSelected({ logTotalM3: e.target.value })}
                  />
                </label>
              </div>
            </div>
          ) : null}

          {(selected.kind === 'panel' || selected.kind === 'other') && (
            <p className="text-xs text-[var(--text-muted)]">{t('wb.rcv.ops.count.unitFieldsHint')}</p>
          )}

          <div className="grid gap-3 sm:grid-cols-3">
            <div className="rounded-md border border-[var(--border-default)] px-3 py-3">
              <p className="text-[10px] uppercase text-[var(--text-muted)]">{t('wb.rcv.ops.count.documentQty')}</p>
              <p className="mt-1 text-lg font-semibold tabular-nums">
                {selected.documentQty} {selected.unit}
              </p>
            </div>
            <div className="rounded-md border border-[var(--border-default)] px-3 py-3">
              <p className="text-[10px] uppercase text-[var(--text-muted)]">{t('wb.rcv.ops.count.aiQty')}</p>
              <Input
                className="mt-1"
                value={selected.aiQty}
                disabled={disabled}
                onChange={(e) => patchSelected({ aiQty: e.target.value })}
              />
              <Button
                type="button"
                size="sm"
                variant="secondary"
                className="mt-2"
                disabled={disabled || !selected.aiQty}
                onClick={acceptAiAsOperator}
              >
                {t('wb.rcv.ops.count.acceptAi')}
              </Button>
            </div>
            <div className="rounded-md border border-[var(--color-primary)]/30 bg-[var(--color-primary)]/5 px-3 py-3">
              <p className="text-[10px] uppercase text-[var(--text-muted)]">{t('wb.rcv.ops.count.operatorQty')}</p>
              <Input
                className="mt-1"
                value={selected.operatorQty}
                disabled={disabled}
                onChange={(e) => patchSelected({ operatorQty: e.target.value })}
              />
              <p className="mt-1 text-[10px] text-[var(--text-muted)]">{t('wb.rcv.ops.count.operatorWins')}</p>
              {vol != null ? (
                <p className="mt-2 text-xs tabular-nums text-[var(--text-secondary)]">
                  {t('wb.rcv.ops.count.autoVolume')}: {vol.toFixed(4)} m³
                </p>
              ) : null}
            </div>
          </div>

          <div className="rounded-md border border-[var(--border-default)] px-3 py-2 text-sm">
            {t('wb.rcv.ops.count.summaryFinal')}:{' '}
            <span className="font-semibold tabular-nums">
              {finalLineQty(selected) || '—'} {selected.unit}
            </span>
          </div>

          <div className="flex flex-wrap gap-2">
            <Button type="button" disabled={disabled || finalLineQty({ ...selected, operatorQty: selected.operatorQty || selected.aiQty }) <= 0} onClick={confirmSelected}>
              {t('wb.rcv.ops.count.confirmLine')}
            </Button>
            <Button type="button" variant="secondary" disabled={disabled} onClick={markRecheck}>
              {t('wb.rcv.ops.count.markRecheck')}
            </Button>
          </div>
        </section>
      ) : null}

      <label className="flex items-center gap-2 text-sm font-medium">
        <input type="checkbox" checked={allDone} disabled readOnly />
        {t('wb.rcv.ops.count.verifyAllLines')}
      </label>
      {!allDone && countable.length > 0 ? (
        <p className="text-xs text-[var(--color-danger)]">{t('wb.rcv.ops.count.needAllCounted')}</p>
      ) : null}
    </div>
  );
}

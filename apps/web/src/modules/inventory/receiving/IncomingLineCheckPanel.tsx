import { useMemo, useState } from 'react';
import { Link } from '@tanstack/react-router';
import { Button, Input } from '@naswood/ui';
import { useI18n } from '@/i18n';
import {
  formatDims,
  parseDefinitionDims,
  parseDimensions,
  rankMaterialMatches,
  type MaterialCandidate,
  type MaterialMatchResult,
} from './materialMatch';
import { formatMaterialCodeWithDims } from '@/modules/inventory/materials/materialNominalDims';
import {
  formatLineDims,
  lineChecksComplete,
  lineDimsChecked,
  lineHasDims,
  lineMatchLabel,
  lineMaterialMatched,
  lineMoistureChecked,
  type DimSample,
  type IncomingLine,
  type MoistureSample,
  type PreAcceptDecision,
} from './incomingLines';

type Props = {
  lines: IncomingLine[];
  onChange: (next: IncomingLine[]) => void;
  materialCandidates: MaterialCandidate[];
  disabled?: boolean;
  onRefreshMaterials?: () => void;
};

const KIND_LABEL: Record<IncomingLine['kind'], string> = {
  lumber: 'Kereste',
  log: 'Tomruk',
  lamella: 'Lamel',
  thermowood: 'Thermowood',
  panel: 'Masif Panel',
  packaged: 'Paketli',
  other: 'Diğer',
};

function num(v: string): number | null {
  const n = Number(String(v).replace(',', '.'));
  return Number.isFinite(n) ? n : null;
}

function deltaLabel(measured: string, target: number | null): string {
  const m = num(measured);
  if (m == null || target == null) return '—';
  const d = m - target;
  const sign = d > 0 ? '+' : '';
  return `${sign}${d.toFixed(1)}`;
}

function updateLine(lines: IncomingLine[], id: string, patch: Partial<IncomingLine>): IncomingLine[] {
  return lines.map((l) => (l.id === id ? { ...l, ...patch } : l));
}

/** Stage 2 — her gelen ürün: nem + ölçü + malzeme kartı + ön kabul. */
export function IncomingLineCheckPanel({
  lines,
  onChange,
  materialCandidates,
  disabled,
  onRefreshMaterials,
}: Props) {
  const { t } = useI18n();
  const [selectedId, setSelectedId] = useState<string | null>(lines[0]?.id ?? null);
  const [showPicker, setShowPicker] = useState(false);
  const [materialSearch, setMaterialSearch] = useState('');
  const selected = lines.find((l) => l.id === selectedId) ?? lines[0] ?? null;

  const matchLabel = selected ? lineMatchLabel(selected) : '';
  const rankedMatches = useMemo(
    () => (selected ? rankMaterialMatches(matchLabel, materialCandidates, undefined, 5) : []),
    [selected, matchLabel, materialCandidates],
  );
  const suggestedMatch: MaterialMatchResult | null = rankedMatches[0] ?? null;

  const pickerList = useMemo(() => {
    const q = materialSearch.trim().toLowerCase();
    if (!q) return materialCandidates.slice(0, 40);
    return materialCandidates
      .filter((m) => m.code.toLowerCase().includes(q) || m.name.toLowerCase().includes(q))
      .slice(0, 40);
  }, [materialCandidates, materialSearch]);

  if (lines.length === 0) {
    return <p className="text-sm text-[var(--text-muted)]">{t('wb.rcv.ops.check.noIncomingYet')}</p>;
  }

  function patchSelected(patch: Partial<IncomingLine>) {
    if (!selected) return;
    onChange(updateLine(lines, selected.id, patch));
  }

  function clearMatch() {
    patchSelected({
      matchConfirmed: false,
      matchedMaterialId: '',
      matchedMaterialCode: '',
      matchScore: null,
    });
  }

  function confirmMatch(result: MaterialMatchResult) {
    if (result.status === 'NO_MATCH') return;
    patchSelected({
      matchConfirmed: true,
      matchedMaterialId: result.material.id,
      matchedMaterialCode: result.material.code,
      matchScore: result.score,
    });
    setShowPicker(false);
  }

  function selectMasterMaterial(m: MaterialCandidate) {
    const scored = rankMaterialMatches(matchLabel, [m], undefined, 1)[0];
    patchSelected({
      matchConfirmed: true,
      matchedMaterialId: m.id,
      matchedMaterialCode: m.code,
      matchScore: scored?.score ?? 0,
    });
    setShowPicker(false);
  }

  function setDecision(preAccept: PreAcceptDecision) {
    if (!selected) return;
    patchSelected({ preAccept });
  }

  function createMaterialHref(line: IncomingLine): string {
    const q = new URLSearchParams();
    q.set('name', line.name);
    if (line.thicknessMm != null) q.set('thicknessMm', String(line.thicknessMm));
    if (line.widthMm != null) q.set('widthMm', String(line.widthMm));
    if (line.lengthMm != null) q.set('lengthMm', String(line.lengthMm));
    q.set('from', 'receiving');
    return `/inventory/master-data/define-material?${q.toString()}`;
  }

  function addMoisture() {
    if (!selected) return;
    const n = selected.moistureSamples.length + 1;
    const sample: MoistureSample = {
      id: `${selected.id}-m${Date.now()}`,
      label: `${t('wb.rcv.ops.check.sample')} ${n}`,
      valuePct: '',
    };
    patchSelected({ moistureSamples: [...selected.moistureSamples, sample] });
  }

  function addDim() {
    if (!selected || !lineHasDims(selected)) return;
    const n = selected.dimSamples.length + 1;
    const sample: DimSample = {
      id: `${selected.id}-d${Date.now()}`,
      label: `${t('wb.rcv.ops.check.sample')} ${n}`,
      thickness: '',
      width: '',
      length: '',
    };
    patchSelected({ dimSamples: [...selected.dimSamples, sample] });
  }

  const targetM = selected ? num(selected.targetMoisturePct) : null;
  const moistureVals =
    selected?.moistureSamples.map((s) => num(s.valuePct)).filter((n): n is number => n != null) ?? [];
  const moistureMin = moistureVals.length ? Math.min(...moistureVals) : null;
  const moistureMax = moistureVals.length ? Math.max(...moistureVals) : null;

  return (
    <section className="space-y-4">
      <div>
        <h3 className="text-sm font-semibold tracking-tight">{t('wb.rcv.ops.check.incomingLinesTitle')}</h3>
        <p className="text-xs text-[var(--text-muted)]">{t('wb.rcv.ops.check.perLineCheckHint')}</p>
      </div>

      <div className="overflow-x-auto rounded-md border border-[var(--border-default)]">
        <table className="w-full min-w-[520px] text-left text-sm">
          <thead className="bg-[var(--color-surface-hover)] text-[10px] uppercase text-[var(--text-muted)]">
            <tr>
              <th className="px-3 py-2">{t('wb.rcv.ops.count.colName')}</th>
              <th className="px-3 py-2">{t('wb.rcv.ops.count.colDims')}</th>
              <th className="px-3 py-2">{t('wb.rcv.ops.check.moisture')}</th>
              <th className="px-3 py-2">{t('wb.rcv.ops.check.dims')}</th>
              <th className="px-3 py-2">{t('wb.rcv.ops.check.materialMatch')}</th>
              <th className="px-3 py-2">{t('wb.rcv.ops.preAcceptTitle')}</th>
            </tr>
          </thead>
          <tbody>
            {lines.map((line) => {
              const active = (selected?.id ?? lines[0]?.id) === line.id;
              const moistOk = lineMoistureChecked(line);
              const dimOk = lineDimsChecked(line);
              return (
                <tr
                  key={line.id}
                  className={`cursor-pointer border-t border-[var(--border-default)] ${active ? 'bg-[var(--color-primary)]/5' : ''}`}
                  onClick={() => !disabled && setSelectedId(line.id)}
                >
                  <td className="px-3 py-2 font-medium">{line.name}</td>
                  <td className="px-3 py-2 tabular-nums text-[var(--text-secondary)]">{formatLineDims(line)}</td>
                  <td className="px-3 py-2 text-xs">
                    {moistOk ? t('wb.rcv.ops.check.sampleOk') : t('wb.rcv.ops.check.samplePending')}
                  </td>
                  <td className="px-3 py-2 text-xs">
                    {!lineHasDims(line)
                      ? t('wb.rcv.ops.check.dimsNa')
                      : dimOk
                        ? t('wb.rcv.ops.check.sampleOk')
                        : t('wb.rcv.ops.check.samplePending')}
                  </td>
                  <td className="px-3 py-2 text-xs font-mono">
                    {line.preAccept === 'reject'
                      ? '—'
                      : lineMaterialMatched(line)
                        ? formatMaterialCodeWithDims({
                            code: line.matchedMaterialCode,
                            definitionJson:
                              materialCandidates.find((m) => m.id === line.matchedMaterialId)
                                ?.definitionJson ?? null,
                          })
                        : t('wb.rcv.ops.check.samplePending')}
                  </td>
                  <td className="px-3 py-2 text-xs">
                    {line.preAccept === 'none'
                      ? '—'
                      : t(`wb.rcv.ops.preAccept.${line.preAccept}`)}
                    {lineChecksComplete(line) &&
                    (line.preAccept === 'reject' || lineMaterialMatched(line))
                      ? ' ✓'
                      : ''}
                  </td>
                </tr>
              );
            })}
          </tbody>
        </table>
      </div>

      {selected ? (
        <div className="space-y-5 rounded-md border border-[var(--border-default)] px-3 py-3">
          <div>
            <h4 className="text-sm font-semibold">
              {selected.name} · {formatLineDims(selected)}
            </h4>
            <p className="text-xs text-[var(--text-muted)]">
              {KIND_LABEL[selected.kind]} · {t('wb.rcv.ops.count.colDocQty')}: {selected.documentQty}{' '}
              {selected.unit}
            </p>
          </div>

          {/* Nem — bu satır */}
          <section className="space-y-3">
            <div className="flex flex-wrap items-end justify-between gap-2">
              <h5 className="text-xs font-semibold uppercase tracking-wide text-[var(--text-muted)]">
                {t('wb.rcv.ops.check.moisture')}
              </h5>
              <label className="flex items-center gap-2 text-xs text-[var(--text-muted)]">
                {t('wb.rcv.ops.check.targetMoisture')}
                <Input
                  className="w-20"
                  value={selected.targetMoisturePct}
                  disabled={disabled}
                  onChange={(e) => patchSelected({ targetMoisturePct: e.target.value })}
                />
                %
              </label>
            </div>
            <div className="overflow-x-auto rounded-md border border-[var(--border-default)]">
              <table className="w-full min-w-[320px] text-left text-sm">
                <thead className="bg-[var(--color-surface-hover)] text-[10px] uppercase text-[var(--text-muted)]">
                  <tr>
                    <th className="px-3 py-2">{t('wb.rcv.ops.check.sample')}</th>
                    <th className="px-3 py-2">{t('wb.rcv.ops.check.measuredPct')}</th>
                    <th className="px-3 py-2">{t('wb.rcv.ops.check.vsTarget')}</th>
                  </tr>
                </thead>
                <tbody>
                  {selected.moistureSamples.map((s, idx) => {
                    const v = num(s.valuePct);
                    const delta = v != null && targetM != null ? v - targetM : null;
                    return (
                      <tr key={s.id} className="border-t border-[var(--border-default)]">
                        <td className="px-3 py-2">
                          <Input
                            value={s.label}
                            disabled={disabled}
                            onChange={(e) => {
                              const moistureSamples = [...selected.moistureSamples];
                              moistureSamples[idx] = { ...s, label: e.target.value };
                              patchSelected({ moistureSamples });
                            }}
                          />
                        </td>
                        <td className="px-3 py-2">
                          <Input
                            value={s.valuePct}
                            disabled={disabled}
                            onChange={(e) => {
                              const moistureSamples = [...selected.moistureSamples];
                              moistureSamples[idx] = { ...s, valuePct: e.target.value };
                              patchSelected({ moistureSamples });
                            }}
                          />
                        </td>
                        <td className="px-3 py-2 tabular-nums text-xs text-[var(--text-muted)]">
                          {delta == null ? '—' : `${delta > 0 ? '+' : ''}${delta.toFixed(1)} pp`}
                        </td>
                      </tr>
                    );
                  })}
                </tbody>
              </table>
            </div>
            <div className="flex flex-wrap items-center gap-2">
              <Button type="button" size="sm" variant="secondary" disabled={disabled} onClick={addMoisture}>
                {t('wb.rcv.ops.check.addSample')}
              </Button>
              {moistureMin != null && moistureMax != null ? (
                <p className="text-xs text-[var(--text-muted)]">
                  {t('wb.rcv.ops.check.moistureRange')
                    .replace('{min}', moistureMin.toFixed(1))
                    .replace('{max}', moistureMax.toFixed(1))}
                </p>
              ) : null}
            </div>
          </section>

          {/* Ölçü — bu satır; hedef belgeden */}
          {lineHasDims(selected) ? (
            <section className="space-y-3">
              <h5 className="text-xs font-semibold uppercase tracking-wide text-[var(--text-muted)]">
                {t('wb.rcv.ops.check.dims')}
              </h5>
              <p className="text-xs text-[var(--text-muted)]">
                {t('wb.rcv.ops.check.ordered')}:{' '}
                <span className="font-medium tabular-nums text-[var(--text-secondary)]">
                  {formatLineDims(selected)}
                </span>{' '}
                — {t('wb.rcv.ops.check.dimsFromDoc')}
              </p>
              <div className="overflow-x-auto rounded-md border border-[var(--border-default)]">
                <table className="w-full min-w-[480px] text-left text-sm">
                  <thead className="bg-[var(--color-surface-hover)] text-[10px] uppercase text-[var(--text-muted)]">
                    <tr>
                      <th className="px-3 py-2">{t('wb.rcv.ops.check.sample')}</th>
                      <th className="px-3 py-2">{t('wb.rcv.ops.check.thickness')}</th>
                      <th className="px-3 py-2">{t('wb.rcv.ops.check.width')}</th>
                      <th className="px-3 py-2">{t('wb.rcv.ops.check.length')}</th>
                      <th className="px-3 py-2">{t('wb.rcv.ops.check.delta')}</th>
                    </tr>
                  </thead>
                  <tbody>
                    {selected.dimSamples.map((s, idx) => (
                      <tr key={s.id} className="border-t border-[var(--border-default)]">
                        <td className="px-3 py-2">
                          <Input
                            value={s.label}
                            disabled={disabled}
                            onChange={(e) => {
                              const dimSamples = [...selected.dimSamples];
                              dimSamples[idx] = { ...s, label: e.target.value };
                              patchSelected({ dimSamples });
                            }}
                          />
                        </td>
                        {(['thickness', 'width', 'length'] as const).map((field) => (
                          <td key={field} className="px-3 py-2">
                            <Input
                              value={s[field]}
                              disabled={disabled}
                              onChange={(e) => {
                                const dimSamples = [...selected.dimSamples];
                                dimSamples[idx] = { ...s, [field]: e.target.value };
                                patchSelected({ dimSamples });
                              }}
                            />
                          </td>
                        ))}
                        <td className="px-3 py-2 text-[10px] tabular-nums text-[var(--text-muted)]">
                          T {deltaLabel(s.thickness, selected.thicknessMm)} · W{' '}
                          {deltaLabel(s.width, selected.widthMm)} · L{' '}
                          {deltaLabel(s.length, selected.lengthMm)}
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
              <Button type="button" size="sm" variant="secondary" disabled={disabled} onClick={addDim}>
                {t('wb.rcv.ops.check.addSample')}
              </Button>
            </section>
          ) : (
            <p className="text-xs text-[var(--text-muted)]">{t('wb.rcv.ops.check.dimsSkipLog')}</p>
          )}

          {/* Malzeme kartı — bu satır */}
          <section className="space-y-3">
            <h5 className="text-xs font-semibold uppercase tracking-wide text-[var(--text-muted)]">
              {t('wb.rcv.ops.check.materialMatch')}
            </h5>
            <p className="text-xs text-[var(--text-muted)]">{t('wb.rcv.ops.check.materialMatchPerLineHint')}</p>
            <p className="rounded-md border border-[var(--border-default)] px-3 py-2 text-sm">
              <span className="text-[10px] uppercase text-[var(--text-muted)]">{t('wb.rcv.ops.incomingLabel')}</span>
              <br />
              <span className="font-medium">{matchLabel}</span>
            </p>

            {materialCandidates.length === 0 ? (
              <p className="text-sm text-[var(--color-danger)]">{t('wb.rcv.noMaterialsInMaster')}</p>
            ) : suggestedMatch ? (
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
                    size="sm"
                    disabled={disabled || suggestedMatch.status === 'NO_MATCH'}
                    onClick={() => confirmMatch(suggestedMatch)}
                  >
                    {t('wb.rcv.confirmMatch')}
                  </Button>
                  <Button
                    type="button"
                    size="sm"
                    variant="secondary"
                    disabled={disabled}
                    onClick={() => {
                      setShowPicker(true);
                      clearMatch();
                    }}
                  >
                    {t('wb.rcv.pickOtherMaterial')}
                  </Button>
                </div>
                {selected.matchConfirmed ? (
                  <p className="mt-2 text-sm font-medium text-[var(--color-primary)]">
                    {t('wb.rcv.matchConfirmedBanner')} ·{' '}
                    {formatMaterialCodeWithDims({
                      code: selected.matchedMaterialCode,
                      definitionJson:
                        materialCandidates.find((m) => m.id === selected.matchedMaterialId)
                          ?.definitionJson ?? null,
                    })}
                    {selected.matchScore != null ? ` · %${selected.matchScore}` : ''}
                  </p>
                ) : null}
              </div>
            ) : (
              <p className="text-sm text-[var(--color-danger)]">{t('wb.rcv.ops.check.noMatchForLine')}</p>
            )}

            {showPicker ? (
              <div className="space-y-2 rounded-md border border-[var(--border-default)] px-3 py-3">
                <Input
                  value={materialSearch}
                  disabled={disabled}
                  placeholder={t('wb.rcv.materialSearchPlaceholder')}
                  onChange={(e) => setMaterialSearch(e.target.value)}
                />
                <ul className="max-h-48 space-y-1 overflow-y-auto text-sm">
                  {pickerList.map((m) => (
                    <li key={m.id}>
                      <button
                        type="button"
                        disabled={disabled}
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

            <div className="flex flex-wrap gap-2">
              <a
                href={createMaterialHref(selected)}
                className="inline-flex h-8 items-center rounded-md border border-[var(--color-primary)]/40 bg-[var(--color-primary)]/5 px-3 text-xs font-medium text-[var(--color-primary)] hover:bg-[var(--color-primary)]/10"
                target="_blank"
                rel="noreferrer"
              >
                {t('wb.rcv.ops.check.createMaterialForLine')}
              </a>
              <Link
                to="/inventory/master-data/materials"
                className="inline-flex h-8 items-center rounded-md border border-[var(--border-default)] px-3 text-xs font-medium hover:bg-[var(--color-surface-hover)]"
                target="_blank"
              >
                {t('wb.rcv.ops.check.openMaterialMenu')}
              </Link>
              <Button
                type="button"
                size="sm"
                variant="secondary"
                disabled={disabled}
                onClick={() => onRefreshMaterials?.()}
              >
                {t('wb.rcv.ops.check.refreshMaterials')}
              </Button>
            </div>
            <p className="text-[10px] text-[var(--text-muted)]">{t('wb.rcv.ops.check.createMaterialHint')}</p>
          </section>

          {/* Ön kabul — bu satır */}
          <section className="space-y-2">
            <h5 className="text-xs font-semibold uppercase tracking-wide text-[var(--text-muted)]">
              {t('wb.rcv.ops.preAcceptTitle')}
            </h5>
            <p className="text-xs text-[var(--text-muted)]">{t('wb.rcv.ops.check.preAcceptHint')}</p>
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
                  variant={selected.preAccept === id ? 'default' : 'secondary'}
                  disabled={disabled}
                  onClick={() => setDecision(id)}
                >
                  {label}
                </Button>
              ))}
            </div>
            {!lineMoistureChecked(selected) || !lineDimsChecked(selected) ? (
              <p className="text-xs text-[var(--color-danger)]">{t('wb.rcv.ops.check.needSamplesBeforeAccept')}</p>
            ) : selected.preAccept !== 'reject' && !lineMaterialMatched(selected) ? (
              <p className="text-xs text-[var(--color-danger)]">{t('wb.rcv.ops.check.needMaterialBeforeAdvance')}</p>
            ) : null}
          </section>
        </div>
      ) : null}
    </section>
  );
}

import { Button, Input } from '@naswood/ui';
import { useI18n } from '@/i18n';
import type { ReactNode } from 'react';

export type PreAcceptDecision = 'none' | 'ok' | 'conditional' | 'reject';
export type QualityVerdict = 'none' | 'pass' | 'conditional' | 'fail';

export type MoistureSample = { id: string; label: string; valuePct: string };
export type DimSample = {
  id: string;
  label: string;
  thickness: string;
  width: string;
  length: string;
};

export type MaterialCheckState = {
  targetMoisturePct: string;
  moistureSamples: MoistureSample[];
  qualityVerdict: QualityVerdict;
  qualityFlags: Record<string, boolean>;
  qualityNotes: string;
  targetThickness: string;
  targetWidth: string;
  targetLength: string;
  dimSamples: DimSample[];
  qualityPhotos: Partial<Record<'defect' | 'measure' | 'moisture' | 'other', boolean>>;
  preAccept: PreAcceptDecision;
};

export const QUALITY_FLAGS = [
  'knot',
  'crack',
  'color',
  'surface',
  'damage',
  'packaging',
] as const;

export type QualityFlag = (typeof QUALITY_FLAGS)[number];

const QUALITY_PHOTO_SLOTS = ['defect', 'measure', 'moisture', 'other'] as const;

export function createDefaultMaterialCheck(): MaterialCheckState {
  return {
    targetMoisturePct: '12',
    moistureSamples: [
      { id: 'm1', label: 'Numune 1', valuePct: '11.8' },
      { id: 'm2', label: 'Numune 2', valuePct: '12.4' },
      { id: 'm3', label: 'Numune 3', valuePct: '13.1' },
      { id: 'm4', label: 'Numune 4', valuePct: '12.0' },
    ],
    qualityVerdict: 'none',
    qualityFlags: {},
    qualityNotes: '',
    targetThickness: '26',
    targetWidth: '140',
    targetLength: '3000',
    dimSamples: [
      { id: 'd1', label: 'Numune 1', thickness: '25.7', width: '139.2', length: '2998' },
      { id: 'd2', label: 'Numune 2', thickness: '26.0', width: '140.1', length: '3001' },
    ],
    qualityPhotos: {},
    preAccept: 'none',
  };
}

function num(v: string): number | null {
  const n = Number(String(v).replace(',', '.'));
  return Number.isFinite(n) ? n : null;
}

function deltaLabel(measured: string, target: string): string {
  const m = num(measured);
  const t = num(target);
  if (m == null || t == null) return '—';
  const d = m - t;
  const sign = d > 0 ? '+' : '';
  return `${sign}${d.toFixed(1)}`;
}

type Props = {
  value: MaterialCheckState;
  onChange: (next: MaterialCheckState) => void;
  disabled?: boolean;
  /** Slot below checks for material-card matching UI (kept in parent). */
  materialMatchSlot?: ReactNode;
};

/** Stage 2 — Malzeme kontrolü: nem, kalite, ölçü, foto, ön kabul. */
export function MaterialCheckStep({ value, onChange, disabled, materialMatchSlot }: Props) {
  const { t } = useI18n();

  function patch(partial: Partial<MaterialCheckState>) {
    onChange({ ...value, ...partial });
  }

  function addMoisture() {
    const n = value.moistureSamples.length + 1;
    patch({
      moistureSamples: [
        ...value.moistureSamples,
        { id: `m${Date.now()}`, label: `${t('wb.rcv.ops.check.sample')} ${n}`, valuePct: '' },
      ],
    });
  }

  function addDim() {
    const n = value.dimSamples.length + 1;
    patch({
      dimSamples: [
        ...value.dimSamples,
        {
          id: `d${Date.now()}`,
          label: `${t('wb.rcv.ops.check.sample')} ${n}`,
          thickness: '',
          width: '',
          length: '',
        },
      ],
    });
  }

  const moistureVals = value.moistureSamples
    .map((s) => num(s.valuePct))
    .filter((n): n is number => n != null);
  const moistureMin = moistureVals.length ? Math.min(...moistureVals) : null;
  const moistureMax = moistureVals.length ? Math.max(...moistureVals) : null;
  const targetM = num(value.targetMoisturePct);

  return (
    <div className="space-y-6">
      <p className="text-sm text-[var(--text-secondary)]">{t('wb.rcv.ops.check.intro')}</p>

      {/* Nem */}
      <section className="space-y-3">
        <div className="flex flex-wrap items-end justify-between gap-2">
          <h3 className="text-sm font-semibold tracking-tight">{t('wb.rcv.ops.check.moisture')}</h3>
          <label className="flex items-center gap-2 text-xs text-[var(--text-muted)]">
            {t('wb.rcv.ops.check.targetMoisture')}
            <Input
              className="w-20"
              value={value.targetMoisturePct}
              disabled={disabled}
              onChange={(e) => patch({ targetMoisturePct: e.target.value })}
            />
            %
          </label>
        </div>
        <div className="overflow-x-auto rounded-md border border-[var(--border-default)]">
          <table className="w-full min-w-[360px] text-left text-sm">
            <thead className="bg-[var(--color-surface-hover)] text-[10px] uppercase text-[var(--text-muted)]">
              <tr>
                <th className="px-3 py-2">{t('wb.rcv.ops.check.sample')}</th>
                <th className="px-3 py-2">{t('wb.rcv.ops.check.measuredPct')}</th>
                <th className="px-3 py-2">{t('wb.rcv.ops.check.vsTarget')}</th>
              </tr>
            </thead>
            <tbody>
              {value.moistureSamples.map((s, idx) => {
                const v = num(s.valuePct);
                const delta = v != null && targetM != null ? v - targetM : null;
                return (
                  <tr key={s.id} className="border-t border-[var(--border-default)]">
                    <td className="px-3 py-2">
                      <Input
                        value={s.label}
                        disabled={disabled}
                        onChange={(e) => {
                          const next = [...value.moistureSamples];
                          next[idx] = { ...s, label: e.target.value };
                          patch({ moistureSamples: next });
                        }}
                      />
                    </td>
                    <td className="px-3 py-2">
                      <Input
                        value={s.valuePct}
                        disabled={disabled}
                        onChange={(e) => {
                          const next = [...value.moistureSamples];
                          next[idx] = { ...s, valuePct: e.target.value };
                          patch({ moistureSamples: next });
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

      {/* Kalite */}
      <section className="space-y-3">
        <h3 className="text-sm font-semibold tracking-tight">{t('wb.rcv.ops.check.quality')}</h3>
        <div className="flex flex-wrap gap-2">
          {(
            [
              ['pass', t('wb.rcv.ops.check.qualityPass')],
              ['conditional', t('wb.rcv.ops.check.qualityConditional')],
              ['fail', t('wb.rcv.ops.check.qualityFail')],
            ] as const
          ).map(([id, label]) => (
            <Button
              key={id}
              type="button"
              size="sm"
              variant={value.qualityVerdict === id ? 'default' : 'secondary'}
              disabled={disabled}
              onClick={() => patch({ qualityVerdict: id })}
            >
              {label}
            </Button>
          ))}
        </div>
        <div className="flex flex-wrap gap-2">
          {QUALITY_FLAGS.map((f) => {
            const on = !!value.qualityFlags[f];
            return (
              <button
                key={f}
                type="button"
                disabled={disabled}
                onClick={() =>
                  patch({ qualityFlags: { ...value.qualityFlags, [f]: !value.qualityFlags[f] } })
                }
                className={`rounded-md border px-2.5 py-1 text-xs ${
                  on
                    ? 'border-[var(--color-danger)] bg-[var(--color-danger)]/10 text-[var(--color-danger)]'
                    : 'border-[var(--border-default)] text-[var(--text-secondary)]'
                }`}
              >
                {t(`wb.rcv.ops.check.flag.${f}`)}
              </button>
            );
          })}
        </div>
        <label className="block space-y-1 text-sm">
          <span className="text-[var(--text-secondary)]">{t('wb.rcv.ops.check.notes')}</span>
          <Input
            value={value.qualityNotes}
            disabled={disabled}
            onChange={(e) => patch({ qualityNotes: e.target.value })}
          />
        </label>
      </section>

      {/* Ölçü */}
      <section className="space-y-3">
        <h3 className="text-sm font-semibold tracking-tight">{t('wb.rcv.ops.check.dims')}</h3>
        <div className="grid gap-2 sm:grid-cols-3">
          {(
            [
              ['targetThickness', t('wb.rcv.ops.check.thickness')],
              ['targetWidth', t('wb.rcv.ops.check.width')],
              ['targetLength', t('wb.rcv.ops.check.length')],
            ] as const
          ).map(([key, label]) => (
            <label key={key} className="block space-y-1 text-xs text-[var(--text-muted)]">
              {t('wb.rcv.ops.check.ordered')} {label} (mm)
              <Input
                value={value[key]}
                disabled={disabled}
                onChange={(e) => patch({ [key]: e.target.value })}
              />
            </label>
          ))}
        </div>
        <div className="overflow-x-auto rounded-md border border-[var(--border-default)]">
          <table className="w-full min-w-[520px] text-left text-sm">
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
              {value.dimSamples.map((s, idx) => (
                <tr key={s.id} className="border-t border-[var(--border-default)]">
                  <td className="px-3 py-2">
                    <Input
                      value={s.label}
                      disabled={disabled}
                      onChange={(e) => {
                        const next = [...value.dimSamples];
                        next[idx] = { ...s, label: e.target.value };
                        patch({ dimSamples: next });
                      }}
                    />
                  </td>
                  {(['thickness', 'width', 'length'] as const).map((field) => (
                    <td key={field} className="px-3 py-2">
                      <Input
                        value={s[field]}
                        disabled={disabled}
                        onChange={(e) => {
                          const next = [...value.dimSamples];
                          next[idx] = { ...s, [field]: e.target.value };
                          patch({ dimSamples: next });
                        }}
                      />
                    </td>
                  ))}
                  <td className="px-3 py-2 text-[10px] tabular-nums text-[var(--text-muted)]">
                    T {deltaLabel(s.thickness, value.targetThickness)} · W{' '}
                    {deltaLabel(s.width, value.targetWidth)} · L {deltaLabel(s.length, value.targetLength)}
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

      {/* Foto */}
      <section className="space-y-3">
        <h3 className="text-sm font-semibold tracking-tight">{t('wb.rcv.ops.check.photos')}</h3>
        <p className="text-xs text-[var(--text-muted)]">{t('wb.rcv.ops.check.photosHint')}</p>
        <div className="grid gap-2 sm:grid-cols-2">
          {QUALITY_PHOTO_SLOTS.map((slot) => {
            const on = !!value.qualityPhotos[slot];
            return (
              <button
                key={slot}
                type="button"
                disabled={disabled}
                onClick={() =>
                  patch({ qualityPhotos: { ...value.qualityPhotos, [slot]: !value.qualityPhotos[slot] } })
                }
                className={`rounded-md border px-3 py-2 text-left text-sm ${
                  on
                    ? 'border-[var(--color-primary)] bg-[var(--color-primary)]/5'
                    : 'border-[var(--border-default)]'
                }`}
              >
                <p className="font-medium">{t(`wb.rcv.ops.check.photo.${slot}`)}</p>
                <p className="text-xs text-[var(--text-muted)]">
                  {on ? t('wb.rcv.ops.photoCaptured') : t('wb.rcv.ops.photoCapture')}
                </p>
              </button>
            );
          })}
        </div>
      </section>

      {/* Ön kabul */}
      <section className="space-y-3">
        <h3 className="text-sm font-semibold tracking-tight">{t('wb.rcv.ops.preAcceptTitle')}</h3>
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
              variant={value.preAccept === id ? 'default' : 'secondary'}
              disabled={disabled}
              onClick={() => patch({ preAccept: id })}
            >
              {label}
            </Button>
          ))}
        </div>
      </section>

      {materialMatchSlot ? (
        <section className="space-y-3 border-t border-[var(--border-default)] pt-4">
          <h3 className="text-sm font-semibold tracking-tight">{t('wb.rcv.ops.check.materialMatch')}</h3>
          <p className="text-xs text-[var(--text-muted)]">{t('wb.rcv.ops.check.materialMatchHint')}</p>
          {materialMatchSlot}
        </section>
      ) : null}
    </div>
  );
}

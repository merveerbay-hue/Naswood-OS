import { Link, useNavigate } from '@tanstack/react-router';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { useMemo, useState } from 'react';
import { Button, Card, CardContent, CardDescription, CardHeader, CardTitle, Input } from '@naswood/ui';
import { createResource } from '@/api/business';
import { useI18n } from '@/i18n';

/**
 * INV-005 / INV-MAT-001 — Material Definition Designer
 * Authority: Material_Definition_Architecture.md
 * NOT a passive Material Master Create form.
 */

type PackId =
  | 'general'
  | 'identity'
  | 'measurement'
  | 'conversion'
  | 'packaging'
  | 'numbering'
  | 'quality'
  | 'traceability'
  | 'costing'
  | 'release';

type DefState = {
  name: string;
  category: string;
  materialType: string;
  species: string;
  thicknessMm: string;
  widthMm: string;
  lengthMm: string;
  density: string;
  moistureBasis: string;
  stockUom: string;
  purchaseUom: string;
  productionUom: string;
  salesUom: string;
  planningUom: string;
  costingUom: string;
  shippingUom: string;
  piecesPerPackage: string;
  packageLabel: string;
  identityClass: string;
  lotPolicy: string;
  numberingSeries: string;
  grade: string;
  moistureMin: string;
  moistureMax: string;
  inspectionPlan: string;
  genealogyRequired: boolean;
  cocRequired: boolean;
  evidenceRequired: boolean;
  costingDriver: string;
  valuationClass: string;
  notes: string;
};

const PACKS: { id: PackId; titleKey: string; hintKey: string }[] = [
  { id: 'general', titleKey: 'md.pack.general', hintKey: 'md.pack.generalHint' },
  { id: 'identity', titleKey: 'md.pack.identity', hintKey: 'md.pack.identityHint' },
  { id: 'measurement', titleKey: 'md.pack.measurement', hintKey: 'md.pack.measurementHint' },
  { id: 'conversion', titleKey: 'md.pack.conversion', hintKey: 'md.pack.conversionHint' },
  { id: 'packaging', titleKey: 'md.pack.packaging', hintKey: 'md.pack.packagingHint' },
  { id: 'numbering', titleKey: 'md.pack.numbering', hintKey: 'md.pack.numberingHint' },
  { id: 'quality', titleKey: 'md.pack.quality', hintKey: 'md.pack.qualityHint' },
  { id: 'traceability', titleKey: 'md.pack.traceability', hintKey: 'md.pack.traceabilityHint' },
  { id: 'costing', titleKey: 'md.pack.costing', hintKey: 'md.pack.costingHint' },
  { id: 'release', titleKey: 'md.pack.release', hintKey: 'md.pack.releaseHint' },
];

const DEFAULT_DEF: DefState = {
  name: 'Thermowood Deck 26×140×4000',
  category: 'Finished Product',
  materialType: 'Lumber / Profile',
  species: 'Sarıçam Thermowood',
  thicknessMm: '26',
  widthMm: '140',
  lengthMm: '4000',
  density: '480',
  moistureBasis: '%6',
  stockUom: 'Piece',
  purchaseUom: 'Piece',
  productionUom: 'm²',
  salesUom: 'Linear meter',
  planningUom: 'Piece',
  costingUom: 'm³',
  shippingUom: 'kg',
  piecesPerPackage: '120',
  packageLabel: 'Standart paket',
  identityClass: 'FG-TWDECK',
  lotPolicy: 'Lot operational · MI lifelong',
  numberingSeries: 'MAT-… / MI class FG-TWDECK / PKG-…',
  grade: 'A',
  moistureMin: '4',
  moistureMax: '8',
  inspectionPlan: 'Inbound grade + moisture',
  genealogyRequired: true,
  cocRequired: true,
  evidenceRequired: true,
  costingDriver: 'm³',
  valuationClass: 'FG-WOOD',
  notes: '',
};

function num(v: string) {
  const n = Number(v);
  return Number.isFinite(n) ? n : 0;
}

function readMintedCode(row: Record<string, unknown> | null | undefined): string | null {
  if (!row) return null;
  for (const key of ['code', 'number', 'Code', 'Number']) {
    const v = row[key];
    if (typeof v === 'string' && v.trim()) return v.trim();
  }
  return null;
}

export function MaterialDefinitionDesigner() {
  const { t } = useI18n();
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const [packIdx, setPackIdx] = useState(0);
  const [maxReached, setMaxReached] = useState(0);
  const [def, setDef] = useState<DefState>(DEFAULT_DEF);
  const [approved, setApproved] = useState(false);
  const [released, setReleased] = useState(false);
  const [mintedCode, setMintedCode] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);

  const pack = PACKS[packIdx];
  const progress = Math.round(((packIdx + (released ? 1 : 0)) / PACKS.length) * 100);

  const equivalents = useMemo(() => {
    const tMm = num(def.thicknessMm);
    const wMm = num(def.widthMm);
    const lMm = num(def.lengthMm);
    const dens = num(def.density);
    const pcs = 1;
    const lm = pcs * (lMm / 1000);
    const m2 = pcs * (wMm / 1000) * (lMm / 1000);
    const m3 = pcs * (tMm / 1000) * (wMm / 1000) * (lMm / 1000);
    const kg = m3 * dens;
    return {
      pcs,
      lm: Number(lm.toFixed(3)),
      m2: Number(m2.toFixed(4)),
      m3: Number(m3.toFixed(6)),
      kg: Number(kg.toFixed(2)),
      t: Number((kg / 1000).toFixed(4)),
    };
  }, [def.thicknessMm, def.widthMm, def.lengthMm, def.density]);

  const gateMessage = useMemo(() => {
    if (pack.id === 'general' && !def.name.trim()) return t('md.gateNeedName');
    if (pack.id === 'measurement' && (!def.thicknessMm || !def.widthMm || !def.lengthMm)) {
      return t('md.gateNeedDims');
    }
    if (pack.id === 'conversion' && !def.stockUom) return t('md.gateNeedStockUom');
    if (pack.id === 'release' && !approved) return t('md.gateNeedApprove');
    return null;
  }, [pack.id, def, approved, t]);

  const persistMutation = useMutation({
    mutationFn: async () => {
      const description = [
        def.species,
        `${def.thicknessMm}×${def.widthMm}×${def.lengthMm}`,
        `Stock=${def.stockUom}`,
        `Sales=${def.salesUom}`,
        `Prod=${def.productionUom}`,
        `Cost=${def.costingUom}`,
        `Pkg=${def.piecesPerPackage}pcs`,
        `MI=${def.identityClass}`,
        `Grade=${def.grade}`,
        def.notes,
      ]
        .filter(Boolean)
        .join(' · ');
      return createResource<Record<string, unknown>>('materials', {
        name: def.name.trim(),
        description,
        category: def.category || def.materialType || 'Raw',
        unitOfMeasure: def.stockUom || 'Piece',
        status: 'Active',
        code: '',
      });
    },
    onSuccess: async (created) => {
      setError(null);
      setMintedCode(readMintedCode(created));
      setReleased(true);
      await queryClient.invalidateQueries({ queryKey: ['business', 'materials'] });
    },
    onError: (e: Error) => setError(e.message),
  });

  function setField<K extends keyof DefState>(key: K, value: DefState[K]) {
    setDef((d) => ({ ...d, [key]: value }));
  }

  function goNext() {
    if (gateMessage) {
      setError(gateMessage);
      return;
    }
    setError(null);
    if (pack.id === 'release') {
      persistMutation.mutate();
      return;
    }
    const next = Math.min(packIdx + 1, PACKS.length - 1);
    setPackIdx(next);
    setMaxReached((m) => Math.max(m, next));
  }

  function Field({
    label,
    value,
    onChange,
    placeholder,
    readOnly,
  }: {
    label: string;
    value: string;
    onChange?: (v: string) => void;
    placeholder?: string;
    readOnly?: boolean;
  }) {
    return (
      <label className="block space-y-1">
        <span className="text-xs font-medium text-[var(--text-muted)]">{label}</span>
        <Input
          value={value}
          readOnly={readOnly}
          placeholder={placeholder}
          onChange={(e) => onChange?.(e.target.value)}
          className={readOnly ? 'bg-[var(--color-surface-hover)] font-mono text-sm' : ''}
        />
      </label>
    );
  }

  return (
    <div className="space-y-4">
      <div className="flex flex-wrap items-start justify-between gap-3">
        <div>
          <p className="text-xs font-medium text-[var(--text-muted)]">INV-MAT-001 · Designer</p>
          <h2 className="text-xl font-semibold tracking-tight">{t('md.title')}</h2>
          <p className="mt-1 max-w-2xl text-sm text-[var(--text-secondary)]">{t('md.desc')}</p>
          <p className="mt-1 text-xs text-[var(--text-muted)]">{t('md.screenType')}</p>
        </div>
        <div className="flex flex-col items-end gap-2">
          <div className="rounded-md border border-[var(--border-default)] bg-[var(--color-surface)] px-3 py-2 text-right">
            <p className="text-[10px] uppercase tracking-wide text-[var(--text-muted)]">{t('wizard.systemCode')}</p>
            <p className="font-mono text-sm font-medium">{mintedCode ?? t('wizard.autoGenerated')}</p>
            <p className="text-[10px] text-[var(--text-muted)]">MAT-…</p>
          </div>
          <Link
            to="/inventory/master-data/materials"
            className="text-sm font-medium text-[var(--color-primary)] hover:underline"
          >
            {t('md.backLibrary')}
          </Link>
        </div>
      </div>

      <div className="h-1.5 overflow-hidden rounded-full bg-[var(--color-surface-hover)]">
        <div className="h-full bg-[var(--color-primary)] transition-all" style={{ width: `${progress}%` }} />
      </div>

      <div className="grid gap-4 lg:grid-cols-[240px_minmax(0,1fr)_280px]">
        <aside className="space-y-1 rounded-lg border border-[var(--border-default)] bg-[var(--color-surface)] p-2">
          <p className="px-2 py-1 text-[10px] font-semibold uppercase tracking-wide text-[var(--text-muted)]">
            {t('md.rulePacks')}
          </p>
          {PACKS.map((p, i) => (
            <button
              key={p.id}
              type="button"
              disabled={i > maxReached && !released}
              onClick={() => i <= maxReached && setPackIdx(i)}
              className={`flex w-full items-center gap-2 rounded-md px-2 py-2 text-left text-sm ${
                i === packIdx
                  ? 'bg-[var(--color-primary)] text-white'
                  : i <= maxReached
                    ? 'hover:bg-[var(--color-surface-hover)]'
                    : 'opacity-40'
              }`}
            >
              <span className="font-mono text-[10px] opacity-70">{String(i + 1).padStart(2, '0')}</span>
              <span className="font-medium">{t(p.titleKey)}</span>
            </button>
          ))}
        </aside>

        <main>
          <Card>
            <CardHeader className="pb-2">
              <CardTitle className="text-base">{t(pack.titleKey)}</CardTitle>
              <CardDescription>{t(pack.hintKey)}</CardDescription>
            </CardHeader>
            <CardContent className="space-y-4">
              {pack.id === 'general' ? (
                <div className="grid gap-3 md:grid-cols-2">
                  <Field label={t('md.fields.name')} value={def.name} onChange={(v) => setField('name', v)} />
                  <Field
                    label={t('md.fields.category')}
                    value={def.category}
                    onChange={(v) => setField('category', v)}
                  />
                  <Field
                    label={t('md.fields.materialType')}
                    value={def.materialType}
                    onChange={(v) => setField('materialType', v)}
                  />
                  <Field
                    label={t('md.fields.species')}
                    value={def.species}
                    onChange={(v) => setField('species', v)}
                  />
                </div>
              ) : null}

              {pack.id === 'identity' ? (
                <div className="space-y-3">
                  <Field
                    label={t('md.fields.identityClass')}
                    value={def.identityClass}
                    onChange={(v) => setField('identityClass', v)}
                    placeholder="FG-TWDECK"
                  />
                  <Field
                    label={t('md.fields.lotPolicy')}
                    value={def.lotPolicy}
                    onChange={(v) => setField('lotPolicy', v)}
                  />
                  <p className="rounded-md border border-[var(--border-default)] px-3 py-2 text-xs text-[var(--text-secondary)]">
                    {t('md.identityLaw')}
                  </p>
                </div>
              ) : null}

              {pack.id === 'measurement' ? (
                <div className="grid gap-3 md:grid-cols-3">
                  <Field
                    label={t('md.fields.thickness')}
                    value={def.thicknessMm}
                    onChange={(v) => setField('thicknessMm', v)}
                  />
                  <Field
                    label={t('md.fields.width')}
                    value={def.widthMm}
                    onChange={(v) => setField('widthMm', v)}
                  />
                  <Field
                    label={t('md.fields.length')}
                    value={def.lengthMm}
                    onChange={(v) => setField('lengthMm', v)}
                  />
                  <Field
                    label={t('md.fields.density')}
                    value={def.density}
                    onChange={(v) => setField('density', v)}
                  />
                  <Field
                    label={t('md.fields.moistureBasis')}
                    value={def.moistureBasis}
                    onChange={(v) => setField('moistureBasis', v)}
                  />
                </div>
              ) : null}

              {pack.id === 'conversion' ? (
                <div className="space-y-3">
                  <div className="grid gap-3 md:grid-cols-2">
                    <Field
                      label={t('md.fields.stockUom')}
                      value={def.stockUom}
                      onChange={(v) => setField('stockUom', v)}
                    />
                    <Field
                      label={t('md.fields.purchaseUom')}
                      value={def.purchaseUom}
                      onChange={(v) => setField('purchaseUom', v)}
                    />
                    <Field
                      label={t('md.fields.productionUom')}
                      value={def.productionUom}
                      onChange={(v) => setField('productionUom', v)}
                    />
                    <Field
                      label={t('md.fields.salesUom')}
                      value={def.salesUom}
                      onChange={(v) => setField('salesUom', v)}
                    />
                    <Field
                      label={t('md.fields.planningUom')}
                      value={def.planningUom}
                      onChange={(v) => setField('planningUom', v)}
                    />
                    <Field
                      label={t('md.fields.costingUom')}
                      value={def.costingUom}
                      onChange={(v) => setField('costingUom', v)}
                    />
                    <Field
                      label={t('md.fields.shippingUom')}
                      value={def.shippingUom}
                      onChange={(v) => setField('shippingUom', v)}
                    />
                  </div>
                  <p className="text-xs text-[var(--text-muted)]">{t('md.conversionLaw')}</p>
                </div>
              ) : null}

              {pack.id === 'packaging' ? (
                <div className="grid gap-3 md:grid-cols-2">
                  <Field
                    label={t('md.fields.piecesPerPackage')}
                    value={def.piecesPerPackage}
                    onChange={(v) => setField('piecesPerPackage', v)}
                  />
                  <Field
                    label={t('md.fields.packageLabel')}
                    value={def.packageLabel}
                    onChange={(v) => setField('packageLabel', v)}
                  />
                  <p className="md:col-span-2 text-xs text-[var(--text-secondary)]">{t('md.packagingLaw')}</p>
                </div>
              ) : null}

              {pack.id === 'numbering' ? (
                <div className="space-y-3">
                  <Field label={t('md.fields.numberingSeries')} value={def.numberingSeries} readOnly />
                  <p className="text-xs text-[var(--text-secondary)]">{t('md.numberingLaw')}</p>
                </div>
              ) : null}

              {pack.id === 'quality' ? (
                <div className="grid gap-3 md:grid-cols-2">
                  <Field label={t('md.fields.grade')} value={def.grade} onChange={(v) => setField('grade', v)} />
                  <Field
                    label={t('md.fields.inspectionPlan')}
                    value={def.inspectionPlan}
                    onChange={(v) => setField('inspectionPlan', v)}
                  />
                  <Field
                    label={t('md.fields.moistureMin')}
                    value={def.moistureMin}
                    onChange={(v) => setField('moistureMin', v)}
                  />
                  <Field
                    label={t('md.fields.moistureMax')}
                    value={def.moistureMax}
                    onChange={(v) => setField('moistureMax', v)}
                  />
                </div>
              ) : null}

              {pack.id === 'traceability' ? (
                <div className="space-y-3">
                  {(
                    [
                      ['genealogyRequired', t('md.fields.genealogyRequired')],
                      ['cocRequired', t('md.fields.cocRequired')],
                      ['evidenceRequired', t('md.fields.evidenceRequired')],
                    ] as const
                  ).map(([key, label]) => (
                    <label key={key} className="flex items-center gap-2 text-sm font-medium">
                      <input
                        type="checkbox"
                        checked={def[key]}
                        onChange={(e) => setField(key, e.target.checked)}
                      />
                      {label}
                    </label>
                  ))}
                  <p className="text-xs text-[var(--text-secondary)]">{t('md.traceLaw')}</p>
                </div>
              ) : null}

              {pack.id === 'costing' ? (
                <div className="grid gap-3 md:grid-cols-2">
                  <Field
                    label={t('md.fields.costingDriver')}
                    value={def.costingDriver}
                    onChange={(v) => setField('costingDriver', v)}
                  />
                  <Field
                    label={t('md.fields.valuationClass')}
                    value={def.valuationClass}
                    onChange={(v) => setField('valuationClass', v)}
                  />
                  <p className="md:col-span-2 text-xs text-[var(--text-secondary)]">{t('md.costingLaw')}</p>
                </div>
              ) : null}

              {pack.id === 'release' ? (
                <div className="space-y-3">
                  <div className="grid gap-2 md:grid-cols-2">
                    {[
                      [t('md.fields.name'), def.name],
                      [t('md.fields.identityClass'), def.identityClass],
                      [t('md.fields.stockUom'), def.stockUom],
                      [t('md.fields.salesUom'), def.salesUom],
                      [t('md.fields.productionUom'), def.productionUom],
                      [t('md.fields.costingUom'), def.costingUom],
                      [t('md.fields.piecesPerPackage'), def.piecesPerPackage],
                      [t('md.fields.grade'), def.grade],
                    ].map(([label, value]) => (
                      <div key={label} className="rounded-md border border-[var(--border-default)] px-3 py-2">
                        <p className="text-[10px] uppercase text-[var(--text-muted)]">{label}</p>
                        <p className="text-sm font-medium">{value}</p>
                      </div>
                    ))}
                  </div>
                  <label className="flex items-center gap-2 text-sm font-medium">
                    <input type="checkbox" checked={approved} onChange={(e) => setApproved(e.target.checked)} />
                    {t('md.approveRelease')}
                  </label>
                  {released ? (
                    <p className="text-sm font-medium text-[var(--color-primary)]">
                      {t('md.releasedBanner')}
                      {mintedCode ? ` · ${mintedCode}` : ''}
                    </p>
                  ) : null}
                </div>
              ) : null}

              {gateMessage && pack.id !== 'release' ? (
                <p className="text-sm text-[var(--color-danger)]">{gateMessage}</p>
              ) : null}
              {error ? <p className="text-sm text-[var(--color-danger)]">{error}</p> : null}

              <div className="flex flex-wrap gap-2 border-t border-[var(--border-default)] pt-3">
                <Button
                  type="button"
                  variant="secondary"
                  disabled={packIdx === 0 || released}
                  onClick={() => setPackIdx((i) => Math.max(0, i - 1))}
                >
                  {t('md.back')}
                </Button>
                {!released ? (
                  <Button
                    type="button"
                    disabled={Boolean(gateMessage) || persistMutation.isPending}
                    onClick={goNext}
                  >
                    {pack.id === 'release' ? t('md.release') : t('md.next')}
                  </Button>
                ) : (
                  <Button type="button" onClick={() => navigate({ to: '/inventory/master-data/materials' })}>
                    {t('md.openLibrary')}
                  </Button>
                )}
              </div>
            </CardContent>
          </Card>
        </main>

        <aside className="space-y-3">
          <div className="rounded-lg border border-[var(--border-default)] bg-[var(--color-surface)] p-3">
            <p className="text-[10px] font-semibold uppercase tracking-wide text-[var(--text-muted)]">
              {t('md.conversionPreview')}
            </p>
            <p className="mt-1 text-xs text-[var(--text-secondary)]">{t('md.conversionPreviewHint')}</p>
            <dl className="mt-3 space-y-1.5 text-sm">
              <div className="flex justify-between gap-2">
                <dt className="text-[var(--text-muted)]">{t('md.eq.pcs')}</dt>
                <dd className="font-mono font-medium">{equivalents.pcs}</dd>
              </div>
              <div className="flex justify-between gap-2">
                <dt className="text-[var(--text-muted)]">{t('md.eq.lm')}</dt>
                <dd className="font-mono font-medium">{equivalents.lm}</dd>
              </div>
              <div className="flex justify-between gap-2">
                <dt className="text-[var(--text-muted)]">{t('md.eq.m2')}</dt>
                <dd className="font-mono font-medium">{equivalents.m2}</dd>
              </div>
              <div className="flex justify-between gap-2">
                <dt className="text-[var(--text-muted)]">{t('md.eq.m3')}</dt>
                <dd className="font-mono font-medium">{equivalents.m3}</dd>
              </div>
              <div className="flex justify-between gap-2">
                <dt className="text-[var(--text-muted)]">{t('md.eq.kg')}</dt>
                <dd className="font-mono font-medium">{equivalents.kg}</dd>
              </div>
              <div className="flex justify-between gap-2">
                <dt className="text-[var(--text-muted)]">{t('md.eq.t')}</dt>
                <dd className="font-mono font-medium">{equivalents.t}</dd>
              </div>
            </dl>
          </div>
          <div className="rounded-lg border border-[var(--border-default)] bg-[var(--color-surface)] p-3 text-xs text-[var(--text-secondary)]">
            <p className="font-semibold text-[var(--text-primary)]">{t('md.layersTitle')}</p>
            <ul className="mt-2 list-inside list-disc space-y-1">
              <li>{t('md.layer.definition')}</li>
              <li>{t('md.layer.identity')}</li>
              <li>{t('md.layer.product')}</li>
            </ul>
          </div>
        </aside>
      </div>
    </div>
  );
}

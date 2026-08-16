import { Link, useNavigate } from '@tanstack/react-router';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { useMemo, useState } from 'react';
import { Button, Card, CardContent, CardDescription, CardHeader, CardTitle, Input } from '@naswood/ui';
import { createResource, searchAllResource } from '@/api/business';
import { useI18n } from '@/i18n';
import {
  HM_TYPE_OPTIONS,
  MP_WOOD_OPTIONS,
  TW_FAMILY_OPTIONS,
  WOOD_OPTIONS_HM,
  YM_TYPE_OPTIONS,
  mintMaterialCode,
  previewMaterialCode,
  type MainCategory,
} from './materialCoding';
import {
  buildDefinitionWithNominal,
  buildDuplicateFingerprint,
  findDuplicateMaterial,
  formatNominalDims,
  type NominalDims,
} from './materialNominalDims';

/**
 * INV-005 / INV-MAT-001 — Material Definition Designer
 * Manual create · system-minted MaterialCode · separate nominal fields · duplicate gate.
 * No automatic Variant/SKU generation.
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

type WidthMode = 'single' | 'range' | 'options';

type DefState = {
  name: string;
  mainCategory: MainCategory;
  materialTypeToken: string;
  woodToken: string;
  productTypeToken: string;
  thicknessMm: string;
  widthMode: WidthMode;
  widthMm: string;
  widthMinMm: string;
  widthMaxMm: string;
  widthOptions: string;
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

type MaterialRow = {
  id?: string;
  code?: string;
  name?: string;
  category?: string;
  definitionJson?: string | null;
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
  name: 'Çam Kereste',
  mainCategory: 'HM',
  materialTypeToken: 'KR',
  woodToken: 'PIN',
  productTypeToken: '',
  thicknessMm: '50',
  widthMode: 'single',
  widthMm: '100',
  widthMinMm: '',
  widthMaxMm: '',
  widthOptions: '',
  lengthMm: '4000',
  density: '480',
  moistureBasis: '',
  stockUom: 'M3',
  purchaseUom: 'M3',
  productionUom: 'M3',
  salesUom: 'M3',
  planningUom: 'PCS',
  costingUom: 'M3',
  shippingUom: 'kg',
  piecesPerPackage: '',
  packageLabel: '',
  identityClass: 'RM-LUMBER',
  lotPolicy: 'Lot operational · MI lifelong',
  numberingSeries: 'HM/YM/MP/TW · sistem üretir',
  grade: '',
  moistureMin: '',
  moistureMax: '',
  inspectionPlan: '',
  genealogyRequired: true,
  cocRequired: false,
  evidenceRequired: true,
  costingDriver: 'm³',
  valuationClass: 'RM-WOOD',
  notes: '',
};

function num(v: string) {
  const n = Number(String(v).replace(',', '.'));
  return Number.isFinite(n) ? n : 0;
}

function parseOpts(raw: string): number[] {
  return String(raw || '')
    .split(/[-–/|,;\s]+/)
    .map((p) => Number(p.replace(',', '.')))
    .filter((n) => Number.isFinite(n) && n > 0);
}

function categoryLabel(cat: MainCategory): string {
  if (cat === 'HM') return 'Hammadde';
  if (cat === 'YM') return 'Yarı Mamul';
  if (cat === 'MP') return 'Masif Panel';
  return 'Thermowood';
}

function buildNominalFromDef(def: DefState): NominalDims {
  const thicknessMm = num(def.thicknessMm) || null;
  const lengthMm = num(def.lengthMm) || null;
  if (def.widthMode === 'range') {
    const widthMinMm = num(def.widthMinMm) || null;
    const widthMaxMm = num(def.widthMaxMm) || null;
    return {
      thicknessMm,
      widthMm: null,
      lengthMm,
      widthMinMm,
      widthMaxMm,
      widthOptionsMm: null,
    };
  }
  if (def.widthMode === 'options') {
    const opts = parseOpts(def.widthOptions);
    return {
      thicknessMm,
      widthMm: null,
      lengthMm,
      widthMinMm: null,
      widthMaxMm: null,
      widthOptionsMm: opts.length >= 2 ? opts : null,
    };
  }
  return {
    thicknessMm,
    widthMm: num(def.widthMm) || null,
    lengthMm,
    widthMinMm: null,
    widthMaxMm: null,
    widthOptionsMm: null,
  };
}

function codingInput(def: DefState, existingCodes: string[]) {
  return {
    mainCategory: def.mainCategory,
    materialType: def.materialTypeToken,
    woodToken: def.woodToken,
    productTypeToken: def.productTypeToken || undefined,
    quality: def.grade || undefined,
    thicknessForCode: def.mainCategory === 'MP' ? num(def.thicknessMm) || null : null,
    existingCodes,
  };
}

function readMintedCode(row: Record<string, unknown> | null | undefined): string | null {
  if (!row) return null;
  for (const key of ['code', 'number', 'Code', 'Number']) {
    const v = row[key];
    if (typeof v === 'string' && v.trim()) return v.trim();
  }
  return null;
}

function SelectField({
  label,
  value,
  onChange,
  options,
}: {
  label: string;
  value: string;
  onChange: (v: string) => void;
  options: { token: string; label: string }[];
}) {
  return (
    <label className="block space-y-1">
      <span className="text-xs font-medium text-[var(--text-muted)]">{label}</span>
      <select
        value={value}
        onChange={(e) => onChange(e.target.value)}
        className="flex h-10 w-full rounded-md border border-[var(--border-default)] bg-[var(--color-surface)] px-3 text-sm"
      >
        {options.map((o) => (
          <option key={o.token} value={o.token}>
            {o.label}
          </option>
        ))}
      </select>
    </label>
  );
}

export function MaterialDefinitionDesigner() {
  const { t } = useI18n();
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const [packIdx, setPackIdx] = useState(0);
  const [maxReached, setMaxReached] = useState(0);
  const [def, setDef] = useState<DefState>(() => {
    if (typeof window === 'undefined') return DEFAULT_DEF;
    const sp = new URLSearchParams(window.location.search);
    if (!sp.get('from') && !sp.get('name')) return DEFAULT_DEF;
    return {
      ...DEFAULT_DEF,
      name: sp.get('name')?.trim() || DEFAULT_DEF.name,
      thicknessMm: sp.get('thicknessMm')?.trim() || DEFAULT_DEF.thicknessMm,
      widthMm: sp.get('widthMm')?.trim() || DEFAULT_DEF.widthMm,
      lengthMm: sp.get('lengthMm')?.trim() || DEFAULT_DEF.lengthMm,
    };
  });
  const [approved, setApproved] = useState(false);
  const [released, setReleased] = useState(false);
  const [mintedCode, setMintedCode] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [dupAck, setDupAck] = useState(false);

  const materialsQuery = useQuery({
    queryKey: ['business', 'materials', 'all-for-designer'],
    queryFn: () => searchAllResource<MaterialRow>('materials'),
  });
  const materials = materialsQuery.data ?? [];
  const existingCodes = useMemo(
    () => materials.map((m) => String(m.code ?? '').trim()).filter(Boolean),
    [materials],
  );

  const pack = PACKS[packIdx];
  const progress = Math.round(((packIdx + (released ? 1 : 0)) / PACKS.length) * 100);
  const nominal = useMemo(() => buildNominalFromDef(def), [def]);
  const dimsDisplay = formatNominalDims(nominal);
  const codePreview = useMemo(
    () => previewMaterialCode(codingInput(def, existingCodes)),
    [def, existingCodes],
  );

  const duplicate = useMemo(() => {
    const fp = buildDuplicateFingerprint({
      mainCategory: def.mainCategory,
      materialType: def.materialTypeToken,
      woodSpecies: def.woodToken,
      productType: def.productTypeToken,
      quality: def.grade,
      dims: nominal,
    });
    return findDuplicateMaterial(materials, fp);
  }, [def, materials, nominal]);

  const equivalents = useMemo(() => {
    const tMm = nominal.thicknessMm ?? 0;
    const wMm =
      nominal.widthMm ??
      (nominal.widthMinMm != null && nominal.widthMaxMm != null
        ? (nominal.widthMinMm + nominal.widthMaxMm) / 2
        : nominal.widthOptionsMm?.[0] ?? 0);
    const lMm = nominal.lengthMm ?? 0;
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
  }, [nominal, def.density]);

  const gateMessage = useMemo(() => {
    if (pack.id === 'general') {
      if (!def.name.trim()) return t('md.gateNeedName');
      if (!def.woodToken) return t('md.gateNeedCoding');
      if ((def.mainCategory === 'HM' || def.mainCategory === 'YM') && !def.materialTypeToken) {
        return t('md.gateNeedCoding');
      }
      if (def.mainCategory === 'YM' && def.materialTypeToken === 'LM' && !def.productTypeToken) {
        return t('md.gateNeedCoding');
      }
      if (def.mainCategory === 'MP' && (!def.productTypeToken || !def.grade)) {
        return t('md.gateNeedCoding');
      }
    }
    if (pack.id === 'measurement') {
      if (!nominal.thicknessMm) return t('md.gateNeedDims');
      if (def.widthMode === 'single' && !nominal.widthMm) return t('md.gateNeedDims');
      if (def.widthMode === 'range' && (nominal.widthMinMm == null || nominal.widthMaxMm == null)) {
        return t('md.gateNeedDims');
      }
      if (def.widthMode === 'options' && !(nominal.widthOptionsMm?.length)) {
        return t('md.gateNeedDims');
      }
      if ((def.mainCategory === 'HM' || def.mainCategory === 'YM') && !nominal.lengthMm) {
        return t('md.gateNeedDims');
      }
    }
    if (pack.id === 'conversion' && !def.stockUom) return t('md.gateNeedStockUom');
    if (pack.id === 'release') {
      if (duplicate && !dupAck) return t('md.gateNeedDupAck');
      if (!approved) return t('md.gateNeedApprove');
      if (!mintMaterialCode(codingInput(def, existingCodes)) && def.mainCategory === 'MP') {
        return t('md.gateMpCodeExists');
      }
    }
    return null;
  }, [pack.id, def, approved, t, nominal, duplicate, dupAck, existingCodes]);

  const persistMutation = useMutation({
    mutationFn: async () => {
      const code = mintMaterialCode(codingInput(def, existingCodes));
      if (!code) throw new Error(t('md.gateMpCodeExists'));
      if (duplicate && !dupAck) throw new Error(t('md.duplicateFound'));

      const definitionPayload = buildDefinitionWithNominal(
        {
          ...def,
          mainCategory: def.mainCategory,
          materialType: def.materialTypeToken,
          materialTypeToken: def.materialTypeToken,
          species: def.woodToken,
          woodToken: def.woodToken,
          productTypeToken: def.productTypeToken,
          grade: def.grade,
          category: categoryLabel(def.mainCategory),
          NominalIsCommercial: true,
          ActualDimsLiveInReceiving: true,
        },
        nominal,
      );

      const description = [
        def.woodToken,
        dimsDisplay !== '—' ? dimsDisplay : null,
        def.materialTypeToken,
        def.grade,
        def.notes,
      ]
        .filter(Boolean)
        .join(' · ')
        .slice(0, 200);

      return createResource<Record<string, unknown>>('materials', {
        name: def.name.trim(),
        description,
        category: categoryLabel(def.mainCategory),
        unitOfMeasure: def.stockUom || 'PCS',
        status: 'Active',
        code,
        definitionJson: JSON.stringify(definitionPayload),
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
    if (
      key === 'mainCategory' ||
      key === 'materialTypeToken' ||
      key === 'woodToken' ||
      key === 'productTypeToken' ||
      key === 'grade' ||
      key === 'thicknessMm' ||
      key === 'widthMm' ||
      key === 'widthMinMm' ||
      key === 'widthMaxMm' ||
      key === 'widthOptions' ||
      key === 'widthMode' ||
      key === 'lengthMm'
    ) {
      setDupAck(false);
    }
  }

  function applyCategoryDefaults(cat: MainCategory) {
    setDef((d) => {
      if (cat === 'HM') {
        return {
          ...d,
          mainCategory: cat,
          materialTypeToken: 'KR',
          woodToken: 'PIN',
          productTypeToken: '',
          widthMode: 'single',
          stockUom: 'M3',
          grade: d.grade,
        };
      }
      if (cat === 'YM') {
        return {
          ...d,
          mainCategory: cat,
          materialTypeToken: 'LM',
          woodToken: 'PIN',
          productTypeToken: 'S',
          widthMode: 'single',
          stockUom: 'M3',
        };
      }
      if (cat === 'MP') {
        return {
          ...d,
          mainCategory: cat,
          materialTypeToken: '',
          woodToken: 'CP',
          productTypeToken: 'S',
          grade: d.grade || 'AA',
          widthMode: 'range',
          widthMinMm: '1200',
          widthMaxMm: '1220',
          widthMm: '',
          lengthMm: '',
          thicknessMm: d.thicknessMm || '18',
          stockUom: 'M2',
        };
      }
      return {
        ...d,
        mainCategory: cat,
        materialTypeToken: '',
        woodToken: 'CP',
        productTypeToken: '',
        widthMode: 'options',
        widthOptions: '92/117/138',
        lengthMm: '',
        stockUom: 'M2',
      };
    });
    setDupAck(false);
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

  const typeOptions =
    def.mainCategory === 'HM' ? HM_TYPE_OPTIONS : def.mainCategory === 'YM' ? YM_TYPE_OPTIONS : [];
  const woodOptions =
    def.mainCategory === 'MP'
      ? MP_WOOD_OPTIONS
      : def.mainCategory === 'TW'
        ? TW_FAMILY_OPTIONS
        : WOOD_OPTIONS_HM;

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
            <p className="font-mono text-sm font-medium">
              {mintedCode
                ? `${mintedCode} · ${dimsDisplay}`
                : `${codePreview} · ${dimsDisplay}`}
            </p>
            <p className="text-[10px] text-[var(--text-muted)]">{t('md.codeWithDimsHint')}</p>
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

      {duplicate ? (
        <div className="rounded-lg border border-amber-500/40 bg-amber-500/10 px-4 py-3 text-sm">
          <p className="font-medium text-[var(--text-primary)]">{t('md.duplicateFound')}</p>
          <p className="mt-1 text-[var(--text-secondary)]">
            {duplicate.code} · {duplicate.name}
            {duplicate.definitionJson
              ? ` · ${formatNominalDims({ thicknessMm: nominal.thicknessMm, widthMm: nominal.widthMm, lengthMm: nominal.lengthMm, widthMinMm: nominal.widthMinMm, widthMaxMm: nominal.widthMaxMm, widthOptionsMm: nominal.widthOptionsMm })}`
              : ''}
          </p>
          <div className="mt-3 flex flex-wrap gap-2">
            {duplicate.id ? (
              <Button
                type="button"
                variant="secondary"
                onClick={() =>
                  navigate({ to: '/inventory/master-data/materials/$id', params: { id: String(duplicate.id) } })
                }
              >
                {t('md.useExisting')}
              </Button>
            ) : null}
            <label className="flex items-center gap-2 text-sm">
              <input type="checkbox" checked={dupAck} onChange={(e) => setDupAck(e.target.checked)} />
              {t('md.dupAckCreateAnyway')}
            </label>
          </div>
        </div>
      ) : null}

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
                  <SelectField
                    label={t('md.fields.mainCategory')}
                    value={def.mainCategory}
                    onChange={(v) => applyCategoryDefaults(v as MainCategory)}
                    options={[
                      { token: 'HM', label: 'Hammadde (HM)' },
                      { token: 'YM', label: 'Yarı Mamul (YM)' },
                      { token: 'MP', label: 'Masif Panel (MP)' },
                      { token: 'TW', label: 'Thermowood (TW)' },
                    ]}
                  />
                  {typeOptions.length > 0 ? (
                    <SelectField
                      label={t('md.fields.materialType')}
                      value={def.materialTypeToken}
                      onChange={(v) => setField('materialTypeToken', v)}
                      options={typeOptions}
                    />
                  ) : null}
                  <SelectField
                    label={
                      def.mainCategory === 'TW' ? t('md.fields.twFamily') : t('md.fields.species')
                    }
                    value={def.woodToken}
                    onChange={(v) => setField('woodToken', v)}
                    options={woodOptions}
                  />
                  {def.mainCategory === 'YM' && def.materialTypeToken === 'LM' ? (
                    <SelectField
                      label={t('md.fields.productType')}
                      value={def.productTypeToken}
                      onChange={(v) => setField('productTypeToken', v)}
                      options={[
                        { token: 'S', label: 'Solid (S)' },
                        { token: 'FJ', label: 'Finger Joint (FJ)' },
                      ]}
                    />
                  ) : null}
                  {def.mainCategory === 'MP' ? (
                    <>
                      <SelectField
                        label={t('md.fields.productType')}
                        value={def.productTypeToken}
                        onChange={(v) => setField('productTypeToken', v)}
                        options={[
                          { token: 'S', label: 'Solid (S)' },
                          { token: 'FJ', label: 'Finger Joint (FJ)' },
                        ]}
                      />
                      <Field
                        label={t('md.fields.grade')}
                        value={def.grade}
                        onChange={(v) => setField('grade', v.toUpperCase())}
                        placeholder="AA"
                      />
                    </>
                  ) : null}
                  <div className="md:col-span-2 rounded-md border border-[var(--border-default)] px-3 py-2">
                    <p className="text-[10px] uppercase text-[var(--text-muted)]">{t('md.fields.systemCode')}</p>
                    <p className="font-mono text-sm font-medium">{codePreview}</p>
                    <p className="text-xs text-[var(--text-muted)]">{t('md.codeNoManual')}</p>
                  </div>
                </div>
              ) : null}

              {pack.id === 'identity' ? (
                <div className="space-y-3">
                  <Field
                    label={t('md.fields.identityClass')}
                    value={def.identityClass}
                    onChange={(v) => setField('identityClass', v)}
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
                <div className="space-y-4">
                  <p className="text-xs text-[var(--text-secondary)]">{t('md.nominalLaw')}</p>
                  <div className="grid gap-3 md:grid-cols-3">
                    <Field
                      label={t('md.fields.thickness')}
                      value={def.thicknessMm}
                      onChange={(v) => setField('thicknessMm', v)}
                    />
                    <div className="md:col-span-2">
                      <SelectField
                        label={t('md.fields.widthMode')}
                        value={def.widthMode}
                        onChange={(v) => setField('widthMode', v as WidthMode)}
                        options={[
                          { token: 'single', label: t('md.widthMode.single') },
                          { token: 'range', label: t('md.widthMode.range') },
                          { token: 'options', label: t('md.widthMode.options') },
                        ]}
                      />
                    </div>
                    {def.widthMode === 'single' ? (
                      <Field
                        label={t('md.fields.width')}
                        value={def.widthMm}
                        onChange={(v) => setField('widthMm', v)}
                      />
                    ) : null}
                    {def.widthMode === 'range' ? (
                      <>
                        <Field
                          label={t('md.fields.widthMin')}
                          value={def.widthMinMm}
                          onChange={(v) => setField('widthMinMm', v)}
                        />
                        <Field
                          label={t('md.fields.widthMax')}
                          value={def.widthMaxMm}
                          onChange={(v) => setField('widthMaxMm', v)}
                        />
                      </>
                    ) : null}
                    {def.widthMode === 'options' ? (
                      <Field
                        label={t('md.fields.widthOptions')}
                        value={def.widthOptions}
                        onChange={(v) => setField('widthOptions', v)}
                        placeholder="92/117/138"
                      />
                    ) : null}
                    <Field
                      label={t('md.fields.length')}
                      value={def.lengthMm}
                      onChange={(v) => setField('lengthMm', v)}
                      placeholder={def.mainCategory === 'MP' || def.mainCategory === 'TW' ? 'opsiyonel' : ''}
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
                  <div className="rounded-md border border-[var(--border-default)] bg-[var(--color-surface-hover)] px-3 py-2">
                    <p className="text-[10px] uppercase text-[var(--text-muted)]">{t('md.fields.dimsDisplay')}</p>
                    <p className="font-mono text-sm font-medium">{dimsDisplay}</p>
                  </div>
                </div>
              ) : null}

              {pack.id === 'conversion' ? (
                <div className="space-y-3">
                  <div className="grid gap-3 md:grid-cols-2">
                    <Field label={t('md.fields.stockUom')} value={def.stockUom} onChange={(v) => setField('stockUom', v)} />
                    <Field label={t('md.fields.purchaseUom')} value={def.purchaseUom} onChange={(v) => setField('purchaseUom', v)} />
                    <Field label={t('md.fields.productionUom')} value={def.productionUom} onChange={(v) => setField('productionUom', v)} />
                    <Field label={t('md.fields.salesUom')} value={def.salesUom} onChange={(v) => setField('salesUom', v)} />
                    <Field label={t('md.fields.planningUom')} value={def.planningUom} onChange={(v) => setField('planningUom', v)} />
                    <Field label={t('md.fields.costingUom')} value={def.costingUom} onChange={(v) => setField('costingUom', v)} />
                    <Field label={t('md.fields.shippingUom')} value={def.shippingUom} onChange={(v) => setField('shippingUom', v)} />
                  </div>
                  <p className="text-xs text-[var(--text-muted)]">{t('md.conversionLaw')}</p>
                </div>
              ) : null}

              {pack.id === 'packaging' ? (
                <div className="grid gap-3 md:grid-cols-2">
                  <Field label={t('md.fields.piecesPerPackage')} value={def.piecesPerPackage} onChange={(v) => setField('piecesPerPackage', v)} />
                  <Field label={t('md.fields.packageLabel')} value={def.packageLabel} onChange={(v) => setField('packageLabel', v)} />
                  <p className="md:col-span-2 text-xs text-[var(--text-secondary)]">{t('md.packagingLaw')}</p>
                </div>
              ) : null}

              {pack.id === 'numbering' ? (
                <div className="space-y-3">
                  <Field label={t('md.fields.numberingSeries')} value={def.numberingSeries} readOnly />
                  <Field label={t('md.fields.systemCode')} value={codePreview} readOnly />
                  <p className="text-xs text-[var(--text-secondary)]">{t('md.numberingLaw')}</p>
                </div>
              ) : null}

              {pack.id === 'quality' ? (
                <div className="grid gap-3 md:grid-cols-2">
                  <Field label={t('md.fields.grade')} value={def.grade} onChange={(v) => setField('grade', v)} />
                  <Field label={t('md.fields.inspectionPlan')} value={def.inspectionPlan} onChange={(v) => setField('inspectionPlan', v)} />
                  <Field label={t('md.fields.moistureMin')} value={def.moistureMin} onChange={(v) => setField('moistureMin', v)} />
                  <Field label={t('md.fields.moistureMax')} value={def.moistureMax} onChange={(v) => setField('moistureMax', v)} />
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
                  <Field label={t('md.fields.costingDriver')} value={def.costingDriver} onChange={(v) => setField('costingDriver', v)} />
                  <Field label={t('md.fields.valuationClass')} value={def.valuationClass} onChange={(v) => setField('valuationClass', v)} />
                  <p className="md:col-span-2 text-xs text-[var(--text-secondary)]">{t('md.costingLaw')}</p>
                </div>
              ) : null}

              {pack.id === 'release' ? (
                <div className="space-y-3">
                  <div className="grid gap-2 md:grid-cols-2">
                    {[
                      [t('md.fields.name'), def.name],
                      [t('md.fields.systemCode'), codePreview],
                      [t('md.fields.dimsDisplay'), dimsDisplay],
                      [t('md.fields.stockUom'), def.stockUom],
                      [t('md.fields.mainCategory'), categoryLabel(def.mainCategory)],
                      [t('md.fields.grade'), def.grade || '—'],
                    ].map(([label, value]) => (
                      <div key={label} className="rounded-md border border-[var(--border-default)] px-3 py-2">
                        <p className="text-[10px] uppercase text-[var(--text-muted)]">{label}</p>
                        <p className="text-sm font-medium">{value}</p>
                      </div>
                    ))}
                  </div>
                  <p className="text-xs text-[var(--text-secondary)]">{t('md.receivingVsNominal')}</p>
                  <label className="flex items-center gap-2 text-sm font-medium">
                    <input type="checkbox" checked={approved} onChange={(e) => setApproved(e.target.checked)} />
                    {t('md.approveRelease')}
                  </label>
                  {released ? (
                    <p className="text-sm font-medium text-[var(--color-primary)]">
                      {t('md.releasedBanner')}
                      {mintedCode ? ` · ${mintedCode} · ${dimsDisplay}` : ''}
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

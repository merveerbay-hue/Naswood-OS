/**
 * Shared draft shape for the designer wizard and Excel bulk create.
 * Code is never user-supplied — mintMaterialCode allocates it at persist time.
 */
import {
  applyThermowoodToken,
  mintMaterialCode,
  suggestMaterialName,
  type MainCategory,
} from './materialCoding';
import {
  COMPLIANCE_SCOPES,
  GRADING_METHODS,
  complianceDefinitionFragment,
  isExcludedFromEn14081,
  validateMaterialCompliance,
  type ComplianceScope,
  type GradingMethod,
} from './materialCompliance';
import {
  buildDefinitionWithNominal,
  buildDuplicateFingerprint,
  findDuplicateMaterial,
  formatNominalDims,
  type NominalDims,
} from './materialNominalDims';

export type WidthMode = 'single' | 'range' | 'options';

export type MaterialCardDraft = {
  name: string;
  mainCategory: MainCategory;
  materialTypeToken: string;
  woodToken: string;
  isThermowood: boolean;
  productTypeToken: string;
  grade: string;
  thicknessMm: string;
  widthMode: WidthMode;
  widthMm: string;
  widthMinMm: string;
  widthMaxMm: string;
  widthOptions: string;
  lengthMm: string;
  stockUom: string;
  countUom: string;
  lotTracking: boolean;
  volumeCalcRequired: boolean;
  status: 'Active' | 'Passive';
  notes: string;
  complianceScope: ComplianceScope;
  supportedGradingMethods: GradingMethod[];
};

export type MaterialLookupRow = {
  id?: string;
  code?: string;
  name?: string;
  definitionJson?: string | null;
};

export function parseNumberToken(v: string): number {
  const n = Number(String(v).replace(',', '.'));
  return Number.isFinite(n) ? n : 0;
}

export function parseWidthOptionList(raw: string): number[] {
  return String(raw || '')
    .split(/[-–/|,;\s]+/)
    .map((p) => Number(p.replace(',', '.')))
    .filter((n) => Number.isFinite(n) && n > 0);
}

export function categoryLabel(cat: MainCategory): string {
  if (cat === 'HM') return 'Hammadde';
  if (cat === 'YM') return 'Yarı Mamul';
  if (cat === 'MP') return 'Masif Panel';
  return 'Thermowood';
}

export function effectiveWoodToken(def: MaterialCardDraft): string {
  if (def.mainCategory === 'TW') return def.woodToken;
  return applyThermowoodToken(def.woodToken, def.isThermowood);
}

export function buildNominalFromDraft(def: MaterialCardDraft): NominalDims {
  const thicknessMm = parseNumberToken(def.thicknessMm) || null;
  const lengthMm = parseNumberToken(def.lengthMm) || null;
  if (def.widthMode === 'range') {
    return {
      thicknessMm,
      widthMm: null,
      lengthMm,
      widthMinMm: parseNumberToken(def.widthMinMm) || null,
      widthMaxMm: parseNumberToken(def.widthMaxMm) || null,
      widthOptionsMm: null,
    };
  }
  if (def.widthMode === 'options') {
    const opts = parseWidthOptionList(def.widthOptions);
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
    widthMm: parseNumberToken(def.widthMm) || null,
    lengthMm,
    widthMinMm: null,
    widthMaxMm: null,
    widthOptionsMm: null,
  };
}

export function codingInputFromDraft(def: MaterialCardDraft, existingCodes: string[]) {
  return {
    mainCategory: def.mainCategory,
    materialType: def.materialTypeToken,
    woodToken: effectiveWoodToken(def),
    productTypeToken: def.productTypeToken || undefined,
    quality: def.grade || undefined,
    thicknessForCode: def.mainCategory === 'MP' ? parseNumberToken(def.thicknessMm) || null : null,
    existingCodes,
  };
}

export function suggestedNameForDraft(def: MaterialCardDraft): string {
  return suggestMaterialName({
    mainCategory: def.mainCategory,
    materialType: def.materialTypeToken,
    baseWoodToken: def.woodToken,
    isThermowood: def.mainCategory === 'TW' ? true : def.isThermowood,
    productTypeToken: def.productTypeToken,
    quality: def.grade,
  });
}

/** Same gates as the designer packs, flattened for one-shot create / bulk import. */
export function validateDraftForCreate(
  def: MaterialCardDraft,
  existingCodes: string[],
): { ok: true } | { ok: false; message: string } {
  if (!def.name.trim()) return { ok: false, message: 'Malzeme adı gerekli.' };
  if (!def.woodToken) return { ok: false, message: 'Ağaç / aile kodu gerekli.' };
  if ((def.mainCategory === 'HM' || def.mainCategory === 'YM') && !def.materialTypeToken) {
    return { ok: false, message: 'Cins / malzeme tipi gerekli.' };
  }
  if (def.mainCategory === 'YM' && def.materialTypeToken === 'LM' && !def.productTypeToken) {
    return { ok: false, message: 'Lamel için ürün tipi (S veya FJ) gerekli.' };
  }
  if (def.mainCategory === 'MP' && (!def.productTypeToken || !def.grade)) {
    return { ok: false, message: 'Masif panel için ürün tipi ve kalite gerekli.' };
  }

  const nominal = buildNominalFromDraft(def);
  if (!nominal.thicknessMm) return { ok: false, message: 'Nominal kalınlık (mm) gerekli.' };
  if (def.widthMode === 'single' && !nominal.widthMm) {
    return { ok: false, message: 'Nominal genişlik (mm) gerekli.' };
  }
  if (def.widthMode === 'range' && (nominal.widthMinMm == null || nominal.widthMaxMm == null)) {
    return { ok: false, message: 'Genişlik aralığı için min ve max gerekli.' };
  }
  if (def.widthMode === 'options' && !(nominal.widthOptionsMm?.length)) {
    return { ok: false, message: 'En az iki genişlik seçeneği gerekli (ör. 92/117/138).' };
  }
  if ((def.mainCategory === 'HM' || def.mainCategory === 'YM') && !nominal.lengthMm) {
    return { ok: false, message: 'Nominal uzunluk (mm) gerekli.' };
  }
  if (!def.stockUom) return { ok: false, message: 'Ana stok birimi gerekli.' };
  if (!def.countUom) return { ok: false, message: 'Sayım birimi gerekli.' };
  if (def.status !== 'Active' && def.status !== 'Passive') {
    return { ok: false, message: 'Durum Active veya Passive olmalıdır.' };
  }

  const excluded = isExcludedFromEn14081({
    mainCategory: def.mainCategory,
    isThermowood: def.isThermowood || def.mainCategory === 'TW',
    productTypeToken: def.productTypeToken,
  });
  const scope = excluded ? 'NORMAL_STOCK' : def.complianceScope;
  const methods = excluded ? [] : def.supportedGradingMethods;
  const compliance = validateMaterialCompliance({
    complianceScope: scope,
    supportedGradingMethods: methods,
    mainCategory: def.mainCategory,
    isThermowood: def.isThermowood || def.mainCategory === 'TW',
    productTypeToken: def.productTypeToken,
  });
  if (!compliance.ok) return { ok: false, message: compliance.message };

  if (!mintMaterialCode(codingInputFromDraft(def, existingCodes)) && def.mainCategory === 'MP') {
    return { ok: false, message: 'Bu masif panel kodu zaten var — kalınlık veya kalite değiştirin.' };
  }
  if (!mintMaterialCode(codingInputFromDraft(def, existingCodes))) {
    return { ok: false, message: 'Malzeme kodu üretilemedi — kodlama alanlarını kontrol edin.' };
  }
  return { ok: true };
}

export function findDraftDuplicate(def: MaterialCardDraft, materials: MaterialLookupRow[]) {
  const fp = buildDuplicateFingerprint({
    mainCategory: def.mainCategory,
    materialType: def.materialTypeToken,
    woodSpecies: effectiveWoodToken(def),
    productType: def.productTypeToken,
    quality: def.grade,
    dims: buildNominalFromDraft(def),
  });
  return findDuplicateMaterial(materials, fp);
}

export function buildMaterialCreateBody(
  def: MaterialCardDraft,
  existingCodes: string[],
): {
  name: string;
  description: string;
  category: string;
  unitOfMeasure: string;
  status: 'Active' | 'Passive';
  code: string;
  definitionJson: string;
} {
  const code = mintMaterialCode(codingInputFromDraft(def, existingCodes));
  if (!code) throw new Error('Malzeme kodu üretilemedi.');

  const woodEff = effectiveWoodToken(def);
  const complianceFrag = complianceDefinitionFragment({
    complianceScope: def.complianceScope,
    supportedGradingMethods: def.supportedGradingMethods,
    mainCategory: def.mainCategory,
    isThermowood: def.isThermowood || def.mainCategory === 'TW',
    productTypeToken: def.productTypeToken,
  });
  const complianceCheck = validateMaterialCompliance({
    ...complianceFrag,
    mainCategory: def.mainCategory,
    isThermowood: def.isThermowood || def.mainCategory === 'TW',
    productTypeToken: def.productTypeToken,
  });
  if (!complianceCheck.ok) throw new Error(complianceCheck.message);

  const nominal = buildNominalFromDraft(def);
  const dimsDisplay = formatNominalDims(nominal);
  const definitionPayload = buildDefinitionWithNominal(
    {
      mainCategory: def.mainCategory,
      materialType: def.materialTypeToken,
      materialTypeToken: def.materialTypeToken,
      species: woodEff,
      woodToken: woodEff,
      baseWoodToken: def.woodToken,
      isThermowood: def.mainCategory === 'TW' ? true : def.isThermowood,
      productTypeToken: def.productTypeToken,
      grade: def.grade,
      category: categoryLabel(def.mainCategory),
      stockUom: def.stockUom,
      countUom: def.countUom,
      lotTracking: def.lotTracking,
      volumeCalcRequired: def.volumeCalcRequired,
      volumeCalcRule: def.volumeCalcRequired ? 'pcs × T × W × L / 1e9 → m³' : null,
      identityClass: 'RM-LUMBER',
      lotPolicy: def.lotTracking ? 'Lot operational · MI lifelong' : 'Lot optional · MI lifelong',
      NominalIsCommercial: true,
      ActualDimsLiveInReceiving: true,
      notes: def.notes,
      complianceScope: complianceFrag.complianceScope,
      supportedGradingMethods: complianceFrag.supportedGradingMethods,
    },
    nominal,
  );

  const description = [
    woodEff,
    def.isThermowood || def.mainCategory === 'TW' ? 'Thermowood' : null,
    dimsDisplay !== '—' ? dimsDisplay : null,
    def.materialTypeToken,
    def.grade,
    def.notes,
  ]
    .filter(Boolean)
    .join(' · ')
    .slice(0, 200);

  return {
    name: def.name.trim(),
    description,
    category: categoryLabel(def.mainCategory),
    unitOfMeasure: def.stockUom || 'PCS',
    status: def.status,
    code,
    definitionJson: JSON.stringify(definitionPayload),
  };
}

export const ALLOWED_COMPLIANCE_SCOPES = COMPLIANCE_SCOPES.map((s) => s.token);
export const ALLOWED_GRADING_METHODS = GRADING_METHODS.map((s) => s.token);

/**
 * Controlled Material Master create from count Excel candidates.
 * Reuses mintMaterialCode — does not invent a second generator.
 * Excel physical dims are NOT copied into master nominals unless the user types them.
 */
import {
  HM_TYPE_OPTIONS,
  WOOD_OPTIONS_HM,
  YM_TYPE_OPTIONS,
  mintMaterialCode,
  previewMaterialCode,
  type MainCategory,
} from '../materials/materialCoding';
import { categoryLabel } from '../materials/materialCardDraft';
import { findDuplicateMaterial, buildDuplicateFingerprint } from '../materials/materialNominalDims';
import { normalizeMatchKey, type CountExcelMaterial } from './cycleCountExcel';

export { HM_TYPE_OPTIONS, WOOD_OPTIONS_HM, YM_TYPE_OPTIONS };
export type { MainCategory };

export function canCreateCountMaterial(roles: string[] | null | undefined): boolean {
  return (roles ?? []).some((r) => r.trim().toLowerCase() === 'administrator');
}

export type CountMaterialCreateDraft = {
  name: string;
  mainCategory: MainCategory;
  materialTypeToken: string;
  woodToken: string;
  thicknessMm: string;
  widthMm: string;
  lengthMm: string;
  stockUom: string;
  countUom: string;
  lotTracking: boolean;
};

export function emptyCountMaterialDraft(excelName: string): CountMaterialCreateDraft {
  return {
    name: excelName.trim(),
    mainCategory: 'HM',
    materialTypeToken: 'KR',
    woodToken: 'PIN',
    thicknessMm: '',
    widthMm: '',
    lengthMm: '',
    stockUom: 'M3',
    countUom: 'PCS',
    lotTracking: true,
  };
}

export function previewCountMaterialCode(draft: CountMaterialCreateDraft, existingCodes: string[]): string {
  return previewMaterialCode({
    mainCategory: draft.mainCategory,
    materialType: draft.materialTypeToken,
    woodToken: draft.woodToken,
    productTypeToken: draft.mainCategory === 'YM' && draft.materialTypeToken === 'LM' ? 'S' : undefined,
    existingCodes,
  });
}

function numOrNull(raw: string): number | null {
  const n = Number(String(raw).replace(',', '.').trim());
  return Number.isFinite(n) && n > 0 ? n : null;
}

export function findCountMaterialDuplicate(
  draft: CountMaterialCreateDraft,
  materials: CountExcelMaterial[],
): CountExcelMaterial | null {
  const byName = materials.find((m) => normalizeMatchKey(m.name) === normalizeMatchKey(draft.name));
  if (byName) return byName;
  const t = numOrNull(draft.thicknessMm);
  const w = numOrNull(draft.widthMm);
  if (t || w) {
    const fp = buildDuplicateFingerprint({
      mainCategory: draft.mainCategory,
      materialType: draft.materialTypeToken,
      woodSpecies: draft.woodToken,
      dims: {
        thicknessMm: t,
        widthMm: w,
        lengthMm: numOrNull(draft.lengthMm),
      },
    });
    const hit = findDuplicateMaterial(materials, fp);
    if (hit) return hit as CountExcelMaterial;
  }
  return (
    materials.find((m) => {
      try {
        const def = m.definitionJson ? (JSON.parse(m.definitionJson) as Record<string, unknown>) : {};
        const type = String(def.materialTypeToken ?? def.materialType ?? '').toUpperCase();
        const wood = String(def.woodToken ?? def.species ?? '').toUpperCase();
        return type === draft.materialTypeToken.toUpperCase() && wood === draft.woodToken.toUpperCase() &&
          normalizeMatchKey(m.name) === normalizeMatchKey(draft.name);
      } catch {
        return false;
      }
    }) ?? null
  );
}

export function buildCountMaterialCreateBody(draft: CountMaterialCreateDraft, existingCodes: string[]) {
  if (!draft.name.trim()) throw new Error('Malzeme adı gerekli.');
  if (!draft.woodToken) throw new Error('Ağaç türü gerekli.');
  if ((draft.mainCategory === 'HM' || draft.mainCategory === 'YM') && !draft.materialTypeToken) {
    throw new Error('Malzeme cinsi gerekli.');
  }
  const productTypeToken = draft.mainCategory === 'YM' && draft.materialTypeToken === 'LM' ? 'S' : undefined;
  const code = mintMaterialCode({
    mainCategory: draft.mainCategory,
    materialType: draft.materialTypeToken,
    woodToken: draft.woodToken,
    productTypeToken,
    existingCodes,
  });
  if (!code) throw new Error('Malzeme kodu üretilemedi.');
  const t = numOrNull(draft.thicknessMm);
  const w = numOrNull(draft.widthMm);
  const l = numOrNull(draft.lengthMm);
  const definition: Record<string, unknown> = {
    mainCategory: draft.mainCategory,
    materialType: draft.materialTypeToken,
    materialTypeToken: draft.materialTypeToken,
    woodToken: draft.woodToken,
    species: draft.woodToken,
    stockUom: draft.stockUom,
    countUom: draft.countUom,
    lotTracking: draft.lotTracking,
    volumeCalcRequired: draft.stockUom === 'M3',
    complianceScope: 'NORMAL_STOCK',
    NominalIsCommercial: true,
    ActualDimsLiveInReceiving: true,
    notes: 'Sayım adayı — fiziksel ölçü master nominali değildir.',
  };
  if (t != null) definition.NominalThicknessMm = t;
  if (w != null) definition.NominalWidthMm = w;
  if (l != null) definition.NominalLengthMm = l;
  return {
    name: draft.name.trim(),
    description: 'Sayım Excel adayı',
    category: categoryLabel(draft.mainCategory),
    unitOfMeasure: draft.stockUom || 'PCS',
    status: 'Active' as const,
    code,
    definitionJson: JSON.stringify(definition),
  };
}

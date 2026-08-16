/**
 * Nominal dimensions next to MaterialCode.
 * Card = nominal/standard; receiving physical measure stays separate.
 */

export type NominalDims = {
  thicknessMm: number | null;
  widthMm: number | null;
  lengthMm: number | null;
  /** e.g. 1200-1220 or 92-117-138 when commercial width is a range */
  widthRange?: string | null;
};

export type MaterialDimsSource = {
  code?: string | null;
  name?: string | null;
  description?: string | null;
  definitionJson?: string | null;
  nominalThicknessMm?: number | null;
  nominalWidthMm?: number | null;
  nominalLengthMm?: number | null;
  widthRange?: string | null;
};

function num(v: unknown): number | null {
  if (v == null || v === '') return null;
  const n = typeof v === 'number' ? v : Number(String(v).replace(',', '.'));
  return Number.isFinite(n) && n > 0 ? n : null;
}

function fmtPart(n: number): string {
  return Number.isInteger(n) ? String(n) : String(n);
}

/** 18×1220×2440 mm · or 16×92-117-138 mm when width is a range and length unknown. */
export function formatNominalDims(dims: NominalDims | null | undefined): string {
  if (!dims) return '—';
  const t = dims.thicknessMm;
  const w = dims.widthMm;
  const len = dims.lengthMm;
  const wr = dims.widthRange?.trim();
  if (t != null && w != null && len != null) return `${fmtPart(t)}×${fmtPart(w)}×${fmtPart(len)} mm`;
  if (t != null && wr) return `${fmtPart(t)}×${wr} mm`;
  if (t != null && w != null) return `${fmtPart(t)}×${fmtPart(w)} mm`;
  if (t != null) return `${fmtPart(t)} mm`;
  return '—';
}

export function parseDefinitionNominal(definitionJson?: string | null): NominalDims | null {
  if (!definitionJson?.trim()) return null;
  try {
    const def = JSON.parse(definitionJson) as Record<string, unknown>;
    const thicknessMm =
      num(def.NominalThicknessMm) ?? num(def.nominalThicknessMm) ?? num(def.thicknessMm);
    const widthMm = num(def.NominalWidthMm) ?? num(def.nominalWidthMm) ?? num(def.widthMm);
    const lengthMm = num(def.NominalLengthMm) ?? num(def.nominalLengthMm) ?? num(def.lengthMm);
    const widthRange =
      (typeof def.widthRange === 'string' && def.widthRange) ||
      (typeof def.WidthRange === 'string' && def.WidthRange) ||
      null;
    if (thicknessMm == null && widthMm == null && lengthMm == null && !widthRange) return null;
    return { thicknessMm, widthMm, lengthMm, widthRange };
  } catch {
    return null;
  }
}

/** Read nominal dims from API row / seed / definitionJson. */
export function readNominalDims(src: MaterialDimsSource): NominalDims | null {
  const fromCols: NominalDims = {
    thicknessMm: num(src.nominalThicknessMm),
    widthMm: num(src.nominalWidthMm),
    lengthMm: num(src.nominalLengthMm),
    widthRange: src.widthRange ?? null,
  };
  if (fromCols.thicknessMm != null || fromCols.widthMm != null || fromCols.lengthMm != null) {
    return fromCols;
  }
  const fromDef = parseDefinitionNominal(src.definitionJson);
  if (fromDef) return fromDef;
  return null;
}

/** CODE · 18×1220×2440 mm */
export function formatMaterialCodeWithDims(src: MaterialDimsSource): string {
  const code = (src.code ?? '').trim() || '—';
  const dims = formatNominalDims(readNominalDims(src));
  if (!dims || dims === '—') return code;
  return `${code} · ${dims}`;
}

/** Build definitionJson fragment that always carries nominal dims beside identity. */
export function buildDefinitionWithNominal(
  base: Record<string, unknown>,
  dims: NominalDims,
): Record<string, unknown> {
  return {
    ...base,
    thicknessMm: dims.thicknessMm != null ? String(dims.thicknessMm) : (base.thicknessMm ?? ''),
    widthMm: dims.widthMm != null ? String(dims.widthMm) : (base.widthMm ?? ''),
    lengthMm: dims.lengthMm != null ? String(dims.lengthMm) : (base.lengthMm ?? ''),
    NominalThicknessMm: dims.thicknessMm,
    NominalWidthMm: dims.widthMm,
    NominalLengthMm: dims.lengthMm,
    widthRange: dims.widthRange ?? null,
  };
}

export type MasterSeedItem = {
  materialCode: string;
  materialName: string;
  mainCategory: string;
  woodSpecies?: string | null;
  productType?: string | null;
  quality?: string | null;
  nominalThicknessMm?: number | null;
  nominalWidthMm?: number | null;
  nominalLengthMm?: number | null;
  widthRange?: string | null;
  dimsLabel?: string | null;
  stockUnit?: string | null;
  countUnit?: string | null;
  label?: string | null;
  cardType?: string | null;
};

export function seedItemToDefinitionJson(item: MasterSeedItem): string {
  const dims: NominalDims = {
    thicknessMm: num(item.nominalThicknessMm),
    widthMm: num(item.nominalWidthMm),
    lengthMm: num(item.nominalLengthMm),
    widthRange: item.widthRange ?? null,
  };
  return JSON.stringify(
    buildDefinitionWithNominal(
      {
        mainCategory: item.mainCategory,
        species: item.woodSpecies,
        materialType: item.productType,
        grade: item.quality,
        stockUom: item.stockUnit ?? 'Piece',
        countUom: item.countUnit ?? 'PCS',
        dimsLabel: item.dimsLabel ?? formatNominalDims(dims),
        cardType: item.cardType ?? (dims.thicknessMm ? 'stock' : 'family'),
      },
      dims,
    ),
  );
}

export function seedItemToCreateBody(item: MasterSeedItem): {
  code: string;
  name: string;
  description: string;
  category: string;
  unitOfMeasure: string;
  status: string;
  definitionJson: string;
} {
  const dimsLabel = item.dimsLabel ?? formatNominalDims(readNominalDims(item));
  const description = [item.woodSpecies, dimsLabel !== '—' ? dimsLabel : null, item.productType, item.quality]
    .filter(Boolean)
    .join(' · ');
  return {
    code: item.materialCode,
    name: item.materialName,
    description: description.slice(0, 200),
    category: item.mainCategory,
    unitOfMeasure: item.stockUnit || item.countUnit || 'Piece',
    status: 'Active',
    definitionJson: seedItemToDefinitionJson(item),
  };
}

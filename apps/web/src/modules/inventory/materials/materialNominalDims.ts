/**
 * Nominal dimensions — separate fields, never a single "50x100x4000" blob.
 * Width range → NominalWidthMinMm / NominalWidthMaxMm (do not collapse to one number).
 * Multi-width options → NominalWidthOptionsMm[] (do not collapse).
 * No automatic SKU/variant generation.
 */

export type NominalDims = {
  thicknessMm: number | null;
  /** Single commercial width when not a range/options set */
  widthMm: number | null;
  lengthMm: number | null;
  widthMinMm?: number | null;
  widthMaxMm?: number | null;
  /** e.g. [92, 117, 138] or [68, 92, 118, 140, 166] */
  widthOptionsMm?: number[] | null;
};

export type MaterialDimsSource = {
  code?: string | null;
  name?: string | null;
  description?: string | null;
  definitionJson?: string | null;
  nominalThicknessMm?: number | null;
  nominalWidthMm?: number | null;
  nominalLengthMm?: number | null;
  nominalWidthMinMm?: number | null;
  nominalWidthMaxMm?: number | null;
  nominalWidthOptionsMm?: number[] | null;
};

function num(v: unknown): number | null {
  if (v == null || v === '') return null;
  const n = typeof v === 'number' ? v : Number(String(v).replace(',', '.'));
  return Number.isFinite(n) && n > 0 ? n : null;
}

function fmtPart(n: number): string {
  return Number.isInteger(n) ? String(n) : String(n);
}

/** Parse "1200-1220" → min/max. Do not collapse to a single width. */
export function parseWidthRange(raw: string | null | undefined): {
  min: number | null;
  max: number | null;
} {
  const s = String(raw ?? '')
    .trim()
    .replace(',', '.');
  if (!s) return { min: null, max: null };
  const m = s.match(/^(\d+(?:\.\d+)?)\s*[-–]\s*(\d+(?:\.\d+)?)$/);
  if (!m) return { min: null, max: null };
  const a = Number(m[1]);
  const b = Number(m[2]);
  if (!(a > 0 && b > 0)) return { min: null, max: null };
  return { min: Math.min(a, b), max: Math.max(a, b) };
}

/** Parse "92-117-138" or "68/92/118/140/166" → options (preserve all). */
export function parseWidthOptions(raw: string | null | undefined): number[] | null {
  const s = String(raw ?? '').trim();
  if (!s) return null;
  // range of exactly two numbers is min/max, not options
  if (/^\d+(?:[.,]\d+)?\s*[-–]\s*\d+(?:[.,]\d+)?$/.test(s.replace(',', '.'))) return null;
  const parts = s.split(/[-–/|,;\s]+/).map((p) => Number(p.replace(',', '.'))).filter((n) => Number.isFinite(n) && n > 0);
  if (parts.length < 2) return null;
  // if exactly 2 and looked like range, already handled
  return parts;
}

/**
 * Parse master-list ölçü text into separate nominal fields.
 * Never stores a single "50x100x4000" blob — always thickness / width(+range|options) / length.
 */
export function parseMasterOlcuToNominal(raw: string | null | undefined): NominalDims | null {
  const s = String(raw ?? '')
    .normalize('NFKC')
    .trim();
  if (!s) return null;

  // 50x100x4000 / 50 × 100 × 4000
  const triple = s.match(
    /^(\d+(?:[.,]\d+)?)\s*[x×X]\s*(\d+(?:[.,]\d+)?)\s*[x×X]\s*(\d+(?:[.,]\d+)?)/,
  );
  if (triple) {
    return {
      thicknessMm: num(triple[1].replace(',', '.')),
      widthMm: num(triple[2].replace(',', '.')),
      lengthMm: num(triple[3].replace(',', '.')),
    };
  }

  // 18 mm / 1200-1220 mm · 16 mm / 92-117-138 mm · 26 mm / 68/92/118
  const slash = s.match(
    /^(\d+(?:[.,]\d+)?)\s*mm?\s*[\/|]\s*(.+?)(?:\s*mm)?$/i,
  );
  if (slash) {
    const thicknessMm = num(slash[1].replace(',', '.'));
    const rest = slash[2].trim().replace(/\s*mm$/i, '').trim();
    const opts = parseWidthOptions(rest);
    if (opts) {
      return { thicknessMm, widthMm: null, lengthMm: null, widthOptionsMm: opts };
    }
    const range = parseWidthRange(rest);
    if (range.min != null && range.max != null) {
      return {
        thicknessMm,
        widthMm: null,
        lengthMm: null,
        widthMinMm: range.min,
        widthMaxMm: range.max,
      };
    }
    const w = num(rest.replace(',', '.'));
    return { thicknessMm, widthMm: w, lengthMm: null };
  }

  // bare 1200-1220
  const rangeOnly = parseWidthRange(s.replace(/\s*mm$/i, ''));
  if (rangeOnly.min != null && rangeOnly.max != null) {
    return {
      thicknessMm: null,
      widthMm: null,
      lengthMm: null,
      widthMinMm: rangeOnly.min,
      widthMaxMm: rangeOnly.max,
    };
  }

  return null;
}

export function formatNominalDims(dims: NominalDims | null | undefined): string {
  if (!dims) return '—';
  const t = dims.thicknessMm;
  const len = dims.lengthMm;
  const opts = dims.widthOptionsMm?.filter((n) => n > 0) ?? [];
  const wMin = dims.widthMinMm;
  const wMax = dims.widthMaxMm;
  const w = dims.widthMm;

  let widthPart: string | null = null;
  if (opts.length >= 2) widthPart = opts.map(fmtPart).join('/');
  else if (wMin != null && wMax != null && wMin !== wMax) widthPart = `${fmtPart(wMin)}–${fmtPart(wMax)}`;
  else if (w != null) widthPart = fmtPart(w);
  else if (wMin != null) widthPart = fmtPart(wMin);

  if (t != null && widthPart && len != null) return `${fmtPart(t)} × ${widthPart} × ${fmtPart(len)} mm`;
  if (t != null && widthPart) return `${fmtPart(t)} × ${widthPart} mm`;
  if (t != null && len != null) return `${fmtPart(t)} × ${fmtPart(len)} mm`;
  if (t != null) return `${fmtPart(t)} mm`;
  return '—';
}

export function parseDefinitionNominal(definitionJson?: string | null): NominalDims | null {
  if (!definitionJson?.trim()) return null;
  try {
    const def = JSON.parse(definitionJson) as Record<string, unknown>;
    const thicknessMm =
      num(def.NominalThicknessMm) ?? num(def.nominalThicknessMm) ?? num(def.thicknessMm);
    const lengthMm = num(def.NominalLengthMm) ?? num(def.nominalLengthMm) ?? num(def.lengthMm);
    let widthMinMm = num(def.NominalWidthMinMm) ?? num(def.nominalWidthMinMm);
    let widthMaxMm = num(def.NominalWidthMaxMm) ?? num(def.nominalWidthMaxMm);
    let widthOptionsMm: number[] | null = null;
    if (Array.isArray(def.NominalWidthOptionsMm)) {
      widthOptionsMm = (def.NominalWidthOptionsMm as unknown[])
        .map((x) => num(x))
        .filter((n): n is number => n != null);
    } else if (Array.isArray(def.nominalWidthOptionsMm)) {
      widthOptionsMm = (def.nominalWidthOptionsMm as unknown[])
        .map((x) => num(x))
        .filter((n): n is number => n != null);
    } else if (typeof def.widthOptions === 'string') {
      widthOptionsMm = parseWidthOptions(def.widthOptions);
    }

    // Legacy widthRange string
    const wr =
      (typeof def.widthRange === 'string' && def.widthRange) ||
      (typeof def.WidthRange === 'string' && def.WidthRange) ||
      null;
    if (wr && widthMinMm == null && widthMaxMm == null && !widthOptionsMm?.length) {
      const opts = parseWidthOptions(wr);
      if (opts) widthOptionsMm = opts;
      else {
        const r = parseWidthRange(wr);
        widthMinMm = r.min;
        widthMaxMm = r.max;
      }
    }

    const widthMm =
      num(def.NominalWidthMm) ??
      num(def.nominalWidthMm) ??
      num(def.widthMm) ??
      (widthMinMm != null && widthMaxMm != null && widthMinMm === widthMaxMm ? widthMinMm : null);

    if (
      thicknessMm == null &&
      widthMm == null &&
      lengthMm == null &&
      widthMinMm == null &&
      !widthOptionsMm?.length
    ) {
      return null;
    }
    return {
      thicknessMm,
      widthMm: widthOptionsMm?.length || (widthMinMm != null && widthMaxMm != null && widthMinMm !== widthMaxMm) ? null : widthMm,
      lengthMm,
      widthMinMm,
      widthMaxMm,
      widthOptionsMm: widthOptionsMm?.length ? widthOptionsMm : null,
    };
  } catch {
    return null;
  }
}

export function readNominalDims(src: MaterialDimsSource): NominalDims | null {
  const fromCols: NominalDims = {
    thicknessMm: num(src.nominalThicknessMm),
    widthMm: num(src.nominalWidthMm),
    lengthMm: num(src.nominalLengthMm),
    widthMinMm: num(src.nominalWidthMinMm),
    widthMaxMm: num(src.nominalWidthMaxMm),
    widthOptionsMm: src.nominalWidthOptionsMm ?? null,
  };
  if (
    fromCols.thicknessMm != null ||
    fromCols.widthMm != null ||
    fromCols.lengthMm != null ||
    fromCols.widthMinMm != null ||
    (fromCols.widthOptionsMm?.length ?? 0) > 0
  ) {
    return fromCols;
  }
  return parseDefinitionNominal(src.definitionJson);
}

export function formatMaterialCodeWithDims(src: MaterialDimsSource): string {
  const code = (src.code ?? '').trim() || '—';
  const dims = formatNominalDims(readNominalDims(src));
  if (!dims || dims === '—') return code;
  return `${code} · ${dims}`;
}

export function buildDefinitionWithNominal(
  base: Record<string, unknown>,
  dims: NominalDims,
): Record<string, unknown> {
  const singleW =
    dims.widthMm ??
    (dims.widthMinMm != null && dims.widthMaxMm != null && dims.widthMinMm === dims.widthMaxMm
      ? dims.widthMinMm
      : null);
  return {
    ...base,
    thicknessMm: dims.thicknessMm != null ? String(dims.thicknessMm) : (base.thicknessMm ?? ''),
    widthMm: singleW != null ? String(singleW) : (base.widthMm ?? ''),
    lengthMm: dims.lengthMm != null ? String(dims.lengthMm) : (base.lengthMm ?? ''),
    NominalThicknessMm: dims.thicknessMm,
    NominalWidthMm: singleW,
    NominalLengthMm: dims.lengthMm,
    NominalWidthMinMm: dims.widthMinMm ?? null,
    NominalWidthMaxMm: dims.widthMaxMm ?? null,
    NominalWidthOptionsMm: dims.widthOptionsMm ?? null,
    dimsLabel: formatNominalDims(dims),
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
  nominalWidthMinMm?: number | null;
  nominalWidthMaxMm?: number | null;
  nominalWidthOptionsMm?: number[] | null;
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
    widthMinMm: num(item.nominalWidthMinMm),
    widthMaxMm: num(item.nominalWidthMaxMm),
    widthOptionsMm: item.nominalWidthOptionsMm ?? null,
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

/** Duplicate fingerprint — type + wood + product type + quality + nominal dims. */
export type DuplicateFingerprint = {
  mainCategory: string;
  materialType: string;
  woodSpecies: string;
  productType: string;
  quality: string;
  thicknessMm: number | null;
  widthMm: number | null;
  widthMinMm: number | null;
  widthMaxMm: number | null;
  widthOptionsKey: string;
  lengthMm: number | null;
};

export function buildDuplicateFingerprint(input: {
  mainCategory: string;
  materialType: string;
  woodSpecies: string;
  productType?: string;
  quality?: string;
  dims: NominalDims;
}): DuplicateFingerprint {
  const opts = (input.dims.widthOptionsMm ?? []).slice().sort((a, b) => a - b);
  return {
    mainCategory: String(input.mainCategory || '').trim().toUpperCase(),
    materialType: String(input.materialType || '').trim().toUpperCase(),
    woodSpecies: String(input.woodSpecies || '').trim().toUpperCase(),
    productType: String(input.productType || '').trim().toUpperCase(),
    quality: String(input.quality || '').trim().toUpperCase(),
    thicknessMm: input.dims.thicknessMm,
    widthMm: input.dims.widthMm,
    widthMinMm: input.dims.widthMinMm ?? null,
    widthMaxMm: input.dims.widthMaxMm ?? null,
    widthOptionsKey: opts.join(','),
    lengthMm: input.dims.lengthMm,
  };
}

export function fingerprintsEqual(a: DuplicateFingerprint, b: DuplicateFingerprint): boolean {
  return (
    a.mainCategory === b.mainCategory &&
    a.materialType === b.materialType &&
    a.woodSpecies === b.woodSpecies &&
    a.productType === b.productType &&
    a.quality === b.quality &&
    a.thicknessMm === b.thicknessMm &&
    a.widthMm === b.widthMm &&
    a.widthMinMm === b.widthMinMm &&
    a.widthMaxMm === b.widthMaxMm &&
    a.widthOptionsKey === b.widthOptionsKey &&
    a.lengthMm === b.lengthMm
  );
}

export function fingerprintFromMaterialRow(row: {
  code?: string;
  category?: string;
  definitionJson?: string | null;
}): DuplicateFingerprint | null {
  const def = parseDefinitionNominal(row.definitionJson);
  if (!def) return null;
  let parsed: Record<string, unknown> = {};
  try {
    parsed = row.definitionJson ? (JSON.parse(row.definitionJson) as Record<string, unknown>) : {};
  } catch {
    parsed = {};
  }
  return buildDuplicateFingerprint({
    mainCategory: String(parsed.mainCategory ?? row.category ?? ''),
    materialType: String(parsed.materialTypeToken ?? parsed.materialType ?? ''),
    woodSpecies: String(parsed.woodToken ?? parsed.species ?? ''),
    productType: String(parsed.productTypeToken ?? parsed.productType ?? ''),
    quality: String(parsed.grade ?? parsed.quality ?? ''),
    dims: def,
  });
}

export function findDuplicateMaterial<T extends { code?: string; category?: string; definitionJson?: string | null; name?: string }>(
  candidates: T[],
  needle: DuplicateFingerprint,
): T | null {
  for (const c of candidates) {
    const fp = fingerprintFromMaterialRow(c);
    if (fp && fingerprintsEqual(fp, needle)) return c;
  }
  return null;
}

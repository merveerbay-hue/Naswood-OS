/** Client-side count qty rules — mirrors InventoryCountMath. */

export type CountQtyMode = 'Piece' | 'CubicMeter' | 'SquareMeter' | 'MeasuredVolume';

export type MaterialCountPolicy = {
  stockUnit: string;
  countUnit: string;
  mode: CountQtyMode;
  dimsRequired: boolean;
};

export function cubicMeters(t: number, w: number, l: number, pcs: number): number {
  return (t * w * l * pcs) / 1_000_000_000;
}

export function squareMeters(w: number, l: number, pcs: number): number {
  return (w * l * pcs) / 1_000_000;
}

export function difference(counted: number, system: number): number {
  return counted - system;
}

export function normalizeUnit(unit?: string | null): string {
  const u = String(unit ?? '')
    .trim()
    .toUpperCase()
    .replace('M³', 'M3')
    .replace('M²', 'M2');
  if (['PIECE', 'PCS', 'ADET', 'EA'].includes(u)) return 'PCS';
  if (['M3', 'CBM'].includes(u)) return 'M3';
  if (['M2', 'SQM'].includes(u)) return 'M2';
  return u || 'PCS';
}

export function resolvePolicy(input: {
  unitOfMeasure?: string | null;
  category?: string | null;
  definitionJson?: string | null;
}): MaterialCountPolicy {
  let def: Record<string, unknown> = {};
  try {
    if (input.definitionJson?.trim()) def = JSON.parse(input.definitionJson) as Record<string, unknown>;
  } catch {
    def = {};
  }
  const stock = normalizeUnit(String(def.stockUom ?? def.StockUom ?? input.unitOfMeasure ?? ''));
  const count = normalizeUnit(String(def.countUom ?? def.CountUom ?? 'PCS'));
  const main = String(def.mainCategory ?? def.MainCategory ?? input.category ?? '')
    .trim()
    .toUpperCase();
  const type = String(def.materialType ?? def.materialTypeToken ?? '')
    .trim()
    .toUpperCase();
  const cat = String(input.category ?? '').toUpperCase();
  const volumeReq = def.volumeCalcRequired === true || def.VolumeCalcRequired === true;
  const isLog =
    ['LOG', 'TOMRUK', 'LG'].includes(main) ||
    type.includes('TOMRUK') ||
    type.includes('LOG') ||
    cat.includes('TOMRUK') ||
    cat.includes('LOG');
  const isHardware =
    ['HW', 'EL', 'MK', 'HARDWARE'].includes(main) ||
    cat.includes('HIRDAVAT') ||
    cat.includes('HARDWARE') ||
    cat.includes('ELEKTRIK') ||
    cat.includes('MEKANIK');
  const isMasifPanel = main === 'MP' || main === 'PANEL' || cat.includes('MASIF');
  if (isLog) return { stockUnit: 'M3', countUnit: count || 'PCS', mode: 'MeasuredVolume', dimsRequired: false };
  if (isHardware || (stock === 'PCS' && !volumeReq && !isMasifPanel))
    return { stockUnit: stock, countUnit: count, mode: 'Piece', dimsRequired: false };
  if (isMasifPanel) return { stockUnit: 'M3', countUnit: count, mode: 'CubicMeter', dimsRequired: true };
  if (stock === 'M2') return { stockUnit: 'M2', countUnit: count, mode: 'SquareMeter', dimsRequired: true };
  if (stock === 'M3' || volumeReq) return { stockUnit: 'M3', countUnit: count, mode: 'CubicMeter', dimsRequired: true };
  return { stockUnit: stock, countUnit: count, mode: 'Piece', dimsRequired: false };
}

export function calculateStockQty(
  policy: MaterialCountPolicy,
  dims: {
    thicknessMm?: number | null;
    widthMm?: number | null;
    lengthMm?: number | null;
    pieceCount?: number | null;
    measuredVolumeM3?: number | null;
  },
): { ok: true; qty: number } | { ok: false; error: string } {
  if (dims.pieceCount != null && dims.pieceCount < 0) return { ok: false, error: 'Fiziksel adet negatif olamaz.' };
  if (policy.mode === 'Piece') {
    if (dims.pieceCount == null) return { ok: false, error: 'Adet gerekli.' };
    return { ok: true, qty: dims.pieceCount };
  }
  if (policy.mode === 'CubicMeter') {
    if (!dims.thicknessMm || !dims.widthMm || !dims.lengthMm || dims.pieceCount == null)
      return { ok: false, error: 'Kalınlık, genişlik, boy ve adet gerekli.' };
    return { ok: true, qty: cubicMeters(dims.thicknessMm, dims.widthMm, dims.lengthMm, dims.pieceCount) };
  }
  if (policy.mode === 'SquareMeter') {
    if (!dims.widthMm || !dims.lengthMm || dims.pieceCount == null)
      return { ok: false, error: 'Genişlik, boy ve adet gerekli.' };
    return { ok: true, qty: squareMeters(dims.widthMm, dims.lengthMm, dims.pieceCount) };
  }
  if (dims.measuredVolumeM3 == null || dims.measuredVolumeM3 < 0) return { ok: false, error: 'MeasuredVolumeM3 gerekli.' };
  if (dims.pieceCount == null) return { ok: false, error: 'Tomruk adedi gerekli.' };
  return { ok: true, qty: dims.measuredVolumeM3 };
}

export function lineStatus(systemQty: number, countedQty: number, hasPhysical: boolean): string {
  if (!hasPhysical && systemQty > 0 && countedQty === 0) return 'MISSING';
  if (systemQty === 0 && countedQty > 0) return 'UNEXPECTED';
  if (countedQty - systemQty === 0) return 'MATCHED';
  return 'VARIANCE';
}

export function physicalKey(
  materialCode: string,
  locationCode: string,
  lot: string,
  t?: number | null,
  w?: number | null,
  l?: number | null,
): string {
  return [materialCode, locationCode, lot || 'LOT-UNKNOWN', t ?? '', w ?? '', l ?? '']
    .map((x) => String(x).trim().toUpperCase())
    .join('|');
}

export function formatMm(t?: number | null, w?: number | null, l?: number | null): string {
  const parts = [t, w, l].filter((n) => n != null && Number(n) > 0);
  return parts.length ? parts.join('×') : '—';
}

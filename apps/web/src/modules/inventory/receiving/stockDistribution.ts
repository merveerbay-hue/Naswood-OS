/** Stage 6 — per-line / per-group warehouse·location distribution + post payload. */

import {
  formatGroupDims,
  formatPhysicalDims,
  groupVolumeM3,
  lineMaterialMatched,
  stockBasisQty,
  stockBasisVolumeM3,
  volumeFromMm,
  type IncomingLine,
  type PhysicalMeasureGroup,
} from './incomingLines';

export type StockBucket = 'available' | 'quarantine';

export type StockDistributionRow = {
  id: string;
  lineId: string;
  /** Physical group id; null = whole-line (no groups / leftover). */
  groupId: string | null;
  qty: string;
  warehouseCode: string;
  locationCode: string;
  bucket: StockBucket;
};

function parseQty(v: string): number {
  const n = Number(String(v).replace(',', '.'));
  return Number.isFinite(n) && n > 0 ? n : 0;
}

export function acceptedStockLines(lines: IncomingLine[]): IncomingLine[] {
  return lines.filter(
    (l) =>
      (l.preAccept === 'ok' || l.preAccept === 'conditional') &&
      l.countStatus === 'counted' &&
      l.stockAccept !== 'reject' &&
      lineMaterialMatched(l) &&
      stockBasisQty(l) > 0,
  );
}

export function defaultBucketForLine(line: IncomingLine): StockBucket {
  if (line.stockAccept === 'conditional' || line.preAccept === 'conditional') return 'quarantine';
  return 'available';
}

function mintDistId(lineId: string, groupId: string | null): string {
  return `dist-${lineId}-${groupId ?? 'line'}-${Date.now()}-${Math.floor(Math.random() * 1000)}`;
}

/** Seed one distribution row per physical group (or one per line). */
export function buildDefaultDistributions(
  lines: IncomingLine[],
  defaultWarehouse: string,
  defaultLocation: string,
): StockDistributionRow[] {
  const wh = defaultWarehouse.trim() || 'WH-RM';
  const locAvail = defaultLocation.trim() || 'A-03';
  const rows: StockDistributionRow[] = [];

  for (const line of acceptedStockLines(lines)) {
    const bucket = defaultBucketForLine(line);
    const loc = bucket === 'quarantine' ? 'K-01' : locAvail;
    const whCode = bucket === 'quarantine' && !defaultWarehouse.trim() ? 'WH-QA' : wh;

    if (line.physicalGroups.length > 0) {
      for (const g of line.physicalGroups) {
        const q = parseQty(g.qty);
        if (q <= 0) continue;
        rows.push({
          id: mintDistId(line.id, g.id),
          lineId: line.id,
          groupId: g.id,
          qty: String(q),
          warehouseCode: whCode,
          locationCode: loc,
          bucket,
        });
      }
    } else {
      const q = stockBasisQty(line);
      if (q <= 0) continue;
      rows.push({
        id: mintDistId(line.id, null),
        lineId: line.id,
        groupId: null,
        qty: String(q),
        warehouseCode: whCode,
        locationCode: loc,
        bucket,
      });
    }
  }
  return rows;
}

export function distributionSumForLine(dists: StockDistributionRow[], lineId: string): number {
  return dists.filter((d) => d.lineId === lineId).reduce((s, d) => s + parseQty(d.qty), 0);
}

export type DistributionValidation =
  | { ok: true }
  | {
      ok: false;
      code: 'empty' | 'over' | 'under' | 'missingWh' | 'invalidWh' | 'invalidLoc' | 'badQty';
      message: string;
      lineId?: string;
    };

/** Optional plant-scoped allowlist — FE mirror of API WH/Loc ownership checks. */
export type DistributionAllowlist = {
  warehouseCodes: ReadonlySet<string>;
  /** warehouseCode (upper) → location codes (upper) */
  locationsByWarehouse: ReadonlyMap<string, ReadonlySet<string>>;
};

/** RULE 5: distribution sum must equal accepted qty; never exceed. */
export function validateDistributions(
  lines: IncomingLine[],
  dists: StockDistributionRow[],
  allowlist?: DistributionAllowlist | null,
): DistributionValidation {
  const accepted = acceptedStockLines(lines);
  if (accepted.length === 0) {
    return { ok: false, code: 'empty', message: 'no-accepted' };
  }
  if (dists.length === 0) {
    return { ok: false, code: 'empty', message: 'no-dist' };
  }

  for (const d of dists) {
    if (!d.warehouseCode.trim() || !d.locationCode.trim()) {
      return { ok: false, code: 'missingWh', message: 'need-wh', lineId: d.lineId };
    }
    if (parseQty(d.qty) <= 0) {
      return { ok: false, code: 'badQty', message: 'bad-qty', lineId: d.lineId };
    }
    if (allowlist) {
      const wh = d.warehouseCode.trim().toUpperCase();
      const loc = d.locationCode.trim().toUpperCase();
      if (!allowlist.warehouseCodes.has(wh)) {
        return { ok: false, code: 'invalidWh', message: 'wh-not-in-plant', lineId: d.lineId };
      }
      const locs = allowlist.locationsByWarehouse.get(wh);
      if (!locs?.has(loc)) {
        return { ok: false, code: 'invalidLoc', message: 'loc-not-in-wh', lineId: d.lineId };
      }
    }
  }

  for (const line of accepted) {
    const acceptedQty = stockBasisQty(line);
    const sum = distributionSumForLine(dists, line.id);
    if (sum > acceptedQty + 1e-9) {
      return { ok: false, code: 'over', message: 'over-dist', lineId: line.id };
    }
    if (sum < acceptedQty - 1e-9) {
      return { ok: false, code: 'under', message: 'under-dist', lineId: line.id };
    }
  }

  // Extra dist rows for non-accepted lines are ignored at post time; flag if present with qty
  for (const d of dists) {
    if (!accepted.some((l) => l.id === d.lineId) && parseQty(d.qty) > 0) {
      return { ok: false, code: 'badQty', message: 'orphan-dist', lineId: d.lineId };
    }
  }

  return { ok: true };
}

export function sliceVolumeM3(
  line: IncomingLine,
  group: PhysicalMeasureGroup | null,
  sliceQty: number,
): number | null {
  if (sliceQty <= 0) return null;
  if (group) {
    const gQty = parseQty(group.qty);
    const gVol = groupVolumeM3(group);
    if (gVol != null && gQty > 0) return (gVol * sliceQty) / gQty;
    const t = Number(group.thicknessMm);
    const w = Number(group.widthMm);
    const len = Number(group.lengthMm);
    if (t > 0 && w > 0 && len > 0) return volumeFromMm(sliceQty, t, w, len);
    return null;
  }
  const total = stockBasisQty(line);
  const vol = stockBasisVolumeM3(line);
  if (vol != null && total > 0) return (vol * sliceQty) / total;
  return null;
}

export function distributionDimsLabel(line: IncomingLine, groupId: string | null): string {
  if (groupId) {
    const g = line.physicalGroups.find((x) => x.id === groupId);
    if (g) return formatGroupDims(g);
  }
  return formatPhysicalDims(line);
}

export type ExecutePostLine = {
  materialCode: string;
  materialId: string;
  warehouseCode: string;
  locationCode: string;
  lotNumber: string;
  packageNumber: string;
  materialIdentityNumber: string;
  quantity: number;
  unitOfMeasure: string;
  barcode: string;
  stockStatus: 'Available' | 'Quarantine';
  /** Actual physical dims for this stock slice (not material nominal). */
  actualThicknessMm?: number | null;
  actualWidthMm?: number | null;
  actualLengthMm?: number | null;
  actualDimsLabel?: string | null;
};

/** Operational lot for a goods-receipt / shipment session — not per dim group / WH / package. */
export function mintLotNumber(now = new Date()): string {
  const year = now.getFullYear();
  const seq = String(Math.floor(10000 + Math.random() * 90000)).padStart(5, '0');
  return `LOT-${year}-${seq}`;
}

function mintPreview(prefix: string) {
  if (prefix === 'LOT') return mintLotNumber();
  const seq =
    new Date().toISOString().slice(2, 10).replace(/-/g, '') +
    '-' +
    String(Math.floor(100000 + Math.random() * 900000));
  return `${prefix}-${seq}`;
}

/** Build multi-line execute payload from validated distributions. */
export function buildExecuteLines(
  lines: IncomingLine[],
  dists: StockDistributionRow[],
  sharedLot?: string,
): ExecutePostLine[] {
  const byId = new Map(acceptedStockLines(lines).map((l) => [l.id, l]));
  // One lot per receiving/shipment — shared across dim groups, warehouses, quality buckets.
  const lot = sharedLot?.trim() || mintLotNumber();
  const result: ExecutePostLine[] = [];

  for (const d of dists) {
    const line = byId.get(d.lineId);
    if (!line) continue;
    const qty = parseQty(d.qty);
    if (qty <= 0) continue;
    const mi = mintPreview('MI');
    const pkg = mintPreview('PKG');
    const group = d.groupId ? line.physicalGroups.find((g) => g.id === d.groupId) ?? null : null;
    const t = group ? Number(group.thicknessMm) : Number(line.physicalGroups[0]?.thicknessMm);
    const w = group ? Number(group.widthMm) : Number(line.physicalGroups[0]?.widthMm);
    const len = group ? Number(group.lengthMm) : Number(line.physicalGroups[0]?.lengthMm);
    const dimsLabel = distributionDimsLabel(line, d.groupId);
    result.push({
      materialCode: line.matchedMaterialCode.trim(),
      materialId: line.matchedMaterialId.trim(),
      warehouseCode: d.warehouseCode.trim(),
      locationCode: d.locationCode.trim(),
      lotNumber: lot,
      packageNumber: pkg,
      materialIdentityNumber: mi,
      quantity: qty,
      unitOfMeasure: 'Piece',
      barcode: pkg,
      stockStatus: d.bucket === 'quarantine' ? 'Quarantine' : 'Available',
      actualThicknessMm: Number.isFinite(t) && t > 0 ? t : null,
      actualWidthMm: Number.isFinite(w) && w > 0 ? w : null,
      actualLengthMm: Number.isFinite(len) && len > 0 ? len : null,
      actualDimsLabel: dimsLabel !== '—' ? dimsLabel : null,
    });
  }
  return result;
}

export function addSplitRow(
  dists: StockDistributionRow[],
  source: StockDistributionRow,
): StockDistributionRow[] {
  const srcQty = parseQty(source.qty);
  if (srcQty < 2) return dists;
  const a = Math.floor(srcQty / 2);
  const b = srcQty - a;
  return dists.flatMap((d) => {
    if (d.id !== source.id) return [d];
    return [
      { ...d, qty: String(a) },
      {
        ...d,
        id: mintDistId(d.lineId, d.groupId),
        qty: String(b),
        // Force operator to pick a (possibly different) active location under the same WH.
        locationCode: '',
      },
    ];
  });
}

export function removeDistributionRow(dists: StockDistributionRow[], id: string): StockDistributionRow[] {
  return dists.filter((d) => d.id !== id);
}

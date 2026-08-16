/** Stage 4 — Sipariş ↔ Gelen liste ↔ Fiziksel (adet + ölçü + hacim). */

import {
  finalLineQty,
  formatDocumentDims,
  formatPhysicalDims,
  lumberVolumeM3,
  linePhysicalVolumeM3,
  physicalDimsDifferFromDocument,
  type IncomingLine,
  type StockAcceptDecision,
} from './incomingLines';

export type CompareStatus = 'ok' | 'diff' | 'unmatched' | 'rejected' | 'dimDiff';
export type CompareDisposition = 'none' | 'accepted' | 'partial' | 'rejected' | 'match' | 'conditional';
export type CompareFilter = 'all' | 'diff' | 'unmatched' | 'rejected' | 'dimDiff';

export type CompareRow = {
  lineId: string;
  name: string;
  unit: IncomingLine['unit'];
  materialCode: string;
  preAccept: IncomingLine['preAccept'];
  stockAccept: StockAcceptDecision;
  poQty: number | null;
  documentQty: number;
  physicalQty: number;
  documentDims: string;
  physicalDims: string;
  poVolumeM3: number | null;
  documentVolumeM3: number | null;
  physicalVolumeM3: number | null;
  dimsDiffer: boolean;
  status: CompareStatus;
};

export function classifyCompareStatus(line: IncomingLine): CompareStatus {
  if (line.preAccept === 'reject') return 'rejected';
  const physical = finalLineQty(line);
  const doc = line.documentQty;
  const po = line.poQty;
  const dimDiff = physicalDimsDifferFromDocument(line);
  if (po == null) return 'unmatched';
  if (physical === doc && doc === po && !dimDiff) return 'ok';
  if (dimDiff && physical === doc && doc === po) return 'dimDiff';
  if (dimDiff) return 'dimDiff';
  return 'diff';
}

export function buildCompareRows(lines: IncomingLine[]): CompareRow[] {
  return lines.map((line) => {
    const physicalQty = finalLineQty(line);
    return {
      lineId: line.id,
      name: line.name,
      unit: line.unit,
      materialCode: line.matchedMaterialCode || '—',
      preAccept: line.preAccept,
      stockAccept: line.stockAccept,
      poQty: line.poQty,
      documentQty: line.documentQty,
      physicalQty,
      documentDims: formatDocumentDims(line),
      physicalDims: formatPhysicalDims(line),
      poVolumeM3: line.poQty != null ? lumberVolumeM3(line, line.poQty) : null,
      documentVolumeM3: lumberVolumeM3(line, line.documentQty),
      physicalVolumeM3: linePhysicalVolumeM3(line),
      dimsDiffer: physicalDimsDifferFromDocument(line),
      status: classifyCompareStatus(line),
    };
  });
}

export function compareSummary(rows: CompareRow[]) {
  return {
    total: rows.length,
    ok: rows.filter((r) => r.status === 'ok').length,
    diff: rows.filter((r) => r.status === 'diff').length,
    dimDiff: rows.filter((r) => r.status === 'dimDiff').length,
    unmatched: rows.filter((r) => r.status === 'unmatched').length,
    rejected: rows.filter((r) => r.status === 'rejected').length,
  };
}

export function compareExceptions(rows: CompareRow[]): CompareRow[] {
  return rows.filter((r) => r.status === 'diff' || r.status === 'unmatched' || r.status === 'dimDiff');
}

export function compareIsResolved(
  rows: CompareRow[],
  disposition: Record<string, CompareDisposition>,
  autoAcceptOk: boolean,
  lines: IncomingLine[],
): boolean {
  if (!autoAcceptOk) return false;
  const exceptions = compareExceptions(rows);
  const exceptionsOk = exceptions.every((r) => {
    const d = disposition[r.lineId] ?? 'none';
    const line = lines.find((l) => l.id === r.lineId);
    const sa = line?.stockAccept ?? 'none';
    return (
      d === 'accepted' ||
      d === 'partial' ||
      d === 'rejected' ||
      d === 'match' ||
      d === 'conditional' ||
      sa === 'ok' ||
      sa === 'conditional' ||
      sa === 'reject'
    );
  });
  if (!exceptionsOk) return false;
  const countable = rows.filter((r) => r.status !== 'rejected');
  return countable.every((r) => {
    const line = lines.find((l) => l.id === r.lineId);
    if (!line) return false;
    if (r.status === 'ok' && autoAcceptOk) {
      return line.stockAccept === 'ok' || line.stockAccept === 'conditional' || line.stockAccept === 'none'
        ? line.stockAccept !== 'none' || true
        : true;
    }
    // For ok lines, auto-set is enough if stockAccept still none — treat auto as ok
    if (r.status === 'ok') return true;
    return line.stockAccept === 'ok' || line.stockAccept === 'conditional' || line.stockAccept === 'reject';
  });
}

/** Exceptions need explicit stockAccept; uyumlu satırlar otomatik kabul ile geçer. */
export function compareReadyForResult(
  lines: IncomingLine[],
  _disposition: Record<string, CompareDisposition>,
  autoAcceptOk: boolean,
): boolean {
  if (!autoAcceptOk) return false;
  const rows = buildCompareRows(lines);
  for (const r of compareExceptions(rows)) {
    const line = lines.find((l) => l.id === r.lineId);
    if (!line) return false;
    if (line.stockAccept !== 'ok' && line.stockAccept !== 'conditional' && line.stockAccept !== 'reject') {
      return false;
    }
  }
  return true;
}

export function formatQty(value: number | null | undefined, unit?: string): string {
  if (value == null || !Number.isFinite(value)) return '—';
  return unit ? `${value} ${unit}` : String(value);
}

export function formatVolume(value: number | null | undefined): string {
  if (value == null || !Number.isFinite(value)) return '—';
  return `${value.toFixed(3)} m³`;
}

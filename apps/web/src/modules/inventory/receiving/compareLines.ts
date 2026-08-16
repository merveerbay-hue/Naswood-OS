/** Stage 4 — Sipariş ↔ Gelen liste ↔ Fiziksel sayım (same IncomingLine ids). */

import {
  finalLineQty,
  formatLineDims,
  type IncomingLine,
} from './incomingLines';

export type CompareStatus = 'ok' | 'diff' | 'unmatched' | 'rejected';
export type CompareDisposition = 'none' | 'accepted' | 'partial' | 'rejected' | 'match';
export type CompareFilter = 'all' | 'diff' | 'unmatched' | 'rejected';

export type CompareRow = {
  lineId: string;
  name: string;
  dims: string;
  unit: IncomingLine['unit'];
  materialCode: string;
  preAccept: IncomingLine['preAccept'];
  poQty: number | null;
  documentQty: number;
  physicalQty: number;
  status: CompareStatus;
};

export function classifyCompareStatus(line: IncomingLine): CompareStatus {
  if (line.preAccept === 'reject') return 'rejected';
  const physical = finalLineQty(line);
  const doc = line.documentQty;
  const po = line.poQty;
  if (po == null) return 'unmatched';
  if (physical === doc && doc === po) return 'ok';
  return 'diff';
}

export function buildCompareRows(lines: IncomingLine[]): CompareRow[] {
  return lines.map((line) => ({
    lineId: line.id,
    name: line.name,
    dims: formatLineDims(line),
    unit: line.unit,
    materialCode: line.matchedMaterialCode || '—',
    preAccept: line.preAccept,
    poQty: line.poQty,
    documentQty: line.documentQty,
    physicalQty: finalLineQty(line),
    status: classifyCompareStatus(line),
  }));
}

export function compareSummary(rows: CompareRow[]) {
  return {
    total: rows.length,
    ok: rows.filter((r) => r.status === 'ok').length,
    diff: rows.filter((r) => r.status === 'diff').length,
    unmatched: rows.filter((r) => r.status === 'unmatched').length,
    rejected: rows.filter((r) => r.status === 'rejected').length,
  };
}

/** Exceptions that need an operator disposition (ok + rejected auto). */
export function compareExceptions(rows: CompareRow[]): CompareRow[] {
  return rows.filter((r) => r.status === 'diff' || r.status === 'unmatched');
}

export function compareIsResolved(
  rows: CompareRow[],
  disposition: Record<string, CompareDisposition>,
  autoAcceptOk: boolean,
): boolean {
  if (!autoAcceptOk) return false;
  const exceptions = compareExceptions(rows);
  return exceptions.every((r) => {
    const d = disposition[r.lineId] ?? 'none';
    return d === 'accepted' || d === 'partial' || d === 'rejected' || d === 'match';
  });
}

export function formatQty(value: number | null | undefined, unit?: string): string {
  if (value == null || !Number.isFinite(value)) return '—';
  return unit ? `${value} ${unit}` : String(value);
}

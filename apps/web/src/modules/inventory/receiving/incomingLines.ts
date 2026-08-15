/** Shared incoming product lines — Stage 1 document → Stage 2 pre-accept → Stage 3 count. */

export type MaterialKind = 'lumber' | 'log' | 'lamella' | 'thermowood' | 'panel' | 'packaged' | 'other';

export type PreAcceptDecision = 'none' | 'ok' | 'conditional' | 'reject';

export type CountStatus = 'pending' | 'counting' | 'counted' | 'recheck';

export type PackageRow = { id: string; qty: string };

export type LogRow = { id: string; diameterCm: string; lengthM: string };

export type IncomingLine = {
  id: string;
  name: string;
  thicknessMm: number | null;
  widthMm: number | null;
  lengthMm: number | null;
  kind: MaterialKind;
  unit: 'adet' | 'm3' | 'm2';
  documentQty: number;
  /** From Stage 2 */
  preAccept: PreAcceptDecision;
  /** Stage 3 count */
  countStatus: CountStatus;
  aiQty: string;
  operatorQty: string;
  packages: string;
  perPackage: string;
  packageRows: PackageRow[];
  logCount: string;
  logTotalM3: string;
  logRows: LogRow[];
  note: string;
};

export function emptyIncomingLineFields(): Pick<
  IncomingLine,
  | 'aiQty'
  | 'operatorQty'
  | 'packages'
  | 'perPackage'
  | 'packageRows'
  | 'logCount'
  | 'logTotalM3'
  | 'logRows'
  | 'note'
  | 'countStatus'
> {
  return {
    aiQty: '',
    operatorQty: '',
    packages: '',
    perPackage: '',
    packageRows: [],
    logCount: '',
    logTotalM3: '',
    logRows: [],
    note: '',
    countStatus: 'pending',
  };
}

/** Seed from Stage 1 document extract (demo). Pre-accept starts undecided. */
export function seedIncomingFromDocuments(): IncomingLine[] {
  return DEMO_INCOMING_FROM_DOCUMENTS.map((line) => ({
    ...line,
    ...emptyIncomingLineFields(),
    packages: line.kind === 'packaged' || line.kind === 'thermowood' || line.id === 'in-2' ? line.packages : '',
    perPackage: line.kind === 'packaged' || line.kind === 'thermowood' || line.id === 'in-2' ? line.perPackage : '',
    logCount: line.kind === 'log' ? String(line.documentQty) : '',
    preAccept: 'none',
  }));
}

/** Batch decision derived from per-line Stage 2 pre-accept. */
export function syncBatchPreAccept(lines: IncomingLine[]): PreAcceptDecision {
  if (lines.length === 0 || lines.some((l) => l.preAccept === 'none')) return 'none';
  if (lines.every((l) => l.preAccept === 'reject')) return 'reject';
  if (lines.some((l) => l.preAccept === 'ok')) return 'ok';
  if (lines.some((l) => l.preAccept === 'conditional')) return 'conditional';
  return 'none';
}

/** Demo lines as if extracted from Stage 1 documents (irsaliye/excel/pdf). */
export const DEMO_INCOMING_FROM_DOCUMENTS: IncomingLine[] = [
  {
    id: 'in-1',
    name: 'Çam Kereste',
    thicknessMm: 26,
    widthMm: 140,
    lengthMm: 3000,
    kind: 'lumber',
    unit: 'adet',
    documentQty: 500,
    preAccept: 'ok',
    countStatus: 'pending',
    aiQty: '',
    operatorQty: '',
    packages: '',
    perPackage: '',
    packageRows: [],
    logCount: '',
    logTotalM3: '',
    logRows: [],
    note: '',
  },
  {
    id: 'in-2',
    name: 'Çam Kereste',
    thicknessMm: 26,
    widthMm: 92,
    lengthMm: 3000,
    kind: 'lumber',
    unit: 'adet',
    documentQty: 300,
    preAccept: 'ok',
    countStatus: 'pending',
    aiQty: '',
    operatorQty: '',
    packages: '6',
    perPackage: '50',
    packageRows: [],
    logCount: '',
    logTotalM3: '',
    logRows: [],
    note: '',
  },
  {
    id: 'in-3',
    name: 'Çam Tomruk',
    thicknessMm: null,
    widthMm: null,
    lengthMm: null,
    kind: 'log',
    unit: 'adet',
    documentQty: 42,
    preAccept: 'ok',
    countStatus: 'pending',
    aiQty: '',
    operatorQty: '',
    packages: '',
    perPackage: '',
    packageRows: [],
    logCount: '42',
    logTotalM3: '',
    logRows: [],
    note: '',
  },
  {
    id: 'in-4',
    name: 'Thermowood Deck',
    thicknessMm: 26,
    widthMm: 140,
    lengthMm: 3000,
    kind: 'thermowood',
    unit: 'adet',
    documentQty: 100,
    preAccept: 'conditional',
    countStatus: 'pending',
    aiQty: '',
    operatorQty: '',
    packages: '4',
    perPackage: '25',
    packageRows: [],
    logCount: '',
    logTotalM3: '',
    logRows: [],
    note: '',
  },
  {
    id: 'in-5',
    name: 'Masif Panel',
    thicknessMm: 20,
    widthMm: 1200,
    lengthMm: 2400,
    kind: 'panel',
    unit: 'adet',
    documentQty: 20,
    preAccept: 'reject',
    countStatus: 'pending',
    aiQty: '',
    operatorQty: '',
    packages: '',
    perPackage: '',
    packageRows: [],
    logCount: '',
    logTotalM3: '',
    logRows: [],
    note: '',
  },
];

export function formatLineDims(line: IncomingLine): string {
  if (line.thicknessMm != null && line.widthMm != null && line.lengthMm != null) {
    return `${line.thicknessMm}×${line.widthMm}×${line.lengthMm} mm`;
  }
  return '—';
}

export function countableLines(lines: IncomingLine[]): IncomingLine[] {
  return lines.filter((l) => l.preAccept === 'ok' || l.preAccept === 'conditional');
}

export function finalLineQty(line: IncomingLine): number {
  const op = Number(String(line.operatorQty).replace(',', '.'));
  if (Number.isFinite(op) && op > 0) return op;
  if (line.kind === 'log') {
    const lc = Number(line.logCount);
    if (Number.isFinite(lc) && lc > 0) return lc;
  }
  const ai = Number(String(line.aiQty).replace(',', '.'));
  if (Number.isFinite(ai) && ai > 0) return ai;
  return 0;
}

/** m³ from piece count × dims in mm. */
export function lumberVolumeM3(line: IncomingLine, qty: number): number | null {
  if (line.thicknessMm == null || line.widthMm == null || line.lengthMm == null) return null;
  if (!Number.isFinite(qty) || qty <= 0) return null;
  return (qty * line.thicknessMm * line.widthMm * line.lengthMm) / 1_000_000_000;
}

export function packageTotal(line: IncomingLine): number | null {
  if (line.packageRows.length > 0) {
    const sum = line.packageRows.reduce((s, r) => s + (Number(r.qty) || 0), 0);
    return sum > 0 ? sum : null;
  }
  const p = Number(line.packages);
  const n = Number(line.perPackage);
  if (Number.isFinite(p) && Number.isFinite(n) && p > 0 && n > 0) return p * n;
  return null;
}

export function lineIsCounted(line: IncomingLine): boolean {
  return line.countStatus === 'counted' && finalLineQty(line) > 0;
}

export function allCountableCounted(lines: IncomingLine[]): boolean {
  const list = countableLines(lines);
  return list.length > 0 && list.every(lineIsCounted);
}

/** Rough match handwriting/excel product text → incoming line id. */
export function matchIncomingByLabel(lines: IncomingLine[], label: string): IncomingLine | null {
  const n = label.toLowerCase().replace(/[×x]/gi, 'x').replace(/\s+/g, '');
  let best: IncomingLine | null = null;
  let bestScore = 0;
  for (const line of countableLines(lines)) {
    const key = `${line.name}${formatLineDims(line)}`.toLowerCase().replace(/[×x]/gi, 'x').replace(/\s+/g, '');
    let score = 0;
    if (key.includes(n) || n.includes(key)) score = 90;
    else {
      const tokens = label.toLowerCase().split(/\s+/).filter((t) => t.length > 2);
      const hit = tokens.filter((t) => key.includes(t.replace('×', 'x'))).length;
      score = tokens.length ? Math.round((hit / tokens.length) * 80) : 0;
      if (line.thicknessMm && n.includes(String(line.thicknessMm))) score += 5;
      if (line.widthMm && n.includes(String(line.widthMm))) score += 5;
      if (line.lengthMm && n.includes(String(line.lengthMm))) score += 5;
    }
    if (score > bestScore) {
      bestScore = score;
      best = line;
    }
  }
  return bestScore >= 40 ? best : null;
}

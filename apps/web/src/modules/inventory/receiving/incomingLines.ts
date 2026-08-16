/** Shared incoming product lines — Stage 1 document → Stage 2 check → Stage 3 count. */

export type MaterialKind = 'lumber' | 'log' | 'lamella' | 'thermowood' | 'panel' | 'packaged' | 'other';

export type PreAcceptDecision = 'none' | 'ok' | 'conditional' | 'reject';

export type CountStatus = 'pending' | 'counting' | 'counted' | 'recheck';

export type MeasureSource = 'manual' | 'ai' | 'handwriting' | 'excel';

export type StockAcceptDecision = 'none' | 'ok' | 'conditional' | 'reject';

/** One physical measure group (same gerçek ölçü). Do not average different dims. */
export type PhysicalMeasureGroup = {
  id: string;
  qty: string;
  thicknessMm: string;
  widthMm: string;
  lengthMm: string;
  source: MeasureSource;
  note: string;
};

export type PackageRow = { id: string; qty: string };

export type LogRow = { id: string; diameterCm: string; lengthM: string };

export type MoistureSample = { id: string; label: string; valuePct: string };

export type DimSample = {
  id: string;
  label: string;
  thickness: string;
  width: string;
  length: string;
};

export type IncomingLine = {
  id: string;
  name: string;
  thicknessMm: number | null;
  widthMm: number | null;
  lengthMm: number | null;
  kind: MaterialKind;
  unit: 'adet' | 'm3' | 'm2';
  documentQty: number;
  /** Purchase order / sipariş qty (null = not on PO). Demo for Stage 4 compare. */
  poQty: number | null;
  /** Stage 2 — per-line moisture */
  targetMoisturePct: string;
  moistureSamples: MoistureSample[];
  /** Stage 2 — per-line dimension samples (targets = document dims) */
  dimSamples: DimSample[];
  /** Stage 2 — pre-accept after checks */
  preAccept: PreAcceptDecision;
  /** Stage 2 — material master match (required for ok/conditional) */
  matchConfirmed: boolean;
  matchedMaterialId: string;
  matchedMaterialCode: string;
  matchScore: number | null;
  /** Stage 3 — physical measure groups (adet + gerçek ölçü). Document dims stay on thicknessMm/widthMm/lengthMm. */
  physicalGroups: PhysicalMeasureGroup[];
  /** Stage 4 — stock acceptance (conditional when physical dims differ from document). */
  stockAccept: StockAcceptDecision;
  stockAcceptReason: string;
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

type LineIdentity = Pick<IncomingLine, 'id' | 'thicknessMm' | 'widthMm' | 'lengthMm' | 'kind'>;

function demoMoisture(lineId: string): MoistureSample[] {
  return [
    { id: `${lineId}-m1`, label: 'Numune 1', valuePct: '' },
    { id: `${lineId}-m2`, label: 'Numune 2', valuePct: '' },
  ];
}

function demoDims(line: LineIdentity): DimSample[] {
  if (line.kind === 'log' || line.thicknessMm == null) return [];
  return [
    {
      id: `${line.id}-d1`,
      label: 'Numune 1',
      thickness: String(line.thicknessMm),
      width: String(line.widthMm ?? ''),
      length: String(line.lengthMm ?? ''),
    },
    { id: `${line.id}-d2`, label: 'Numune 2', thickness: '', width: '', length: '' },
  ];
}

export function emptyIncomingLineFields(line: LineIdentity): Pick<
  IncomingLine,
  | 'targetMoisturePct'
  | 'moistureSamples'
  | 'dimSamples'
  | 'matchConfirmed'
  | 'matchedMaterialId'
  | 'matchedMaterialCode'
  | 'matchScore'
  | 'physicalGroups'
  | 'stockAccept'
  | 'stockAcceptReason'
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
    targetMoisturePct: '12',
    moistureSamples: demoMoisture(line.id),
    dimSamples: demoDims(line),
    matchConfirmed: false,
    matchedMaterialId: '',
    matchedMaterialCode: '',
    matchScore: null,
    physicalGroups: [],
    stockAccept: 'none',
    stockAcceptReason: '',
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

type DemoSpec = {
  id: string;
  name: string;
  thicknessMm: number | null;
  widthMm: number | null;
  lengthMm: number | null;
  kind: MaterialKind;
  unit: 'adet' | 'm3' | 'm2';
  documentQty: number;
  poQty: number | null;
  preAccept: PreAcceptDecision;
  packages?: string;
  perPackage?: string;
  logCount?: string;
};

function buildDemoLine(s: DemoSpec): IncomingLine {
  const empty = emptyIncomingLineFields(s);
  return {
    id: s.id,
    name: s.name,
    thicknessMm: s.thicknessMm,
    widthMm: s.widthMm,
    lengthMm: s.lengthMm,
    kind: s.kind,
    unit: s.unit,
    documentQty: s.documentQty,
    poQty: s.poQty,
    preAccept: s.preAccept,
    ...empty,
    packages: s.packages ?? '',
    perPackage: s.perPackage ?? '',
    logCount: s.logCount ?? '',
  };
}

/** Demo lines as if extracted from Stage 1 documents (irsaliye/excel/pdf). */
export const DEMO_INCOMING_FROM_DOCUMENTS: IncomingLine[] = [
  buildDemoLine({
    id: 'in-1',
    name: 'Çam Kereste',
    thicknessMm: 50,
    widthMm: 100,
    lengthMm: 4000,
    kind: 'lumber',
    unit: 'adet',
    documentQty: 100,
    poQty: 100,
    preAccept: 'ok',
  }),
  buildDemoLine({
    id: 'in-2',
    name: 'Çam Kereste',
    thicknessMm: 26,
    widthMm: 92,
    lengthMm: 3000,
    kind: 'lumber',
    unit: 'adet',
    documentQty: 300,
    poQty: 300,
    preAccept: 'ok',
    packages: '6',
    perPackage: '50',
  }),
  buildDemoLine({
    id: 'in-3',
    name: 'Çam Tomruk',
    thicknessMm: null,
    widthMm: null,
    lengthMm: null,
    kind: 'log',
    unit: 'adet',
    documentQty: 42,
    poQty: 40,
    preAccept: 'ok',
    logCount: '42',
  }),
  buildDemoLine({
    id: 'in-4',
    name: 'Thermowood Deck',
    thicknessMm: 26,
    widthMm: 140,
    lengthMm: 3000,
    kind: 'thermowood',
    unit: 'adet',
    documentQty: 100,
    poQty: null,
    preAccept: 'conditional',
    packages: '4',
    perPackage: '25',
  }),
  buildDemoLine({
    id: 'in-5',
    name: 'Masif Panel',
    thicknessMm: 20,
    widthMm: 1200,
    lengthMm: 2400,
    kind: 'panel',
    unit: 'adet',
    documentQty: 20,
    poQty: 20,
    preAccept: 'reject',
  }),
];

/** Seed from Stage 1 document extract (demo). Pre-accept starts undecided. */
export function seedIncomingFromDocuments(): IncomingLine[] {
  return DEMO_INCOMING_FROM_DOCUMENTS.map((line) => ({
    ...line,
    ...emptyIncomingLineFields(line),
    packages: line.packages,
    perPackage: line.perPackage,
    logCount: line.logCount,
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

export function lineHasDims(line: IncomingLine): boolean {
  return line.thicknessMm != null && line.widthMm != null && line.lengthMm != null;
}

/** At least one moisture reading entered. */
export function lineMoistureChecked(line: IncomingLine): boolean {
  return line.moistureSamples.some((s) => {
    const n = Number(String(s.valuePct).replace(',', '.'));
    return Number.isFinite(n) && n > 0;
  });
}

/** For dimmed products: at least one full sample; logs/no-dims skip. */
export function lineDimsChecked(line: IncomingLine): boolean {
  if (!lineHasDims(line)) return true;
  return line.dimSamples.some((s) => {
    const t = Number(String(s.thickness).replace(',', '.'));
    const w = Number(String(s.width).replace(',', '.'));
    const len = Number(String(s.length).replace(',', '.'));
    return Number.isFinite(t) && Number.isFinite(w) && Number.isFinite(len) && t > 0 && w > 0 && len > 0;
  });
}

export function lineChecksComplete(line: IncomingLine): boolean {
  return lineMoistureChecked(line) && lineDimsChecked(line) && line.preAccept !== 'none';
}

export function lineMaterialMatched(line: IncomingLine): boolean {
  return line.matchConfirmed && !!line.matchedMaterialId.trim() && !!line.matchedMaterialCode.trim();
}

/** Reject needs checks only; ok/conditional also need material master match. */
export function lineReadyForNextStage(line: IncomingLine): boolean {
  if (!lineChecksComplete(line)) return false;
  if (line.preAccept === 'reject') return true;
  return lineMaterialMatched(line);
}

export function allLinesChecksComplete(lines: IncomingLine[]): boolean {
  return lines.length > 0 && lines.every(lineChecksComplete);
}

export function allLinesReadyForCount(lines: IncomingLine[]): boolean {
  return lines.length > 0 && lines.every(lineReadyForNextStage) && countableLines(lines).length > 0;
}

export function lineMatchLabel(line: IncomingLine): string {
  return `${line.name} ${formatLineDims(line)}`.replace(/\s+—\s*$/, '').trim();
}

export function formatLineDims(line: IncomingLine): string {
  if (line.thicknessMm != null && line.widthMm != null && line.lengthMm != null) {
    return `${line.thicknessMm}×${line.widthMm}×${line.lengthMm} mm`;
  }
  return '—';
}

export function countableLines(lines: IncomingLine[]): IncomingLine[] {
  return lines.filter((l) => l.preAccept === 'ok' || l.preAccept === 'conditional');
}

function parseMm(v: string): number | null {
  const n = Number(String(v).replace(',', '.'));
  return Number.isFinite(n) && n > 0 ? n : null;
}

export function createEmptyPhysicalGroup(
  lineId: string,
  source: MeasureSource = 'manual',
  seed?: Partial<PhysicalMeasureGroup>,
): PhysicalMeasureGroup {
  return {
    id: `${lineId}-pg-${Date.now()}-${Math.floor(Math.random() * 1000)}`,
    qty: seed?.qty ?? '',
    thicknessMm: seed?.thicknessMm ?? '',
    widthMm: seed?.widthMm ?? '',
    lengthMm: seed?.lengthMm ?? '',
    source: seed?.source ?? source,
    note: seed?.note ?? '',
  };
}

/** Volume of one group: adet × T × W × L (mm → m). */
export function groupVolumeM3(group: PhysicalMeasureGroup): number | null {
  const qty = parseMm(group.qty);
  const t = parseMm(group.thicknessMm);
  const w = parseMm(group.widthMm);
  const len = parseMm(group.lengthMm);
  if (qty == null || t == null || w == null || len == null) return null;
  return (qty * t * w * len) / 1_000_000_000;
}

export function formatGroupDims(group: PhysicalMeasureGroup): string {
  const t = group.thicknessMm.trim();
  const w = group.widthMm.trim();
  const len = group.lengthMm.trim();
  if (!t || !w || !len) return '—';
  return `${t}×${w}×${len} mm`;
}

export function groupIsComplete(group: PhysicalMeasureGroup): boolean {
  return groupVolumeM3(group) != null || (parseMm(group.qty) != null && !group.thicknessMm && !group.widthMm);
}

/** Sum of physical group qtys; falls back to operator/ai/log. */
export function finalLineQty(line: IncomingLine): number {
  if (line.physicalGroups.length > 0) {
    const sum = line.physicalGroups.reduce((s, g) => s + (parseMm(g.qty) ?? 0), 0);
    if (sum > 0) return sum;
  }
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

/** Total physical m³ from groups; else document dims × qty (legacy display). */
export function linePhysicalVolumeM3(line: IncomingLine): number | null {
  if (line.physicalGroups.length > 0) {
    let total = 0;
    let any = false;
    for (const g of line.physicalGroups) {
      const v = groupVolumeM3(g);
      if (v != null) {
        total += v;
        any = true;
      }
    }
    return any ? total : null;
  }
  return lumberVolumeM3(line, finalLineQty(line));
}

/** Document/nominal dims (material card / gelen liste) — never mutated by physical measure. */
export function formatDocumentDims(line: IncomingLine): string {
  return formatLineDims(line);
}

/** Physical dims summary (one group or "N grup"). */
export function formatPhysicalDims(line: IncomingLine): string {
  const complete = line.physicalGroups.filter((g) => parseMm(g.thicknessMm) && parseMm(g.widthMm) && parseMm(g.lengthMm));
  if (complete.length === 0) return '—';
  if (complete.length === 1) return formatGroupDims(complete[0]!);
  return complete.map((g) => `${parseMm(g.qty) ?? '?'}@${formatGroupDims(g)}`).join(' · ');
}

/** Stock basis = physical groups (after ok/conditional stockAccept). */
export function stockBasisQty(line: IncomingLine): number {
  if (line.stockAccept === 'reject' || line.preAccept === 'reject') return 0;
  return finalLineQty(line);
}

export function stockBasisVolumeM3(line: IncomingLine): number | null {
  if (line.stockAccept === 'reject' || line.preAccept === 'reject') return null;
  return linePhysicalVolumeM3(line);
}

export function stockBasisDimsLabel(line: IncomingLine): string {
  if (line.stockAccept === 'reject' || line.preAccept === 'reject') return '—';
  const phys = formatPhysicalDims(line);
  return phys !== '—' ? phys : formatDocumentDims(line);
}

export function physicalDimsDifferFromDocument(line: IncomingLine): boolean {
  if (!lineHasDims(line) || line.physicalGroups.length === 0) return false;
  return line.physicalGroups.some((g) => {
    const t = parseMm(g.thicknessMm);
    const w = parseMm(g.widthMm);
    const len = parseMm(g.lengthMm);
    if (t == null || w == null || len == null) return false;
    return t !== line.thicknessMm || w !== line.widthMm || len !== line.lengthMm;
  });
}

/** m³ from piece count × dims in mm (document dims — for PO/doc volume only). */
export function lumberVolumeM3(line: IncomingLine, qty: number): number | null {
  if (line.thicknessMm == null || line.widthMm == null || line.lengthMm == null) return null;
  if (!Number.isFinite(qty) || qty <= 0) return null;
  return (qty * line.thicknessMm * line.widthMm * line.lengthMm) / 1_000_000_000;
}

export function volumeFromMm(qty: number, thicknessMm: number, widthMm: number, lengthMm: number): number {
  return (qty * thicknessMm * widthMm * lengthMm) / 1_000_000_000;
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
  if (line.countStatus !== 'counted') return false;
  if (line.physicalGroups.length > 0) {
    return (
      finalLineQty(line) > 0 &&
      line.physicalGroups.every(
        (g) =>
          parseMm(g.qty) != null &&
          (line.kind === 'log' ||
            (parseMm(g.thicknessMm) != null && parseMm(g.widthMm) != null && parseMm(g.lengthMm) != null)),
      )
    );
  }
  return finalLineQty(line) > 0;
}

export function allCountableCounted(lines: IncomingLine[]): boolean {
  const list = countableLines(lines);
  return list.length > 0 && list.every(lineIsCounted);
}

export function allCountableStockAccepted(lines: IncomingLine[]): boolean {
  const list = countableLines(lines);
  return (
    list.length > 0 &&
    list.every((l) => l.stockAccept === 'ok' || l.stockAccept === 'conditional' || l.stockAccept === 'reject')
  );
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

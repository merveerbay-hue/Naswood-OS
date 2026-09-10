import { downloadTextFile, parseMaterialCardWorkbook } from '../materials/materialCardBulk';
import { calculateStockQty, type MaterialCountPolicy } from './inventoryCountCalc';

export const COUNT_TEMPLATE_COLUMNS = [
  'MaterialCode',
  'MaterialName',
  'ThicknessMm',
  'WidthMm',
  'LengthMm',
  'PieceCount',
  'MeasuredVolumeM3',
  'LocationCode',
  'Note',
] as const;

export type CountExcelRow = {
  materialCode: string;
  materialName: string;
  thicknessMm: number | null;
  widthMm: number | null;
  lengthMm: number | null;
  pieceCount: number | null;
  measuredVolumeM3: number | null;
  locationCode: string;
  note: string;
  match: 'code' | 'name-review' | 'missing';
  matchMessage?: string;
};

function csvCell(v: string): string {
  if (/[;"\n]/.test(v)) return `"${v.replace(/"/g, '""')}"`;
  return v;
}

export function buildCountTemplateCsv(rows: CountExcelRow[]): string {
  const header = COUNT_TEMPLATE_COLUMNS.join(';');
  const body = rows.map((r) =>
    [
      r.materialCode,
      r.materialName,
      r.thicknessMm ?? '',
      r.widthMm ?? '',
      r.lengthMm ?? '',
      r.pieceCount ?? '',
      r.measuredVolumeM3 ?? '',
      r.locationCode,
      r.note,
    ]
      .map((c) => csvCell(String(c)))
      .join(';'),
  );
  return [header, ...body].join('\n');
}

export function downloadCountTemplate(filename: string, rows: CountExcelRow[]) {
  downloadTextFile(filename, '\uFEFF' + buildCountTemplateCsv(rows), 'text/csv;charset=utf-8');
}

function num(v: unknown): number | null {
  const n = Number(String(v ?? '').replace(',', '.').trim());
  return Number.isFinite(n) && String(v ?? '').trim() !== '' ? n : null;
}

export async function parseCountWorkbook(file: File): Promise<string[][]> {
  return parseMaterialCardWorkbook(file);
}

export function mapCountSheet(
  table: string[][],
  materials: { code?: string; name?: string }[],
): CountExcelRow[] {
  if (!table.length) return [];
  const headers = table[0]!.map((h) =>
    String(h || '')
      .trim()
      .toLowerCase()
      .replace(/[^a-z0-9]/g, ''),
  );
  const idx = (names: string[]) => headers.findIndex((h) => names.includes(h));
  const iCode = idx(['materialcode', 'malzemekodu', 'kod']);
  const iName = idx(['materialname', 'malzemeadi', 'malzeme', 'ad']);
  const iT = idx(['thicknessmm', 'kalinlik', 't']);
  const iW = idx(['widthmm', 'genislik', 'w']);
  const iL = idx(['lengthmm', 'uzunluk', 'boy', 'l']);
  const iPcs = idx(['piececount', 'adet', 'pcs', 'miktar']);
  const iVol = idx(['measuredvolumem3', 'hacim', 'm3']);
  const iLoc = idx(['locationcode', 'lokasyon']);
  const iNote = idx(['note', 'not', 'aciklama']);

  const out: CountExcelRow[] = [];
  for (const row of table.slice(1)) {
    if (!row.some((c) => String(c ?? '').trim())) continue;
    const code = String(row[iCode] ?? '').trim();
    const name = String(row[iName] ?? '').trim();
    let match: CountExcelRow['match'] = 'missing';
    let matchMessage: string | undefined;
    let resolvedCode = code;
    if (code) {
      const hit = materials.find((m) => String(m.code ?? '').toUpperCase() === code.toUpperCase());
      match = hit ? 'code' : 'missing';
      if (!hit) matchMessage = 'Malzeme kartı bulunamadı';
    } else if (name) {
      const hits = materials.filter(
        (m) => String(m.name ?? '').trim().toLowerCase() === name.toLowerCase(),
      );
      if (hits.length === 1) {
        match = 'code';
        resolvedCode = String(hits[0]!.code ?? '');
      } else {
        match = 'name-review';
        matchMessage = 'Eşleşme gerekli';
      }
    }
    out.push({
      materialCode: resolvedCode,
      materialName: name,
      thicknessMm: iT >= 0 ? num(row[iT]) : null,
      widthMm: iW >= 0 ? num(row[iW]) : null,
      lengthMm: iL >= 0 ? num(row[iL]) : null,
      pieceCount: iPcs >= 0 ? num(row[iPcs]) : null,
      measuredVolumeM3: iVol >= 0 ? num(row[iVol]) : null,
      locationCode: iLoc >= 0 ? String(row[iLoc] ?? '').trim() : '',
      note: iNote >= 0 ? String(row[iNote] ?? '').trim() : '',
      match,
      matchMessage,
    });
  }
  return out;
}

export function countedQtyForExcelRow(row: CountExcelRow, policy: MaterialCountPolicy) {
  return calculateStockQty(policy, row);
}

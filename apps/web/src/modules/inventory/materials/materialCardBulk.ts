/**
 * Excel/CSV template + parse for bulk material-card create.
 * MaterialCode is never a column — the system mints it on save.
 */
import {
  HM_TYPE_OPTIONS,
  MP_WOOD_OPTIONS,
  TW_FAMILY_OPTIONS,
  WOOD_OPTIONS_HM,
  YM_TYPE_OPTIONS,
  mintMaterialCode,
  previewMaterialCode,
  type MainCategory,
} from './materialCoding';
import type { ComplianceScope, GradingMethod } from './materialCompliance';
import {
  buildNominalFromDraft,
  codingInputFromDraft,
  effectiveWoodToken,
  findDraftDuplicate,
  suggestedNameForDraft,
  validateDraftForCreate,
  type MaterialCardDraft,
  type MaterialLookupRow,
  type WidthMode,
} from './materialCardDraft';
import { buildDuplicateFingerprint, fingerprintsEqual } from './materialNominalDims';

export const MATERIAL_CARD_TEMPLATE_COLUMNS = [
  'Name',
  'MainCategory',
  'MaterialType',
  'WoodToken',
  'IsThermowood',
  'ProductType',
  'Grade',
  'ThicknessMm',
  'WidthMode',
  'WidthMm',
  'WidthMinMm',
  'WidthMaxMm',
  'WidthOptions',
  'LengthMm',
  'StockUom',
  'CountUom',
  'LotTracking',
  'VolumeCalcRequired',
  'Status',
  'Notes',
  'ComplianceScope',
  'SupportedGradingMethods',
] as const;

export type TemplateColumn = (typeof MATERIAL_CARD_TEMPLATE_COLUMNS)[number];

const COLUMN_ALIASES: Record<string, TemplateColumn> = {
  name: 'Name',
  ad: 'Name',
  malzemeadi: 'Name',
  maincategory: 'MainCategory',
  urungrubu: 'MainCategory',
  kategori: 'MainCategory',
  materialtype: 'MaterialType',
  cins: 'MaterialType',
  malzemetipi: 'MaterialType',
  woodtoken: 'WoodToken',
  agac: 'WoodToken',
  agacturu: 'WoodToken',
  isthermowood: 'IsThermowood',
  thermowood: 'IsThermowood',
  producttype: 'ProductType',
  uruntipi: 'ProductType',
  grade: 'Grade',
  kalite: 'Grade',
  thicknessmm: 'ThicknessMm',
  kalinlik: 'ThicknessMm',
  nominalkalinlikmm: 'ThicknessMm',
  widthmode: 'WidthMode',
  genisliktipi: 'WidthMode',
  widthmm: 'WidthMm',
  genislik: 'WidthMm',
  widthminmm: 'WidthMinMm',
  genislikmin: 'WidthMinMm',
  widthmaxmm: 'WidthMaxMm',
  genislikmax: 'WidthMaxMm',
  widthoptions: 'WidthOptions',
  genisliksecenekleri: 'WidthOptions',
  lengthmm: 'LengthMm',
  uzunluk: 'LengthMm',
  stockuom: 'StockUom',
  anastokbirimi: 'StockUom',
  countuom: 'CountUom',
  sayimbirimi: 'CountUom',
  lottracking: 'LotTracking',
  lottakibi: 'LotTracking',
  volumecalcrequired: 'VolumeCalcRequired',
  hacimhesabi: 'VolumeCalcRequired',
  status: 'Status',
  durum: 'Status',
  notes: 'Notes',
  not: 'Notes',
  compliancescope: 'ComplianceScope',
  urunkapsami: 'ComplianceScope',
  supportedgradingmethods: 'SupportedGradingMethods',
  siniflandirmayontemleri: 'SupportedGradingMethods',
};

export type BulkRowResult =
  | { line: number; ok: true; draft: MaterialCardDraft; previewCode: string }
  | { line: number; ok: false; message: string; draft?: MaterialCardDraft };

function normHeader(raw: string): string {
  return String(raw || '')
    .normalize('NFKC')
    .replace(/^\uFEFF/, '')
    .trim()
    .toLowerCase()
    .replace(/[^a-z0-9ğüşöçıİ]+/gi, '')
    .replace(/ı/g, 'i')
    .replace(/ğ/g, 'g')
    .replace(/ü/g, 'u')
    .replace(/ş/g, 's')
    .replace(/ö/g, 'o')
    .replace(/ç/g, 'c');
}

export function resolveTemplateColumn(header: string): TemplateColumn | null {
  const n = normHeader(header);
  if (!n) return null;
  if ((MATERIAL_CARD_TEMPLATE_COLUMNS as readonly string[]).includes(header.trim())) {
    return header.trim() as TemplateColumn;
  }
  return COLUMN_ALIASES[n] ?? null;
}

export function parseBoolCell(raw: string | undefined, fallback: boolean): boolean {
  const s = String(raw ?? '')
    .trim()
    .toLowerCase()
    .replace(/ı/g, 'i')
    .replace(/ğ/g, 'g')
    .replace(/ü/g, 'u')
    .replace(/ş/g, 's')
    .replace(/ö/g, 'o')
    .replace(/ç/g, 'c');
  if (!s) return fallback;
  if (['1', 'true', 'yes', 'y', 'evet', 'e', 'x'].includes(s)) return true;
  if (['0', 'false', 'no', 'n', 'hayir', 'h'].includes(s)) return false;
  return fallback;
}

function parseMainCategory(raw: string): MainCategory | null {
  const s = String(raw || '').trim().toUpperCase();
  if (s === 'HM' || s === 'YM' || s === 'MP' || s === 'TW') return s;
  if (s === 'HAMMADDE') return 'HM';
  if (s.includes('YARI')) return 'YM';
  if (s.includes('MASIF') || s.includes('PANEL')) return 'MP';
  if (s.includes('THERMO')) return 'TW';
  return null;
}

function parseWidthMode(raw: string, draft: Partial<MaterialCardDraft>): WidthMode {
  const s = String(raw || '').trim().toLowerCase();
  if (s === 'range' || s.includes('aralik') || s.includes('aralık')) return 'range';
  if (s === 'options' || s.includes('secenek') || s.includes('çoklu') || s.includes('coklu')) {
    return 'options';
  }
  if (s === 'single' || s.includes('tek')) return 'single';
  if (draft.widthMinMm && draft.widthMaxMm) return 'range';
  if (draft.widthOptions) return 'options';
  return 'single';
}

export function parseGradingMethodsCell(raw: string | undefined): GradingMethod[] {
  const parts = String(raw ?? '')
    .toUpperCase()
    .split(/[|,;/+\s]+/)
    .map((p) => p.trim())
    .filter(Boolean);
  const out: GradingMethod[] = [];
  for (const p of parts) {
    if (p === 'VISUAL' || p === 'GORSEL' || p === 'GÖRSEL') out.push('VISUAL');
    else if (p === 'MACHINE' || p === 'MAKINE' || p === 'MAKİNE') out.push('MACHINE');
  }
  return [...new Set(out)];
}

export function rowToDraft(cells: Partial<Record<TemplateColumn, string>>): MaterialCardDraft {
  const mainCategory = parseMainCategory(cells.MainCategory ?? '') ?? 'HM';
  const isTw = mainCategory === 'TW' || parseBoolCell(cells.IsThermowood, false);
  const widthMm = String(cells.WidthMm ?? '').trim();
  const widthMinMm = String(cells.WidthMinMm ?? '').trim();
  const widthMaxMm = String(cells.WidthMaxMm ?? '').trim();
  const widthOptions = String(cells.WidthOptions ?? '').trim();
  const widthMode = parseWidthMode(cells.WidthMode ?? '', { widthMinMm, widthMaxMm, widthOptions });
  const stockDefault = mainCategory === 'MP' || mainCategory === 'TW' ? 'M2' : 'M3';
  const volumeDefault = mainCategory === 'HM' || mainCategory === 'YM';
  const scopeRaw = String(cells.ComplianceScope ?? '')
    .trim()
    .toUpperCase();
  const complianceScope: ComplianceScope =
    scopeRaw === 'STRUCTURAL_TIMBER' || scopeRaw.includes('YAPISAL')
      ? 'STRUCTURAL_TIMBER'
      : 'NORMAL_STOCK';
  const statusRaw = String(cells.Status ?? 'Active').trim().toLowerCase();
  const status: 'Active' | 'Passive' =
    statusRaw === 'passive' || statusRaw === 'pasif' ? 'Passive' : 'Active';

  const draft: MaterialCardDraft = {
    name: String(cells.Name ?? '').trim(),
    mainCategory,
    materialTypeToken: String(cells.MaterialType ?? '').trim().toUpperCase(),
    woodToken: String(cells.WoodToken ?? '').trim().toUpperCase(),
    isThermowood: isTw,
    productTypeToken: String(cells.ProductType ?? '').trim().toUpperCase(),
    grade: String(cells.Grade ?? '').trim().toUpperCase(),
    thicknessMm: String(cells.ThicknessMm ?? '').trim(),
    widthMode,
    widthMm,
    widthMinMm,
    widthMaxMm,
    widthOptions,
    lengthMm: String(cells.LengthMm ?? '').trim(),
    stockUom: String(cells.StockUom ?? stockDefault).trim().toUpperCase() || stockDefault,
    countUom: String(cells.CountUom ?? 'PCS').trim().toUpperCase() || 'PCS',
    lotTracking: parseBoolCell(cells.LotTracking, true),
    volumeCalcRequired: parseBoolCell(cells.VolumeCalcRequired, volumeDefault),
    status,
    notes: String(cells.Notes ?? '').trim(),
    complianceScope,
    supportedGradingMethods: parseGradingMethodsCell(cells.SupportedGradingMethods),
  };
  if (!draft.name) draft.name = suggestedNameForDraft(draft);
  return draft;
}

function splitCsvLine(line: string, sep: string): string[] {
  const out: string[] = [];
  let cur = '';
  let inQ = false;
  for (let i = 0; i < line.length; i += 1) {
    const ch = line[i];
    if (ch === '"') {
      if (inQ && line[i + 1] === '"') {
        cur += '"';
        i += 1;
      } else {
        inQ = !inQ;
      }
    } else if (ch === sep && !inQ) {
      out.push(cur);
      cur = '';
    } else {
      cur += ch;
    }
  }
  out.push(cur);
  return out;
}

function detectCsvSep(headerLine: string): string {
  const counts = {
    ';': (headerLine.match(/;/g) ?? []).length,
    ',': (headerLine.match(/,/g) ?? []).length,
    '\t': (headerLine.match(/\t/g) ?? []).length,
  };
  if (counts['\t'] >= counts[';'] && counts['\t'] >= counts[',']) return '\t';
  if (counts[';'] >= counts[',']) return ';';
  return ',';
}

export function parseDelimitedTable(text: string): string[][] {
  const raw = text.replace(/^\uFEFF/, '').replace(/\r\n/g, '\n').replace(/\r/g, '\n');
  const lines = raw.split('\n').filter((l) => l.trim().length > 0);
  if (lines.length === 0) return [];
  const sep = detectCsvSep(lines[0]);
  return lines.map((l) => splitCsvLine(l, sep).map((c) => c.trim()));
}

function xmlUnescape(s: string): string {
  return s
    .replace(/&lt;/g, '<')
    .replace(/&gt;/g, '>')
    .replace(/&quot;/g, '"')
    .replace(/&apos;/g, "'")
    .replace(/&amp;/g, '&');
}

/** SpreadsheetML 2003 (the format we download). */
export function parseSpreadsheetMl(xml: string): string[][] {
  const sheetMatch =
    xml.match(/<Worksheet[^>]*ss:Name="Kartlar"[\s\S]*?<\/Worksheet>/i) ??
    xml.match(/<Worksheet[\s\S]*?<\/Worksheet>/i);
  if (!sheetMatch) return [];
  const rows: string[][] = [];
  const rowRe = /<Row\b[^>]*>([\s\S]*?)<\/Row>/gi;
  let rowM: RegExpExecArray | null;
  while ((rowM = rowRe.exec(sheetMatch[0]))) {
    const cells: string[] = [];
    let col = 0;
    const cellRe = /<Cell\b([^>]*)>([\s\S]*?)<\/Cell>|<Cell\b([^>]*)\/>/gi;
    let cellM: RegExpExecArray | null;
    while ((cellM = cellRe.exec(rowM[1] ?? ''))) {
      const attrs = cellM[1] ?? cellM[3] ?? '';
      const idx = attrs.match(/ss:Index="(\d+)"/i);
      if (idx) col = Number(idx[1]) - 1;
      const data = (cellM[2] ?? '').match(/<Data\b[^>]*>([\s\S]*?)<\/Data>/i);
      cells[col] = data ? xmlUnescape(data[1].replace(/<[^>]+>/g, '')) : '';
      col += 1;
    }
    rows.push(cells.map((c) => c ?? ''));
  }
  return rows;
}

function xmlText(el: string): string {
  return xmlUnescape(el.replace(/<[^>]+>/g, ''));
}

export function parseXlsxSheetXml(sheetXml: string, sharedStrings: string[]): string[][] {
  const rows: string[][] = [];
  const rowRe = /<row\b[^>]*>([\s\S]*?)<\/row>/gi;
  let rowM: RegExpExecArray | null;
  while ((rowM = rowRe.exec(sheetXml))) {
    const cells: string[] = [];
    const cellRe = /<c\b([^>]*)>([\s\S]*?)<\/c>/gi;
    let cellM: RegExpExecArray | null;
    while ((cellM = cellRe.exec(rowM[1]))) {
      const attrs = cellM[1];
      const ref = attrs.match(/\br="([A-Z]+)(\d+)"/i);
      const colLetters = ref?.[1] ?? 'A';
      let col = 0;
      for (const ch of colLetters.toUpperCase()) col = col * 26 + (ch.charCodeAt(0) - 64);
      col -= 1;
      const t = attrs.match(/\bt="([^"]+)"/)?.[1];
      const v = cellM[2].match(/<v>([\s\S]*?)<\/v>/)?.[1] ?? '';
      const is = cellM[2].match(/<is>([\s\S]*?)<\/is>/)?.[1];
      let value = '';
      if (t === 's') value = sharedStrings[Number(v)] ?? '';
      else if (t === 'inlineStr' && is) value = xmlText(is);
      else value = xmlUnescape(v);
      cells[col] = value;
    }
    rows.push(cells.map((c) => c ?? ''));
  }
  return rows;
}

export function parseSharedStringsXml(xml: string): string[] {
  const out: string[] = [];
  const siRe = /<si>([\s\S]*?)<\/si>/gi;
  let m: RegExpExecArray | null;
  while ((m = siRe.exec(xml))) {
    const texts = [...m[1].matchAll(/<t[^>]*>([\s\S]*?)<\/t>/g)].map((x) => xmlUnescape(x[1]));
    out.push(texts.join(''));
  }
  return out;
}

async function inflateRaw(compressed: Uint8Array): Promise<Uint8Array> {
  if (typeof DecompressionStream === 'undefined') {
    throw new Error('Bu ortamda .xlsx açılamıyor — CSV veya şablon .xls kullanın.');
  }
  const stream = new Blob([compressed]).stream().pipeThrough(new DecompressionStream('deflate-raw'));
  const buf = await new Response(stream).arrayBuffer();
  return new Uint8Array(buf);
}

export async function unzipLocalFiles(buf: ArrayBuffer): Promise<Map<string, Uint8Array>> {
  const view = new DataView(buf);
  const u8 = new Uint8Array(buf);
  const files = new Map<string, Uint8Array>();
  let offset = 0;
  const td = new TextDecoder();
  while (offset + 30 <= u8.length) {
    if (view.getUint32(offset, true) !== 0x04034b50) break;
    const flags = view.getUint16(offset + 6, true);
    const method = view.getUint16(offset + 8, true);
    const compSize = view.getUint32(offset + 18, true);
    const nameLen = view.getUint16(offset + 26, true);
    const extraLen = view.getUint16(offset + 28, true);
    if (flags & 0x8) {
      throw new Error('xlsx veri tanımı desteklenmiyor — CSV olarak kaydedip yükleyin.');
    }
    const name = td.decode(u8.subarray(offset + 30, offset + 30 + nameLen));
    const start = offset + 30 + nameLen + extraLen;
    const compressed = u8.subarray(start, start + compSize);
    let data: Uint8Array;
    if (method === 0) data = compressed.slice();
    else if (method === 8) data = await inflateRaw(compressed);
    else throw new Error(`Desteklenmeyen zip yöntemi (${method}).`);
    files.set(name, data);
    offset = start + compSize;
  }
  return files;
}

export async function parseXlsxWorkbook(buf: ArrayBuffer): Promise<string[][]> {
  const sheets = await parseXlsxAllSheets(buf);
  return sheets.find((s) => /sayım|sayim|count/i.test(s.name) && !/^_/.test(s.name))?.rows
    ?? sheets.find((s) => !/^_/.test(s.name))?.rows
    ?? sheets[0]?.rows
    ?? [];
}

/** All worksheets (including hidden _MATERIALS). */
export async function parseXlsxAllSheets(buf: ArrayBuffer): Promise<{ name: string; rows: string[][] }[]> {
  const files = await unzipLocalFiles(buf);
  const decoder = new TextDecoder();
  const shared = files.get('xl/sharedStrings.xml');
  const strings = shared ? parseSharedStringsXml(decoder.decode(shared)) : [];
  const wb = files.get('xl/workbook.xml');
  const rels = files.get('xl/_rels/workbook.xml.rels');
  const idToTarget = new Map<string, string>();
  if (rels) {
    const relXml = decoder.decode(rels);
    const re = /<Relationship\b([^>]*)\/?>/gi;
    let m: RegExpExecArray | null;
    while ((m = re.exec(relXml))) {
      const id = m[1].match(/\bId="([^"]+)"/i)?.[1];
      const target = m[1].match(/\bTarget="([^"]+)"/i)?.[1];
      if (id && target) idToTarget.set(id, target.replace(/^\//, '').replace(/^xl\//, ''));
    }
  }
  const out: { name: string; rows: string[][] }[] = [];
  if (wb) {
    const wbXml = decoder.decode(wb);
    const sheetRe = /<sheet\b([^>]*)\/?>/gi;
    let sm: RegExpExecArray | null;
    while ((sm = sheetRe.exec(wbXml))) {
      const name = sm[1].match(/\bname="([^"]+)"/i)?.[1] ?? `Sheet${out.length + 1}`;
      const rid = sm[1].match(/\br:id="([^"]+)"/i)?.[1];
      const target = rid ? idToTarget.get(rid) : null;
      const path = target
        ? target.startsWith('xl/')
          ? target
          : `xl/${target.replace(/^\.\//, '')}`
        : `xl/worksheets/sheet${out.length + 1}.xml`;
      const bytes = files.get(path) ?? files.get(path.replace(/^xl\//, 'xl/worksheets/'));
      if (!bytes) continue;
      out.push({ name, rows: parseXlsxSheetXml(decoder.decode(bytes), strings) });
    }
  }
  if (out.length === 0) {
    const first = files.get('xl/worksheets/sheet1.xml');
    if (first) out.push({ name: 'Sayım', rows: parseXlsxSheetXml(decoder.decode(first), strings) });
  }
  return out;
}

export function tableToDrafts(
  table: string[][],
  existingCodes: string[],
  materials: MaterialLookupRow[],
): BulkRowResult[] {
  if (table.length < 2) return [];
  const headers = table[0].map((h) => resolveTemplateColumn(h));
  if (!headers.some((h) => h === 'MainCategory')) {
    throw new Error('Şablon başlığı bulunamadı. İlk satırda MainCategory (Ürün grubu) olmalı.');
  }
  const results: BulkRowResult[] = [];
  const codes = [...existingCodes];
  const accepted: MaterialCardDraft[] = [];
  for (let i = 1; i < table.length; i += 1) {
    const line = i + 1;
    const row = table[i];
    const cells: Partial<Record<TemplateColumn, string>> = {};
    headers.forEach((col, idx) => {
      if (col) cells[col] = row[idx] ?? '';
    });
    const empty = !String(cells.MainCategory ?? '').trim() && !String(cells.Name ?? '').trim();
    if (empty) continue;
    if (!String(cells.MainCategory ?? '').trim()) {
      results.push({ line, ok: false, message: 'MainCategory boş.' });
      continue;
    }
    const draft = rowToDraft(cells);
    const gate = validateDraftForCreate(draft, codes);
    if (!gate.ok) {
      results.push({ line, ok: false, message: gate.message, draft });
      continue;
    }
    const dup = findDraftDuplicate(draft, materials);
    const intra = accepted.find((prev) =>
      fingerprintsEqual(
        buildDuplicateFingerprint({
          mainCategory: prev.mainCategory,
          materialType: prev.materialTypeToken,
          woodSpecies: effectiveWoodToken(prev),
          productType: prev.productTypeToken,
          quality: prev.grade,
          dims: buildNominalFromDraft(prev),
        }),
        buildDuplicateFingerprint({
          mainCategory: draft.mainCategory,
          materialType: draft.materialTypeToken,
          woodSpecies: effectiveWoodToken(draft),
          productType: draft.productTypeToken,
          quality: draft.grade,
          dims: buildNominalFromDraft(draft),
        }),
      ),
    );
    if (dup || intra) {
      results.push({
        line,
        ok: false,
        message: `Aynı özelliklerde kart var (${dup?.code ?? dup?.name ?? 'dosyada tekrar'}). Satırı düzeltin veya tekil kart sihirbazından onaylayın.`,
        draft,
      });
      continue;
    }
    const previewCode = previewMaterialCode(codingInputFromDraft(draft, codes));
    const reserved = mintMaterialCode(codingInputFromDraft(draft, codes));
    if (reserved) codes.push(reserved);
    accepted.push(draft);
    results.push({ line, ok: true, draft, previewCode: reserved ?? previewCode });
  }
  return results;
}

export const EXAMPLE_TEMPLATE_ROWS: string[][] = [
  [...MATERIAL_CARD_TEMPLATE_COLUMNS],
  [
    'Çam Kereste',
    'HM',
    'KR',
    'PIN',
    'Hayır',
    '',
    '',
    '50',
    'single',
    '100',
    '',
    '',
    '',
    '4000',
    'M3',
    'PCS',
    'Evet',
    'Evet',
    'Active',
    '',
    'NORMAL_STOCK',
    '',
  ],
  [
    'Çam Kereste',
    'HM',
    'KR',
    'PIN',
    'Hayır',
    '',
    '',
    '50',
    'single',
    '150',
    '',
    '',
    '',
    '4000',
    'M3',
    'PCS',
    'Evet',
    'Evet',
    'Active',
    'İkinci ticari ölçü = ikinci kart',
    'NORMAL_STOCK',
    '',
  ],
  [
    'Çam Solid Lamel',
    'YM',
    'LM',
    'PIN',
    'Hayır',
    'S',
    '',
    '26',
    'single',
    '100',
    '',
    '',
    '',
    '3000',
    'M3',
    'PCS',
    'Evet',
    'Evet',
    'Active',
    '',
    'NORMAL_STOCK',
    '',
  ],
];

export function buildCsvTemplate(): string {
  const esc = (v: string) => {
    if (/[;"\n]/.test(v)) return `"${v.replace(/"/g, '""')}"`;
    return v;
  };
  return EXAMPLE_TEMPLATE_ROWS.map((r) => r.map(esc).join(';')).join('\r\n') + '\r\n';
}

function xmlEsc(v: string): string {
  return v
    .replace(/&/g, '&amp;')
    .replace(/</g, '&lt;')
    .replace(/>/g, '&gt;')
    .replace(/"/g, '&quot;');
}

function ssRow(cells: string[]): string {
  return `<Row>${cells
    .map((c) => `<Cell><Data ss:Type="String">${xmlEsc(c)}</Data></Cell>`)
    .join('')}</Row>`;
}

export function buildSpreadsheetMlTemplate(): string {
  const kodlar: string[][] = [
    ['Alan', 'İzinli değerler'],
    ['MainCategory', 'HM | YM | MP | TW'],
    ['MaterialType (HM)', HM_TYPE_OPTIONS.map((o) => `${o.token}=${o.label}`).join(' · ')],
    ['MaterialType (YM)', YM_TYPE_OPTIONS.map((o) => `${o.token}=${o.label}`).join(' · ')],
    ['WoodToken (HM/YM)', WOOD_OPTIONS_HM.map((o) => `${o.token}=${o.labelTr}`).join(' · ')],
    ['WoodToken (MP)', MP_WOOD_OPTIONS.map((o) => `${o.token}=${o.labelTr}`).join(' · ')],
    ['WoodToken (TW aile)', TW_FAMILY_OPTIONS.map((o) => `${o.token}=${o.label}`).join(' · ')],
    ['IsThermowood', 'Evet | Hayır  (PIN → kodda TPIN)'],
    ['ProductType', 'S | FJ  (YM lamel ve MP zorunlu)'],
    ['WidthMode', 'single | range | options'],
    ['StockUom / CountUom', 'M3 · M2 · PCS · KG · TON · PACKAGE'],
    ['Status', 'Active | Passive'],
    ['ComplianceScope', 'NORMAL_STOCK | STRUCTURAL_TIMBER'],
    ['SupportedGradingMethods', 'STRUCTURAL_TIMBER için VISUAL ve/veya MACHINE'],
    ['MaterialCode', 'YAZMAYIN — sistem üretir (ör. HM-KR-PIN-001)'],
  ];
  const talimat = [
    ['Naswood OS — Malzeme kartı toplu yükleme şablonu'],
    ['1. Kartlar sayfasını doldurun. Örnek satırları silin veya üzerine yazın.'],
    ['2. Her satır ayrı bir malzeme kartıdır. Aynı ağaç/cins + farklı ticari ölçü = yeni kart.'],
    ['3. MaterialCode sütunu yoktur — kod sistem tarafından üretilir.'],
    ['4. Nominal ölçü kart üzerindedir; mal kabuldeki gerçek ölçü bu kartı değiştirmez.'],
    ['5. Kaydederken Excel “XML Spreadsheet 2003” veya “CSV UTF-8 (ayırıcı: noktalı virgül)” kullanın.'],
    ['6. Designer’da “Excel ile yükle” deyip dosyayı seçin; geçerli satırlar sırayla oluşturulur.'],
  ].map((r) => ssRow(r));

  return `<?xml version="1.0" encoding="UTF-8"?>
<?mso-application progid="Excel.Sheet"?>
<Workbook xmlns="urn:schemas-microsoft-com:office:spreadsheet"
 xmlns:o="urn:schemas-microsoft-com:office:office"
 xmlns:x="urn:schemas-microsoft-com:office:excel"
 xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet">
  <Styles>
    <Style ss:ID="Default" ss:Name="Normal"><Font ss:FontName="Calibri" ss:Size="11"/></Style>
    <Style ss:ID="Header"><Font ss:FontName="Calibri" ss:Size="11" ss:Bold="1"/><Interior ss:Color="#FFF4E5" ss:Pattern="Solid"/></Style>
  </Styles>
  <Worksheet ss:Name="Kartlar">
    <Table>
      ${EXAMPLE_TEMPLATE_ROWS.map((r, i) =>
        i === 0
          ? `<Row>${r
              .map((c) => `<Cell ss:StyleID="Header"><Data ss:Type="String">${xmlEsc(c)}</Data></Cell>`)
              .join('')}</Row>`
          : ssRow(r),
      ).join('\n      ')}
    </Table>
  </Worksheet>
  <Worksheet ss:Name="Kodlar">
    <Table>
      ${kodlar.map((r, i) => (i === 0 ? `<Row>${r.map((c) => `<Cell ss:StyleID="Header"><Data ss:Type="String">${xmlEsc(c)}</Data></Cell>`).join('')}</Row>` : ssRow(r))).join('\n      ')}
    </Table>
  </Worksheet>
  <Worksheet ss:Name="Talimat">
    <Table>
      ${talimat.join('\n      ')}
    </Table>
  </Worksheet>
</Workbook>
`;
}

export function downloadTextFile(filename: string, content: string, mime: string) {
  const blob = new Blob([content], { type: mime });
  const url = URL.createObjectURL(blob);
  const a = document.createElement('a');
  a.href = url;
  a.download = filename;
  a.click();
  URL.revokeObjectURL(url);
}

export async function parseMaterialCardWorkbook(file: File): Promise<string[][]> {
  const name = file.name.toLowerCase();
  if (name.endsWith('.xlsx')) {
    return parseXlsxWorkbook(await file.arrayBuffer());
  }
  const text = await file.text();
  const trimmed = text.trimStart();
  if (trimmed.startsWith('<?xml') || trimmed.startsWith('<Workbook')) {
    return parseSpreadsheetMl(text);
  }
  return parseDelimitedTable(text);
}


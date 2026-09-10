import {
  parseDelimitedTable,
  parseSpreadsheetMl,
  parseXlsxWorkbook,
} from '../materials/materialCardBulk';
import { formatNominalDims, readNominalDims } from '../materials/materialNominalDims';
import type { MaterialCandidate } from '../receiving/materialMatch';

export type CountExcelMaterial = MaterialCandidate & {
  isActive?: boolean;
  materialType?: string | null;
};

export type CountTemplateSnapshotRow = {
  materialCode: string;
  materialName: string;
};

export type ExcelPreviewStatus = 'MATCHED' | 'REVIEW_REQUIRED' | 'INVALID';

export type CountExcelPreviewRow = {
  excelRow: number;
  materialLabel: string;
  materialId?: string;
  materialCode?: string;
  materialName?: string;
  thicknessMm?: number | null;
  widthMm?: number | null;
  lengthMm?: number | null;
  pieceCount?: number | null;
  quantity?: number | null;
  uom: string;
  measuredVolumeM3?: number | null;
  physicalGroupLabel?: string;
  note: string;
  status: ExcelPreviewStatus;
  error?: string;
  matchHint?: string;
};

export type CountExcelPreview = {
  total: number;
  matched: number;
  reviewRequired: number;
  invalid: number;
  ready: number;
  rows: CountExcelPreviewRow[];
};

type MaterialOption = {
  id: string;
  code: string;
  name: string;
  label: string;
  type: string;
  thicknessMm?: number | null;
  widthMm?: number | null;
  definitionJson?: string | null;
};

function fold(s: string): string {
  return String(s ?? '')
    .trim()
    .toLowerCase()
    .replace(/ı/g, 'i')
    .replace(/i̇/g, 'i')
    .replace(/ğ/g, 'g')
    .replace(/ü/g, 'u')
    .replace(/ş/g, 's')
    .replace(/ö/g, 'o')
    .replace(/ç/g, 'c');
}

function idx(header: string[], ...names: string[]): number {
  const n = names.map((x) => fold(x));
  return header.findIndex((h) => n.includes(fold(h)));
}

function cell(row: string[], i: number): string {
  if (i < 0 || i >= row.length) return '';
  return String(row[i] ?? '').trim();
}

function num(s: string): number | undefined {
  const t = s.replace(',', '.').replace(/[^\d.-]/g, '');
  if (!t) return undefined;
  const n = Number(t);
  return Number.isFinite(n) ? n : undefined;
}

function xmlEscape(s: string): string {
  return s.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;');
}

function normalizeLabel(s: string): string {
  return fold(s).replace(/\s+/g, ' ');
}

export function friendlyMaterialLabel(m: CountExcelMaterial, opts: MaterialOption[]): string {
  const name = (m.name || m.code || '').trim() || 'Malzeme';
  const sameName = opts.filter((o) => o.name.toLowerCase() === name.toLowerCase());
  const dims = formatNominalDims(readNominalDims({ definitionJson: m.definitionJson }));
  if (sameName.length > 1 && dims && dims !== '—') return `${name} — ${dims}`;
  return name;
}

export function buildMaterialOptions(cards: CountExcelMaterial[]): MaterialOption[] {
  const active = cards.filter((c) => c.isActive !== false && String(c.status ?? 'Active').toLowerCase() !== 'inactive');
  const draft: MaterialOption[] = active.map((c) => {
    const d = readNominalDims({ definitionJson: c.definitionJson });
    return {
      id: c.id,
      code: c.code,
      name: c.name || c.code,
      label: '',
      type: `${c.materialType ?? ''} ${c.category ?? ''}`.trim(),
      thicknessMm: d?.thicknessMm ?? null,
      widthMm: d?.widthMm ?? null,
      definitionJson: c.definitionJson,
    };
  });
  return draft.map((o) => {
    const card = active.find((c) => c.id === o.id)!;
    return { ...o, label: friendlyMaterialLabel(card, draft) };
  });
}

function parseAllSpreadsheetMl(xml: string): { name: string; rows: string[][] }[] {
  const sheets: { name: string; rows: string[][] }[] = [];
  const re = /<Worksheet\b([^>]*)>([\s\S]*?)<\/Worksheet>/gi;
  let m: RegExpExecArray | null;
  while ((m = re.exec(xml))) {
    const name = (m[1].match(/ss:Name="([^"]+)"/i)?.[1] ?? `Sheet${sheets.length + 1}`).trim();
    const inner = `<?xml version="1.0"?><Workbook xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet"><Worksheet ss:Name="${name}">${m[2]}</Worksheet></Workbook>`;
    sheets.push({ name, rows: parseSpreadsheetMl(inner) });
  }
  return sheets;
}

function parseHiddenMap(sheets: { name: string; rows: string[][] }[]): Map<string, { id: string; code: string }> {
  const map = new Map<string, { id: string; code: string }>();
  const sheet = sheets.find((s) => /naswoodmap|_map/i.test(s.name));
  if (!sheet || sheet.rows.length < 2) return map;
  const h = sheet.rows[0].map((x) => String(x).toLowerCase());
  const iLabel = idx(h, 'label', 'malzeme');
  const iId = idx(h, 'materialid');
  const iCode = idx(h, 'materialcode');
  for (const row of sheet.rows.slice(1)) {
    const label = cell(row, iLabel);
    if (!label) continue;
    map.set(normalizeLabel(label), { id: cell(row, iId), code: cell(row, iCode) });
  }
  return map;
}

function lumberNeedsDims(opt?: MaterialOption): boolean {
  const t = `${opt?.type ?? ''} ${opt?.definitionJson ?? ''}`.toUpperCase();
  return (
    t.includes('LUMBER') ||
    t.includes('KERESTE') ||
    t.includes('"MATERIALTYPETOKEN":"KR"') ||
    t.includes('"MATERIALTYPE":"KR"') ||
    /\bKR\b/.test(opt?.type ?? '')
  );
}

function matchMaterial(
  label: string,
  hidden: { id?: string; code?: string } | undefined,
  explicitCode: string,
  explicitId: string,
  opts: MaterialOption[],
  actual: { t?: number; w?: number },
): { status: ExcelPreviewStatus; opt?: MaterialOption; hint?: string } {
  if (explicitId) {
    const hit = opts.find((o) => o.id.toLowerCase() === explicitId.toLowerCase());
    if (hit) return { status: 'MATCHED', opt: hit, hint: 'MaterialId' };
  }
  if (hidden?.id) {
    const hit = opts.find((o) => o.id.toLowerCase() === hidden.id!.toLowerCase());
    if (hit) return { status: 'MATCHED', opt: hit, hint: 'template-map' };
  }
  const code = explicitCode || hidden?.code || '';
  if (code) {
    const hit = opts.find((o) => o.code.toLowerCase() === code.toLowerCase());
    if (hit) return { status: 'MATCHED', opt: hit, hint: 'MaterialCode' };
  }
  const n = normalizeLabel(label);
  if (!n) return { status: 'INVALID', hint: 'Malzeme boş' };

  const byName = opts.filter((o) => normalizeLabel(o.name) === n || normalizeLabel(o.label) === n);
  if (byName.length === 1) return { status: 'MATCHED', opt: byName[0], hint: 'MaterialName' };
  if (byName.length > 1) {
    const types = new Set(byName.map((o) => o.type.trim().toLowerCase()));
    if (types.size === 1 && byName.length === 1) {
      return { status: 'MATCHED', opt: byName[0], hint: 'name+type' };
    }
    if (actual.t != null && actual.w != null) {
      const dimHits = byName.filter((o) => o.thicknessMm === actual.t && o.widthMm === actual.w);
      if (dimHits.length === 1) return { status: 'MATCHED', opt: dimHits[0], hint: 'name+nominal' };
    }
    return { status: 'REVIEW_REQUIRED', hint: 'Aynı isimde birden fazla kart' };
  }
  return { status: 'REVIEW_REQUIRED', hint: 'Kart seçilmeli' };
}

function finishRow(r: CountExcelPreviewRow, opt?: MaterialOption): CountExcelPreviewRow {
  if (r.status === 'MATCHED' && lumberNeedsDims(opt) && (r.thicknessMm == null || r.widthMm == null || r.lengthMm == null)) {
    return { ...r, status: 'INVALID', error: 'Kereste için kalınlık, genişlik ve uzunluk gerekli' };
  }
  return r;
}

export function parseCountTable(
  table: string[][],
  cards: CountExcelMaterial[],
  hiddenMap?: Map<string, { id: string; code: string }>,
): CountExcelPreview {
  const opts = buildMaterialOptions(cards);
  const hidden = hiddenMap ?? new Map<string, { id: string; code: string }>();
  if (!table || table.length < 2) {
    return { total: 0, matched: 0, reviewRequired: 0, invalid: 0, ready: 0, rows: [] };
  }

  const h = table[0];
  const iLabel = idx(h, 'malzeme', 'material', 'materialname', 'malzeme adı', 'malzeme adi');
  const iCode = idx(h, 'materialcode', 'malzemekodu');
  const iId = idx(h, 'materialid');
  const iT = idx(h, 'kalınlık mm', 'kalinlik mm', 'thicknessmm', 'thickness', 'kalınlık', 'kalinlik');
  const iW = idx(h, 'genişlik mm', 'genislik mm', 'widthmm', 'width', 'genişlik', 'genislik');
  const iL = idx(h, 'uzunluk mm', 'lengthmm', 'length', 'uzunluk');
  const iPcs = idx(h, 'adet', 'piececount', 'pcs', 'parça', 'parca');
  const iQty = idx(h, 'miktar', 'quantity', 'qty');
  const iUom = idx(h, 'birim', 'uom');
  const iVol = idx(h, 'ölçülen hacim', 'olculen hacim', 'measuredvolumem3', 'hacim');
  const iGroup = idx(
    h,
    'paket / istif etiketi',
    'paket/istif etiketi',
    'physicalgrouplabel',
    'istif',
    'paket',
    'packageno',
    'packagenumber',
  );
  const iNote = idx(h, 'not', 'note', 'notes');

  const rows: CountExcelPreviewRow[] = [];
  table.slice(1).forEach((row, i) => {
    const label = cell(row, iLabel) || (iCode >= 0 ? cell(row, iCode) : '');
    const code = iCode >= 0 ? cell(row, iCode) : '';
    const id = iId >= 0 ? cell(row, iId) : '';
    const t = num(cell(row, iT));
    const w = num(cell(row, iW));
    const l = num(cell(row, iL));
    const pcs = num(cell(row, iPcs));
    const qtyRaw = num(cell(row, iQty));
    const vol = num(cell(row, iVol));
    const group = cell(row, iGroup);
    const note = cell(row, iNote);
    const uom = cell(row, iUom) || (pcs != null ? 'PCS' : 'M3');
    if (!label && !code && t == null && w == null && pcs == null && qtyRaw == null && vol == null) return;

    const mapHit = hidden.get(normalizeLabel(label));
    const m = matchMaterial(label, mapHit, code, id, opts, { t, w });
    let status = m.status;
    let error: string | undefined;
    if (m.hint === 'Malzeme boş') error = 'Malzeme boş';
    const qty = pcs ?? qtyRaw;
    if (status !== 'INVALID' && (qty == null || qty <= 0) && vol == null) {
      status = 'INVALID';
      error = 'Adet veya miktar gerekli';
    }

    rows.push(
      finishRow(
        {
          excelRow: i + 2,
          materialLabel: label,
          materialId: m.opt?.id,
          materialCode: m.opt?.code,
          materialName: m.opt?.name,
          thicknessMm: t ?? null,
          widthMm: w ?? null,
          lengthMm: l ?? null,
          pieceCount: pcs ?? null,
          quantity: qty ?? vol ?? null,
          uom,
          measuredVolumeM3: vol ?? null,
          physicalGroupLabel: group || undefined,
          note,
          status,
          error,
          matchHint: m.hint,
        },
        m.opt,
      ),
    );
  });

  return summarize(rows);
}

function summarize(rows: CountExcelPreviewRow[]): CountExcelPreview {
  const matched = rows.filter((r) => r.status === 'MATCHED').length;
  const reviewRequired = rows.filter((r) => r.status === 'REVIEW_REQUIRED').length;
  const invalid = rows.filter((r) => r.status === 'INVALID').length;
  return { total: rows.length, matched, reviewRequired, invalid, ready: matched, rows };
}

/** Apply one Excel label → Material Master mapping to all matching REVIEW rows (or a single row). */
export function applyLabelMapping(
  preview: CountExcelPreview,
  excelLabel: string,
  card: CountExcelMaterial,
  onlyExcelRow?: number,
): CountExcelPreview {
  const n = normalizeLabel(excelLabel);
  const opt = buildMaterialOptions([card])[0];
  const rows = preview.rows.map((r) => {
    if (r.status === 'INVALID') return r;
    if (onlyExcelRow != null && r.excelRow !== onlyExcelRow) return r;
    if (normalizeLabel(r.materialLabel) !== n) return r;
    if (onlyExcelRow == null && r.status === 'MATCHED') return r;
    return finishRow(
      {
        ...r,
        materialId: card.id,
        materialCode: card.code,
        materialName: card.name,
        status: 'MATCHED',
        error: undefined,
        matchHint: onlyExcelRow != null ? 'satır seçimi' : 'toplu eşleştirme',
      },
      opt,
    );
  });
  return summarize(rows);
}

export function uniqueReviewLabels(preview: CountExcelPreview): string[] {
  const seen = new Set<string>();
  const out: string[] = [];
  for (const r of preview.rows) {
    if (r.status !== 'REVIEW_REQUIRED') continue;
    const k = normalizeLabel(r.materialLabel);
    if (seen.has(k)) continue;
    seen.add(k);
    out.push(r.materialLabel);
  }
  return out;
}

export async function parseCountWorkbook(file: File, cards: CountExcelMaterial[]): Promise<CountExcelPreview> {
  const name = file.name.toLowerCase();
  let sheets: { name: string; rows: string[][] }[] = [];
  if (name.endsWith('.xlsx')) {
    sheets = [{ name: 'Sayım', rows: await parseXlsxWorkbook(await file.arrayBuffer()) }];
  } else {
    const text = await file.text();
    const trimmed = text.trimStart();
    if (trimmed.startsWith('<?xml') || trimmed.includes('<Workbook')) {
      sheets = parseAllSpreadsheetMl(text);
    } else {
      sheets = [{ name: 'Sayım', rows: parseDelimitedTable(text) }];
    }
  }
  const hidden = parseHiddenMap(sheets);
  const data =
    sheets.find((s) => /sayım|sayim|count/i.test(s.name) && !/_naswood/i.test(s.name)) ??
    sheets.find((s) => !/^_/.test(s.name)) ??
    sheets[0];
  return parseCountTable(data?.rows ?? [], cards, hidden);
}

export function buildFieldCountCsv(cards: CountExcelMaterial[], snapshot: CountTemplateSnapshotRow[]): string {
  const opts = buildMaterialOptions(cards);
  const byCode = new Map(opts.map((o) => [o.code.toLowerCase(), o]));
  const header = 'Malzeme;Kalınlık mm;Genişlik mm;Uzunluk mm;Adet;Paket / İstif Etiketi;Ölçülen Hacim;Not';
  const lines =
    snapshot.length > 0
      ? snapshot.map((s) => {
          const o = byCode.get(s.materialCode.toLowerCase());
          return `${o?.label || s.materialName};;;;;;;`;
        })
      : ['Çam Kereste;45;90;4000;120;İstif A;;'];
  return '\uFEFF' + [header, ...lines].join('\n');
}

export function downloadFieldCountTemplate(cards: CountExcelMaterial[], snapshot: CountTemplateSnapshotRow[]) {
  const opts = buildMaterialOptions(cards);
  const labels = opts.map((o) => o.label);
  const listRows = labels.map((l) => `<Row><Cell><Data ss:Type="String">${xmlEscape(l)}</Data></Cell></Row>`).join('');
  const mapRows = opts
    .map(
      (o) =>
        `<Row><Cell><Data ss:Type="String">${xmlEscape(o.label)}</Data></Cell><Cell><Data ss:Type="String">${xmlEscape(o.id)}</Data></Cell><Cell><Data ss:Type="String">${xmlEscape(o.code)}</Data></Cell></Row>`,
    )
    .join('');

  const byCode = new Map(opts.map((o) => [o.code.toLowerCase(), o]));
  const body =
    snapshot.length > 0
      ? snapshot
          .map((s) => {
            const o = byCode.get(s.materialCode.toLowerCase());
            const label = o?.label || s.materialName || s.materialCode;
            return `<Row>
<Cell><Data ss:Type="String">${xmlEscape(label)}</Data></Cell>
<Cell></Cell>
<Cell></Cell>
<Cell></Cell>
<Cell></Cell>
<Cell></Cell>
<Cell></Cell>
<Cell></Cell>
</Row>`;
          })
          .join('')
      : `<Row>
<Cell><Data ss:Type="String">${xmlEscape(labels[0] || 'Çam Kereste')}</Data></Cell>
<Cell><Data ss:Type="Number">45</Data></Cell>
<Cell><Data ss:Type="Number">90</Data></Cell>
<Cell><Data ss:Type="Number">4000</Data></Cell>
<Cell><Data ss:Type="Number">120</Data></Cell>
<Cell><Data ss:Type="String">İstif A</Data></Cell>
<Cell></Cell>
<Cell></Cell>
</Row>`;

  const lastList = Math.max(1, labels.length);
  const xml = `<?xml version="1.0"?>
<?mso-application progid="Excel.Sheet"?>
<Workbook xmlns="urn:schemas-microsoft-com:office:spreadsheet"
 xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet"
 xmlns:x="urn:schemas-microsoft-com:office:excel">
<Names>
<NamedRange ss:Name="MaterialList" ss:RefersTo="=_NaswoodList!R1C1:R${lastList}C1"/>
</Names>
<Worksheet ss:Name="Sayım">
<Table>
<Row>
<Cell><Data ss:Type="String">Malzeme</Data></Cell>
<Cell><Data ss:Type="String">Kalınlık mm</Data></Cell>
<Cell><Data ss:Type="String">Genişlik mm</Data></Cell>
<Cell><Data ss:Type="String">Uzunluk mm</Data></Cell>
<Cell><Data ss:Type="String">Adet</Data></Cell>
<Cell><Data ss:Type="String">Paket / İstif Etiketi</Data></Cell>
<Cell><Data ss:Type="String">Ölçülen Hacim</Data></Cell>
<Cell><Data ss:Type="String">Not</Data></Cell>
</Row>
${body}
</Table>
<DataValidation xmlns="urn:schemas-microsoft-com:office:excel">
<Range>R2C1:R501C1</Range>
<Type>List</Type>
<Value>MaterialList</Value>
</DataValidation>
</Worksheet>
<Worksheet ss:Name="_NaswoodList" ss:Visible="ss:Hidden">
<Table>${listRows || '<Row><Cell><Data ss:Type="String"></Data></Cell></Row>'}</Table>
</Worksheet>
<Worksheet ss:Name="_NaswoodMap" ss:Visible="ss:Hidden">
<Table>
<Row>
<Cell><Data ss:Type="String">Label</Data></Cell>
<Cell><Data ss:Type="String">MaterialId</Data></Cell>
<Cell><Data ss:Type="String">MaterialCode</Data></Cell>
</Row>
${mapRows}
</Table>
</Worksheet>
</Workbook>`;

  const blob = new Blob([xml], { type: 'application/vnd.ms-excel' });
  const a = document.createElement('a');
  a.href = URL.createObjectURL(blob);
  a.download = 'stok-sayim-saha.xls';
  a.click();
  URL.revokeObjectURL(a.href);
}

export function downloadFieldCountCsv(cards: CountExcelMaterial[], snapshot: CountTemplateSnapshotRow[]) {
  const csv = buildFieldCountCsv(cards, snapshot);
  const blob = new Blob([csv], { type: 'text/csv;charset=utf-8' });
  const a = document.createElement('a');
  a.href = URL.createObjectURL(blob);
  a.download = 'stok-sayim-saha.csv';
  a.click();
  URL.revokeObjectURL(a.href);
}

export function buildCountTemplateCsv(snapshot: CountTemplateSnapshotRow[]): string {
  return buildFieldCountCsv([], snapshot);
}

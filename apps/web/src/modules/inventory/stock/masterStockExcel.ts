/** Read-only master stock Excel — not the cycle-count field template. */

const CRC_TABLE = (() => {
  const t = new Uint32Array(256);
  for (let n = 0; n < 256; n += 1) {
    let c = n;
    for (let k = 0; k < 8; k += 1) c = c & 1 ? 0xedb88320 ^ (c >>> 1) : c >>> 1;
    t[n] = c >>> 0;
  }
  return t;
})();

function crc32(data: Uint8Array): number {
  let c = 0xffffffff;
  for (let i = 0; i < data.length; i += 1) c = CRC_TABLE[(c ^ data[i]) & 0xff] ^ (c >>> 8);
  return (c ^ 0xffffffff) >>> 0;
}

function concat(parts: Uint8Array[]): Uint8Array {
  const len = parts.reduce((n, p) => n + p.length, 0);
  const out = new Uint8Array(len);
  let o = 0;
  for (const p of parts) {
    out.set(p, o);
    o += p.length;
  }
  return out;
}

function zipStore(files: { name: string; data: Uint8Array }[]): Uint8Array {
  const enc = new TextEncoder();
  const locals: Uint8Array[] = [];
  const centrals: Uint8Array[] = [];
  let offset = 0;
  for (const f of files) {
    const name = enc.encode(f.name.replace(/\\/g, '/'));
    const crc = crc32(f.data);
    const local = new Uint8Array(30 + name.length + f.data.length);
    const v = new DataView(local.buffer);
    v.setUint32(0, 0x04034b50, true);
    v.setUint16(4, 20, true);
    v.setUint16(8, 0, true);
    v.setUint32(14, crc, true);
    v.setUint32(18, f.data.length, true);
    v.setUint32(22, f.data.length, true);
    v.setUint16(26, name.length, true);
    local.set(name, 30);
    local.set(f.data, 30 + name.length);
    locals.push(local);

    const central = new Uint8Array(46 + name.length);
    const cv = new DataView(central.buffer);
    cv.setUint32(0, 0x02014b50, true);
    cv.setUint16(4, 20, true);
    cv.setUint16(6, 20, true);
    cv.setUint32(16, crc, true);
    cv.setUint32(20, f.data.length, true);
    cv.setUint32(24, f.data.length, true);
    cv.setUint16(28, name.length, true);
    cv.setUint32(42, offset, true);
    central.set(name, 46);
    centrals.push(central);
    offset += local.length;
  }
  const cd = concat(centrals);
  const eocd = new Uint8Array(22);
  const ev = new DataView(eocd.buffer);
  ev.setUint32(0, 0x06054b50, true);
  ev.setUint16(8, files.length, true);
  ev.setUint16(10, files.length, true);
  ev.setUint32(12, cd.length, true);
  ev.setUint32(16, offset, true);
  return concat([...locals, cd, eocd]);
}

function xmlEsc(s: string): string {
  return s.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;');
}

function colLetter(i: number): string {
  let n = i + 1;
  let s = '';
  while (n > 0) {
    const r = (n - 1) % 26;
    s = String.fromCharCode(65 + r) + s;
    n = Math.floor((n - 1) / 26);
  }
  return s;
}

function inlineCell(r: number, c: number, value: string): string {
  const ref = `${colLetter(c)}${r}`;
  if (value !== '' && Number.isFinite(Number(value)) && /^-?\d+(?:\.\d+)?$/.test(value)) {
    return `<c r="${ref}"><v>${xmlEsc(value)}</v></c>`;
  }
  return `<c r="${ref}" t="inlineStr"><is><t xml:space="preserve">${xmlEsc(value)}</t></is></c>`;
}

function sheetXml(rows: string[][], selected = false): string {
  const maxCols = Math.max(1, ...rows.map((r) => r.length));
  const last = `${colLetter(maxCols - 1)}${Math.max(1, rows.length)}`;
  const sheetData = rows
    .map((row, i) => {
      const r = i + 1;
      const cells = row.map((val, c) => inlineCell(r, c, val ?? '')).join('');
      return `<row r="${r}">${cells}</row>`;
    })
    .join('');
  const view = selected
    ? `<sheetViews><sheetView tabSelected="1" workbookViewId="0"><pane ySplit="1" topLeftCell="A2" activePane="bottomLeft" state="frozen"/></sheetView></sheetViews>`
    : `<sheetViews><sheetView workbookViewId="0"/></sheetViews>`;
  return `<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<worksheet xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main"
 xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships">
<dimension ref="A1:${last}"/>
${view}
<sheetData>${sheetData}</sheetData>
</worksheet>`;
}

export const MASTER_STOCK_SHEETS = ['GUNCEL_STOK', 'PAKET_LISTESI', 'RAPOR_BILGISI'] as const;

export type MasterStockExportRow = {
  materialCode?: string;
  materialName?: string;
  woodSpecies?: string;
  actualThicknessMm?: number | null;
  actualWidthMm?: number | null;
  actualLengthMm?: number | null;
  lot?: string;
  factory?: string;
  warehouse?: string;
  location?: string;
  pieceCount?: number | null;
  packageCount?: number;
  stockUnit?: string;
  stockQuantity?: number;
  stockStatus?: string;
};

export type MasterStockExportPackage = {
  packageNo?: string;
  physicalGroupLabel?: string;
  materialCode?: string;
  materialName?: string;
  actualThicknessMm?: number | null;
  actualWidthMm?: number | null;
  actualLengthMm?: number | null;
  lot?: string;
  factory?: string;
  warehouse?: string;
  location?: string;
  pieceCount?: number | null;
  stockUnit?: string;
  stockQuantity?: number;
  status?: string;
};

export type MasterStockReportInfo = {
  reportDate?: string;
  reportTime?: string;
  timeZone?: string;
  factory?: string;
  warehouseFilter?: string;
  locationFilter?: string;
  stockStatusFilter?: string;
  materialFilter?: string;
  lotFilter?: string;
  preparedBy?: string;
  totalStockRows?: number;
  totalPackages?: number;
  generatedAt?: string;
  scopeNote?: string;
};

function cell(v: unknown): string {
  if (v == null || v === '') return '';
  return String(v);
}

export function masterStockFileName(generatedAt: Date, timeZone = 'Europe/Istanbul'): string {
  const fmt = new Intl.DateTimeFormat('en-CA', {
    timeZone,
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
    hourCycle: 'h23',
  });
  const parts = Object.fromEntries(fmt.formatToParts(generatedAt).map((p) => [p.type, p.value]));
  const date = `${parts.year}-${parts.month}-${parts.day}`;
  const hm = `${parts.hour}${parts.minute}`;
  return `NASWOOD_Stok_Listesi_${date}_${hm}.xlsx`;
}

export function buildMasterStockXlsxBytes(opts: {
  stockRows: MasterStockExportRow[];
  packageRows: MasterStockExportPackage[];
  report: MasterStockReportInfo;
}): Uint8Array {
  const stockHeaders = [
    'MaterialCode',
    'MaterialName',
    'WoodSpecies',
    'ActualThicknessMm',
    'ActualWidthMm',
    'ActualLengthMm',
    'Lot',
    'Factory',
    'Warehouse',
    'Location',
    'PieceCount',
    'PackageCount',
    'StockUnit',
    'StockQuantity',
    'StockStatus',
  ];
  const pkgHeaders = [
    'PackageNo',
    'PhysicalGroupLabel',
    'MaterialCode',
    'MaterialName',
    'ActualThicknessMm',
    'ActualWidthMm',
    'ActualLengthMm',
    'Lot',
    'Factory',
    'Warehouse',
    'Location',
    'PieceCount',
    'StockUnit',
    'StockQuantity',
    'Status',
  ];

  const stock = [
    stockHeaders,
    ...opts.stockRows.map((r) => [
      cell(r.materialCode),
      cell(r.materialName),
      cell(r.woodSpecies),
      cell(r.actualThicknessMm),
      cell(r.actualWidthMm),
      cell(r.actualLengthMm),
      cell(r.lot),
      cell(r.factory),
      cell(r.warehouse),
      cell(r.location),
      cell(r.pieceCount),
      cell(r.packageCount),
      cell(r.stockUnit),
      cell(r.stockQuantity),
      cell(r.stockStatus),
    ]),
  ];
  const pkgs = [
    pkgHeaders,
    ...opts.packageRows.map((r) => [
      cell(r.packageNo),
      cell(r.physicalGroupLabel),
      cell(r.materialCode),
      cell(r.materialName),
      cell(r.actualThicknessMm),
      cell(r.actualWidthMm),
      cell(r.actualLengthMm),
      cell(r.lot),
      cell(r.factory),
      cell(r.warehouse),
      cell(r.location),
      cell(r.pieceCount),
      cell(r.stockUnit),
      cell(r.stockQuantity),
      cell(r.status),
    ]),
  ];
  const report = [
    ['Alan', 'Değer'],
    ['Rapor Tarihi', cell(opts.report.reportDate)],
    ['Rapor Saati', cell(opts.report.reportTime)],
    ['Saat Dilimi', cell(opts.report.timeZone)],
    ['Fabrika', cell(opts.report.factory)],
    ['Depo Filtresi', cell(opts.report.warehouseFilter)],
    ['Lokasyon Filtresi', cell(opts.report.locationFilter)],
    ['Stok Durumu Filtresi', cell(opts.report.stockStatusFilter)],
    ['Material Filtresi', cell(opts.report.materialFilter)],
    ['Lot Filtresi', cell(opts.report.lotFilter)],
    ['Hazırlayan Kullanıcı', cell(opts.report.preparedBy)],
    ['Toplam Stok Satırı', cell(opts.report.totalStockRows)],
    ['Toplam Paket', cell(opts.report.totalPackages)],
    ['Rapor Zamanı (UTC)', cell(opts.report.generatedAt)],
    ['Kapsam', cell(opts.report.scopeNote)],
  ];

  const utf8 = (s: string) => new TextEncoder().encode(s);
  const contentTypes = `<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<Types xmlns="http://schemas.openxmlformats.org/package/2006/content-types">
<Default Extension="rels" ContentType="application/vnd.openxmlformats-package.relationships+xml"/>
<Default Extension="xml" ContentType="application/xml"/>
<Override PartName="/xl/workbook.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml"/>
<Override PartName="/xl/worksheets/sheet1.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml"/>
<Override PartName="/xl/worksheets/sheet2.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml"/>
<Override PartName="/xl/worksheets/sheet3.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml"/>
<Override PartName="/xl/styles.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.styles+xml"/>
</Types>`;
  const rels = `<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">
<Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument" Target="xl/workbook.xml"/>
</Relationships>`;
  const wbRels = `<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<Relationships xmlns="http://schemas.openxmlformats.org/officeDocument/2006/relationships">
<Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet" Target="worksheets/sheet1.xml"/>
<Relationship Id="rId2" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet" Target="worksheets/sheet2.xml"/>
<Relationship Id="rId3" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet" Target="worksheets/sheet3.xml"/>
<Relationship Id="rId4" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles" Target="styles.xml"/>
</Relationships>`;
  const workbook = `<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<workbook xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main"
 xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships">
<sheets>
<sheet name="GUNCEL_STOK" sheetId="1" r:id="rId1"/>
<sheet name="PAKET_LISTESI" sheetId="2" r:id="rId2"/>
<sheet name="RAPOR_BILGISI" sheetId="3" r:id="rId3"/>
</sheets>
</workbook>`;
  const styles = `<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<styleSheet xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main">
<fonts count="1"><font><sz val="11"/><name val="Calibri"/></font></fonts>
<fills count="1"><fill><patternFill patternType="none"/></fill></fills>
<borders count="1"><border/></borders>
<cellStyleXfs count="1"><xf/></cellStyleXfs>
<cellXfs count="1"><xf/></cellXfs>
</styleSheet>`;

  return zipStore([
    { name: '[Content_Types].xml', data: utf8(contentTypes) },
    { name: '_rels/.rels', data: utf8(rels) },
    { name: 'xl/workbook.xml', data: utf8(workbook) },
    { name: 'xl/_rels/workbook.xml.rels', data: utf8(wbRels) },
    { name: 'xl/styles.xml', data: utf8(styles) },
    { name: 'xl/worksheets/sheet1.xml', data: utf8(sheetXml(stock, true)) },
    { name: 'xl/worksheets/sheet2.xml', data: utf8(sheetXml(pkgs)) },
    { name: 'xl/worksheets/sheet3.xml', data: utf8(sheetXml(report)) },
  ]);
}

export function downloadMasterStockXlsx(bytes: Uint8Array, fileName: string) {
  const blob = new Blob([bytes], {
    type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
  });
  const url = URL.createObjectURL(blob);
  const a = document.createElement('a');
  a.href = url;
  a.download = fileName;
  a.click();
  URL.revokeObjectURL(url);
}

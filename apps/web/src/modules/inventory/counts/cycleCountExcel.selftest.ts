/**
 * Run: npx tsx apps/web/src/modules/inventory/counts/cycleCountExcel.selftest.ts
 */
import {
  applyLabelMapping,
  buildFieldCountCsv,
  parseCountTable,
  uniqueReviewLabels,
  type CountExcelMaterial,
} from './cycleCountExcel';

function assert(cond: unknown, msg: string): void {
  if (!cond) throw new Error(msg);
}

const pine: CountExcelMaterial = {
  id: '11111111-1111-1111-1111-111111111111',
  code: 'HM-KR-PIN-001',
  name: 'Çam Kereste',
  category: 'HM',
  materialType: 'KR',
  status: 'Active',
  definitionJson: JSON.stringify({
    NominalThicknessMm: 50,
    NominalWidthMm: 100,
    stockUom: 'M3',
    materialType: 'KR',
  }),
};

const pineWide: CountExcelMaterial = {
  ...pine,
  id: '22222222-2222-2222-2222-222222222222',
  code: 'HM-KR-PIN-002',
  definitionJson: JSON.stringify({
    NominalThicknessMm: 50,
    NominalWidthMm: 150,
    stockUom: 'M3',
    materialType: 'KR',
  }),
};

const spruce: CountExcelMaterial = {
  id: '33333333-3333-3333-3333-333333333333',
  code: 'HM-KR-SPR-001',
  name: 'Ladin Kereste',
  category: 'HM',
  materialType: 'KR',
  status: 'Active',
  definitionJson: JSON.stringify({ NominalThicknessMm: 50, NominalWidthMm: 150, materialType: 'KR' }),
};

// TEST 1 — name-only field sheet, no MaterialCode / PackageNo / LotNo
const t1 = parseCountTable(
  [
    ['Malzeme', 'Kalınlık mm', 'Genişlik mm', 'Uzunluk mm', 'Adet'],
    ['Çam Kereste', '45', '90', '4000', '120'],
  ],
  [pine],
);
assert(t1.total === 1, 'TEST1 parsed');
assert(t1.invalid === 0, 'TEST1 not invalid');
assert(t1.rows[0]?.status === 'MATCHED' || t1.rows[0]?.status === 'REVIEW_REQUIRED', 'TEST1 match or review');
assert(t1.rows[0]?.materialCode === 'HM-KR-PIN-001', 'TEST1 resolved master');
assert(t1.rows[0]?.thicknessMm === 45 && t1.rows[0]?.widthMm === 90 && t1.rows[0]?.lengthMm === 4000, 'TEST1 actual dims');
assert(!t1.rows[0]?.physicalGroupLabel, 'TEST1 no package required');

// TEST 2 — 250 rows, 3 unmatched names; rest kept
const header = ['Malzeme', 'Kalınlık mm', 'Genişlik mm', 'Uzunluk mm', 'Adet'];
const t2rows: string[][] = [header];
for (let i = 0; i < 247; i++) t2rows.push(['Çam Kereste', '45', '90', '4000', '1']);
t2rows.push(['Bilinmeyen A', '45', '90', '4000', '1']);
t2rows.push(['Bilinmeyen B', '45', '90', '4000', '1']);
t2rows.push(['Bilinmeyen C', '45', '90', '4000', '1']);
const t2 = parseCountTable(t2rows, [pine]);
assert(t2.total === 250, 'TEST2 all rows kept');
assert(t2.matched === 247, 'TEST2 247 matched');
assert(t2.reviewRequired === 3, 'TEST2 3 review');
assert(t2.invalid === 0, 'TEST2 none invalid');

// TEST 3 — no PackageNo is valid
assert(t1.rows[0]?.status === 'MATCHED', 'TEST3 valid without PackageNo');

// TEST 4 — İstif A is PhysicalGroupLabel, not PackageNo
const t4 = parseCountTable(
  [
    ['Malzeme', 'Kalınlık mm', 'Genişlik mm', 'Uzunluk mm', 'Adet', 'Paket / İstif Etiketi'],
    ['Çam Kereste', '45', '90', '4000', '120', 'İstif A'],
  ],
  [pine],
);
assert(t4.rows[0]?.physicalGroupLabel === 'İstif A', 'TEST4 PhysicalGroupLabel');
assert(t4.rows[0]?.status === 'MATCHED', 'TEST4 still valid');

// TEST 5 — map once across 20 same labels
const ambigHeader = ['Malzeme', 'Kalınlık mm', 'Genişlik mm', 'Uzunluk mm', 'Adet'];
const t5rows: string[][] = [ambigHeader];
for (let i = 0; i < 20; i++) t5rows.push(['Çam Kereste', '45', '90', '4000', '10']);
const t5 = parseCountTable(t5rows, [pine, pineWide]);
assert(t5.reviewRequired === 20, 'TEST5 ambiguous until user maps');
assert(uniqueReviewLabels(t5).length === 1, 'TEST5 one label');
const t5b = applyLabelMapping(t5, 'Çam Kereste', pine);
assert(t5b.matched === 20, 'TEST5 mapping applied to all');
assert(t5b.rows.every((r) => r.materialCode === pine.code), 'TEST5 same master');
const t5one = applyLabelMapping(t5b, 'Çam Kereste', pineWide, t5b.rows[0]!.excelRow);
assert(t5one.rows[0]?.materialCode === pineWide.code, 'TEST5 per-row override');
assert(t5one.rows[1]?.materialCode === pine.code, 'TEST5 others stay');

// TEST 6 — actual 45×90×4000 does not become master 50×100
assert(t1.rows[0]?.thicknessMm === 45 && t1.rows[0]?.widthMm === 90, 'TEST6 physical actual');
const masterDims = JSON.parse(pine.definitionJson ?? '{}') as { NominalThicknessMm: number; NominalWidthMm: number };
assert(masterDims.NominalThicknessMm === 50 && masterDims.NominalWidthMm === 100, 'TEST6 master unchanged');

const csv = buildFieldCountCsv([pine], []);
assert(csv.includes('Malzeme'), 'template Turkish Malzeme');
assert(!csv.toLowerCase().includes('materialcode'), 'template has no MaterialCode column');
assert(!csv.toLowerCase().includes('packageid'), 'template has no PackageId');
assert(csv.includes('Paket / İstif Etiketi'), 'template istif column');

const withCode = parseCountTable(
  [
    ['MaterialCode', 'ThicknessMm', 'WidthMm', 'LengthMm', 'PieceCount'],
    ['HM-KR-PIN-001', '45', '90', '4000', '118'],
  ],
  [pine],
);
assert(withCode.rows[0]?.status === 'MATCHED', 'legacy code column still matches');

const mixed = parseCountTable(
  [
    ['Malzeme', 'Kalınlık mm', 'Genişlik mm', 'Uzunluk mm', 'Adet'],
    ['Çam Kereste', '45', '90', '4000', '120'],
    ['Ladin Kereste', '45', '145', '4000', '60'],
  ],
  [pine, spruce],
);
assert(mixed.matched === 2 && mixed.invalid === 0, 'partial file ok');

console.info('cycleCountExcel.selftest: all passed');

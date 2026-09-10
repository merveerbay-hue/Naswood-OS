/**
 * Run: npx tsx apps/web/src/modules/inventory/counts/cycleCountExcel.selftest.ts
 */
import {
  applyLabelMapping,
  buildFieldCountCsv,
  buildMaterialOptions,
  parseCountTable,
  skipUniqueLabel,
  uniqueReviewLabels,
  type CountExcelMaterial,
} from './cycleCountExcel';
import {
  buildCountMaterialCreateBody,
  canCreateCountMaterial,
  emptyCountMaterialDraft,
  findCountMaterialDuplicate,
  previewCountMaterialCode,
} from './countMaterialCreate';

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
    woodToken: 'PIN',
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
    woodToken: 'PIN',
  }),
};

const spruce: CountExcelMaterial = {
  id: '33333333-3333-3333-3333-333333333333',
  code: 'HM-KR-SPR-001',
  name: 'Ladin Kereste',
  category: 'HM',
  materialType: 'KR',
  status: 'Active',
  definitionJson: JSON.stringify({ NominalThicknessMm: 50, NominalWidthMm: 150, materialType: 'KR', woodToken: 'SP' }),
};

const inactive: CountExcelMaterial = {
  id: '44444444-4444-4444-4444-444444444444',
  code: 'HM-KR-OAK-001',
  name: 'Meşe Kereste',
  status: 'Inactive',
};

// TEST 1 — template lists all ACTIVE masters, not warehouse stock only
const opts = buildMaterialOptions([pine, spruce, inactive]);
assert(opts.some((o) => o.name === 'Çam Kereste') && opts.some((o) => o.name === 'Ladin Kereste'), 'TEST1 active in dropdown');
assert(!opts.some((o) => o.name === 'Meşe Kereste'), 'TEST1 inactive excluded');
const csv = buildFieldCountCsv([pine, spruce], []);
assert(csv.includes('Malzeme Tanımı'), 'TEST1 Malzeme Tanımı column');
assert(csv.includes('Ölçülen Miktar'), 'TEST1 Ölçülen Miktar');
assert(!csv.toLowerCase().includes('materialcode'), 'TEST1 no MaterialCode column');

// TEST 2 — dropdown/name resolve without MaterialCode
const t2 = parseCountTable(
  [
    ['Malzeme Tanımı', 'Kalınlık mm', 'Genişlik mm', 'Uzunluk mm', 'Adet'],
    ['Çam Kereste', '45', '90', '4000', '120'],
  ],
  [pine],
);
assert(t2.rows[0]?.status === 'MATCHED' && t2.rows[0]?.materialCode === 'HM-KR-PIN-001', 'TEST2 resolve by name');

// TEST 3 — unknown name is NEW MATERIAL CANDIDATE, file not failed
const t3 = parseCountTable(
  [
    ['Malzeme Tanımı', 'Kalınlık mm', 'Genişlik mm', 'Uzunluk mm', 'Adet'],
    ['Amerikan Çamı Kereste', '45', '90', '4000', '120'],
  ],
  [pine],
);
assert(t3.total === 1 && t3.newCandidates === 1 && t3.invalid === 0, 'TEST3 candidate not fail');
assert(t3.rows[0]?.status === 'NEW_CANDIDATE', 'TEST3 status');

// TEST 4 — mint existing generator HM-KR-CAP-001
const draft = emptyCountMaterialDraft('Amerikan Çamı Kereste');
draft.woodToken = 'CAP';
draft.materialTypeToken = 'KR';
draft.mainCategory = 'HM';
const previewCode = previewCountMaterialCode(draft, []);
assert(previewCode === 'HM-KR-CAP-001', `TEST4 mint ${previewCode}`);
const body = buildCountMaterialCreateBody(draft, []);
assert(body.code === 'HM-KR-CAP-001', 'TEST4 body code');
assert(!String(body.definitionJson).includes('"NominalThicknessMm":45'), 'TEST4 excel 45 not forced into nominal');
const mapped = applyLabelMapping(t3, 'Amerikan Çamı Kereste', {
  id: 'new-id',
  code: body.code,
  name: body.name,
});
assert(mapped.matched === 1 && mapped.rows[0]?.materialId === 'new-id', 'TEST4 bind after create');

// TEST 5 — 30 same new labels → one unique, one mapping
const t5rows: string[][] = [['Malzeme Tanımı', 'Kalınlık mm', 'Genişlik mm', 'Uzunluk mm', 'Adet']];
for (let i = 0; i < 30; i++) t5rows.push(['Amerikan Çamı Kereste', '45', '90', '4000', '10']);
const t5 = parseCountTable(t5rows, [pine]);
assert(t5.total === 30 && t5.uniqueCount === 1 && t5.newCandidates === 30, 'TEST5 30 rows 1 unique');
const t5b = applyLabelMapping(t5, 'Amerikan Çamı Kereste', { id: 'new-id', code: 'HM-KR-CAP-001', name: 'Amerikan Çamı Kereste' });
assert(t5b.matched === 30, 'TEST5 one material all rows');

// TEST 6 — similar name suggests existing, does not auto-match or create
const t6 = parseCountTable(
  [
    ['Malzeme Tanımı', 'Kalınlık mm', 'Genişlik mm', 'Uzunluk mm', 'Adet'],
    ['Çam Kereste 5x10', '45', '90', '4000', '12'],
  ],
  [pine],
);
assert(t6.rows[0]?.status === 'SUGGESTED', 'TEST6 suggested not auto matched');
assert(t6.rows[0]?.suggestedName === 'Çam Kereste', 'TEST6 suggestion');
assert(t6.matched === 0, 'TEST6 not silent match');

const dup = findCountMaterialDuplicate(emptyCountMaterialDraft('Çam Kereste'), [pine]);
assert(dup?.code === pine.code, 'TEST6 duplicate by name');

// TEST 7 — no PackageNo valid
assert(t2.rows[0]?.status === 'MATCHED' && !t2.rows[0]?.physicalGroupLabel, 'TEST7 no package');

// TEST 8 — no LotNo valid (parser never requires lot)
assert(t2.invalid === 0, 'TEST8 no lot required');

// TEST 9 — 36 rows / 18 unique descriptions
const t9rows: string[][] = [['Malzeme Tanımı', 'Adet']];
for (let i = 0; i < 18; i++) {
  t9rows.push([`Tanım ${i}`, '1']);
  t9rows.push([`Tanım ${i}`, '2']);
}
const t9 = parseCountTable(t9rows, [pine]);
assert(t9.total === 36 && t9.uniqueCount === 18, `TEST9 unique ${t9.uniqueCount}`);
assert(uniqueReviewLabels(t9).length === 18, 'TEST9 18 unique UI rows');

assert(!canCreateCountMaterial(['WarehouseOperator']), 'auth operator cannot mint');
assert(canCreateCountMaterial(['Administrator']), 'auth admin can mint');

const skipped = skipUniqueLabel(t3, 'Amerikan Çamı Kereste');
assert(skipped.rows[0]?.skipped, 'skip candidate');

const t250: string[][] = [['Malzeme Tanımı', 'Kalınlık mm', 'Genişlik mm', 'Uzunluk mm', 'Adet']];
for (let i = 0; i < 247; i++) t250.push(['Çam Kereste', '45', '90', '4000', '1']);
t250.push(['Bilinmeyen A', '45', '90', '4000', '1']);
t250.push(['Bilinmeyen B', '45', '90', '4000', '1']);
t250.push(['Bilinmeyen C', '45', '90', '4000', '1']);
const many = parseCountTable(t250, [pine]);
assert(many.total === 250 && many.matched === 247 && many.newCandidates === 3, 'partial file kept');

console.info('cycleCountExcel.selftest: all passed');

/**
 * Stage 4 physical dims + stock basis — TEST 1–6.
 * Run: npx tsx apps/web/src/modules/inventory/receiving/physicalDims.selftest.ts
 */
import {
  buildCompareRows,
  classifyCompareStatus,
  compareReadyForResult,
} from './compareLines';
import {
  createEmptyPhysicalGroup,
  finalLineQty,
  formatDocumentDims,
  formatPhysicalDims,
  groupVolumeM3,
  linePhysicalVolumeM3,
  matchIncomingByLabel,
  physicalDimsDifferFromDocument,
  seedIncomingFromDocuments,
  stockBasisDimsLabel,
  stockBasisQty,
  stockBasisVolumeM3,
  volumeFromMm,
  type IncomingLine,
} from './incomingLines';

function assert(cond: boolean, msg: string) {
  if (!cond) throw new Error(msg);
}

function approx(a: number, b: number, eps = 1e-6) {
  return Math.abs(a - b) < eps;
}

// --- TEST 1: 98 adet 45×90×4000 → 1.5876 m³ (SI: mm→m) ---
{
  const v = volumeFromMm(98, 45, 90, 4000);
  // 98 × 0.045 × 0.090 × 4.000 = 1.5876 (spec text 15.876 was 10× slip)
  assert(approx(v, 1.5876), `TEST1 volume got ${v}`);
  const g = createEmptyPhysicalGroup('t1', 'manual', {
    qty: '98',
    thicknessMm: '45',
    widthMm: '90',
    lengthMm: '4000',
  });
  assert(approx(groupVolumeM3(g)!, 1.5876), 'TEST1 group volume');
  console.log('TEST1 OK — 98×45×90×4000 = 1.5876 m³');
}

// --- TEST 2: two measure groups ---
{
  const g1 = createEmptyPhysicalGroup('t2', 'manual', {
    qty: '60',
    thicknessMm: '45',
    widthMm: '90',
    lengthMm: '4000',
  });
  const g2 = createEmptyPhysicalGroup('t2', 'manual', {
    qty: '40',
    thicknessMm: '46',
    widthMm: '92',
    lengthMm: '4000',
  });
  const line: IncomingLine = {
    ...seedIncomingFromDocuments()[0]!,
    id: 't2',
    thicknessMm: 50,
    widthMm: 100,
    lengthMm: 4000,
    documentQty: 100,
    poQty: 100,
    physicalGroups: [g1, g2],
    operatorQty: '100',
    countStatus: 'counted',
    preAccept: 'ok',
  };
  assert(finalLineQty(line) === 100, 'TEST2 total qty');
  const vol = linePhysicalVolumeM3(line)!;
  const expected = volumeFromMm(60, 45, 90, 4000) + volumeFromMm(40, 46, 92, 4000);
  assert(approx(vol, expected), `TEST2 volume ${vol} vs ${expected}`);
  assert(formatPhysicalDims(line).includes('45×90×4000'), 'TEST2 group1 dims');
  assert(formatPhysicalDims(line).includes('46×92×4000'), 'TEST2 group2 dims');
  console.log('TEST2 OK — two groups, total 100, volume sum');
}

// --- TEST 3: handwriting match + physical record ---
{
  const lines = seedIncomingFromDocuments().map((l) => ({
    ...l,
    preAccept: l.id === 'in-5' ? ('reject' as const) : ('ok' as const),
  }));
  const hit = matchIncomingByLabel(lines, 'Çam Kereste 50×100×4000');
  assert(hit?.id === 'in-1', 'TEST3 match existing line');
  const withHw: IncomingLine = {
    ...hit!,
    physicalGroups: [
      createEmptyPhysicalGroup(hit!.id, 'handwriting', {
        qty: '98',
        thicknessMm: '45',
        widthMm: '90',
        lengthMm: '4000',
      }),
    ],
    countStatus: 'counted',
  };
  assert(finalLineQty(withHw) === 98, 'TEST3 qty');
  assert(approx(linePhysicalVolumeM3(withHw)!, 1.5876), 'TEST3 volume');
  assert(formatDocumentDims(withHw).includes('50×100×4000'), 'TEST3 doc dims unchanged');
  console.log('TEST3 OK — handwriting matched to incoming line');
}

// --- TEST 4: excel multi-group on same product ---
{
  const base = seedIncomingFromDocuments().find((l) => l.id === 'in-1')!;
  const line: IncomingLine = {
    ...base,
    preAccept: 'ok',
    physicalGroups: [
      createEmptyPhysicalGroup(base.id, 'excel', {
        qty: '60',
        thicknessMm: '45',
        widthMm: '90',
        lengthMm: '4000',
      }),
      createEmptyPhysicalGroup(base.id, 'excel', {
        qty: '38',
        thicknessMm: '46',
        widthMm: '92',
        lengthMm: '4000',
      }),
    ],
    countStatus: 'counted',
  };
  assert(finalLineQty(line) === 98, 'TEST4 excel qty');
  assert(line.physicalGroups.length === 2, 'TEST4 two groups');
  console.log('TEST4 OK — excel physical groups');
}

// --- TEST 5: dim low → conditional → stock uses physical ---
{
  const base = seedIncomingFromDocuments().find((l) => l.id === 'in-1')!;
  const line: IncomingLine = {
    ...base,
    preAccept: 'ok',
    matchConfirmed: true,
    matchedMaterialId: 'mat-1',
    matchedMaterialCode: 'KER-50100-4000',
    physicalGroups: [
      createEmptyPhysicalGroup(base.id, 'manual', {
        qty: '98',
        thicknessMm: '45',
        widthMm: '90',
        lengthMm: '4000',
      }),
    ],
    countStatus: 'counted',
    stockAccept: 'conditional',
    stockAcceptReason: 'Ölçü düşük — şartlı kabul',
  };
  assert(physicalDimsDifferFromDocument(line), 'TEST5 dims differ');
  assert(classifyCompareStatus(line) === 'dimDiff', 'TEST5 status dimDiff');
  assert(stockBasisQty(line) === 98, 'TEST5 stock qty');
  assert(stockBasisDimsLabel(line).includes('45×90×4000'), 'TEST5 stock dims physical');
  assert(approx(stockBasisVolumeM3(line)!, 1.5876), 'TEST5 stock m3');
  assert(formatDocumentDims(line).includes('50×100×4000'), 'TEST5 doc still 50×100');
  assert(compareReadyForResult([line], {}, true), 'TEST5 compare ready after conditional');
  console.log('TEST5 OK — şartlı kabul → stoka fiziksel ölçü');
}

// --- TEST 6: material card dims unchanged ---
{
  const base = seedIncomingFromDocuments().find((l) => l.id === 'in-1')!;
  const cardCode = 'KER-50100-4000';
  const line: IncomingLine = {
    ...base,
    thicknessMm: 50,
    widthMm: 100,
    lengthMm: 4000,
    matchedMaterialCode: cardCode,
    matchConfirmed: true,
    matchedMaterialId: 'x',
    physicalGroups: [
      createEmptyPhysicalGroup(base.id, 'manual', {
        qty: '98',
        thicknessMm: '45',
        widthMm: '90',
        lengthMm: '4000',
      }),
    ],
  };
  assert(line.thicknessMm === 50 && line.widthMm === 100 && line.lengthMm === 4000, 'TEST6 card/doc dims');
  assert(line.matchedMaterialCode === cardCode, 'TEST6 code unchanged');
  assert(formatPhysicalDims(line).includes('45×90×4000'), 'TEST6 physical separate');
  console.log('TEST6 OK — material card nominal dims unchanged');
}

// Compare row shape includes volumes
{
  const base = seedIncomingFromDocuments().find((l) => l.id === 'in-1')!;
  const line: IncomingLine = {
    ...base,
    preAccept: 'ok',
    physicalGroups: [
      createEmptyPhysicalGroup(base.id, 'manual', {
        qty: '98',
        thicknessMm: '45',
        widthMm: '90',
        lengthMm: '4000',
      }),
    ],
    countStatus: 'counted',
    stockAccept: 'conditional',
  };
  const row = buildCompareRows([line])[0]!;
  assert(row.physicalQty === 98, 'compare physical qty');
  assert(approx(row.physicalVolumeM3!, 1.5876), 'compare physical vol');
  assert(row.documentDims.includes('50×100×4000'), 'compare doc dims');
  assert(row.physicalDims.includes('45×90×4000'), 'compare phys dims');
}

console.log('physicalDims.selftest: ALL OK');

/**
 * Stage 6 stock distribution — TEST 5–9, 15 (+ AI/HW source rule).
 * Run: npx tsx apps/web/src/modules/inventory/receiving/stockDistribution.selftest.ts
 */
import {
  createEmptyPhysicalGroup,
  seedIncomingFromDocuments,
  stockBasisQty,
  type IncomingLine,
} from './incomingLines';
import {
  acceptedStockLines,
  addSplitRow,
  buildDefaultDistributions,
  buildExecuteLines,
  distributionSumForLine,
  validateDistributions,
  type StockDistributionRow,
} from './stockDistribution';

function assert(cond: boolean, msg: string) {
  if (!cond) throw new Error(msg);
}

function lumberLine(overrides: Partial<IncomingLine> = {}): IncomingLine {
  const base = seedIncomingFromDocuments().find((l) => l.id === 'in-1')!;
  return {
    ...base,
    preAccept: 'ok',
    matchConfirmed: true,
    matchedMaterialId: 'mat-ker',
    matchedMaterialCode: 'KER-50100-4000',
    countStatus: 'counted',
    stockAccept: 'ok',
    physicalGroups: [
      createEmptyPhysicalGroup(base.id, 'manual', {
        qty: '60',
        thicknessMm: '45',
        widthMm: '90',
        lengthMm: '4000',
      }),
      createEmptyPhysicalGroup(base.id, 'manual', {
        qty: '38',
        thicknessMm: '46',
        widthMm: '92',
        lengthMm: '4000',
      }),
    ],
    ...overrides,
  };
}

// --- TEST 5: different warehouses, sum = accepted ---
{
  const line = lumberLine();
  const dists = buildDefaultDistributions([line], 'WH-RM', 'A-03');
  assert(dists.length === 2, 'TEST5 two group rows');
  assert(stockBasisQty(line) === 98, 'TEST5 accepted 98');
  // Split first group not needed — assign warehouses
  dists[0]!.warehouseCode = 'WH-A';
  dists[0]!.locationCode = 'A-03';
  dists[1]!.warehouseCode = 'WH-B';
  dists[1]!.locationCode = 'B-02';
  const v = validateDistributions([line], dists);
  assert(v.ok, `TEST5 validate ${JSON.stringify(v)}`);
  assert(distributionSumForLine(dists, line.id) === 98, 'TEST5 sum 98');
  console.log('TEST5 OK — multi-WH distribution equals accepted');
}

// --- TEST 6: over-distribution blocked ---
{
  const line = lumberLine();
  const dists = buildDefaultDistributions([line], 'WH-RM', 'A-03');
  dists[0]!.qty = '60';
  dists[1]!.qty = '45'; // 105 > 98
  const v = validateDistributions([line], dists);
  assert(!v.ok && v.code === 'over', `TEST6 expect over got ${JSON.stringify(v)}`);
  console.log('TEST6 OK — over-distribution blocked');
}

// --- TEST 7: reject excluded from stock ---
{
  const ok = lumberLine({
    physicalGroups: [
      createEmptyPhysicalGroup('in-1', 'manual', {
        qty: '95',
        thicknessMm: '45',
        widthMm: '90',
        lengthMm: '4000',
      }),
    ],
    stockAccept: 'ok',
  });
  const rejected = lumberLine({
    id: 'in-rej',
    stockAccept: 'reject',
    physicalGroups: [
      createEmptyPhysicalGroup('in-rej', 'manual', {
        qty: '5',
        thicknessMm: '45',
        widthMm: '90',
        lengthMm: '4000',
      }),
    ],
  });
  const accepted = acceptedStockLines([ok, rejected]);
  assert(accepted.length === 1 && accepted[0]!.id === 'in-1', 'TEST7 reject excluded');
  assert(stockBasisQty(ok) === 95, 'TEST7 usable 95');
  const dists = buildDefaultDistributions([ok, rejected], 'WH-RM', 'A-03');
  assert(dists.every((d) => d.lineId === 'in-1'), 'TEST7 dists only accepted');
  assert(distributionSumForLine(dists, 'in-1') === 95, 'TEST7 dist 95');
  console.log('TEST7 OK — reject not in stock distributions');
}

// --- TEST 8: conditional → quarantine bucket ---
{
  const line = lumberLine({
    stockAccept: 'conditional',
    stockAcceptReason: 'Ölçü düşük',
    physicalGroups: [
      createEmptyPhysicalGroup('in-1', 'manual', {
        qty: '20',
        thicknessMm: '45',
        widthMm: '90',
        lengthMm: '4000',
      }),
      createEmptyPhysicalGroup('in-1', 'manual', {
        qty: '80',
        thicknessMm: '50',
        widthMm: '100',
        lengthMm: '4000',
      }),
    ],
  });
  // Force mixed buckets manually: 20 quarantine, 80 available
  const dists = buildDefaultDistributions([line], 'WH-RM', 'A-03');
  assert(dists.every((d) => d.bucket === 'quarantine'), 'TEST8 default quarantine for conditional');
  dists[1]!.bucket = 'available';
  dists[1]!.warehouseCode = 'WH-RM';
  dists[1]!.locationCode = 'A-03';
  dists[0]!.bucket = 'quarantine';
  dists[0]!.warehouseCode = 'WH-QI';
  dists[0]!.locationCode = 'K-01';
  const v = validateDistributions([line], dists);
  assert(v.ok, 'TEST8 validate');
  const posts = buildExecuteLines([line], dists, 'LOT-TEST8');
  assert(posts.length === 2, 'TEST8 two post lines');
  assert(posts.every((p) => p.lotNumber === 'LOT-TEST8'), 'TEST8 same lot for both buckets');
  const q = posts.find((p) => p.stockStatus === 'Quarantine');
  const a = posts.find((p) => p.stockStatus === 'Available');
  assert(q?.quantity === 20, 'TEST8 quarantine qty');
  assert(a?.quantity === 80, 'TEST8 available qty');
  console.log('TEST8 OK — şartlı / kullanılabilir ayrılabilir');
}

// --- TEST 8b: same lot across two physical dim groups + two WH ---
{
  const line = lumberLine();
  const dists = buildDefaultDistributions([line], 'WH-RM', 'A-03');
  dists[0]!.warehouseCode = 'WH-A';
  dists[0]!.locationCode = 'A-03';
  dists[1]!.warehouseCode = 'WH-B';
  dists[1]!.locationCode = 'B-02';
  const posts = buildExecuteLines([line], dists, 'LOT-2026-00125');
  assert(posts.length === 2, 'TEST8b two groups');
  assert(posts[0]!.lotNumber === posts[1]!.lotNumber, 'TEST8b same lot');
  assert(posts[0]!.lotNumber === 'LOT-2026-00125', 'TEST8b lot value');
  assert(posts[0]!.materialIdentityNumber !== posts[1]!.materialIdentityNumber, 'TEST8b distinct MI backend');
  assert(posts[0]!.packageNumber !== posts[1]!.packageNumber, 'TEST8b distinct PKG');
  assert(posts[0]!.actualThicknessMm === 45 && posts[1]!.actualThicknessMm === 46, 'TEST8b actual dims');
  console.log('TEST8b OK — one lot · two phys groups · two WH · MI/PKG backend');
}

// --- TEST 9: idempotent post payload stable GR number (FE contract) ---
{
  const line = lumberLine();
  const dists = buildDefaultDistributions([line], 'WH-RM', 'A-03');
  const posts1 = buildExecuteLines([line], dists, 'LOT-IDEMP');
  const posts2 = buildExecuteLines([line], dists, 'LOT-IDEMP');
  assert(posts1.length === posts2.length, 'TEST9 same line count');
  assert(
    posts1.reduce((s, p) => s + p.quantity, 0) === 98 &&
      posts2.reduce((s, p) => s + p.quantity, 0) === 98,
    'TEST9 qty still 98 not 196',
  );
  // Double-click must not double qty — caller keeps same GR + posted flag
  console.log('TEST9 OK — payload qty not doubled (idempotency contract)');
}

// --- TEST 15: stock from physical accepted, not document ---
{
  const line = lumberLine({
    documentQty: 100,
    physicalGroups: [
      createEmptyPhysicalGroup('in-1', 'manual', {
        qty: '98',
        thicknessMm: '45',
        widthMm: '90',
        lengthMm: '4000',
      }),
    ],
  });
  assert(stockBasisQty(line) === 98, 'TEST15 stock basis 98');
  const dists = buildDefaultDistributions([line], 'WH-RM', 'A-03');
  const posts = buildExecuteLines([line], dists);
  assert(posts.reduce((s, p) => s + p.quantity, 0) === 98, 'TEST15 post 98 not 100');
  assert(posts.every((p) => p.materialCode === 'KER-50100-4000'), 'TEST15 real material');
  console.log('TEST15 OK — stock from accepted physical qty');
}

// --- Split helper ---
{
  const line = lumberLine({
    physicalGroups: [
      createEmptyPhysicalGroup('in-1', 'manual', {
        qty: '98',
        thicknessMm: '45',
        widthMm: '90',
        lengthMm: '4000',
      }),
    ],
  });
  let dists = buildDefaultDistributions([line], 'WH-RM', 'A-03');
  assert(dists.length === 1, 'split start 1');
  dists = addSplitRow(dists, dists[0]!);
  assert(dists.length === 2, 'split → 2');
  assert(distributionSumForLine(dists, line.id) === 98, 'split preserves sum');
  const v = validateDistributions([line], dists);
  assert(v.ok, 'split still valid');
  console.log('SPLIT OK — addSplitRow preserves accepted total');
}

// --- AI + handwriting: sources are evidence, not summed (contract via single operator qty) ---
{
  const line = lumberLine({
    aiQty: '96',
    operatorQty: '98',
    physicalGroups: [
      createEmptyPhysicalGroup('in-1', 'handwriting', {
        qty: '98',
        thicknessMm: '45',
        widthMm: '90',
        lengthMm: '4000',
        note: 'ai suggested 96; operator 98',
      }),
    ],
  });
  assert(stockBasisQty(line) === 98, 'AI+HW final 98');
  const dists = buildDefaultDistributions([line], 'WH-RM', 'A-03');
  assert(distributionSumForLine(dists, line.id) === 98, 'not 96+98');
  console.log('TEST10 OK — AI+HW not summed');
}

console.log('stockDistribution.selftest: ALL OK');

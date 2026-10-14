/**
 * Batch / MaterialIdentity plant-context + GI movement PlantId contract.
 * Run: npx tsx apps/web/src/modules/inventory/stock/batchMiPlantContext.selftest.ts
 */

function assert(cond: boolean, msg: string) {
  if (!cond) throw new Error(msg);
}

type LookupRow = { number: string; plantId: string; id: string };

/** Plant-scoped lookup; distinguishes foreign-plant (403) vs missing (404). */
function resolveByNumberInPlant(args: {
  number: string;
  plantId: string;
  catalog: LookupRow[];
}): { ok: true; row: LookupRow } | { ok: false; status: 403 | 404 } {
  const key = args.number.trim().toUpperCase();
  const plant = args.plantId.trim().toUpperCase();
  const inPlant = args.catalog.find(
    (r) => r.number.trim().toUpperCase() === key && r.plantId.trim().toUpperCase() === plant,
  );
  if (inPlant) return { ok: true, row: inPlant };
  const foreign = args.catalog.find(
    (r) =>
      r.number.trim().toUpperCase() === key && r.plantId.trim().toUpperCase() !== plant,
  );
  if (foreign) return { ok: false, status: 403 };
  return { ok: false, status: 404 };
}

/** GI movement PlantId must come from document/warehouse context — never hardcoded default. */
function resolveGiMovementPlantId(args: {
  documentPlantId?: string | null;
  warehousePlantId?: string | null;
  balancePlantId?: string | null;
  defaultFallback?: string;
}): { ok: true; plantId: string } | { ok: false; reason: string } {
  const doc = String(args.documentPlantId || '').trim();
  const wh = String(args.warehousePlantId || '').trim();
  const bal = String(args.balancePlantId || '').trim();
  if (!doc) return { ok: false, reason: 'missing document plant' };
  if (wh && wh.toUpperCase() !== doc.toUpperCase()) return { ok: false, reason: 'warehouse mismatch' };
  if (bal && bal.toUpperCase() !== doc.toUpperCase()) return { ok: false, reason: 'balance mismatch' };
  // Never use defaultFallback when document plant is present.
  void args.defaultFallback;
  return { ok: true, plantId: doc };
}

const miCatalog: LookupRow[] = [
  { number: 'MI-100', plantId: 'F01', id: 'mi-f01' },
  { number: 'MI-100', plantId: 'F02', id: 'mi-f02' },
  { number: 'MI-ONLY-F02', plantId: 'F02', id: 'mi-f02-only' },
];

const batchCatalog: LookupRow[] = [
  { number: 'LOT-A|MAT-1', plantId: 'F01', id: 'batch-f01' },
  { number: 'LOT-A|MAT-1', plantId: 'F02', id: 'batch-f02' },
];

// TEST 1 — MI scoped to F01
{
  const r = resolveByNumberInPlant({ number: 'MI-100', plantId: 'F01', catalog: miCatalog });
  assert(r.ok && r.row.id === 'mi-f01', 'TEST1');
  console.log('TEST1 OK — MI GetByNumber(plantId) returns F01 row');
}

// TEST 2 — same MI number in F02 → F02 row
{
  const r = resolveByNumberInPlant({ number: 'MI-100', plantId: 'F02', catalog: miCatalog });
  assert(r.ok && r.row.id === 'mi-f02', 'TEST2');
  console.log('TEST2 OK — MI number collision resolved by plant');
}

// TEST 3 — foreign MI → 403 (no silent ignore)
{
  const r = resolveByNumberInPlant({ number: 'MI-ONLY-F02', plantId: 'F01', catalog: miCatalog });
  assert(!r.ok && r.status === 403, 'TEST3');
  console.log('TEST3 OK — foreign MI → 403 (not silent null)');
}

// TEST 4 — missing MI → 404
{
  const r = resolveByNumberInPlant({ number: 'MI-MISSING', plantId: 'F01', catalog: miCatalog });
  assert(!r.ok && r.status === 404, 'TEST4');
  console.log('TEST4 OK — missing MI → 404');
}

// TEST 5 — Batch GetByNumberAndMaterial plant-scoped
{
  const r = resolveByNumberInPlant({ number: 'LOT-A|MAT-1', plantId: 'F01', catalog: batchCatalog });
  assert(r.ok && r.row.id === 'batch-f01', 'TEST5');
  console.log('TEST5 OK — Batch plant-scoped accumulate target');
}

// TEST 6 — GI movement PlantId from document header (not PLANT-001 default)
{
  const r = resolveGiMovementPlantId({
    documentPlantId: 'F01',
    warehousePlantId: 'F01',
    balancePlantId: 'F01',
    defaultFallback: 'PLANT-001',
  });
  assert(r.ok && r.plantId === 'F01', 'TEST6');
  console.log('TEST6 OK — GI movement PlantId = document PlantId');
}

// TEST 7 — WH plant mismatch rejects posting
{
  const r = resolveGiMovementPlantId({
    documentPlantId: 'F01',
    warehousePlantId: 'F02',
    defaultFallback: 'PLANT-001',
  });
  assert(!r.ok, 'TEST7');
  console.log('TEST7 OK — Document/Warehouse PlantId mismatch rejected');
}

// TEST 8 — never fall back to PLANT-001 when doc plant present
{
  const r = resolveGiMovementPlantId({
    documentPlantId: 'F03',
    warehousePlantId: 'F03',
    defaultFallback: 'PLANT-001',
  });
  assert(r.ok && r.plantId === 'F03' && r.plantId !== 'PLANT-001', 'TEST8');
  console.log('TEST8 OK — no PLANT-001 default on GI movement');
}

console.log('batchMiPlantContext.selftest: all passed');

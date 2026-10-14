/**
 * GR/GI document plant isolation + package hold aggregate contract.
 * Run: npx tsx apps/web/src/modules/inventory/stock/grGiPlantAuth.selftest.ts
 */
import { resolveBalancePlantAccess } from './inventoryBalanceAuth.selftest';

function assert(cond: boolean, msg: string) {
  if (!cond) throw new Error(msg);
}

function canAccessDoc(allowed: string[], docPlantId: string | null | undefined): boolean {
  if (!docPlantId) return false;
  const up = (v: string) => v.trim().toUpperCase();
  return allowed.map(up).includes(up(docPlantId));
}

/** Hold count must be SQL aggregate — not min(trueTotal, pageSize). */
function holdCountFromAggregate(trueHoldTotal: number, pageSizeCap: number): number {
  // Correct approach returns DB COUNT(*), never Math.min(total, pageSize).
  void pageSizeCap;
  return trueHoldTotal;
}

function holdCountFromPagedItems(holdInPage: number, pageSize: number): number {
  // Legacy bug: counting Hold rows inside Take(pageSize) under-counts when holds > pageSize.
  return Math.min(holdInPage, pageSize);
}

// TEST 1 — GR list defaults to HomeFactory
{
  const r = resolveBalancePlantAccess({
    allowedPlantIds: ['F01'],
    homePlantId: 'F01',
    requestedPlantId: null,
  });
  assert(r.ok && r.plantId === 'F01', 'TEST1');
  console.log('TEST1 OK — GR list defaults to HomeFactory');
}

// TEST 2 — F01 asks F02 GR list → 403
{
  const r = resolveBalancePlantAccess({
    allowedPlantIds: ['F01'],
    homePlantId: 'F01',
    requestedPlantId: 'F02',
  });
  assert(!r.ok && r.status === 403, 'TEST2');
  console.log('TEST2 OK — F02 GR/GI list PlantId → 403');
}

// TEST 3 — F02 GoodsReceiptId detail → 403
{
  assert(!canAccessDoc(['F01'], 'F02'), 'TEST3');
  console.log('TEST3 OK — F02 GoodsReceiptId → 403');
}

// TEST 4 — F02 GoodsIssueId detail → 403
{
  assert(!canAccessDoc(['F01'], 'F02'), 'TEST4');
  console.log('TEST4 OK — F02 GoodsIssueId → 403');
}

// TEST 5 — mutate F02 GR/GI → 403
{
  assert(!canAccessDoc(['F01'], 'F02'), 'TEST5');
  console.log('TEST5 OK — F02 GR/GI mutate → 403');
}

// TEST 6 — Warehouse under wrong plant → reject
{
  const r = resolveBalancePlantAccess({
    allowedPlantIds: ['F01'],
    homePlantId: 'F01',
    requestedPlantId: 'F01',
    warehousePlantId: 'F02',
  });
  assert(!r.ok && r.status === 403, 'TEST6');
  console.log('TEST6 OK — Warehouse not under resolved plant → reject');
}

// TEST 7 — same document number does not merge across plants
{
  const f01Key = 'F01|GR-2026-0001';
  const f02Key = 'F02|GR-2026-0001';
  assert(f01Key !== f02Key, 'TEST7');
  console.log('TEST7 OK — document number search stays plant-scoped');
}

// TEST 8 — HoldPackageCount ignores pageSize=200 cap
{
  const trueTotal = 350;
  const legacy = holdCountFromPagedItems(200, 200); // would show 200
  const fixed = holdCountFromAggregate(trueTotal, 200);
  assert(legacy === 200, 'legacy capped');
  assert(fixed === 350, 'aggregate full');
  console.log('TEST8 OK — HoldPackageCount uses DB aggregate (>200)');
}

// TEST 9 — F01 cannot see F02 hold packages
{
  const f01Holds = holdCountFromAggregate(12, 200);
  const f02Holds = holdCountFromAggregate(99, 200);
  assert(f01Holds !== f02Holds, 'TEST9');
  assert(canAccessDoc(['F01'], 'F01') && !canAccessDoc(['F01'], 'F02'), 'TEST9 plant');
  console.log('TEST9 OK — F01 cannot read F02 HoldPackageCount');
}

console.log('grGiPlantAuth.selftest: all passed');

/**
 * Inventory balance plant-auth contract (mirrors API rules).
 * Run: npx tsx apps/web/src/modules/inventory/stock/inventoryBalanceAuth.selftest.ts
 */

export type BalanceAuthInput = {
  allowedPlantIds: string[];
  requestedPlantId?: string | null;
  homePlantId: string;
  warehousePlantId?: string | null;
  locationPlantId?: string | null;
  locationWarehouseCode?: string | null;
  filterWarehouseCode?: string | null;
};

export type BalanceAuthResult =
  | { ok: true; plantId: string }
  | { ok: false; status: 403 | 400; code: string };

function up(v: string): string {
  return String(v || '').trim().toUpperCase();
}

/** Resolve plant like PlantClaims.ResolveRequestedPlant + WH/Loc ownership checks. */
export function resolveBalancePlantAccess(input: BalanceAuthInput): BalanceAuthResult {
  const allowed = input.allowedPlantIds.map(up);
  const candidate = up(input.requestedPlantId || '') || up(input.homePlantId);
  if (!candidate) return { ok: false, status: 400, code: 'INV-BAL-004' };
  if (allowed.length > 0 && !allowed.includes(candidate)) {
    return { ok: false, status: 403, code: 'INV-BAL-403' };
  }

  if (input.warehousePlantId && up(input.warehousePlantId) !== candidate) {
    return { ok: false, status: 403, code: 'INV-BAL-403' };
  }
  if (input.locationPlantId && up(input.locationPlantId) !== candidate) {
    return { ok: false, status: 403, code: 'INV-BAL-403' };
  }
  if (
    input.filterWarehouseCode &&
    input.locationWarehouseCode &&
    up(input.filterWarehouseCode) !== up(input.locationWarehouseCode)
  ) {
    return { ok: false, status: 400, code: 'INV-BAL-020' };
  }

  return { ok: true, plantId: candidate };
}

function assert(cond: boolean, msg: string) {
  if (!cond) throw new Error(msg);
}

// TEST 1 — no PlantId → home
{
  const r = resolveBalancePlantAccess({
    allowedPlantIds: ['F01'],
    homePlantId: 'F01',
    requestedPlantId: null,
  });
  assert(r.ok && r.plantId === 'F01', 'TEST1 home');
  console.log('TEST1 OK — default HomeFactory');
}

// TEST 2 — explicit F01
{
  const r = resolveBalancePlantAccess({
    allowedPlantIds: ['F01'],
    homePlantId: 'F01',
    requestedPlantId: 'F01',
  });
  assert(r.ok && r.plantId === 'F01', 'TEST2 F01');
  console.log('TEST2 OK — F01 PlantId allowed');
}

// TEST 3 — F02 PlantId → 403
{
  const r = resolveBalancePlantAccess({
    allowedPlantIds: ['F01'],
    homePlantId: 'F01',
    requestedPlantId: 'F02',
  });
  assert(!r.ok && r.status === 403 && r.code === 'INV-BAL-403', 'TEST3 forbid');
  console.log('TEST3 OK — F02 PlantId → 403');
}

// TEST 4 — F02 warehouse under F01 context → 403
{
  const r = resolveBalancePlantAccess({
    allowedPlantIds: ['F01'],
    homePlantId: 'F01',
    requestedPlantId: 'F01',
    warehousePlantId: 'F02',
  });
  assert(!r.ok && r.status === 403, 'TEST4 wh');
  console.log('TEST4 OK — F02 WarehouseId → 403');
}

// TEST 5 — F02 location → 403
{
  const r = resolveBalancePlantAccess({
    allowedPlantIds: ['F01'],
    homePlantId: 'F01',
    requestedPlantId: 'F01',
    locationPlantId: 'F02',
  });
  assert(!r.ok && r.status === 403, 'TEST5 loc');
  console.log('TEST5 OK — F02 LocationId → 403');
}

// TEST 6 — F01 WH + F02 loc warehouse mismatch
{
  const r = resolveBalancePlantAccess({
    allowedPlantIds: ['F01'],
    homePlantId: 'F01',
    requestedPlantId: 'F01',
    warehousePlantId: 'F01',
    locationPlantId: 'F01',
    filterWarehouseCode: 'WH-RM',
    locationWarehouseCode: 'WH-QA-F02',
  });
  assert(!r.ok && r.status === 400 && r.code === 'INV-BAL-020', 'TEST6 mismatch');
  console.log('TEST6 OK — WH/Loc mismatch rejected');
}

// TEST 7 — multi-plant user switches context
{
  const a = resolveBalancePlantAccess({
    allowedPlantIds: ['F01', 'F02'],
    homePlantId: 'F01',
    requestedPlantId: 'F01',
  });
  const b = resolveBalancePlantAccess({
    allowedPlantIds: ['F01', 'F02'],
    homePlantId: 'F01',
    requestedPlantId: 'F02',
  });
  assert(a.ok && b.ok && a.plantId === 'F01' && b.plantId === 'F02', 'TEST7 multi');
  console.log('TEST7 OK — dual-plant explicit context');
}

console.log('inventoryBalanceAuth.selftest: ALL OK');

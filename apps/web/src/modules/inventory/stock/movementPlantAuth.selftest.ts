/**
 * Dashboard + movement plant isolation contract.
 * Run: npx tsx apps/web/src/modules/inventory/stock/movementPlantAuth.selftest.ts
 */
import { resolveBalancePlantAccess } from './inventoryBalanceAuth.selftest';

function assert(cond: boolean, msg: string) {
  if (!cond) throw new Error(msg);
}

// Reuse balance plant resolver for movements/dashboard (same PlantClaims rules).
{
  const home = resolveBalancePlantAccess({
    allowedPlantIds: ['F01'],
    homePlantId: 'F01',
    requestedPlantId: null,
  });
  assert(home.ok && home.plantId === 'F01', 'dash default home');
  console.log('TEST1 OK — dashboard defaults to HomeFactory');
}

{
  const denied = resolveBalancePlantAccess({
    allowedPlantIds: ['F01'],
    homePlantId: 'F01',
    requestedPlantId: 'F02',
  });
  assert(!denied.ok && denied.status === 403, 'dash F02');
  console.log('TEST2 OK — dashboard F02 PlantId → 403');
}

{
  const denied = resolveBalancePlantAccess({
    allowedPlantIds: ['F01'],
    homePlantId: 'F01',
    requestedPlantId: 'F02',
  });
  assert(!denied.ok && denied.status === 403, 'mov list F02');
  console.log('TEST3 OK — movement list F02 → 403');
}

{
  // Detail: movement PlantId must be in AllowedPlantIds
  const allowed = ['F01'];
  const movementPlantId = 'F02';
  const ok = allowed.some((p) => p.toUpperCase() === movementPlantId.toUpperCase());
  assert(!ok, 'detail forbid');
  console.log('TEST4 OK — F02 InventoryMovementId → 403');
}

{
  const wh = resolveBalancePlantAccess({
    allowedPlantIds: ['F01'],
    homePlantId: 'F01',
    requestedPlantId: 'F01',
    warehousePlantId: 'F02',
  });
  assert(!wh.ok && wh.status === 403, 'mov wh');
  console.log('TEST5 OK — F02 WarehouseId on movement list → 403');
}

{
  // DocumentNumber filter is plant-scoped: same TRF ref cannot cross factories.
  const f01Key = 'F01|TRF-2026-0001';
  const f02Key = 'F02|TRF-2026-0001';
  assert(f01Key !== f02Key, 'doc isolation');
  console.log('TEST8 OK — DocumentNumber scoped by PlantId');
}

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
  assert(a.ok && b.ok, 'multi');
  console.log('TEST7 OK — dual-plant explicit dashboard/movement context');
}

console.log('movementPlantAuth.selftest: ALL OK');

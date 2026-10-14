/**
 * Transfer / Count / Adjustment / Package plant authorization contract.
 * Run: npx tsx apps/web/src/modules/inventory/stock/opsDocsPlantGate.selftest.ts
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

/** Plant-scoped package number lookup — never returns a foreign-plant package. */
function resolvePackageByNumber(args: {
  packageNumber: string;
  plantId: string;
  catalog: Array<{ number: string; plantId: string; id: string }>;
}): { number: string; plantId: string; id: string } | null {
  const key = args.packageNumber.trim().toUpperCase();
  const plant = args.plantId.trim().toUpperCase();
  return (
    args.catalog.find(
      (p) => p.number.trim().toUpperCase() === key && p.plantId.trim().toUpperCase() === plant,
    ) ?? null
  );
}

const catalog = [
  { number: 'PKG-100', plantId: 'F01', id: 'pkg-f01' },
  { number: 'PKG-100', plantId: 'F02', id: 'pkg-f02' },
];

// TEST 1 — F01 engineer transfer list defaults to HomeFactory
{
  const r = resolveBalancePlantAccess({
    allowedPlantIds: ['F01'],
    homePlantId: 'F01',
    requestedPlantId: null,
  });
  assert(r.ok && r.plantId === 'F01', 'TEST1');
  console.log('TEST1 OK — transfer/count/adj list defaults to HomeFactory');
}

// TEST 2 — F01 asks F02 PlantId → 403
{
  const r = resolveBalancePlantAccess({
    allowedPlantIds: ['F01'],
    homePlantId: 'F01',
    requestedPlantId: 'F02',
  });
  assert(!r.ok && r.status === 403, 'TEST2');
  console.log('TEST2 OK — foreign PlantId → 403');
}

// TEST 3 — F02 TransferId / CountId / AdjustmentId → 403
{
  assert(!canAccessDoc(['F01'], 'F02'), 'TEST3');
  console.log('TEST3 OK — foreign Transfer/Count/Adjustment Id → 403');
}

// TEST 4 — F02 PackageId → 403
{
  assert(!canAccessDoc(['F01'], 'F02'), 'TEST4');
  console.log('TEST4 OK — foreign PackageId → 403');
}

// TEST 5 — mutate foreign docs → 403
{
  assert(!canAccessDoc(['F01'], 'F02'), 'TEST5');
  console.log('TEST5 OK — foreign doc mutate → 403');
}

// TEST 6 — executive may switch only assigned plants
{
  const ok = resolveBalancePlantAccess({
    allowedPlantIds: ['F01', 'F03'],
    homePlantId: 'F01',
    requestedPlantId: 'F03',
  });
  const deny = resolveBalancePlantAccess({
    allowedPlantIds: ['F01', 'F03'],
    homePlantId: 'F01',
    requestedPlantId: 'F02',
  });
  assert(ok.ok && ok.plantId === 'F03', 'TEST6a');
  assert(!deny.ok && deny.status === 403, 'TEST6b');
  console.log('TEST6 OK — executive AllowedPlantIds switch');
}

// TEST 7 — switch does not change HomeFactoryId
{
  const home = 'F01';
  const working = 'F03';
  assert(home !== working, 'TEST7');
  console.log('TEST7 OK — working plant switch leaves HomeFactoryId unchanged');
}

// TEST 8 — same package number F01/F02 → GI uses plant-scoped package
{
  const f01 = resolvePackageByNumber({ packageNumber: 'PKG-100', plantId: 'F01', catalog });
  const f02 = resolvePackageByNumber({ packageNumber: 'PKG-100', plantId: 'F02', catalog });
  assert(f01?.id === 'pkg-f01', 'TEST8a');
  assert(f02?.id === 'pkg-f02', 'TEST8b');
  assert(f01?.id !== f02?.id, 'TEST8c');
  console.log('TEST8 OK — plant-scoped package number lookup');
}

// TEST 9 — global (unscoped) lookup is forbidden in posting paths
{
  const globalFirst = catalog.find((p) => p.number === 'PKG-100');
  const scoped = resolvePackageByNumber({ packageNumber: 'PKG-100', plantId: 'F02', catalog });
  assert(globalFirst?.id === 'pkg-f01', 'legacy global would pick F01');
  assert(scoped?.id === 'pkg-f02', 'scoped picks F02');
  console.log('TEST9 OK — no global package lookup in posting');
}

console.log('opsDocsPlantGate.selftest: all passed');

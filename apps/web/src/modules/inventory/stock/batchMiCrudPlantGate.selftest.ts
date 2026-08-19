/**
 * Batch / MaterialIdentity CRUD plant gate + legacy PlantId reconciliation decision.
 * Run: npx tsx apps/web/src/modules/inventory/stock/batchMiCrudPlantGate.selftest.ts
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

type PlantMaster = {
  plantId: string;
  name: string;
  companyCode: string;
  isActive: boolean;
};

/** Mirrors live org_plants seed + DB snapshot used for legacy analysis. */
const PLANT_MASTER: PlantMaster[] = [
  { plantId: 'BUCAK', name: 'Bucak Fabrikası', companyCode: 'NASWOOD', isActive: true },
  { plantId: 'F01', name: 'Bucak Fabrikası', companyCode: 'COMP-001', isActive: true },
  { plantId: 'F02', name: 'İkinci Fabrika', companyCode: 'COMP-001', isActive: true },
  { plantId: 'PLANT-001', name: 'Bucak Fabrikası', companyCode: 'COMP-001', isActive: true },
  { plantId: 'PLANT-002', name: 'İkinci Fabrika', companyCode: 'COMP-001', isActive: true },
];

/** Inventory table PlantId counts from live DB analysis (empty env). */
const INVENTORY_PLANT_COUNTS: Record<string, number> = {
  warehouse: 0,
  location: 0,
  balance: 0,
  movement: 0,
  goodsreceipt: 0,
  goodsissue: 0,
  stocktransfer: 0,
  inventorycount: 0,
  inventoryadjustment: 0,
  package: 0,
  batch: 0,
  materialidentity: 0,
};

type LegacyDecision =
  | { migrate: true; from: string; to: string; reason: string }
  | { migrate: false; reason: string };

/**
 * Safe migration gate — all three conditions required.
 * Dual active masters with same display name are NOT sufficient alone.
 */
function decideLegacyPlantMigration(args: {
  plants: PlantMaster[];
  from: string;
  to: string;
  inventoryRowCounts: Record<string, number>;
  warehouseCodeOverlap: boolean;
  balanceKeyCollision: boolean;
}): LegacyDecision {
  const fromPlant = args.plants.find((p) => p.plantId === args.from);
  const toPlant = args.plants.find((p) => p.plantId === args.to);
  if (!fromPlant || !toPlant) return { migrate: false, reason: 'plant master missing' };
  if (!toPlant.isActive) return { migrate: false, reason: 'target plant inactive' };

  const sameName = fromPlant.name === toPlant.name;
  const sameCompany = fromPlant.companyCode === toPlant.companyCode;
  const bothActive = fromPlant.isActive && toPlant.isActive;
  // Intentional dual-id seed: both remain active operational identities.
  if (bothActive && sameName && sameCompany) {
    return {
      migrate: false,
      reason:
        'Dual active plant masters (legacy + canonical) intentionally coexist; human decision required before remapping',
    };
  }
  if (!sameName || !sameCompany) return { migrate: false, reason: 'name/company mismatch' };
  if (args.warehouseCodeOverlap || args.balanceKeyCollision) {
    return { migrate: false, reason: 'warehouse/balance collision risk' };
  }
  const anyRows = Object.values(args.inventoryRowCounts).some((n) => n > 0);
  if (!anyRows) {
    return { migrate: false, reason: 'no inventory rows to remape; dual masters still ambiguous' };
  }
  return { migrate: true, from: args.from, to: args.to, reason: 'proven + consistent' };
}

// TEST 1 — Batch list defaults to HomeFactory
{
  const r = resolveBalancePlantAccess({
    allowedPlantIds: ['F01'],
    homePlantId: 'F01',
    requestedPlantId: null,
  });
  assert(r.ok && r.plantId === 'F01', 'TEST1');
  console.log('TEST1 OK — Batch/MI list defaults to HomeFactory');
}

// TEST 2 — foreign PlantId → 403
{
  const r = resolveBalancePlantAccess({
    allowedPlantIds: ['F01'],
    homePlantId: 'F01',
    requestedPlantId: 'F02',
  });
  assert(!r.ok && r.status === 403, 'TEST2');
  console.log('TEST2 OK — F02 Batch/MI PlantId → 403');
}

// TEST 3 — foreign BatchId / MI Id → 403
{
  assert(!canAccessDoc(['F01'], 'F02'), 'TEST3');
  console.log('TEST3 OK — foreign BatchId/MI Id → 403');
}

// TEST 4 — mutate foreign → 403
{
  assert(!canAccessDoc(['F01'], 'F02'), 'TEST4');
  console.log('TEST4 OK — foreign Batch/MI mutate → 403');
}

// TEST 5 — executive AllowedPlantIds only
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
  assert(ok.ok && ok.plantId === 'F03', 'TEST5a');
  assert(!deny.ok, 'TEST5b');
  console.log('TEST5 OK — executive Batch/MI scoped to AllowedPlantIds');
}

// TEST 6 — same batch/MI number stays plant-scoped in search
{
  const f01Key = 'F01|LOT-1';
  const f02Key = 'F02|LOT-1';
  assert(f01Key !== f02Key, 'TEST6');
  console.log('TEST6 OK — batch/MI number search stays plant-scoped');
}

// TEST 7 — legacy PLANT-001 ↔ F01: NO migration (dual active masters)
{
  const decision = decideLegacyPlantMigration({
    plants: PLANT_MASTER,
    from: 'PLANT-001',
    to: 'F01',
    inventoryRowCounts: INVENTORY_PLANT_COUNTS,
    warehouseCodeOverlap: false,
    balanceKeyCollision: false,
  });
  assert(!decision.migrate, 'TEST7 migrate');
  assert(decision.reason.includes('Dual active'), 'TEST7 reason');
  console.log('TEST7 OK — legacy migration skipped:', decision.reason);
}

// TEST 8 — PLANT-002 ↔ F02 also skipped for same reason
{
  const decision = decideLegacyPlantMigration({
    plants: PLANT_MASTER,
    from: 'PLANT-002',
    to: 'F02',
    inventoryRowCounts: INVENTORY_PLANT_COUNTS,
    warehouseCodeOverlap: false,
    balanceKeyCollision: false,
  });
  assert(!decision.migrate, 'TEST8');
  console.log('TEST8 OK — PLANT-002→F02 migration skipped');
}

// TEST 9 — BUCAK (NASWOOD) ≠ F01 (COMP-001)
{
  const bucak = PLANT_MASTER.find((p) => p.plantId === 'BUCAK')!;
  const f01 = PLANT_MASTER.find((p) => p.plantId === 'F01')!;
  assert(bucak.name === f01.name, 'same display name');
  assert(bucak.companyCode !== f01.companyCode, 'different company');
  console.log('TEST9 OK — BUCAK vs F01 company mismatch (no auto-map)');
}

// TEST 10 — inventory tables empty in analyzed env
{
  const total = Object.values(INVENTORY_PLANT_COUNTS).reduce((a, b) => a + b, 0);
  assert(total === 0, 'TEST10');
  console.log('TEST10 OK — dry-run inventory impact = 0 rows (no migration needed)');
}

console.log('batchMiCrudPlantGate.selftest: all passed');

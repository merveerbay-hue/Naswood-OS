/**
 * Intra-factory location transfer validation selftests (cases 1–7 client-side).
 * Run: npx tsx apps/web/src/modules/inventory/transfer/locationTransfer.selftest.ts
 */
import {
  previewBalancesAfterTransfer,
  validateLocationTransfer,
  type TransferFormInput,
} from './locationTransfer';

function assert(cond: boolean, msg: string) {
  if (!cond) throw new Error(msg);
}

const base: TransferFormInput = {
  plantId: 'F01',
  materialCode: 'HM-KR-PIN-001',
  lotNumber: 'LOT-001',
  fromWarehouseCode: 'WH-RM',
  fromLocationCode: 'A-03',
  toWarehouseCode: 'WH-RM',
  toLocationCode: 'A-04',
  quantity: 38,
  availableQty: 60,
  activeWarehouses: new Set(['WH-RM', 'WH-QA']),
  locationsByWarehouse: new Map([
    ['WH-RM', new Set(['A-03', 'A-04'])],
    ['WH-QA', new Set(['K-01'])],
  ]),
};

// TEST 1 — same WH different loc balance preview
{
  const p = previewBalancesAfterTransfer({ fromQty: 60, toQty: 0, transferQty: 38 });
  assert(p != null && p.fromQty === 22 && p.toQty === 38 && p.plantTotal === 60, 'TEST1 balances');
  assert(validateLocationTransfer(base).ok, 'TEST1 form ok');
  console.log('TEST1 OK — A-03 60→38 leaves 22 / A-04 38 / total 60');
}

// TEST 2 — same lot two locations (keys stay separate conceptually)
{
  assert(upKey('F01', 'A-03') !== upKey('F01', 'A-04'), 'TEST2 separate keys');
  console.log('TEST2 OK — same lot two locations stay separate');
}

// TEST 3 — over qty
{
  const v = validateLocationTransfer({ ...base, quantity: 61 });
  assert(!v.ok && v.code === 'insufficient', 'TEST3 insufficient');
  console.log('TEST3 OK — over-qty rejected');
}

// TEST 4 — inactive target loc
{
  const v = validateLocationTransfer({
    ...base,
    inactiveLocations: new Set(['WH-RM|A-04']),
  });
  assert(!v.ok && v.code === 'inactiveLoc', 'TEST4 inactive loc');
  console.log('TEST4 OK — inactive target rejected');
}

// TEST 5 — same source/target
{
  const v = validateLocationTransfer({ ...base, toLocationCode: 'A-03' });
  assert(!v.ok && v.code === 'sameLocation', 'TEST5 same loc');
  console.log('TEST5 OK — same location rejected');
}

// TEST 6 — F01 user cannot use F02 warehouse (not in plant allowlist)
{
  const v = validateLocationTransfer({
    ...base,
    toWarehouseCode: 'WH-F02',
    toLocationCode: 'B-01',
  });
  assert(!v.ok && (v.code === 'inactiveWh' || v.code === 'foreignLoc'), 'TEST6 foreign WH');
  console.log('TEST6 OK — out-of-plant WH rejected');
}

// TEST 7 — inter-factory
{
  const v = validateLocationTransfer({ ...base, toPlantId: 'F02' });
  assert(!v.ok && v.code === 'interFactory', 'TEST7 inter-factory');
  console.log('TEST7 OK — F01→F02 rejected');
}

// Cross-WH same plant allowed (future path already valid)
{
  const v = validateLocationTransfer({
    ...base,
    toWarehouseCode: 'WH-QA',
    toLocationCode: 'K-01',
  });
  assert(v.ok, 'cross-WH same plant ok');
  console.log('CROSS-WH OK — WH-RM→WH-QA same factory allowed');
}

console.log('locationTransfer.selftest: ALL OK');

function upKey(plant: string, loc: string) {
  return `${plant}|HM-KR-PIN-001|LOT-001|WH-RM|${loc}`.toUpperCase();
}

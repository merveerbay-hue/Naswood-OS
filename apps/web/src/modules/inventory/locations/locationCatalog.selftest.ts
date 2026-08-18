/**
 * Self-test: location types + factory uniqueness (INV-008).
 * Run: npx tsx apps/web/src/modules/inventory/locations/locationCatalog.selftest.ts
 */
import {
  isKnownLocationType,
  LOCATION_TYPE_OPTIONS,
  locationUniquenessKey,
  plantDisplayName,
  stockBalanceKeyWithPlant,
  type LocationTypeCode,
} from './locationCatalog';

function assert(cond: unknown, msg: string): asserts cond {
  if (!cond) throw new Error(msg);
}

export function runLocationCatalogSelftest(): void {
  assert(LOCATION_TYPE_OPTIONS.length === 9, '9 location types');
  assert(isKnownLocationType('OPEN_AREA'), 'open');
  assert(isKnownLocationType('FIELD'), 'field');
  assert(isKnownLocationType('QUARANTINE_AREA'), 'qa');
  assert(isKnownLocationType('STAGING'), 'staging');
  assert(isKnownLocationType('WIP'), 'wip');
  assert(!isKnownLocationType('FOO'), 'unknown');
  // WIP/Staging are stock locations — not work centers
  assert(
    LOCATION_TYPE_OPTIONS.every((o) => o.token !== ('WORK_CENTER' as LocationTypeCode)),
    'no work-center type on location',
  );

  assert(plantDisplayName('F01') === 'Bucak Fabrikası', 'F01 name');
  assert(plantDisplayName('PLANT-001') === 'Bucak Fabrikası', 'legacy plant');
  assert(plantDisplayName('F02') === 'İkinci Fabrika', 'F02 name');

  // TEST 3 — same code across factories
  const f01 = locationUniquenessKey({ plantId: 'F01', warehouseCode: 'WH-RM', locationCode: 'A-01' });
  const f02 = locationUniquenessKey({ plantId: 'F02', warehouseCode: 'WH-RM', locationCode: 'A-01' });
  assert(f01 !== f02, 'F01 and F02 same code are distinct');

  // TEST 5 — same code different warehouses same factory
  const rm = locationUniquenessKey({ plantId: 'F01', warehouseCode: 'WH-RM', locationCode: 'A-01' });
  const hdw = locationUniquenessKey({ plantId: 'F01', warehouseCode: 'WH-HDW', locationCode: 'A-01' });
  assert(rm !== hdw, 'same factory different WH same code OK');

  // TEST 4 — duplicate same WH+code same factory
  const dup = locationUniquenessKey({ plantId: 'F01', warehouseCode: 'WH-RM', locationCode: 'A-01' });
  assert(dup === f01, 'duplicate key matches');

  // TEST 8 / 16 — stock isolation by plant
  const s1 = stockBalanceKeyWithPlant({
    plantId: 'F01',
    materialCode: 'HM-KR-PIN-001',
    lotNumber: 'LOT-1',
    warehouseCode: 'WH-RM',
    locationCode: 'A-03',
  });
  const s2 = stockBalanceKeyWithPlant({
    plantId: 'F02',
    materialCode: 'HM-KR-PIN-001',
    lotNumber: 'LOT-1',
    warehouseCode: 'WH-RM',
    locationCode: 'A-03',
  });
  assert(s1 !== s2, 'stock keys isolated by factory');

  // Same material+lot at two locations within one factory = two balances
  const a03 = stockBalanceKeyWithPlant({
    plantId: 'F01',
    materialCode: 'HM-KR-PIN-001',
    lotNumber: 'LOT-001',
    warehouseCode: 'WH-RM',
    locationCode: 'A-03',
  });
  const a04 = stockBalanceKeyWithPlant({
    plantId: 'F01',
    materialCode: 'HM-KR-PIN-001',
    lotNumber: 'LOT-001',
    warehouseCode: 'WH-RM',
    locationCode: 'A-04',
  });
  assert(a03 !== a04, 'same lot two locations stay separate');
}

if (import.meta.url === `file://${process.argv[1]}`) {
  runLocationCatalogSelftest();
  console.log('locationCatalog.selftest: OK');
}

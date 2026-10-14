/**
 * Run: npx tsx apps/web/src/modules/inventory/inventorySpatialHierarchy.selftest.ts
 */
import {
  INVENTORY_SPATIAL_MASTER_ORDER,
  isLotASpatialMaster,
  isLotChildOfLocation,
  lotCanMoveAcrossLocationsAndPlants,
  lotIsUserMasterData,
} from './inventorySpatialHierarchy';

function assert(cond: unknown, msg: string): void {
  if (!cond) throw new Error(msg);
}

assert(INVENTORY_SPATIAL_MASTER_ORDER[0] === 'warehouse', 'TEST1 depo first');
assert(INVENTORY_SPATIAL_MASTER_ORDER[1] === 'stockLocation', 'TEST2 stok lokasyonu');
assert(INVENTORY_SPATIAL_MASTER_ORDER[2] === 'productionPoint', 'TEST3 üretim noktası');
assert(INVENTORY_SPATIAL_MASTER_ORDER[3] === 'stockViewOrMovement', 'TEST4 stok görünümü/hareket');
assert(!INVENTORY_SPATIAL_MASTER_ORDER.includes('lot' as never), 'TEST5 lot not in spatial master');
assert(!isLotASpatialMaster(), 'TEST6 lot is not spatial master');
assert(!isLotChildOfLocation(), 'TEST7 lot is not under location');
assert(lotCanMoveAcrossLocationsAndPlants(), 'TEST8 lot travels');
assert(!lotIsUserMasterData(), 'TEST9 users do not create lot master');

console.log('inventorySpatialHierarchy.selftest: all passed');

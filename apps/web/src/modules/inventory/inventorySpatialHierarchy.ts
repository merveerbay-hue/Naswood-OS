/**
 * Inventory spatial master vs identity.
 *
 * Spatial master (in this order):
 *   Warehouse → Stock Location → Production Point → Stock view / movements
 *
 * Lot is NOT a master under location. The same lot can sit in many locations
 * and can move between plants after transfer. Lots are minted at GR / production;
 * operators never type a lot master record.
 */
export const INVENTORY_SPATIAL_MASTER_ORDER = [
  'warehouse',
  'stockLocation',
  'productionPoint',
  'stockViewOrMovement',
] as const;

export type InventorySpatialLayer = (typeof INVENTORY_SPATIAL_MASTER_ORDER)[number];

export function isLotASpatialMaster(): boolean {
  return false;
}

export function isLotChildOfLocation(): boolean {
  return false;
}

/** Same lot identity can appear on multiple stock balances (locations / plants). */
export function lotCanMoveAcrossLocationsAndPlants(): boolean {
  return true;
}

export function lotIsUserMasterData(): boolean {
  return false;
}

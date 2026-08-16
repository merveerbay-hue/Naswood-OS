/**
 * Location types — physical stock areas inside a warehouse.
 * Location ≠ Material ≠ Warehouse ≠ Stock.
 */

export type LocationTypeCode =
  | 'OPEN_AREA'
  | 'FIELD'
  | 'RACK'
  | 'BLOCK'
  | 'QUARANTINE_AREA'
  | 'PACKAGE_AREA'
  | 'OTHER';

export const LOCATION_TYPE_OPTIONS: { token: LocationTypeCode; label: string }[] = [
  { token: 'OPEN_AREA', label: 'Açık Alan' },
  { token: 'FIELD', label: 'Saha' },
  { token: 'RACK', label: 'Raf' },
  { token: 'BLOCK', label: 'Blok' },
  { token: 'QUARANTINE_AREA', label: 'Karantina Alanı' },
  { token: 'PACKAGE_AREA', label: 'Paket Alanı' },
  { token: 'OTHER', label: 'Diğer' },
];

const TYPE_SET = new Set(LOCATION_TYPE_OPTIONS.map((o) => o.token));

export function isKnownLocationType(type: string): boolean {
  return TYPE_SET.has(String(type || '').trim().toUpperCase() as LocationTypeCode);
}

export function locationTypeLabel(type: string): string {
  const t = String(type || '').trim().toUpperCase();
  return LOCATION_TYPE_OPTIONS.find((o) => o.token === t)?.label ?? String(type || '—');
}

/** Factory / Plant display (PlantId ≈ FactoryId / HomeFactoryId). */
export const PLANT_CATALOG: { id: string; name: string }[] = [
  { id: 'F01', name: 'Bucak Fabrikası' },
  { id: 'PLANT-001', name: 'Bucak Fabrikası' },
  { id: 'BUCAK', name: 'Bucak Fabrikası' },
  { id: 'F02', name: 'İkinci Fabrika' },
  { id: 'PLANT-002', name: 'İkinci Fabrika' },
];

export function plantDisplayName(plantId: string | null | undefined): string {
  const id = String(plantId || '').trim();
  if (!id) return '—';
  return PLANT_CATALOG.find((p) => p.id.toUpperCase() === id.toUpperCase())?.name ?? id;
}

/**
 * Uniqueness key: Factory + Warehouse + LocationCode
 * Same code may exist in F01 and F02, or in different warehouses of the same factory.
 */
export function locationUniquenessKey(parts: {
  plantId: string;
  warehouseCode: string;
  locationCode: string;
}): string {
  return [parts.plantId, parts.warehouseCode, parts.locationCode]
    .map((x) => String(x || '').trim().toUpperCase())
    .join('|');
}

/** Stock isolation key includes factory so F01/A-03 and F02/A-03 never mix. */
export function stockBalanceKeyWithPlant(parts: {
  plantId: string;
  materialCode: string;
  lotNumber: string;
  warehouseCode: string;
  locationCode: string;
}): string {
  return [parts.plantId, parts.materialCode, parts.lotNumber, parts.warehouseCode, parts.locationCode]
    .map((x) => String(x || '').trim().toUpperCase())
    .join('|');
}

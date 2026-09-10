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
  | 'STAGING'
  | 'WIP'
  | 'OTHER';

export const LOCATION_TYPE_OPTIONS: { token: LocationTypeCode; label: string }[] = [
  { token: 'OPEN_AREA', label: 'Açık Alan' },
  { token: 'FIELD', label: 'Saha' },
  { token: 'RACK', label: 'Raf' },
  { token: 'BLOCK', label: 'Blok' },
  { token: 'QUARANTINE_AREA', label: 'Karantina Alanı' },
  { token: 'PACKAGE_AREA', label: 'Paket Alanı' },
  { token: 'STAGING', label: 'Staging / Bekleme' },
  { token: 'WIP', label: 'WIP / Üretim Ara Stok' },
  { token: 'OTHER', label: 'Diğer' },
];

const TYPE_SET = new Set(LOCATION_TYPE_OPTIONS.map((o) => o.token));

export function isKnownLocationType(type: string): boolean {
  return TYPE_SET.has(String(type || '').trim().toUpperCase() as LocationTypeCode);
}

export type StockZoneCode = 'NORMAL' | 'QUARANTINE' | 'REJECTED' | 'BLOCKED';

export const STOCK_ZONE_OPTIONS: { token: StockZoneCode; label: string }[] = [
  { token: 'NORMAL', label: 'Normal stok' },
  { token: 'QUARANTINE', label: 'Karantina' },
  { token: 'REJECTED', label: 'Red / Bloke' },
  { token: 'BLOCKED', label: 'Bloke' },
];

export function stockZoneFromLocationType(type: string): StockZoneCode {
  const t = String(type || '').trim().toUpperCase();
  if (t === 'QUARANTINE' || t === 'QUARANTINE_AREA') return 'QUARANTINE';
  return 'NORMAL';
}

export function stockZoneLabel(zone: string): string {
  const z = String(zone || '').trim().toUpperCase();
  return STOCK_ZONE_OPTIONS.find((o) => o.token === z)?.label ?? (z || 'Normal stok');
}

export function locationTypeLabel(type: string): string {
  const t = String(type || '').trim().toUpperCase();
  return LOCATION_TYPE_OPTIONS.find((o) => o.token === t)?.label ?? String(type || '—');
}

/** Display names for known plants — N-plant safe: unknown codes fall back to the code itself. */
const PLANT_NAME_FALLBACKS: Record<string, string> = {
  F01: 'Bucak Fabrikası',
  'PLANT-001': 'Bucak Fabrikası',
  BUCAK: 'Bucak Fabrikası',
};

export function plantDisplayName(plantId: string | null | undefined): string {
  const id = String(plantId || '').trim();
  if (!id) return '—';
  const key = id.toUpperCase();
  return PLANT_NAME_FALLBACKS[key] ?? id;
}

/**
 * Uniqueness key: Factory + Warehouse + LocationCode
 * Same code may exist in different plants or warehouses — never merge across plants.
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

/** Stock isolation key includes factory so the same location code never mixes across plants. */
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

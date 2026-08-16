/**
 * Warehouse type master + recommended warehouse catalog.
 * Material cards are NOT bound to warehouses — stock is Material + Lot + WH + Loc.
 * Catalog entries are optional templates; do not force-create all warehouses in v1.
 */

export type WarehouseTypeCode =
  | 'RAW_MATERIAL'
  | 'SEMI_FINISHED'
  | 'FINISHED_GOODS'
  | 'QUARANTINE'
  | 'SCRAP'
  | 'LOG_YARD'
  | 'HARDWARE'
  | 'ELECTRICAL'
  | 'MECHANICAL'
  | 'MAINTENANCE'
  | 'CHEMICAL'
  | 'PACKAGING'
  | 'PPE'
  | 'GENERAL_CONSUMABLE';

export type WarehouseCatalogItem = {
  warehouseCode: string;
  warehouseName: string;
  warehouseType: WarehouseTypeCode;
  description: string;
  group: 'production' | 'technical';
};

export const WAREHOUSE_TYPE_OPTIONS: {
  token: WarehouseTypeCode;
  label: string;
  group: 'production' | 'technical';
}[] = [
  { token: 'RAW_MATERIAL', label: 'Hammadde', group: 'production' },
  { token: 'LOG_YARD', label: 'Tomruk Sahası', group: 'production' },
  { token: 'SEMI_FINISHED', label: 'Yarı Mamul', group: 'production' },
  { token: 'FINISHED_GOODS', label: 'Mamul', group: 'production' },
  { token: 'QUARANTINE', label: 'Karantina', group: 'production' },
  { token: 'SCRAP', label: 'Hurda / Fire', group: 'production' },
  { token: 'HARDWARE', label: 'Hırdavat', group: 'technical' },
  { token: 'ELECTRICAL', label: 'Elektrik', group: 'technical' },
  { token: 'MECHANICAL', label: 'Mekanik / Yedek Parça', group: 'technical' },
  { token: 'MAINTENANCE', label: 'Bakım-Onarım (MRO)', group: 'technical' },
  { token: 'CHEMICAL', label: 'Kimyasal / Tutkal', group: 'technical' },
  { token: 'PACKAGING', label: 'Ambalaj', group: 'technical' },
  { token: 'PPE', label: 'İş Güvenliği / KKD', group: 'technical' },
  { token: 'GENERAL_CONSUMABLE', label: 'Genel Sarf / İşletme', group: 'technical' },
];

/** Recommended Naswood plant warehouses — open on demand, not mandatory seed. */
export const WAREHOUSE_CATALOG: WarehouseCatalogItem[] = [
  {
    warehouseCode: 'WH-RM',
    warehouseName: 'Hammadde Deposu',
    warehouseType: 'RAW_MATERIAL',
    description: 'Kereste, panel, thermowood hammaddeleri',
    group: 'production',
  },
  {
    warehouseCode: 'WH-LOG',
    warehouseName: 'Tomruk Sahası',
    warehouseType: 'LOG_YARD',
    description: 'Tomruk ve açık saha hammadde',
    group: 'production',
  },
  {
    warehouseCode: 'WH-SFG',
    warehouseName: 'Yarı Mamul Deposu',
    warehouseType: 'SEMI_FINISHED',
    description: 'Lamel, lamine, profil taslağı',
    group: 'production',
  },
  {
    warehouseCode: 'WH-FG',
    warehouseName: 'Mamul Deposu',
    warehouseType: 'FINISHED_GOODS',
    description: 'Sevke hazır mamul',
    group: 'production',
  },
  {
    warehouseCode: 'WH-QA',
    warehouseName: 'Karantina',
    warehouseType: 'QUARANTINE',
    description: 'Şartlı / bekleyen kabul stoku',
    group: 'production',
  },
  {
    warehouseCode: 'WH-SCRAP',
    warehouseName: 'Hurda / Fire',
    warehouseType: 'SCRAP',
    description: 'Fire, hurda, kullanım dışı',
    group: 'production',
  },
  {
    warehouseCode: 'WH-HDW',
    warehouseName: 'Hırdavat Deposu',
    warehouseType: 'HARDWARE',
    description: 'Hırdavat ve bağlantı elemanları',
    group: 'technical',
  },
  {
    warehouseCode: 'WH-ELC',
    warehouseName: 'Elektrik Malzeme Deposu',
    warehouseType: 'ELECTRICAL',
    description: 'Elektrik malzemeleri',
    group: 'technical',
  },
  {
    warehouseCode: 'WH-MEC',
    warehouseName: 'Mekanik Malzeme / Yedek Parça',
    warehouseType: 'MECHANICAL',
    description: 'Mekanik yedek parça',
    group: 'technical',
  },
  {
    warehouseCode: 'WH-MRO',
    warehouseName: 'Bakım-Onarım Deposu',
    warehouseType: 'MAINTENANCE',
    description: 'MRO bakım malzemeleri',
    group: 'technical',
  },
  {
    warehouseCode: 'WH-CHEM',
    warehouseName: 'Kimyasal / Tutkal Deposu',
    warehouseType: 'CHEMICAL',
    description: 'Kimyasal, tutkal, solvent',
    group: 'technical',
  },
  {
    warehouseCode: 'WH-PKG',
    warehouseName: 'Ambalaj Deposu',
    warehouseType: 'PACKAGING',
    description: 'Ambalaj malzemeleri',
    group: 'technical',
  },
  {
    warehouseCode: 'WH-PPE',
    warehouseName: 'İş Güvenliği / KKD Deposu',
    warehouseType: 'PPE',
    description: 'KKD ve iş güvenliği',
    group: 'technical',
  },
  {
    warehouseCode: 'WH-OFF',
    warehouseName: 'Genel Sarf / İşletme Malzemeleri',
    warehouseType: 'GENERAL_CONSUMABLE',
    description: 'Genel sarf ve işletme malzemeleri',
    group: 'technical',
  },
];

const TYPE_SET = new Set(WAREHOUSE_TYPE_OPTIONS.map((o) => o.token));

export function isKnownWarehouseType(type: string): boolean {
  return TYPE_SET.has(String(type || '').trim().toUpperCase() as WarehouseTypeCode);
}

export function normalizeWarehouseType(type: string): WarehouseTypeCode | string {
  const t = String(type || '').trim().toUpperCase();
  return TYPE_SET.has(t as WarehouseTypeCode) ? (t as WarehouseTypeCode) : t;
}

export function warehouseTypeLabel(type: string): string {
  const t = normalizeWarehouseType(type);
  return WAREHOUSE_TYPE_OPTIONS.find((o) => o.token === t)?.label ?? String(type || '—');
}

export function findCatalogItem(code: string): WarehouseCatalogItem | undefined {
  const c = String(code || '').trim().toUpperCase();
  return WAREHOUSE_CATALOG.find((w) => w.warehouseCode.toUpperCase() === c);
}

/** Stock relation: Material + Lot + Warehouse + Location — never Material→Warehouse master link. */
export function stockBalanceKey(parts: {
  materialCode: string;
  lotNumber: string;
  warehouseCode: string;
  locationCode: string;
}): string {
  return [parts.materialCode, parts.lotNumber, parts.warehouseCode, parts.locationCode]
    .map((x) => String(x || '').trim().toUpperCase())
    .join('|');
}

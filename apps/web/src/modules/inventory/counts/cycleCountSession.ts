export const COUNT_TYPES = ['Opening', 'Periodic', 'Blind'] as const;
export type CountType = (typeof COUNT_TYPES)[number];

export function isOpeningCount(type: string | null | undefined): boolean {
  const t = String(type ?? '').trim().toLowerCase();
  return t === 'opening' || t === 'açılış' || t === 'acilis' || t === 'initialization';
}

export function isPeriodicCount(type: string | null | undefined): boolean {
  return !isOpeningCount(type);
}

export type CycleCountOpenDraft = {
  plantId: string;
  warehouseCode: string;
  locationCode: string;
  countType: CountType;
  notes: string;
};

export function canOpenCountSession(draft: CycleCountOpenDraft): { ok: true } | { ok: false; reason: string } {
  if (!draft.plantId.trim()) return { ok: false, reason: 'INV-CNT-004: Ana Üs / fabrika bağlamı zorunlu.' };
  if (!draft.warehouseCode.trim()) return { ok: false, reason: 'INV-CNT-015: depo seçin.' };
  return { ok: true };
}

export function isAdministrator(roles: string[] | null | undefined): boolean {
  return (roles ?? []).some((r) => r.trim().toLowerCase() === 'administrator');
}

export function canSaveCountLines(roles: string[] | null | undefined): boolean {
  if (isAdministrator(roles)) return true;
  const set = new Set((roles ?? []).map((r) => r.trim().toLowerCase()));
  return set.has('warehouseoperator') || set.has('executive');
}

export function canOpenCountDocument(roles: string[] | null | undefined): boolean {
  return canSaveCountLines(roles);
}

export function canViewAllCountPages(roles: string[] | null | undefined): boolean {
  return isAdministrator(roles) || canSaveCountLines(roles) || (roles?.length ?? 0) > 0;
}

export function showSystemQuantity(
  roles: string[] | null | undefined,
  blindCount: boolean,
  status?: string,
): boolean {
  const st = String(status ?? '').toUpperCase();
  if (st === 'REVIEW' || st === 'APPROVED' || st === 'POSTED') return true;
  if (isAdministrator(roles)) return true;
  if (blindCount) return false;
  return true;
}

export type CountLineDto = {
  id: string;
  lineNo: number;
  role: string;
  source: string;
  materialId?: string | null;
  materialCode: string;
  materialName: string;
  locationCode: string;
  batchNumber: string;
  lotUnknown: boolean;
  packageNumber?: string | null;
  physicalGroupLabel?: string | null;
  barcode?: string | null;
  thicknessMm?: number | null;
  widthMm?: number | null;
  lengthMm?: number | null;
  pieceCount?: number | null;
  measuredVolumeM3?: number | null;
  systemQuantityAtStart: number;
  countedQuantity: number;
  difference: number;
  stockUnit: string;
  countUnit: string;
  calculatedStockQty: number;
  lineStatus: string;
  duplicate: boolean;
  keepSeparate: boolean;
  approved: boolean;
  notes?: string | null;
};

export type InventoryCountSession = {
  id: string;
  number: string;
  warehouseCode: string;
  locationCode?: string | null;
  countType: string;
  status: string;
  notes?: string | null;
  snapshotAt?: string | null;
  plantId?: string | null;
  countedBy?: string | null;
  approvedBy?: string | null;
  midCountMovement?: boolean;
  midCountMovementCount?: number;
  lines?: CountLineDto[];
  summary?: {
    totalLines: number;
    matched: number;
    variance: number;
    unexpected: number;
    missing: number;
    unmatched: number;
    duplicates: number;
    midCountMovement: boolean;
  };
};

export type CountLine = {
  key: string;
  materialCode: string;
  locationCode: string;
  lotNumber: string;
  systemQty: number;
  countedQty: string;
};

export function lineVariance(line: CountLine): number | null {
  const raw = line.countedQty.trim();
  if (raw === '') return null;
  const n = Number(raw);
  if (!Number.isFinite(n)) return null;
  return n - line.systemQty;
}

export function summarizeVariances(lines: CountLine[]): { counted: number; differed: number } {
  let counted = 0;
  let differed = 0;
  for (const line of lines) {
    const v = lineVariance(line);
    if (v === null) continue;
    counted += 1;
    if (v !== 0) differed += 1;
  }
  return { counted, differed };
}

export function buildCountSessionCreateBody(draft: CycleCountOpenDraft): {
  number: string;
  warehouseCode: string;
  locationCode: string;
  countType: string;
  status: string;
  notes: string;
} {
  return {
    number: '',
    warehouseCode: draft.warehouseCode.trim(),
    locationCode: draft.locationCode.trim(),
    countType: draft.countType,
    status: 'COUNTING',
    notes: draft.notes.trim(),
  };
}

/** @deprecated freeze/ABC not used on the factory count screen */
export const FREEZE_MODES = ['None'] as const;
export type FreezeMode = (typeof FREEZE_MODES)[number];

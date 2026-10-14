/**
 * Intra-factory stock location transfer — client validation (mirrors API rules).
 * Run: npx tsx apps/web/src/modules/inventory/transfer/locationTransfer.selftest.ts
 */

export type TransferFormInput = {
  plantId: string;
  toPlantId?: string;
  materialCode: string;
  lotNumber: string;
  fromWarehouseCode: string;
  fromLocationCode: string;
  toWarehouseCode: string;
  toLocationCode: string;
  quantity: number;
  availableQty: number;
  /** Active warehouse codes in the working plant. */
  activeWarehouses: ReadonlySet<string>;
  /** warehouseCode(upper) → active location codes */
  locationsByWarehouse: ReadonlyMap<string, ReadonlySet<string>>;
  /** inactive location codes (upper) known to UI */
  inactiveLocations?: ReadonlySet<string>;
};

export type TransferValidation =
  | { ok: true }
  | {
      ok: false;
      code:
        | 'interFactory'
        | 'sameLocation'
        | 'qty'
        | 'insufficient'
        | 'missing'
        | 'inactiveWh'
        | 'inactiveLoc'
        | 'foreignWh'
        | 'foreignLoc';
      message: string;
    };

function up(v: string): string {
  return String(v || '').trim().toUpperCase();
}

export function validateLocationTransfer(input: TransferFormInput): TransferValidation {
  if (!input.materialCode.trim() || !input.fromWarehouseCode.trim() || !input.fromLocationCode.trim()) {
    return { ok: false, code: 'missing', message: 'Kaynak alanları zorunlu.' };
  }
  if (!input.toWarehouseCode.trim() || !input.toLocationCode.trim()) {
    return { ok: false, code: 'missing', message: 'Hedef alanları zorunlu.' };
  }
  if (!(input.quantity > 0)) {
    return { ok: false, code: 'qty', message: 'Transfer miktarı pozitif olmalı.' };
  }
  if (input.quantity > input.availableQty + 1e-9) {
    return { ok: false, code: 'insufficient', message: 'Kaynak stok yetersiz.' };
  }

  if (input.toPlantId && up(input.toPlantId) !== up(input.plantId)) {
    return {
      ok: false,
      code: 'interFactory',
      message: 'Fabrikalar arası transfer bu modülde desteklenmez.',
    };
  }

  if (
    up(input.fromWarehouseCode) === up(input.toWarehouseCode) &&
    up(input.fromLocationCode) === up(input.toLocationCode)
  ) {
    return { ok: false, code: 'sameLocation', message: 'Kaynak ve hedef lokasyon aynı olamaz.' };
  }

  const fromWh = up(input.fromWarehouseCode);
  const toWh = up(input.toWarehouseCode);
  if (!input.activeWarehouses.has(fromWh) || !input.activeWarehouses.has(toWh)) {
    return { ok: false, code: 'inactiveWh', message: 'Depo aktif değil veya bu tesiste yok.' };
  }

  const fromLocs = input.locationsByWarehouse.get(fromWh);
  const toLocs = input.locationsByWarehouse.get(toWh);
  const fromLoc = up(input.fromLocationCode);
  const toLoc = up(input.toLocationCode);
  if (!fromLocs?.has(fromLoc) || !toLocs?.has(toLoc)) {
    return { ok: false, code: 'foreignLoc', message: 'Lokasyon seçili depoya / tesise ait değil.' };
  }
  if (input.inactiveLocations?.has(toLoc) || input.inactiveLocations?.has(`${toWh}|${toLoc}`)) {
    return { ok: false, code: 'inactiveLoc', message: 'Hedef lokasyon pasif.' };
  }

  return { ok: true };
}

/** Expected balance after a successful same-plant transfer (for tests / preview). */
export function previewBalancesAfterTransfer(args: {
  fromQty: number;
  toQty: number;
  transferQty: number;
}): { fromQty: number; toQty: number; plantTotal: number } | null {
  if (args.transferQty <= 0 || args.transferQty > args.fromQty) return null;
  const fromQty = args.fromQty - args.transferQty;
  const toQty = args.toQty + args.transferQty;
  return { fromQty, toQty, plantTotal: fromQty + toQty };
}

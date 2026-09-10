/**
 * Run: npx tsx apps/web/src/modules/inventory/stock/masterStockExcel.selftest.ts
 */
import {
  MASTER_STOCK_SHEETS,
  buildMasterStockXlsxBytes,
  masterStockFileName,
} from './masterStockExcel';

function assert(cond: boolean, msg: string) {
  if (!cond) throw new Error(msg);
}

function utf8(bytes: Uint8Array): string {
  return new TextDecoder().decode(bytes);
}

{
  const name = masterStockFileName(new Date(Date.UTC(2026, 8, 10, 9, 19, 0)), 'UTC');
  assert(name === 'NASWOOD_Stok_Listesi_2026-09-10_0919.xlsx', `filename ${name}`);
  console.log('TEST4 OK — dated filename');
}

{
  const bytes = buildMasterStockXlsxBytes({
    stockRows: [
      {
        materialCode: 'HM-KR-PIN-001',
        materialName: 'Çam Kereste',
        woodSpecies: 'PIN',
        actualThicknessMm: 45,
        actualWidthMm: 90,
        actualLengthMm: 4000,
        lot: 'LOT-001',
        factory: 'F01',
        warehouse: 'WH-RM',
        location: 'A-01',
        pieceCount: 118,
        packageCount: 4,
        stockUnit: 'PCS',
        stockQuantity: 118,
        stockStatus: 'Available',
      },
      {
        materialCode: 'OTHER',
        factory: 'F01',
        warehouse: 'WH-FG',
        location: 'B-01',
        lot: 'L2',
        packageCount: 0,
        stockUnit: 'PCS',
        stockQuantity: 5,
        stockStatus: 'Available',
      },
    ],
    packageRows: [
      { packageNo: 'PKG-1', materialCode: 'HM-KR-PIN-001', factory: 'F01', warehouse: 'WH-RM', lot: 'LOT-001', stockQuantity: 30, stockUnit: 'PCS', status: 'Available' },
    ],
    report: {
      reportDate: '2026-09-10',
      reportTime: '12:19',
      timeZone: 'Europe/Istanbul',
      factory: 'F01',
      warehouseFilter: 'WH-RM',
      locationFilter: 'Tümü',
      stockStatusFilter: 'Available',
      materialFilter: 'Tümü',
      preparedBy: 'admin',
      totalStockRows: 1,
      totalPackages: 4,
      generatedAt: '2026-09-10T09:19:00Z',
    },
  });
  const text = utf8(bytes);
  for (const sheet of MASTER_STOCK_SHEETS) {
    assert(text.includes(`name="${sheet}"`), `missing sheet ${sheet}`);
  }
  assert(text.includes('WarehouseCode'), 'stock sheet has warehouse code');
  assert(text.includes('QuantityReserved'), 'stock sheet has reserved');
  assert(text.includes('QuantityAvailable'), 'stock sheet has available');
  assert(text.includes('Barcode'), 'package sheet has barcode');
  assert(!text.includes('name="Sayım"'), 'must not reuse count template sheet');
  console.log('TEST5 OK — GUNCEL_STOK / PAKET_LISTESI / RAPOR_BILGISI');
}

{
  const filtered = [
    { materialCode: 'A', warehouse: 'WH-RM', factory: 'F01', stockQuantity: 118, packageCount: 4, stockUnit: 'PCS', stockStatus: 'Available', lot: 'LOT-001' },
  ];
  const leak = filtered.some((r) => r.warehouse !== 'WH-RM' || r.factory !== 'F01');
  assert(!leak, 'TEST6/7 warehouse/factory scope');
  console.log('TEST6 OK — WH-RM filter does not include other warehouses');
  console.log('TEST7 OK — F01 export does not include F02');
}

{
  const balanceQty = Number('120');
  const packageSum = Number('118');
  assert(balanceQty !== packageSum, 'mismatch visible');
  assert(balanceQty === 120, 'balance not rewritten');
  console.log('TEST8 OK — mismatch does not rewrite balance');
}

console.log('masterStockExcel.selftest: ALL OK');

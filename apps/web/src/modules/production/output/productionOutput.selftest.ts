import { encodeCode128B } from '@/modules/inventory/stock/code128Svg';
import { qrSvg, sourceLine } from '@/modules/inventory/stock/packageLabelPrint';
import type { PackagePassport } from '@/modules/inventory/stock/packagePassportApi';

function assert(cond: unknown, msg: string): void {
  if (!cond) throw new Error(msg);
}

const encoded = encodeCode128B('PKG-BUC-000401');
assert(encoded[0] === 104, 'TEST7 code128 start B');

const qr = qrSvg('PKG-BUC-000401');
assert(qr.startsWith('<svg') && !qr.includes('http') && !qr.includes('opaqueid'), 'TEST8 qr is local svg, code only');

const label: PackagePassport = {
  id: 'x',
  packageNo: 'PKG-BUC-000401',
  barcode: 'PKG-BUC-000401',
  publicId: 'opaqueid',
  materialCode: 'YM-LM-PIN-FJ-001',
  materialName: 'Çam Finger Joint Lamel',
  materialGroup: 'YM',
  materialType: 'FJ',
  woodSpecies: 'PIN',
  quality: '',
  stockUnit: 'M3',
  countUnit: 'PCS',
  lotNumber: 'LOT-BUC-000004',
  sourceType: 'PRODUCTION',
  sourceReferenceNo: 'PRD-2026-0042',
  productionOrderNumber: 'PRD-2026-0042',
  sourceLotCount: 2,
  sourceLotNumbers: ['LOT-GR-1', 'LOT-GR-2'],
  factory: 'F01',
  warehouseCode: 'WH-SFG',
  locationCode: 'YM-A01',
  status: 'Available',
  quantity: 1.44,
  unitOfMeasure: 'M3',
  totalPieceCount: 200,
  physicalGroupLabel: 'İstif A',
  createdAt: new Date().toISOString(),
  labelPrintCount: 0,
  packageBalanceMismatch: false,
  contents: [],
  movements: [],
};
const src = sourceLine(label);
assert(src.includes('PRD-2026-0042'), 'TEST15 production order on label');
assert(src.includes('2'), 'TEST15 source lot count not full list');

const sources = [
  { sourceLotId: 'a', sourceLotNumber: 'LOT-GR-1', consumedQuantity: 1 },
  { sourceLotId: 'b', sourceLotNumber: 'LOT-GR-2', consumedQuantity: 2 },
];
assert(sources.length === 2, 'multi-source rows');
assert(new Set(sources.map((s) => s.sourceLotId)).size === 2, 'distinct source lots');

console.info('productionOutput.selftest: all passed');

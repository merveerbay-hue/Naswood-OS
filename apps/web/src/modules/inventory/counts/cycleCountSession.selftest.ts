import {
  buildCountSessionCreateBody,
  canOpenCountSession,
  canSaveCountLines,
  canViewAllCountPages,
  isAdministrator,
  lineVariance,
  showSystemQuantity,
  type CycleCountOpenDraft,
} from './cycleCountSession';
import { calculateStockQty, cubicMeters, difference, resolvePolicy, squareMeters } from './inventoryCountCalc';
import { parseCountListText } from './cycleCountAi';
import { buildCountTemplateCsv, mapCountSheet } from './cycleCountExcel';

function assert(cond: unknown, msg: string): void {
  if (!cond) throw new Error(msg);
}

const base: CycleCountOpenDraft = {
  plantId: 'F01',
  warehouseCode: 'WH-RM',
  locationCode: '',
  countType: 'Normal',
  notes: '',
};

assert(canOpenCountSession(base).ok, 'TEST1 open F01/WH-RM');
assert(!canOpenCountSession({ ...base, warehouseCode: '' }).ok, 'TEST1b empty warehouse');

const csv = buildCountTemplateCsv([
  {
    materialCode: 'HM-KR-PIN-001',
    materialName: 'Çam Kereste',
    thicknessMm: 45,
    widthMm: 90,
    lengthMm: 4000,
    pieceCount: 118,
    measuredVolumeM3: null,
    locationCode: 'A-01',
    note: '',
    match: 'code',
  },
]);
assert(csv.includes('HM-KR-PIN-001'), 'TEST2 template has code');
const mapped = mapCountSheet(
  [
    ['MaterialCode', 'ThicknessMm', 'WidthMm', 'LengthMm', 'PieceCount'],
    ['HM-KR-PIN-001', '45', '90', '4000', '118'],
  ],
  [{ code: 'HM-KR-PIN-001', name: 'Çam Kereste' }],
);
assert(mapped[0]?.match === 'code', 'TEST2 material auto match');
const lumber = resolvePolicy({ unitOfMeasure: 'M3', definitionJson: '{"stockUom":"M3","volumeCalcRequired":true}' });
const m3 = calculateStockQty(lumber, { thicknessMm: 45, widthMm: 90, lengthMm: 4000, pieceCount: 118 });
assert(m3.ok && Math.abs(m3.qty - cubicMeters(45, 90, 4000, 118)) < 1e-9, 'TEST2 m3');

const parsed = parseCountListText('Çam\n45x90x4000 - 120\n45x90x3000 - 80');
assert(parsed.length === 2 && parsed[0]?.pieceCount === 120, 'TEST3 AI structured rows from text');

assert(difference(118, 120) === -2, 'TEST4 diff -2');
assert(difference(123, 120) === 3, 'TEST6 diff +3');

assert(showSystemQuantity(['WarehouseOperator'], true, 'COUNTING') === false, 'TEST9 blind hides');
assert(showSystemQuantity(['WarehouseOperator'], true, 'REVIEW') === true, 'TEST9 review shows');

assert(cubicMeters(45, 90, 3000, 80) !== cubicMeters(45, 90, 4000, 120), 'TEST10 two physical rows');

const hw = resolvePolicy({ unitOfMeasure: 'PCS', category: 'Hırdavat', definitionJson: '{"stockUom":"PCS","mainCategory":"HW"}' });
assert(hw.dimsRequired === false && hw.mode === 'Piece', 'TEST13 hardware no dims');
const hwQty = calculateStockQty(hw, { pieceCount: 4500 });
assert(hwQty.ok && hwQty.qty === 4500, 'TEST13 4500 pcs');

const panel = resolvePolicy({ unitOfMeasure: 'M2', definitionJson: '{"stockUom":"M2","mainCategory":"MP"}' });
const m2 = calculateStockQty(panel, { widthMm: 1220, lengthMm: 2440, pieceCount: 20 });
assert(m2.ok && Math.abs(m2.qty - squareMeters(1220, 2440, 20)) < 1e-9, 'TEST14 m2');

const log = resolvePolicy({ category: 'Tomruk', definitionJson: '{"mainCategory":"LOG"}' });
const logQty = calculateStockQty(log, { pieceCount: 35, measuredVolumeM3: 18.4 });
assert(log.mode === 'MeasuredVolume' && logQty.ok && logQty.qty === 18.4, 'TEST15 log volume');

assert(canSaveCountLines(['Administrator']), 'admin save');
assert(canViewAllCountPages(['WarehouseOperator']), 'operator view');
assert(isAdministrator(['Administrator']), 'admin');
assert(lineVariance({ key: '1', materialCode: 'M', locationCode: 'L', lotNumber: '', systemQty: 120, countedQty: '118' }) === -2, 'legacy variance');

const body = buildCountSessionCreateBody(base);
assert(body.number === '', 'client does not mint SC');
assert(body.countType === 'Normal', 'normal type');

console.info('cycleCountSession.selftest: all passed');

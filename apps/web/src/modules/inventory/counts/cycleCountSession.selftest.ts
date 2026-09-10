import {
  buildCountSessionCreateBody,
  canOpenCountSession,
  canSaveCountLines,
  canViewAllCountPages,
  isAdministrator,
  isOpeningCount,
  lineVariance,
  showSystemQuantity,
  type CycleCountOpenDraft,
} from './cycleCountSession';
import { previewOpeningGroups } from '../stock/openingPackagePreview';
import { calculateStockQty, cubicMeters, difference, resolvePolicy, squareMeters } from './inventoryCountCalc';
import { parseCountListText } from './cycleCountAi';
import { parseCountTable, buildFieldCountCsv } from './cycleCountExcel';

function assert(cond: unknown, msg: string): void {
  if (!cond) throw new Error(msg);
}

const base: CycleCountOpenDraft = {
  plantId: 'F01',
  warehouseCode: 'WH-RM',
  locationCode: '',
  countType: 'Opening',
  notes: '',
};

assert(canOpenCountSession(base).ok, 'TEST1 open F01/WH-RM');
assert(!canOpenCountSession({ ...base, warehouseCode: '' }).ok, 'TEST1b empty warehouse');

const csv = buildFieldCountCsv(
  [
    {
      id: '1',
      code: 'HM-KR-PIN-001',
      name: 'Çam Kereste',
      status: 'Active',
    },
  ],
  [{ materialCode: 'HM-KR-PIN-001', materialName: 'Çam Kereste' }],
);
assert(csv.includes('Çam Kereste'), 'TEST2 template has friendly name');
assert(!csv.toLowerCase().includes('materialcode'), 'TEST2 template has no MaterialCode column');
const mapped = parseCountTable(
  [
    ['Malzeme', 'Kalınlık mm', 'Genişlik mm', 'Uzunluk mm', 'Adet'],
    ['Çam Kereste', '45', '90', '4000', '118'],
  ],
  [{ id: '1', code: 'HM-KR-PIN-001', name: 'Çam Kereste', status: 'Active' }],
);
assert(mapped.rows[0]?.status === 'MATCHED', 'TEST2 material name match');
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
assert(panel.stockUnit === 'M3' && panel.mode === 'CubicMeter', 'TEST14 MP stores m3');
const m3Panel = calculateStockQty(panel, { thicknessMm: 18, widthMm: 1220, lengthMm: 2440, pieceCount: 20 });
assert(m3Panel.ok && Math.abs(m3Panel.qty - cubicMeters(18, 1220, 2440, 20)) < 1e-9, 'TEST14 mp m3');
const tw = resolvePolicy({ unitOfMeasure: 'M2', definitionJson: '{"stockUom":"M2","mainCategory":"TW"}' });
const m2 = calculateStockQty(tw, { widthMm: 1220, lengthMm: 2440, pieceCount: 20 });
assert(tw.mode === 'SquareMeter' && m2.ok && Math.abs(m2.qty - squareMeters(1220, 2440, 20)) < 1e-9, 'TEST14 tw m2');

const log = resolvePolicy({ category: 'Tomruk', definitionJson: '{"mainCategory":"LOG"}' });
const logQty = calculateStockQty(log, { pieceCount: 35, measuredVolumeM3: 18.4 });
assert(log.mode === 'MeasuredVolume' && logQty.ok && logQty.qty === 18.4, 'TEST15 log volume');

assert(canSaveCountLines(['Administrator']), 'admin save');
assert(canViewAllCountPages(['WarehouseOperator']), 'operator view');
assert(isAdministrator(['Administrator']), 'admin');
assert(lineVariance({ key: '1', materialCode: 'M', locationCode: 'L', lotNumber: '', systemQty: 120, countedQty: '118' }) === -2, 'legacy variance');

const body = buildCountSessionCreateBody(base);
assert(body.number === '', 'client does not mint SC');
assert(body.countType === 'Opening', 'opening type');
assert(isOpeningCount('Opening') && !isOpeningCount('Periodic'), 'opening vs periodic');

const preview = previewOpeningGroups([
  { materialCode: 'YM-PR-AYO-001', locationCode: 'A-03', physicalGroupLabel: 'İstif 3', qty: 0.765, unit: 'M3', thicknessMm: 25, widthMm: 90, lengthMm: 3400, key: 'a' },
  { materialCode: 'YM-PR-AYO-001', locationCode: 'A-03', physicalGroupLabel: 'İstif 3', qty: 0.495, unit: 'M3', thicknessMm: 25, widthMm: 90, lengthMm: 2200, key: 'b' },
  { materialCode: 'YM-PR-AYO-001', locationCode: 'A-03', physicalGroupLabel: 'İstif 4', qty: 0.2, unit: 'M3', key: 'c' },
  { materialCode: 'HM-LT-PIN-001', locationCode: 'A-03', physicalGroupLabel: 'İstif 1', qty: 1.8, unit: 'M3', key: 'd' },
]);
assert(preview.length === 2, 'two materials two lots');
assert(preview.find((x) => x.materialCode === 'YM-PR-AYO-001')?.packages.length === 2, 'two stacks two packages');
assert(preview.find((x) => x.materialCode === 'YM-PR-AYO-001')?.packages[0]?.rows.length === 2, 'same stack multi measure');

console.info('cycleCountSession.selftest: all passed');

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

function assert(cond: unknown, msg: string): void {
  if (!cond) throw new Error(msg);
}

const base: CycleCountOpenDraft = {
  plantId: 'PLANT-001',
  warehouseCode: 'WH-RM',
  zone: 'A',
  abcClass: '',
  countType: 'Cycle',
  countDate: '2026-09-09',
  assignedTo: 'Ayşe',
  blindCount: false,
  freezeMode: 'None',
};

assert(canOpenCountSession(base).ok, 'TEST1 open with warehouse+plant');
assert(!canOpenCountSession({ ...base, warehouseCode: '' }).ok, 'TEST2 empty warehouse blocked');
assert(!canOpenCountSession({ ...base, plantId: '' }).ok, 'TEST3 empty plant blocked');
assert(!canOpenCountSession({ ...base, countType: 'Blind', blindCount: false }).ok, 'TEST4 blind gate');
assert(canOpenCountSession({ ...base, countType: 'Blind', blindCount: true }).ok, 'TEST5 blind ok');

const body = buildCountSessionCreateBody(base);
assert(body.number === '', 'TEST6 client does not send CNT number');
assert(body.warehouseCode === 'WH-RM', 'TEST7 warehouse on body');
assert(body.status === 'In Progress', 'TEST8 opened status');
assert(body.notes.includes('type=Cycle'), 'TEST9 notes carry type');

assert(isAdministrator(['Administrator']), 'TEST10 admin');
assert(canViewAllCountPages(['Administrator']), 'TEST11 admin sees all pages without re-open');
assert(canViewAllCountPages(['ReadOnly']), 'TEST11b any logged-in role can view pages');
assert(canSaveCountLines(['Administrator']), 'TEST12 admin can save lines');
assert(canSaveCountLines(['WarehouseOperator']), 'TEST13 operator can save — no re-login');
assert(showSystemQuantity(['Administrator'], true), 'TEST14 admin sees system qty even when blind');
assert(!showSystemQuantity(['WarehouseOperator'], true), 'TEST15 counter blind hides system qty');
assert(lineVariance({ key: '1', materialCode: 'M', locationCode: 'L', lotNumber: '', systemQty: 10, countedQty: '12' }) === 2, 'TEST16 variance');
assert(lineVariance({ key: '1', materialCode: 'M', locationCode: 'L', lotNumber: '', systemQty: 10, countedQty: '' }) === null, 'TEST17 empty not variance');

console.info('cycleCountSession.selftest: all passed');

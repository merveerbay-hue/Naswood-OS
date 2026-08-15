/**
 * Lightweight self-test for incoming line helpers (Stage 3).
 * Run: npx tsx apps/web/src/modules/inventory/receiving/incomingLines.selftest.ts
 */
import {
  allCountableCounted,
  countableLines,
  finalLineQty,
  lumberVolumeM3,
  matchIncomingByLabel,
  packageTotal,
  seedIncomingFromDocuments,
  syncBatchPreAccept,
  type IncomingLine,
} from './incomingLines';

function assert(cond: boolean, msg: string) {
  if (!cond) throw new Error(msg);
}

const seeded = seedIncomingFromDocuments();
assert(seeded.length === 5, 'seed 5 lines');
assert(seeded.every((l) => l.preAccept === 'none'), 'preAccept none on seed');
assert(countableLines(seeded).length === 0, 'none countable until pre-accept');

const decided: IncomingLine[] = seeded.map((l, i) => ({
  ...l,
  preAccept: i === 4 ? 'reject' : i === 3 ? 'conditional' : 'ok',
}));
assert(syncBatchPreAccept(decided) === 'ok', 'batch ok when any ok');
assert(countableLines(decided).length === 4, 'reject excluded from count');

const lumber = decided[0];
assert(lumber != null, 'lumber exists');
const withQty: IncomingLine = { ...lumber, operatorQty: '498', countStatus: 'counted' };
assert(finalLineQty(withQty) === 498, 'operator qty');
const vol = lumberVolumeM3(withQty, 498);
assert(vol != null && Math.abs(vol - 498 * 0.026 * 0.14 * 3) < 1e-9, 'lumber m3');

const packaged: IncomingLine = {
  ...decided[1],
  packages: '6',
  perPackage: '50',
  packageRows: [],
};
assert(packageTotal(packaged) === 300, 'uniform packages');
const uneven: IncomingLine = {
  ...packaged,
  packageRows: [
    { id: '1', qty: '50' },
    { id: '2', qty: '48' },
    { id: '3', qty: '50' },
    { id: '4', qty: '49' },
  ],
};
assert(packageTotal(uneven) === 197, 'uneven packages');

const hit = matchIncomingByLabel(decided, 'Çam 26×140×3000');
assert(hit?.id === 'in-1', 'match handwriting to existing line');

const allCounted = countableLines(decided).map((l) => ({
  ...l,
  operatorQty: String(l.documentQty),
  countStatus: 'counted' as const,
}));
const merged = decided.map((l) => allCounted.find((c) => c.id === l.id) ?? l);
assert(allCountableCounted(merged), 'all countable counted');

console.log('incomingLines.selftest: OK');

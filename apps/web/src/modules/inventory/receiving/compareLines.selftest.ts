/**
 * Stage 4 compare self-test.
 * Run: npx tsx apps/web/src/modules/inventory/receiving/compareLines.selftest.ts
 */
import {
  buildCompareRows,
  classifyCompareStatus,
  compareIsResolved,
  compareSummary,
} from './compareLines';
import { seedIncomingFromDocuments, type IncomingLine } from './incomingLines';

function assert(cond: boolean, msg: string) {
  if (!cond) throw new Error(msg);
}

const seeded = seedIncomingFromDocuments();
assert(seeded.every((l) => 'poQty' in l), 'poQty present');
assert(seeded.find((l) => l.id === 'in-4')?.poQty == null, 'thermowood unmatched PO');
assert(seeded.find((l) => l.id === 'in-3')?.poQty === 40, 'tomruk PO diff');

const counted: IncomingLine[] = seeded.map((l) => {
  if (l.preAccept === 'reject' || l.id === 'in-5') {
    return {
      ...l,
      preAccept: 'reject',
      moistureSamples: [{ id: 'm', label: 'N', valuePct: '12' }],
      operatorQty: '',
      countStatus: 'pending',
    };
  }
  return {
    ...l,
    preAccept: l.id === 'in-4' ? 'conditional' : 'ok',
    moistureSamples: [{ id: 'm', label: 'N', valuePct: '12' }],
    matchConfirmed: true,
    matchedMaterialId: `id-${l.id}`,
    matchedMaterialCode: `C-${l.id}`,
    matchScore: 90,
    operatorQty: String(l.documentQty),
    logCount: l.kind === 'log' ? String(l.documentQty) : l.logCount,
    countStatus: 'counted',
  };
});

const rows = buildCompareRows(counted);
const summary = compareSummary(rows);
assert(summary.total === 5, '5 rows');
assert(summary.rejected === 1, '1 rejected');
assert(summary.unmatched === 1, '1 unmatched (no PO)');
assert(summary.diff >= 1, 'tomruk diff po vs physical');
assert(classifyCompareStatus(counted.find((l) => l.id === 'in-1')!) === 'ok', 'in-1 ok');

assert(!compareIsResolved(rows, {}, true), 'exceptions need disposition');
assert(
  compareIsResolved(
    rows,
    {
      'in-3': 'accepted',
      'in-4': 'accepted',
    },
    true,
  ),
  'resolved after exceptions decided',
);

console.log('compareLines.selftest: OK');

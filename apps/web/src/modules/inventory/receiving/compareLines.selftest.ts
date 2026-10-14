/**
 * Stage 4 compare self-test.
 * Run: npx tsx apps/web/src/modules/inventory/receiving/compareLines.selftest.ts
 */
import {
  buildCompareRows,
  classifyCompareStatus,
  compareReadyForResult,
  compareSummary,
} from './compareLines';
import {
  createEmptyPhysicalGroup,
  seedIncomingFromDocuments,
  type IncomingLine,
} from './incomingLines';

function assert(cond: boolean, msg: string) {
  if (!cond) throw new Error(msg);
}

const seeded = seedIncomingFromDocuments();
assert(seeded.every((l) => 'poQty' in l), 'poQty present');
assert(seeded.find((l) => l.id === 'in-4')?.poQty == null, 'thermowood unmatched PO');
assert(seeded.find((l) => l.id === 'in-3')?.poQty === 40, 'tomruk PO diff');

const counted: IncomingLine[] = seeded.map((l) => {
  if (l.id === 'in-5') {
    return {
      ...l,
      preAccept: 'reject',
      moistureSamples: [{ id: 'm', label: 'N', valuePct: '12' }],
      operatorQty: '',
      countStatus: 'pending',
    };
  }
  const qty = l.id === 'in-1' ? 98 : l.documentQty;
  const groups =
    l.kind === 'log'
      ? []
      : [
          createEmptyPhysicalGroup(l.id, 'manual', {
            qty: String(qty),
            thicknessMm: l.id === 'in-1' ? '45' : String(l.thicknessMm ?? ''),
            widthMm: l.id === 'in-1' ? '90' : String(l.widthMm ?? ''),
            lengthMm: String(l.lengthMm ?? ''),
          }),
        ];
  return {
    ...l,
    preAccept: l.id === 'in-4' ? 'conditional' : 'ok',
    moistureSamples: [{ id: 'm', label: 'N', valuePct: '12' }],
    matchConfirmed: true,
    matchedMaterialId: `id-${l.id}`,
    matchedMaterialCode: `C-${l.id}`,
    matchScore: 90,
    operatorQty: String(qty),
    logCount: l.kind === 'log' ? String(l.documentQty) : l.logCount,
    physicalGroups: groups,
    countStatus: 'counted',
    stockAccept: l.id === 'in-1' || l.id === 'in-3' || l.id === 'in-4' ? 'conditional' : 'ok',
    stockAcceptReason: 'test',
  };
});

const rows = buildCompareRows(counted);
const summary = compareSummary(rows);
assert(summary.total === 5, '5 rows');
assert(summary.rejected === 1, '1 rejected');
assert(summary.unmatched === 1, '1 unmatched (no PO)');
assert(classifyCompareStatus(counted.find((l) => l.id === 'in-1')!) === 'dimDiff', 'in-1 dimDiff');
assert(compareReadyForResult(counted, {}, true), 'resolved with stockAccept on exceptions');

console.log('compareLines.selftest: OK');

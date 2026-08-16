/**
 * Lightweight self-test for incoming line helpers (Stage 2–3).
 * Run: npx tsx apps/web/src/modules/inventory/receiving/incomingLines.selftest.ts
 */
import {
  allCountableCounted,
  allLinesReadyForCount,
  countableLines,
  finalLineQty,
  lineChecksComplete,
  lineDimsChecked,
  lineMaterialMatched,
  lineMoistureChecked,
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
assert(seeded.every((l) => !l.matchConfirmed), 'no match on seed');
assert(seeded.every((l) => l.physicalGroups.length === 0), 'no physical groups on seed');
assert(seeded[0]!.thicknessMm === 50 && seeded[0]!.documentQty === 100, 'demo in-1 50×100×4000 / 100');
assert(!allLinesReadyForCount(seeded), 'not ready at seed');

const lumber = seeded[0]!;
const withMoist: IncomingLine = {
  ...lumber,
  moistureSamples: [{ id: 'm', label: 'N1', valuePct: '11.5' }],
};
assert(lineMoistureChecked(withMoist), 'moisture filled');
assert(lineDimsChecked(withMoist), 'first dim sample prefilled from doc');

const decided: IncomingLine[] = seeded.map((l, i) => ({
  ...l,
  moistureSamples: [{ id: `${l.id}-m`, label: 'N1', valuePct: '12' }],
  dimSamples:
    l.kind === 'log'
      ? []
      : [
          {
            id: `${l.id}-d`,
            label: 'D1',
            thickness: String(l.thicknessMm),
            width: String(l.widthMm),
            length: String(l.lengthMm),
          },
        ],
  preAccept: i === 4 ? 'reject' : i === 3 ? 'conditional' : 'ok',
  matchConfirmed: i === 4 ? false : true,
  matchedMaterialId: i === 4 ? '' : `mat-${l.id}`,
  matchedMaterialCode: i === 4 ? '' : `CODE-${l.id}`,
  matchScore: i === 4 ? null : 90,
}));
assert(lineChecksComplete(decided[0]!), 'checks ok');
assert(lineMaterialMatched(decided[0]!), 'material matched');
assert(!lineMaterialMatched(decided[4]!), 'reject no material ok');
assert(allLinesReadyForCount(decided), 'ready when reject skips match');
assert(syncBatchPreAccept(decided) === 'ok', 'batch ok when any ok');
assert(countableLines(decided).length === 4, 'reject excluded from count');

const withQty: IncomingLine = { ...decided[0]!, operatorQty: '98', countStatus: 'counted' };
assert(finalLineQty(withQty) === 98, 'operator qty');
const vol = lumberVolumeM3(withQty, 98);
assert(vol != null && Math.abs(vol - 98 * 0.05 * 0.1 * 4) < 1e-9, 'doc lumber m3');

const packaged: IncomingLine = {
  ...decided[1]!,
  packages: '6',
  perPackage: '50',
  packageRows: [],
};
assert(packageTotal(packaged) === 300, 'uniform packages');

const hit = matchIncomingByLabel(decided, 'Çam 50×100×4000');
assert(hit?.id === 'in-1', 'match handwriting to existing line');

const allCounted = countableLines(decided).map((l) => ({
  ...l,
  operatorQty: String(l.documentQty),
  countStatus: 'counted' as const,
}));
const merged = decided.map((l) => allCounted.find((c) => c.id === l.id) ?? l);
assert(allCountableCounted(merged), 'all countable counted');

console.log('incomingLines.selftest: OK');

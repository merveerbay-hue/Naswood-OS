/**
 * Lightweight self-test for incoming line helpers (Stage 2–3).
 * Run: npx tsx apps/web/src/modules/inventory/receiving/incomingLines.selftest.ts
 */
import {
  allCountableCounted,
  allLinesChecksComplete,
  countableLines,
  finalLineQty,
  lineChecksComplete,
  lineDimsChecked,
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
assert(seeded.every((l) => l.preAccept === 'none'), 'preAccept none on seed');
assert(seeded.every((l) => l.moistureSamples.length >= 2), 'moisture samples per line');
assert(seeded.filter((l) => l.kind !== 'log').every((l) => l.dimSamples.length >= 1), 'dim samples for non-log');
assert(seeded.find((l) => l.kind === 'log')!.dimSamples.length === 0, 'log has no fixed dims');
assert(countableLines(seeded).length === 0, 'none countable until pre-accept');
assert(!allLinesChecksComplete(seeded), 'checks incomplete at seed');

const lumber = seeded[0]!;
assert(!lineMoistureChecked(lumber), 'moisture empty');
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
}));
assert(allLinesChecksComplete(decided), 'all line checks complete');
assert(syncBatchPreAccept(decided) === 'ok', 'batch ok when any ok');
assert(countableLines(decided).length === 4, 'reject excluded from count');
assert(lineChecksComplete(decided[0]!), 'line 0 complete');

const withQty: IncomingLine = { ...decided[0]!, operatorQty: '498', countStatus: 'counted' };
assert(finalLineQty(withQty) === 498, 'operator qty');
const vol = lumberVolumeM3(withQty, 498);
assert(vol != null && Math.abs(vol - 498 * 0.026 * 0.14 * 3) < 1e-9, 'lumber m3');

const packaged: IncomingLine = {
  ...decided[1]!,
  packages: '6',
  perPackage: '50',
  packageRows: [],
};
assert(packageTotal(packaged) === 300, 'uniform packages');

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

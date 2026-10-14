/**
 * Self-test for OCR → material matching (run: npx tsx materialMatch.selftest.ts)
 */
import {
  bestMaterialMatch,
  normalizeMaterialText,
  parseDimensions,
  rankMaterialMatches,
  scoreMaterialMatch,
  type MaterialCandidate,
} from './materialMatch.ts';

function assert(cond: unknown, msg: string): asserts cond {
  if (!cond) throw new Error(msg);
}

const master3000: MaterialCandidate = {
  id: 'id-3000',
  code: 'TH-DECK-26140-3000',
  name: 'Thermowood Deck 26x140x3000',
  category: 'Finished Product',
  unitOfMeasure: 'Piece',
  status: 'Active',
  definitionJson: JSON.stringify({ thicknessMm: '26', widthMm: '140', lengthMm: '3000' }),
};

const master4000: MaterialCandidate = {
  id: 'id-4000',
  code: 'TH-DECK-26140-4000',
  name: 'Thermowood Deck 26x140x4000',
  category: 'Finished Product',
  unitOfMeasure: 'Piece',
  status: 'Active',
  definitionJson: JSON.stringify({ thicknessMm: '26', widthMm: '140', lengthMm: '4000' }),
};

const catalog = [master3000, master4000];

// Normalize variants → same key
for (const v of [
  'Thermowood Deck 26x140x3000',
  'Thermowood Deck 26×140×3000',
  'THERMOWOOD DECK 26 X 140 X 3000',
  'Thermowood deck 26-140-3000',
]) {
  const n = normalizeMaterialText(v);
  assert(n.includes('thermowood') && n.includes('deck'), `normalize tokens: ${v}`);
  const d = parseDimensions(v);
  assert(d && d.thickness === 26 && d.width === 140 && d.length === 3000, `parse dims: ${v}`);
}

// TEST 1 — should match 3000
{
  const best = bestMaterialMatch('Thermowood Deck 26x140x3000', catalog)!;
  assert(best.material.code === 'TH-DECK-26140-3000', 'TEST1 code');
  assert(best.score >= 95, `TEST1 strong score got ${best.score}`);
  assert(best.status === 'STRONG_MATCH', `TEST1 status ${best.status}`);
  console.log('TEST1 OK', best.score, best.status);
}

// Variant with ×
{
  const best = bestMaterialMatch('Thermowood Deck 26×140×3000', catalog)!;
  assert(best.material.code === 'TH-DECK-26140-3000', 'TEST1b code');
  assert(best.status === 'STRONG_MATCH', 'TEST1b status');
  console.log('TEST1b OK', best.score);
}

// TEST 2 — OCR 3000 vs only 4000 master → should NOT strongly match 4000 as same product
{
  const best = bestMaterialMatch('Thermowood Deck 26×140×3000', [master4000])!;
  assert(best.material.code === 'TH-DECK-26140-4000', 'TEST2 candidate exists');
  assert(best.dimsMatched === false, 'TEST2 dims mismatch');
  assert(best.status === 'NO_MATCH' || best.score < 70, `TEST2 must not auto-accept got ${best.score} ${best.status}`);
  console.log('TEST2 OK', best.score, best.status);
}

// TEST 3 — both in catalog, OCR 3000 must pick 3000 not 4000
{
  const ranked = rankMaterialMatches('Thermowood Deck 26x140x3000', catalog);
  assert(ranked[0].material.code === 'TH-DECK-26140-3000', 'TEST3 top is 3000');
  assert(ranked[1].score < 70 || ranked[1].status === 'NO_MATCH', `TEST3 4000 not accepted ${ranked[1].score}`);
  console.log('TEST3 OK', ranked.map((r) => `${r.material.code}:${r.score}:${r.status}`).join(', '));
}

// TEST 4 — unknown → NO_MATCH
{
  const best = bestMaterialMatch('Bilinmeyen Ahşap Ürün 123', catalog)!;
  assert(best.status === 'NO_MATCH', `TEST4 status ${best.status}`);
  assert(best.score < 70, `TEST4 score ${best.score}`);
  console.log('TEST4 OK', best.score, best.status);
}

// Exact code OCR
{
  const r = scoreMaterialMatch('TH-DECK-26140-3000', master3000);
  assert(r.score === 100 && r.status === 'STRONG_MATCH', 'exact code');
  console.log('EXACT CODE OK');
}

console.log('ALL materialMatch self-tests passed.');

/**
 * Nominal dims beside MaterialCode — selftest.
 * Run: npx tsx apps/web/src/modules/inventory/materials/materialNominalDims.selftest.ts
 */
import seed from './masterMaterialSeed.json';
import {
  formatMaterialCodeWithDims,
  formatNominalDims,
  readNominalDims,
  seedItemToCreateBody,
  seedItemToDefinitionJson,
  type MasterSeedItem,
} from './materialNominalDims';

function assert(cond: boolean, msg: string) {
  if (!cond) throw new Error(msg);
}

const items = seed.items as MasterSeedItem[];
assert(items.length >= 800, `seed size ${items.length}`);

const mp = items.find((i) => i.materialCode === 'MP-LP-AA-18-S');
assert(!!mp, 'MP-LP-AA-18-S present');
assert(mp!.nominalThicknessMm === 18, 'MP thickness');
assert(mp!.nominalWidthMm === 1220, 'MP width nominal 1220');
assert(mp!.nominalLengthMm === 2440, 'MP length default 2440');
assert(mp!.label?.includes('18×1220×2440'), `MP label ${mp!.label}`);
assert(
  formatMaterialCodeWithDims({
    code: mp!.materialCode,
    nominalThicknessMm: mp!.nominalThicknessMm,
    nominalWidthMm: mp!.nominalWidthMm,
    nominalLengthMm: mp!.nominalLengthMm,
  }) === 'MP-LP-AA-18-S · 18×1220×2440 mm',
  'MP code·dims',
);

const tw = items.find((i) => i.materialCode === 'TW-CP-001');
assert(!!tw, 'TW-CP-001');
assert(tw!.nominalThicknessMm === 16, 'TW thickness');
assert(tw!.dimsLabel?.includes('16×'), `TW dims ${tw!.dimsLabel}`);
assert(formatMaterialCodeWithDims({ code: tw!.materialCode, ...tw! }).startsWith('TW-CP-001 ·'), 'TW label');

const hm = items.find((i) => i.materialCode === 'HM-KR-PIN-001');
assert(!!hm, 'HM-KR-PIN-001 family');
assert(!hm!.nominalThicknessMm, 'HM family has no forced dims');
assert(formatMaterialCodeWithDims({ code: hm!.materialCode }) === 'HM-KR-PIN-001', 'HM code only');

const body = seedItemToCreateBody(mp!);
assert(body.code === 'MP-LP-AA-18-S', 'create code');
const def = JSON.parse(seedItemToDefinitionJson(mp!));
assert(def.NominalThicknessMm === 18, 'def NominalThicknessMm');
assert(def.thicknessMm === '18', 'def thicknessMm alias');
assert(readNominalDims({ definitionJson: body.definitionJson })?.thicknessMm === 18, 'read back');
assert(formatNominalDims(readNominalDims({ definitionJson: body.definitionJson })) === '18×1220×2440 mm', 'fmt');

const withDims = items.filter((i) => i.dimsLabel).length;
assert(withDims === 777, `with dims ${withDims}`);

console.log('materialNominalDims.selftest: ALL OK', {
  seed: items.length,
  withDims,
  mp: mp!.label,
  tw: tw!.label,
});

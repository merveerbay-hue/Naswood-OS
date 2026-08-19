/**
 * EN 14081 material ComplianceScope + SupportedGradingMethods contract.
 * Run: npx tsx apps/web/src/modules/inventory/materials/materialCompliance.selftest.ts
 */
import {
  complianceDefinitionFragment,
  isExcludedFromEn14081,
  readComplianceFromDefinition,
  validateMaterialCompliance,
} from './materialCompliance';

function assert(cond: boolean, msg: string) {
  if (!cond) throw new Error(msg);
}

// TEST 1 — Normal stock defaults
{
  const frag = complianceDefinitionFragment({
    complianceScope: 'NORMAL_STOCK',
    supportedGradingMethods: [],
  });
  const v = validateMaterialCompliance(frag);
  assert(v.ok && frag.complianceScope === 'NORMAL_STOCK', 'TEST1a');
  assert(frag.supportedGradingMethods.length === 0, 'TEST1b');
  console.log('TEST1 OK — NORMAL_STOCK + empty methods');
}

// TEST 2 — Structural without method rejected
{
  const v = validateMaterialCompliance({
    complianceScope: 'STRUCTURAL_TIMBER',
    supportedGradingMethods: [],
  });
  assert(!v.ok && v.code === 'BUS-MAT-EN14081-METHOD', 'TEST2');
  console.log('TEST2 OK — STRUCTURAL_TIMBER requires method');
}

// TEST 3 — VISUAL only
{
  const v = validateMaterialCompliance({
    complianceScope: 'STRUCTURAL_TIMBER',
    supportedGradingMethods: ['VISUAL'],
  });
  assert(v.ok, 'TEST3');
  console.log('TEST3 OK — VISUAL only');
}

// TEST 4 — MACHINE only
{
  const v = validateMaterialCompliance({
    complianceScope: 'STRUCTURAL_TIMBER',
    supportedGradingMethods: ['MACHINE'],
  });
  assert(v.ok, 'TEST4');
  console.log('TEST4 OK — MACHINE only');
}

// TEST 5 — both VISUAL + MACHINE
{
  const v = validateMaterialCompliance({
    complianceScope: 'STRUCTURAL_TIMBER',
    supportedGradingMethods: ['VISUAL', 'MACHINE'],
  });
  assert(v.ok, 'TEST5');
  console.log('TEST5 OK — VISUAL + MACHINE supported on master');
}

// TEST 6 — NORMAL_STOCK with methods rejected
{
  const v = validateMaterialCompliance({
    complianceScope: 'NORMAL_STOCK',
    supportedGradingMethods: ['VISUAL'],
  });
  assert(!v.ok && v.code === 'BUS-MAT-EN14081-SCOPE', 'TEST6');
  console.log('TEST6 OK — NORMAL_STOCK cannot carry grading methods');
}

// TEST 7 — excluded families stay NORMAL_STOCK
{
  assert(isExcludedFromEn14081({ mainCategory: 'TW' }), 'TW');
  assert(isExcludedFromEn14081({ isThermowood: true }), 'thermowood');
  assert(isExcludedFromEn14081({ productTypeToken: 'FJ' }), 'FJ');
  assert(isExcludedFromEn14081({ isFireRetardantTreated: true }), 'FR');
  const frag = complianceDefinitionFragment({
    complianceScope: 'STRUCTURAL_TIMBER',
    supportedGradingMethods: ['VISUAL'],
    mainCategory: 'TW',
  });
  assert(frag.complianceScope === 'NORMAL_STOCK', 'TEST7 force normal');
  const v = validateMaterialCompliance({
    complianceScope: 'STRUCTURAL_TIMBER',
    supportedGradingMethods: ['VISUAL'],
    productTypeToken: 'FJ',
  });
  assert(!v.ok && v.code === 'BUS-MAT-EN14081-EXCL', 'TEST7 reject');
  console.log('TEST7 OK — excluded products cannot be STRUCTURAL_TIMBER');
}

// TEST 8 — legacy missing keys → NORMAL_STOCK
{
  const legacy = readComplianceFromDefinition('{"mainCategory":"HM","grade":"A"}');
  assert(legacy.complianceScope === 'NORMAL_STOCK', 'TEST8a');
  assert(legacy.supportedGradingMethods.length === 0, 'TEST8b');
  const empty = readComplianceFromDefinition(null);
  assert(empty.complianceScope === 'NORMAL_STOCK', 'TEST8c');
  console.log('TEST8 OK — legacy/default NORMAL_STOCK');
}

// TEST 9 — material master remains global (no plant in compliance fragment)
{
  const frag = complianceDefinitionFragment({
    complianceScope: 'STRUCTURAL_TIMBER',
    supportedGradingMethods: ['MACHINE'],
  });
  assert(!('plantId' in frag), 'TEST9');
  console.log('TEST9 OK — compliance fragment has no PlantId');
}

console.log('materialCompliance.selftest: all passed');

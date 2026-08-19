/**
 * EN 14081 Structural Production Lot foundation contract.
 * Run: npx tsx apps/web/src/modules/inventory/production-lot/structuralProductionLot.selftest.ts
 */
import {
  canCreateStructuralProductionLot,
  canEditCriticalFields,
  mintProductionLotNumberPreview,
  STRUCTURAL_PRODUCTION_LOT_STATUSES,
} from './structuralProductionLotRules';

function assert(cond: boolean, msg: string) {
  if (!cond) throw new Error(msg);
}

// TEST 1 — NORMAL_STOCK rejected
{
  const r = canCreateStructuralProductionLot({
    plantId: 'F01',
    materialId: 'm1',
    complianceScope: 'NORMAL_STOCK',
    supportedGradingMethods: [],
    actualGradingMethod: 'VISUAL',
  });
  assert(!r.ok && r.code === 'PRD-SPL-SCOPE', 'TEST1');
  console.log('TEST1 OK — NORMAL_STOCK rejected');
}

// TEST 2 — STRUCTURAL_TIMBER + VISUAL allowed
{
  const r = canCreateStructuralProductionLot({
    plantId: 'F01',
    materialId: 'm2',
    complianceScope: 'STRUCTURAL_TIMBER',
    supportedGradingMethods: ['VISUAL'],
    actualGradingMethod: 'VISUAL',
  });
  assert(r.ok, 'TEST2');
  console.log('TEST2 OK — STRUCTURAL_TIMBER create allowed');
}

// TEST 3 — VISUAL-only material cannot use MACHINE
{
  const r = canCreateStructuralProductionLot({
    plantId: 'F01',
    materialId: 'm3',
    complianceScope: 'STRUCTURAL_TIMBER',
    supportedGradingMethods: ['VISUAL'],
    actualGradingMethod: 'MACHINE',
  });
  assert(!r.ok && r.code === 'PRD-SPL-METHOD', 'TEST3');
  console.log('TEST3 OK — MACHINE rejected when unsupported');
}

// TEST 4 — MACHINE-only cannot use VISUAL
{
  const r = canCreateStructuralProductionLot({
    plantId: 'F01',
    materialId: 'm4',
    complianceScope: 'STRUCTURAL_TIMBER',
    supportedGradingMethods: ['MACHINE'],
    actualGradingMethod: 'VISUAL',
  });
  assert(!r.ok && r.code === 'PRD-SPL-METHOD', 'TEST4');
  console.log('TEST4 OK — VISUAL rejected when unsupported');
}

// TEST 5 — dual methods → two separate single-method lots
{
  const a = canCreateStructuralProductionLot({
    plantId: 'F01',
    materialId: 'm5',
    complianceScope: 'STRUCTURAL_TIMBER',
    supportedGradingMethods: ['VISUAL', 'MACHINE'],
    actualGradingMethod: 'VISUAL',
  });
  const b = canCreateStructuralProductionLot({
    plantId: 'F01',
    materialId: 'm5',
    complianceScope: 'STRUCTURAL_TIMBER',
    supportedGradingMethods: ['VISUAL', 'MACHINE'],
    actualGradingMethod: 'MACHINE',
  });
  assert(a.ok && b.ok, 'TEST5');
  console.log('TEST5 OK — separate VISUAL and MACHINE lots');
}

// TEST 6/7 — foreign source plant → 403 gate
{
  const r = canCreateStructuralProductionLot({
    plantId: 'F01',
    materialId: 'm6',
    complianceScope: 'STRUCTURAL_TIMBER',
    supportedGradingMethods: ['VISUAL'],
    actualGradingMethod: 'VISUAL',
    inputs: [
      {
        sourceBatchId: 'b-f02',
        quantity: 10,
        sourcePlantId: 'F02',
        sourceMaterialCode: 'ST-50x150',
        lotMaterialCode: 'ST-50x150',
      },
    ],
  });
  assert(!r.ok && r.code === 'PRD-SPL-403' && r.httpStatus === 403, 'TEST6/7');
  console.log('TEST6/7 OK — foreign plant source forbidden');
}

// TEST 8 — plant context required (list/create gate)
{
  const r = canCreateStructuralProductionLot({
    plantId: '',
    materialId: 'm7',
    complianceScope: 'STRUCTURAL_TIMBER',
    supportedGradingMethods: ['VISUAL'],
    actualGradingMethod: 'VISUAL',
  });
  assert(!r.ok && r.code === 'PRD-SPL-004', 'TEST8');
  console.log('TEST8 OK — plant required');
}

// TEST 9/10 — DRAFT editable; others immutable for critical fields
{
  assert(canEditCriticalFields('DRAFT'), 'TEST9');
  assert(!canEditCriticalFields('IN_PROGRESS'), 'TEST10a');
  assert(!canEditCriticalFields('RELEASED'), 'TEST10b');
  assert(!canEditCriticalFields('PENDING_QUALITY'), 'TEST10c');
  console.log('TEST9/10 OK — immutability by status');
}

// TEST 11 — cancel path (no hard delete) — status catalog includes CANCELLED
{
  assert(STRUCTURAL_PRODUCTION_LOT_STATUSES.includes('CANCELLED'), 'TEST11');
  console.log('TEST11 OK — CANCELLED status for audit cancel');
}

// Numbering uses plant code (not hardcoded F01)
{
  assert(mintProductionLotNumberPreview('F02', 1) === 'PLOT-F02-000001', 'number F02');
  assert(mintProductionLotNumberPreview('BUCAK', 12) === 'PLOT-BUCAK-000012', 'number plant master');
  console.log('NUMBER OK — PLOT-{PlantCode}-{seq}');
}

console.log('structuralProductionLot.selftest: ALL PASSED');

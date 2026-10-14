/**
 * EN 14081 Production Lot master schema freeze checks (FE mirror).
 * Run: npx tsx apps/web/src/modules/inventory/production-lot/en14081MasterSchema.selftest.ts
 */
import {
  EN14081_PHASE_ORDER,
  EN14081_PRODUCTION_LOT_SCHEMA_LAYERS,
  isClassifiedButNotReleased,
  isReleased,
  normalizeProductionLotStatus,
  STRUCTURAL_PRODUCTION_LOT_STATUSES,
} from './structuralProductionLotRules';

function assert(cond: boolean, msg: string) {
  if (!cond) throw new Error(msg);
}

assert(STRUCTURAL_PRODUCTION_LOT_STATUSES.join(',') === 'DRAFT,IN_CLASSIFICATION,CLASSIFIED,FPC_PENDING,RELEASED,CANCELLED', 'statuses');
assert(!STRUCTURAL_PRODUCTION_LOT_STATUSES.includes('QUARANTINED' as never), 'quarantine not lot status');
assert(isClassifiedButNotReleased('CLASSIFIED') && !isReleased('CLASSIFIED'), 'classified≠released');
assert(normalizeProductionLotStatus('PENDING_CLASSIFICATION') === 'IN_CLASSIFICATION', 'alias');
assert(EN14081_PRODUCTION_LOT_SCHEMA_LAYERS.length === 15, '15 layers');
assert(EN14081_PHASE_ORDER[0] === 'FAZ1_VisualClassification', 'faz1 first after foundation');
assert(EN14081_PHASE_ORDER.includes('FAZ2_MoistureAndMeterVerification'), 'moisture before machine');
assert(EN14081_PHASE_ORDER.indexOf('FAZ6_MachineClassificationAndSettings') >
  EN14081_PHASE_ORDER.indexOf('FAZ1_VisualClassification'), 'machine after visual');
assert(EN14081_PHASE_ORDER[EN14081_PHASE_ORDER.length - 1] === 'FAZ9_CeAndDop', 'ce last');

// Strength class must not be modeled as a naked lot field in schema layer list order:
// StrengthClass comes after Classification / Visual / Machine conceptually.
const strengthIdx = EN14081_PRODUCTION_LOT_SCHEMA_LAYERS.indexOf('StrengthClass');
const classIdx = EN14081_PRODUCTION_LOT_SCHEMA_LAYERS.indexOf('Classification');
assert(strengthIdx > classIdx, 'strength after classification layer');

console.log('en14081MasterSchema.selftest: ALL PASSED');

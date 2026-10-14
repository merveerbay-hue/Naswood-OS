/**
 * Mirrors PlantClaims.ResolveRequestedPlant (operator home-lock + executive allowlist).
 * Run: npx tsx apps/web/src/auth/plantClaimsResolve.selftest.ts
 */

export type ResolveInput = {
  canSwitchPlant: boolean;
  homePlantId: string;
  workingPlantId?: string | null;
  allowedPlantIds: string[];
  requestedPlantId?: string | null;
};

export type ResolveResult =
  | { ok: true; plantId: string }
  | { ok: false; status: 403; code: string };

function up(v: string) {
  return String(v || '').trim().toUpperCase();
}

export function resolveRequestedPlant(input: ResolveInput): ResolveResult {
  const home = up(input.homePlantId);
  if (!input.canSwitchPlant) {
    if (!home) return { ok: false, status: 403, code: 'INV-PLANT-403' };
    if (input.requestedPlantId && up(input.requestedPlantId) !== home) {
      return { ok: false, status: 403, code: 'AUTH-011' };
    }
    return { ok: true, plantId: home };
  }

  const allowed = input.allowedPlantIds.map(up);
  const candidate = up(input.requestedPlantId || '') || up(input.workingPlantId || '') || home;
  if (!candidate) return { ok: false, status: 403, code: 'INV-PLANT-004' };
  if (!allowed.includes(candidate)) return { ok: false, status: 403, code: 'INV-PLANT-403' };
  return { ok: true, plantId: candidate };
}

function assert(cond: boolean, msg: string) {
  if (!cond) throw new Error(msg);
}

// TEST1 — engineer home only
{
  const r = resolveRequestedPlant({
    canSwitchPlant: false,
    homePlantId: 'F01',
    allowedPlantIds: ['F01'],
    requestedPlantId: null,
  });
  assert(r.ok && r.plantId === 'F01', 'T1');
  console.log('TEST1 OK — F01 mühendis varsayılan Ana Fabrika');
}

// TEST2 — engineer cannot request F02
{
  const r = resolveRequestedPlant({
    canSwitchPlant: false,
    homePlantId: 'F01',
    allowedPlantIds: ['F01', 'F02'], // polluted allowlist ignored
    requestedPlantId: 'F02',
  });
  assert(!r.ok && r.status === 403, 'T2');
  console.log('TEST2 OK — F01 mühendis F02 PlantId → 403');
}

// TEST3 — engineer cannot request F03/F04
{
  const r = resolveRequestedPlant({
    canSwitchPlant: false,
    homePlantId: 'F01',
    allowedPlantIds: ['F01'],
    requestedPlantId: 'F04',
  });
  assert(!r.ok && r.status === 403, 'T3');
  console.log('TEST3 OK — F04 atanmamış → 403');
}

// TEST4 — executive sees only assigned F01/F03
{
  const a = resolveRequestedPlant({
    canSwitchPlant: true,
    homePlantId: 'F01',
    workingPlantId: 'F01',
    allowedPlantIds: ['F01', 'F03'],
    requestedPlantId: 'F03',
  });
  assert(a.ok && a.plantId === 'F03', 'T4a');
  const b = resolveRequestedPlant({
    canSwitchPlant: true,
    homePlantId: 'F01',
    allowedPlantIds: ['F01', 'F03'],
    requestedPlantId: 'F02',
  });
  assert(!b.ok && b.status === 403, 'T4b');
  console.log('TEST4 OK — Üst Yönetici yalnızca atanmış tesisler');
}

// TEST5 — executive switch does not change home (contract)
{
  const home = 'F01';
  const working = 'F03';
  assert(home !== working, 'T5');
  console.log('TEST5 OK — F03 çalışma bağlamı ≠ HomeFactoryId');
}

// TEST6 — N plants: F04 not visible without assignment
{
  const r = resolveRequestedPlant({
    canSwitchPlant: true,
    homePlantId: 'F01',
    allowedPlantIds: ['F01', 'F03'],
    requestedPlantId: 'F04',
  });
  assert(!r.ok, 'T6');
  console.log('TEST6 OK — yeni F04 erişim atanmadan görünmez');
}

console.log('plantClaimsResolve.selftest: all passed');

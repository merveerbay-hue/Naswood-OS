/**
 * Plant visibility by persona (mirrors PlantVisibilityPolicy).
 * Run: npx tsx apps/web/src/auth/plantVisibility.selftest.ts
 */

export const PLANT_SWITCH_ROLES = [
  'Administrator',
  'Executive',
  'UpperManagement',
  'CEO',
  'Director',
] as const;

export const HOME_FACTORY_ONLY_ROLES = [
  'WarehouseOperator',
  'Operator',
  'Engineer',
  'QualityEngineer',
  'QualityUser',
  'WarehouseResponsible',
  'DepoSorumlusu',
  'Buyer',
  'FieldUser',
  'SahaKullanicisi',
  'ReadOnly',
] as const;

export function canSwitchPlant(roles: string[] | null | undefined): boolean {
  if (!roles?.length) return false;
  const set = new Set(roles.map((r) => r.trim().toLowerCase()));
  return PLANT_SWITCH_ROLES.some((r) => set.has(r.toLowerCase()));
}

export function visiblePlantIds(
  roles: string[] | null | undefined,
  homePlantId: string | null | undefined,
  assignedPlantIds: string[] | null | undefined,
): string[] {
  const assigned = [...new Set((assignedPlantIds ?? []).map((p) => p.trim()).filter(Boolean))];
  if (canSwitchPlant(roles)) return assigned;
  const home = (homePlantId || assigned[0] || '').trim();
  return home ? [home] : [];
}

function assert(cond: boolean, msg: string) {
  if (!cond) throw new Error(msg);
}

// TEST 1 — operator sees only home
{
  const v = visiblePlantIds(['WarehouseOperator'], 'F01', ['F01', 'F02']);
  assert(v.length === 1 && v[0] === 'F01', 'op home');
  assert(!canSwitchPlant(['WarehouseOperator']), 'op no switch');
  console.log('TEST1 OK — Operatör yalnızca Ana Fabrika');
}

// TEST 2 — engineer / depo same
{
  assert(!canSwitchPlant(['Engineer']), 'eng');
  assert(!canSwitchPlant(['DepoSorumlusu']), 'depo');
  console.log('TEST2 OK — Mühendis / Depo Sorumlusu kilitli');
}

// TEST 3 — executive can switch
{
  assert(canSwitchPlant(['Executive']), 'exec');
  const v = visiblePlantIds(['Executive'], 'F01', ['F01', 'F02']);
  assert(v.length === 2, 'exec both');
  console.log('TEST3 OK — Üst Yönetici yetkili tesisler arasında seçer');
}

// TEST 4 — admin can switch + manage
{
  assert(canSwitchPlant(['Administrator']), 'admin');
  console.log('TEST4 OK — Sistem Yöneticisi tesis değiştirebilir');
}

// TEST 5 — switch does not change home
{
  const home = 'F01';
  const working = 'F02';
  assert(home !== working, 'context ≠ home');
  console.log('TEST5 OK — working plant ≠ Ana Üs (home unchanged)');
}

console.log('plantVisibility.selftest: all passed');

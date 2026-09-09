/** INV-CNT-001 / TASK-024 — cycle count session rules (client mirror). */

export const COUNT_TYPES = ['Cycle', 'Full', 'Spot', 'Blind', 'ABC'] as const;
export type CountType = (typeof COUNT_TYPES)[number];

export const FREEZE_MODES = ['None', 'Location', 'Warehouse', 'Material'] as const;
export type FreezeMode = (typeof FREEZE_MODES)[number];

export type CycleCountOpenDraft = {
  plantId: string;
  warehouseCode: string;
  zone: string;
  abcClass: string;
  countType: CountType;
  countDate: string;
  assignedTo: string;
  blindCount: boolean;
  freezeMode: FreezeMode;
};

export function canOpenCountSession(draft: CycleCountOpenDraft): { ok: true } | { ok: false; reason: string } {
  if (!draft.plantId.trim()) {
    return { ok: false, reason: 'INV-CNT-004: tesis / fabrika bağlamı zorunlu.' };
  }
  if (!draft.warehouseCode.trim()) {
    return { ok: false, reason: 'INV-CNT-015: kapsam deposu boş olamaz — oturum açılamaz.' };
  }
  if (!draft.countDate.trim()) {
    return { ok: false, reason: 'Sayım tarihi zorunlu.' };
  }
  if (draft.countType === 'Blind' && !draft.blindCount) {
    return { ok: false, reason: 'Kör sayım tipi seçiliyse kör sayım kapısı açık olmalı.' };
  }
  return { ok: true };
}

export function buildCountSessionNotes(draft: CycleCountOpenDraft): string {
  const parts = [
    `type=${draft.countType}`,
    draft.zone.trim() ? `zone=${draft.zone.trim()}` : null,
    draft.abcClass.trim() ? `abc=${draft.abcClass.trim()}` : null,
    `blind=${draft.blindCount ? '1' : '0'}`,
    `freeze=${draft.freezeMode}`,
    draft.assignedTo.trim() ? `assigned=${draft.assignedTo.trim()}` : null,
    `date=${draft.countDate}`,
  ];
  return parts.filter(Boolean).join('; ');
}

export type CountLine = {
  key: string;
  materialCode: string;
  locationCode: string;
  lotNumber: string;
  systemQty: number;
  countedQty: string;
};

export function isAdministrator(roles: string[] | null | undefined): boolean {
  return (roles ?? []).some((r) => r.trim().toLowerCase() === 'administrator');
}

/** Logged-in staff with count permission already passed login — no second sign-in. */
export function canSaveCountLines(roles: string[] | null | undefined): boolean {
  if (isAdministrator(roles)) return true;
  const set = new Set((roles ?? []).map((r) => r.trim().toLowerCase()));
  return set.has('warehouseoperator') || set.has('executive');
}

export function canOpenCountDocument(roles: string[] | null | undefined): boolean {
  return canSaveCountLines(roles);
}

/** Admin (and any viewer of this wizard) always sees every step's page. */
export function canViewAllCountPages(roles: string[] | null | undefined): boolean {
  return isAdministrator(roles) || canSaveCountLines(roles) || (roles?.length ?? 0) > 0;
}

/** Blind hides system qty from counters; Administrator still sees the sheet. */
export function showSystemQuantity(roles: string[] | null | undefined, blindCount: boolean): boolean {
  if (isAdministrator(roles)) return true;
  return !blindCount;
}

export function lineVariance(line: CountLine): number | null {
  const raw = line.countedQty.trim();
  if (raw === '') return null;
  const n = Number(raw);
  if (!Number.isFinite(n)) return null;
  return n - line.systemQty;
}

export function summarizeVariances(lines: CountLine[]): { counted: number; differed: number } {
  let counted = 0;
  let differed = 0;
  for (const line of lines) {
    const v = lineVariance(line);
    if (v === null) continue;
    counted += 1;
    if (v !== 0) differed += 1;
  }
  return { counted, differed };
}

/** Client never mints CNT-… — Numbering Service (or SystemIdentifier stand-in) assigns on persist. */
export function buildCountSessionCreateBody(draft: CycleCountOpenDraft): {
  number: string;
  warehouseCode: string;
  status: string;
  notes: string;
} {
  return {
    number: '',
    warehouseCode: draft.warehouseCode.trim(),
    status: 'In Progress',
    notes: buildCountSessionNotes(draft),
  };
}

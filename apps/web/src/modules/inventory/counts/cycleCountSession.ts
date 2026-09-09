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

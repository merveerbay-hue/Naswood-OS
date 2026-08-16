/**
 * Master MaterialCode mint — user never types the code.
 * Format examples:
 *   HM-KR-PIN-001 · HM-KR-TPIN-001 · YM-LM-PIN-S-001 · YM-LM-TPIN-S-001
 *   MP-CP-AA-18-S · MP-TCP-AA-18-S · TW-CP-001
 *
 * Thermowood = same cins + ağaç, wood token prefixed with T (PIN→TPIN, CP→TCP).
 * No automatic SKU/variant generation.
 */

export type MainCategory = 'HM' | 'YM' | 'MP' | 'TW';

export type MaterialCodingInput = {
  mainCategory: MainCategory;
  /** HM: TR|KR|LT|LAMT · YM: LM|LAM|PR · MP ignored (from wood+quality) · TW: family */
  materialType: string;
  /**
   * Effective wood token in the code (already includes T-prefix when Thermowood).
   * PIN | TPIN | SP | TSP | CP | TCP | …
   */
  woodToken: string;
  /** YM lamel: S|FJ · MP: S|FJ · else empty */
  productTypeToken?: string;
  /** MP quality AA|AB|… */
  quality?: string;
  /** MP thickness in code (18) */
  thicknessForCode?: number | null;
  /** Existing material codes to compute next sequence */
  existingCodes: string[];
};

const HM_TYPES = new Set(['TR', 'KR', 'LT', 'LAMT']);
const YM_TYPES = new Set(['LM', 'LAM', 'PR']);
const MP_WOOD_BASE = new Set([
  'LP',
  'CP',
  'KP',
  'OP',
  'CV',
  'SP',
  'IR',
  'DB',
  'KS',
  'TK',
  'LM',
  'SD',
  'GK',
  'HS',
]);
const TW_FAMILY = new Set([
  'CP',
  'CA',
  'CI',
  'CY',
  'CAP',
  'ZP',
  'ZA',
  'ZI',
  'ZY',
  'ZAP',
  'SP',
  'SA',
  'SI',
  'SY',
  'SAP',
  'KP',
  'KA',
  'KI',
]);

export const WOOD_OPTIONS_HM: { token: string; label: string; labelTr: string }[] = [
  { token: 'PIN', label: 'Pine / Çam', labelTr: 'Çam' },
  { token: 'SP', label: 'Spruce / Ladin', labelTr: 'Ladin' },
  { token: 'FIR', label: 'Fir / Göknar', labelTr: 'Göknar' },
  { token: 'BEE', label: 'Beech / Kayın', labelTr: 'Kayın' },
  { token: 'OAK', label: 'Oak / Meşe', labelTr: 'Meşe' },
  { token: 'ASH', label: 'Ash / Dişbudak', labelTr: 'Dişbudak' },
  { token: 'WAL', label: 'Walnut / Ceviz', labelTr: 'Ceviz' },
  { token: 'CHE', label: 'Chestnut / Kestane', labelTr: 'Kestane' },
  { token: 'IRO', label: 'Iroko', labelTr: 'İroko' },
  { token: 'SAP', label: 'Sapelli', labelTr: 'Sapelli' },
  { token: 'AYO', label: 'Ayous', labelTr: 'Ayous' },
  { token: 'CAP', label: 'American Pine', labelTr: 'Amerikan Çam' },
  { token: 'TEK', label: 'Teak', labelTr: 'Teak' },
];

export const HM_TYPE_OPTIONS = [
  { token: 'TR', label: 'Tomruk' },
  { token: 'KR', label: 'Kereste' },
  { token: 'LT', label: 'Lata' },
  { token: 'LAMT', label: 'Lamel Taslak' },
];

export const YM_TYPE_OPTIONS = [
  { token: 'LM', label: 'Lamel' },
  { token: 'LAM', label: 'Lamine' },
  { token: 'PR', label: 'Profil Taslağı' },
];

export const MP_WOOD_OPTIONS = [
  { token: 'LP', label: 'Ladin', labelTr: 'Ladin' },
  { token: 'CP', label: 'Çam', labelTr: 'Çam' },
  { token: 'KP', label: 'Kayın', labelTr: 'Kayın' },
  { token: 'OP', label: 'Meşe', labelTr: 'Meşe' },
  { token: 'CV', label: 'Ceviz', labelTr: 'Ceviz' },
  { token: 'SP', label: 'Sapelli', labelTr: 'Sapelli' },
  { token: 'IR', label: 'İroko', labelTr: 'İroko' },
  { token: 'DB', label: 'Dişbudak', labelTr: 'Dişbudak' },
  { token: 'KS', label: 'Kestane', labelTr: 'Kestane' },
  { token: 'TK', label: 'Teak', labelTr: 'Teak' },
  { token: 'LM', label: 'Limba', labelTr: 'Limba' },
  { token: 'SD', label: 'Sedir', labelTr: 'Sedir' },
  { token: 'GK', label: 'Göknar', labelTr: 'Göknar' },
  { token: 'HS', label: 'Huş', labelTr: 'Huş' },
];

export const TW_FAMILY_OPTIONS = [
  { token: 'CP', label: 'Cephe / Pine' },
  { token: 'CA', label: 'Cephe / Ash' },
  { token: 'CI', label: 'Cephe / Iroko' },
  { token: 'CY', label: 'Cephe / Ayous' },
  { token: 'CAP', label: 'Cephe / American Pine' },
  { token: 'ZP', label: 'Zemin / Pine' },
  { token: 'ZA', label: 'Zemin / Ash' },
  { token: 'ZI', label: 'Zemin / Iroko' },
  { token: 'ZY', label: 'Zemin / Ayous' },
  { token: 'ZAP', label: 'Zemin / American Pine' },
  { token: 'SP', label: 'Sunshade / Pine' },
  { token: 'SA', label: 'Sunshade / Ash' },
  { token: 'SI', label: 'Sunshade / Iroko' },
  { token: 'SY', label: 'Sunshade / Ayous' },
  { token: 'SAP', label: 'Sunshade / American Pine' },
  { token: 'KP', label: 'Karkas / Pine' },
  { token: 'KA', label: 'Karkas / Ash' },
  { token: 'KI', label: 'Karkas / Iroko' },
];

function normToken(v: string): string {
  return String(v || '')
    .trim()
    .toUpperCase()
    .replace(/[^A-Z0-9]/g, '');
}

/** True when token already encodes Thermowood (TPIN, TCP, …). */
export function isThermowoodToken(woodToken: string): boolean {
  const w = normToken(woodToken);
  if (!w || w[0] !== 'T' || w.length < 2) return false;
  // T alone / TEK (teak) are not thermo prefixes
  if (w === 'TEK' || w === 'TK') return false;
  const base = stripThermowoodToken(w);
  return base !== w;
}

/** PIN ← TPIN, CP ← TCP. Leaves unknown tokens unchanged. */
export function stripThermowoodToken(woodToken: string): string {
  const w = normToken(woodToken);
  if (!w.startsWith('T') || w.length < 2) return w;
  if (w === 'TEK' || w === 'TK') return w;
  const rest = w.slice(1);
  const hmBases = new Set(WOOD_OPTIONS_HM.map((o) => o.token));
  if (hmBases.has(rest) || MP_WOOD_BASE.has(rest)) return rest;
  // TW family tokens are never T-prefixed this way
  return w;
}

/**
 * Effective code wood token.
 * isThermowood=true → PIN→TPIN, CP→TCP.
 * TW finished-goods category does not use this (family codes stay CP/CA/…).
 */
export function applyThermowoodToken(baseWoodToken: string, isThermowood: boolean): string {
  const base = stripThermowoodToken(baseWoodToken);
  if (!base) return '';
  if (!isThermowood) return base;
  if (isThermowoodToken(baseWoodToken)) return normToken(baseWoodToken);
  return `T${base}`;
}

function isAllowedMpWood(wood: string): boolean {
  const base = stripThermowoodToken(wood);
  return MP_WOOD_BASE.has(base);
}

function isAllowedHmYmWood(wood: string): boolean {
  return wood.length >= 2;
}

/** Prefix without sequence, e.g. HM-KR-TPIN or MP-TCP-AA-18-S or YM-LM-PIN-S */
export function buildCodePrefix(input: MaterialCodingInput): string | null {
  const cat = input.mainCategory;
  const type = normToken(input.materialType);
  const wood = normToken(input.woodToken);
  const pt = normToken(input.productTypeToken ?? '');
  const q = normToken(input.quality ?? '');

  if (cat === 'HM') {
    if (!HM_TYPES.has(type) || !isAllowedHmYmWood(wood)) return null;
    return `HM-${type}-${wood}`;
  }
  if (cat === 'YM') {
    if (!YM_TYPES.has(type) || !isAllowedHmYmWood(wood)) return null;
    if (type === 'LM') {
      if (pt !== 'S' && pt !== 'FJ') return null;
      return `YM-LM-${wood}-${pt}`;
    }
    return `YM-${type}-${wood}`;
  }
  if (cat === 'MP') {
    if (!isAllowedMpWood(wood) || !q) return null;
    if (pt !== 'S' && pt !== 'FJ') return null;
    const th = input.thicknessForCode;
    if (th == null || !(th > 0)) return null;
    return `MP-${wood}-${q}-${Math.round(th)}-${pt}`;
  }
  if (cat === 'TW') {
    if (!TW_FAMILY.has(wood)) return null;
    return `TW-${wood}`;
  }
  return null;
}

function nextSeq(prefix: string, existingCodes: string[]): number {
  const re = new RegExp(`^${prefix.replace(/[.*+?^${}()|[\]\\]/g, '\\$&')}-(\\d{3,})$`, 'i');
  let max = 0;
  for (const raw of existingCodes) {
    const c = String(raw || '').trim().toUpperCase();
    const m = c.match(re);
    if (m) max = Math.max(max, Number(m[1]));
  }
  return max + 1;
}

/** Mint next MaterialCode for prefix. */
export function mintMaterialCode(input: MaterialCodingInput): string | null {
  const prefix = buildCodePrefix(input);
  if (!prefix) return null;
  if (input.mainCategory === 'MP') {
    const code = prefix;
    const exists = input.existingCodes.some((c) => c.trim().toUpperCase() === code.toUpperCase());
    return exists ? null : code;
  }
  const seq = nextSeq(prefix, input.existingCodes);
  return `${prefix}-${String(seq).padStart(3, '0')}`;
}

/** Preview without allocating. */
export function previewMaterialCode(input: MaterialCodingInput): string {
  const prefix = buildCodePrefix(input);
  if (!prefix) return '—';
  if (input.mainCategory === 'MP') return prefix;
  const seq = nextSeq(prefix, input.existingCodes);
  return `${prefix}-${String(seq).padStart(3, '0')}`;
}

function woodLabelTr(baseToken: string): string {
  const base = stripThermowoodToken(baseToken);
  const hm = WOOD_OPTIONS_HM.find((o) => o.token === base);
  if (hm) return hm.labelTr;
  const mp = MP_WOOD_OPTIONS.find((o) => o.token === base);
  if (mp) return mp.labelTr;
  return base;
}

function typeLabelTr(mainCategory: MainCategory, materialType: string): string {
  if (mainCategory === 'MP') return 'Masif Panel';
  if (mainCategory === 'TW') return 'Thermowood';
  const all = [...HM_TYPE_OPTIONS, ...YM_TYPE_OPTIONS];
  return all.find((o) => o.token === normToken(materialType))?.label ?? materialType;
}

/**
 * Suggested material name — Thermowood is visible in the name itself.
 * Examples: "Çam Kereste" · "Thermowood Çam Kereste" · "Çam Solid Lamel" · "Thermowood Çam Masif Panel AA"
 */
export function suggestMaterialName(input: {
  mainCategory: MainCategory;
  materialType: string;
  /** Base wood (PIN/CP) — thermo flag applied separately */
  baseWoodToken: string;
  isThermowood: boolean;
  productTypeToken?: string;
  quality?: string;
}): string {
  if (input.mainCategory === 'TW') {
    const fam = TW_FAMILY_OPTIONS.find((o) => o.token === normToken(input.baseWoodToken));
    return fam ? `Thermowood ${fam.label}` : 'Thermowood';
  }

  const wood = woodLabelTr(input.baseWoodToken);
  const kind = typeLabelTr(input.mainCategory, input.materialType);
  const pt = normToken(input.productTypeToken ?? '');
  const q = normToken(input.quality ?? '');

  let core = `${wood} ${kind}`;
  if (input.mainCategory === 'YM' && normToken(input.materialType) === 'LM') {
    const ptLabel = pt === 'FJ' ? 'Finger Joint' : pt === 'S' ? 'Solid' : '';
    core = ptLabel ? `${wood} ${ptLabel} ${kind}` : `${wood} ${kind}`;
  }
  if (input.mainCategory === 'MP') {
    core = q ? `${wood} Masif Panel ${q}` : `${wood} Masif Panel`;
    if (pt === 'FJ') core = `${core} FJ`;
    else if (pt === 'S') core = `${core} Solid`;
  }

  if (input.isThermowood) return `Thermowood ${core}`;
  return core;
}
